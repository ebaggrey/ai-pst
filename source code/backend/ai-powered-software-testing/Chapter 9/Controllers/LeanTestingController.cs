using Chapter_9.Exceptions;
using Chapter_9.Models.Errors;
using Chapter_9.Models.Requests;
using Chapter_9.Models.Responses;
using Chapter_9.Orchestrators;
using Chapter_9.Settings;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Options;

namespace Chapter_9.Controllers
{
    [ApiController]
    [Route("api/lean-testing")]
    //[ApiExplorerSettings(GroupName = "lean-testing")]
    public class LeanTestingController : ControllerBase
    {
        private readonly ILeanTestingOrchestrator _orchestrator;
        private readonly ILogger<LeanTestingController> _logger;
        private readonly LeanTestingSettings _settings;

        public LeanTestingController(
            ILeanTestingOrchestrator orchestrator,
            ILogger<LeanTestingController> logger,
            IOptions<LeanTestingSettings> settings)
        {
            _orchestrator = orchestrator;
            _logger = logger;
            _settings = settings.Value;
        }

        [HttpPost("prioritize")]
        [ProducesResponseType(typeof(TestingPriorityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PrioritizeTestingEffort([FromBody] PriorityRequest request)
        {
            // Validate feature count
            if (request.Features.Length > _settings.MaxFeaturesPerBatch)
            {
                return BadRequest(new LeanErrorResponse
                {
                    Context = "prioritization",
                    ErrorType = "scope-too-large",
                    LeanPrincipleViolated = "Limit Work in Progress",
                    Message = $"Cannot effectively prioritize more than {_settings.MaxFeaturesPerBatch} features at once",
                    RecoverySteps = new[]
                    {
                        $"Prioritize in batches of 10-{_settings.MaxFeaturesPerBatch/2} features",
                        "Group related features first",
                        "Use higher-level categories initially"
                    },
                    FallbackSuggestion = "Manual prioritization with AI-assisted scoring"
                });
            }

            if (request.MaxTestingBudget <= 0)
            {
                return BadRequest(new LeanErrorResponse
                {
                    Context = "prioritization",
                    ErrorType = "invalid-constraints",
                    LeanPrincipleViolated = "Respect Budget Constraints",
                    Message = "Testing budget must be greater than zero",
                    RecoverySteps = new[] { "Set realistic time budget", "Adjust feature scope", "Re-evaluate constraints" },
                    FallbackSuggestion = "Time-boxed testing with fixed constraints"
                });
            }

            try
            {
                _logger.LogInformation(
                    "Prioritizing {FeatureCount} features with {Method} method and {BudgetHours} hour budget",
                    request.Features.Length, request.PrioritizationMethod, request.MaxTestingBudget);

                var response = await _orchestrator.PrioritizeTestingEffortAsync(request);

                _logger.LogInformation(
                    "Prioritization complete: Top {TopFeatureCount} features deliver {ValuePercentage}% of value",
                    response.Features.Count(f => f.Priority <= 3),
                    response.Features.Where(f => f.Priority <= 3).Sum(f => f.BusinessValue) /
                    response.Features.Sum(f => f.BusinessValue) * 100);

                return Ok(response);
            }
            catch (ConstraintImpossibleException cex)
            {
                _logger.LogWarning(cex, "Constraints impossible to satisfy");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LeanErrorResponse
                {
                    Context = "prioritization",
                    ErrorType = "constraint-impossible",
                    LeanPrincipleViolated = "Set Realistic Constraints",
                    Message = "Cannot satisfy all constraints with given features",
                    RecoverySteps = new[]
                    {
                        $"Increase time budget by {cex.MinimumAdditionalTime:F0} hours",
                        $"Reduce feature count by {cex.FeaturesToRemove} features",
                        "Accept lower confidence levels"
                    },
                    FallbackSuggestion = "Manual triage with constraint relaxation",
                    DiagnosticData = new LeanDiagnosticData
                    {
                        ConstraintAnalysis = cex.ConstraintAnalysis,
                        SuggestedAdjustments = cex.SuggestedAdjustments
                    }
                });
            }
        }

        [HttpPost("minimal-coverage")]
        [ProducesResponseType(typeof(CoverageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GenerateMinimalTestCoverage([FromBody] CoverageRequest request)
        {
            // Validate minimalism constraints
            if (request.Constraints.MaxTestCases < 1)
            {
                return BadRequest("Need at least 1 test case");
            }

            if (request.Constraints.MaxTestCases > _settings.MaxTestCasesPerFeature)
            {
                return BadRequest(new LeanErrorResponse
                {
                    Context = "coverage-generation",
                    ErrorType = "over-testing",
                    LeanPrincipleViolated = "Minimal Viable Coverage",
                    Message = $"Maximum test cases exceeds lean guidelines ({_settings.MaxTestCasesPerFeature} max)",
                    RecoverySteps = new[]
                    {
                        $"Reduce max test cases to 10-{_settings.MaxTestCasesPerFeature/2}",
                        "Increase confidence target acceptance",
                        "Focus on highest-risk scenarios"
                    },
                    FallbackSuggestion = "Manual test selection with risk-based approach"
                });
            }

            try
            {
                _logger.LogInformation(
                    "Generating minimal test coverage for {FeatureName} with {ConfidenceTarget} confidence target",
                    request.Feature.Name, request.ConfidenceTarget);

                var response = await _orchestrator.GenerateMinimalTestCoverageAsync(request);

                _logger.LogInformation(
                    "Generated {TestCount} tests achieving {CoveragePercentage}% value coverage",
                    response.TestCases.Length, response.CoverageMetrics.ValueCoverage * 100);

                return Ok(response);
            }
            catch (CoverageImpossibleException ciex)
            {
                _logger.LogWarning(ciex, "Cannot achieve confidence target with constraints");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LeanErrorResponse
                {
                    Context = "coverage-generation",
                    ErrorType = "coverage-impossible",
                    LeanPrincipleViolated = "Set Achievable Goals",
                    Message = "Cannot achieve confidence target with given constraints",
                    RecoverySteps = new[]
                    {
                        $"Increase max test cases to {ciex.MinimumTestsRequired}",
                        $"Reduce confidence target to {ciex.AchievableConfidence:P0}",
                        "Accept higher risk in untested areas"
                    },
                    FallbackSuggestion = "Manual test design with constraint awareness",
                    DiagnosticData = new LeanDiagnosticData
                    {
                        CoverageGapAnalysis = ciex.CoverageGapAnalysis,
                        ConstraintImpact = ciex.ConstraintImpact
                    }
                });
            }
        }

        [HttpPost("automation-threshold")]
        [ProducesResponseType(typeof(AutomationDecisionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DecideAutomationThreshold([FromBody] AutomationDecisionRequest request)
        {
            // Validate cost data
            if (request.AutomationCost.InitialCost <= 0 || request.ManualCost.ExecutionCost <= 0)
            {
                return BadRequest("Cost data must be positive values");
            }

            if (request.ROIThreshold < _settings.MinROIThreshold)
            {
                return BadRequest($"ROI threshold must be at least {_settings.MinROIThreshold} (break-even)");
            }

            try
            {
                _logger.LogInformation(
                    "Deciding automation for {ScenarioName}, ROI threshold: {ROIThreshold}",
                    request.TestScenario.Name, request.ROIThreshold);

                var response = await _orchestrator.DecideAutomationThresholdAsync(request);

                _logger.LogInformation(
                    "Automation decision: {Decision} with {ROIValue}x ROI",
                    response.Decision.Decision, response.ROI.ROIValue);

                return Ok(response);
            }
            catch (CostCalculationException ccex)
            {
                _logger.LogWarning(ccex, "Cost calculation failed");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LeanErrorResponse
                {
                    Context = "automation-decision",
                    ErrorType = "cost-calculation-error",
                    LeanPrincipleViolated = "Measure What Matters",
                    Message = "Cannot calculate accurate costs for automation decision",
                    RecoverySteps = new[]
                    {
                        "Provide more detailed cost estimates",
                        "Use historical data for similar scenarios",
                        "Simplify cost categories"
                    },
                    FallbackSuggestion = "Rule-based decision with estimated costs",
                    DiagnosticData = new LeanDiagnosticData
                    {
                        MissingCostData = ccex.MissingData,
                        EstimationChallenges = ccex.EstimationChallenges
                    }
                });
            }
        }

        [HttpPost("optimize-maintenance")]
        [ProducesResponseType(typeof(MaintenanceOptimizationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> OptimizeTestMaintenance([FromBody] MaintenanceRequest request)
        {
            if (request.ExistingTests.TestCases.Length == 0)
            {
                return BadRequest("Need existing tests to optimize");
            }

            if (request.AllowedActions.Length == 0)
            {
                return BadRequest("Need to specify allowed optimization actions");
            }

            try
            {
                _logger.LogInformation(
                    "Optimizing maintenance for {TestCount} tests with {Strategy} strategy",
                    request.ExistingTests.TestCases.Length, request.OptimizationStrategy);

                var response = await _orchestrator.OptimizeTestMaintenanceAsync(request);

                _logger.LogInformation(
                    "Optimization complete: {ActionsTaken} actions, estimated {SavingsPercentage}% maintenance reduction",
                    response.Optimization.Actions.Sum(a => a.AffectedTests.Length),
                    response.Savings.MaintenanceReduction * 100);

                return Ok(response);
            }
            catch (PreservationViolationException pvex)
            {
                _logger.LogWarning(pvex, "Optimization violates preservation rules");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LeanErrorResponse
                {
                    Context = "maintenance-optimization",
                    ErrorType = "preservation-violation",
                    LeanPrincipleViolated = "Preserve Value",
                    Message = "Optimization would violate preservation rules",
                    RecoverySteps = new[]
                    {
                        "Relax preservation rules",
                        "Adjust optimization strategy",
                        "Manual review of affected tests"
                    },
                    FallbackSuggestion = "Manual optimization with rule validation",
                    DiagnosticData = new LeanDiagnosticData
                    {
                        ViolatedRules = pvex.ViolatedRules,
                        AffectedTests = pvex.AffectedTests
                    }
                });
            }
        }

        [HttpPost("measure-roi")]
        [ProducesResponseType(typeof(ROIAnalysisResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LeanErrorResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> MeasureTestingROI([FromBody] ROIRequest request)
        {
            if (request.TestInvestments.Length == 0)
            {
                return BadRequest("Need test investments to measure ROI");
            }

            if (request.Outcomes.Length == 0)
            {
                return BadRequest("Need outcomes to compare against investments");
            }

            try
            {
                _logger.LogInformation(
                    "Measuring ROI for {InvestmentCount} investments over {Period}",
                    request.TestInvestments.Length, request.MeasurementPeriod);

                var response = await _orchestrator.MeasureTestingROIAsync(request);

                _logger.LogInformation(
                    "ROI analysis complete: Overall ROI {OverallROI}, {InsightCount} insights generated",
                    response.OverallROI.ROIValue, response.Insights.Length);

                return Ok(response);
            }
            catch (ROICalculationException ricex)
            {
                _logger.LogWarning(ricex, "ROI calculation failed");

                return StatusCode(StatusCodes.Status422UnprocessableEntity, new LeanErrorResponse
                {
                    Context = "roi-measurement",
                    ErrorType = "calculation-error",
                    LeanPrincipleViolated = "Measure What You Can",
                    Message = "Cannot calculate complete ROI with available data",
                    RecoverySteps = new[]
                    {
                        "Provide more complete investment data",
                        "Focus on tangible metrics only",
                        "Use simplified ROI calculation"
                    },
                    FallbackSuggestion = "Qualitative assessment with limited metrics",
                    DiagnosticData = new LeanDiagnosticData
                    {
                        DataGaps = ricex.MissingData,
                        CalculationLimitations = ricex.Limitations
                    }
                });
            }
        }
    }
}