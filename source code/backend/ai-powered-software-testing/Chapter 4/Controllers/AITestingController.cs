using Microsoft.AspNetCore.Mvc;
using Chapter_4.Exceptions;
using Chapter_4.Models.Domain;
using Chapter_4.Services.Interfaces;
using Chapter_4.Settings;


namespace Chapter_4.Controllers
{
   
        [ApiController]
        [Route("api/ai-testing")]
        [ApiExplorerSettings(GroupName = "ai-testing")]
        public class AITestingController : ControllerBase
        {
            private readonly IAICapabilityAssessor _capabilityAssessor;
            private readonly IAIRobustnessTester _robustnessTester;
            private readonly IAIBiasDetector _biasDetector;
            private readonly IAIHallucinationDetector _hallucinationDetector;
            private readonly IAIDriftMonitor _driftMonitor;
            private readonly ILogger<AITestingController> _logger;
            private readonly AITestingConfiguration _configuration;

            public AITestingController(
                IAICapabilityAssessor capabilityAssessor,
                IAIRobustnessTester robustnessTester,
                IAIBiasDetector biasDetector,
                IAIHallucinationDetector hallucinationDetector,
                IAIDriftMonitor driftMonitor,
                ILogger<AITestingController> logger,
                AITestingConfiguration configuration)
            {
                _capabilityAssessor = capabilityAssessor;
                _robustnessTester = robustnessTester;
                _biasDetector = biasDetector;
                _hallucinationDetector = hallucinationDetector;
                _driftMonitor = driftMonitor;
                _logger = logger;
                _configuration = configuration;
            }

            [HttpPost("assess-capabilities")]
            [ProducesResponseType(typeof(AICapabilityReport), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(AITestingError), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(typeof(AITestingError), StatusCodes.Status503ServiceUnavailable)]
            public async Task<IActionResult> AssessAICapabilities([FromBody] AIAssessmentRequest request)
            {
                // Validate we're not overwhelming the AI service
                if (request.RigorLevel == "thorough" && request.Dimensions.Length > _configuration.MaxDimensionsForThorough)
                {
                    return BadRequest(new AITestingError
                    {
                        TestType = "capability-assessment",
                        Provider = request.Provider,
                        FailureMode = "configuration",
                        DiagnosticInfo = new DiagnosticInfo
                        {
                            ErrorCode = 400,
                            ErrorMessage = $"Too many dimensions for thorough assessment. Maximum is {_configuration.MaxDimensionsForThorough}",
                            SuggestedInvestigation = new[] {
                            $"Reduce dimensions to {_configuration.MaxDimensionsForThorough} or fewer",
                            "Use standard rigor level"
                        }
                        },
                        FallbackAction = "auto-reduce-dimensions"
                    });
                }

                try
                {
                    _logger.LogInformation(
                        "Starting capability assessment for {Provider} with {DimensionCount} dimensions",
                        request.Provider, request.Dimensions.Length);

                    // Run comprehensive capability assessment
                    var report = await _capabilityAssessor.AssessCapabilitiesAsync(request);

                    // Add statistical significance testing
                    report = await AddStatisticalSignificanceAsync(report);

                    // Generate actionable recommendations
                    report.Recommendations = GenerateProviderSpecificRecommendations(report);

                    _logger.LogInformation(
                        "Capability assessment complete: {Provider} scored {Score}/100",
                        report.Provider, report.OverallScore);

                    return Ok(report);
                }
                catch (AIOverloadException aoex)
                {
                    _logger.LogWarning(aoex, "AI service overloaded during assessment");

                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new AITestingError
                    {
                        TestType = "capability-assessment",
                        Provider = request.Provider,
                        FailureMode = "timeout",
                        DiagnosticInfo = new DiagnosticInfo
                        {
                            ErrorCode = 503,
                            ErrorMessage = "AI service cannot handle assessment load",
                            SuggestedInvestigation = new[] {
                            "Try during off-peak hours",
                            "Reduce test rigor",
                            "Use cached results"
                        }
                        },
                        FallbackAction = "schedule-for-later",
                        Metadata = new TestMetadata
                        {
                            TestCaseId = Guid.NewGuid().ToString(),
                            InputPrompt = "Capability assessment suite",
                            ExpectedBehavior = "Complete assessment without service overload"
                        }
                    });
                }
                catch (InconsistentAIResponseException irex)
                {
                    _logger.LogError(irex, "AI showing inconsistent responses");

                    // This is actually valuable test data
                    var inconsistentReport = await CaptureInconsistencyReportAsync(request, irex);
                    return Ok(inconsistentReport);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error during capability assessment");
                    return StatusCode(StatusCodes.Status500InternalServerError, new AITestingError
                    {
                        TestType = "capability-assessment",
                        Provider = request.Provider,
                        FailureMode = "unexpected",
                        DiagnosticInfo = new DiagnosticInfo
                        {
                            ErrorCode = 500,
                            ErrorMessage = "An unexpected error occurred",
                            SuggestedInvestigation = new[] { "Check service logs", "Verify input parameters" }
                        },
                        FallbackAction = "retry-later"
                    });
                }
            }

