
using Microsoft.Extensions.Options;
using Chapter_2.Models;
using Chapter_2.Services.Interfaces;
namespace Chapter_2.Services
{
   

    public class OnboardingOrchestrator : IOnboardingOrchestrator
    {
        private readonly ILLMServiceFactory _llmServiceFactory;
        private readonly ITestPatternRecognizer _patternRecognizer;
        private readonly ICodeAnalyzer _codeAnalyzer;
        private readonly ILogger<OnboardingOrchestrator> _logger;
        private readonly OnboardingSettings _settings;

        public OnboardingOrchestrator(
            ILLMServiceFactory llmServiceFactory,
            ITestPatternRecognizer patternRecognizer,
            ICodeAnalyzer codeAnalyzer,
            ILogger<OnboardingOrchestrator> logger,
            IOptions<OnboardingSettings> settings)
        {
            _llmServiceFactory = llmServiceFactory;
            _patternRecognizer = patternRecognizer;
            _codeAnalyzer = codeAnalyzer;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<FirstTestFix> CreateFirstAIFixAsync(FirstFixRequest request)
        {
            _logger.LogInformation(
                "Creating first AI fix for {TestPath} (failure rate: {FailureRate}%)",
                request.FlakyTestPath, request.FailureRatePercent);

            // Analyze the flaky test
            var analysis = await _patternRecognizer.AnalyzeFlakyTestAsync(
                request.TestContent,
                request.ObservedFailures);

            // Select LLM based on fix strategy
            var llmService = _llmServiceFactory.GetServiceForTask(
                task: "fix-flaky-test",
                strategy: request.FixStrategy,
                context: analysis.TechnicalContext);

            // Generate the fix
            var fixPrompt = BuildFixPrompt(analysis, request);
            var rawFix = await llmService.GenerateTestCodeAsync(fixPrompt, "Fix this flaky test");

            // Transform into actual code changes
            var transformedFix = await TransformAIFixAsync(rawFix, request.TestContent, analysis);

            // Create before/after metrics
            var metrics = new FixMetrics
            {
                WaitStatementsBefore = CountWaitStatements(request.TestContent),
                WaitStatementsAfter = CountWaitStatements(transformedFix),
                ComplexityBefore = CalculateCyclomaticComplexity(request.TestContent),
                ComplexityAfter = CalculateCyclomaticComplexity(transformedFix),
                ReadabilityScoreChange = CalculateReadabilityImprovement(request.TestContent, transformedFix)
            };

            return new FirstTestFix
            {
                OriginalTest = request.TestContent,
                FixedTest = transformedFix,
                Changes = ExtractChanges(request.TestContent, transformedFix),
                Explanation = GenerateFixExplanation(analysis, transformedFix),
                Confidence = CalculateFixConfidence(analysis, transformedFix),
                Metrics = metrics
            };
        }

        public async Task<LearningPath> Generate90DayLearningPathAsync(LearningPathRequest request)
        {
            var llmService = _llmServiceFactory.GetServiceForTask(
                task: "learning-path",
                strategy: "comprehensive",
                context: $"Target role: {request.TargetRole}");

            var prompt = BuildLearningPathPrompt(request);
            var rawPath = await llmService.GenerateTestCodeAsync(prompt, "Create a learning path");

            return ParseLearningPath(rawPath, request);
        }

        public async Task<InterviewInsights> GenerateInterviewQuestionsAsync(QuestionGenerationRequest request)
        {
            var llmService = request.Style switch
            {
                "collaborative" => _llmServiceFactory.GetService("claude"),
                "technical" => _llmServiceFactory.GetService("deepseek"),
                "cultural" => _llmServiceFactory.GetService("chatgpt"),
                _ => _llmServiceFactory.GetService("chatgpt")
            };

            var prompt = BuildInterviewPrompt(request);
            var questions = await llmService.GenerateTestCodeAsync(prompt, "Generate interview questions");

            return new InterviewInsights
            {
                Questions = ParseQuestions(questions),
                QuestionCategories = CategorizeQuestions(questions),
                FollowUpPrompts = GenerateFollowUps(request),
                TimingSuggestions = EstimateInterviewTiming(questions)
            };
        }

        public async Task<CodebaseInsights> GenerateOnboardingInsightsAsync(CodeAnalysis analysis, FirstImpressionRequest request)
        {
            // Implement insight generation logic
            return new CodebaseInsights
            {
                HighestRiskArea = analysis.TechnicalDebt
                    .OrderByDescending(td => td.RiskLevel)
                    .ThenByDescending(td => td.EffortDays)
                    .FirstOrDefault()?.Description ?? "Unknown",
                Strengths = ExtractStrengths(analysis),
                Weaknesses = ExtractWeaknesses(analysis),
                QuickWinOpportunities = ExtractQuickWins(analysis),
                Recommendations = GenerateRecommendations(analysis)
            };
        }

        private string BuildFixPrompt(FlakyTestAnalysis analysis, FirstFixRequest request)
        {
            return $@"
        Fix this flaky test with a {request.FixStrategy} approach:
        
        TEST:
        {request.TestContent}
        
        OBSERVED FAILURES:
        {string.Join("\n", request.ObservedFailures)}
        
        FAILURE RATE: {request.FailureRatePercent}%
        
        ANALYSIS:
        {analysis.RootCause}
        
        CONTEXT:
        - Environment: {request.EnvironmentContext}
        - Framework: {analysis.TestFramework}
        - Pattern: {analysis.FlakyPattern}
        
        REQUIREMENTS:
        1. Keep the test intent identical
        2. Add proper waiting/retry logic
        3. Improve error messages
        4. Add comments about why the fix works
        5. Don't over-engineer - this is a first fix
        
        Return ONLY the fixed test code with no explanations.
        ";
        }

        private string BuildLearningPathPrompt(LearningPathRequest request)
        {
            return $@"
        Create a {request.TimelineDays}-day learning path for a {request.TargetRole}.
        
        CURRENT SKILLS: {string.Join(", ", request.CurrentSkills)}
        DESIRED SKILLS: {string.Join(", ", request.DesiredSkills)}
        LEARNING STYLE: {request.LearningStyle}
        HOURS/WEEK: {request.HoursPerWeek}
        
        Structure the path with phases, milestones, and resources.
        Focus on practical, hands-on learning.
        Include regular checkpoints and assessments.
        ";
        }

        private string BuildInterviewPrompt(QuestionGenerationRequest request)
        {
            return $@"
        Generate {request.Count} interview questions for a {request.TargetRole} position.
        
        STYLE: {request.Style}
        AUDIENCE: {request.Audience}
        FOCUS AREAS: {string.Join(", ", request.FocusAreas)}
        EXPERIENCE LEVEL: {request.ExperienceLevel}/5
        
        Include a mix of technical, behavioral, and situational questions.
        Provide follow-up questions for deeper probing.
        Include evaluation criteria for each question.
        ";
        }

        // Helper methods
        private async Task<string> TransformAIFixAsync(string rawFix, string originalTest, FlakyTestAnalysis analysis)
        {
            // Implement transformation logic
            return rawFix;
        }

        private int CountWaitStatements(string code) => code.Split("Thread.Sleep", StringSplitOptions.None).Length - 1;

        private int CalculateCyclomaticComplexity(string code) => code.Split(new[] { "if", "while", "for", "case", "catch", "&&", "||", "?" }, StringSplitOptions.None).Length - 1;

        private decimal CalculateReadabilityImprovement(string before, string after) => 0.85m;

        private Change[] ExtractChanges(string before, string after) => Array.Empty<Change>();

        private string GenerateFixExplanation(FlakyTestAnalysis analysis, string fixedTest) => "Fixed race condition by adding proper synchronization.";

        private decimal CalculateFixConfidence(FlakyTestAnalysis analysis, string fixedTest) => 0.92m;

        private Question[] ParseQuestions(string rawQuestions) => Array.Empty<Question>();

        private Dictionary<string, string[]> CategorizeQuestions(string rawQuestions) => new();

        private string[] GenerateFollowUps(QuestionGenerationRequest request) => Array.Empty<string>();

        private TimingSuggestions EstimateInterviewTiming(string questions) => new();

        private LearningPath ParseLearningPath(string rawPath, LearningPathRequest request) => new();

        private string[] ExtractStrengths(CodeAnalysis analysis) => new[] { "Good test coverage", "Clean architecture" };

        private string[] ExtractWeaknesses(CodeAnalysis analysis) => new[] { "Flaky tests", "Technical debt" };

        private string[] ExtractQuickWins(CodeAnalysis analysis) => new[] { "Fix obvious null checks", "Add missing unit tests" };

        private Dictionary<string, string> GenerateRecommendations(CodeAnalysis analysis) => new();
    }
}
