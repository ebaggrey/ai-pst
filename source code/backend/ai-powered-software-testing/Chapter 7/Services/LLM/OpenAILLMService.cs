using Chapter_7.Interfaces;
using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;
using Chapter_7.Settings;
using Microsoft.Extensions.Options;

using System.Text;
using System.Text.Json;

namespace Chapter_7.Services.LLM
{
  
    public class OpenAILLMService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly LLMSettings _settings;
        private readonly ILogger<OpenAILLMService> _logger;

        public OpenAILLMService(
            HttpClient httpClient,
            IOptions<LLMSettings> settings,
            ILogger<OpenAILLMService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
        }

        // Pipeline Generation
        public async Task<DecisionPoint[]> GenerateDecisionPointsAsync(PipelineDefinition pipeline, CodebaseAnalysis codebase)
        {
            var prompt = $"Generate intelligent decision points for a CI/CD pipeline with stages: {string.Join(", ", pipeline.Stages.Select(s => s.Name))}. " +
                        $"Codebase: {codebase.Language}, {codebase.TotalLines} lines, {codebase.TestCoverage * 100}% test coverage.";

            var response = await CallLLMAsync<DecisionPoint[]>(prompt, "decision_points");
            return response ?? Array.Empty<DecisionPoint>();
        }

        public async Task<RecoveryPath[]> GenerateRecoveryPathsAsync(PipelineDefinition pipeline, CodebaseAnalysis codebase)
        {
            var prompt = $"Generate recovery paths for pipeline failures in stages: {string.Join(", ", pipeline.Stages.Select(s => s.Name))}";
            var response = await CallLLMAsync<RecoveryPath[]>(prompt, "recovery_paths");
            return response ?? Array.Empty<RecoveryPath>();
        }

        public async Task<OptimizationSuggestion[]> GenerateOptimizationSuggestionsAsync(PipelineDefinition pipeline, EstimatedMetrics metrics)
        {
            var prompt = $"Suggest optimizations for pipeline with estimated duration {metrics.EstimatedDuration} and cost {metrics.EstimatedCost}";
            var response = await CallLLMAsync<OptimizationSuggestion[]>(prompt, "optimization_suggestions");
            return response ?? Array.Empty<OptimizationSuggestion>();
        }

        public async Task<MonitoringConfiguration> GenerateMonitoringConfigAsync(PipelineDefinition pipeline, TeamPractices practices)
        {
            var prompt = $"Create monitoring configuration for pipeline with {pipeline.Stages.Length} stages. Team practices: {JsonSerializer.Serialize(practices)}";
            var response = await CallLLMAsync<MonitoringConfiguration>(prompt, "monitoring_config");
            return response ?? new MonitoringConfiguration { Metrics = Array.Empty<string>() };
        }

        public async Task<AdaptationGuidance> GenerateAdaptationGuidanceAsync(PipelineDefinition pipeline, CodebaseAnalysis codebase)
        {
            var prompt = $"Provide adaptation guidance for pipeline handling {codebase.Language} codebase";
            var response = await CallLLMAsync<AdaptationGuidance>(prompt, "adaptation_guidance");
            return response ?? new AdaptationGuidance { Recommendations = Array.Empty<string>() };
        }

        public async Task<ConstraintSuggestion[]> ResolveConstraintConflictsAsync(ConflictingConstraint[] conflicts)
        {
            var prompt = $"Resolve these conflicting pipeline constraints: {JsonSerializer.Serialize(conflicts)}";
            var response = await CallLLMAsync<ConstraintSuggestion[]>(prompt, "constraint_suggestions");
            return response ?? Array.Empty<ConstraintSuggestion>();
        }

        // Diagnosis
        public async Task<ParsedFailure> ParseFailureLogsAsync(FailureLogs logs)
        {
            var prompt = $"Parse these pipeline failure logs: {logs.RawLogs.Substring(0, Math.Min(500, logs.RawLogs.Length))}...";
            var response = await CallLLMAsync<ParsedFailure>(prompt, "parsed_failure");
            return response ?? new ParsedFailure { ErrorType = "Unknown", Message = "Failed to parse logs" };
        }

