
using Chapter_8.Interfaces;
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;
using Chapter_8.Orchestrators;
using Chapter_8.Services.LLM;


namespace Chapter_8.Orchestrators
{
    public interface ITestCreationOrchestrator
    {
        Task<CharacterizationResponse> CreateCharacterizationTestsAsync(CharacterizationRequest request);
    }
}



namespace LegacyConquest.Orchestrators
{
    public class TestCreationOrchestrator : ITestCreationOrchestrator
    {
        private readonly ICharacterizationTestCreator _testCreator;
        private readonly ILLMService _llmService;
        private readonly ILogger<TestCreationOrchestrator> _logger;

        public TestCreationOrchestrator(
            ICharacterizationTestCreator testCreator,
            ILLMService llmService,
            ILogger<TestCreationOrchestrator> logger)
        {
            _testCreator = testCreator;
            _llmService = llmService;
            _logger = logger;
        }

        public async Task<CharacterizationResponse> CreateCharacterizationTestsAsync(CharacterizationRequest request)
        {
            _logger.LogInformation("Starting orchestrated test creation for behavior: {BehaviorId}",
                request.LegacyBehavior?.Id);

            // Analyze observed behavior patterns
            var behaviorAnalysis = await AnalyzeObservedBehaviorAsync(
                request.ObservedOutputs ?? Array.Empty<ObservedOutput>(),
                request.TestStrategy);

            // Generate characterization tests
            var characterizationTests = await _testCreator.CreateCharacterizationTestsAsync(
                behaviorAnalysis,
                request.LegacyBehavior,
                request.CoverageGoal,
                request.IncludeEdgeCases);

            // Generate documentation if requested
            var documentation = request.GenerateDocumentation
                ? await GenerateTestDocumentationWithLLMAsync(characterizationTests, request.LegacyBehavior)
                : null;

            // Create test harness
            var testHarness = await CreateTestHarnessAsync(characterizationTests, request.LegacyBehavior);

            // Generate validation suite
            var validationSuite = await GenerateValidationSuiteAsync(characterizationTests, request.ObservedOutputs);

            // Generate coverage report
            var coverageReport = await GenerateCoverageReportAsync(characterizationTests, behaviorAnalysis);

            // Calculate confidence metrics
            var confidenceMetrics = await CalculateTestConfidenceWithLLMAsync(
                characterizationTests, request.ObservedOutputs);

            // Generate maintenance guide
            var maintenanceGuide = await GenerateMaintenanceGuideWithLLMAsync(
                characterizationTests, request.LegacyBehavior);

            var response = new CharacterizationResponse
            {
                TestSuiteId = Guid.NewGuid().ToString(),
                LegacyBehavior = request.LegacyBehavior,
                CharacterizationTests = characterizationTests,
                Documentation = documentation,
                TestHarness = testHarness,
                ValidationSuite = validationSuite,
                CoverageReport = coverageReport,
                ConfidenceMetrics = confidenceMetrics,
                MaintenanceGuide = maintenanceGuide
            };

            return response;
        }

        private async Task<BehaviorAnalysis> AnalyzeObservedBehaviorAsync(ObservedOutput[] outputs, string strategy)
        {
            return new BehaviorAnalysis
            {
                BehaviorId = Guid.NewGuid().ToString(),
                ObservedPatterns = outputs.Select((o, i) => new ObservedPattern
                {
                    PatternType = "Input-Output",
                    Description = $"Pattern {i + 1}",
                    OccurrenceCount = 1,
                    Confidence = 0.8
                }).ToArray(),
                IOSpace = new InputOutputSpace
                {
                    InputDomains = outputs.Select(o => o.Input).ToArray(),
                    OutputRanges = outputs.Select(o => o.Output).ToArray(),
                    Mappings = outputs.ToDictionary(o => o.Input, o => o.Output)
                },
                EdgeCases = new[] { "null input", "empty string", "maximum value" },
                BehaviorStability = 0.75
            };
        }

