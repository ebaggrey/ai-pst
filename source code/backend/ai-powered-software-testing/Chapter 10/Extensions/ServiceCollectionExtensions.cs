

// Extensions/ServiceCollectionExtensions.cs
using Chapter_10.Interfaces;
using Chapter_10.Orchestrators;
using Chapter_10.Services;
using Chapter_10.Services.Analysis;
using MetricsThatMatter.Services;
using MetricsThatMatter.Services.Analysis;

namespace MetricsThatMatter.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMetricsThatMatterServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register all orchestrators
            services.AddScoped<IMetricAnalysisOrchestrator, MetricAnalysisOrchestrator>();
            services.AddScoped<IHealthCalculationOrchestrator, HealthCalculationOrchestrator>();
            services.AddScoped<IPredictionOrchestrator, PredictionOrchestrator>();
            services.AddScoped<IInsightOrchestrator, InsightOrchestrator>();
            services.AddScoped<IOptimizationOrchestrator, OptimizationOrchestrator>();

            // Register core services
            services.AddScoped<IMetricDesigner, MetricDesignerService>();
            services.AddScoped<IHealthScoreCalculator, HealthScoreCalculatorService>();
            services.AddScoped<IQualityPredictor, QualityPredictorService>();
            services.AddScoped<IInsightGenerator, InsightGeneratorService>();
            services.AddScoped<IMetricOptimizer, MetricOptimizerService>();

            // Register analysis services
            services.AddScoped<IObjectiveAnalyzer, ObjectiveAnalyzerService>();
            services.AddScoped<IActivityMapper, ActivityMapperService>();
            services.AddScoped<IRelationshipDefiner, RelationshipDefinerService>();
            services.AddScoped<ICollectionPlanner, CollectionPlannerService>();
            services.AddScoped<IInterpretationBuilder, InterpretationBuilderService>();
            services.AddScoped<IValidationRuleCreator, ValidationRuleCreatorService>();
            services.AddScoped<IImplementationGuideGenerator, ImplementationGuideGeneratorService>();
            services.AddScoped<ISuccessCriteriaDefiner, SuccessCriteriaDefinerService>();

            // Register metric processing services
            services.AddScoped<IMetricNormalizer, MetricNormalizerService>();
            services.AddScoped<IMetricWeighter, MetricWeighterService>();
            services.AddScoped<IFactorIdentifier, FactorIdentifierService>();
            services.AddScoped<IHealthImprovementGenerator, HealthImprovementGeneratorService>();
            services.AddScoped<ITrendAnalyzer, TrendAnalyzerService>();
            services.AddScoped<IBenchmarkComparer, BenchmarkComparerService>();
            services.AddScoped<IAlertDeterminer, AlertDeterminerService>();

            // Register prediction services
            services.AddScoped<ICurrentStateAnalyzer, CurrentStateAnalyzerService>();
            services.AddScoped<IPatternIdentifier, PatternIdentifierService>();
            services.AddScoped<IConfidenceCalculator, ConfidenceCalculatorService>();
            services.AddScoped<IInterventionGenerator, InterventionGeneratorService>();
            services.AddScoped<IRiskAssessor, RiskAssessorService>();
            services.AddScoped<IMonitoringRecommender, MonitoringRecommenderService>();
            services.AddScoped<IDecisionSupportGenerator, DecisionSupportGeneratorService>();

            // Register insight services
            services.AddScoped<IMetricAnalyzer, MetricAnalyzerService>();
            services.AddScoped<IActionabilityFilter, ActionabilityFilterService>();
            services.AddScoped<IImpactPrioritizer, ImpactPrioritizerService>();
            services.AddScoped<IImplementationGuidanceGenerator, ImplementationGuidanceGeneratorService>();
            services.AddScoped<IInsightQualityAssessor, InsightQualityAssessorService>();
            services.AddScoped<IValidationStepGenerator, ValidationStepGeneratorService>();
            services.AddScoped<ICommunicationPlanner, CommunicationPlannerService>();
            services.AddScoped<IInsightMonitoringPlanner, InsightMonitoringPlannerService>();

            // Register optimization services
            services.AddScoped<IMetricValueAnalyzer, MetricValueAnalyzerService>();
            services.AddScoped<ICollectionCostAnalyzer, CollectionCostAnalyzerService>();
            services.AddScoped<IOptimizationBenefitCalculator, OptimizationBenefitCalculatorService>();
            services.AddScoped<IImplementationPlanGenerator, ImplementationPlanGeneratorService>();
            services.AddScoped<IPreservationValidator, PreservationValidatorService>();
            services.AddScoped<IOptimizationRiskAssessor, OptimizationRiskAssessorService>();
            services.AddScoped<IOptimizationMonitoringPlanner, OptimizationMonitoringPlannerService>();
            services.AddScoped<IContinuousImprovementPlanner, ContinuousImprovementPlannerService>();




            // Register configuration for services that need it
            services.AddSingleton<IConfiguration>(configuration);

            return services;
        }
    }
}
