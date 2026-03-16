
using Chapter_8.Exceptions;
using Chapter_8.Models.Errors;
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;
using Chapter_8.Orchestrators;
using Microsoft.AspNetCore.Mvc;

namespace Chapter_8.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LegacyConquestController : ControllerBase
    {
        private readonly ILegacyAnalysisOrchestrator _analysisOrchestrator;
        private readonly IWrapperGenerationOrchestrator _wrapperOrchestrator;
        private readonly ITestCreationOrchestrator _testOrchestrator;
        private readonly IModernizationPlanningOrchestrator _planningOrchestrator;
        private readonly IHealthMonitoringOrchestrator _healthOrchestrator;
        private readonly ILogger<LegacyConquestController> _logger;

        public LegacyConquestController(
            ILegacyAnalysisOrchestrator analysisOrchestrator,
            IWrapperGenerationOrchestrator wrapperOrchestrator,
            ITestCreationOrchestrator testOrchestrator,
            IModernizationPlanningOrchestrator planningOrchestrator,
            IHealthMonitoringOrchestrator healthOrchestrator,
            ILogger<LegacyConquestController> logger)
        {
            _analysisOrchestrator = analysisOrchestrator;
            _wrapperOrchestrator = wrapperOrchestrator;
            _testOrchestrator = testOrchestrator;
            _planningOrchestrator = planningOrchestrator;
            _healthOrchestrator = healthOrchestrator;
            _logger = logger;
        }

        [HttpPost("analyze")]
        [ProducesResponseType(typeof(LegacyAnalysisResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AnalyzeLegacyCodebase([FromBody] LegacyAnalysisRequest request)
        {
            try
            {
                // Validate request
                if (request.Codebase.TotalLines > 1000000 && request.AnalysisDepth == "comprehensive")
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "analysis-scale",
                        Message = "Codebase too large for comprehensive analysis",
                        RecoverySteps = new[]
                        {
                            "Use 'standard' or 'quick' analysis depth",
                            "Analyze subsystems separately",
                            "Focus on specific modules only"
                        },
                        FallbackSuggestion = "Incremental analysis starting with highest-risk areas"
                    });
                }

                if (string.IsNullOrEmpty(request.BusinessContext?.CriticalFlows?.FirstOrDefault()?.Description))
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "missing-context",
                        Message = "Need business context to understand legacy code purpose",
                        RecoverySteps = new[]
                        {
                            "Provide at least one critical business flow",
                            "Describe key business processes",
                            "List stakeholder concerns"
                        },
                        FallbackSuggestion = "Analysis without business context will focus on technical aspects only"
                    });
                }

                _logger.LogInformation(
                    "Analyzing legacy codebase: {TechnologyStack}, {AgeYears} years old, {TotalLines} lines",
                    string.Join(", ", request.Codebase.TechnologyStack ?? Array.Empty<string>()),
                    request.Codebase.AgeYears,
                    request.Codebase.TotalLines);

                var response = await _analysisOrchestrator.AnalyzeLegacyCodebaseAsync(request);

                _logger.LogInformation(
                    "Analysis complete: Found {RiskCount} risk hotspots, {AssumptionCount} hidden assumptions",
                    response.RiskHotspots?.Length ?? 0,
                    response.HiddenAssumptions?.Length ?? 0);

                return Ok(response);
            }
            catch (CodeComplexityException ccex)
            {
                _logger.LogWarning(ccex, "Legacy code too complex for analysis");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LegacyErrorResponse
                {
                    ErrorType = "complexity-overload",
                    Message = ccex.Message,
                    RecoverySteps = new[]
                    {
                        "Simplify analysis focus areas",
                        "Provide more specific code samples",
                        "Use manual analysis with guided templates"
                    },
                    FallbackSuggestion = ccex.SuggestedApproach ?? "Human-led code archaeology with AI assistance",
                    DiagnosticData = new LegacyDiagnosticData
                    {
                        ProblematicComplexity = ccex.ComplexityScore,
                        SuggestedSimplification = ccex.SuggestedApproach
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during code analysis");
                throw;
            }
        }

        [HttpPost("generate-wrappers")]
        [ProducesResponseType(typeof(WrapperGenerationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GenerateSafeWrappers([FromBody] WrapperRequest request)
        {
            try
            {
                if (request.LegacyModule == null)
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "missing-module",
                        Message = "Need legacy module to generate wrappers",
                        RecoverySteps = new[] { "Provide a valid legacy module" }
                    });
                }

                if (request.LegacyModule.ComplexityScore > 8 && request.SafetyLevel == "aggressive")
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "safety-mismatch",
                        Message = "Complex modules require conservative safety measures",
                        RecoverySteps = new[]
                        {
                            "Use 'conservative' safety level for complex modules",
                            "Break module into smaller pieces first",
                            "Add more validation requirements"
                        },
                        FallbackSuggestion = "Manual wrapper design with safety review"
                    });
                }

                _logger.LogInformation(
                    "Generating {WrapperType} wrapper for legacy module: {ModuleName}",
                    request.WrapperType, request.LegacyModule.Name);

                var response = await _wrapperOrchestrator.GenerateWrappersAsync(request);

                _logger.LogInformation(
                    "Generated wrapper with {SafetyFeatureCount} safety features, {TestCount} validation tests",
                    response.GeneratedWrapper?.SafetyFeatures?.Length ?? 0,
                    response.ValidationTests?.Length ?? 0);

                return Ok(response);
            }
            catch (WrapperComplexityException wcex)
            {
                _logger.LogWarning(wcex, "Wrapper generation too complex");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LegacyErrorResponse
                {
                    ErrorType = "wrapper-complexity",
                    Message = wcex.Message,
                    RecoverySteps = new[]
                    {
                        "Simplify module interface first",
                        "Generate wrapper for subset of functionality",
                        "Use simpler wrapper type"
                    },
                    FallbackSuggestion = "Manual wrapper implementation with AI-generated templates",
                    DiagnosticData = new LegacyDiagnosticData
                    {
                        ProblematicModule = wcex.ModuleName,
                        ComplexityIssues = wcex.ComplexityIssues
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during wrapper generation");
                throw;
            }
        }

        [HttpPost("create-characterization-tests")]
        [ProducesResponseType(typeof(CharacterizationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateCharacterizationTests([FromBody] CharacterizationRequest request)
        {
            try
            {
                if (request.ObservedOutputs?.Length < 5)
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "insufficient-data",
                        Message = "Need at least 5 observed outputs for meaningful characterization",
                        RecoverySteps = new[] { "Provide more observed outputs", "Use record-replay to capture more data" }
                    });
                }

                if (string.IsNullOrEmpty(request.LegacyBehavior?.Description))
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "missing-behavior",
                        Message = "Need description of legacy behavior to test",
                        RecoverySteps = new[] { "Describe the legacy behavior clearly" }
                    });
                }

                _logger.LogInformation(
                    "Creating characterization tests for legacy behavior: {BehaviorDescription}",
                    request.LegacyBehavior.Description[..Math.Min(50, request.LegacyBehavior.Description.Length)]);

                var response = await _testOrchestrator.CreateCharacterizationTestsAsync(request);

                _logger.LogInformation(
                    "Created {TestCount} characterization tests with {CoveragePercentage}% coverage",
                    response.CharacterizationTests?.Length ?? 0,
                    response.CoverageReport?.CoveragePercentage * 100 ?? 0);

                return Ok(response);
            }
            catch (BehaviorAmbiguityException baex)
            {
                _logger.LogWarning(baex, "Legacy behavior too ambiguous for characterization");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LegacyErrorResponse
                {
                    ErrorType = "behavior-ambiguity",
                    Message = baex.Message,
                    RecoverySteps = new[]
                    {
                        "Provide more specific behavior descriptions",
                        "Add more observed outputs",
                        "Clarify expected vs observed behavior"
                    },
                    FallbackSuggestion = "Manual test creation with behavior sampling",
                    DiagnosticData = new LegacyDiagnosticData
                    {
                        AmbiguityAreas = baex.AmbiguityAreas,
                        SuggestedClarifications = baex.ClarificationQuestions
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during test creation");
                throw;
            }
        }

        [HttpPost("plan-modernization")]
        [ProducesResponseType(typeof(ModernizationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PlanIncrementalModernization([FromBody] RoadmapRequest request)
        {
            try
            {
                if (request.LegacyAnalysis == null)
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "missing-analysis",
                        Message = "Need legacy analysis for modernization planning",
                        RecoverySteps = new[] { "Run code analysis first" }
                    });
                }

                if (request.BusinessPriorities?.Length == 0)
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "missing-priorities",
                        Message = "Need business priorities to guide modernization",
                        RecoverySteps = new[] { "Define business priorities", "Identify key stakeholders" }
                    });
                }

                _logger.LogInformation(
                    "Planning {ModernizationApproach} modernization with {ConstraintCount} constraints",
                    request.ModernizationApproach,
                    request.Constraints?.GetConstraintCount() ?? 0);

                var response = await _planningOrchestrator.PlanModernizationAsync(request);

                _logger.LogInformation(
                    "Created {PhaseCount}-phase roadmap with {StepCount} total steps",
                    response.Roadmap?.Phases?.Length ?? 0,
                    response.Roadmap?.Phases?.Sum(p => p.Steps?.Length ?? 0) ?? 0);

                return Ok(response);
            }
            catch (ConstraintViolationException cvex)
            {
                _logger.LogWarning(cvex, "Modernization constraints cannot be satisfied");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LegacyErrorResponse
                {
                    ErrorType = "constraint-violation",
                    Message = cvex.Message,
                    RecoverySteps = new[]
                    {
                        "Relax one or more constraints",
                        "Extend timeline",
                        "Increase budget",
                        "Accept higher risk levels"
                    },
                    FallbackSuggestion = "Manual roadmap creation with constraint negotiation",
                    DiagnosticData = new LegacyDiagnosticData
                    {
                        ConflictingConstraints = cvex.ConflictingConstraints,
                        SuggestedAdjustments = cvex.ConstraintAdjustments
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during modernization planning");
                throw;
            }
        }

        [HttpPost("monitor-health")]
        [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LegacyErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> MonitorLegacyHealth([FromBody] HealthRequest request)
        {
            try
            {
                if (request.MonitoredSystems?.Length == 0)
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "missing-systems",
                        Message = "Need systems to monitor",
                        RecoverySteps = new[] { "Specify at least one system to monitor" }
                    });
                }

                if (request.TelemetryData?.Length < 100)
                {
                    return BadRequest(new LegacyErrorResponse
                    {
                        ErrorType = "insufficient-telemetry",
                        Message = "Need sufficient telemetry data for meaningful health analysis",
                        RecoverySteps = new[] { "Collect more telemetry data", "Increase sampling rate" }
                    });
                }

                _logger.LogInformation(
                    "Monitoring health of {SystemCount} legacy systems with {DataPointCount} telemetry points",
                    request.MonitoredSystems?.Length ?? 0,
                    request.TelemetryData?.Length ?? 0);

                var response = await _healthOrchestrator.MonitorHealthAsync(request);

                _logger.LogInformation(
                    "Health monitoring complete: {HealthyCount} healthy, {WarningCount} warnings, {CriticalCount} critical",
                    response.HealthScores?.Count(s => s.Status == "healthy") ?? 0,
                    response.HealthScores?.Count(s => s.Status == "warning") ?? 0,
                    response.HealthScores?.Count(s => s.Status == "critical") ?? 0);

                return Ok(response);
            }
            catch (TelemetryInconsistencyException tiex)
            {
                _logger.LogWarning(tiex, "Telemetry data inconsistent for analysis");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LegacyErrorResponse
                {
                    ErrorType = "telemetry-inconsistency",
                    Message = tiex.Message,
                    RecoverySteps = new[]
                    {
                        "Clean telemetry data",
                        "Provide more consistent time periods",
                        "Use simpler health indicators"
                    },
                    FallbackSuggestion = "Manual health assessment with sampled data",
                    DiagnosticData = new LegacyDiagnosticData
                    {
                        InconsistencyDetails = tiex.InconsistencyDetails,
                        DataQualityIssues = tiex.DataQualityIssues
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during health monitoring");
                throw;
            }
        }
    }
}