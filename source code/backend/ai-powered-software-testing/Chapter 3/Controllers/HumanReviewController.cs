using Microsoft.AspNetCore.Mvc;

namespace Chapter_3.Controllers
{
    using Chapter_3.Models.Domain;
    using Chapter_3.Models.Exceptions;
    using Chapter_3.Models.Requests;
    using Chapter_3.Models.Responses;
    using Chapter_3.Models.Supporting;
    using Chapter_3.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    // Controllers/HumanReviewController.cs
    [ApiController]
    [Route("api/human-review")]
    [ApiExplorerSettings(GroupName = "human-in-the-loop")]
    public class HumanReviewController : ControllerBase
    {
        private readonly IHumanReviewOrchestrator _reviewOrchestrator;
        private readonly ICollaborationSessionManager _sessionManager;
        private readonly IJudgmentProcessor _judgmentProcessor;
        private readonly ILogger<HumanReviewController> _logger;

        public HumanReviewController(
            IHumanReviewOrchestrator reviewOrchestrator,
            ICollaborationSessionManager sessionManager,
            IJudgmentProcessor judgmentProcessor,
            ILogger<HumanReviewController> logger)
        {
            _reviewOrchestrator = reviewOrchestrator;
            _sessionManager = sessionManager;
            _judgmentProcessor = judgmentProcessor;
            _logger = logger;
        }

        #region Public Endpoints

        /// <summary>
        /// Submit AI-generated test for human review
        /// </summary>
        [HttpPost("submit")]
        [ProducesResponseType(typeof(ReviewSessionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HumanReviewError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitForHumanReview([FromBody] ReviewRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationError(request));
            }

            try
            {
                _logger.LogInformation(
                    "Starting human review for {TestType} (risk: {RiskLevel})",
                    request.GeneratedContent.TestType,
                    request.Context.RiskLevel);

                var session = await _reviewOrchestrator.CreateReviewSessionAsync(request);
                var reviewers = await DetermineReviewersAsync(request);
                var workspace = await PrepareReviewWorkspaceAsync(session, request.ReviewerGuidance);

                var response = await CreateReviewSessionResponse(session, reviewers, workspace, request);

                _logger.LogInformation(
                    "Review session {SessionId} created with {ReviewerCount} reviewers",
                    session.Id, reviewers.Length);

                return CreatedAtAction(
                    nameof(GetReviewSession),
                    new { sessionId = session.Id },
                    response);
            }
            catch (ReviewContextException rcex)
            {
                _logger.LogWarning(rcex, "Context issue for review submission");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, CreateContextError(request, rcex));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to submit for human review");
                return StatusCode(StatusCodes.Status500InternalServerError, CreateGenericError(ex));
            }
        }

