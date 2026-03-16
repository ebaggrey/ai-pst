
using Chapter_9.Models.Requests;

namespace Chapter_9.Models.Responses
{
    // Models/Responses/TestingPriorityResponse.cs
   
        public class TestingPriorityResponse
        {
            public string PrioritizationId { get; set; }
            public OptimizedFeature[] Features { get; set; }
            public string[] Reasoning { get; set; }
            public ExpectedROI ExpectedROI { get; set; }
            public ConfidenceScore[] ConfidenceScores { get; set; }
            public TradeOff[] TradeOffs { get; set; }
            public AlternativePrioritization[] NextBestAlternatives { get; set; }
            public ResourceAllocation ResourceAllocation { get; set; }
            public RiskAssessment RiskAssessment { get; set; }
        }

        public class OptimizedFeature : Feature
        {
            public int Priority { get; set; }
            public double PriorityScore { get; set; }
            public double ExpectedValuePerHour { get; set; }
            public double RiskAdjustedValue { get; set; }
            public string[] OptimizationRationale { get; set; }
        }

        public class ExpectedROI
        {
            public double ExpectedValue { get; set; }
            public double ExpectedCost { get; set; }
            public double Ratio { get; set; }
            public TimeSpan ExpectedPaybackPeriod { get; set; }
        }

        public class ConfidenceScore
        {
            public string FeatureId { get; set; }
            public double Score { get; set; }
            public string Rationale { get; set; }
        }

        public class TradeOff
        {
            public string Description { get; set; }
            public string[] Options { get; set; }
            public string Recommended { get; set; }
        }

        public class AlternativePrioritization
        {
            public string Name { get; set; }
            public OptimizedFeature[] Features { get; set; }
            public string Rationale { get; set; }
        }

        public class ResourceAllocation
        {
            public Dictionary<string, double> Allocations { get; set; }
            public Dictionary<string, string> Timeline { get; set; }
        }

        public class RiskAssessment
        {
            public double OverallRiskScore { get; set; }
            public RiskFactor[] RiskFactors { get; set; }
            public MitigationStrategy[] Mitigations { get; set; }
        }

        public class RiskFactor
        {
            public string Name { get; set; }
            public double Probability { get; set; }
            public double Impact { get; set; }
        }

        public class MitigationStrategy
        {
            public string Risk { get; set; }
            public string Strategy { get; set; }
        }

        // Models/Responses/CoverageResponse.cs
        public class CoverageResponse
        {
            public string SuiteId { get; set; }
            public Feature Feature { get; set; }
            public TestCase[] TestCases { get; set; }
            public CoverageMetrics CoverageMetrics { get; set; }
            public ExecutionPlan ExecutionPlan { get; set; }
            public string[] MaintenanceGuidance { get; set; }
            public double ConfidenceAchieved { get; set; }
            public double EfficiencyScore { get; set; }
            public SimplificationOpportunity[] SimplificationOpportunities { get; set; }
        }

        public class CoverageMetrics
        {
            public double FunctionalCoverage { get; set; }
            public double RiskCoverage { get; set; }
            public double ValueCoverage { get; set; }
            public double RequirementCoverage { get; set; }
            public GapAnalysis CoverageGaps { get; set; }
        }

        public class GapAnalysis
        {
            public string[] UntestedRequirements { get; set; }
            public string[] LowCoverageAreas { get; set; }
            public string[] Recommendations { get; set; }
        }

        public class ExecutionPlan
        {
            public TestExecution[] Executions { get; set; }
            public double TotalDurationMinutes { get; set; }
            public string ParallelizationStrategy { get; set; }
        }

        public class TestExecution
        {
            public string TestId { get; set; }
            public DateTime ScheduledTime { get; set; }
            public string Environment { get; set; }
            public string[] Prerequisites { get; set; }
        }

        public class SimplificationOpportunity
        {
            public string TestId { get; set; }
            public string Suggestion { get; set; }
            public double ImpactScore { get; set; }
            public string Implementation { get; set; }
        }

        // Models/Responses/AutomationDecisionResponse.cs
        public class AutomationDecisionResponse
        {
            public string DecisionId { get; set; }
            public TestScenario TestScenario { get; set; }
            public AutomationDecision Decision { get; set; }
            public CostAnalysis CostAnalysis { get; set; }
            public ROIAnalysis ROI { get; set; }
            public AutomationPlan ImplementationPlan { get; set; }
            public MonitoringPlan MonitoringPlan { get; set; }
            public AutomationOption[] AlternativeOptions { get; set; }
            public Timeline ReviewTimeline { get; set; }
        }

        public class AutomationDecision
        {
            public string Decision { get; set; } // "automate", "manual", "hybrid"
            public double Confidence { get; set; }
            public string[] Rationale { get; set; }
            public DecisionFactor[] Factors { get; set; }
        }

        public class CostAnalysis
        {
            public double TotalAutomationCost { get; set; }
            public double TotalManualCost { get; set; }
            public double BreakEvenPoint { get; set; }
            public CostBreakdown AutomationBreakdown { get; set; }
            public CostBreakdown ManualBreakdown { get; set; }
        }

        public class CostBreakdown
        {
            public double Initial { get; set; }
            public double Recurring { get; set; }
            public double Maintenance { get; set; }
        }

        public class ROIAnalysis
        {
            public double ROIValue { get; set; }
            public double PaybackPeriod { get; set; }
            public double NetPresentValue { get; set; }
            public string[] Assumptions { get; set; }
        }

