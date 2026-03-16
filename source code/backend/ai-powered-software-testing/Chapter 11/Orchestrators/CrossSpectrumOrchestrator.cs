
// Orchestrators/CrossSpectrumOrchestrator.cs
using Chapter_11.Exceptions;
using Chapter_11.Interfaces;
using Chapter_11.Models.Analysis;
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;
using Chapter_11.Services;

namespace Chapter_11.Orchestrators
{
    public class CrossSpectrumOrchestrator : ICrossSpectrumOrchestrator
    {
        private readonly ILLMService _llmService;
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<CrossSpectrumOrchestrator> _logger;

        public CrossSpectrumOrchestrator(
            ILLMService llmService,
            IDatabaseService databaseService,
            ILogger<CrossSpectrumOrchestrator> logger)
        {
            _llmService = llmService;
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<Orchestration> OrchestrateTestingAsync(
            TestSuiteAnalysis suiteAnalysis,
            OrchestrationStrategy strategy,
            Models.Requests.ExecutionContext context)
        {
            try
            {
                _logger.LogInformation("Orchestrating {TestCount} tests with {Strategy} strategy",
                    suiteAnalysis.Tests?.Length ?? 0, strategy);

                // Check complexity
                if (suiteAnalysis.Tests != null && suiteAnalysis.Tests.Length > 100)
                {
                    throw new OrchestrationComplexityException(
                        "Test suite too complex for orchestration",
                        new[] { $"Too many tests: {suiteAnalysis.Tests.Length}", "Complex test dependencies detected" },
                        new[] { "Batch tests into smaller groups", "Use incremental execution strategy", "Consider parallel execution with careful dependency management" }
                    );
                }

                // Use LLM to optimize test execution order
                var prompt = $"Create optimal test execution plan for {suiteAnalysis.Tests?.Length ?? 0} tests using {strategy} strategy. Consider dependencies and resource constraints.";
                var executionPlan = await _llmService.GenerateCompletionAsync(prompt);

                var executionPlans = suiteAnalysis.Tests?.Select((t, index) => new TestExecutionPlan
                {
                    TestId = t.Id,
                    Order = CalculateExecutionOrder(t, suiteAnalysis.Dependencies, index + 1),
                    Dependencies = GetTestDependencies(t.Id, suiteAnalysis.Dependencies)
                }).ToArray() ?? Array.Empty<TestExecutionPlan>();

                // Optimize execution order based on strategy
                executionPlans = OptimizeExecutionOrder(executionPlans, strategy);

                var orchestration = new Orchestration
                {
                    Id = Guid.NewGuid().ToString(),
                    Strategy = strategy,
                    ExecutionPlans = executionPlans
                };

                // Save orchestration plan
                await _databaseService.SaveAsync(orchestration);

                return orchestration;
            }
            catch (OrchestrationComplexityException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to orchestrate testing");
                throw new OrchestrationComplexityException(
                    "Unexpected error during test orchestration",
                    new[] { ex.Message },
                    new[] { "Simplify test suite structure", "Contact support for assistance" }
                );
            }
        }

        private int CalculateExecutionOrder(Test test, TestDependency[] dependencies, int defaultOrder)
        {
            if (dependencies == null) return defaultOrder;

            var hasDependencies = dependencies.Any(d => d.TestId == test.Id);
            return hasDependencies ? defaultOrder + 100 : defaultOrder; // Dependencies run later
        }

        private string[] GetTestDependencies(string testId, TestDependency[] dependencies)
        {
            if (dependencies == null) return Array.Empty<string>();

            return dependencies
                .Where(d => d.TestId == testId)
                .Select(d => d.DependsOnTestId)
                .ToArray();
        }

        private TestExecutionPlan[] OptimizeExecutionOrder(TestExecutionPlan[] plans, OrchestrationStrategy strategy)
        {
            return strategy switch
            {
                OrchestrationStrategy.Sequential => plans.OrderBy(p => p.Order).ToArray(),
                OrchestrationStrategy.Parallel => plans.OrderBy(p => p.Dependencies.Length).ToArray(),
                OrchestrationStrategy.PriorityBased => plans.OrderBy(p => p.Order).ThenBy(p => p.Dependencies.Length).ToArray(),
                _ => plans
            };
        }
    }
}