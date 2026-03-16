

using Chapter_6.Exceptions;
using Chapter_6.Interfaces.BDDSupercharged.Interfaces;
using Chapter_6.Models;
using Chapter_6.Models.Requests;
using Chapter_6.Models.Responses;


namespace Chapter_6.Interfaces.Implementations
{
    // Services/Evolvers/ScenarioEvolver.cs
        public class ScenarioEvolver : IScenarioEvolver
        {
            private readonly ILogger<ScenarioEvolver> _logger;

            public ScenarioEvolver(ILogger<ScenarioEvolver> logger)
            {
                _logger = logger;
            }

            public async Task<EvolutionResult> EvolveScenarioAsync(
                BDDScenario scenario,
                string newInformation,
                BreakingChange[] breakingChanges,
                string evolutionStrategy)
            {
                _logger.LogInformation(
                    "Evolving scenario '{ScenarioTitle}' with strategy '{Strategy}' and {ChangeCount} breaking changes",
                    scenario.Title, evolutionStrategy, breakingChanges?.Length ?? 0);

                try
                {
                    // Validate inputs
                    if (scenario == null)
                        throw new ArgumentNullException(nameof(scenario));

                    if (string.IsNullOrEmpty(scenario.Title))
                        throw new ArgumentException("Scenario must have a title", nameof(scenario));

                    // Apply evolution based on strategy
                    var evolvedScenario = evolutionStrategy.ToLower() switch
                    {
                        "preserve-intent" => EvolveWithIntentPreservation(scenario, newInformation, breakingChanges),
                        "adapt-to-changes" => EvolveWithAdaptation(scenario, newInformation, breakingChanges),
                        "merge-and-refactor" => EvolveWithMergeAndRefactor(scenario, newInformation, breakingChanges),
                        _ => EvolveWithIntentPreservation(scenario, newInformation, breakingChanges) // Default
                    };

                    // Calculate preservation score
                    var preservationScore = CalculatePreservationScore(scenario, evolvedScenario);

                    // Create evolution record
                    var evolutionRecord = CreateEvolutionRecord(scenario, evolvedScenario, breakingChanges, preservationScore);

                    // Check if preservation is below threshold
                    if (preservationScore < 0.5 && evolutionStrategy == "preserve-intent")
                    {
                        throw new IntentPreservationException(
                            $"Failed to preserve scenario intent (preservation score: {preservationScore:P0})",
                            scenario.Title,
                            new[] { "High number of breaking changes", "Conflicting requirements", "Ambiguous new information" });
                    }

                    _logger.LogDebug(
                        "Successfully evolved scenario '{ScenarioTitle}', preservation score: {PreservationScore:P0}",
                        scenario.Title, preservationScore);

                    return new EvolutionResult
                    {
                        EvolvedScenario = evolvedScenario,
                        EvolutionRecord = evolutionRecord,
                        PreservationScore = preservationScore
                    };
                }
                catch (Exception ex) when (ex is not IntentPreservationException)
                {
                    _logger.LogError(ex, "Failed to evolve scenario '{ScenarioTitle}'", scenario.Title);
                    throw new IntentPreservationException(
                        $"Failed to evolve scenario '{scenario.Title}' while preserving intent",
                        scenario.Title,
                        ex);
                }
            }

            private BDDScenario EvolveWithIntentPreservation(BDDScenario scenario, string newInformation, BreakingChange[] breakingChanges)
            {
                var evolved = new BDDScenario
                {
                    Title = $"{scenario.Title} (Evolved)",
                    Description = UpdateDescription(scenario.Description, newInformation),
                    Tags = UpdateTags(scenario.Tags, breakingChanges, "preserved"),
                    Examples = UpdateExamples(scenario.Examples, newInformation)
                };

                // Evolve Given steps
                evolved.Given = EvolveSteps(
                    scenario.Given,
                    breakingChanges,
                    "Given",
                    (step, change) => !ShouldRemoveGivenStep(step, change));

                // Evolve When steps
                evolved.When = EvolveSteps(
                    scenario.When,
                    breakingChanges,
                    "When",
                    (step, change) => !ShouldRemoveWhenStep(step, change));

                // Evolve Then steps
                evolved.Then = EvolveSteps(
                    scenario.Then,
                    breakingChanges,
                    "Then",
                    (step, change) => !ShouldRemoveThenStep(step, change));

                return evolved;
            }