        public class AutomationPlan
        {
            public string[] Phases { get; set; }
            public ResourceRequirement[] Resources { get; set; }
            public Timeline Timeline { get; set; }
        }

        public class ResourceRequirement
        {
            public string ResourceType { get; set; }
            public int Quantity { get; set; }
            public TimeSpan Duration { get; set; }
        }

        public class Timeline
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public Milestone[] Milestones { get; set; }
        }

        public class Milestone
        {
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public string[] Deliverables { get; set; }
        }

        public class MonitoringPlan
        {
            public MetricDefinition[] Metrics { get; set; }
            public AlertThreshold[] Alerts { get; set; }
            public ReviewSchedule Reviews { get; set; }
        }

        public class MetricDefinition
        {
            public string Name { get; set; }
            public string CollectionMethod { get; set; }
            public string Frequency { get; set; }
        }

        public class AlertThreshold
        {
            public string Metric { get; set; }
            public double WarningThreshold { get; set; }
            public double CriticalThreshold { get; set; }
        }

        public class ReviewSchedule
        {
            public string Frequency { get; set; }
            public string Responsible { get; set; }
        }

        public class AutomationOption
        {
            public string Name { get; set; }
            public AutomationDecision Decision { get; set; }
            public string Rationale { get; set; }
        }

        // Models/Responses/MaintenanceOptimizationResponse.cs
        public class MaintenanceOptimizationResponse
        {
            public string OptimizationId { get; set; }
            public TestSuite OriginalTests { get; set; }
            public OptimizationResult Optimization { get; set; }
            public MaintenanceSavings Savings { get; set; }
            public ImplementationGuidance ImplementationGuidance { get; set; }
            public ValidationResult PreservationValidation { get; set; }
            public RiskAssessment RiskAssessment { get; set; }
            public MonitoringPlan MonitoringPlan { get; set; }
            public ImprovementPlan ContinuousImprovement { get; set; }
        }

        public class OptimizationResult
        {
            public OptimizationAction[] Actions { get; set; }
            public TestSuite OptimizedSuite { get; set; }
            public Dictionary<string, double> Metrics { get; set; }
        }

        public class OptimizationAction
        {
            public string Type { get; set; } // "remove", "consolidate", "simplify"
            public string[] AffectedTests { get; set; }
            public string Rationale { get; set; }
            public double Impact { get; set; }
        }

        public class MaintenanceSavings
        {
            public double MaintenanceReduction { get; set; }
            public double CostSavings { get; set; }
            public double TimeSavingsHours { get; set; }
            public SavingsBreakdown Breakdown { get; set; }
        }

        public class SavingsBreakdown
        {
            public double ExecutionSavings { get; set; }
            public double MaintenanceSavings { get; set; }
            public double ReportingSavings { get; set; }
        }

        public class ImplementationGuidance
        {
            public string[] Steps { get; set; }
            public string[] Risks { get; set; }
            public string[] Prerequisites { get; set; }
        }

        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string[] Violations { get; set; }
            public string[] Warnings { get; set; }
        }

        public class ImprovementPlan
        {
            public string[] Recommendations { get; set; }
            public Timeline Timeline { get; set; }
            public string[] SuccessCriteria { get; set; }
        }

        // Models/Responses/ROIAnalysisResponse.cs
        public class ROIAnalysisResponse
        {
            public string AnalysisId { get; set; }
            public TimeSpan MeasurementPeriod { get; set; }
            public TangibleAnalysis TangibleAnalysis { get; set; }
            public IntangibleAnalysis IntangibleAnalysis { get; set; }
            public OverallROI OverallROI { get; set; }
            public Insight[] Insights { get; set; }
            public Recommendation[] Recommendations { get; set; }
            public BenchmarkComparison BenchmarkComparison { get; set; }
            public ROIForecast Forecasting { get; set; }
            public VisualizationData VisualizationData { get; set; }
        }

        public class TangibleAnalysis
        {
            public double TotalCost { get; set; }
            public double TotalBenefit { get; set; }
            public double ROI { get; set; }
            public CostBenefitItem[] CostBreakdown { get; set; }
            public CostBenefitItem[] BenefitBreakdown { get; set; }
        }

        public class CostBenefitItem
        {
            public string Category { get; set; }
            public double Amount { get; set; }
            public double Percentage { get; set; }
        }

        public class IntangibleAnalysis
        {
            public double QualityScore { get; set; }
            public double TeamMorale { get; set; }
            public double CustomerSatisfaction { get; set; }
            public string[] QualitativeBenefits { get; set; }
        }

        public class OverallROI
        {
            public double ROIValue { get; set; }
            public double PaybackPeriod { get; set; }
            public double NetPresentValue { get; set; }
            public string Confidence { get; set; }
        }

        public class Insight
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public double Impact { get; set; }
        }

        public class Recommendation
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public double ExpectedImpact { get; set; }
            public string Priority { get; set; }
        }

        public class BenchmarkComparison
        {
            public double IndustryAverage { get; set; }
            public double BestInClass { get; set; }
            public double Percentile { get; set; }
        }

        public class ROIForecast
        {
            public double ProjectedROI { get; set; }
            public string[] Trends { get; set; }
            public string[] Recommendations { get; set; }
        }

        public class VisualizationData
        {
            public Dictionary<string, double[]> TimeSeries { get; set; }
            public Dictionary<string, double> Distribution { get; set; }
            public string ChartConfig { get; set; }
        }
    }