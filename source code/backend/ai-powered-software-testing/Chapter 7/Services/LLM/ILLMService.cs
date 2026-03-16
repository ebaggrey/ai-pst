using Chapter_7.Interfaces;
using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;

namespace Chapter_7.Services.LLM
{
    public interface ILLMService
    {
        // Pipeline Generation
        Task<DecisionPoint[]> GenerateDecisionPointsAsync(PipelineDefinition pipeline, CodebaseAnalysis codebase);
        Task<RecoveryPath[]> GenerateRecoveryPathsAsync(PipelineDefinition pipeline, CodebaseAnalysis codebase);
        Task<OptimizationSuggestion[]> GenerateOptimizationSuggestionsAsync(PipelineDefinition pipeline, EstimatedMetrics metrics);
        Task<MonitoringConfiguration> GenerateMonitoringConfigAsync(PipelineDefinition pipeline, TeamPractices practices);
        Task<AdaptationGuidance> GenerateAdaptationGuidanceAsync(PipelineDefinition pipeline, CodebaseAnalysis codebase);
        Task<ConstraintSuggestion[]> ResolveConstraintConflictsAsync(ConflictingConstraint[] conflicts);

        // Diagnosis
        Task<ParsedFailure> ParseFailureLogsAsync(FailureLogs logs);
        Task<ChangeCorrelation> CorrelateWithChangesAsync(ParsedFailure failure, RecentChanges changes);
        Task<RemediationStep[]> GenerateRemediationStepsAsync(RootCause rootCause, ParsedFailure failure);
        Task<PreventionStrategy[]> GeneratePreventionStrategiesAsync(RootCause rootCause, PipelineContext context);
        Task<HistoricalFailure[]> FindSimilarHistoricalFailuresAsync(ParsedFailure failure);
        Task<ImpactAssessment> AssessFailureImpactAsync(ParsedFailure failure, PipelineContext context);

        // Optimization
        Task<PerformanceAnalysis> AnalyzeCurrentPerformanceAsync(PipelineMetrics metrics);
        Task<TradeOffAnalysis> EvaluateOptimizationTradeOffsAsync(OptimizationStrategy[] strategies, OptimizationRequest request);
        Task<ImplementationPlan> GenerateImplementationPlanAsync(OptimizationStrategy[] strategies, TradeOffAnalysis tradeOffs, OptimizationConstraints constraints);
        Task<ExpectedImprovement[]> CalculateExpectedImprovementsAsync(OptimizationStrategy[] strategies, PipelineMetrics currentMetrics);
        Task<RiskAssessment> AssessOptimizationRisksAsync(OptimizationStrategy[] strategies, OptimizationConstraints constraints);
        Task<ValidationPlan> GenerateValidationPlanAsync(OptimizationStrategy[] strategies);

        // Prediction
        Task<ChangeAnalysis> AnalyzeProposedChangesAsync(ProposedChange[] changes);
        Task<PatternMatch[]> MatchAgainstHistoricalPatternsAsync(ChangeAnalysis analysis, HistoricalData historicalData);
        Task<Mitigation[]> GenerateMitigationsAsync(Prediction[] predictions, ProposedChange[] changes);
        Task<RiskScore[]> CalculateRiskScoresAsync(Prediction[] predictions, HistoricalData historicalData);
        Task<double[]> CalculatePredictionConfidenceAsync(Prediction[] predictions, PatternMatch[] patternMatches);
        Task<RecommendedAction[]> GenerateRecommendedActionsAsync(Prediction[] predictions, RiskScore[] riskScores);
        Task<MonitoringRecommendation[]> GenerateMonitoringRecommendationsAsync(Prediction[] predictions);

        // Adaptation
        Task<PipelineDefinition> GetCurrentPipelineAsync();
        Task<AdaptationNeeds> DetermineAdaptationNeedsAsync(string changeType, ImpactAssessment assessment, PipelineDefinition currentPipeline);
        Task<ValidationResult[]> ValidateAdaptationPlanAsync(AdaptationPlan plan, PipelineDefinition currentPipeline);
        Task<RollbackPlan> GenerateRollbackPlanAsync(AdaptationPlan plan, PipelineDefinition currentPipeline);
        Task<EffortEstimate> EstimateAdaptationEffortAsync(AdaptationPlan plan, ImpactAssessment assessment);
        Task<ImplementationStep[]> GenerateImplementationStepsAsync(AdaptationPlan plan);
        Task<TestingStrategy> GenerateTestingStrategyAsync(AdaptationPlan plan, PipelineDefinition currentPipeline);
        Task<CommunicationPlan> GenerateCommunicationPlanAsync(AdaptationPlan plan, ImpactAssessment assessment);
    }
}
