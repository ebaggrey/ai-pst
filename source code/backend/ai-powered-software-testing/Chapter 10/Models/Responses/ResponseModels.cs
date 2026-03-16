
using Chapter_10.Models.Requests;

namespace Chapter_10.Models.Responses
{
    public class MetricDesignResponse
    {
        public string DesignId { get; set; }
        public DesignedMetric[] Metrics { get; set; }
        public MetricRelationship[] Relationships { get; set; }
        public CollectionPlan CollectionPlan { get; set; }
        public InterpretationFramework InterpretationFramework { get; set; }
        public ValidationRule[] ValidationRules { get; set; }
        public ImplementationGuidance ImplementationGuidance { get; set; }
        public SuccessCriteria SuccessCriteria { get; set; }
    }

    public class DesignedMetric
    {
        public string MetricId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Formula { get; set; }
        public string DataSource { get; set; }
        public string[] BusinessObjectives { get; set; }
        public double TargetValue { get; set; }
        public string Unit { get; set; }
        public string[] Dimensions { get; set; }
        public double Weight { get; set; }
        public string Category { get; set; }
    }

    public class MetricRelationship
    {
        public string SourceMetricId { get; set; }
        public string TargetMetricId { get; set; }
        public string RelationshipType { get; set; } // "correlation", "causation", "composition"
        public double Strength { get; set; }
    }

    public class CollectionPlan
    {
        public string[] CollectionSteps { get; set; }
        public string[] Tools { get; set; }
        public string Frequency { get; set; }
        public string ResponsibleTeam { get; set; }
    }

    public class InterpretationFramework
    {
        public string[] InterpretationRules { get; set; }
        public ThresholdDefinition[] Thresholds { get; set; }
        public string[] OutlierHandling { get; set; }
    }

    public class ThresholdDefinition
    {
        public string MetricId { get; set; }
        public double Green { get; set; }
        public double Yellow { get; set; }
        public double Red { get; set; }
    }

    public class ValidationRule
    {
        public string MetricId { get; set; }
        public string Rule { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ImplementationGuidance
    {
        public string[] Prerequisites { get; set; }
        public string[] Steps { get; set; }
        public string[] Pitfalls { get; set; }
        public string[] SuccessFactors { get; set; }
    }

    public class SuccessCriteria
    {
        public string[] QuantitativeCriteria { get; set; }
        public string[] QualitativeCriteria { get; set; }
        public string ReviewPeriod { get; set; }
    }
}

// Models/Responses/HealthScoreResponse.cs
namespace Chapter_10.Models.Responses
{
    public class HealthScoreResponse
    {
        public string HealthScoreId { get; set; }
        public double OverallScore { get; set; }
        public ComponentScore[] ComponentScores { get; set; }
        public double Confidence { get; set; }
        public ContributingFactor[] ContributingFactors { get; set; }
        public ImprovementRecommendation[] Recommendations { get; set; }
        public TrendAnalysis TrendAnalysis { get; set; }
        public BenchmarkComparison BenchmarkComparison { get; set; }
        public AlertStatus AlertStatus { get; set; }
    }

    public class ComponentScore
    {
        public string ComponentName { get; set; }
        public double Score { get; set; }
        public double Weight { get; set; }
        public string Status { get; set; }
    }

    public class ContributingFactor
    {
        public string Factor { get; set; }
        public double Impact { get; set; }
        public string Direction { get; set; } // "positive", "negative"
    }

    public class ImprovementRecommendation
    {
        public string MetricId { get; set; }
        public string Recommendation { get; set; }
        public double Impact { get; set; }
        public string Priority { get; set; }
        public string[] Steps { get; set; }
    }

    public class TrendAnalysis
    {
        public string Direction { get; set; }
        public double Rate { get; set; }
        public string[] Observations { get; set; }
    }

    public class BenchmarkComparison
    {
        public double Percentile { get; set; }
        public double GapToAverage { get; set; }
        public double GapToBest { get; set; }
    }

    public class AlertStatus
    {
        public string Level { get; set; }
        public string[] Alerts { get; set; }
        public string RecommendedAction { get; set; }
    }
}

// Models/Responses/PredictionResponse.cs
namespace Chapter_10.Models.Responses
{
    public class PredictionResponse
    {
        public string PredictionId { get; set; }
        public CurrentStateAnalysis CurrentState { get; set; }
        public MetricPrediction[] Predictions { get; set; }
        public PredictionConfidence PredictionConfidence { get; set; }
        public Intervention[] Interventions { get; set; }
        public RiskAssessment RiskAssessment { get; set; }
        public MonitoringRecommendation[] MonitoringRecommendations { get; set; }
        public DecisionSupport DecisionSupport { get; set; }
    }

    public class CurrentStateAnalysis
    {
        public DateTime AnalysisDate { get; set; }
        public Dictionary<string, double> CurrentValues { get; set; }
        public string[] Strengths { get; set; }
        public string[] Weaknesses { get; set; }
    }

    public class MetricPrediction
    {
        public string MetricId { get; set; }
        public string MetricName { get; set; }
        public DateTime[] Dates { get; set; }
        public double[] Values { get; set; }
        public double[] LowerBound { get; set; }
        public double[] UpperBound { get; set; }
        public string Trend { get; set; }
    }