            private BDDScenario EvolveWithAdaptation(BDDScenario scenario, string newInformation, BreakingChange[] breakingChanges)
            {
                var evolved = new BDDScenario
                {
                    Title = $"{scenario.Title} (Adapted)",
                    Description = AdaptDescription(scenario.Description, newInformation, breakingChanges),
                    Tags = UpdateTags(scenario.Tags, breakingChanges, "adapted"),
                    Examples = AdaptExamples(scenario.Examples, newInformation, breakingChanges)
                };

                // Adapt Given steps more aggressively
                evolved.Given = AdaptSteps(
                    scenario.Given,
                    breakingChanges,
                    "Given");

                // Adapt When steps more aggressively
                evolved.When = AdaptSteps(
                    scenario.When,
                    breakingChanges,
                    "When");

                // Adapt Then steps more aggressively
                evolved.Then = AdaptSteps(
                    scenario.Then,
                    breakingChanges,
                    "Then");

                return evolved;
            }

            private BDDScenario EvolveWithMergeAndRefactor(BDDScenario scenario, string newInformation, BreakingChange[] breakingChanges)
            {
                var evolved = new BDDScenario
                {
                    Title = $"{scenario.Title} (Refactored)",
                    Description = RefactorDescription(scenario.Description, newInformation),
                    Tags = UpdateTags(scenario.Tags, breakingChanges, "refactored"),
                    Examples = RefactorExamples(scenario.Examples, breakingChanges)
                };

                // Refactor Given steps (merge similar steps, remove duplicates)
                evolved.Given = RefactorSteps(scenario.Given, "Given");

                // Refactor When steps
                evolved.When = RefactorSteps(scenario.When, "When");

                // Refactor Then steps
                evolved.Then = RefactorSteps(scenario.Then, "Then");

                return evolved;
            }

            private string UpdateDescription(string originalDescription, string newInformation)
            {
                if (string.IsNullOrWhiteSpace(newInformation))
                    return originalDescription;

                return $"{originalDescription}\n\nEvolution Notes: {newInformation}";
            }

            private string AdaptDescription(string originalDescription, string newInformation, BreakingChange[] breakingChanges)
            {
                var adapted = originalDescription;

                if (!string.IsNullOrWhiteSpace(newInformation))
                {
                    adapted += $"\n\nAdapted to: {newInformation}";
                }

                if (breakingChanges?.Length > 0)
                {
                    var changeDescriptions = breakingChanges
                        .Select(bc => $"- {bc.Type}: {bc.Description}")
                        .ToArray();

                    adapted += $"\n\nBreaking Changes Addressed:\n{string.Join("\n", changeDescriptions)}";
                }

                return adapted;
            }

            private string RefactorDescription(string originalDescription, string newInformation)
            {
                return $"[Refactored] {originalDescription}\n\nRefactoring rationale: {newInformation ?? "Improved clarity and maintainability"}";
            }

            private string[] UpdateTags(string[] originalTags, BreakingChange[] breakingChanges, string evolutionType)
            {
                var updatedTags = originalTags?.ToList() ?? new List<string>();

                // Add evolution type tag
                updatedTags.Add($"evolved-{evolutionType}");
                updatedTags.Add($"updated-{DateTime.UtcNow:yyyyMMdd}");

                // Add tags for breaking change types
                if (breakingChanges?.Length > 0)
                {
                    foreach (var change in breakingChanges)
                    {
                        if (change.ImpactLevel == "high")
                            updatedTags.Add("breaking-change-high");
                        else if (change.ImpactLevel == "medium")
                            updatedTags.Add("breaking-change-medium");

                        updatedTags.Add($"change-{change.Type}");
                    }
                }

                return updatedTags.Distinct().ToArray();
            }

            private string[] UpdateExamples(string[] originalExamples, string newInformation)
            {
                var examples = originalExamples?.ToList() ?? new List<string>();

                if (!string.IsNullOrWhiteSpace(newInformation) && newInformation.Contains("example", StringComparison.OrdinalIgnoreCase))
                {
                    examples.Add($"New example based on: {newInformation}");
                }

                return examples.ToArray();
            }

            private string[] AdaptExamples(string[] originalExamples, string newInformation, BreakingChange[] breakingChanges)
            {
                var examples = originalExamples?.ToList() ?? new List<string>();

                // Add adaptation notes
                examples.Add("=== Adaptation Notes ===");

                if (!string.IsNullOrWhiteSpace(newInformation))
                {
                    examples.Add($"Adapted to incorporate: {newInformation}");
                }

                if (breakingChanges?.Length > 0)
                {
                    examples.Add($"Addresses {breakingChanges.Length} breaking changes");
                }

                return examples.ToArray();
            }

            private string[] RefactorExamples(string[] originalExamples, BreakingChange[] breakingChanges)
            {
                if (originalExamples == null || originalExamples.Length == 0)
                    return new[] { "Refactored for clarity and consistency" };

                // Simplify examples
                return originalExamples
                    .Select(example => example.Length > 100 ? example.Substring(0, 97) + "..." : example)
                    .ToArray();
            }