        public async Task<ChangeCorrelation> CorrelateWithChangesAsync(ParsedFailure failure, RecentChanges changes)
        {
            var prompt = $"Correlate failure '{failure.ErrorType}' with recent changes: {JsonSerializer.Serialize(changes.Changes)}";
            var response = await CallLLMAsync<ChangeCorrelation>(prompt, "change_correlation");
            return response ?? new ChangeCorrelation { CorrelationScore = 0.5 };
        }

        public async Task<RemediationStep[]> GenerateRemediationStepsAsync(RootCause rootCause, ParsedFailure failure)
        {
            var prompt = $"Generate remediation steps for root cause: {rootCause.Summary}";
            var response = await CallLLMAsync<RemediationStep[]>(prompt, "remediation_steps");
            return response ?? Array.Empty<RemediationStep>();
        }

        public async Task<PreventionStrategy[]> GeneratePreventionStrategiesAsync(RootCause rootCause, PipelineContext context)
        {
            var prompt = $"Generate prevention strategies for {rootCause.Summary} in pipeline {context.PipelineId}";
            var response = await CallLLMAsync<PreventionStrategy[]>(prompt, "prevention_strategies");
            return response ?? Array.Empty<PreventionStrategy>();
        }

        public async Task<HistoricalFailure[]> FindSimilarHistoricalFailuresAsync(ParsedFailure failure)
        {
            var prompt = $"Find historical failures similar to: {failure.ErrorType} - {failure.Message}";
            var response = await CallLLMAsync<HistoricalFailure[]>(prompt, "historical_failures");
            return response ?? Array.Empty<HistoricalFailure>();
        }

        public async Task<ImpactAssessment> AssessFailureImpactAsync(ParsedFailure failure, PipelineContext context)
        {
            var prompt = $"Assess impact of failure {failure.ErrorType} on pipeline {context.PipelineId}";
            var response = await CallLLMAsync<ImpactAssessment>(prompt, "impact_assessment");
            return response ?? new ImpactAssessment { ImpactScore = 0.5, AffectedComponents = Array.Empty<string>() };
        }

        // Optimization
        public async Task<PerformanceAnalysis> AnalyzeCurrentPerformanceAsync(PipelineMetrics metrics)
        {
            var prompt = $"Analyze pipeline performance: duration {metrics.AverageDuration}, success rate {metrics.SuccessRate * 100}%";
            var response = await CallLLMAsync<PerformanceAnalysis>(prompt, "performance_analysis");
            return response ?? new PerformanceAnalysis { EfficiencyScore = 0.5, Bottlenecks = Array.Empty<Bottleneck>() };
        }

        public async Task<TradeOffAnalysis> EvaluateOptimizationTradeOffsAsync(OptimizationStrategy[] strategies, OptimizationRequest request)
        {
            var prompt = $"Evaluate trade-offs between {strategies.Length} optimization strategies";
            var response = await CallLLMAsync<TradeOffAnalysis>(prompt, "tradeoff_analysis");
            return response ?? new TradeOffAnalysis { TradeOffs = Array.Empty<TradeOff>() };
        }

        public async Task<ImplementationPlan> GenerateImplementationPlanAsync(OptimizationStrategy[] strategies, TradeOffAnalysis tradeOffs, OptimizationConstraints constraints)
        {
            var prompt = $"Generate implementation plan for {strategies.Length} optimization strategies";
            var response = await CallLLMAsync<ImplementationPlan>(prompt, "implementation_plan");
            return response ?? new ImplementationPlan { Steps = Array.Empty<ImplementationStep>() };
        }

        public async Task<ExpectedImprovement[]> CalculateExpectedImprovementsAsync(OptimizationStrategy[] strategies, PipelineMetrics currentMetrics)
        {
            var prompt = $"Calculate expected improvements for {strategies.Length} optimization strategies";
            var response = await CallLLMAsync<ExpectedImprovement[]>(prompt, "expected_improvements");
            return response ?? Array.Empty<ExpectedImprovement>();
        }

