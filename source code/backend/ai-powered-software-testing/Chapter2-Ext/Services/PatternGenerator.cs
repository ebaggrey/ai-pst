

using Chapter2_Ext;
using Chapter2_Ext.Models;


namespace PatterChapter2_Ext.Services
{
    public class PatternGenerator : IPatternGenerator
    {
        private readonly ILogger<PatternGenerator> _logger;

        public PatternGenerator(ILogger<PatternGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<TestingPatternDto> GeneratePatternFromExamples(
            string area, List<TestExampleDto> examples)
        {
            _logger.LogInformation("Generating pattern from {Count} examples for area: {Area}",
                examples.Count, area);

            // Analyze examples to extract common patterns
            var commonElements = ExtractCommonElements(examples);

            var pattern = new TestingPatternDto
            {
                Name = GeneratePatternName(area, commonElements),
                Area = area,
                ProblemStatement = GenerateProblemStatement(area, examples),
                Solution = GenerateSolution(commonElements),
                Implementation = GenerateImplementation(examples),
                AiAssistance = GenerateAiAssistance(examples)
            };

            await System.Threading.Tasks.Task.Delay(100); // Simulate async processing

            return pattern;
        }

        public async Task<TestingPatternDto> EnhancePatternWithAI(TestingPatternDto pattern)
        {
            _logger.LogInformation("Enhancing pattern {PatternId} with AI", pattern.Id);

            // Simulate AI enhancement
            pattern.AiAssistance.PromptTemplates.AddRange(new[]
            {
                $"Generate test for {pattern.Area} scenario:",
                $"Validate {pattern.Name} pattern implementation:",
                $"Optimize {pattern.Area} test cases using:"
            });

            pattern.AiAssistance.CommonPitfalls.AddRange(new[]
            {
                "Not handling edge cases properly",
                "Missing assertions for critical paths",
                "Overcomplicating the test setup"
            });

            await Task.Delay(150); // Simulate AI processing time

            return pattern;
        }

        private List<string> ExtractCommonElements(List<TestExampleDto> examples)
        {
            var commonSteps = new List<string>();

            foreach (var example in examples)
            {
                // Extract common patterns from test examples
                if (example.Input.Contains("verify") || example.ExpectedOutput.Contains("assert"))
                {
                    commonSteps.Add("verification-step");
                }

                if (example.Tags?.Contains("integration") == true)
                {
                    commonSteps.Add("integration-setup");
                }
            }

            return commonSteps.Distinct().ToList();
        }

        private string GeneratePatternName(string area, List<string> commonElements)
        {
            var mainElement = commonElements.FirstOrDefault() ?? "testing";
            return $"{area.Replace(" ", "-")}-{mainElement}-pattern".ToLowerInvariant();
        }

        private string GenerateProblemStatement(string area, List<TestExampleDto> examples)
        {
            var commonIssues = examples
                .Select(e => e.ActualOutput)
                .Distinct()
                .Take(3)
                .ToList();

            return $"Testing {area} is challenging due to {string.Join(", ", commonIssues)}. " +
                   $"Need a consistent approach that handles {examples.Count} different scenarios.";
        }

        private string GenerateSolution(List<string> commonElements)
        {
            return $"Implement a standardized approach using {string.Join(" and ", commonElements)} " +
                   "to ensure consistency and reduce maintenance overhead.";
        }

        private PatternImplementation GenerateImplementation(List<TestExampleDto> examples)
        {
            var codeExamples = examples
                .Take(3)
                .Select(e => $"// Example: {e.TestName}\n// Input: {e.Input}\n// Expected: {e.ExpectedOutput}")
                .ToList();

            return new PatternImplementation
            {
                CodeExamples = codeExamples,
                Configuration = new Dictionary<string, object>
                {
                    { "timeout", 30 },
                    { "retryCount", 3 },
                    { "assertionLibrary", "built-in" }
                },
                DosAndDonts = new List<string>
                {
                    "DO: Keep tests independent",
                    "DO: Use descriptive test names",
                    "DON'T: Hardcode test data",
                    "DON'T: Skip cleanup steps"
                }
            };
        }

        private AiAssistance GenerateAiAssistance(List<TestExampleDto> examples)
        {
            return new AiAssistance
            {
                PromptTemplates = new List<string>
                {
                    "Given the pattern, generate a test for:",
                    "Validate this implementation against the pattern:"
                },
                ValidationRules = new List<string>
                {
                    "Test must be repeatable",
                    "All assertions must be clear",
                    "Setup and teardown must be present"
                },
                CommonPitfalls = new List<string>
                {
                    "Forgetting to reset state",
                    "Testing implementation details",
                    "Over-mocking dependencies"
                }
            };
        }
    }
}
