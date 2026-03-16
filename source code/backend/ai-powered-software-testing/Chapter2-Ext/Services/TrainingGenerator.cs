//namespace Chapter2_Ext.Services
//{
//    public class TrainingGenerator
//    {
//    }
//}


using Chapter2_Ext.Models;
using System.Text;


namespace Chapter2_Ext.Services
{
    public class TrainingGenerator : ITrainingGenerator
    {
        private readonly ILogger<TrainingGenerator> _logger;
        private readonly Random _random = new();

        public TrainingGenerator(ILogger<TrainingGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<TrainingMaterials> CreateWorkshopMaterials(
            TestingPatternDto pattern,
            TrainingGenerationRequest config)
        {
            try
            {
                _logger.LogInformation("Creating workshop materials for pattern: {PatternId}",
                    pattern.Id);

                var materials = new TrainingMaterials
                {
                    PatternId = pattern.Id,
                    Audience = config.Audience,
                    Format = config.Format,
                    DurationMinutes = config.DurationMinutes,
                    Title = GenerateWorkshopTitle(pattern, config.Audience),
                    Prerequisites = config.Prerequisites ?? GenerateDefaultPrerequisites(config.Audience),
                    LearningObjectives = config.LearningObjectives,
                    Modules = GenerateWorkshopModules(pattern, config),
                    HandsOn = GenerateHandsOnSection(pattern, config.IncludeHandsOn),
                    Assessment = GenerateAssessment(pattern, config.Audience)
                };

                await Task.Delay(100); // Simulate processing time

                _logger.LogDebug("Generated workshop materials with {ModuleCount} modules",
                    materials.Modules.Count);

                return materials;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workshop materials for pattern: {PatternId}",
                    pattern.Id);
                throw;
            }
        }

        public async Task<TrainingMaterials> CreateQuickStartGuide(
            TestingPatternDto pattern,
            string audience)
        {
            try
            {
                _logger.LogInformation("Creating quick start guide for pattern: {PatternId}",
                    pattern.Id);

                var materials = new TrainingMaterials
                {
                    PatternId = pattern.Id,
                    Audience = audience,
                    Format = "quick-start",
                    DurationMinutes = 15,
                    Title = $"{pattern.Name} - Quick Start Guide",
                    Prerequisites = GenerateDefaultPrerequisites(audience),
                    LearningObjectives = new List<string>
                    {
                        $"Understand the purpose of {pattern.Name}",
                        $"Implement basic usage of {pattern.Name}",
                        $"Know where to find more detailed information"
                    },
                    Modules = GenerateQuickStartModules(pattern),
                    HandsOn = new HandsOnSection
                    {
                        Included = true,
                        Exercises = GenerateQuickStartExercises(pattern),
                        SetupInstructions = "No special setup required",
                        ExpectedOutcome = "Able to implement pattern in simple scenarios"
                    },
                    Assessment = new Assessment
                    {
                        QuizQuestions = new List<Question>
                        {
                            new Question
                            {
                                Text = $"What is the main purpose of {pattern.Name}?",
                                Options = new List<string>
                                {
                                    pattern.ProblemStatement,
                                    pattern.Solution,
                                    "To make testing harder",
                                    "To replace all existing tests"
                                },
                                CorrectAnswerIndex = 0,
                                Explanation = "The pattern addresses the stated problem"
                            }
                        },
                        PassingScore = 70
                    }
                };

                await Task.Delay(50); // Simulate processing time

                return materials;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quick start guide for pattern: {PatternId}",
                    pattern.Id);
                throw;
            }
        }

        private string GenerateWorkshopTitle(TestingPatternDto pattern, string audience)
        {
            var audienceSpecific = audience.ToLowerInvariant() switch
            {
                "developers" => "Developer Workshop",
                "qa" => "QA Specialist Workshop",
                "devops" => "DevOps Implementation Workshop",
                "managers" => "Manager Overview Workshop",
                _ => "Team Workshop"
            };

            return $"{pattern.Name.ToUpperInvariant()}: {audienceSpecific}";
        }

        private List<string> GenerateDefaultPrerequisites(string audience)
        {
            var prerequisites = new List<string>
            {
                "Basic understanding of software testing",
                "Familiarity with our application domain"
            };

            if (audience.ToLowerInvariant() == "developers")
            {
                prerequisites.Add("Programming experience in our tech stack");
                prerequisites.Add("Understanding of unit testing principles");
            }
            else if (audience.ToLowerInvariant() == "qa")
            {
                prerequisites.Add("Experience with test case design");
                prerequisites.Add("Knowledge of testing methodologies");
            }

            return prerequisites;
        }

