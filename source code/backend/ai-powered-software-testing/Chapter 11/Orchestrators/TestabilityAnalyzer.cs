
// Orchestrators/TestabilityAnalyzer.cs
using Chapter_11.Exceptions;
using Chapter_11.Interfaces;
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;
using Chapter_11.Services;

namespace Chapter_11.Orchestrators
{
    public class TestabilityAnalyzer : ITestabilityAnalyzer
    {
        private readonly ILLMService _llmService;
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<TestabilityAnalyzer> _logger;

        public TestabilityAnalyzer(
            ILLMService llmService,
            IDatabaseService databaseService,
            ILogger<TestabilityAnalyzer> logger)
        {
            _llmService = llmService;
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<TestabilityAnalysis> AnalyzeTestabilityAsync(
            Codebase codebase,
            TestabilityFramework framework,
            AnalysisDepth depth)
        {
            try
            {
                _logger.LogInformation("Analyzing testability with depth {Depth}", depth);

                // Check framework compatibility
                if (!framework.SupportedLanguages.Contains(codebase.Files.FirstOrDefault()?.Language))
                {
                    throw new TestabilityFrameworkException(
                        "Framework incompatible with codebase",
                        new[] { "Language not supported" },
                        new[] { codebase.Files.FirstOrDefault()?.Language }
                    );
                }

                var prompt = $"Analyze testability of codebase with {codebase.TotalLines} lines using {framework.Name}";
                var analysis = await _llmService.GenerateCompletionAsync(prompt);

                return new TestabilityAnalysis
                {
                    Id = Guid.NewGuid().ToString(),
                    CodeSmells = new CodeSmell[0],
                    DependencyIssues = new DependencyIssue[0],
                    ComplexityMetrics = new[]
                    {
                        new ComplexityMetric
                        {
                            Name = "Cyclomatic Complexity",
                            Value = 45,
                            Threshold = 50,
                            ExceedsThreshold = false
                        }
                    }
                };
            }
            catch (TestabilityFrameworkException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to analyze testability");
                throw;
            }
        }
    }
}