        public async Task<RiskAssessment> AssessOptimizationRisksAsync(OptimizationStrategy[] strategies, OptimizationConstraints constraints)
        {
            var prompt = $"Assess risks for {strategies.Length} optimization strategies";
            var response = await CallLLMAsync<RiskAssessment>(prompt, "risk_assessment");
            return response ?? new RiskAssessment { Risks = Array.Empty<Risk>() };
        }

        public async Task<ValidationPlan> GenerateValidationPlanAsync(OptimizationStrategy[] strategies)
        {
            var prompt = $"Generate validation plan for {strategies.Length} optimization strategies";
            var response = await CallLLMAsync<ValidationPlan>(prompt, "validation_plan");
            return response ?? new ValidationPlan { TestCases = Array.Empty<string>() };
        }

        // Prediction
        public async Task<ChangeAnalysis> AnalyzeProposedChangesAsync(ProposedChange[] changes)
        {
            var prompt = $"Analyze these proposed pipeline changes: {JsonSerializer.Serialize(changes)}";
            var response = await CallLLMAsync<ChangeAnalysis>(prompt, "change_analysis");
            return response ?? new ChangeAnalysis { Changes = changes, ImpactScores = new Dictionary<string, double>() };
        }

        public async Task<PatternMatch[]> MatchAgainstHistoricalPatternsAsync(ChangeAnalysis analysis, HistoricalData historicalData)
        {
            var prompt = $"Match changes against {historicalData.Runs?.Length ?? 0} historical pipeline runs";
            var response = await CallLLMAsync<PatternMatch[]>(prompt, "pattern_matches");
            return response ?? Array.Empty<PatternMatch>();
        }

        public async Task<Mitigation[]> GenerateMitigationsAsync(Prediction[] predictions, ProposedChange[] changes)
        {
            var prompt = $"Generate mitigations for {predictions.Length} predicted issues";
            var response = await CallLLMAsync<Mitigation[]>(prompt, "mitigations");
            return response ?? Array.Empty<Mitigation>();
        }

        public async Task<RiskScore[]> CalculateRiskScoresAsync(Prediction[] predictions, HistoricalData historicalData)
        {
            var prompt = $"Calculate risk scores for {predictions.Length} predictions";
            var response = await CallLLMAsync<RiskScore[]>(prompt, "risk_scores");
            return response ?? Array.Empty<RiskScore>();
        }

        public async Task<double[]> CalculatePredictionConfidenceAsync(Prediction[] predictions, PatternMatch[] patternMatches)
        {
            var prompt = $"Calculate confidence scores for {predictions.Length} predictions";
            var response = await CallLLMAsync<double[]>(prompt, "confidence_scores");
            return response ?? predictions.Select(p => p.Probability).ToArray();
        }

        public async Task<RecommendedAction[]> GenerateRecommendedActionsAsync(Prediction[] predictions, RiskScore[] riskScores)
        {
            var prompt = $"Generate recommended actions for {predictions.Length} predictions";
            var response = await CallLLMAsync<RecommendedAction[]>(prompt, "recommended_actions");
            return response ?? Array.Empty<RecommendedAction>();
        }

        public async Task<MonitoringRecommendation[]> GenerateMonitoringRecommendationsAsync(Prediction[] predictions)
        {
            var prompt = $"Generate monitoring recommendations for {predictions.Length} predicted issues";
            var response = await CallLLMAsync<MonitoringRecommendation[]>(prompt, "monitoring_recommendations");
            return response ?? Array.Empty<MonitoringRecommendation>();
        }

        // Adaptation
        public async Task<PipelineDefinition> GetCurrentPipelineAsync()
        {
            var prompt = "Get current pipeline configuration";
            var response = await CallLLMAsync<PipelineDefinition>(prompt, "current_pipeline");
            return response ?? new PipelineDefinition { Stages = Array.Empty<Stage>() };
        }