        private List<Module> GenerateWorkshopModules(
            TestingPatternDto pattern,
            TrainingGenerationRequest config)
        {
            var modules = new List<Module>
            {
                new Module
                {
                    Title = "Introduction & Problem Space",
                    DurationMinutes = 10,
                    Content = GenerateIntroductionContent(pattern),
                    KeyPoints = new List<string>
                    {
                        $"Why {pattern.Name} was created",
                        "Common challenges without this pattern",
                        "Expected benefits and outcomes"
                    }
                },
                new Module
                {
                    Title = "Pattern Overview & Concepts",
                    DurationMinutes = 20,
                    Content = GeneratePatternOverview(pattern),
                    KeyPoints = pattern.Implementation.DosAndDonts.Take(5).ToList()
                },
                new Module
                {
                    Title = "Implementation Deep Dive",
                    DurationMinutes = 25,
                    Content = GenerateImplementationContent(pattern),
                    KeyPoints = pattern.Implementation.CodeExamples
                        .Take(3)
                        .Select(example => $"Example: {example.Substring(0, Math.Min(50, example.Length))}...")
                        .ToList()
                },
                new Module
                {
                    Title = "Best Practices & Pitfalls",
                    DurationMinutes = 15,
                    Content = GenerateBestPracticesContent(pattern),
                    KeyPoints = pattern.AiAssistance.CommonPitfalls.Take(5).ToList()
                },
                new Module
                {
                    Title = "Adoption Strategy & Metrics",
                    DurationMinutes = 10,
                    Content = GenerateAdoptionContent(pattern),
                    KeyPoints = new List<string>
                    {
                        $"Time savings: {pattern.AdoptionMetrics.EstimatedTimeSave}",
                        $"Error reduction: {pattern.AdoptionMetrics.ErrorReduction}",
                        $"Team satisfaction target: {pattern.AdoptionMetrics.TeamSatisfaction}/10"
                    }
                }
            };

            // Adjust durations based on total workshop time
            var totalModuleTime = modules.Sum(m => m.DurationMinutes);
            if (totalModuleTime > config.DurationMinutes)
            {
                var scaleFactor = (double)config.DurationMinutes / totalModuleTime;
                foreach (var module in modules)
                {
                    module.DurationMinutes = (int)(module.DurationMinutes * scaleFactor);
                }
            }

            return modules;
        }

        private List<Module> GenerateQuickStartModules(TestingPatternDto pattern)
        {
            return new List<Module>
            {
                new Module
                {
                    Title = "What & Why",
                    DurationMinutes = 3,
                    Content = $"The {pattern.Name} pattern addresses: {pattern.ProblemStatement}. Solution: {pattern.Solution}",
                    KeyPoints = new List<string>
                    {
                        "Problem addressed",
                        "Core solution approach",
                        "When to use this pattern"
                    }
                },
                new Module
                {
                    Title = "Getting Started",
                    DurationMinutes = 7,
                    Content = string.Join("\n\n", pattern.Implementation.CodeExamples.Take(2)),
                    KeyPoints = pattern.Implementation.DosAndDonts.Take(3).ToList()
                },
                new Module
                {
                    Title = "Next Steps",
                    DurationMinutes = 5,
                    Content = "Explore advanced usage, review common pitfalls, and consider automation options",
                    KeyPoints = new List<string>
                    {
                        "Review full documentation",
                        "Join pattern discussion group",
                        "Provide feedback on usage"
                    }
                }
            };
        }

        private HandsOnSection GenerateHandsOnSection(TestingPatternDto pattern, bool includeHandsOn)
        {
            if (!includeHandsOn)
            {
                return new HandsOnSection { Included = false };
            }

            return new HandsOnSection
            {
                Included = true,
                SetupInstructions = GenerateSetupInstructions(pattern),
                Exercises = GenerateExercises(pattern),
                ExpectedOutcome = "Participants will create 2-3 working implementations of the pattern"
            };
        }

