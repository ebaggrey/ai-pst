using Chapter_5.Models.Requests;

namespace Chapter_5.Models.Domain
{
    // Models/Domain/TestedImplementation.cs
    public class TestedImplementation
    {
        public CodeSnippet Implementation { get; set; }
        public TestResult[] TestResults { get; set; }
        public ImplementationAnalysis Analysis { get; set; }
        public bool PassesAllTests { get; set; }
        public double QualityScore { get; set; }
    }

    // Models/Domain/ImplementationAnalysis.cs
    public class ImplementationAnalysis
    {
        public string ImplementationId { get; set; }
        public bool PassesTests { get; set; }
        public double CodeQuality { get; set; }
        public double MaintainabilityScore { get; set; }
        public string[] Issues { get; set; }
    }

    // Models/Domain/RefactoringPlan.cs
    public class RefactoringPlan
    {
        public string Id { get; set; }
        public RefactoringStep[] Steps { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public string? RiskLevel { get; set; }
    }

    // Models/Domain/ChangePrediction.cs
    public class ChangePrediction
    {
        public string Id { get; set; }
        public string Area { get; set; }
        public string Description { get; set; }
        public double Probability { get; set; }
        public double Confidence { get; set; }
        public DateTimeOffset Timeline { get; set; }
    }

    // Models/Domain/FutureTestRecommendation.cs
    public class FutureTestRecommendation
    {
        public string Id { get; set; }
        public string PredictionId { get; set; }
        public string TestType { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string ImplementationEffort { get; set; }
    }

    // Models/ValueObjects/ComplexityMetrics.cs
    public class ComplexityMetrics
    {
        public int CyclomaticComplexity { get; set; }
        public int LinesOfCode { get; set; }
        public int MethodCount { get; set; }
        public int DepthOfInheritance { get; set; }
        public int ClassCoupling { get; set; }  
    }

    // Models/ValueObjects/TestValidation.cs
    public class TestValidation
    {
        public GeneratedTest Test { get; set; }
        public bool IsFailingByDesign { get; set; }
        public FailureDetails FailureDetails { get; set; }
        public List<string> Issues { get; set; } = new List<string>();
    }

    // Models/ValueObjects/FailureDetails.cs
    public class FailureDetails
    {
        public string Expected { get; set; }
        public string Actual { get; set; }
        public string Message { get; set; }
    }

    // Models/ValueObjects/ImplementationSuggestion.cs
    public class ImplementationSuggestion
    {
        public string Id { get; set; }
        public string Approach { get; set; }
        public CodeSnippet CodeSnippet { get; set; }
        public string Explanation { get; set; }
        public string[] Tradeoffs { get; set; }
    }
}
