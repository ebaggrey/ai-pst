//namespace Chapter_8.Services
//{
//    public class CharacterizationTestCreator
//    {
//    }
//}

// Services/CharacterizationTestCreator.cs
using Chapter_8.Exceptions;
using Chapter_8.Interfaces;
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;

namespace Chapter_8.Services
{
    public class CharacterizationTestCreator : ICharacterizationTestCreator
    {
        private readonly ILogger<CharacterizationTestCreator> _logger;

        public CharacterizationTestCreator(ILogger<CharacterizationTestCreator> logger)
        {
            _logger = logger;
        }

        public async Task<CharacterizationTest[]> CreateCharacterizationTestsAsync(
            BehaviorAnalysis behaviorAnalysis,
            LegacyBehavior legacyBehavior,
            double coverageGoal,
            bool includeEdgeCases)
        {
            try
            {
                _logger.LogInformation("Creating characterization tests for behavior {BehaviorId}",
                    legacyBehavior.Id);

                // Simulate test creation
                await System.Threading.Tasks.Task.Delay(600);

                // Check for ambiguity
                if (behaviorAnalysis.BehaviorStability < 0.3)
                {
                    throw new BehaviorAmbiguityException(
                        "Behavior is too unstable for characterization",
                        new[] { "Input/output mapping inconsistent", "Multiple possible interpretations" },
                        new[] { "Provide more examples", "Document expected behavior" })
                    {
                        AmbiguityAreas = new[] { "Input domain", "Output validation" },
                        ClarificationQuestions = new[] { "What should happen with null inputs?", "How to handle edge cases?" }
                    };
                }

                var tests = new List<CharacterizationTest>();

                // Generate tests from observed patterns
                foreach (var pattern in behaviorAnalysis.ObservedPatterns ?? Array.Empty<ObservedPattern>())
                {
                    tests.Add(new CharacterizationTest
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"Test_{pattern.PatternType}_{pattern.OccurrenceCount}",
                        TestCode = GenerateTestCode(pattern, legacyBehavior),
                        Inputs = new[] { "sample input" },
                        ExpectedOutput = "expected output",
                        Category = pattern.PatternType
                    });
                }

                // Add edge cases if requested
                if (includeEdgeCases && behaviorAnalysis.EdgeCases != null)
                {
                    foreach (var edgeCase in behaviorAnalysis.EdgeCases)
                    {
                        tests.Add(new CharacterizationTest
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = $"EdgeCase_{edgeCase}",
                            TestCode = $"// Test edge case: {edgeCase}",
                            Inputs = new[] { edgeCase },
                            ExpectedOutput = "edge case handling",
                            Category = "EdgeCase"
                        });
                    }
                }

                _logger.LogInformation("Created {TestCount} characterization tests", tests.Count);

                return tests.ToArray();
            }
            catch (Exception ex) when (ex is not BehaviorAmbiguityException)
            {
                _logger.LogError(ex, "Error creating characterization tests");
                throw;
            }
        }

        private string GenerateTestCode(ObservedPattern pattern, LegacyBehavior behavior)
        {
            return $@"
[Fact]
public void {pattern.PatternType}_ShouldProduceExpectedOutput()
{{
    // Arrange
    var behavior = new {behavior.Category}Behavior();
    var input = GetTestInput();

    // Act
    var result = behavior.Execute(input);

    // Assert
    Assert.Equal(expectedOutput, result);
}}";
        }
    }
}