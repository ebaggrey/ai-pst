
using Chapter_10.Analysis;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;

namespace Chapter_10.Interfaces
{
    public interface IMetricDesigner
    {
        Task<MetricDesignResult> DesignMetricsAsync(
            ObjectiveAnalysis objectiveAnalysis,
            ActivityValueMapping activityMapping,
            string[] designPrinciples,
            MetricConstraints constraints);
    }

    public class MetricDesignResult
    {
        public DesignedMetric[] Metrics { get; set; }
    }
}

// Interfaces/IHealthScoreCalculator.cs
namespace Chapter_10.Interfaces
{
    public interface IHealthScoreCalculator
    {
        Task<HealthScore> CalculateHealthScoreAsync(
            WeightedMetric[] weightedMetrics,
            HistoricalBaseline[] baselines,
            double confidenceThreshold);
    }

    public class HealthScore
    {
        public double OverallScore { get; set; }
        public ComponentScore[] ComponentScores { get; set; }
        public double Confidence { get; set; }
    }
}

// Interfaces/IQualityPredictor.cs
namespace Chapter_10.Interfaces
{
    public interface IQualityPredictor
    {
        Task<MetricPrediction[]> PredictTrendsAsync(
            CurrentStateAnalysis currentAnalysis,
            DetectedPattern[] patterns,
            int predictionHorizon,
            double[] confidenceIntervals);
    }

    public class DetectedPattern
    {
        public string Type { get; set; }
        public double Strength { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}

// Interfaces/IInsightGenerator.cs
namespace Chapter_10.Interfaces
{
    public interface IInsightGenerator
    {
        Task<RawInsight[]> GenerateInsightsAsync(
            MetricAnalysis metricAnalysis,
            string[] insightTypes,
            InsightContext context);
    }

    public class RawInsight
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public double Confidence { get; set; }
        public string[] AffectedMetrics { get; set; }
    }
}

// Interfaces/IMetricOptimizer.cs
namespace Chapter_10.Interfaces
{
    public interface IMetricOptimizer
    {
        Task<OptimizationRecommendation> OptimizeCollectionAsync(
            MetricValueAnalysis valueAnalysis,
            CollectionCostAnalysis costAnalysis,
            string[] optimizationGoals,
            PreservationRule[] preservationRules);
    }
}

// Supporting interfaces and classes
namespace Chapter_10.Interfaces
{
    public interface IObjectiveAnalyzer
    {
        Task<ObjectiveAnalysis> AnalyzeObjectivesAsync(string[] objectives);
    }

    public interface IActivityMapper
    {
        Task<ActivityValueMapping> MapActivitiesToValueAsync(
            string[] activities,
            string[] objectives);
    }

    public interface IRelationshipDefiner
    {
        Task<MetricRelationship[]> DefineRelationshipsAsync(DesignedMetric[] metrics);
    }

    public interface ICollectionPlanner
    {
        Task<CollectionPlan> CreateCollectionPlanAsync(
            DesignedMetric[] metrics,
            MetricConstraints constraints);
    }

    public interface IInterpretationBuilder
    {
        Task<InterpretationFramework> BuildInterpretationFrameworkAsync(
            DesignedMetric[] metrics,
            string[] objectives);
    }

    public interface IValidationRuleCreator
    {
        Task<ValidationRule[]> CreateValidationRulesAsync(DesignedMetric[] metrics);
    }

    public interface IImplementationGuideGenerator
    {
        Task<ImplementationGuidance> GenerateImplementationGuidanceAsync(
            MetricDesignResult design,
            string[] activities);
    }

    public interface ISuccessCriteriaDefiner
    {
        Task<SuccessCriteria> DefineSuccessCriteriaAsync(
            MetricDesignResult design,
            string[] objectives);
    }

    public interface IMetricNormalizer
    {
        Task<NormalizedMetric[]> NormalizeMetricsAsync(
            MetricValue[] values,
            HistoricalBaseline[] baselines,
            string method);
    }

    public interface IMetricWeighter
    {
        Task<WeightedMetric[]> ApplyWeightingAsync(
            NormalizedMetric[] metrics,
            string strategy,
            HistoricalBaseline[] baselines);
    }

    public interface IFactorIdentifier
    {
        Task<ContributingFactor[]> IdentifyContributingFactorsAsync(
            HealthScore healthScore,
            WeightedMetric[] metrics);
    }

    public interface IHealthImprovementGenerator
    {
        Task<ImprovementRecommendation[]> GenerateHealthImprovementsAsync(
            HealthScore healthScore,
            WeightedMetric[] metrics,
            HistoricalBaseline[] baselines);
    }

    public interface ITrendAnalyzer
    {
        Task<TrendAnalysis> AnalyzeHealthTrendsAsync(
            HealthScore healthScore,
            HistoricalBaseline[] baselines);
    }

    public interface IBenchmarkComparer
    {
        Task<BenchmarkComparison> CompareToBenchmarksAsync(
            HealthScore healthScore,
            HistoricalBaseline[] baselines);
    }