            private string[] EvolveSteps(
                string[] originalSteps,
                BreakingChange[] breakingChanges,
                string stepType,
                Func<string, BreakingChange, bool> shouldKeepStep)
            {
                if (originalSteps == null || originalSteps.Length == 0)
                    return Array.Empty<string>();

                var evolvedSteps = new List<string>();

                foreach (var step in originalSteps)
                {
                    var evolvedStep = step;
                    var shouldKeep = true;

                    // Apply breaking changes
                    if (breakingChanges != null)
                    {
                        foreach (var change in breakingChanges)
                        {
                            if (!shouldKeepStep(step, change))
                            {
                                shouldKeep = false;
                                break;
                            }

                            evolvedStep = ApplyBreakingChange(evolvedStep, change, stepType);
                        }
                    }

                    if (shouldKeep)
                    {
                        evolvedSteps.Add(evolvedStep);
                    }
                }

                // Add new steps if needed
                if (breakingChanges != null)
                {
                    foreach (var change in breakingChanges.Where(bc => bc.Type == "addition"))
                    {
                        evolvedSteps.Add($"{stepType} {change.Description}");
                    }
                }

                return evolvedSteps.ToArray();
            }

            private string[] AdaptSteps(string[] originalSteps, BreakingChange[] breakingChanges, string stepType)
            {
                if (originalSteps == null || originalSteps.Length == 0)
                    return Array.Empty<string>();

                var adaptedSteps = new List<string>();

                foreach (var step in originalSteps)
                {
                    var adaptedStep = step;

                    // Apply adaptations more aggressively
                    if (breakingChanges != null)
                    {
                        foreach (var change in breakingChanges)
                        {
                            adaptedStep = AdaptStepToChange(adaptedStep, change, stepType);
                        }
                    }

                    adaptedSteps.Add(adaptedStep);
                }

                // Add adaptation markers
                if (adaptedSteps.Count > 0 && breakingChanges?.Length > 0)
                {
                    adaptedSteps.Add($"{stepType} the system adapts to {breakingChanges.Length} breaking changes");
                }

                return adaptedSteps.ToArray();
            }

            private string[] RefactorSteps(string[] originalSteps, string stepType)
            {
                if (originalSteps == null || originalSteps.Length == 0)
                    return Array.Empty<string>();

                // Merge similar steps
                var mergedSteps = new List<string>();
                var stepGroups = originalSteps
                    .GroupBy(step => GetStepKeyword(step))
                    .ToList();

                foreach (var group in stepGroups)
                {
                    if (group.Count() == 1)
                    {
                        mergedSteps.Add(group.First());
                    }
                    else
                    {
                        // Merge similar steps
                        var mergedStep = $"{stepType} {string.Join(" and ", group.Select(step => RemoveStepPrefix(step)))}";
                        mergedSteps.Add(mergedStep);
                    }
                }

                return mergedSteps.ToArray();
            }

            private string ApplyBreakingChange(string step, BreakingChange change, string stepType)
            {
                return change.Type.ToLower() switch
                {
                    "rename" => step.Replace(change.AffectedAreas?.FirstOrDefault() ?? "", change.Description),
                    "remove" => change.AffectedAreas?.Any(area => step.Contains(area)) == true
                        ? $"{stepType} [REMOVED: {change.Description}]"
                        : step,
                    "modify" => $"{step} [Modified: {change.Description}]",
                    "replace" => change.Description,
                    _ => step
                };
            }

            private string AdaptStepToChange(string step, BreakingChange change, string stepType)
            {
                var adapted = step;

                // Check if this step is affected by the change
                var isAffected = change.AffectedAreas?.Any(area =>
                    step.Contains(area, StringComparison.OrdinalIgnoreCase)) == true;

                if (isAffected)
                {
                    adapted = change.Type.ToLower() switch
                    {
                        "rename" => step.Replace(
                            change.AffectedAreas.First(),
                            $"{change.Description} (renamed)"),
                        "remove" => $"{stepType} [Adapted: previously {RemoveStepPrefix(step)}]",
                        "modify" => $"{step} [Adapted: {change.Description}]",
                        "replace" => $"{stepType} {change.Description} (replacement)",
                        _ => $"{step} [Adapted for {change.Type} change]"
                    };
                }

                return adapted;
            }

            private bool ShouldRemoveGivenStep(string step, BreakingChange change)
            {
                // Given steps are setup/context - rarely removed completely
                return change.Type == "remove" &&
                       change.AffectedAreas?.Any(area =>
                           step.Contains(area, StringComparison.OrdinalIgnoreCase) &&
                           change.Description.Contains("obsolete", StringComparison.OrdinalIgnoreCase)) == true;
            }

