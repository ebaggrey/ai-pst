using Chapter_5.Models.Domain;

namespace Chapter_5.Models.Requests
{
    // Models/Requests/TDDRequest.cs
    public class TDDRequest
    {
        public UserStory UserStory { get; set; }
        public string TddStyle { get; set; } = "classic";
        public Constraint[] Constraints { get; set; } = Array.Empty<Constraint>();
        public bool GenerateMultipleApproaches { get; set; } = true;
        public int MaxComplexityLevel { get; set; } = 5;
    }

    // Models/Requests/ImplementationRequest.cs
    public class ImplementationRequest
    {
        public GeneratedTest FailingTest { get; set; }
        public FailureDetails FailureDetails { get; set; }
        public string ImplementationStrategy { get; set; } = "simplest-first";
        public Constraint[] Constraints { get; set; } = Array.Empty<Constraint>();
        public bool AllowMultipleImplementations { get; set; } = true;
    }

    // Models/Requests/RefactorRequest.cs
    public class RefactorRequest
    {
        public CodeSnippet WorkingCode { get; set; }
        public TestSuite? TestSuite { get; set; }
        public RefactoringGoal[] RefactoringGoals { get; set; } = Array.Empty<RefactoringGoal>();
        public SafetyMeasures SafetyMeasures { get; set; } = new SafetyMeasures();
        public Constraint[] Constraints { get; set; } = Array.Empty<Constraint>();
    }

    // Models/Requests/FuturePredictionRequest.cs
    public class FuturePredictionRequest
    {
        public CodeSnippet CurrentCode { get; set; }
        public ProductRoadmap ProductRoadmap { get; set; }
        public TimeHorizon TimeHorizon { get; set; } = TimeHorizon.Quarterly;
        public double ConfidenceThreshold { get; set; } = 0.7;
        public string[] FocusAreas { get; set; } = Array.Empty<string>();
    }

    // Models/Responses/TDDCycleResponse.cs
    public class TDDCycleResponse
    {
        public string CycleId { get; set; }
        public string Phase { get; set; }
        public GeneratedTest GeneratedTest { get; set; }
        public ImplementationSuggestion[] ImplementationSuggestions { get; set; }
        public RefactoringHint[] RefactoringHints { get; set; }
        public TimeSpan EstimatedTimeline { get; set; }
        public ConfidenceMetrics ConfidenceMetrics { get; set; }
        public LearningPoint[] LearningPoints { get; set; }
        public string[] NextSteps { get; set; }
    }

    // Models/Responses/ImplementationResponse.cs
    public class ImplementationResponse
    {
        public string CycleId { get; set; }
        public TestedImplementation[] Implementations { get; set; }
        public TestedImplementation RecommendedImplementation { get; set; }
        public TestedImplementation[] AlternativeApproaches { get; set; }
        public CodeSmellAnalysis CodeSmellAnalysis { get; set; }
        public RefactoringOpportunity[] RefactoringOpportunities { get; set; }
        public TDDCycle NextTDDCycle { get; set; }
        public ImplementationDiagnostic Diagnostic { get; set; }
        public string SuggestedNextStep { get; set; }
    }

    // Models/Responses/RefactorResponse.cs
    public class RefactorResponse
    {
        public string CycleId { get; set; }
        public RefactoringStepResult[] CompletedSteps { get; set; }
        public CodeSnippet OriginalCode { get; set; }
        public CodeSnippet RefactoredCode { get; set; }
        public ImprovementMetrics ImprovementMetrics { get; set; }
        public TestSafetyReport TestSafetyReport { get; set; }
        public MaintenanceImpact FutureMaintenanceImpact { get; set; }
        public RefactoringOpportunity[] AdditionalRefactoringOpportunities { get; set; }
        public RefactoringStep FailedStep { get; set; }
        public FailureAnalysis FailureAnalysis { get; set; }
        public RollbackSuggestion RollbackSuggestion { get; set; }
        public bool SafeToContinue { get; set; }
    }

    // Models/Responses/FuturePredictionResponse.cs
    public class FuturePredictionResponse
    {
        public string PredictionId { get; set; }
        public TimeHorizon TimeHorizon { get; set; }
        public ChangePrediction[] ChangePredictions { get; set; }
        public FutureTestRecommendation[] FutureTestRecommendations { get; set; }
        public Dictionary<string, FutureTestRecommendation[]> PrioritizedRecommendations { get; set; }
        public ConfidenceSummary ConfidenceSummary { get; set; }
        public ImplementationTimeline ImplementationTimeline { get; set; }
        public RiskMitigationStrategy[] RiskMitigationStrategies { get; set; }
    }

    // Models/Responses/TDDErrorResponse.cs
    public class TDDErrorResponse
    {
        public string Phase { get; set; }
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public string[] RecoveryStrategy { get; set; }
        public string SuggestedFallback { get; set; }
        public string LearningOpportunity { get; set; }
    }

    // Models/Domain/UserStory.cs
    public class UserStory
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AcceptanceCriteria[] AcceptanceCriteria { get; set; }
        public string[] BusinessRules { get; set; }
        public Example[] Examples { get; set; }
    }

    // Models/Domain/GeneratedTest.cs
    public class GeneratedTest
    {
        public string TestCode { get; set; }
        public string TestFramework { get; set; }
        public string TestName { get; set; }
        public string[] Dependencies { get; set; }
        public bool IsFailingByDesign { get; set; }
        public FailureDetails ExpectedFailure { get; set; }
    }

    // Models/Domain/TestResult.cs
    public class TestResult
    {
        public string TestName { get; set; }
        public bool Passed { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan Duration { get; set; }
        public AssertionFailure[] AssertionFailures { get; set; }
    }

    // Models/Domain/CodeSnippet.cs
    public class CodeSnippet
    {
        public string Id { get; set; }
        public string Language { get; set; } = "csharp";
        public string Code { get; set; }
        public Dependency[] Dependencies { get; set; }
        public ComplexityMetrics ComplexityMetrics { get; set; }
    }

    // Models/Enums/TDDEnums.cs
    public enum TddStyle
    {
        Classic,
        OutsideIn,
        InsideOut,
        London,
        Chicago
    }

    public enum TimeHorizon
    {
        Immediate,
        Weekly,
        Monthly,
        Quarterly,
        Yearly
    }

    public enum ImplementationStrategy
    {
        SimplestFirst,
        MostReadable,
        MostPerformant,
        MostExtensible
    }

    // Models/ValueObjects/Constraint.cs
    public class Constraint
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }

    // Models/ValueObjects/AcceptanceCriteria.cs
    public class AcceptanceCriteria
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string[] TestConditions { get; set; }
    }

    // Models/ValueObjects/RefactoringGoal.cs
    public class RefactoringGoal
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
    }

    // Models/ValueObjects/SafetyMeasures.cs
    public class SafetyMeasures
    {
        public bool PreserveBehavior { get; set; } = true;
        public bool CreateCheckpoints { get; set; } = true;
        public bool SuggestRollbackPoints { get; set; } = true;
        public int MaxStepsWithoutCommit { get; set; } = 5;
    }
}