            [HttpPost("test-robustness")]
            [ProducesResponseType(typeof(RobustnessTestReport), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> TestPromptRobustness([FromBody] RobustnessTestRequest request)
            {
                // Validate prompt variations
                if (request.Variations.Length > _configuration.MaxPromptVariations)
                {
                    return BadRequest($"Too many variations - limit to {_configuration.MaxPromptVariations} for meaningful testing");
                }

                if (string.IsNullOrWhiteSpace(request.BasePrompt))
                {
                    return BadRequest("Base prompt cannot be empty");
                }

                try
                {
                    _logger.LogInformation(
                        "Testing robustness with {VariationCount} variations, {RunCount} runs each",
                        request.Variations.Length, request.NumberOfRuns);

                    var report = await _robustnessTester.TestRobustnessAsync(request);

                    // Calculate variance and identify problematic variations
                    report.VarianceAnalysis = await AnalyzeVarianceAsync(report);
                    report.Antipatterns = DetectPromptAntipatterns(report);
                    report.OptimizationSuggestions = GeneratePromptOptimizations(report);

                    return Ok(report);
                }
                catch (PromptInjectionException piex)
                {
                    _logger.LogWarning(piex, "Prompt injection attempt detected");

                    // Record this as a security finding
                    var securityReport = await GenerateSecurityReportAsync(request, piex);
                    return Ok(securityReport);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during robustness testing");
                    return StatusCode(StatusCodes.Status500InternalServerError, new AITestingError
                    {
                        TestType = "robustness-testing",
                        Provider = request.Provider,
                        FailureMode = "unexpected",
                        DiagnosticInfo = new DiagnosticInfo
                        {
                            ErrorCode = 500,
                            ErrorMessage = "An unexpected error occurred during robustness testing"
                        }
                    });
                }
            }

            [HttpPost("detect-bias")]
            [ProducesResponseType(typeof(BiasDetectionReport), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(AITestingError), StatusCodes.Status400BadRequest)]
            [ProducesResponseType(typeof(AITestingError), StatusCodes.Status422UnprocessableEntity)]
            public async Task<IActionResult> DetectAIBias([FromBody] BiasDetectionRequest request)
            {
                // Validate sensitivity settings
                if (request.SensitivityThreshold < _configuration.MinSensitivityThreshold ||
                    request.SensitivityThreshold > _configuration.MaxSensitivityThreshold)
                {
                    return BadRequest($"Sensitivity threshold must be between {_configuration.MinSensitivityThreshold} and {_configuration.MaxSensitivityThreshold}");
                }

                try
                {
                    _logger.LogInformation(
                        "Detecting bias in {ContextAreas} with {MethodCount} methods",
                        string.Join(", ", request.ContextAreas),
                        request.DetectionMethods.Length);

                    var report = await _biasDetector.DetectBiasAsync(request);

                    // Add statistical validation if requested
                    if (request.RequireStatisticalSignificance)
                    {
                        report = await AddStatisticalValidationAsync(report);
                    }

                    // Generate mitigation strategies
                    report.MitigationStrategies = GenerateBiasMitigations(report);
                    report.LongTermMonitoringPlan = CreateBiasMonitoringPlan(report);

                    return Ok(report);
                }
                catch (BiasDetectionComplexityException bdcex)
                {
                    _logger.LogWarning(bdcex, "Bias detection too complex for current configuration");

                    return StatusCode(StatusCodes.Status422UnprocessableEntity, new AITestingError
                    {
                        TestType = "bias-detection",
                        FailureMode = "complexity",
                        DiagnosticInfo = new DiagnosticInfo
                        {
                            ErrorCode = 422,
                            ErrorMessage = "Bias detection requires more specific context",
                            SuggestedInvestigation = new[] {
                            "Narrow context areas",
                            "Provide demographic data",
                            "Use simpler detection methods"
                        }
                        },
                        FallbackAction = "simplify-request"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during bias detection");
                    return StatusCode(StatusCodes.Status500InternalServerError, new AITestingError
                    {
                        TestType = "bias-detection",
                        FailureMode = "unexpected",
                        DiagnosticInfo = new DiagnosticInfo
                        {
                            ErrorCode = 500,
                            ErrorMessage = "An unexpected error occurred during bias detection"
                        }
                    });
                }
            }

            [HttpPost("test-hallucinations")]
            [ProducesResponseType(typeof(HallucinationDetectionReport), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> TestForHallucinations([FromBody] HallucinationTestRequest request)
            {
                // Validate known facts
                if (request.KnownFacts.Length < _configuration.MinFactsForHallucinationTest)
                {
                    return BadRequest($"Need at least {_configuration.MinFactsForHallucinationTest} known facts for meaningful hallucination testing");
                }

                try
                {
                    _logger.LogInformation(
                        "Testing {Provider} for hallucinations with {FactCount} known facts",
                        request.Provider, request.KnownFacts.Length);

                    var report = await _hallucinationDetector.DetectHallucinationsAsync(request);

                    // Categorize hallucinations by severity
                    report.Hallucinations = CategorizeBySeverity(report.Hallucinations);

                    // Generate confidence adjustments
                    report.ConfidenceAdjustments = CalculateConfidenceAdjustments(report);

                    // Create verification rules for future prompts
                    report.VerificationRules = GenerateVerificationRules(report);

                    _logger.LogWarning(
                        "Hallucination test found {HallucinationCount} hallucinations (max allowed: {MaxAllowed})",
                        report.Hallucinations.Count(h => h.Severity == "high"),
                        request.MaxAllowedHallucinations);

                    return Ok(report);
                }
                catch (FactVerificationException fvex)
                {
                    _logger.LogError(fvex, "Failed to verify facts against sources");

                    // Partial report with verification failures highlighted
                    var partialReport = await GeneratePartialHallucinationReportAsync(request, fvex);
                    return Ok(partialReport);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during hallucination testing");
                    return StatusCode(StatusCodes.Status500InternalServerError, new AITestingError
                    {
                        TestType = "hallucination-testing",
                        Provider = request.Provider,
                        FailureMode = "unexpected",
                        DiagnosticInfo = new DiagnosticInfo
                        {
                            ErrorCode = 500,
                            ErrorMessage = "An unexpected error occurred during hallucination testing"
                        }
                    });
                }
            }

            [HttpPost("monitor-drift")]
            [ProducesResponseType(typeof(DriftDetectionReport), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(typeof(AITestingError), StatusCodes.Status422UnprocessableEntity)]
            public async Task<IActionResult> MonitorAIDrift([FromBody] DriftDetectionRequest request)
            {
                // Validate baseline data
                if (request.Baseline.TestResults.Length < _configuration.MinBaselineResults)
                {
                    return BadRequest($"Baseline needs at least {_configuration.MinBaselineResults} test results for meaningful drift detection");
                }

                if (request.MinimumDataPoints < _configuration.MinDataPointsForDrift)
                {
                    return BadRequest($"Minimum data points should be at least {_configuration.MinDataPointsForDrift} for statistical significance");
                }

                try
                {
                    _logger.LogInformation(
                        "Monitoring AI drift over {Timeframe} with {MetricCount} metrics",
                        request.Timeframe, request.MetricsToMonitor.Length);

                    var report = await _driftMonitor.MonitorDriftAsync(request);

                    // Calculate drift significance
                    report.DriftSignificance = CalculateDriftSignificance(report);

                    // Generate alerts if needed
                    if (report.DriftSignificance > request.DriftThreshold)
                    {
                        report.Alerts = GenerateDriftAlerts(report);
                        report.RecommendedActions = DetermineCorrectiveActions(report);

                        _logger.LogWarning(
                            "Significant AI drift detected: {DriftPercentage}% beyond threshold",
                            (report.DriftSignificance - request.DriftThreshold) * 100);
                    }

                    return Ok(report);
                }
                catch (InsufficientDataException idex)
                {
                    _logger.LogWarning(idex, "Insufficient data for drift detection");

                    return StatusCode(StatusCodes.Status422UnprocessableEntity, new AITestingError
                    {
                        TestType = "drift-monitoring",
                        FailureMode = "insufficient-data",
                        DiagnosticInfo = new DiagnosticInfo
                        {
                            ErrorCode = 422,
                            ErrorMessage = "Not enough data points for drift detection",
                            SuggestedInvestigation = new[] {
                            "Collect more data",
                            "Extend timeframe",
                            "Reduce metric count"
                        }
                        },
                        FallbackAction = "continue-collecting-data"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during drift monitoring");
                    return StatusCode(StatusCodes.Status500InternalServerError, new AITestingError
                    {
                        TestType = "drift-monitoring",
                        FailureMode = "unexpected",
                        DiagnosticInfo = new DiagnosticInfo
                        {
                            ErrorCode = 500,
                            ErrorMessage = "An unexpected error occurred during drift monitoring"
                        }
                    });
                }
            }

            #region Helper Methods
            private async Task<AICapabilityReport> AddStatisticalSignificanceAsync(AICapabilityReport report)
            {
                await Task.Delay(10); // Simulate async processing
                                      // In real implementation, add statistical significance calculations
                return report;
            }

            private List<Recommendation> GenerateProviderSpecificRecommendations(AICapabilityReport report)
            {
                var recommendations = new List<Recommendation>();

                if (report.OverallScore < 70)
                {
                    recommendations.Add(new Recommendation
                    {
                        Area = "Overall Performance",
                        Suggestion = "Consider switching to a different model or provider",
                        Priority = "high",
                        Impact = "Significant improvement in accuracy and reliability"
                    });
                }

                if (report.DimensionScores.Any(d => d.Value < 60))
                {
                    var weakDimensions = report.DimensionScores.Where(d => d.Value < 60).Select(d => d.Key);
                    recommendations.Add(new Recommendation
                    {
                        Area = "Weak Dimensions",
                        Suggestion = $"Focus on improving: {string.Join(", ", weakDimensions)}",
                        Priority = "medium",
                        Impact = "Targeted improvement in specific capabilities"
                    });
                }

                return recommendations;
            }

            private async Task<AICapabilityReport> CaptureInconsistencyReportAsync(AIAssessmentRequest request, InconsistentAIResponseException irex)
            {
                await Task.Delay(10);
                return new AICapabilityReport
                {
                    Provider = request.Provider,
                    OverallScore = 50, // Low score due to inconsistency
                    Recommendations = new List<Recommendation>
                {
                    new Recommendation
                    {
                        Area = "Consistency",
                        Suggestion = "Model shows high variance in responses. Consider adding temperature control or prompt engineering.",
                        Priority = "high",
                        Impact = "Critical for production reliability"
                    }
                }
                };
            }

            private async Task<VarianceAnalysis> AnalyzeVarianceAsync(RobustnessTestReport report)
            {
                await Task.Delay(10);
                return new VarianceAnalysis
                {
                    OverallVariance = report.VariationResults.Any() ?
                        report.VariationResults.Average(v => 100 - v.ConsistencyScore) / 100 : 0,
                    HighVarianceVariations = report.VariationResults
                        .Where(v => v.ConsistencyScore < 70)
                        .Select(v => v.Variation)
                        .ToArray(),
                    StableVariations = report.VariationResults
                        .Where(v => v.ConsistencyScore > 90)
                        .Select(v => v.Variation)
                        .ToArray()
                };
            }

            private List<Antipattern> DetectPromptAntipatterns(RobustnessTestReport report)
            {
                var antipatterns = new List<Antipattern>();

                // Simple detection logic - in reality, this would be more sophisticated
                if (report.BasePrompt.Length > 500)
                {
                    antipatterns.Add(new Antipattern
                    {
                        Pattern = "OverlyLongPrompt",
                        Description = "Prompt exceeds recommended length",
                        Severity = "medium",
                        Fix = "Break into multiple prompts or reduce verbosity"
                    });
                }

                return antipatterns;
            }

            private List<OptimizationSuggestion> GeneratePromptOptimizations(RobustnessTestReport report)
            {
                var suggestions = new List<OptimizationSuggestion>();

                // Simple optimization suggestions
                if (report.AverageConsistencyScore < 80)
                {
                    suggestions.Add(new OptimizationSuggestion
                    {
                        Area = "Prompt Structure",
                        Suggestion = "Add more explicit instructions and examples",
                        ExpectedImprovement = 15.5m
                    });
                }

                return suggestions;
            }

            private async Task<BiasDetectionReport> AddStatisticalValidationAsync(BiasDetectionReport report)
            {
                await Task.Delay(10);
                // In real implementation, add statistical validation
                return report;
            }

            private List<MitigationStrategy> GenerateBiasMitigations(BiasDetectionReport report)
            {
                var strategies = new List<MitigationStrategy>();

                foreach (var finding in report.Findings.Where(f => f.Severity == "high"))
                {
                    strategies.Add(new MitigationStrategy
                    {
                        FindingId = finding.Context,
                        Strategy = $"Address {finding.BiasType} bias in {finding.Context}",
                        Implementation = "Review training data and add counter-examples",
                        Timeline = "immediate"
                    });
                }

                return strategies;
            }

            private MonitoringPlan CreateBiasMonitoringPlan(BiasDetectionReport report)
            {
                return new MonitoringPlan
                {
                    Metrics = report.Findings.Select(f => $"{f.Context}.{f.BiasType}").ToArray(),
                    Frequency = "weekly",
                    Triggers = new[] { "new_data", "model_update", "user_complaints" },
                    ReportingFormat = "dashboard"
                };
            }

            private async Task<RobustnessTestReport> GenerateSecurityReportAsync(RobustnessTestRequest request, PromptInjectionException piex)
            {
                await Task.Delay(10);
                return new RobustnessTestReport
                {
                    BasePrompt = request.BasePrompt,
                    VariationCount = request.Variations.Length,
                    RunCount = request.NumberOfRuns,
                    Antipatterns = new List<Antipattern>
                {
                    new Antipattern
                    {
                        Pattern = "PromptInjection",
                        Description = $"Detected injection attempt: {piex.MaliciousPattern}",
                        Severity = "critical",
                        Fix = "Add input sanitization and validation"
                    }
                }
                };
            }

            private List<HallucinationFinding> CategorizeBySeverity(List<HallucinationFinding> hallucinations)
            {
                // Simple categorization - in reality, use more sophisticated logic
                foreach (var hallucination in hallucinations)
                {
                    if (hallucination.Severity == "high" && string.IsNullOrEmpty(hallucination.Correction))
                    {
                        hallucination.Severity = "critical";
                    }
                }
                return hallucinations;
            }

            private List<ConfidenceAdjustment> CalculateConfidenceAdjustments(HallucinationDetectionReport report)
            {
                var adjustments = new List<ConfidenceAdjustment>();

                if (report.HallucinationRate > 0.1m) // More than 10% hallucination rate
                {
                    adjustments.Add(new ConfidenceAdjustment
                    {
                        Context = "General responses",
                        AdjustmentFactor = 0.7m, // Reduce confidence by 30%
                        Reason = "High hallucination rate detected"
                    });
                }

                return adjustments;
            }

            private List<VerificationRule> GenerateVerificationRules(HallucinationDetectionReport report)
            {
                var rules = new List<VerificationRule>();

                var hallucinatedTopics = report.Hallucinations
                    .Select(h => h.Category)
                    .Distinct();

                foreach (var topic in hallucinatedTopics)
                {
                    rules.Add(new VerificationRule
                    {
                        Pattern = $"*{topic}*",
                        VerificationMethod = "cross_reference",
                        ConfidenceLevel = "medium",
                        AutoVerify = true
                    });
                }

                return rules;
            }

            private async Task<HallucinationDetectionReport> GeneratePartialHallucinationReportAsync(HallucinationTestRequest request, FactVerificationException fvex)
            {
                await Task.Delay(10);
                return new HallucinationDetectionReport
                {
                    Provider = request.Provider,
                    TotalTests = request.KnownFacts.Length,
                    HallucinationCount = 0, // Unknown due to verification failure
                    Recommendations = new List<Recommendation>
                {
                    new Recommendation
                    {
                        Area = "Fact Verification",
                        Suggestion = "Fix verification source configuration",
                        Priority = "high",
                        Impact = "Enable accurate hallucination detection"
                    }
                }
                };
            }

            private decimal CalculateDriftSignificance(DriftDetectionReport report)
            {
                // Simple calculation - average of significant drifts
                var significantDrifts = report.MetricDrifts.Values
                    .Where(m => m.Significant)
                    .Select(m => Math.Abs(m.DriftAmount));

                return significantDrifts.Any() ? significantDrifts.Average() : 0;
            }

            private List<Alert> GenerateDriftAlerts(DriftDetectionReport report)
            {
                var alerts = new List<Alert>();

                foreach (var metric in report.MetricDrifts.Values.Where(m => m.Significant))
                {
                    alerts.Add(new Alert
                    {
                        Metric = metric.MetricName,
                        Severity = Math.Abs(metric.DriftAmount) > 0.3m ? "critical" : "warning",
                        Message = $"Metric '{metric.MetricName}' drifted by {metric.DriftAmount:P2}. Direction: {metric.Direction}"
                    });
                }

                return alerts;
            }

            private List<RecommendedAction> DetermineCorrectiveActions(DriftDetectionReport report)
            {
                var actions = new List<RecommendedAction>();

                if (report.DriftSignificance > 0.2m)
                {
                    actions.Add(new RecommendedAction
                    {
                        Action = "Retrain model with recent data",
                        Priority = "high",
                        Impact = "High - should address root cause",
                        Effort = "high"
                    });
                }
                else
                {
                    actions.Add(new RecommendedAction
                    {
                        Action = "Adjust confidence thresholds for affected metrics",
                        Priority = "medium",
                        Impact = "Medium - temporary mitigation",
                        Effort = "low"
                    });
                }

                return actions;
            }
            #endregion
        }
    
}
