using Chapter_2.Exceptions;
using Chapter_2.Models;
using Chapter_2.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chapter_2.Controllers
{
    [ApiController]
    [Route("api/onboarding")]
    [ApiExplorerSettings(GroupName = "90-day-plan")]
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingOrchestrator _onboardingOrchestrator;
        private readonly ICodebaseAnalyzer _codebaseAnalyzer;
        private readonly ILogger<OnboardingController> _logger;

        public OnboardingController(
            IOnboardingOrchestrator onboardingOrchestrator,
            ICodebaseAnalyzer codebaseAnalyzer,
            ILogger<OnboardingController> logger)
        {
            _onboardingOrchestrator = onboardingOrchestrator;
            _codebaseAnalyzer = codebaseAnalyzer;
            _logger = logger;
        }

        [HttpPost("codebase-analysis")]
        [ProducesResponseType(typeof(CodebaseAnalysisResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OnboardingErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AnalyzeCodebase([FromBody] FirstImpressionRequest request)
        {
            // Custom validation for onboarding context
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                return BadRequest(new OnboardingErrorResponse
                {
                    Phase = "discovery",
                    ErrorType = "validation",
                    Message = "The codebase analysis request needs some adjustments",
                    RecoveryAction = "Check repository URL and focus areas",
                    SuggestedFallback = "Start with a smaller directory or specific file types",
                    Context = new Dictionary<string, object> { ["validationErrors"] = errors }
                });
            }

            try
            {
                _logger.LogInformation(
                    "Starting codebase analysis for {RepoUrl} with focus {FocusAreas}",
                    MaskSensitiveUrl(request.RepositoryUrl),
                    string.Join(", ", request.FocusAreas));

                // Step 1: Clone and analyze
                var analysis = await _codebaseAnalyzer.AnalyzeAsync(request, TimeSpan.FromMinutes(5));

                // Step 2: Generate onboarding-specific insights
                var insights = await _onboardingOrchestrator.GenerateOnboardingInsightsAsync(analysis, request);

                // Step 3: Create phased recommendations
                var recommendations = CreatePhasedRecommendations(analysis, insights, request.Metadata.MaxRecommendations);

                var response = new CodebaseAnalysisResponse
                {
                    AnalysisId = Guid.NewGuid().ToString(),
                    MaturityScore = CalculateTestingMaturity(analysis),
                    HighestRiskArea = insights.HighestRiskArea,
                    QuickWins = recommendations.Where(r => r.EffortDays <= 2).Select(r => r.Description).ToArray(),
                    TechnicalDebtAreas = analysis.TechnicalDebt.Select(td => td.Description).ToArray(),
                    TestCoverage = analysis.TestCoverage,
                    RecommendationTimeline = recommendations,
                    NextSteps = GenerateFirstWeekActionItems(insights),
                    ConfidenceScore = analysis.ConfidenceScore
                };

                _logger.LogInformation(
                    "Codebase analysis complete: Maturity {Maturity}/10, {QuickWinCount} quick wins identified",
                    response.MaturityScore, response.QuickWins.Length);
                return Ok(response);
            }
            catch (RepositoryAccessException raex)
            {
                _logger.LogWarning(raex, "Cannot access repository {RepoUrl}", request.RepositoryUrl);
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new OnboardingErrorResponse
                {
                    Phase = "discovery",
                    ErrorType = "connectivity",
                    Message = "Couldn't access the code repository",
                    RecoveryAction = "Check repository permissions and URL",
                    SuggestedFallback = "Upload code samples directly or analyze test directories only"
                });
            }
            catch (AnalysisTimeoutException atex)
            {
                _logger.LogWarning(atex, "Analysis timed out for {RepoUrl}", request.RepositoryUrl);

                // Return partial analysis with clear caveats
                var partialResponse = await GeneratePartialAnalysisAsync(request);
                return Ok(partialResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during codebase analysis");

                return StatusCode(StatusCodes.Status500InternalServerError, new OnboardingErrorResponse
                {
                    Phase = "discovery",
                    ErrorType = "analysis",
                    Message = "Something went wrong during analysis",
                    RecoveryAction = "Try a smaller scope or different focus areas",
                    SuggestedFallback = "Use manual code review for initial assessment"
                });
            }
        }

        [HttpPost("generate-questions")]
        [ProducesResponseType(typeof(InterviewQuestionsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OnboardingErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateTeamInterviewQuestions([FromBody] QuestionGenerationRequest request)
        {
            // Validate question generation request
            if (request.Count > 20)
            {
                return BadRequest(new OnboardingErrorResponse
                {
                    Phase = "planning",
                    ErrorType = "validation",
                    Message = "Too many questions requested",
                    RecoveryAction = "Keep it under 20 for effective interviews",
                    SuggestedFallback = "Generate 10 questions and add more if needed"
                });
            }

            try
            {
                var insights = await _onboardingOrchestrator.GenerateInterviewQuestionsAsync(request);

                return Ok(new InterviewQuestionsResponse
                {
                    Questions = insights.Questions,
                    QuestionCategories = insights.QuestionCategories,
                    UsageTips = GenerateQuestionUsageTips(request.Style, request.Audience)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating interview questions");
                return StatusCode(StatusCodes.Status500InternalServerError, new OnboardingErrorResponse
                {
                    Phase = "planning",
                    ErrorType = "generation",
                    Message = "Failed to generate interview questions",
                    RecoveryAction = "Try with different parameters",
                    SuggestedFallback = "Use predefined question templates"
                });
            }
        }

        [HttpPost("first-test-fix")]
        [ProducesResponseType(typeof(FirstFixResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OnboardingErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFirstTestFix([FromBody] FirstFixRequest request)
        {
            // This is where you tackle that first flaky test
            if (string.IsNullOrEmpty(request.FlakyTestPath))
            {
                return BadRequest(new OnboardingErrorResponse
                {
                    Phase = "execution",
                    ErrorType = "validation",
                    Message = "Need the path to the flaky test to fix it",
                    RecoveryAction = "Provide the test file path",
                    SuggestedFallback = "Try with a sample test first"
                });
            }

            try
            {
                var fix = await _onboardingOrchestrator.CreateFirstAIFixAsync(request);

                return Ok(new FirstFixResponse
                {
                    OriginalTest = fix.OriginalTest,
                    FixedTest = fix.FixedTest,
                    ChangesMade = fix.Changes,
                    Explanation = fix.Explanation,
                    Confidence = fix.Confidence,
                    BeforeAfterMetrics = fix.Metrics,
                    NextLearningStep = "Now run it 20 times to verify it's not flaky anymore"
                });
            }
            catch (TestAnalysisException taex)
            {
                return StatusCode(422, new OnboardingErrorResponse
                {
                    Phase = "execution",
                    ErrorType = "analysis",
                    Message = "Couldn't understand the test structure",
                    RecoveryAction = "Check test format and syntax",
                    SuggestedFallback = "Share a simpler test or a different flaky test"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test fix");
                return StatusCode(StatusCodes.Status500InternalServerError, new OnboardingErrorResponse
                {
                    Phase = "execution",
                    ErrorType = "fix",
                    Message = "Failed to create test fix",
                    RecoveryAction = "Try with different fix strategy",
                    SuggestedFallback = "Manual code review and fix"
                });
            }
        }

        [HttpPost("learning-path")]
        [ProducesResponseType(typeof(LearningPath), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateLearningPath([FromBody] LearningPathRequest request)
        {
            try
            {
                var path = await _onboardingOrchestrator.Generate90DayLearningPathAsync(request);
                return Ok(path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating learning path");
                return StatusCode(StatusCodes.Status500InternalServerError, new OnboardingErrorResponse
                {
                    Phase = "planning",
                    ErrorType = "generation",
                    Message = "Failed to generate learning path",
                    RecoveryAction = "Try with different parameters",
                    SuggestedFallback = "Use standard 90-day plan template"
                });
            }
        }

        // Helper methods
        private string MaskSensitiveUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                return $"{uri.Scheme}://{uri.Host}/***";
            }
            catch
            {
                return "invalid-url/***";
            }
        }

        private decimal CalculateTestingMaturity(CodeAnalysis analysis)
        {
            if (analysis.TestCoverage == null) return 1;

            var coverageScore = analysis.TestCoverage.Overall * 0.4m;
            var debtScore = (1 - (analysis.TechnicalDebt.Length / 20m)) * 0.3m;
            var confidenceScore = (decimal)analysis.ConfidenceScore * 0.3m;

            return Math.Clamp((coverageScore + debtScore + confidenceScore) * 10, 1, 10);
        }

        private RecommendationTimeline[] CreatePhasedRecommendations(CodeAnalysis analysis, CodebaseInsights insights, int maxRecommendations)
        {
            var recommendations = new List<RecommendationTimeline>();

            // Immediate (week 1)
            recommendations.Add(new RecommendationTimeline
            {
                Description = "Set up local development environment",
                Phase = "immediate",
                EffortDays = 0.5m,
                Priority = 1,
                ExpectedImpact = "Enable local testing and debugging"
            });

            // Short-term (weeks 2-4)
            if (analysis.TechnicalDebt.Any())
            {
                recommendations.Add(new RecommendationTimeline
                {
                    Description = "Address high-priority technical debt",
                    Phase = "short-term",
                    EffortDays = 3,
                    Priority = 2,
                    ExpectedImpact = "Improve code quality and reduce bugs"
                });
            }

            // Medium-term (weeks 5-8)
            recommendations.Add(new RecommendationTimeline
            {
                Description = "Implement comprehensive test automation",
                Phase = "medium-term",
                EffortDays = 10,
                Priority = 3,
                ExpectedImpact = "Increase test coverage and reliability"
            });

            // Long-term (weeks 9-12)
            recommendations.Add(new RecommendationTimeline
            {
                Description = "Establish CI/CD pipeline with quality gates",
                Phase = "long-term",
                EffortDays = 15,
                Priority = 4,
                ExpectedImpact = "Automate quality assurance process"
            });

            return recommendations.Take(maxRecommendations).ToArray();
        }

        private string[] GenerateFirstWeekActionItems(CodebaseInsights insights)
        {
            var items = new List<string>
        {
            "Review codebase analysis report",
            "Identify key stakeholders and schedule introductions",
            "Set up development and testing environment",
            "Run existing test suite to understand current state",
            "Document initial observations and questions"
        };

            if (!string.IsNullOrEmpty(insights.HighestRiskArea))
            {
                items.Add($"Focus on understanding the {insights.HighestRiskArea} area");
            }

            return items.ToArray();
        }

        private string[] GenerateQuestionUsageTips(string style, string audience)
        {
            var tips = new List<string>
        {
            "Use open-ended questions to encourage discussion",
            "Allow time for thoughtful responses",
            "Take notes during the interview",
            "Follow up on interesting points"
        };

            if (style == "technical")
            {
                tips.Add("Include practical coding exercises");
                tips.Add("Focus on problem-solving approach, not just syntax");
            }
            else if (style == "collaborative")
            {
                tips.Add("Discuss team dynamics and collaboration experiences");
                tips.Add("Ask about conflict resolution approaches");
            }

            return tips.ToArray();
        }

        private async Task<CodebaseAnalysisResponse> GeneratePartialAnalysisAsync(FirstImpressionRequest request)
        {
            var partialAnalysis = await _codebaseAnalyzer.GeneratePartialAnalysisAsync(request);

            return new CodebaseAnalysisResponse
            {
                AnalysisId = Guid.NewGuid().ToString(),
                MaturityScore = 5, // Default mid-range
                HighestRiskArea = "Analysis incomplete",
                QuickWins = Array.Empty<string>(),
                TechnicalDebtAreas = Array.Empty<string>(),
                TestCoverage = new TestCoverageBreakdown(),
                RecommendationTimeline = Array.Empty<RecommendationTimeline>(),
                NextSteps = new[] { "Complete full analysis when possible", "Start with manual code review" },
                ConfidenceScore = 0.5,
                Metadata = new Dictionary<string, object>
                {
                    ["status"] = "partial",
                    ["note"] = "Analysis was interrupted or timed out"
                }
            };
        }
    }
}
