using Chapter_5.Models.Requests;

namespace Chapter_5.Models.Domain
{
    // Models/Domain/TDDCycle.cs
    public class TDDCycle
    {
        public string Id { get; set; }
        public string Phase { get; set; } // "test", "implement", "refactor"
        public string Description { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public string[] FocusAreas { get; set; }
        public GeneratedTest Test { get; set; }
        public CodeSnippet Implementation { get; set; }
        public RefactoringStep[] RefactoringSteps { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public string Status { get; set; } // "planned", "in-progress", "completed", "failed"
    }

    // Models/Domain/RefactoringStep.cs
    public class RefactoringStep
    {
        public string Id { get; set; }
        public string Type { get; set; } // "extract-method", "rename", "inline", etc.
        public string Description { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string[] SafetyChecks { get; set; }
        public bool RequiresManualReview { get; set; }
        public int Order { get; set; }
    }

    // Models/Domain/RefactoringStepResult.cs
    public class RefactoringStepResult
    {
        public RefactoringStep Step { get; set; }
        public bool Successful { get; set; }
        public string ResultingCode { get; set; }
        public TimeSpan Duration { get; set; }
        public TestResult[] TestResults { get; set; }
        public string FailureReason { get; set; }
        public FailureAnalysis FailureAnalysis { get; set; }
        public bool SafeToContinue { get; set; }

        public string? Result { get; set; } // Summary of the result
        public CodeSnippet? CodeAfterStep { get; set; } // The code after this step
    }

    // Models/Domain/FailureAnalysis.cs
    public class FailureAnalysis
    {
        public string RootCause { get; set; }
        public string[] ContributingFactors { get; set; }
        public string[] SuggestedFixes { get; set; }
        public string ImpactLevel { get; set; } // "low", "medium", "high", "critical"
        public bool CanRetry { get; set; }
        public string[] PrerequisitesForRetry { get; set; }
    }

    // Models/Domain/ImprovementMetrics.cs
    public class ImprovementMetrics
    {
        public double OverallImprovement { get; set; }
        public double MaintainabilityGain { get; set; }
        public double ReadabilityGain { get; set; }
        public double PerformanceChange { get; set; }
        public int ComplexityReduction { get; set; }
        public int LinesOfCodeChange { get; set; }
    }

    // Models/Domain/TestSafetyReport.cs
    public class TestSafetyReport
    {
        public int TotalTests { get; set; }
        public int PassingTests { get; set; }
        public int FailingTests { get; set; }
        public TestCoverage Coverage { get; set; }
        public string[] SafetyIssues { get; set; }
        public bool AllTestsPass { get; set; }
        public TimeSpan TotalTestDuration { get; set; }
    }

    // Models/Domain/TestCoverage.cs
    public class TestCoverage
    {
        public double LineCoverage { get; set; }
        public double BranchCoverage { get; set; }
        public double MethodCoverage { get; set; }
        public string[] UncoveredLines { get; set; }
    }

    // Models/Domain/MaintenanceImpact.cs
    public class MaintenanceImpact
    {
        public double EstimatedMaintenanceCost { get; set; }
        public string[] RiskFactors { get; set; }
        public string[] ImprovementAreas { get; set; }
        public double TechnicalDebtReduction { get; set; }
        public string LongTermSustainability { get; set; } // "poor", "fair", "good", "excellent"
    }

    // Models/Domain/CodeAnalysis.cs
    public class CodeAnalysis
    {
        public string CodeId { get; set; }
        public double Complexity { get; set; }
        public double MaintainabilityIndex { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int LinesOfCode { get; set; }
        public int DepthOfInheritance { get; set; }
        public int ClassCoupling { get; set; }
        public string[] CodeSmells { get; set; }
        public string[] ImprovementOpportunities { get; set; }
    }

    // Models/Domain/RoadmapAnalysis.cs
    public class RoadmapAnalysis
    {
        public ProductRoadmap Roadmap { get; set; }
        public string[] HighPriorityFeatures { get; set; }
        public string[] TechnicalRequirements { get; set; }
        public string[] Dependencies { get; set; }
        public DateTimeOffset EstimatedCompletion { get; set; }
        public string[] RiskAreas { get; set; }

        public RoadmapFeature[] Features { get; set; }
    }

    // Models/Domain/ProductRoadmap.cs
    public class ProductRoadmap
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public RoadmapFeature[] Features { get; set; }
        public Milestone[] Milestones { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }

    // Models/Domain/RoadmapFeature.cs
    public class RoadmapFeature
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; } // "low", "medium", "high", "critical"
        public DateTimeOffset TargetDate { get; set; }
        public string Status { get; set; } // "planned", "in-progress", "completed", "deferred"
        public string[] Dependencies { get; set; }
    }

    // Models/Domain/Milestone.cs
    public class Milestone
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Date { get; set; }
        public string[] Deliverables { get; set; }
        public string Status { get; set; } // "upcoming", "in-progress", "achieved", "delayed"
    }

