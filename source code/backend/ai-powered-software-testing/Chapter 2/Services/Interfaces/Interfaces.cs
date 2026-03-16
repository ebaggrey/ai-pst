using Chapter_2.Models;

namespace Chapter_2.Services.Interfaces
{
    public interface IOnboardingOrchestrator
    {
        Task<FirstTestFix> CreateFirstAIFixAsync(FirstFixRequest request);
        Task<LearningPath> Generate90DayLearningPathAsync(LearningPathRequest request);
        Task<InterviewInsights> GenerateInterviewQuestionsAsync(QuestionGenerationRequest request);
        Task<CodebaseInsights> GenerateOnboardingInsightsAsync(CodeAnalysis analysis, FirstImpressionRequest request);
    }



    public interface ICodebaseAnalyzer
    {
        Task<CodeAnalysis> AnalyzeAsync(FirstImpressionRequest request, TimeSpan timeout);
        Task<CodeAnalysis> GeneratePartialAnalysisAsync(FirstImpressionRequest request);
    }


    public interface ITestPatternRecognizer
    {
        Task<FlakyTestAnalysis> AnalyzeFlakyTestAsync(string testContent, string[] observedFailures);
        Task<TestPattern[]> RecognizePatternsAsync(string codebasePath);
        Task<TestPattern[]> GetRecommendedPatternsAsync(string context);
    }

    public class FlakyTestAnalysis
    {
        public string RootCause { get; set; } = string.Empty;
        public string TestFramework { get; set; } = string.Empty;
        public string FlakyPattern { get; set; } = string.Empty;
        public string TechnicalContext { get; set; } = string.Empty;
        public string[] SuggestedFixes { get; set; } = Array.Empty<string>();
        public decimal Confidence { get; set; }
    }

    public class TestPattern
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string[] UseCases { get; set; } = Array.Empty<string>();
        public string[] ImplementationSteps { get; set; } = Array.Empty<string>();
        public string[] ExampleCode { get; set; } = Array.Empty<string>();
        public string[] Pitfalls { get; set; } = Array.Empty<string>();
    }


    public interface ICodeAnalyzer
    {
        Task<CodeMetrics> AnalyzeFileAsync(string filePath);
        Task<CodeStructure> AnalyzeStructureAsync(string directoryPath);
        Task<DependencyGraph> AnalyzeDependenciesAsync(string filePath);
    }

    public class CodeMetrics
    {
        public int LinesOfCode { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int MaintainabilityIndex { get; set; }
        public decimal TestCoverage { get; set; }
        public int DependenciesCount { get; set; }
    }

   
    public class DependencyGraph
    {
        public Node[] Nodes { get; set; } = Array.Empty<Node>();
        public Edge[] Edges { get; set; } = Array.Empty<Edge>();
    }

    public class Node
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class Edge
    {
        public string SourceId { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
    }



    public interface ILLMService
    {
        string ProviderName { get; }
        Task<string> GenerateTestCodeAsync(string prompt, string context);
        Task<TestGenerationResult> GenerateTestWithAnalysisAsync(string code, string context);
        Task<decimal> GetConfidenceScoreAsync(string code, string test);
    }

    public class TestGenerationResult
    {
        public string GeneratedCode { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string[] Assumptions { get; set; } = Array.Empty<string>();
        public string[] Limitations { get; set; } = Array.Empty<string>();
        public decimal Confidence { get; set; }
    }



    public interface ILearningPathGenerator
    {
        Task<LearningPath> GenerateLearningPathAsync(LearningPathRequest request);
        Task<LearningPath> AdjustLearningPathAsync(string pathId, string[] feedback);
        Task<ProgressReport> GetProgressAsync(string pathId);
    }

    public class ProgressReport
    {
        public string PathId { get; set; } = string.Empty;
        public decimal CompletionPercentage { get; set; }
        public Milestone[] CompletedMilestones { get; set; } = Array.Empty<Milestone>();
        public Milestone[] UpcomingMilestones { get; set; } = Array.Empty<Milestone>();
        public Resource[] RecommendedResources { get; set; } = Array.Empty<Resource>();
    }




}
