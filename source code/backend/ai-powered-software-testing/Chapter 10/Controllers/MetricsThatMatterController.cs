using Chapter_10.Exceptions;
using Chapter_10.Interfaces;
using Chapter_10.Models.Errors;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;
using Chapter_10.Orchestrators;

using Microsoft.AspNetCore.Mvc;



namespace Chapter_10.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/metrics-that-matter")]
    [Produces("application/json")]
    public class MetricsThatMatterController : ControllerBase
    {
        private readonly IMetricDesigner _metricDesigner;
        private readonly IHealthScoreCalculator _healthScoreCalculator;
        private readonly IQualityPredictor _qualityPredictor;
        private readonly IInsightGenerator _insightGenerator;
        private readonly IMetricOptimizer _metricOptimizer;
        private readonly IMetricAnalysisOrchestrator _metricAnalysisOrchestrator;
        private readonly IHealthCalculationOrchestrator _healthCalculationOrchestrator;
        private readonly IPredictionOrchestrator _predictionOrchestrator;
        private readonly IInsightOrchestrator _insightOrchestrator;
        private readonly IOptimizationOrchestrator _optimizationOrchestrator;
        private readonly ILogger<MetricsThatMatterController> _logger;

        public MetricsThatMatterController(
            IMetricDesigner metricDesigner,
            IHealthScoreCalculator healthScoreCalculator,
            IQualityPredictor qualityPredictor,
            IInsightGenerator insightGenerator,
            IMetricOptimizer metricOptimizer,
            IMetricAnalysisOrchestrator metricAnalysisOrchestrator,
            IHealthCalculationOrchestrator healthCalculationOrchestrator,
            IPredictionOrchestrator predictionOrchestrator,
            IInsightOrchestrator insightOrchestrator,
            IOptimizationOrchestrator optimizationOrchestrator,
            ILogger<MetricsThatMatterController> logger)
        {
            _metricDesigner = metricDesigner;
            _healthScoreCalculator = healthScoreCalculator;
            _qualityPredictor = qualityPredictor;
            _insightGenerator = insightGenerator;
            _metricOptimizer = metricOptimizer;
            _metricAnalysisOrchestrator = metricAnalysisOrchestrator;
            _healthCalculationOrchestrator = healthCalculationOrchestrator;
            _predictionOrchestrator = predictionOrchestrator;
            _insightOrchestrator = insightOrchestrator;
            _optimizationOrchestrator = optimizationOrchestrator;
            _logger = logger;
        }

        [HttpPost("design-metrics")]
        [ProducesResponseType(typeof(MetricDesignResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DesignImpactfulMetrics([FromBody] MetricDesignRequest request)
        {
            // Validate we're not designing too many metrics
            if (request.BusinessObjectives.Length > 10)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "objective-overload",
                    Message = "Too many business objectives for meaningful metric design",
                    RecoverySteps = new[]
                    {
                        "Focus on 3-5 key objectives",
                        "Group related objectives",
                        "Prioritize by business impact"
                    },
                    FallbackSuggestion = "Use standard metric framework with objective mapping"
                });
            }

            if (request.Constraints.MaxMetrics < 3)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "insufficient-metrics",
                    Message = "Need at least 3 metrics for meaningful measurement",
                    RecoverySteps = new[]
                    {
                        "Increase max metrics to 5-7",
                        "Accept limited coverage",
                        "Focus on critical areas only"
                    },
                    FallbackSuggestion = "Use minimum viable metric set"
                });
            }

            try
            {
                _logger.LogInformation(
                    "Designing metrics for {ObjectiveCount} business objectives with {PrincipleCount} design principles",
                    request.BusinessObjectives.Length,
                    request.DesignPrinciples.Length);

                // Analyze business objectives for measurable outcomes
                var objectiveAnalysis = await _metricAnalysisOrchestrator
                    .AnalyzeObjectivesForMeasurementAsync(request.BusinessObjectives);

                // Map testing activities to business value
                var activityMapping = await _metricAnalysisOrchestrator
                    .MapActivitiesToValueAsync(request.TestingActivities, request.BusinessObjectives);

                // Design impactful metrics
                var metricDesign = await _metricDesigner.DesignMetricsAsync(
                    objectiveAnalysis,
                    activityMapping,
                    request.DesignPrinciples,
                    request.Constraints);

                // Define relationships between metrics
                var relationships = await _metricAnalysisOrchestrator
                    .DefineMetricRelationshipsAsync(metricDesign.Metrics);

                // Create collection plan
                var collectionPlan = await _metricAnalysisOrchestrator
                    .CreateCollectionPlanAsync(metricDesign.Metrics, request.Constraints);

                // Build interpretation framework
                var interpretationFramework = await _metricAnalysisOrchestrator
                    .BuildInterpretationFrameworkAsync(metricDesign.Metrics, request.BusinessObjectives);

                var response = new MetricDesignResponse
                {
                    DesignId = Guid.NewGuid().ToString(),
                    Metrics = metricDesign.Metrics,
                    Relationships = relationships,
                    CollectionPlan = collectionPlan,
                    InterpretationFramework = interpretationFramework,
                    ValidationRules = await _metricAnalysisOrchestrator
                        .CreateValidationRulesAsync(metricDesign.Metrics),
                    ImplementationGuidance = await _metricAnalysisOrchestrator
                        .GenerateImplementationGuidanceAsync(metricDesign, request.TestingActivities),
                    SuccessCriteria = await _metricAnalysisOrchestrator
                        .DefineSuccessCriteriaAsync(metricDesign, request.BusinessObjectives)
                };

                _logger.LogInformation(
                    "Designed {MetricCount} metrics with {RelationshipCount} relationships",
                    metricDesign.Metrics.Length, relationships.Length);

                return Ok(response);
            }
            catch (ObjectiveAmbiguityException oaex)
            {
                _logger.LogWarning(oaex, "Business objectives too ambiguous for metric design");
                throw; // Let middleware handle it
            }
        }

        [HttpPost("calculate-health")]
        [ProducesResponseType(typeof(HealthScoreResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CalculateTestingHealthScore([FromBody] HealthScoreRequest request)
        {
            if (request.MetricValues.Length == 0)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "missing-metrics",
                    Message = "Need metric values to calculate health score",
                    RecoverySteps = new[] { "Provide metric values", "Check data collection" }
                });
            }

            if (request.HistoricalBaselines.Length < 10)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "insufficient-history",
                    Message = "Need sufficient historical data for meaningful baseline comparison",
                    RecoverySteps = new[]
                    {
                        "Collect more historical data",
                        "Use industry benchmarks",
                        "Reduce confidence requirements"
                    }
                });
            }

            try
            {
                _logger.LogInformation(
                    "Calculating health score from {MetricCount} metrics with {BaselineCount} historical baselines",
                    request.MetricValues.Length,
                    request.HistoricalBaselines.Length);

                // Normalize metric values
                var normalizedMetrics = await _healthCalculationOrchestrator
                    .NormalizeMetricsAsync(request.MetricValues, request.HistoricalBaselines, request.NormalizationMethod);

                // Apply weighting strategy
                var weightedMetrics = await _healthCalculationOrchestrator
                    .ApplyWeightingAsync(normalizedMetrics, request.WeightingStrategy, request.HistoricalBaselines);

                // Calculate overall health score
                var healthScore = await _healthScoreCalculator.CalculateHealthScoreAsync(
                    weightedMetrics,
                    request.HistoricalBaselines,
                    request.ConfidenceThreshold);

                // Identify contributing factors
                var contributingFactors = await _healthCalculationOrchestrator
                    .IdentifyContributingFactorsAsync(healthScore, weightedMetrics);

                // Generate improvement recommendations
                var recommendations = await _healthCalculationOrchestrator
                    .GenerateHealthImprovementsAsync(healthScore, weightedMetrics, request.HistoricalBaselines);

                var response = new HealthScoreResponse
                {
                    HealthScoreId = Guid.NewGuid().ToString(),
                    OverallScore = healthScore.OverallScore,
                    ComponentScores = healthScore.ComponentScores,
                    Confidence = healthScore.Confidence,
                    ContributingFactors = contributingFactors,
                    Recommendations = recommendations,
                    TrendAnalysis = await _healthCalculationOrchestrator
                        .AnalyzeHealthTrendsAsync(healthScore, request.HistoricalBaselines),
                    BenchmarkComparison = await _healthCalculationOrchestrator
                        .CompareToBenchmarksAsync(healthScore, request.HistoricalBaselines),
                    AlertStatus = await _healthCalculationOrchestrator
                        .DetermineAlertStatusAsync(healthScore, request.HistoricalBaselines)
                };

                _logger.LogInformation(
                    "Health score calculated: {OverallScore}/100 with {Confidence} confidence",
                    healthScore.OverallScore, healthScore.Confidence);

                return Ok(response);
            }
            catch (BaselineInconsistencyException)
            {
                _logger.LogWarning("Historical baselines inconsistent");
                throw;
            }
        }

        [HttpPost("predict-trends")]
        [ProducesResponseType(typeof(PredictionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PredictQualityTrends([FromBody] PredictionRequest request)
        {
            if (request.CurrentMetrics == null || request.CurrentMetrics.Length == 0)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "missing-metrics",
                    Message = "Need current metrics for prediction",
                    RecoverySteps = new[] { "Provide current metrics", "Check data availability" }
                });
            }

            if (request.HistoricalTrends.Length < 30)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "insufficient-history",
                    Message = "Need at least 30 historical data points for trend prediction",
                    RecoverySteps = new[]
                    {
                        "Collect more historical data",
                        "Use shorter prediction horizon",
                        "Apply alternative forecasting methods"
                    }
                });
            }

            try
            {
                _logger.LogInformation(
                    "Predicting quality trends for {PredictionHorizon} days with {ConfidenceLevels} confidence levels",
                    request.PredictionHorizon,
                    string.Join(", ", request.ConfidenceIntervals ?? new double[] { 0.95 }));

                // Analyze current state
                var currentAnalysis = await _predictionOrchestrator
                    .AnalyzeCurrentStateAsync(request.CurrentMetrics, request.HistoricalTrends);

                // Identify patterns and seasonality
                var patterns = await _predictionOrchestrator
                    .IdentifyPatternsAsync(request.HistoricalTrends, request.PredictionHorizon);

                // Generate predictions
                var predictions = await _qualityPredictor.PredictTrendsAsync(
                    currentAnalysis,
                    patterns,
                    request.PredictionHorizon,
                    request.ConfidenceIntervals ?? new double[] { 0.95 });

                // Generate interventions if requested
                var interventions = request.IncludeInterventions
                    ? await _predictionOrchestrator.GenerateInterventionsAsync(
                        predictions, request.CurrentMetrics, request.HistoricalTrends)
                    : Array.Empty<Intervention>();

                // Calculate prediction confidence
                var predictionConfidence = await _predictionOrchestrator
                    .CalculatePredictionConfidenceAsync(predictions, request.HistoricalTrends);

                var response = new PredictionResponse
                {
                    PredictionId = Guid.NewGuid().ToString(),
                    CurrentState = currentAnalysis,
                    Predictions = predictions,
                    PredictionConfidence = predictionConfidence,
                    Interventions = interventions,
                    RiskAssessment = await _predictionOrchestrator
                        .AssessPredictionRisksAsync(predictions, request.HistoricalTrends),
                    MonitoringRecommendations = await _predictionOrchestrator
                        .GenerateMonitoringRecommendationsAsync(predictions, request.PredictionHorizon),
                    DecisionSupport = await _predictionOrchestrator
                        .GenerateDecisionSupportAsync(predictions, interventions)
                };

                _logger.LogInformation(
                    "Generated {PredictionCount} predictions with average confidence {AverageConfidence}",
                    predictions.Length,
                    predictionConfidence.AverageConfidence);

                return Ok(response);
            }
            catch (PatternDetectionException)
            {
                _logger.LogWarning("Pattern detection failed for prediction");
                throw;
            }
        }

        [HttpPost("generate-insights")]
        [ProducesResponseType(typeof(InsightResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GenerateActionableInsights([FromBody] InsightRequest request)
        {
            if (request.Metrics.Length == 0)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "missing-metrics",
                    Message = "Need metrics to generate insights",
                    RecoverySteps = new[] { "Provide metrics", "Check data collection" }
                });
            }

            if (request.ActionabilityThreshold < 0.5 || request.ActionabilityThreshold > 0.95)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "invalid-threshold",
                    Message = "Actionability threshold must be between 0.5 and 0.95",
                    RecoverySteps = new[]
                    {
                        "Set threshold between 0.5 and 0.95",
                        "Use default threshold of 0.7"
                    }
                });
            }

            try
            {
                _logger.LogInformation(
                    "Generating insights from {MetricCount} metrics with {InsightTypeCount} insight types",
                    request.Metrics.Length,
                    request.InsightTypes?.Length ?? 0);

                // Analyze metrics for patterns and anomalies
                var metricAnalysis = await _insightOrchestrator
                    .AnalyzeMetricsForInsightsAsync(request.Metrics, request.InsightTypes ?? new[] { "all" });

                // Generate insights
                var rawInsights = await _insightGenerator.GenerateInsightsAsync(
                    metricAnalysis,
                    request.InsightTypes ?? new[] { "all" },
                    request.Context ?? new InsightContext());

                // Filter for actionability
                var actionableInsights = await _insightOrchestrator
                    .FilterForActionabilityAsync(rawInsights, request.ActionabilityThreshold, request.Context);

                // Prioritize insights by impact
                var prioritizedInsights = await _insightOrchestrator
                    .PrioritizeInsightsByImpactAsync(actionableInsights, request.Context);

                // Generate implementation guidance
                var implementationGuidance = await _insightOrchestrator
                    .GenerateImplementationGuidanceAsync(prioritizedInsights);

                var response = new InsightResponse
                {
                    InsightId = Guid.NewGuid().ToString(),
                    GeneratedInsights = prioritizedInsights,
                    ImplementationGuidance = implementationGuidance,
                    InsightQuality = await _insightOrchestrator
                        .AssessInsightQualityAsync(prioritizedInsights, request.Metrics),
                    ValidationSteps = await _insightOrchestrator
                        .GenerateValidationStepsAsync(prioritizedInsights),
                    CommunicationPlan = await _insightOrchestrator
                        .GenerateCommunicationPlanAsync(prioritizedInsights, request.Context),
                    MonitoringPlan = await _insightOrchestrator
                        .CreateInsightMonitoringPlanAsync(prioritizedInsights)
                };

                _logger.LogInformation(
                    "Generated {InsightCount} actionable insights with average actionability {AverageActionability}",
                    prioritizedInsights.Length,
                    prioritizedInsights.Average(i => i.ActionabilityScore));

                return Ok(response);
            }
            catch (InsightGenerationException)
            {
                _logger.LogWarning("Insight generation failed");
                throw;
            }
        }

        [HttpPost("optimize-collection")]
        [ProducesResponseType(typeof(OptimizationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MetricErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> OptimizeMetricCollection([FromBody] OptimizationRequest request)
        {
            if (request.CurrentMetrics.Length == 0)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "missing-metrics",
                    Message = "Need current metrics to optimize",
                    RecoverySteps = new[] { "Provide current metrics", "Define metrics first" }
                });
            }

            if (request.ResourceConstraints.Length == 0)
            {
                return BadRequest(new MetricErrorResponse
                {
                    ErrorType = "missing-constraints",
                    Message = "Need resource constraints for optimization",
                    RecoverySteps = new[]
                    {
                        "Define resource constraints",
                        "Use default constraints",
                        "Set realistic limits"
                    }
                });
            }

            try
            {
                _logger.LogInformation(
                    "Optimizing collection for {MetricCount} metrics with {ConstraintCount} resource constraints",
                    request.CurrentMetrics.Length,
                    request.ResourceConstraints.Length);

                // Analyze current metric value
                var metricValueAnalysis = await _optimizationOrchestrator
                    .AnalyzeMetricValueAsync(request.CurrentMetrics, request.PreservationRules ?? Array.Empty<PreservationRule>());

                // Analyze collection costs
                var collectionCostAnalysis = await _optimizationOrchestrator
                    .AnalyzeCollectionCostsAsync(request.CurrentMetrics, request.ResourceConstraints);

                // Generate optimization recommendations
                var optimization = await _metricOptimizer.OptimizeCollectionAsync(
                    metricValueAnalysis,
                    collectionCostAnalysis,
                    request.OptimizationGoals ?? new[] { "efficiency" },
                    request.PreservationRules ?? Array.Empty<PreservationRule>());

                // Calculate expected benefits
                var expectedBenefits = await _optimizationOrchestrator
                    .CalculateOptimizationBenefitsAsync(optimization, metricValueAnalysis, collectionCostAnalysis);

                // Generate implementation plan
                var implementationPlan = await _optimizationOrchestrator
                    .GenerateOptimizationImplementationPlanAsync(optimization, request.CurrentMetrics);

                // Validate optimization preserves value
                var preservationValidation = await _optimizationOrchestrator
                    .ValidatePreservationAsync(optimization, request.PreservationRules ?? Array.Empty<PreservationRule>(), metricValueAnalysis);

                var response = new OptimizationResponse
                {
                    OptimizationId = Guid.NewGuid().ToString(),
                    CurrentMetrics = request.CurrentMetrics,
                    Optimization = optimization,
                    ExpectedBenefits = expectedBenefits,
                    ImplementationPlan = implementationPlan,
                    PreservationValidation = preservationValidation,
                    RiskAssessment = await _optimizationOrchestrator
                        .AssessOptimizationRisksAsync(optimization, metricValueAnalysis),
                    MonitoringPlan = await _optimizationOrchestrator
                        .CreateOptimizationMonitoringPlanAsync(optimization),
                    ContinuousImprovement = await _optimizationOrchestrator
                        .GenerateContinuousImprovementPlanAsync(optimization)
                };

                _logger.LogInformation(
                    "Optimization complete: {ActionsRecommended} actions, estimated {CostReduction}% cost reduction",
                    optimization.RecommendedActions?.Length ?? 0,
                    (expectedBenefits.CostReduction * 100).ToString("F2"));

                return Ok(response);
            }
            catch (OptimizationConflictException)
            {
                _logger.LogWarning("Optimization goals conflict");
                throw;
            }
        }
    }
}