    // Models/Domain/ConfidenceSummary.cs
    public class ConfidenceSummary
    {
        public double AverageConfidence { get; set; }
        public int HighConfidencePredictions { get; set; } // confidence >= 0.8
        public int MediumConfidencePredictions { get; set; } // confidence >= 0.6
        public int LowConfidencePredictions { get; set; } // confidence < 0.6
        public string OverallReliability { get; set; } // "high", "medium", "low"
    }

    // Models/Domain/ImplementationTimeline.cs
    public class ImplementationTimeline
    {
        public Phase[] Phases { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public Dependency[] Dependencies { get; set; }
        public Risk[] Risks { get; set; }
    }

    // Models/Domain/Phase.cs
    public class Phase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string[] Deliverables { get; set; }
        public string Status { get; set; }
    }

    // Models/Domain/Dependency.cs
    public class Dependency
    {
        public string FromPhase { get; set; }
        public string ToPhase { get; set; }
        public string Type { get; set; } // "finish-start", "start-start", "finish-finish"
        public int LagDays { get; set; }
    }

    // Models/Domain/Risk.cs
    public class Risk
    {
        public string Description { get; set; }
        public string Impact { get; set; } // "low", "medium", "high"
        public string Probability { get; set; } // "low", "medium", "high"
        public string MitigationStrategy { get; set; }
        public string Owner { get; set; }
    }

    // Models/Domain/RiskMitigationStrategy.cs
    public class RiskMitigationStrategy
    {
        public string PredictionId { get; set; }
        public string Strategy { get; set; }
        public string Rationale { get; set; }
        public string[] ImplementationSteps { get; set; }
        public string ExpectedOutcome { get; set; }
        public string Owner { get; set; }
    }

    // Models/Domain/RefactoringHint.cs
    public class RefactoringHint
    {
        public string ImplementationId { get; set; }
        public string Suggestion { get; set; }
        public string Reason { get; set; }
        public string Priority { get; set; } // "low", "medium", "high"
        public string EstimatedEffort { get; set; } // "small", "medium", "large"
    }

    // Models/Domain/RefactoringOpportunity.cs
    public class RefactoringOpportunity
    {
        public string ImplementationId { get; set; }
        public string Area { get; set; } // "maintainability", "performance", "readability", "duplication"
        public string Suggestion { get; set; }
        public double ExpectedImprovement { get; set; }
        public string Effort { get; set; } // "very-low", "low", "medium", "high", "very-high"
        public string Priority { get; set; }
    }

    // Models/Domain/CodeSmellAnalysis.cs
    public class CodeSmellAnalysis
    {
        public string ImplementationId { get; set; }
        public CodeSmell[] CodeSmells { get; set; }
        public double OverallSmellScore { get; set; } // 0-1, lower is better
        public string[] Recommendations { get; set; }
        public string Severity { get; set; } // "none", "low", "medium", "high", "critical"
    }

    // Models/Domain/CodeSmell.cs
    public class CodeSmell
    {
        public string Type { get; set; } // "long-method", "duplicate-code", "complex-condition", etc.
        public string Location { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string FixSuggestion { get; set; }
    }

    // Models/Domain/ConfidenceMetrics.cs
    public class ConfidenceMetrics
    {
        public double TestQuality { get; set; }
        public double ImplementationQuality { get; set; }
        public double RefactoringSafety { get; set; }
        public double OverallConfidence { get; set; }
        public string[] ConfidenceFactors { get; set; }
        public string[] RiskFactors { get; set; }
    }

    // Models/Domain/LearningPoint.cs
    public class LearningPoint
    {
        public string Category { get; set; } // "tdd", "design", "testing", "refactoring"
        public string Title { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }
        public string Impact { get; set; } // "low", "medium", "high"
    }

    // Models/Domain/Example.cs
    public class Example
    {
        public string Scenario { get; set; }
        public string Given { get; set; }
        public string When { get; set; }
        public string Then { get; set; }
        public string ExpectedResult { get; set; }
    }

    // Models/Domain/TestSuite.cs
    public class TestSuite
    {
        public string Name { get; set; }
        public GeneratedTest[] Tests { get; set; }
        public string Framework { get; set; }
        public DateTimeOffset LastRun { get; set; }
        public double PassRate { get; set; }
        public TestCoverage Coverage { get; set; }
    }

    // Models/Domain/AssertionFailure.cs
    public class AssertionFailure
    {
        public string Expected { get; set; }
        public string Actual { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    // Models/Domain/ImplementationDiagnostic.cs
    public class ImplementationDiagnostic
    {
        public string Issue { get; set; }
        public string RootCause { get; set; }
        public string[] FailedTests { get; set; }
        public string[] SuggestedFixes { get; set; }
        public string Severity { get; set; }
    }

    // Models/Domain/RollbackSuggestion.cs
    public class RollbackSuggestion
    {
        public int StepNumber { get; set; }
        public string Reason { get; set; }
        public string CodeState { get; set; }
        public string[] TestsToVerify { get; set; }
        public bool Recommended { get; set; }
    }
}
