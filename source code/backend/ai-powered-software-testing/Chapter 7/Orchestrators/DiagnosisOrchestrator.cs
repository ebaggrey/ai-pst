namespace Chapter_7.Orchestrators
{
    using Chapter_7.Interfaces;
    using Chapter_7.Models.Requests;
    using Chapter_7.Models.Responses;
    using Chapter_7.Services.LLM;

    public class DiagnosisOrchestrator : IDiagnosisOrchestrator
    {
        private readonly IPipelineDiagnostician _pipelineDiagnostician;
        private readonly ILLMService _llmService;
        private readonly ILogger<DiagnosisOrchestrator> _logger;

        public DiagnosisOrchestrator(
            IPipelineDiagnostician pipelineDiagnostician,
            ILLMService llmService,
            ILogger<DiagnosisOrchestrator> logger)
        {
            _pipelineDiagnostician = pipelineDiagnostician;
            _llmService = llmService;
            _logger = logger;
        }

        public async Task<DiagnosisResponse> DiagnoseFailureAsync(DiagnosisRequest request)
        {
            // Parse and analyze failure logs using LLM
            var parsedFailure = await _llmService.ParseFailureLogsAsync(request.FailureLogs);

            // Correlate with recent changes using LLM
            var changeCorrelation = await _llmService.CorrelateWithChangesAsync(
                parsedFailure,
                request.RecentChanges);

            // Identify root cause
            var rootCause = await _pipelineDiagnostician.IdentifyRootCauseAsync(
                parsedFailure,
                changeCorrelation,
                request.DiagnosisDepth);

            // Generate remediation steps using LLM if requested
            var remediation = request.IncludeRemediation
                ? await _llmService.GenerateRemediationStepsAsync(rootCause, parsedFailure)
                : Array.Empty<RemediationStep>();

            // Suggest prevention strategies using LLM if requested
            var prevention = request.PreventionStrategies
                ? await _llmService.GeneratePreventionStrategiesAsync(rootCause, request.PipelineContext)
                : Array.Empty<PreventionStrategy>();

            // Find similar historical failures using LLM
            var similarFailures = await _llmService.FindSimilarHistoricalFailuresAsync(parsedFailure);

            // Assess failure impact using LLM
            var impactAssessment = await _llmService.AssessFailureImpactAsync(
                parsedFailure,
                request.PipelineContext);

            return new DiagnosisResponse
            {
                DiagnosisId = Guid.NewGuid().ToString(),
                FailureDetails = parsedFailure,
                RootCause = rootCause,
                Confidence = CalculateDiagnosisConfidence(rootCause, parsedFailure),
                RemediationSteps = remediation,
                PreventionStrategies = prevention,
                SimilarHistoricalFailures = similarFailures,
                ImpactAssessment = impactAssessment
            };
        }

        private double CalculateDiagnosisConfidence(RootCause rootCause, ParsedFailure failure)
        {
            // Simple confidence calculation
            if (rootCause.Component == "Unknown")
                return 0.3;

            if (failure.StackTrace?.Length > 100)
                return 0.9;

            return 0.7;
        }
    }
}
