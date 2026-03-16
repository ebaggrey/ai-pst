using Chapter_3.Models.Domain;
using Chapter_3.Models.Exceptions;
using Chapter_3.Models.Requests;
using Chapter_3.Models.Responses;
using Chapter_3.Models.Supporting;
using Chapter_3.Services.Interfaces;
using Chapter_3.Settings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Chapter_3.Services
{
    // Services/Implementations/HumanReviewOrchestrator.cs
    public class HumanReviewOrchestrator : IHumanReviewOrchestrator
    {
        private readonly ILLMServiceFactory _llmFactory;
        private readonly IReviewSessionStore _sessionStore;
        private readonly IJudgmentAnalyzer _judgmentAnalyzer;
        private readonly ILogger<HumanReviewOrchestrator> _logger;
        private readonly HumanLoopSettings _settings;

        public HumanReviewOrchestrator(
            ILLMServiceFactory llmFactory,
            IReviewSessionStore sessionStore,
            IJudgmentAnalyzer judgmentAnalyzer,
            ILogger<HumanReviewOrchestrator> logger,
            IOptions<HumanLoopSettings> settings)
        {
            _llmFactory = llmFactory;
            _sessionStore = sessionStore;
            _judgmentAnalyzer = judgmentAnalyzer;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<ReviewSession> CreateReviewSessionAsync(ReviewRequest request)
        {
            _logger.LogInformation("Creating review session for {TestType}", request.GeneratedContent.TestType);

            var analysis = await AnalyzeForHumanReviewAsync(request.GeneratedContent);

            var session = new ReviewSession
            {
                Id = GenerateSessionId(),
                OriginalContent = request.GeneratedContent,
                CurrentContent = request.GeneratedContent.DeepClone(),
                Context = request.Context,
                Analysis = analysis,
                CreatedAt = DateTime.UtcNow,
                Status = ReviewSessionStatus.AwaitingReview,
                AiConfidenceStatement = GenerateConfidenceStatement(analysis),
                AreasNeedingHumanJudgment = IdentifyHumanJudgmentAreas(analysis),
                SuggestedReviewFocus = DetermineReviewFocus(analysis, request.Context)
            };

            session.InitialQuestions = await GenerateInitialQuestionsAsync(session);
            await _sessionStore.StoreSessionAsync(session);

            _logger.LogDebug("Session {SessionId} created with {QuestionCount} initial questions",
                session.Id, session.InitialQuestions.Length);

            return session;
        }

        public async Task<AiEditAnalysis> AnalyzeHumanEditAsync(ReviewSession session, CollaborativeEditRequest request)
        {
            var llmService = _llmFactory.GetServiceForTask(
                task: "analyze-human-edit",
                strategy: "understanding-intent",
                context: "Review session edits");

            var prompt = BuildEditAnalysisPrompt(session, request);
            var analysisJson = await llmService.GenerateTestCodeAsync(prompt, "Analyze this human edit");

            var analysis = JsonSerializer.Deserialize<AiEditAnalysis>(analysisJson) ?? new AiEditAnalysis();
            analysis.AnalyzedAt = DateTime.UtcNow;
            analysis.EditImpact = await CalculateEditImpactAsync(session, request, analysis);
            analysis.PotentialIssues = await IdentifyPotentialIssuesAsync(session, request, analysis);

            return analysis;
        }

        public async Task<AiClarification> GenerateClarificationAsync(ReviewSession session, ClarificationRequest request, QuestionAnalysis questionAnalysis)
        {
            var llmService = questionAnalysis.QuestionType switch
            {
                "technical" => _llmFactory.GetService("deepseek"),
                "business" => _llmFactory.GetService("claude"),
                "testing-methodology" => _llmFactory.GetService("chatgpt"),
                "edge-case" => _llmFactory.GetService("gemini"),
                _ => _llmFactory.GetService("chatgpt")
            };

            var prompt = BuildClarificationPrompt(session, request, questionAnalysis);

            try
            {
                var response = await llmService.GenerateTestCodeAsync(prompt, "Answer the human's question");

                return new AiClarification
                {
                    DirectAnswer = ExtractDirectAnswer(response),
                    Alternatives = ExtractAlternativeAnswers(response),
                    Confidence = CalculateAnswerConfidence(response, questionAnalysis),
                    Assumptions = ExtractAssumptions(response),
                    RecommendedAction = ExtractRecommendedAction(response),
                    WhenToAskHuman = DetermineHumanEscalation(response, questionAnalysis)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate clarification for session {SessionId}", session.Id);

                return new AiClarification
                {
                    DirectAnswer = "I need to think about this more. Could you provide additional context?",
                    Alternatives = new[] { "Check the documentation", "Consult with a team member" },
                    Confidence = 0.3,
                    Assumptions = new[] { "More context would help" },
                    RecommendedAction = "Provide more specific examples or constraints"
                };
            }
        }

        public async Task<QuestionAnalysis> AnalyzeHumanQuestionAsync(ClarificationRequest request)
        {
            var llmService = _llmFactory.GetService("claude");
            var prompt = $@"
        Analyze this question from a human reviewer:
        
        QUESTION: {request.HumanQuestion}
        
        CONTEXT TAGS: {string.Join(", ", request.ContextTags)}
        
        Analyze:
        1. What type of question is this?
        2. Is it ambiguous? If so, what are the possible interpretations?
        3. What key topics does it cover?
        4. What underlying need might the human have?
        5. What context might be missing?
        ";

            var analysisJson = await llmService.GenerateTestCodeAsync(prompt, "Analyze human question");
            return JsonSerializer.Deserialize<QuestionAnalysis>(analysisJson) ?? new QuestionAnalysis();
        }

        public async Task LearnFromJudgmentAsync(JudgmentRequest request)
        {
            var learning = await _judgmentAnalyzer.ExtractLearningPointsAsync(request);
            await UpdateRelevantModelAsync(learning);
            await StoreJudgmentForTrainingAsync(request);

            _logger.LogInformation("Learned from human judgment: {KeyLearning}",
                learning.FirstOrDefault()?.Description ?? "No key learning");
        }

        private string GenerateSessionId() => $"hr_{Guid.NewGuid().ToString("N")[..16]}";

        private async Task<ReviewAnalysis> AnalyzeForHumanReviewAsync(GeneratedTest test)
        {
            var llmService = _llmFactory.GetService("claude");

            var prompt = $@"
        Analyze this generated test for human review needs:
        
        TEST:
        {test.Content}
        
        METADATA:
        Confidence: {test.ConfidenceScore}
        Test Type: {test.TestType}
        Framework: {test.Framework}
        
        Analyze:
        1. What parts definitely need human review?
        2. What parts are likely correct?
        3. What edge cases might be missing?
        4. What business context might change this test?
        5. What could make this test flaky?
        
        Return JSON with analysis.
        ";

            var analysisJson = await llmService.GenerateTestCodeAsync(prompt, "Analyze for human review");
            return JsonSerializer.Deserialize<ReviewAnalysis>(analysisJson) ?? new ReviewAnalysis();
        }

        private string GenerateConfidenceStatement(ReviewAnalysis analysis)
        {
            if (analysis.OverallConfidence > 0.8)
                return "I'm quite confident in this test, but please review the highlighted areas.";
            else if (analysis.OverallConfidence > 0.6)
                return "I'm moderately confident, but human review is recommended for several areas.";
            else
                return "I have low confidence in this test and recommend thorough human review.";
        }

        private JudgmentArea[] IdentifyHumanJudgmentAreas(ReviewAnalysis analysis)
        {
            var areas = new List<JudgmentArea>();

            if (analysis.NeedsHumanReview.Any())
            {
                areas.Add(new JudgmentArea
                {
                    Area = "Critical Review Needed",
                    Description = "These parts require careful human review",
                    WhyHumanNeeded = "AI confidence is low in these areas",
                    Guidance = "Check for logical errors, edge cases, and business logic alignment"
                });
            }

            if (analysis.MissingEdgeCases.Any())
            {
                areas.Add(new JudgmentArea
                {
                    Area = "Missing Edge Cases",
                    Description = "Potential edge cases that might be missing",
                    WhyHumanNeeded = "AI may not have considered all edge cases",
                    Guidance = "Consider additional test scenarios and boundary conditions"
                });
            }

            if (analysis.BusinessContextConcerns.Any())
            {
                areas.Add(new JudgmentArea
                {
                    Area = "Business Context",
                    Description = "Areas where business knowledge is needed",
                    WhyHumanNeeded = "AI lacks specific business context",
                    Guidance = "Verify alignment with business rules and requirements"
                });
            }

            return areas.ToArray();
        }

        private string[] DetermineReviewFocus(ReviewAnalysis analysis, ReviewContext context)
        {
            var focusAreas = new List<string>();

            if (context.RiskLevel == "high" || context.RiskLevel == "critical")
                focusAreas.Add("Security and data integrity");

            if (analysis.FlakinessRisks.Any())
                focusAreas.Add("Test stability and reliability");

            if (analysis.MissingEdgeCases.Any())
                focusAreas.Add("Edge case coverage");

            return focusAreas.ToArray();
        }

        private async Task<InitialQuestion[]> GenerateInitialQuestionsAsync(ReviewSession session)
        {
            var llmService = _llmFactory.GetService("chatgpt");
            var prompt = $@"
        Generate initial questions to guide human review of this test:
        
        TEST CONTEXT:
        Purpose: {session.Context.TestPurpose}
        Risk Level: {session.Context.RiskLevel}
        Test Type: {session.GeneratedContent.TestType}
        
        ANALYSIS:
        {string.Join("\n", session.Analysis.NeedsHumanReview)}
        
        Generate 3-5 questions to help the reviewer focus on the most important aspects.
        ";

            var questionsJson = await llmService.GenerateTestCodeAsync(prompt, "Generate review questions");
            return JsonSerializer.Deserialize<InitialQuestion[]>(questionsJson) ?? Array.Empty<InitialQuestion>();
        }

        private string BuildEditAnalysisPrompt(ReviewSession session, CollaborativeEditRequest request)
        {
            return $@"
        Analyze this human edit to a test during review:
        
        ORIGINAL TEST:
        {session.OriginalContent.Content}
        
        HUMAN EDIT:
        {request.UserEdit.Content}
        
        EDIT INTENT (from human): {request.UserEdit.Intent}
        
        CONTEXT:
        Test Purpose: {session.Context.TestPurpose}
        Risk Level: {session.Context.RiskLevel}
        
        Analyze:
        1. Does the edit align with the intent?
        2. Does it improve the test? How?
        3. Does it introduce any issues?
        4. What would you suggest differently?
        5. Should the AI learn from this edit?
        
        Be constructive and specific.
        ";
        }

        private async Task<EditImpact> CalculateEditImpactAsync(ReviewSession session, CollaborativeEditRequest request, AiEditAnalysis analysis)
        {
            var llmService = _llmFactory.GetService("deepseek");
            var prompt = $@"
        Calculate the impact of this edit:
        
        EDIT: {request.UserEdit.Content}
        INTENT: {request.UserEdit.Intent}
        ANALYSIS: {string.Join(", ", analysis.Improvements)}
        
        Determine impact level and specific areas affected.
        ";

            var impactJson = await llmService.GenerateTestCodeAsync(prompt, "Calculate edit impact");
            return JsonSerializer.Deserialize<EditImpact>(impactJson) ?? new EditImpact();
        }

        private async Task<string[]> IdentifyPotentialIssuesAsync(ReviewSession session, CollaborativeEditRequest request, AiEditAnalysis analysis)
        {
            var llmService = _llmFactory.GetService("deepseek");
            var prompt = $@"
        Identify potential issues with this edit:
        
        ORIGINAL: {session.OriginalContent.Content}
        EDIT: {request.UserEdit.Content}
        TEST TYPE: {session.GeneratedContent.TestType}
        
        Look for:
        1. Syntax errors
        2. Logical issues
        3. Performance impacts
        4. Security concerns
        ";

            var issuesJson = await llmService.GenerateTestCodeAsync(prompt, "Identify potential issues");
            return JsonSerializer.Deserialize<string[]>(issuesJson) ?? Array.Empty<string>();
        }

        private string BuildClarificationPrompt(ReviewSession session, ClarificationRequest request, QuestionAnalysis questionAnalysis)
        {
            return $@"
        Answer this human question about a test review:
        
        QUESTION: {request.HumanQuestion}
        
        QUESTION ANALYSIS:
        Type: {questionAnalysis.QuestionType}
        Topics: {string.Join(", ", questionAnalysis.KeyTopics)}
        
        TEST CONTEXT:
        Purpose: {session.Context.TestPurpose}
        Risk Level: {session.Context.RiskLevel}
        Test Type: {session.GeneratedContent.TestType}
        
        CURRENT TEST CONTENT:
        {session.CurrentContent.Content}
        
        Provide a helpful, specific answer.
        ";
        }

        private string ExtractDirectAnswer(string llmResponse)
        {
            var lines = llmResponse.Split('\n');
            return lines.FirstOrDefault(l => !string.IsNullOrWhiteSpace(l)) ?? "I don't have a specific answer.";
        }

        private string[] ExtractAlternativeAnswers(string llmResponse)
        {
            var alternatives = new List<string>();
            var lines = llmResponse.Split('\n');

            foreach (var line in lines)
            {
                if (line.StartsWith("Alternative:") || line.StartsWith("Or:"))
                    alternatives.Add(line.Substring(line.IndexOf(':') + 1).Trim());
            }

            return alternatives.ToArray();
        }

        private double CalculateAnswerConfidence(string response, QuestionAnalysis questionAnalysis)
        {
            var confidence = 0.7;

            if (questionAnalysis.IsAmbiguous)
                confidence -= 0.3;

            if (response.Length < 50)
                confidence -= 0.2;

            if (response.Contains("I'm not sure") || response.Contains("I don't know"))
                confidence -= 0.4;

            return Math.Max(0.1, Math.Min(1.0, confidence));
        }

        private string[] ExtractAssumptions(string response)
        {
            var assumptions = new List<string>();
            var lines = response.Split('\n');

            foreach (var line in lines)
            {
                if (line.StartsWith("Assuming") || line.StartsWith("If"))
                    assumptions.Add(line.Trim());
            }

            return assumptions.ToArray();
        }

        private string ExtractRecommendedAction(string response)
        {
            var lines = response.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("Recommend") || line.StartsWith("Suggest") || line.Contains("you should"))
                    return line.Trim();
            }

            return "Review the test carefully and apply your judgment.";
        }

        private string DetermineHumanEscalation(string response, QuestionAnalysis questionAnalysis)
        {
            if (questionAnalysis.ClarityScore < 0.5)
                return "When the question remains unclear after clarification attempts";

            if (response.Contains("complex business rule") || response.Contains("legal requirement"))
                return "When business rules or legal requirements are involved";

            if (response.Contains("subjective") || response.Contains("opinion"))
                return "When the answer involves subjective judgment";

            return "When technical details are unclear or contradictory";
        }

        private async Task UpdateRelevantModelAsync(LearningPoint[] learningPoints)
        {
            _logger.LogInformation("Updating models with {Count} learning points", learningPoints.Length);
            await Task.Delay(100);
        }

        private async Task StoreJudgmentForTrainingAsync(JudgmentRequest request)
        {
            _logger.LogInformation("Storing judgment for future training");
            await Task.Delay(100);
        }
    }

    // Services/Implementations/CollaborationSessionManager.cs
    public class CollaborationSessionManager : ICollaborationSessionManager
    {
        private readonly ConcurrentDictionary<string, ReviewSession> _activeSessions = new();
        private readonly ILogger<CollaborationSessionManager> _logger;
        private readonly CollaborationSettings _settings;
        private readonly ISuggestionEngine _suggestionEngine;

        public CollaborationSessionManager(
            ILogger<CollaborationSessionManager> logger,
            IOptions<CollaborationSettings> settings,
            ISuggestionEngine suggestionEngine)
        {
            _logger = logger;
            _settings = settings.Value;
            _suggestionEngine = suggestionEngine;
        }

        public Task<ReviewSession> GetSessionAsync(string sessionId)
        {
            if (_activeSessions.TryGetValue(sessionId, out var session))
                return Task.FromResult(session);

            throw new SessionNotFoundException(sessionId);
        }

        public async Task<ReviewSession> ApplyHumanEditAsync(string sessionId, CollaborativeEditRequest request)
        {
            if (!_activeSessions.TryGetValue(sessionId, out var session))
                throw new SessionNotFoundException($"Session {sessionId} not found");

            var validationResult = await _suggestionEngine.ValidateEditAsync(session.CurrentContent, request.UserEdit);
            if (!validationResult.IsValid)
                throw new InvalidEditException(request.UserEdit.EditorId, validationResult.ErrorMessage);

            var updatedContent = ApplyEditToContent(session.CurrentContent, request.UserEdit);

            session.CurrentContent = updatedContent;
            session.EditHistory = session.EditHistory.Append(new EditRecord
            {
                Edit = request.UserEdit,
                AppliedAt = DateTime.UtcNow,
                AppliedBy = request.UserEdit.EditorId,
                Impact = validationResult.Impact
            }).ToArray();

            session.LastModified = DateTime.UtcNow;

            if (ShouldSuggestRelatedEdits(request, validationResult))
            {
                session.AiSuggestions = await GenerateRelatedSuggestionsAsync(session, request);
            }

            _logger.LogDebug(
                "Applied edit to session {SessionId}. Edit history now {EditCount} items",
                sessionId, session.EditHistory.Length);

            return session;
        }

        public async Task<ClarificationThread> AddClarificationRoundAsync(
            string sessionId,
            ClarificationRequest request,
            ClarificationResponse response)
        {
            var session = await GetSessionAsync(sessionId);

            var thread = session.ClarificationThreads.LastOrDefault();
            if (thread == null)
            {
                thread = new ClarificationThread
                {
                    ThreadId = Guid.NewGuid().ToString(),
                    StartedAt = DateTime.UtcNow
                };
                session.ClarificationThreads = session.ClarificationThreads.Append(thread).ToArray();
            }

            var round = new ClarificationRound
            {
                RoundId = response.RoundId,
                HumanQuestion = request.HumanQuestion,
                AiResponse = response.AiAnswer,
                AskedAt = DateTime.UtcNow,
                QuestionType = request.QuestionType
            };

            thread.Rounds = thread.Rounds.Append(round).ToArray();
            thread.LastActivity = DateTime.UtcNow;

            if (response.RelevanceScore > 0.8 && response.AiAnswer.Confidence > 0.7)
            {
                thread.Resolved = true;
                thread.ResolvedAt = DateTime.UtcNow;
            }

            return thread;
        }

        public Task CloseSessionAsync(string sessionId, ReviewOutcome outcome)
        {
            if (!_activeSessions.TryGetValue(sessionId, out var session))
                throw new SessionNotFoundException($"Session {sessionId} not found");

            session.Status = ReviewSessionStatus.Closed;
            session.ClosedAt = DateTime.UtcNow;
            session.Outcome = outcome;
            session.Summary = GenerateSessionSummary(session);

            ArchiveSession(session);
            _activeSessions.TryRemove(sessionId, out _);

            _logger.LogInformation(
                "Closed session {SessionId} with outcome: {DecisionSummary}",
                sessionId, outcome.DecisionSummary);

            return Task.CompletedTask;
        }

        private GeneratedTest ApplyEditToContent(GeneratedTest currentContent, UserEdit edit)
        {
            var newTest = currentContent.DeepClone();
            newTest.Content = edit.Content;
            newTest.GeneratedAt = DateTime.UtcNow;
            return newTest;


        }

        private bool ShouldSuggestRelatedEdits(CollaborativeEditRequest request, EditValidationResult validationResult)
        {
            return validationResult.Impact.ImpactLevel == "high" &&
                   request.RequestAiAnalysis &&
                   _settings.Tools.Suggestions.Enabled;
        }

        private async Task<AiSuggestion[]> GenerateRelatedSuggestionsAsync(ReviewSession session, CollaborativeEditRequest request)
        {
            try
            {
                var suggestions = await _suggestionEngine.GenerateSuggestionsAsync(session, request.EditContext);
                return suggestions.Select(s => new AiSuggestion
                {
                    Suggestion = s.Suggestion,
                    Type = s.Type,
                    Confidence = s.Confidence,
                    Reasoning = s.Reasoning,
                    GeneratedAt = DateTime.UtcNow,
                    AutoApply = s.Confidence > 0.8 && _settings.Tools.Suggestions.AutoApply
                }).ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate related suggestions");
                return Array.Empty<AiSuggestion>();
            }
        }

        private SessionSummary GenerateSessionSummary(ReviewSession session)
        {
            return new SessionSummary
            {
                SessionId = session.Id,
                CreatedAt = session.CreatedAt,
                ClosedAt = session.ClosedAt,
                Duration = (session.ClosedAt ?? DateTime.UtcNow) - session.CreatedAt,
                EditCount = session.EditHistory.Length,
                ClarificationCount = session.ClarificationThreads.Sum(t => t.Rounds.Length),
                FinalDecision = session.Outcome?.Decision ?? "unknown",
                KeyInsights = ExtractKeyInsights(session),
                Metrics = GenerateMetrics(session)
            };
        }

        private string[] ExtractKeyInsights(ReviewSession session)
        {
            var insights = new List<string>();

            if (session.EditHistory.Any())
                insights.Add($"Applied {session.EditHistory.Length} edits during review");

            if (session.ClarificationThreads.Any())
                insights.Add($"Had {session.ClarificationThreads.Length} clarification threads");

            if (session.AiSuggestions.Any())
                insights.Add($"Generated {session.AiSuggestions.Length} AI suggestions");

            return insights.ToArray();
        }

        private Dictionary<string, object> GenerateMetrics(ReviewSession session)
        {
            return new Dictionary<string, object>
            {
                ["edits_applied"] = session.EditHistory.Length,
                ["clarification_rounds"] = session.ClarificationThreads.Sum(t => t.Rounds.Length),
                ["ai_suggestions"] = session.AiSuggestions.Length,
                ["suggestions_accepted"] = session.Outcome?.AcceptedSuggestions?.Length ?? 0,
                ["session_duration_minutes"] = ((session.ClosedAt ?? DateTime.UtcNow) - session.CreatedAt).TotalMinutes
            };
        }

        private void ArchiveSession(ReviewSession session)
        {
            _logger.LogInformation("Archiving session {SessionId}", session.Id);
        }
    }

    // Services/Implementations/JudgmentProcessor.cs
    public class JudgmentProcessor : IJudgmentProcessor
    {
        private readonly ILogger<JudgmentProcessor> _logger;

        public JudgmentProcessor(ILogger<JudgmentProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<ReviewOutcome> ProcessJudgmentAsync(ReviewSession session, JudgmentRequest request)
        {
            _logger.LogInformation("Processing judgment for session {SessionId}", session.Id);

            var outcome = new ReviewOutcome
            {
                Decision = request.Judgment.Decision,
                DecisionSummary = request.Judgment.Reasoning,
                AppliedEdits = session.EditHistory.Select(e => e.Edit.Intent).ToArray(),
                AcceptedSuggestions = session.AiSuggestions
                    .Where(s => request.Judgment.SuggestedImprovements.Contains(s.Suggestion))
                    .Select(s => s.Suggestion)
                    .ToArray(),
                DecidedAt = DateTime.UtcNow,
                DecidedBy = "human_reviewer",
                Metadata = new Dictionary<string, object>
                {
                    ["confidence"] = request.Judgment.ConfidenceInJudgment,
                    ["areas_reviewed"] = request.AreasReviewed
                }
            };

            await Task.Delay(100);
            return outcome;
        }

        public async Task<ReviewInsight[]> ExtractInsightsAsync(ReviewSession session, JudgmentRequest request, ReviewOutcome outcome)
        {
            var insights = new List<ReviewInsight>();

            if (session.EditHistory.Any())
            {
                insights.Add(new ReviewInsight
                {
                    Insight = "Human edits improved test quality",
                    Category = "quality_improvement",
                    Impact = "positive",
                    Evidence = session.EditHistory.Select(e => e.Edit.Intent).ToArray(),
                    Actionable = true
                });
            }

            if (!string.IsNullOrEmpty(request.FeedbackForAi))
            {
                insights.Add(new ReviewInsight
                {
                    Insight = "Human provided specific feedback for AI improvement",
                    Category = "learning_opportunity",
                    Impact = "educational",
                    Evidence = new[] { request.FeedbackForAi },
                    Actionable = true
                });
            }

            if (request.Judgment.AreasOfConcern.Any())
            {
                insights.Add(new ReviewInsight
                {
                    Insight = "Identified areas of concern in generated test",
                    Category = "risk_identification",
                    Impact = "risk_mitigation",
                    Evidence = request.Judgment.AreasOfConcern,
                    Actionable = true
                });
            }

            await Task.Delay(50);
            return insights.ToArray();
        }
    }

    // Services/Implementations/LearningFocusedJudgmentAnalyzer.cs
    public class LearningFocusedJudgmentAnalyzer : IJudgmentAnalyzer
    {
        private readonly ILogger<LearningFocusedJudgmentAnalyzer> _logger;

        public LearningFocusedJudgmentAnalyzer(ILogger<LearningFocusedJudgmentAnalyzer> logger)
        {
            _logger = logger;
        }

        public async Task<LearningPoint[]> ExtractLearningPointsAsync(JudgmentRequest request)
        {
            _logger.LogInformation("Extracting learning points from judgment");

            var learningPoints = new List<LearningPoint>();

            if (request.Judgment.SuggestedImprovements.Any())
            {
                learningPoints.Add(new LearningPoint
                {
                    Category = "test_improvement",
                    Description = "Human suggested improvements for test generation",
                    Impact = "improves test quality",
                    Examples = request.Judgment.SuggestedImprovements,
                    AppliedToModel = false
                });
            }

            if (request.Judgment.AreasOfConcern.Any())
            {
                learningPoints.Add(new LearningPoint
                {
                    Category = "risk_areas",
                    Description = "Areas where AI needs improvement based on human feedback",
                    Impact = "reduces risk",
                    Examples = request.Judgment.AreasOfConcern,
                    AppliedToModel = false
                });
            }

            if (!string.IsNullOrEmpty(request.Judgment.Reasoning))
            {
                learningPoints.Add(new LearningPoint
                {
                    Category = "decision_pattern",
                    Description = "Human decision-making pattern for test approval",
                    Impact = "improves decision modeling",
                    Examples = new[] { request.Judgment.Reasoning },
                    AppliedToModel = false
                });
            }

            await Task.Delay(100);
            return learningPoints.ToArray();
        }

        public async Task<ModelUpdateSummary[]> AnalyzeForModelUpdatesAsync(JudgmentRequest request)
        {
            _logger.LogInformation("Analyzing judgment for model updates");

            var updates = new List<ModelUpdateSummary>();

            if (request.Judgment.SuggestedImprovements.Any())
            {
                updates.Add(new ModelUpdateSummary
                {
                    ModelName = "TestGenerationModel",
                    UpdateType = "quality_improvement",
                    AreasUpdated = new[] { "test_content_generation", "edge_case_detection" },
                    ConfidenceImpact = 0.1,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (request.Judgment.Decision == "request-revision")
            {
                updates.Add(new ModelUpdateSummary
                {
                    ModelName = "ConfidenceCalibrationModel",
                    UpdateType = "confidence_adjustment",
                    AreasUpdated = new[] { "confidence_scoring", "review_needs_prediction" },
                    ConfidenceImpact = -0.05,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await Task.Delay(100);
            return updates.ToArray();
        }
    }

    // Services/Implementations/InMemorySessionStore.cs
    public class InMemorySessionStore : IReviewSessionStore
    {
        private readonly ConcurrentDictionary<string, ReviewSession> _sessions = new();
        private readonly ILogger<InMemorySessionStore> _logger;

        public InMemorySessionStore(ILogger<InMemorySessionStore> logger)
        {
            _logger = logger;
        }

        public Task<ReviewSession> GetSessionAsync(string sessionId)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
                return Task.FromResult(session);

            return Task.FromResult<ReviewSession>(null);
        }

        public Task StoreSessionAsync(ReviewSession session)
        {
            _sessions[session.Id] = session;
            _logger.LogDebug("Stored session {SessionId}", session.Id);
            return Task.CompletedTask;
        }

        public Task UpdateSessionAsync(ReviewSession session)
        {
            if (_sessions.ContainsKey(session.Id))
            {
                _sessions[session.Id] = session;
                _logger.LogDebug("Updated session {SessionId}", session.Id);
            }
            return Task.CompletedTask;
        }

        public Task DeleteSessionAsync(string sessionId)
        {
            _sessions.TryRemove(sessionId, out _);
            _logger.LogDebug("Deleted session {SessionId}", sessionId);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ReviewSession>> GetSessionsByStatusAsync(ReviewSessionStatus status)
        {
            var sessions = _sessions.Values.Where(s => s.Status == status);
            return Task.FromResult(sessions);
        }
    }

    // Services/Implementations/LLMServiceFactory.cs
    public class LLMServiceFactory : ILLMServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LLMServiceFactory> _logger;

        public LLMServiceFactory(IServiceProvider serviceProvider, ILogger<LLMServiceFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public ILLMService GetService(string serviceName)
        {
            _logger.LogDebug("Getting LLM service: {ServiceName}", serviceName);

            return serviceName.ToLower() switch
            {
                "chatgpt" => _serviceProvider.GetService<DialogueOptimizedChatGPTService>()!,
                "claude" => _serviceProvider.GetService<ClarificationSpecialistClaudeService>()!,
                "deepseek" => _serviceProvider.GetService<EditAnalysisDeepSeekService>()!,
                "gemini" => _serviceProvider.GetService<ILLMService>()!, // Placeholder
                _ => _serviceProvider.GetService<DialogueOptimizedChatGPTService>()!
            };
        }

        public ILLMService GetServiceForTask(string task, string strategy, string context)
        {
            _logger.LogDebug("Getting LLM service for task: {Task}, strategy: {Strategy}", task, strategy);

            return task switch
            {
                "analyze-human-edit" => GetService("deepseek"),
                "generate-clarification" => GetService("claude"),
                "answer-question" => GetService("chatgpt"),
                _ => GetService("chatgpt")
            };
        }
    }

    // Services/Implementations/DialogueOptimizedChatGPTService.cs
    public class DialogueOptimizedChatGPTService : ILLMService
    {
        private readonly ILogger<DialogueOptimizedChatGPTService> _logger;

        public DialogueOptimizedChatGPTService(ILogger<DialogueOptimizedChatGPTService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GenerateTestCodeAsync(string prompt, string context)
        {
            _logger.LogInformation("Generating test code with context: {Context}", context);

            await Task.Delay(200);

            return $$"""
        {
            "test": "// Generated test code based on prompt",
            "confidence": 0.85,
            "explanation": "Generated based on {{context}}",
            "areas_to_review": ["edge cases", "business logic"]
        }
        """;
        }

        public async Task<string> AnalyzeContentAsync(string content, string analysisType)
        {
            _logger.LogInformation("Analyzing content for {AnalysisType}", analysisType);

            await Task.Delay(150);

            return $$"""
        {
            "analysis_type": "{{analysisType}}",
            "findings": ["Content appears reasonable", "Some areas need human review"],
            "confidence": 0.75,
            "recommendations": ["Review edge cases", "Verify business logic"]
        }
        """;
        }

        public async Task<string> AnswerQuestionAsync(string question, string context)
        {
            _logger.LogInformation("Answering question with context: {Context}", context);

            await Task.Delay(100);

            return "Based on the provided context, the answer would be: [Generated answer]. However, please verify with human judgment for critical applications.";
        }
    }

    // Services/Implementations/ClarificationSpecialistClaudeService.cs
    public class ClarificationSpecialistClaudeService : ILLMService
    {
        private readonly ILogger<ClarificationSpecialistClaudeService> _logger;

        public ClarificationSpecialistClaudeService(ILogger<ClarificationSpecialistClaudeService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GenerateTestCodeAsync(string prompt, string context)
        {
            _logger.LogInformation("Claude generating test code for: {Context}", context);
            await Task.Delay(250);
            return "{\"test\": \"// Claude-generated test with careful consideration\", \"confidence\": 0.88}";
        }

        public async Task<string> AnalyzeContentAsync(string content, string analysisType)
        {
            _logger.LogInformation("Claude analyzing content: {AnalysisType}", analysisType);
            await Task.Delay(200);
            return "{\"analysis\": \"Claude provides thoughtful analysis with considerations for edge cases.\"}";
        }

        public async Task<string> AnswerQuestionAsync(string question, string context)
        {
            _logger.LogInformation("Claude answering question: {Question}", question);
            await Task.Delay(150);
            return "Claude's answer: I've considered your question carefully. The key points are... [detailed answer]";
        }
    }

    // Services/Implementations/EditAnalysisDeepSeekService.cs
    public class EditAnalysisDeepSeekService : ILLMService
    {
        private readonly ILogger<EditAnalysisDeepSeekService> _logger;

        public EditAnalysisDeepSeekService(ILogger<EditAnalysisDeepSeekService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GenerateTestCodeAsync(string prompt, string context)
        {
            _logger.LogInformation("DeepSeek generating test code for: {Context}", context);
            await Task.Delay(300);
            return "{\"test\": \"// DeepSeek-generated code with technical precision\", \"confidence\": 0.82}";
        }

        public async Task<string> AnalyzeContentAsync(string content, string analysisType)
        {
            _logger.LogInformation("DeepSeek analyzing edit: {AnalysisType}", analysisType);
            await Task.Delay(250);
            return "{\"edit_analysis\": {\"alignment\": true, \"improvements\": [\"fixed edge case\"], \"issues\": []}}";
        }

        public async Task<string> AnswerQuestionAsync(string question, string context)
        {
            _logger.LogInformation("DeepSeek answering technical question");
            await Task.Delay(200);
            return "DeepSeek's technical answer: [Precise technical explanation with code examples]";
        }
    }

    // Services/Implementations/RealTimeCollaborationTools.cs
    public class RealTimeCollaborationTools : ICollaborationTools
    {
        private readonly ILogger<RealTimeCollaborationTools> _logger;

        public RealTimeCollaborationTools(ILogger<RealTimeCollaborationTools> logger)
        {
            _logger = logger;
        }

        public async Task<string> CreateWorkspaceAsync(ReviewSession session)
        {
            _logger.LogInformation("Creating workspace for session {SessionId}", session.Id);
            await Task.Delay(100);
            return $"https://collab.example.com/workspace/{session.Id}";
        }

        public async Task<DiffResult> CalculateDiffAsync(string original, string modified)
        {
            _logger.LogInformation("Calculating diff between content");

            await Task.Delay(50);

            return new DiffResult
            {
                Original = original,
                Modified = modified,
                Changes = new[]
                {
                new DiffChange
                {
                    Type = "modify",
                    LineNumber = 1,
                    OldContent = original.Length > 0 ? original.Split('\n').FirstOrDefault() : "",
                    Content = modified.Length > 0 ? modified.Split('\n').FirstOrDefault() : ""
                }
            },
                Summary = $"Found {Math.Abs(original.Length - modified.Length)} character difference"
            };
        }

        public async Task<SuggestionResult[]> GenerateSuggestionsAsync(ReviewSession session, string context)
        {
            _logger.LogInformation("Generating suggestions for session {SessionId}", session.Id);

            await Task.Delay(150);

            return new[]
            {
            new SuggestionResult
            {
                Suggestion = "Consider adding more edge case tests",
                Type = "improvement",
                Confidence = 0.75,
                Reasoning = "The current test covers main flow but could benefit from edge cases",
                AffectedAreas = new[] { "test coverage", "robustness" }
            }
        };
        }
    }

    // Services/Implementations/IntelligentDiffService.cs
    public class IntelligentDiffService : IDiffService
    {
        private readonly ILogger<IntelligentDiffService> _logger;

        public IntelligentDiffService(ILogger<IntelligentDiffService> logger)
        {
            _logger = logger;
        }

        public async Task<DiffResult> CalculateDiffAsync(string original, string modified, DiffOptions options)
        {
            _logger.LogInformation("Calculating intelligent diff with options: WordLevel={WordLevel}", options.WordLevel);

            await Task.Delay(100);

            var originalLines = original.Split('\n');
            var modifiedLines = modified.Split('\n');

            var changes = new List<DiffChange>();

            for (int i = 0; i < Math.Max(originalLines.Length, modifiedLines.Length); i++)
            {
                var orig = i < originalLines.Length ? originalLines[i] : "";
                var mod = i < modifiedLines.Length ? modifiedLines[i] : "";

                if (orig != mod)
                {
                    changes.Add(new DiffChange
                    {
                        Type = orig == "" ? "add" : mod == "" ? "remove" : "modify",
                        LineNumber = i + 1,
                        OldContent = orig,
                        Content = mod
                    });
                }
            }

            return new DiffResult
            {
                Original = original,
                Modified = modified,
                Changes = changes.ToArray(),
                Summary = $"Found {changes.Count} line changes"
            };
        }

        public async Task<MergeResult> MergeChangesAsync(string baseContent, string[] variations)
        {
            _logger.LogInformation("Merging {VariationCount} variations", variations.Length);

            await Task.Delay(200);

            var conflicts = new List<MergeConflict>();

            if (variations.Length > 1 && variations[0] != variations[1])
            {
                conflicts.Add(new MergeConflict
                {
                    LineNumber = 1,
                    Options = variations,
                    Suggestion = "Choose the most appropriate version or create a combination"
                });
            }

            return new MergeResult
            {
                MergedContent = variations.FirstOrDefault() ?? baseContent,
                Conflicts = conflicts.ToArray()
            };
        }
    }

    // Services/Implementations/ContextAwareSuggestionEngine.cs
    public class ContextAwareSuggestionEngine : ISuggestionEngine
    {
        private readonly ILLMServiceFactory _llmFactory;
        private readonly ILogger<ContextAwareSuggestionEngine> _logger;

        public ContextAwareSuggestionEngine(ILLMServiceFactory llmFactory, ILogger<ContextAwareSuggestionEngine> logger)
        {
            _llmFactory = llmFactory;
            _logger = logger;
        }

        public async Task<SuggestionResult[]> GenerateSuggestionsAsync(ReviewSession session, string context)
        {
            _logger.LogInformation("Generating context-aware suggestions for session {SessionId}", session.Id);

            var llmService = _llmFactory.GetService("chatgpt");
            var prompt = $@"
        Generate suggestions for improving this test based on review context:
        
        TEST: {session.CurrentContent.Content}
        CONTEXT: {context}
        REVIEW FOCUS: {string.Join(", ", session.SuggestedReviewFocus)}
        
        Provide 2-3 specific, actionable suggestions.
        ";

            var suggestionsJson = await llmService.GenerateTestCodeAsync(prompt, "Generate suggestions");

            await Task.Delay(100);

            return new[]
            {
            new SuggestionResult
            {
                Suggestion = "Add null checks for input parameters",
                Type = "safety",
                Confidence = 0.85,
                Reasoning = "Prevents null reference exceptions in edge cases",
                AffectedAreas = new[] { "input validation", "error handling" }
            },
            new SuggestionResult
            {
                Suggestion = "Include timeout handling for async operations",
                Type = "reliability",
                Confidence = 0.75,
                Reasoning = "Improves test stability in slow environments",
                AffectedAreas = new[] { "async handling", "performance" }
            }
        };
        }

        public async Task<EditValidationResult> ValidateEditAsync(GeneratedTest currentContent, UserEdit edit)
        {
            _logger.LogInformation("Validating edit from {EditorId}", edit.EditorId);

            if (string.IsNullOrWhiteSpace(edit.Content))
                return EditValidationResult.Invalid("Edit content is empty");

            if (edit.Content.Length > 10000)
                return EditValidationResult.Invalid("Edit too large (max 10000 characters)");

            var syntaxCheck = await CheckSyntaxAsync(currentContent.Content, edit);
            if (!syntaxCheck.IsValid)
                return EditValidationResult.Invalid($"Syntax error: {syntaxCheck.Error}");

            var dangerousPatterns = DetectDangerousPatterns(edit.Content);
            if (dangerousPatterns.Any())
                return EditValidationResult.Invalid($"Contains dangerous patterns: {string.Join(", ", dangerousPatterns)}");

            return EditValidationResult.Valid(new EditImpact
            {
                ImpactLevel = DetermineImpactLevel(currentContent.Content, edit.Content),
                AffectedAreas = DetermineAffectedAreas(currentContent.Content, edit.Content),
                Benefits = new[] { "Improved test clarity", "Better edge case coverage" },
                Summary = "Edit improves test quality"
            });
        }

        private async Task<(bool IsValid, string Error)> CheckSyntaxAsync(string currentContent, UserEdit edit)
        {
            await Task.Delay(50);

            if (edit.Content.Contains("while(true)"))
                return (false, "Potential infinite loop");

            if (edit.Content.Contains("System.IO.File.Delete"))
                return (false, "File deletion in test code");

            return (true, "");
        }

        private string[] DetectDangerousPatterns(string content)
        {
            var dangerousPatterns = new List<string>();

            if (content.Contains("Thread.Sleep(10000)"))
                dangerousPatterns.Add("Long sleep in test");

            if (content.Contains("Environment.Exit"))
                dangerousPatterns.Add("Process termination in test");

            if (content.Contains("Random()") && !content.Contains("seed"))
                dangerousPatterns.Add("Non-deterministic random");

            return dangerousPatterns.ToArray();
        }

        private string DetermineImpactLevel(string original, string edit)
        {
            var similarity = CalculateSimilarity(original, edit);

            return similarity switch
            {
                > 0.9 => "low",
                > 0.7 => "medium",
                _ => "high"
            };
        }

        private string[] DetermineAffectedAreas(string original, string edit)
        {
            var areas = new List<string>();

            if (edit.Contains("Assert"))
                areas.Add("assertions");

            if (edit.Contains("async") || edit.Contains("Task"))
                areas.Add("async operations");

            if (edit.Contains("Exception") || edit.Contains("catch"))
                areas.Add("exception handling");

            return areas.ToArray();
        }

        private double CalculateSimilarity(string a, string b)
        {
            if (a == b) return 1.0;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0.0;

            var maxLength = Math.Max(a.Length, b.Length);
            var editDistance = ComputeLevenshteinDistance(a, b);

            return 1.0 - (double)editDistance / maxLength;
        }

        private int ComputeLevenshteinDistance(string a, string b)
        {
            var matrix = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++)
                matrix[i, 0] = i;

            for (int j = 0; j <= b.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[a.Length, b.Length];
        }
    }

    // Services/Implementations/CollaborationHub.cs
    public class CollaborationHub : Hub, ICollaborationHub
    {
        private readonly ILogger<CollaborationHub> _logger;
        private readonly ICollaborationSessionManager _sessionManager;

        public CollaborationHub(ILogger<CollaborationHub> logger, ICollaborationSessionManager sessionManager)
        {
            _logger = logger;
            _sessionManager = sessionManager;
        }

        public async Task JoinSession(string sessionId)
        {
            _logger.LogInformation("User {ConnectionId} joining session {SessionId}", Context.ConnectionId, sessionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

            try
            {
                var session = await _sessionManager.GetSessionAsync(sessionId);
                await Clients.Caller.SendAsync("SessionJoined", session);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get session {SessionId}", sessionId);
                await Clients.Caller.SendAsync("Error", $"Failed to join session: {ex.Message}");
            }
        }

        public async Task LeaveSession(string sessionId)
        {
            _logger.LogInformation("User {ConnectionId} leaving session {SessionId}", Context.ConnectionId, sessionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        }

        public async Task SendEdit(string sessionId, UserEdit edit)
        {
            _logger.LogInformation("User {ConnectionId} sending edit to session {SessionId}", Context.ConnectionId, sessionId);

            try
            {
                var request = new CollaborativeEditRequest
                {
                    UserEdit = edit,
                    EditContext = "real-time collaboration",
                    RequestAiAnalysis = true
                };

                var updatedSession = await _sessionManager.ApplyHumanEditAsync(sessionId, request);

                await Clients.Group(sessionId).SendAsync("EditApplied", new
                {
                    Editor = edit.EditorId,
                    Edit = edit,
                    Session = updatedSession,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply edit to session {SessionId}", sessionId);
                await Clients.Caller.SendAsync("EditFailed", new
                {
                    Error = ex.Message,
                    EditId = edit.EditorId
                });
            }
        }

        public async Task SendMessage(string sessionId, string message)
        {
            _logger.LogDebug("User {ConnectionId} sending message to session {SessionId}", Context.ConnectionId, sessionId);
            await Clients.Group(sessionId).SendAsync("NewMessage", new
            {
                Sender = Context.ConnectionId,
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