        private string GenerateSetupInstructions(TestingPatternDto pattern)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Workshop Setup Instructions");
            sb.AppendLine();
            sb.AppendLine("## Required Software:");
            sb.AppendLine("- Development environment with our tech stack");
            sb.AppendLine("- Test runner configured");
            sb.AppendLine("- Access to sample code repository");
            sb.AppendLine();
            sb.AppendLine("## Preparation:");
            sb.AppendLine("1. Clone the workshop repository");
            sb.AppendLine($"2. Review the {pattern.Name} documentation");
            sb.AppendLine("3. Set up test database (if needed)");
            sb.AppendLine("4. Verify test runner is working");

            return sb.ToString();
        }

        private List<Exercise> GenerateExercises(TestingPatternDto pattern)
        {
            return new List<Exercise>
            {
                new Exercise
                {
                    Title = "Basic Implementation",
                    Description = $"Implement the {pattern.Name} pattern for a simple scenario",
                    SolutionHint = "Start with the basic code example and adapt it",
                    Solution = pattern.Implementation.CodeExamples.FirstOrDefault() ?? "Refer to documentation"
                },
                new Exercise
                {
                    Title = "Edge Case Handling",
                    Description = $"Extend the pattern to handle an edge case in {pattern.Area}",
                    SolutionHint = "Consider boundary conditions and error scenarios",
                    Solution = "Implement additional validation and error handling"
                },
                new Exercise
                {
                    Title = "Pattern Optimization",
                    Description = $"Optimize an existing implementation using {pattern.Name} best practices",
                    SolutionHint = "Review the DOs and DON'Ts list",
                    Solution = "Refactor to follow all pattern guidelines"
                }
            };
        }

        private List<Exercise> GenerateQuickStartExercises(TestingPatternDto pattern)
        {
            return new List<Exercise>
            {
                new Exercise
                {
                    Title = "Hello World Implementation",
                    Description = $"Create a minimal implementation of {pattern.Name}",
                    SolutionHint = "Use the first code example as a starting point",
                    Solution = pattern.Implementation.CodeExamples.FirstOrDefault() ?? "Basic implementation code"
                }
            };
        }

        private Assessment GenerateAssessment(TestingPatternDto pattern, string audience)
        {
            var questions = new List<Question>
            {
                new Question
                {
                    Text = $"What problem does {pattern.Name} primarily solve?",
                    Options = new List<string>
                    {
                        pattern.ProblemStatement,
                        "A completely different problem",
                        "No specific problem",
                        "Only technical debt"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "The pattern is designed to address the stated problem"
                },
                new Question
                {
                    Text = "Which of these is a key DO guideline for this pattern?",
                    Options = pattern.Implementation.DosAndDonts
                        .Where(g => g.StartsWith("DO:", StringComparison.OrdinalIgnoreCase))
                        .Take(3)
                        .Concat(new[] { "DO: Ignore all guidelines" })
                        .ToList(),
                    CorrectAnswerIndex = 0,
                    Explanation = "Follow the established DO guidelines"
                },
                new Question
                {
                    Text = "What is the expected repeatability score for this pattern?",
                    Options = new List<string>
                    {
                        $"{pattern.QualityIndicators.RepeatabilityScore}%",
                        "50%",
                        "100%",
                        "Not measured"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = $"The pattern targets {pattern.QualityIndicators.RepeatabilityScore}% repeatability"
                }
            };

            return new Assessment
            {
                QuizQuestions = questions,
                PracticalTask = new PracticalTask
                {
                    Description = audience.ToLowerInvariant() == "developers"
                        ? $"Implement {pattern.Name} for a real-world scenario in your codebase"
                        : $"Design test cases using {pattern.Name} for a new feature",
                    Requirements = new List<string>
                    {
                        "Follow all pattern guidelines",
                        "Include proper assertions",
                        "Document any deviations",
                        "Peer review required"
                    },
                    SuccessCriteria = new List<string>
                    {
                        "Implementation follows pattern",
                        "All tests pass",
                        "Code review approved",
                        "Pattern metrics collected"
                    }
                },
                PassingScore = 80
            };
        }

        private string GenerateIntroductionContent(TestingPatternDto pattern)
        {
            return $@"
# {pattern.Name} Pattern Introduction

## The Problem
{pattern.ProblemStatement}

## Why It Matters
Without a standardized approach, teams face:
- Inconsistent test quality
- Difficult test maintenance
- Knowledge silos
- Slower development cycles

## The Solution
{pattern.Solution}

## Target Audience
This pattern is designed for teams working with {pattern.Area} who need consistent, maintainable testing approaches.
";
        }

        private string GeneratePatternOverview(TestingPatternDto pattern)
        {
            return $@"
# {pattern.Name} Pattern Overview

## Core Concepts
- **Standardization**: Consistent approach across all tests
- **Maintainability**: Easy to update and extend
- **Repeatability**: {pattern.QualityIndicators.RepeatabilityScore}% score target
- **Learnability**: {pattern.QualityIndicators.LearningCurve} learning curve

## Key Components
1. **Code Examples**: {pattern.Implementation.CodeExamples.Count} provided examples
2. **Configuration**: {pattern.Implementation.Configuration?.Count ?? 0} configurable parameters
3. **Guidelines**: {pattern.Implementation.DosAndDonts.Count} DOs and DON'Ts
4. **AI Assistance**: {pattern.AiAssistance.PromptTemplates.Count} prompt templates

## Quality Indicators
- Repeatability: {pattern.QualityIndicators.RepeatabilityScore}%
- Learning Curve: {pattern.QualityIndicators.LearningCurve}
- Maintenance Cost: {pattern.QualityIndicators.MaintenanceCost}
";
        }

        private string GenerateImplementationContent(TestingPatternDto pattern)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Implementation Deep Dive");
            sb.AppendLine();
            sb.AppendLine("## Basic Implementation");
            sb.AppendLine("```csharp");
            sb.AppendLine(pattern.Implementation.CodeExamples.FirstOrDefault() ?? "// Implementation code");
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("## Configuration Options");
            foreach (var config in pattern.Implementation.Configuration ?? new Dictionary<string, object>())
            {
                sb.AppendLine($"- **{config.Key}**: {config.Value}");
            }
            sb.AppendLine();
            sb.AppendLine("## Common Variations");
            for (int i = 1; i < Math.Min(3, pattern.Implementation.CodeExamples.Count); i++)
            {
                sb.AppendLine($"### Variation {i}");
                sb.AppendLine("```csharp");
                sb.AppendLine(pattern.Implementation.CodeExamples[i]);
                sb.AppendLine("```");
            }

            return sb.ToString();
        }

        private string GenerateBestPracticesContent(TestingPatternDto pattern)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Best Practices & Common Pitfalls");
            sb.AppendLine();
            sb.AppendLine("## DOs:");
            foreach (var guideline in pattern.Implementation.DosAndDonts.Where(g => g.StartsWith("DO:", StringComparison.OrdinalIgnoreCase)))
            {
                sb.AppendLine($"- {guideline}");
            }
            sb.AppendLine();
            sb.AppendLine("## DON'Ts:");
            foreach (var guideline in pattern.Implementation.DosAndDonts.Where(g => g.StartsWith("DON'T:", StringComparison.OrdinalIgnoreCase)))
            {
                sb.AppendLine($"- {guideline}");
            }
            sb.AppendLine();
            sb.AppendLine("## Common Pitfalls:");
            foreach (var pitfall in pattern.AiAssistance.CommonPitfalls)
            {
                sb.AppendLine($"- {pitfall}");
            }
            sb.AppendLine();
            sb.AppendLine("## AI Prompt Templates:");
            foreach (var prompt in pattern.AiAssistance.PromptTemplates)
            {
                sb.AppendLine($"- `{prompt}`");
            }

            return sb.ToString();
        }

        private string GenerateAdoptionContent(TestingPatternDto pattern)
        {
            return $@"
# Adoption Strategy & Metrics

## Expected Benefits
- **Time Savings**: {pattern.AdoptionMetrics.EstimatedTimeSave}
- **Error Reduction**: {pattern.AdoptionMetrics.ErrorReduction}
- **Team Satisfaction**: Target {pattern.AdoptionMetrics.TeamSatisfaction}/10

## Rollout Plan
1. **Pilot Phase**: Select 1-2 teams for initial adoption
2. **Feedback Collection**: Gather usage feedback and refine
3. **Documentation**: Update based on real-world usage
4. **Full Rollout**: Deploy across all relevant teams

## Success Metrics
- Pattern adoption rate
- Test creation time reduction
- Defect escape rate
- Team satisfaction scores

## Support Resources
- Pattern documentation
- Community forum
- Office hours
- Dedicated Slack channel
";
        }
    }
}