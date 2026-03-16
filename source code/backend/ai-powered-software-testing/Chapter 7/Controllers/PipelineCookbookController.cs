using Microsoft.AspNetCore.Mvc;
using Chapter_7.Exceptions;
using Chapter_7.Models.Errors;
using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;
using Chapter_7.Orchestrators;

namespace Chapter_7.Controllers
{
 
    [ApiController]
    [Route("api/pipeline-cookbook")]
    [ApiExplorerSettings(GroupName = "pipeline-cookbook")]
    public class PipelineCookbookController : ControllerBase
    {
        private readonly IPipelineOrchestrator _pipelineOrchestrator;
        private readonly IDiagnosisOrchestrator _diagnosisOrchestrator;
        private readonly IOptimizationOrchestrator _optimizationOrchestrator;
        private readonly IPredictionOrchestrator _predictionOrchestrator;
        private readonly IAdaptationOrchestrator _adaptationOrchestrator;
        private readonly ILogger<PipelineCookbookController> _logger;

        public PipelineCookbookController(
            IPipelineOrchestrator pipelineOrchestrator,
            IDiagnosisOrchestrator diagnosisOrchestrator,
            IOptimizationOrchestrator optimizationOrchestrator,
            IPredictionOrchestrator predictionOrchestrator,
            IAdaptationOrchestrator adaptationOrchestrator,
            ILogger<PipelineCookbookController> logger)
        {
            _pipelineOrchestrator = pipelineOrchestrator;
            _diagnosisOrchestrator = diagnosisOrchestrator;
            _optimizationOrchestrator = optimizationOrchestrator;
            _predictionOrchestrator = predictionOrchestrator;
            _adaptationOrchestrator = adaptationOrchestrator;
            _logger = logger;
        }

        [HttpPost("generate-pipeline")]
        [ProducesResponseType(typeof(IntelligentPipelineResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GenerateIntelligentPipeline([FromBody] PipelineGenerationRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Generating intelligent pipeline for {Language} codebase with {TestCoverage}% test coverage",
                    request.CodebaseAnalysis.Language,
                    request.CodebaseAnalysis.TestCoverage * 100);

                var response = await _pipelineOrchestrator.GeneratePipelineAsync(request);
                return Ok(response);
            }
            catch (ConstraintConflictException ex)
            {
                _logger.LogWarning(ex, "Pipeline constraints conflict");
                return Ok(await _pipelineOrchestrator.HandleConstraintConflictAsync(ex, request));
            }
            catch (PipelineComplexityException ex)
            {
                _logger.LogWarning(ex, "Pipeline generation too complex");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new PipelineErrorResponse
                {
                    ErrorType = "complexity-overload",
                    Message = ex.Message,
                    RecoverySteps = new[]
                    {
                    "Simplify codebase structure",
                    "Generate pipeline for subsystems separately",
                    "Provide more specific requirements"
                },
                    FallbackSuggestion = "Manual pipeline design with template assistance"
                });
            }
        }