        /// <summary>
        /// Get review session details
        /// </summary>
        [HttpGet("{sessionId}")]
        [ProducesResponseType(typeof(ReviewSession), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReviewSession([FromRoute] string sessionId)
        {
            try
            {
                var session = await _sessionManager.GetSessionAsync(sessionId);
                if (session == null)
                {
                    return NotFound($"Review session {sessionId} not found");
                }

                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get review session {SessionId}", sessionId);
                return StatusCode(StatusCodes.Status500InternalServerError, CreateGenericError(ex));
            }
        }

        /// <summary>
        /// Collaborate on test with real-time editing
        /// </summary>
        [HttpPost("{sessionId}/collaborate")]
        [ProducesResponseType(typeof(CollaborationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CollaborationError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CollaborateOnTest(
            [FromRoute] string sessionId,
            [FromBody] CollaborativeEditRequest request)
        {
            var validationError = ValidateEditRequest(request);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            try
            {
                var session = await _sessionManager.GetSessionAsync(sessionId);
                if (session == null)
                {
                    return NotFound($"Review session {sessionId} not found");
                }

                var updatedSession = await _sessionManager.ApplyHumanEditAsync(sessionId, request);
                var aiResponse = await _reviewOrchestrator.AnalyzeHumanEditAsync(updatedSession, request);
                var impactAnalysis = await AnalyzeEditImpactAsync(updatedSession, request);

                var response = await CreateCollaborationResponse(updatedSession, aiResponse, impactAnalysis, request);

                return Ok(response);
            }
            catch (EditConflictException ecex)
            {
                _logger.LogWarning(ecex, "Edit conflict in session {SessionId}", sessionId);
                return Conflict(await CreateEditConflictError(sessionId, ecex));
            }
            catch (SessionNotFoundException)
            {
                return NotFound($"Review session {sessionId} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to collaborate on session {SessionId}", sessionId);
                return StatusCode(StatusCodes.Status500InternalServerError, CreateGenericError(ex));
            }
        }

        /// <summary>
        /// Request clarification from AI about the test
        /// </summary>
        [HttpPost("{sessionId}/clarify")]
        [ProducesResponseType(typeof(ClarificationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestClarification(
            [FromRoute] string sessionId,
            [FromBody] ClarificationRequest request)
        {
            var validationError = ValidateClarificationRequest(request);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            try
            {
                var session = await _sessionManager.GetSessionAsync(sessionId);
                if (session == null)
                {
                    return NotFound($"Review session {sessionId} not found");
                }

                var questionAnalysis = await _reviewOrchestrator.AnalyzeHumanQuestionAsync(request);
                var aiAnswer = await _reviewOrchestrator.GenerateClarificationAsync(session, request, questionAnalysis);
                var relevanceScore = await CalculateAnswerRelevanceAsync(request.HumanQuestion, aiAnswer.DirectAnswer);

                var response = await CreateClarificationResponse(questionAnalysis, aiAnswer, relevanceScore, request);

                return Ok(response);
            }
            catch (AmbiguousQuestionException aqex)
            {
                _logger.LogInformation(aqex, "Ambiguous question in session {SessionId}", sessionId);
                return Ok(await CreateAmbiguousQuestionResponse(request, aqex));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process clarification for session {SessionId}", sessionId);
                return StatusCode(StatusCodes.Status500InternalServerError, CreateGenericError(ex));
            }
        }

        /// <summary>
        /// Provide final human judgment on the test
        /// </summary>
        [HttpPost("{sessionId}/judge")]
        [ProducesResponseType(typeof(JudgmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HumanReviewError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ProvideHumanJudgment(
            [FromRoute] string sessionId,
            [FromBody] JudgmentRequest request)
        {
            var validationError = ValidateJudgmentRequest(request);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            try
            {
                var session = await _sessionManager.GetSessionAsync(sessionId);
                if (session == null)
                {
                    return NotFound($"Review session {sessionId} not found");
                }

                var outcome = await _judgmentProcessor.ProcessJudgmentAsync(session, request);
                await _reviewOrchestrator.LearnFromJudgmentAsync(request);
                var insights = await ExtractReviewInsightsAsync(session, request, outcome);

                var response = await CreateJudgmentResponse(outcome, insights, request);

                await _sessionManager.CloseSessionAsync(sessionId, outcome);

                _logger.LogInformation(
                    "Human judgment completed for session {SessionId}: {Decision}",
                    sessionId, request.Judgment.Decision);

                return Ok(response);
            }
            catch (JudgmentProcessingException jpex)
            {
                _logger.LogError(jpex, "Failed to process judgment for session {SessionId}", sessionId);
                return StatusCode(StatusCodes.Status500InternalServerError, CreateJudgmentProcessingError(jpex));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process judgment for session {SessionId}", sessionId);
                return StatusCode(StatusCodes.Status500InternalServerError, CreateGenericError(ex));
            }
        }

        #endregion

        #region Helper Methods

        private HumanReviewError CreateValidationError(ReviewRequest request)
        {
            return new HumanReviewError
            {
                ErrorCode = "INSUFFICIENT_CONTEXT",
                Message = "Can't start review without proper context",
                RequiredElements = new[] { "generatedContent", "testPurpose", "riskLevel" },
                MissingElements = GetMissingElements(request),
                Suggestion = "Provide test purpose and risk level for meaningful review"
            };
        }

        private HumanReviewError CreateContextError(ReviewRequest request, ReviewContextException ex)
        {
            return new HumanReviewError
            {
                ErrorCode = ex.ErrorCode ?? "CONTEXT_UNCLEAR",
                Message = "The AI needs more context for you to review effectively",
                Suggestion = "Add more detail about what you're trying to test and why",
                ContextEnhancementPrompts = GenerateContextPrompts(request),
                MissingElements = ex.MissingContext,
                ReportedAt = DateTime.UtcNow
            };
        }

        private HumanReviewError CreateGenericError(Exception ex)
        {
            return new HumanReviewError
            {
                ErrorCode = "INTERNAL_ERROR",
                Message = "An unexpected error occurred",
                Suggestion = "Please try again later or contact support",
                RecoveryActions = new[] { "retry", "contact-support" },
                ReportedAt = DateTime.UtcNow
            };
        }

        private string[] GetMissingElements(ReviewRequest request)
        {
            var missing = new List<string>();

            if (request.GeneratedContent == null)
                missing.Add("generatedContent");
            if (string.IsNullOrEmpty(request.Context.TestPurpose))
                missing.Add("testPurpose");
            if (request.Context.RiskLevel == null)
                missing.Add("riskLevel");

            return missing.ToArray();
        }

        private async Task<ReviewSessionResponse> CreateReviewSessionResponse(
            ReviewSession session,
            string[] reviewers,
            dynamic workspace,
            ReviewRequest request)
        {
            return new ReviewSessionResponse
            {
                SessionId = session.Id,
                ReviewersAssigned = reviewers,
                EstimatedReviewTime = EstimateReviewTime(request),
                ReviewChecklist = GenerateReviewChecklist(request),
                WorkspaceUrl = workspace.Url,
                InitialQuestions = await GenerateInitialQuestionsAsync(session),
                AiConfidenceStatement = session.AiConfidenceStatement,
                AreasNeedingHumanJudgment = session.AreasNeedingHumanJudgment,
                AvailableTools = new CollaborationTools
                {
                    RealTimeEditing = true,
                    Comments = true,
                    Suggestions = true,
                    Chat = true,
                    VersionHistory = true,
                    SideBySideDiff = true
                },
                QuickActions = new[]
                {
                "approve-as-is",
                "request-clarification",
                "start-editing",
                "escalate-to-team"
            }
            };
        }

        private async Task<string[]> DetermineReviewersAsync(ReviewRequest request)
        {
            var reviewers = new List<string>();

            if (request.Context.RiskLevel == "high" || request.Context.RiskLevel == "critical")
            {
                reviewers.Add("senior-reviewer");
                reviewers.Add("domain-expert");
            }
            else
            {
                reviewers.Add("standard-reviewer");
            }

            if (request.Context.TechnicalDomains.Contains("security"))
                reviewers.Add("security-expert");

            await Task.CompletedTask;
            return reviewers.ToArray();
        }

        private async Task<dynamic> PrepareReviewWorkspaceAsync(ReviewSession session, string guidance)
        {
            await Task.CompletedTask;
            return new
            {
                Url = $"https://collab.example.com/review/{session.Id}",
                Tools = new[] { "code-editor", "chat", "suggestions", "diff-viewer" },
                Guidance = guidance,
                CreatedAt = DateTime.UtcNow
            };
        }

        private TimeSpan EstimateReviewTime(ReviewRequest request)
        {
            var baseTime = TimeSpan.FromMinutes(15);

            if (request.Context.RiskLevel == "high")
                baseTime = baseTime.Add(TimeSpan.FromMinutes(30));

            if (request.GeneratedContent.Content.Length > 1000)
                baseTime = baseTime.Add(TimeSpan.FromMinutes(10));

            return baseTime;
        }

        private ReviewChecklistItem[] GenerateReviewChecklist(ReviewRequest request)
        {
            return new[]
            {
            new ReviewChecklistItem
            {
                Item = "Test Purpose Alignment",
                Category = "business",
                Description = "Does the test align with the stated purpose?",
                IsRequired = true,
                Guidance = "Check if test covers the intended functionality"
            },
            new ReviewChecklistItem
            {
                Item = "Edge Cases",
                Category = "technical",
                Description = "Are important edge cases covered?",
                IsRequired = true,
                Guidance = "Look for boundary conditions and error cases"
            },
            new ReviewChecklistItem
            {
                Item = "Code Quality",
                Category = "technical",
                Description = "Is the code readable and maintainable?",
                IsRequired = true,
                Guidance = "Check naming, structure, and comments"
            }
        };
        }

        private async Task<InitialQuestion[]> GenerateInitialQuestionsAsync(ReviewSession session)
        {
            await Task.CompletedTask;
            return new[]
            {
            new InitialQuestion
            {
                Question = "Does this test adequately cover the main business requirement?",
                Type = "validating",
                WhyImportant = "Ensures test serves its purpose",
                IsRequired = true,
                Priority = 1
            },
            new InitialQuestion
            {
                Question = "What edge cases might be missing?",
                Type = "probing",
                WhyImportant = "Improves test robustness",
                IsRequired = false,
                Priority = 2
            }
        };
        }

        private string[] GenerateContextPrompts(ReviewRequest request)
        {
            return new[]
            {
            "What specific behavior are you testing?",
            "What are the acceptance criteria?",
            "Are there any special constraints or requirements?"
        };
        }

        private string ValidateEditRequest(CollaborativeEditRequest request)
        {
            if (request.UserEdit.Content.Length > 10000)
            {
                return "Edit too large - consider breaking into smaller changes";
            }

            if (string.IsNullOrWhiteSpace(request.UserEdit.Intent))
            {
                return "Please describe why you're making this change";
            }

            return null;
        }

        private async Task<CollaborationResponse> CreateCollaborationResponse(
            ReviewSession session,
            AiEditAnalysis aiResponse,
            EditImpact impactAnalysis,
            CollaborativeEditRequest request)
        {
            return new CollaborationResponse
            {
                Session = session,
                AiPerspective = aiResponse,
                ImpactAnalysis = impactAnalysis,
                SuggestedNextEdits = await SuggestRelatedEditsAsync(session, request),
                LearningOpportunities = ExtractLearningFromEdit(request, aiResponse)
            };
        }

        private async Task<EditImpact> AnalyzeEditImpactAsync(ReviewSession session, CollaborativeEditRequest request)
        {
            await Task.CompletedTask;
            return new EditImpact
            {
                ImpactLevel = "medium",
                AffectedAreas = new[] { "test logic", "assertions" },
                Benefits = new[] { "Improved clarity", "Better coverage" },
                Summary = "Edit improves test quality"
            };
        }

        private async Task<AiSuggestion[]> SuggestRelatedEditsAsync(ReviewSession session, CollaborativeEditRequest request)
        {
            await Task.CompletedTask;
            return Array.Empty<AiSuggestion>();
        }

        private LearningOpportunity[] ExtractLearningFromEdit(CollaborativeEditRequest request, AiEditAnalysis aiResponse)
        {
            return new[]
            {
            new LearningOpportunity
            {
                Opportunity = "Understanding human edit patterns",
                Category = "edit-analysis",
                WhyImportant = "Improves AI's ability to generate better tests",
                ShouldPrioritize = true
            }
        };
        }

        private async Task<CollaborationError> CreateEditConflictError(string sessionId, EditConflictException ecex)
        {
            return new CollaborationError
            {
                SessionId = sessionId,
                ConflictType = ecex.ConflictType,
                ConflictingEdits = ecex.ConflictingEdits,
                ResolutionOptions = new[] { "keep-both", "use-latest", "merge-manually" },
                AiMergeSuggestion = await GenerateMergeSuggestionAsync(sessionId, ecex),
                OccurredAt = DateTime.UtcNow
            };
        }

        private async Task<string> GenerateMergeSuggestionAsync(string sessionId, EditConflictException exception)
        {
            await Task.CompletedTask;
            return "Consider merging the edits by taking the best parts of each version. Focus on maintaining test quality.";
        }

        private string ValidateClarificationRequest(ClarificationRequest request)
        {
            if (request.HumanQuestion.Length < 10)
            {
                return "Please ask a more detailed question";
            }

            if (request.HumanQuestion.Length > 1000)
            {
                return "Question too long - try breaking into multiple questions";
            }

            return null;
        }

        private async Task<ClarificationResponse> CreateClarificationResponse(
            QuestionAnalysis questionAnalysis,
            AiClarification aiAnswer,
            double relevanceScore,
            ClarificationRequest request)
        {
            var response = new ClarificationResponse
            {
                RoundId = Guid.NewGuid().ToString(),
                QuestionAnalysis = questionAnalysis,
                AiAnswer = aiAnswer,
                RelevanceScore = relevanceScore,
                SuggestedFollowUps = GenerateFollowUpQuestions(request, aiAnswer),
                ConfidenceStatement = aiAnswer.ConfidenceStatement,
                WhenToAskHuman = DetermineWhenToEscalate(questionAnalysis, aiAnswer)
            };

            if (relevanceScore < 0.6)
            {
                response.RephrasingSuggestions = await GenerateRephrasingSuggestionsAsync(request);
            }

            return response;
        }

        private async Task<double> CalculateAnswerRelevanceAsync(string question, string answer)
        {
            await Task.CompletedTask;
            var questionWords = question.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var answerWords = answer.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var matchingWords = questionWords.Intersect(answerWords).Count();
            return (double)matchingWords / Math.Max(questionWords.Length, 1);
        }

        private string[] GenerateFollowUpQuestions(ClarificationRequest request, AiClarification answer)
        {
            return new[]
            {
            "Could you provide an example?",
            "How does this apply to edge cases?",
            "Are there any gotchas to watch out for?"
        };
        }

        private string DetermineWhenToEscalate(QuestionAnalysis questionAnalysis, AiClarification answer)
        {
            if (answer.Confidence < 0.5 || questionAnalysis.IsAmbiguous)
                return "Immediately - AI confidence is low";

            return "If the answer doesn't fully address your concern";
        }

        private async Task<string[]> GenerateRephrasingSuggestionsAsync(ClarificationRequest request)
        {
            await Task.CompletedTask;
            return new[]
            {
            "Could you provide more specific context about what you're testing?",
            "What part of the test are you referring to?",
            "Could you ask about a specific aspect of the test?"
        };
        }

        private async Task<ClarificationResponse> CreateAmbiguousQuestionResponse(
            ClarificationRequest request,
            AmbiguousQuestionException aqex)
        {
            return new ClarificationResponse
            {
                RoundId = Guid.NewGuid().ToString(),
                QuestionAnalysis = new QuestionAnalysis { IsAmbiguous = true },
                AiAnswer = new AiClarification
                {
                    DirectAnswer = "I'm not sure what you're asking. Could you clarify?",
                    Alternatives = await GenerateInterpretationsAsync(request),
                    Confidence = 0.3,
                    Assumptions = new[] { "Multiple interpretations possible" },
                    RecommendedAction = "Rephrase with more specific context"
                }
            };
        }

        private async Task<string[]> GenerateInterpretationsAsync(ClarificationRequest request)
        {
            await Task.CompletedTask;
            return new[]
            {
            "You might be asking about test methodology",
            "You might be asking about business logic",
            "You might be asking about technical implementation"
        };
        }

        private string ValidateJudgmentRequest(JudgmentRequest request)
        {
            if (string.IsNullOrEmpty(request.Judgment.Reasoning))
            {
                return "Please provide reasoning for your judgment";
            }

            if (request.Judgment.Decision == "request-revision" &&
                !request.Judgment.SuggestedImprovements.Any())
            {
                return "Please suggest improvements when requesting revision";
            }

            return null;
        }

        private async Task<JudgmentResponse> CreateJudgmentResponse(
            ReviewOutcome outcome,
            ReviewInsight[] insights,
            JudgmentRequest request)
        {
            return new JudgmentResponse
            {
                Outcome = outcome,
                Insights = insights,
                NextSteps = DetermineNextSteps(outcome),
                FeedbackForHuman = GenerateFeedbackForHuman(request),
                ModelUpdatesApplied = await GetModelUpdateSummaryAsync(request)
            };
        }

        private async Task<ReviewInsight[]> ExtractReviewInsightsAsync(
            ReviewSession session,
            JudgmentRequest request,
            ReviewOutcome outcome)
        {
            await Task.CompletedTask;
            return new[]
            {
            new ReviewInsight
            {
                Insight = "Human feedback identified areas for AI improvement",
                Category = "learning",
                Impact = "educational",
                Evidence = request.Judgment.SuggestedImprovements,
                Actionable = true
            }
        };
        }

        private string[] DetermineNextSteps(ReviewOutcome outcome)
        {
            return outcome.Decision switch
            {
                "approve" => new[] { "Deploy test", "Monitor results" },
                "request-revision" => new[] { "Revise test", "Resubmit for review" },
                _ => new[] { "Investigate issues", "Consider alternative approach" }
            };
        }

        private string GenerateFeedbackForHuman(JudgmentRequest request)
        {
            return "Thank you for your thorough review. Your feedback will help improve AI test generation.";
        }

        private async Task<ModelUpdateSummary[]> GetModelUpdateSummaryAsync(JudgmentRequest request)
        {
            await Task.CompletedTask;
            return Array.Empty<ModelUpdateSummary>();
        }

        private HumanReviewError CreateJudgmentProcessingError(JudgmentProcessingException jpex)
        {
            return new HumanReviewError
            {
                ErrorCode = jpex.ErrorCode ?? "JUDGMENT_PROCESSING_FAILED",
                Message = "Couldn't process your judgment",
                Suggestion = "Try saving your feedback and we'll process it later",
                RecoveryActions = new[] { "retry", "save-as-draft", "contact-support" },
                ReportedAt = DateTime.UtcNow
            };
        }

        #endregion
    }
}