        public async Task<AdaptationNeeds> DetermineAdaptationNeedsAsync(string changeType, ImpactAssessment assessment, PipelineDefinition currentPipeline)
        {
            var prompt = $"Determine adaptation needs for {changeType} change with impact score {assessment.ImpactScore}";
            var response = await CallLLMAsync<AdaptationNeeds>(prompt, "adaptation_needs");
            return response ?? new AdaptationNeeds { ChangeType = changeType, RequiredModifications = Array.Empty<string>() };
        }

        public async Task<ValidationResult[]> ValidateAdaptationPlanAsync(AdaptationPlan plan, PipelineDefinition currentPipeline)
        {
            var prompt = $"Validate adaptation plan with {plan.Steps?.Length ?? 0} steps";
            var response = await CallLLMAsync<ValidationResult[]>(prompt, "validation_results");
            return response ?? Array.Empty<ValidationResult>();
        }

        public async Task<RollbackPlan> GenerateRollbackPlanAsync(AdaptationPlan plan, PipelineDefinition currentPipeline)
        {
            var prompt = $"Generate rollback plan for adaptation with {plan.Steps?.Length ?? 0} steps";
            var response = await CallLLMAsync<RollbackPlan>(prompt, "rollback_plan");
            return response ?? new RollbackPlan { Steps = Array.Empty<RollbackStep>() };
        }

        public async Task<EffortEstimate> EstimateAdaptationEffortAsync(AdaptationPlan plan, ImpactAssessment assessment)
        {
            var prompt = $"Estimate effort for adaptation plan with {plan.Steps?.Length ?? 0} steps";
            var response = await CallLLMAsync<EffortEstimate>(prompt, "effort_estimate");
            return response ?? new EffortEstimate { Duration = TimeSpan.FromHours(8), Complexity = 3 };
        }

        public async Task<ImplementationStep[]> GenerateImplementationStepsAsync(AdaptationPlan plan)
        {
            var prompt = $"Generate implementation steps for adaptation plan";
            var response = await CallLLMAsync<ImplementationStep[]>(prompt, "implementation_steps");
            return response ?? Array.Empty<ImplementationStep>();
        }

        public async Task<TestingStrategy> GenerateTestingStrategyAsync(AdaptationPlan plan, PipelineDefinition currentPipeline)
        {
            var prompt = $"Generate testing strategy for adaptation plan";
            var response = await CallLLMAsync<TestingStrategy>(prompt, "testing_strategy");
            return response ?? new TestingStrategy { TestTypes = Array.Empty<string>() };
        }

        public async Task<CommunicationPlan> GenerateCommunicationPlanAsync(AdaptationPlan plan, ImpactAssessment assessment)
        {
            var prompt = $"Generate communication plan for adaptation with impact score {assessment.ImpactScore}";
            var response = await CallLLMAsync<CommunicationPlan>(prompt, "communication_plan");
            return response ?? new CommunicationPlan { Stakeholders = Array.Empty<string>() };
        }

        private async Task<T> CallLLMAsync<T>(string prompt, string responseType)
        {
            try
            {
                var requestBody = new
                {
                    model = _settings.Model,
                    messages = new[]
                    {
                    new { role = "system", content = _settings.SystemPrompt },
                    new { role = "user", content = prompt }
                },
                    temperature = _settings.Temperature,
                    max_tokens = _settings.MaxTokens,
                    response_format = new { type = "json_object" }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var llmResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);

                if (llmResponse?.Choices?.FirstOrDefault()?.Message?.Content != null)
                {
                    var result = JsonSerializer.Deserialize<T>(llmResponse.Choices[0].Message.Content);
                    return result;
                }

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling LLM service for {ResponseType}", responseType);
                return default;
            }
        }

        private class OpenAIResponse
        {
            public Choice[] Choices { get; set; }
        }

        private class Choice
        {
            public Message Message { get; set; }
        }

        private class Message
        {
            public string Content { get; set; }
        }
    }
}