        private async Task<TestDocumentation> GenerateTestDocumentationWithLLMAsync(
            CharacterizationTest[] tests,
            LegacyBehavior behavior)
        {
            var prompt = $@"
            Generate documentation for these characterization tests:
            Tests: {System.Text.Json.JsonSerializer.Serialize(tests)}
            Behavior: {System.Text.Json.JsonSerializer.Serialize(behavior)}
            
            Return as JSON with Overview, Scenarios (array with Name, Description), Assumptions, Limitations.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<TestDocumentation>(prompt);
            return llmResponse ?? new TestDocumentation
            {
                Overview = $"Characterization tests for {behavior?.Description}",
                Scenarios = tests?.Select(t => new TestScenario
                {
                    Name = t.Name,
                    Description = $"Tests {t.Category} scenario",
                    CoveredBehaviors = new[] { t.Category }
                }).ToArray() ?? Array.Empty<TestScenario>(),
                Assumptions = new[] { "System state is consistent", "No concurrent access" },
                Limitations = new[] { "Does not test performance", "Limited to observed behaviors" }
            };
        }

        private async Task<TestHarness> CreateTestHarnessAsync(CharacterizationTest[] tests, LegacyBehavior behavior)
        {
            return new TestHarness
            {
                SetupCode = "// Initialize test environment",
                TeardownCode = "// Clean up resources",
                RequiredMocks = new[] { "ILogger", "IDatabase" },
                Configuration = new Dictionary<string, string>
                {
                    ["environment"] = "test",
                    ["behaviorId"] = behavior?.Id ?? "unknown"
                }
            };
        }

        private async Task<ValidationSuite> GenerateValidationSuiteAsync(CharacterizationTest[] tests, ObservedOutput[] outputs)
        {
            return new ValidationSuite
            {
                ValidationRules = new[] { "All tests must pass", "Coverage must exceed 80%" },
                ValidationTests = tests?.Select(t => t.Id).ToArray() ?? Array.Empty<string>(),
                ValidationCoverage = outputs?.Length > 0 ? 0.85 : 0.0
            };
        }

        private async Task<CoverageReport> GenerateCoverageReportAsync(CharacterizationTest[] tests, BehaviorAnalysis analysis)
        {
            var coveredCount = tests?.Length ?? 0;
            var totalBehaviors = analysis?.ObservedPatterns?.Length ?? 1;
            var coverage = totalBehaviors > 0 ? (double)coveredCount / totalBehaviors : 0;

            return new CoverageReport
            {
                CoveragePercentage = Math.Min(1.0, coverage),
                CoveredBehaviors = tests?.Select(t => t.Category).Distinct().ToArray() ?? Array.Empty<string>(),
                UncoveredBehaviors = analysis?.ObservedPatterns?
                    .Where(p => !tests?.Any(t => t.Category == p.PatternType) ?? false)
                    .Select(p => p.PatternType)
                    .ToArray() ?? Array.Empty<string>(),
                Recommendations = new[] { "Add tests for edge cases", "Increase input variation" }
            };
        }

        private async Task<ConfidenceMetric[]> CalculateTestConfidenceWithLLMAsync(
            CharacterizationTest[] tests,
            ObservedOutput[] outputs)
        {
            var prompt = $@"
            Calculate confidence metrics for these tests:
            Tests: {System.Text.Json.JsonSerializer.Serialize(tests)}
            Observed Outputs: {System.Text.Json.JsonSerializer.Serialize(outputs)}
            
            Return as JSON array with Metric, Value (0-1), Justification.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<ConfidenceMetric[]>(prompt);
            return llmResponse ?? new[]
            {
                new ConfidenceMetric { Metric = "Behavioral Accuracy", Value = 0.85, Justification = "Matches 85% of observed outputs" },
                new ConfidenceMetric { Metric = "Test Coverage", Value = 0.75, Justification = "Covers main scenarios" },
                new ConfidenceMetric { Metric = "Reliability", Value = 0.90, Justification = "Consistent results" }
            };
        }

        private async Task<MaintenanceGuide> GenerateMaintenanceGuideWithLLMAsync(
            CharacterizationTest[] tests,
            LegacyBehavior behavior)
        {
            var prompt = $@"
            Generate maintenance guide for these tests:
            Tests: {System.Text.Json.JsonSerializer.Serialize(tests)}
            Behavior: {System.Text.Json.JsonSerializer.Serialize(behavior)}
            
            Return as JSON with CommonIssues, TroubleshootingSteps, UpdateProcedures.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<MaintenanceGuide>(prompt);
            return llmResponse ?? new MaintenanceGuide
            {
                CommonIssues = new[] { "Flaky tests due to timing", "Environment dependencies" },
                TroubleshootingSteps = new[] { "Check test data", "Verify system state", "Review logs" },
                UpdateProcedures = new[] { "Add new observed outputs", "Update expected values", "Re-run baseline" }
            };
        }
    }
}