    public interface IAlertDeterminer
    {
        Task<AlertStatus> DetermineAlertStatusAsync(
            HealthScore healthScore,
            HistoricalBaseline[] baselines);
    }

    public interface ICurrentStateAnalyzer
    {
        Task<CurrentStateAnalysis> AnalyzeCurrentStateAsync(
            MetricValue[] currentMetrics,
            HistoricalTrend[] trends);
    }

    public interface IPatternIdentifier
    {
        Task<DetectedPattern[]> IdentifyPatternsAsync(
            HistoricalTrend[] trends,
            int horizon);
    }

    public interface IConfidenceCalculator
    {
        Task<PredictionConfidence> CalculatePredictionConfidenceAsync(
            MetricPrediction[] predictions,
            HistoricalTrend[] trends);
    }

    public interface IInterventionGenerator
    {
        Task<Intervention[]> GenerateInterventionsAsync(
            MetricPrediction[] predictions,
            MetricValue[] currentMetrics,
            HistoricalTrend[] trends);
    }

    public interface IRiskAssessor
    {
        Task<RiskAssessment> AssessPredictionRisksAsync(
            MetricPrediction[] predictions,
            HistoricalTrend[] trends);
    }

    public interface IMonitoringRecommender
    {
        Task<MonitoringRecommendation[]> GenerateMonitoringRecommendationsAsync(
            MetricPrediction[] predictions,
            int horizon);
    }

    public interface IDecisionSupportGenerator
    {
        Task<DecisionSupport> GenerateDecisionSupportAsync(
            MetricPrediction[] predictions,
            Intervention[] interventions);
    }

    public interface IMetricAnalyzer
    {
        Task<MetricAnalysis> AnalyzeMetricsForInsightsAsync(
            MetricValue[] metrics,
            string[] insightTypes);
    }

    public interface IActionabilityFilter
    {
        Task<ActionableInsight[]> FilterForActionabilityAsync(
            RawInsight[] insights,
            double threshold,
            InsightContext context);
    }

    public interface IImpactPrioritizer
    {
        Task<ActionableInsight[]> PrioritizeInsightsByImpactAsync(
            ActionableInsight[] insights,
            InsightContext context);
    }

    public interface IImplementationGuidanceGenerator
    {
        Task<InsightImplementationGuidance> GenerateImplementationGuidanceAsync(
            ActionableInsight[] insights);
    }

    public interface IInsightQualityAssessor
    {
        Task<InsightQuality> AssessInsightQualityAsync(
            ActionableInsight[] insights,
            MetricValue[] metrics);
    }

    public interface IValidationStepGenerator
    {
        Task<ValidationStep[]> GenerateValidationStepsAsync(ActionableInsight[] insights);
    }

    public interface ICommunicationPlanner
    {
        Task<CommunicationPlan> GenerateCommunicationPlanAsync(
            ActionableInsight[] insights,
            InsightContext context);
    }

    public interface IInsightMonitoringPlanner
    {
        Task<InsightMonitoringPlan> CreateInsightMonitoringPlanAsync(
            ActionableInsight[] insights);
    }

    public interface IMetricValueAnalyzer
    {
        Task<MetricValueAnalysis> AnalyzeMetricValueAsync(
            MetricDefinition[] metrics,
            PreservationRule[] rules);
    }

    public interface ICollectionCostAnalyzer
    {
        Task<CollectionCostAnalysis> AnalyzeCollectionCostsAsync(
            MetricDefinition[] metrics,
            ResourceConstraint[] constraints);
    }

    public interface IOptimizationBenefitCalculator
    {
        Task<ExpectedBenefits> CalculateOptimizationBenefitsAsync(
            OptimizationRecommendation optimization,
            MetricValueAnalysis valueAnalysis,
            CollectionCostAnalysis costAnalysis);
    }

    public interface IImplementationPlanGenerator
    {
        Task<ImplementationPlan> GenerateOptimizationImplementationPlanAsync(
            OptimizationRecommendation optimization,
            MetricDefinition[] currentMetrics);
    }

    public interface IPreservationValidator
    {
        Task<PreservationValidation> ValidatePreservationAsync(
            OptimizationRecommendation optimization,
            PreservationRule[] rules,
            MetricValueAnalysis valueAnalysis);
    }

    public interface IOptimizationRiskAssessor
    {
        Task<OptimizationRiskAssessment> AssessOptimizationRisksAsync(
            OptimizationRecommendation optimization,
            MetricValueAnalysis valueAnalysis);
    }

    public interface IOptimizationMonitoringPlanner
    {
        Task<OptimizationMonitoringPlan> CreateOptimizationMonitoringPlanAsync(
            OptimizationRecommendation optimization);
    }

    public interface IContinuousImprovementPlanner
    {
        Task<ContinuousImprovementPlan> GenerateContinuousImprovementPlanAsync(
            OptimizationRecommendation optimization);
    }
}