        [HttpPost("diagnose-failure")]
        [ProducesResponseType(typeof(DiagnosisResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DiagnosePipelineFailure([FromBody] DiagnosisRequest request)
        {
            if (string.IsNullOrEmpty(request.FailureLogs?.RawLogs))
            {
                return BadRequest(new PipelineErrorResponse
                {
                    ErrorType = "missing-logs",
                    Message = "Failure logs are required for diagnosis",
                    RecoverySteps = new[] { "Provide complete failure logs" },
                    FallbackSuggestion = "Manual log inspection"
                });
            }

            try
            {
                _logger.LogInformation(
                    "Diagnosing pipeline failure from {LogLength} bytes of logs",
                    request.FailureLogs.RawLogs.Length);

                var response = await _diagnosisOrchestrator.DiagnoseFailureAsync(request);
                return Ok(response);
            }
            catch (LogParseException ex)
            {
                _logger.LogError(ex, "Failed to parse pipeline failure logs");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new PipelineErrorResponse
                {
                    ErrorType = "log-parsing-error",
                    Message = ex.Message,
                    RecoverySteps = new[]
                    {
                    "Provide logs in standard format",
                    "Include more context around failure",
                    "Try manual diagnosis with error snippets"
                },
                    FallbackSuggestion = "Manual log analysis with pattern matching"
                });
            }
        }

        [HttpPost("optimize-performance")]
        [ProducesResponseType(typeof(OptimizationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> OptimizePipelinePerformance([FromBody] OptimizationRequest request)
        {
            if (request.CurrentMetrics?.AverageDuration == TimeSpan.Zero)
            {
                return BadRequest(new PipelineErrorResponse
                {
                    ErrorType = "missing-metrics",
                    Message = "Need current performance metrics for optimization",
                    RecoverySteps = new[] { "Collect pipeline performance metrics first" },
                    FallbackSuggestion = "Use estimated metrics based on similar pipelines"
                });
            }

            try
            {
                _logger.LogInformation(
                    "Optimizing pipeline with current duration: {Duration}, success rate: {SuccessRate}%",
                    request.CurrentMetrics.AverageDuration,
                    request.CurrentMetrics.SuccessRate * 100);

                var response = await _optimizationOrchestrator.OptimizePerformanceAsync(request);
                return Ok(response);
            }
            catch (OptimizationConflictException ex)
            {
                _logger.LogWarning(ex, "Optimization goals conflict");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new PipelineErrorResponse
                {
                    ErrorType = "goal-conflict",
                    Message = ex.Message,
                    RecoverySteps = new[]
                    {
                    "Prioritize optimization goals",
                    "Relax some constraints",
                    "Optimize in phases"
                },
                    FallbackSuggestion = "Manual optimization with goal balancing"
                });
            }
        }

        [HttpPost("predict-issues")]
        [ProducesResponseType(typeof(PredictionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PredictPipelineIssues([FromBody] PredictionRequest request)
        {
            if (request.ProposedChanges?.Length == 0)
            {
                return BadRequest(new PipelineErrorResponse
                {
                    ErrorType = "missing-changes",
                    Message = "Need proposed changes for prediction",
                    RecoverySteps = new[] { "Specify at least one proposed change" },
                    FallbackSuggestion = "Skip prediction for this change"
                });
            }

            if (request.HistoricalData?.Runs?.Length < 10)
            {
                return BadRequest(new PipelineErrorResponse
                {
                    ErrorType = "insufficient-data",
                    Message = "Need at least 10 historical runs for accurate prediction",
                    RecoverySteps = new[] { "Collect more pipeline run data", "Use alternative prediction method" },
                    FallbackSuggestion = "Manual risk assessment based on change type"
                });
            }

            try
            {
                _logger.LogInformation(
                    "Predicting pipeline issues for {ChangeCount} proposed changes",
                    request.ProposedChanges.Length);

                var response = await _predictionOrchestrator.PredictIssuesAsync(request);
                return Ok(response);
            }
            catch (InsufficientHistoryException ex)
            {
                _logger.LogWarning(ex, "Insufficient historical data for prediction");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new PipelineErrorResponse
                {
                    ErrorType = "insufficient-history",
                    Message = ex.Message,
                    RecoverySteps = new[]
                    {
                    "Collect more pipeline run data",
                    "Reduce confidence threshold",
                    "Use simpler prediction models"
                },
                    FallbackSuggestion = "Manual risk assessment based on change type"
                });
            }
        }

        [HttpPost("adapt-to-change")]
        [ProducesResponseType(typeof(AdaptationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PipelineErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AdaptPipelineToChange([FromBody] AdaptationRequest request)
        {
            if (request.ImpactAssessment == null)
            {
                return BadRequest(new PipelineErrorResponse
                {
                    ErrorType = "missing-assessment",
                    Message = "Need impact assessment for adaptation",
                    RecoverySteps = new[] { "Perform impact assessment first" },
                    FallbackSuggestion = "Use conservative adaptation strategy"
                });
            }

            try
            {
                _logger.LogInformation(
                    "Adapting pipeline to {ChangeType} change with {ImpactScore} impact score",
                    request.ChangeType,
                    request.ImpactAssessment.ImpactScore);

                var response = await _adaptationOrchestrator.AdaptPipelineAsync(request);
                return Ok(response);
            }
            catch (AdaptationComplexityException ex)
            {
                _logger.LogError(ex, "Pipeline adaptation too complex");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new PipelineErrorResponse
                {
                    ErrorType = "adaptation-complexity",
                    Message = ex.Message,
                    RecoverySteps = new[]
                    {
                    "Break change into smaller increments",
                    "Manual adaptation with guided steps",
                    "Temporary workarounds while adapting"
                },
                    FallbackSuggestion = "Manual pipeline redesign with expert review"
                });
            }
        }
    }
}