    public class PredictionConfidence
    {
        public double AverageConfidence { get; set; }
        public Dictionary<string, double> MetricConfidence { get; set; }
        public string[] ConfidenceFactors { get; set; }
    }

    public class Intervention
    {
        public string MetricId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime RecommendedDate { get; set; }
        public double ExpectedImpact { get; set; }
    }

    public class RiskAssessment
    {
        public string OverallRisk { get; set; }
        public string[] IdentifiedRisks { get; set; }
        public string[] MitigationStrategies { get; set; }
    }

    public class MonitoringRecommendation
    {
        public string MetricId { get; set; }
        public string Frequency { get; set; }
        public string Threshold { get; set; }
    }

    public class DecisionSupport
    {
        public string[] DecisionPoints { get; set; }
        public string[] RecommendedActions { get; set; }
        public Dictionary<string, string> Tradeoffs { get; set; }
    }
}

// Models/Responses/InsightResponse.cs
namespace Chapter_10.Models.Responses
{
    public class InsightResponse
    {
        public string InsightId { get; set; }
        public ActionableInsight[] GeneratedInsights { get; set; }
        public InsightImplementationGuidance ImplementationGuidance { get; set; }
        public InsightQuality InsightQuality { get; set; }
        public ValidationStep[] ValidationSteps { get; set; }
        public CommunicationPlan CommunicationPlan { get; set; }
        public InsightMonitoringPlan MonitoringPlan { get; set; }
    }

    public class ActionableInsight
    {
        public string InsightId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public double ActionabilityScore { get; set; }
        public double Impact { get; set; }
        public string[] AffectedMetrics { get; set; }
        public string[] RecommendedActions { get; set; }
        public string Priority { get; set; }
    }

    public class InsightImplementationGuidance
    {
        public string[] Prerequisites { get; set; }
        public string[] Steps { get; set; }
        public string[] Resources { get; set; }
    }

    public class InsightQuality
    {
        public double Accuracy { get; set; }
        public double Relevance { get; set; }
        public double Timeliness { get; set; }
        public string[] Limitations { get; set; }
    }

    public class ValidationStep
    {
        public string Step { get; set; }
        public string Method { get; set; }
        public string ExpectedOutcome { get; set; }
    }

    public class CommunicationPlan
    {
        public string[] Stakeholders { get; set; }
        public string Message { get; set; }
        public string Channel { get; set; }
        public DateTime Timeline { get; set; }
    }

    public class InsightMonitoringPlan
    {
        public string[] Metrics { get; set; }
        public string ReviewFrequency { get; set; }
        public string SuccessCriteria { get; set; }
    }
}

// Models/Responses/OptimizationResponse.cs
namespace Chapter_10.Models.Responses
{
    public class OptimizationResponse
    {
        public string OptimizationId { get; set; }
        public MetricDefinition[] CurrentMetrics { get; set; }
        public OptimizationRecommendation Optimization { get; set; }
        public ExpectedBenefits ExpectedBenefits { get; set; }
        public ImplementationPlan ImplementationPlan { get; set; }
        public PreservationValidation PreservationValidation { get; set; }
        public OptimizationRiskAssessment RiskAssessment { get; set; }
        public OptimizationMonitoringPlan MonitoringPlan { get; set; }
        public ContinuousImprovementPlan ContinuousImprovement { get; set; }
    }

    public class OptimizationRecommendation
    {
        public RecommendedAction[] RecommendedActions { get; set; }
        public MetricConsolidation[] Consolidations { get; set; }
        public string[] DeprecatedMetrics { get; set; }
        public string[] NewMetrics { get; set; }
    }

    public class RecommendedAction
    {
        public string MetricId { get; set; }
        public string Action { get; set; } // "keep", "modify", "remove", "add"
        public string Rationale { get; set; }
        public double CostSaving { get; set; }
    }

    public class MetricConsolidation
    {
        public string[] SourceMetricIds { get; set; }
        public string TargetMetricId { get; set; }
        public string ConsolidationMethod { get; set; }
    }

    public class ExpectedBenefits
    {
        public double CostReduction { get; set; }
        public double EfficiencyGain { get; set; }
        public double QualityImprovement { get; set; }
        public string[] AdditionalBenefits { get; set; }
    }

    public class ImplementationPlan
    {
        public string[] Phases { get; set; }
        public string Timeline { get; set; }
        public string[] Dependencies { get; set; }
        public string[] RollbackSteps { get; set; }
    }

    public class PreservationValidation
    {
        public bool IsPreserved { get; set; }
        public string[] ValidatedRules { get; set; }
        public string[] Warnings { get; set; }
    }

    public class OptimizationRiskAssessment
    {
        public string[] Risks { get; set; }
        public string[] Mitigations { get; set; }
    }

    public class OptimizationMonitoringPlan
    {
        public string[] KeyIndicators { get; set; }
        public string ReviewFrequency { get; set; }
    }

    public class ContinuousImprovementPlan
    {
        public string[] ReviewCycles { get; set; }
        public string[] FeedbackLoops { get; set; }
    }
}