            private bool ShouldRemoveWhenStep(string step, BreakingChange change)
            {
                // When steps are actions - might be removed if action is no longer supported
                return change.Type == "remove" &&
                       change.AffectedAreas?.Any(area =>
                           step.Contains(area, StringComparison.OrdinalIgnoreCase)) == true;
            }

            private bool ShouldRemoveThenStep(string step, BreakingChange change)
            {
                // Then steps are outcomes - might be removed if outcome is no longer valid
                return change.Type == "remove" &&
                       change.ImpactLevel == "high" &&
                       change.AffectedAreas?.Any(area =>
                           step.Contains(area, StringComparison.OrdinalIgnoreCase)) == true;
            }

            private double CalculatePreservationScore(BDDScenario original, BDDScenario evolved)
            {
                var scores = new List<double>();

                // Compare titles
                scores.Add(CalculateStringSimilarity(original.Title, evolved.Title));

                // Compare steps
                if (original.Given.Length > 0 && evolved.Given.Length > 0)
                    scores.Add(CalculateStepPreservation(original.Given, evolved.Given));

                if (original.When.Length > 0 && evolved.When.Length > 0)
                    scores.Add(CalculateStepPreservation(original.When, evolved.When));

                if (original.Then.Length > 0 && evolved.Then.Length > 0)
                    scores.Add(CalculateStepPreservation(original.Then, evolved.Then));

                // Compare tags (if any)
                if (original.Tags.Length > 0)
                    scores.Add(CalculateTagPreservation(original.Tags, evolved.Tags));

                return scores.Count > 0 ? scores.Average() : 0.5; // Default to 0.5 if no comparison possible
            }

            private double CalculateStepPreservation(string[] originalSteps, string[] evolvedSteps)
            {
                if (originalSteps.Length == 0) return 0;

                var preservedCount = 0;
                foreach (var originalStep in originalSteps)
                {
                    if (evolvedSteps.Any(evolvedStep =>
                        evolvedStep.Contains(RemoveStepPrefix(originalStep), StringComparison.OrdinalIgnoreCase)))
                    {
                        preservedCount++;
                    }
                }

                return (double)preservedCount / originalSteps.Length;
            }

            private double CalculateTagPreservation(string[] originalTags, string[] evolvedTags)
            {
                if (originalTags.Length == 0) return 0;

                var preservedTags = originalTags.Intersect(evolvedTags, StringComparer.OrdinalIgnoreCase).Count();
                return (double)preservedTags / originalTags.Length;
            }

            private double CalculateStringSimilarity(string str1, string str2)
            {
                if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                    return 0;

                var words1 = str1.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var words2 = str2.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var commonWords = words1.Intersect(words2, StringComparer.OrdinalIgnoreCase).Count();
                var totalWords = Math.Max(words1.Length, words2.Length);

                return (double)commonWords / totalWords;
            }

            private EvolutionRecord CreateEvolutionRecord(
                BDDScenario original,
                BDDScenario evolved,
                BreakingChange[] breakingChanges,
                double preservationScore)
            {
                var changes = new List<string>();
                var rationale = new List<string>();

                // Record changes
                if (original.Title != evolved.Title)
                    changes.Add($"Title changed from '{original.Title}' to '{evolved.Title}'");

                if (original.Description != evolved.Description)
                    changes.Add("Description updated");

                // Record breaking changes addressed
                if (breakingChanges?.Length > 0)
                {
                    rationale.Add($"Addressed {breakingChanges.Length} breaking changes:");
                    rationale.AddRange(breakingChanges.Select(bc => $"- {bc.Type}: {bc.Description}"));
                }

                // Add preservation rationale
                rationale.Add($"Intent preservation score: {preservationScore:P0}");

                return new EvolutionRecord
                {
                    OriginalScenarioId = GetScenarioId(original),
                    EvolvedScenarioId = GetScenarioId(evolved),
                    Changes = changes.ToArray(),
                    Rationale = rationale.ToArray(),
                    PreservationScore = preservationScore,
                    EvolvedAt = DateTime.UtcNow
                };
            }

            private string GetScenarioId(BDDScenario scenario)
            {
                // Create a simple ID from title hash
                return Math.Abs(scenario.Title.GetHashCode()).ToString();
            }

            private string GetStepKeyword(string step)
            {
                // Extract main keyword from step
                var words = step.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return words.Length > 1 ? words[1].ToLower() : "unknown";
            }

            private string RemoveStepPrefix(string step)
            {
                // Remove Given/When/Then/And prefix
                var prefixes = new[] { "Given ", "When ", "Then ", "And " };
                foreach (var prefix in prefixes)
                {
                    if (step.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        return step.Substring(prefix.Length);
                    }
                }
                return step;
            }
        }
    
}
