
namespace Chapter_9.Models.Requests
{
    public class PriorityRequest
    {
        public Feature[] Features { get; set; }
        public TestingConstraints Constraints { get; set; }
        public PrioritizationMethod PrioritizationMethod { get; set; }
        public double MaxTestingBudget { get; set; }
        public CostOfDelay CostOfDelay { get; set; }
    }

    public class Feature
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double BusinessValue { get; set; }
        public double RiskLevel { get; set; }
        public double ImplementationComplexity { get; set; }
        public double TestingEffort { get; set; }
        public string[] Dependencies { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
    }

    public class TestingConstraints
    {
        public double MaxTimeHours { get; set; }
        public double MaxCost { get; set; }
        public int MaxParallelTests { get; set; }
        public string[] RequiredEnvironments { get; set; }
        public Dictionary<string, string> ComplianceRules { get; set; }
    }

    public class CostOfDelay
    {
        public double DailyCost { get; set; }
        public string DelayType { get; set; } // "user-adoption", "market-window", "compliance"
        public DateTime? Deadline { get; set; }
    }

    public enum PrioritizationMethod
    {
        WeightedShortestJobFirst,
        CostOfDelayDividedByDuration,
        BusinessValueFirst,
        RiskBased,
        MoSCoW
    }

    // Models/Requests/CoverageRequest.cs
    public class CoverageRequest
    {
        public Feature Feature { get; set; }
        public double ConfidenceTarget { get; set; }
        public RiskProfile RiskProfile { get; set; }
        public CoverageConstraints Constraints { get; set; }
        public OptimizationGoal OptimizationGoal { get; set; }
    }

    public class RiskProfile
    {
        public string[] HighRiskAreas { get; set; }
        public string[] CriticalFlows { get; set; }
        public double RiskTolerance { get; set; }
    }

    public class CoverageConstraints
    {
        public int MaxTestCases { get; set; }
        public double MaxExecutionTimeMinutes { get; set; }
        public string[] RequiredCoverageTypes { get; set; }
    }

    public enum OptimizationGoal
    {
        MaximizeCoverage,
        MinimizeTests,
        BalanceRiskAndEffort
    }

    // Models/Requests/AutomationDecisionRequest.cs
    public class AutomationDecisionRequest
    {
        public TestScenario TestScenario { get; set; }
        public AutomationCost AutomationCost { get; set; }
        public ManualCost ManualCost { get; set; }
        public double ROIThreshold { get; set; }
        public DecisionFactor[] DecisionFactors { get; set; }
    }

    public class TestScenario
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ExecutionFrequency { get; set; }
        public double AverageDurationMinutes { get; set; }
        public string[] TestDataRequirements { get; set; }
    }

    public class AutomationCost
    {
        public double InitialCost { get; set; }
        public double MaintenanceCostPerMonth { get; set; }
        public double InfrastructureCost { get; set; }
        public double TrainingCost { get; set; }
    }

    public class ManualCost
    {
        public double ExecutionCost { get; set; }
        public double SetupCost { get; set; }
        public double ReportingCost { get; set; }
    }

    public class DecisionFactor
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public double Score { get; set; }
    }

    // Models/Requests/MaintenanceRequest.cs
    public class MaintenanceRequest
    {
        public TestSuite ExistingTests { get; set; }
        public ChangeImpact ChangeImpact { get; set; }
        public OptimizationStrategy OptimizationStrategy { get; set; }
        public string[] AllowedActions { get; set; }
        public PreservationRule[] PreservationRules { get; set; }
    }

    public class TestSuite
    {
        public string SuiteId { get; set; }
        public TestCase[] TestCases { get; set; }
        public string ApplicationArea { get; set; }
    }

    public class TestCase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Steps { get; set; }
        public string[] CoveredRequirements { get; set; }
        public double ExecutionFrequency { get; set; }
        public DateTime LastExecution { get; set; }
        public double FailureRate { get; set; }
        public double MaintenanceCost { get; set; }
        public double BusinessCriticality { get; set; }
    }

    public class ChangeImpact
    {
        public string[] ChangedComponents { get; set; }
        public string[] AffectedFeatures { get; set; }
        public double ImpactRadius { get; set; }
    }

    public enum OptimizationStrategy
    {
        ConsolidateSimilarTests,
        RemoveRedundantTests,
        SimplifyTestSteps,
        RetireObsoleteTests
    }

    public class PreservationRule
    {
        public string RuleId { get; set; }
        public string Description { get; set; }
        public bool MustPreserve { get; set; }
    }

    // Models/Requests/ROIRequest.cs
    public class ROIRequest
    {
        public TestInvestment[] TestInvestments { get; set; }
        public TestOutcome[] Outcomes { get; set; }
        public TimeSpan MeasurementPeriod { get; set; }
        public string[] CostCategories { get; set; }
        public string[] ValueCategories { get; set; }
        public bool IncludeIntangibles { get; set; }
    }

    public class TestInvestment
    {
        public string Category { get; set; }
        public double Cost { get; set; }
        public DateTime Date { get; set; }
    }

    public class TestOutcome
    {
        public string Category { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // "tangible", "intangible"
    }

    // Models/Responses/TestingPriorityResponse.cs
   
        //public class TestingPriorityResponse
        //{
        //    public string PrioritizationId { get; set; }
        //    public OptimizedFeature[] Features { get; set; }
        //    public string[] Reasoning { get; set; }
        //    public ExpectedROI ExpectedROI { get; set; }
        //    public ConfidenceScore[] ConfidenceScores { get; set; }
        //    public TradeOff[] TradeOffs { get; set; }
        //    public AlternativePrioritization[] NextBestAlternatives { get; set; }
        //    public ResourceAllocation ResourceAllocation { get; set; }
        //    public RiskAssessment RiskAssessment { get; set; }
        //}

       
}
