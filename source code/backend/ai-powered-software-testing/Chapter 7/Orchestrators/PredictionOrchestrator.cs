namespace Chapter_7.Orchestrators
{
    using Chapter_7.Interfaces;
    using Chapter_7.Models.Requests;
    using Chapter_7.Models.Responses;
    using Chapter_7.Services.LLM;

    public class PredictionOrchestrator : IPredictionOrchestrator
    {
        private readonly IPipelinePredictor _pipelinePredictor;
        private readonly ILLMService _llmService;
        private readonly ILogger<PredictionOrchestrator> _logger;

        public PredictionOrchestrator(
            IPipelinePredictor pipelinePredictor,
            ILLMService llmService,
            ILogger<PredictionOrchestrator> logger)
        {
            _pipelinePredictor = pipelinePredictor;
            _llmService = llmService;
            _logger = logger;
        }

        public async Task<PredictionResponse> PredictIssuesAsync(PredictionRequest request)
        {
            // Analyze proposed changes using LLM
            var changeAnalysis = await _llmService.AnalyzeProposedChangesAsync(request.ProposedChanges);

            // Match against historical patterns using LLM
            var patternMatches = await _llmService.MatchAgainstHistoricalPatternsAsync(
                changeAnalysis,
                request.HistoricalData);

            // Generate predictions
            var predictions = await _pipelinePredictor.PredictIssuesAsync(
                changeAnalysis,
                patternMatches,
                request.PredictionHorizon,
                request.ConfidenceThreshold);

            // Generate mitigations if requested using LLM
            var mitigations = request.IncludeMitigations
                ? await _llmService.GenerateMitigationsAsync(predictions, request.ProposedChanges)
                : Array.Empty<Mitigation>();

            // Calculate risk scores using LLM
            var riskScores = await _llmService.CalculateRiskScoresAsync(predictions, request.HistoricalData);

            // Calculate confidence scores using LLM
            var confidenceScores = await _llmService.CalculatePredictionConfidenceAsync(predictions, patternMatches);

            // Generate recommended actions using LLM
            var recommendedActions = await _llmService.GenerateRecommendedActionsAsync(predictions, riskScores);

            // Generate monitoring recommendations using LLM
            var monitoringRecommendations = await _llmService.GenerateMonitoringRecommendationsAsync(predictions);

            return new PredictionResponse
            {
                PredictionId = Guid.NewGuid().ToString(),
                ProposedChanges = request.ProposedChanges,
                Predictions = predictions,
                ConfidenceScores = confidenceScores,
                RiskScores = riskScores,
                Mitigations = mitigations,
                RecommendedActions = recommendedActions,
                MonitoringRecommendations = monitoringRecommendations,
                HistoricalEvidence = patternMatches
            };
        }
    }
}
