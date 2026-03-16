using Chapter_1.Exceptions;
using Chapter_1.Models;
using Chapter_1.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chapter_1.Controllers
{
    // Controllers/LandscapeAnalysisController.cs - Complete
    [ApiController]
    [Route("api/landscape")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class LandscapeAnalysisController : ControllerBase
    {
        private readonly ILandscapeAnalyzer _landscapeAnalyzer;
        private readonly ILLMOrchestrator _llmOrchestrator;
        private readonly ITestSynthesisService _testSynthesisService;
        private readonly ILogger<LandscapeAnalysisController> _logger;

        public LandscapeAnalysisController(
            ILandscapeAnalyzer landscapeAnalyzer,
            ILLMOrchestrator llmOrchestrator,
            ITestSynthesisService testSynthesisService,
            ILogger<LandscapeAnalysisController> logger)
        {
            _landscapeAnalyzer = landscapeAnalyzer;
            _llmOrchestrator = llmOrchestrator;
            _testSynthesisService = testSynthesisService;
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeTestingLandscape(
            [FromBody] LandscapeTestRequest request)
        {
            // Custom validation that understands testing context
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
                    );

                _logger.LogWarning("Invalid landscape request: {@Errors}", errors);

                return BadRequest(new LandscapeError
                {
                    ErrorCode = "VALIDATION_FAILED",
                    Message = "The landscape analysis request has some issues",
                    RecoverySteps = new[] { "Check required fields", "Verify architecture details" },
                    Context = new Dictionary<string, object> { ["validationErrors"] = errors }
                });
            }

            try
            {
                _logger.LogInformation(
                    "Analyzing testing landscape for {AppName} ({Architecture})",
                    request.ApplicationProfile.Name,
                    request.ApplicationProfile.ArchitectureType);

                // Step 1: Analyze the architecture
                var analysis = await _landscapeAnalyzer.AnalyzeAsync(request.ApplicationProfile);

                // Step 2: Generate tailored prompts for each testing area
                var prompts = GenerateTargetedPrompts(analysis, request.TestingFocus);

                // Step 3: Use multiple LLMs in parallel for different aspects
                var results = await _llmOrchestrator.OrchestrateAnalysisAsync(prompts);

                // Step 4: Synthesize results into coherent strategy
                var response = await _testSynthesisService.SynthesizeAsync(analysis, results, request);

                _logger.LogInformation(
                    "Generated landscape strategy with {ScenarioCount} scenarios",
                    response.HighPriorityScenarios.Length);

                return Ok(response);
            }
            catch (ArchitectureAnalysisException aex)
            {
                _logger.LogError(aex, "Failed to analyze application architecture");
                return StatusCode(422, new LandscapeError // 422 Unprocessable Entity
                {
                    ErrorCode = "ARCHITECTURE_UNPROCESSABLE",
                    Message = "Couldn't make sense of the application architecture",
                    RecoverySteps = new[]
                    {
                    "Simplify the architecture description",
                    "Focus on one component at a time",
                    "Provide more details about integration points"
                },
                    FallbackSuggestion = GenerateBasicTestingChecklist(request)
                });
            }
            catch (LLMCoordinationException lex)
            {
                _logger.LogError(lex, "LLM orchestration failed");

                // Try fallback to a single provider
                var fallbackResult = await TryFallbackAnalysisAsync(request);

                if (fallbackResult != null)
                    return Ok(fallbackResult);

                return StatusCode(503, new LandscapeError
                {
                    ErrorCode = "LLM_UNAVAILABLE",
                    Message = "All AI services are currently unavailable",
                    RecoverySteps = new[] { "Try again in a few minutes", "Use manual analysis mode" },
                    FallbackSuggestion = "Focus on smoke tests for critical paths only"
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error in landscape analysis");

                return StatusCode(500, new LandscapeError
                {
                    ErrorCode = "INTERNAL_ANALYSIS_FAILURE",
                    Message = "Something went wrong during analysis",
                    RecoverySteps = new[] { "Contact support with the error code" },
                    Context = new Dictionary<string, object>
                    {
                        ["errorId"] = Guid.NewGuid().ToString(),
                        ["timestamp"] = DateTime.UtcNow
                    }
                });
            }
        }

        private Dictionary<string, string> GenerateTargetedPrompts(
            ArchitectureAnalysis analysis,
            string[] focusAreas)
        {
            var prompts = new Dictionary<string, string>();

            foreach (var area in focusAreas)
            {
                prompts[area] = area.ToLower() switch
                {
                    "integration" => BuildIntegrationPrompt(analysis),
                    "performance" => BuildPerformancePrompt(analysis),
                    "security" => BuildSecurityPrompt(analysis),
                    "ui" => BuildUIPrompt(analysis),
                    "api" => BuildApiPrompt(analysis),
                    "database" => BuildDatabasePrompt(analysis),
                    _ => BuildGenericPrompt(analysis, area)
                };
            }

            return prompts;
        }

        private string BuildIntegrationPrompt(ArchitectureAnalysis analysis)
        {
            return $@"Analyze integration points for {analysis.ApplicationName}:
Integration Points: {analysis.IntegrationPoints.Count}
Critical Dependencies: {analysis.Dependencies.CriticalDependencies.Count}

Generate integration test scenarios focusing on:
1. API contract validation
2. Error handling between services
3. Data consistency across boundaries
4. Performance under load
5. Circuit breaker patterns

Consider these specific integration points:
{string.Join("\n", analysis.IntegrationPoints.Select(ip => $"- {ip.Source} -> {ip.Target} via {ip.Protocol}"))}";
        }

        private string BuildPerformancePrompt(ArchitectureAnalysis analysis)
        {
            return $@"Design performance testing strategy for {analysis.ApplicationName}:
Architecture Type: {analysis.ApplicationName}
Complexity Score: {analysis.Complexity.Score}/10
Critical User Journeys: {analysis.ApplicationName} has {analysis.IntegrationPoints.Count} integration points

Focus on:
1. Load testing thresholds
2. Stress testing scenarios
3. End-to-end latency measurements
4. Resource utilization patterns
5. Scalability bottlenecks";
        }

        private string BuildSecurityPrompt(ArchitectureAnalysis analysis)
        {
            return $@"Create security testing plan for {analysis.ApplicationName}:
Risk Score: {analysis.RiskScore}/10
Data Sources: Multiple external integrations

Security test areas:
1. Authentication and authorization flows
2. Input validation and sanitization
3. Data protection at rest and in transit
4. API security (rate limiting, injection prevention)
5. Compliance requirements check";
        }

        private string BuildUIPrompt(ArchitectureAnalysis analysis)
        {
            return $@"Design UI/UX testing strategy for {analysis.ApplicationName}:
Frontend Technologies: Multiple frameworks
Expected User Scale: Large

Focus on:
1. Cross-browser compatibility
2. Responsive design testing
3. Accessibility compliance (WCAG)
4. User journey validation
5. Performance metrics (LCP, FID, CLS)
6. Visual regression testing";
        }

        private string BuildApiPrompt(ArchitectureAnalysis analysis)
        {
            return $@"Create comprehensive API testing plan:
API Count: {analysis.IntegrationPoints.Count(ip => ip.Protocol.Contains("HTTP"))}
Protocols: {string.Join(", ", analysis.IntegrationPoints.Select(ip => ip.Protocol).Distinct())}

Test coverage should include:
1. RESTful API best practices
2. GraphQL query/mutation validation
3. WebSocket real-time communication
4. gRPC service contracts
5. API versioning strategies";
        }

        private string BuildDatabasePrompt(ArchitectureAnalysis analysis)
        {
            return $@"Design database testing strategy:
Data Complexity: {analysis.Complexity.Score}/10
Integration Points with DB: {analysis.IntegrationPoints.Count(ip => ip.Target.Contains("database"))}

Focus on:
1. Data integrity and consistency
2. Transaction isolation levels
3. Query performance optimization
4. Migration and rollback testing
5. Backup and recovery procedures";
        }

        private string BuildGenericPrompt(ArchitectureAnalysis analysis, string area)
        {
            return $@"Provide testing recommendations for {area} area in {analysis.ApplicationName}:
Overall Risk Score: {analysis.RiskScore}/10
Testing Priority: {analysis.TestingPriority}

Consider:
1. Risk-based testing approach
2. Industry best practices for {area}
3. Integration with existing systems
4. Monitoring and observability
5. Automation opportunities";
        }

        private async Task<TestLandscapeResponse> TryFallbackAnalysisAsync(LandscapeTestRequest request)
        {
            try
            {
                _logger.LogWarning("Attempting fallback analysis for {AppName}", request.ApplicationProfile.Name);

                // Simple fallback using a single LLM provider
                var analysis = await _landscapeAnalyzer.AnalyzeAsync(request.ApplicationProfile);
                var basicPrompt = $@"Provide a basic testing strategy for {request.ApplicationProfile.Name}
Architecture: {request.ApplicationProfile.ArchitectureType}
Focus Areas: {string.Join(", ", request.TestingFocus)}

Keep it simple and practical with 3-5 key recommendations.";

                var fallbackResult = await _llmOrchestrator.ProcessSinglePromptAsync(
                    "general",
                    basicPrompt,
                    "chatgpt");

                return new TestLandscapeResponse
                {
                    AnalysisId = Guid.NewGuid().ToString(),
                    Summary = $"Fallback analysis: {fallbackResult.Content.Substring(0, Math.Min(500, fallbackResult.Content.Length))}...",
                    HighPriorityScenarios = new[]
                    {
                    new TestScenario
                    {
                        Title = "Basic Smoke Tests",
                        Description = "Ensure critical paths are working",
                        Priority = "high",
                        Steps = new[] { "Deploy application", "Run basic user journeys", "Verify core functionality" }
                    }
                },
                    GeneratedAt = DateTime.UtcNow,
                    EstimatedEffort = TimeSpan.FromDays(2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallback analysis also failed");
                return null;
            }
        }

        private string GenerateBasicTestingChecklist(LandscapeTestRequest request)
        {
            return $@"Basic Testing Checklist for {request.ApplicationProfile.Name}:

1. ✅ Smoke Tests - Verify deployment and basic functionality
2. ✅ Critical Path Testing - Test: {string.Join(", ", request.ApplicationProfile.CriticalUserJourneys.Take(3))}
3. ✅ API Integration - Test key endpoints
4. ✅ UI Sanity - Basic user interface checks
5. ✅ Data Validation - Verify data persistence and retrieval

Start with these 5 areas before expanding to comprehensive testing.";
        }

        // Additional endpoint for getting analysis by ID
        [HttpGet("analysis/{id}")]
        public async Task<IActionResult> GetAnalysis(string id)
        {
            // Implementation would retrieve from database/cache
            return NotFound(new LandscapeError
            {
                ErrorCode = "ANALYSIS_NOT_FOUND",
                Message = $"Analysis with ID {id} was not found",
                RecoverySteps = new[] { "Check the analysis ID", "Request a new analysis" },
                Timestamp = DateTime.UtcNow
            });
        }

        // Health check endpoint
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Services = new[] { "LandscapeAnalyzer", "LLMOrchestrator", "TestSynthesis" }
            });
        }
    }
}
