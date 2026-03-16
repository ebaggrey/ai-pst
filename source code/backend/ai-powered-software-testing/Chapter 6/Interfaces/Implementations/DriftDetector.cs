
using Chapter_6.Exceptions;
using Chapter_6.Interfaces.BDDSupercharged.Interfaces;
using Chapter_6.Models.Requests;
using Chapter_6.Models.Responses;
using System.Text.RegularExpressions;


namespace Chapter_6.Interfaces.Implementations
{
   
        public class DriftDetector : IDriftDetector
        {
            private readonly ILogger<DriftDetector> _logger;

            public DriftDetector(ILogger<DriftDetector> logger)
            {
                _logger = logger;
            }

            public async Task<DriftFinding[]> DetectDriftUsingMethodAsync(
                BDDScenario[] documentedScenarios,
                ImplementedBehavior[] implementedBehavior,
                string method,
                double sensitivity)
            {
                _logger.LogInformation(
                    "Detecting drift using method '{Method}' with sensitivity {Sensitivity} for {ScenarioCount} scenarios",
                    method, sensitivity, documentedScenarios.Length);

                try
                {
                    if (documentedScenarios == null || documentedScenarios.Length == 0)
                        throw new ArgumentException("No documented scenarios provided", nameof(documentedScenarios));

                    if (implementedBehavior == null || implementedBehavior.Length == 0)
                        throw new ArgumentException("No implemented behavior provided", nameof(implementedBehavior));

                    ValidateSensitivity(sensitivity);

                    var findings = method.ToLower() switch
                    {
                        "semantic" => await DetectSemanticDriftAsync(documentedScenarios, implementedBehavior, sensitivity),
                        "structural" => await DetectStructuralDriftAsync(documentedScenarios, implementedBehavior, sensitivity),
                        "behavioral" => await DetectBehavioralDriftAsync(documentedScenarios, implementedBehavior, sensitivity),
                        "keyword" => await DetectKeywordDriftAsync(documentedScenarios, implementedBehavior, sensitivity),
                        "hybrid" => await DetectHybridDriftAsync(documentedScenarios, implementedBehavior, sensitivity),
                        _ => throw new DriftAnalysisException(
                            $"Unsupported drift detection method: {method}",
                            method,
                            documentedScenarios.Select(s => s.Title).ToArray())
                    };

                    _logger.LogDebug("Found {FindingCount} drift findings using method '{Method}'",
                        findings.Length, method);

                    return findings;
                }
                catch (Exception ex) when (!(ex is DriftAnalysisException))
                {
                    _logger.LogError(ex, "Failed to detect drift using method '{Method}'", method);
                    throw new DriftAnalysisException(
                        $"Drift detection failed for method '{method}'",
                        method,
                        documentedScenarios.Select(s => s.Title).ToArray(),
                        ex);
                }
            }

            private async Task<DriftFinding[]> DetectSemanticDriftAsync(
                BDDScenario[] documentedScenarios,
                ImplementedBehavior[] implementedBehavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                foreach (var scenario in documentedScenarios)
                {
                    var matchingBehavior = FindMatchingBehavior(scenario, implementedBehavior);

                    if (matchingBehavior == null)
                    {
                        findings.Add(CreateMissingImplementationFinding(scenario));
                        continue;
                    }

                    // Semantic analysis of Given steps
                    var givenFindings = AnalyzeStepSemantics(
                        scenario.Given,
                        matchingBehavior.Steps,
                        "Given",
                        scenario.Title,
                        sensitivity);
                    findings.AddRange(givenFindings);

                    // Semantic analysis of When steps
                    var whenFindings = AnalyzeStepSemantics(
                        scenario.When,
                        matchingBehavior.Steps,
                        "When",
                        scenario.Title,
                        sensitivity);
                    findings.AddRange(whenFindings);

                    // Semantic analysis of Then steps
                    var thenFindings = AnalyzeStepSemantics(
                        scenario.Then,
                        matchingBehavior.Outcomes,
                        "Then",
                        scenario.Title,
                        sensitivity);
                    findings.AddRange(thenFindings);

                    // Check for semantic contradictions
                    var contradictionFindings = DetectSemanticContradictions(
                        scenario,
                        matchingBehavior,
                        sensitivity);
                    findings.AddRange(contradictionFindings);
                }

                await Task.Delay(10); // Simulate async processing
                return findings.ToArray();
            }

            private async Task<DriftFinding[]> DetectStructuralDriftAsync(
                BDDScenario[] documentedScenarios,
                ImplementedBehavior[] implementedBehavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                foreach (var scenario in documentedScenarios)
                {
                    var matchingBehavior = FindMatchingBehavior(scenario, implementedBehavior);

                    if (matchingBehavior == null)
                    {
                        findings.Add(CreateMissingImplementationFinding(scenario));
                        continue;
                    }

                    // Step count analysis
                    var stepCountFinding = AnalyzeStepCounts(scenario, matchingBehavior, sensitivity);
                    if (stepCountFinding != null)
                        findings.Add(stepCountFinding);

                    // Step ordering analysis
                    var orderingFinding = AnalyzeStepOrdering(scenario, matchingBehavior, sensitivity);
                    if (orderingFinding != null)
                        findings.Add(orderingFinding);

                    // Step granularity analysis
                    var granularityFinding = AnalyzeStepGranularity(scenario, matchingBehavior, sensitivity);
                    if (granularityFinding != null)
                        findings.Add(granularityFinding);

                    // Tag structure analysis
                    var tagFinding = AnalyzeTagStructure(scenario, matchingBehavior, sensitivity);
                    if (tagFinding != null)
                        findings.Add(tagFinding);
                }

                await Task.Delay(10);
                return findings.ToArray();
            }

            private async Task<DriftFinding[]> DetectBehavioralDriftAsync(
                BDDScenario[] documentedScenarios,
                ImplementedBehavior[] implementedBehavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                foreach (var scenario in documentedScenarios)
                {
                    var matchingBehavior = FindMatchingBehavior(scenario, implementedBehavior);

                    if (matchingBehavior == null)
                    {
                        findings.Add(CreateMissingImplementationFinding(scenario));
                        continue;
                    }

                    // Edge case coverage
                    var edgeCaseFindings = AnalyzeEdgeCaseCoverage(scenario, matchingBehavior, sensitivity);
                    findings.AddRange(edgeCaseFindings);

                    // Outcome completeness
                    var outcomeFindings = AnalyzeOutcomeCompleteness(scenario, matchingBehavior, sensitivity);
                    findings.AddRange(outcomeFindings);

                    // Error handling behavior
                    var errorHandlingFindings = AnalyzeErrorHandling(scenario, matchingBehavior, sensitivity);
                    findings.AddRange(errorHandlingFindings);

                    // Performance behavior
                    var performanceFindings = AnalyzePerformanceBehavior(scenario, matchingBehavior, sensitivity);
                    findings.AddRange(performanceFindings);
                }

                await Task.Delay(10);
                return findings.ToArray();
            }

            private async Task<DriftFinding[]> DetectKeywordDriftAsync(
                BDDScenario[] documentedScenarios,
                ImplementedBehavior[] implementedBehavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                foreach (var scenario in documentedScenarios)
                {
                    var matchingBehavior = FindMatchingBehavior(scenario, implementedBehavior);

                    if (matchingBehavior == null)
                    {
                        findings.Add(CreateMissingImplementationFinding(scenario));
                        continue;
                    }

                    // Keyword extraction and comparison
                    var keywordFindings = AnalyzeKeywordDrift(scenario, matchingBehavior, sensitivity);
                    findings.AddRange(keywordFindings);

                    // Terminology consistency
                    var terminologyFindings = AnalyzeTerminologyConsistency(scenario, matchingBehavior, sensitivity);
                    findings.AddRange(terminologyFindings);

                    // Business vocabulary alignment
                    var vocabularyFindings = AnalyzeBusinessVocabulary(scenario, matchingBehavior, sensitivity);
                    findings.AddRange(vocabularyFindings);
                }

                await Task.Delay(10);
                return findings.ToArray();
            }

            private async Task<DriftFinding[]> DetectHybridDriftAsync(
                BDDScenario[] documentedScenarios,
                ImplementedBehavior[] implementedBehavior,
                double sensitivity)
            {
                // Combine multiple detection methods
                var allFindings = new List<DriftFinding>();

                var semanticFindings = await DetectSemanticDriftAsync(
                    documentedScenarios, implementedBehavior, sensitivity);
                allFindings.AddRange(semanticFindings);

                var structuralFindings = await DetectStructuralDriftAsync(
                    documentedScenarios, implementedBehavior, sensitivity);
                allFindings.AddRange(structuralFindings);

                var behavioralFindings = await DetectBehavioralDriftAsync(
                    documentedScenarios, implementedBehavior, sensitivity);
                allFindings.AddRange(behavioralFindings);

                // Consolidate similar findings
                var consolidated = ConsolidateFindings(allFindings);
                return consolidated.ToArray();
            }

            private ImplementedBehavior FindMatchingBehavior(BDDScenario scenario, ImplementedBehavior[] implementedBehavior)
            {
                // First try exact match by ScenarioId
                var exactMatch = implementedBehavior.FirstOrDefault(
                    ib => ib.ScenarioId == scenario.Title.GetHashCode().ToString());

                if (exactMatch != null)
                    return exactMatch;

                // Try fuzzy matching by title
                return implementedBehavior.FirstOrDefault(ib =>
                    ib.Steps.Any(step => step.Contains(scenario.Title, StringComparison.OrdinalIgnoreCase)) ||
                    IsTitleSimilar(ib.ScenarioId, scenario.Title));
            }

            private bool IsTitleSimilar(string behaviorId, string scenarioTitle)
            {
                // Simple similarity check - in production would use more sophisticated algorithm
                var scenarioHash = scenarioTitle.GetHashCode().ToString();
                return behaviorId.Contains(scenarioHash) ||
                       scenarioTitle.Contains(behaviorId, StringComparison.OrdinalIgnoreCase);
            }

            private DriftFinding CreateMissingImplementationFinding(BDDScenario scenario)
            {
                return new DriftFinding
                {
                    Type = "missing-implementation",
                    ScenarioId = GetScenarioIdentifier(scenario),
                    Description = $"No implementation found for scenario: '{scenario.Title}'",
                    Severity = "high",
                    Evidence = new[]
                    {
                    $"Documented scenario: {scenario.Title}",
                    $"Scenario has {scenario.Given.Length + scenario.When.Length + scenario.Then.Length} steps",
                    $"Tags: {string.Join(", ", scenario.Tags)}"
                },
                    Impact = new[]
                    {
                    "Untested functionality",
                    "Potential business risk",
                    "Documentation-Implementation mismatch"
                }
                };
            }

            private DriftFinding[] AnalyzeStepSemantics(
                string[] documentedSteps,
                string[] implementedSteps,
                string stepType,
                string scenarioTitle,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                foreach (var documentedStep in documentedSteps)
                {
                    var normalizedDocStep = NormalizeStep(documentedStep);
                    var semanticMatches = implementedSteps
                        .Where(implStep => CalculateSemanticSimilarity(normalizedDocStep, implStep) >= sensitivity)
                        .ToList();

                    if (semanticMatches.Count == 0)
                    {
                        // No semantic match found
                        findings.Add(new DriftFinding
                        {
                            Type = $"semantic-{stepType.ToLower()}-mismatch",
                            ScenarioId = GetScenarioIdentifier(scenarioTitle),
                            Description = $"{stepType} step semantically missing in implementation: '{documentedStep}'",
                            Severity = CalculateSeverity(stepType, 0),
                            Evidence = new[]
                            {
                            $"Documented step: {documentedStep}",
                            $"No semantically similar steps found in implementation"
                        },
                            Impact = new[]
                            {
                            $"Missing {stepType} behavior",
                            "Potential functionality gap"
                        }
                        });
                    }
                    else if (semanticMatches.Count > 1)
                    {
                        // Multiple possible matches - ambiguity
                        findings.Add(new DriftFinding
                        {
                            Type = $"ambiguous-{stepType.ToLower()}-implementation",
                            ScenarioId = GetScenarioIdentifier(scenarioTitle),
                            Description = $"{stepType} step has multiple possible implementations: '{documentedStep}'",
                            Severity = "medium",
                            Evidence = semanticMatches.Take(3).Select(m => $"Possible match: {m}").ToArray(),
                            Impact = new[]
                            {
                            "Implementation ambiguity",
                            "Potential inconsistent behavior"
                        }
                        });
                    }
                }

                return findings.ToArray();
            }

            private DriftFinding[] DetectSemanticContradictions(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                // Check for contradictions between documented and implemented behavior
                var allDocumentedText = string.Join(" ",
                    scenario.Given.Concat(scenario.When).Concat(scenario.Then));

                var allImplementedText = string.Join(" ",
                    behavior.Steps.Concat(behavior.Outcomes));

                var contradictions = FindSemanticContradictions(
                    allDocumentedText,
                    allImplementedText);

                foreach (var contradiction in contradictions)
                {
                    findings.Add(new DriftFinding
                    {
                        Type = "semantic-contradiction",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = $"Semantic contradiction found: {contradiction}",
                        Severity = "high",
                        Evidence = new[]
                        {
                        $"Documented context: {allDocumentedText[..Math.Min(100, allDocumentedText.Length)]}...",
                        $"Implemented context: {allImplementedText[..Math.Min(100, allImplementedText.Length)]}..."
                    },
                        Impact = new[]
                        {
                        "Conflicting requirements",
                        "Potential system inconsistency"
                    }
                    });
                }

                return findings.ToArray();
            }

            private DriftFinding AnalyzeStepCounts(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var documentedStepCount = scenario.Given.Length + scenario.When.Length + scenario.Then.Length;
                var implementedStepCount = behavior.Steps.Length + behavior.Outcomes.Length;

                var stepRatio = implementedStepCount > 0
                    ? (double)documentedStepCount / implementedStepCount
                    : 0;

                if (Math.Abs(1 - stepRatio) > (1 - sensitivity))
                {
                    return new DriftFinding
                    {
                        Type = "step-count-mismatch",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = $"Step count mismatch: Documented={documentedStepCount}, Implemented={implementedStepCount}",
                        Severity = stepRatio < 0.5 ? "high" : stepRatio < 0.8 ? "medium" : "low",
                        Evidence = new[]
                        {
                        $"Documented steps: {documentedStepCount}",
                        $"Implemented steps: {implementedStepCount}",
                        $"Ratio: {stepRatio:F2}"
                    },
                        Impact = new[]
                        {
                        "Maintenance complexity",
                        "Potential over/under implementation"
                    }
                    };
                }

                return null;
            }

            private DriftFinding AnalyzeStepOrdering(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                // Check if Given/When/Then order is preserved
                var implementedSequence = ExtractStepSequence(behavior);
                var documentedSequence = new[] { "Given", "When", "Then" };

                var orderingScore = CalculateOrderingSimilarity(documentedSequence, implementedSequence);

                if (orderingScore < sensitivity)
                {
                    return new DriftFinding
                    {
                        Type = "step-ordering-drift",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = "Step ordering differs from documented structure",
                        Severity = orderingScore < 0.3 ? "high" : "medium",
                        Evidence = new[]
                        {
                        $"Expected order: Given → When → Then",
                        $"Actual order: {string.Join(" → ", implementedSequence)}",
                        $"Ordering similarity: {orderingScore:F2}"
                    },
                        Impact = new[]
                        {
                        "Confusing test structure",
                        "Potential logical errors"
                    }
                    };
                }

                return null;
            }

            private DriftFinding AnalyzeStepGranularity(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var documentedGranularity = CalculateAverageStepLength(
                    scenario.Given.Concat(scenario.When).Concat(scenario.Then));

                var implementedGranularity = CalculateAverageStepLength(
                    behavior.Steps.Concat(behavior.Outcomes));

                var granularityRatio = implementedGranularity > 0
                    ? documentedGranularity / implementedGranularity
                    : 0;

                if (Math.Abs(1 - granularityRatio) > (1 - sensitivity))
                {
                    return new DriftFinding
                    {
                        Type = "step-granularity-drift",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = $"Step granularity differs significantly",
                        Severity = granularityRatio < 0.3 || granularityRatio > 3 ? "medium" : "low",
                        Evidence = new[]
                        {
                        $"Documented avg step length: {documentedGranularity:F0} chars",
                        $"Implemented avg step length: {implementedGranularity:F0} chars",
                        $"Granularity ratio: {granularityRatio:F2}"
                    },
                        Impact = new[]
                        {
                        "Different abstraction levels",
                        "Maintenance challenges"
                    }
                    };
                }

                return null;
            }

            private DriftFinding[] AnalyzeEdgeCaseCoverage(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                // Extract edge cases from examples
                var documentedEdgeCases = ExtractEdgeCases(scenario.Examples);
                var implementedEdgeCases = behavior.EdgeCases ?? Array.Empty<string>();

                // Find missing edge cases
                var missingEdgeCases = documentedEdgeCases
                    .Except(implementedEdgeCases, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (missingEdgeCases.Length > 0)
                {
                    findings.Add(new DriftFinding
                    {
                        Type = "missing-edge-cases",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = $"{missingEdgeCases.Length} documented edge cases not implemented",
                        Severity = missingEdgeCases.Length > 3 ? "high" : "medium",
                        Evidence = missingEdgeCases.Take(3).ToArray(),
                        Impact = new[]
                        {
                        "Incomplete testing",
                        "Potential undiscovered bugs"
                    }
                    });
                }

                // Find undocumented edge cases
                var undocumentedEdgeCases = implementedEdgeCases
                    .Except(documentedEdgeCases, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (undocumentedEdgeCases.Length > sensitivity * 10)
                {
                    findings.Add(new DriftFinding
                    {
                        Type = "undocumented-edge-cases",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = $"{undocumentedEdgeCases.Length} edge cases implemented but not documented",
                        Severity = "low",
                        Evidence = undocumentedEdgeCases.Take(3).ToArray(),
                        Impact = new[]
                        {
                        "Documentation lag",
                        "Knowledge gap"
                    }
                    });
                }

                return findings.ToArray();
            }

            private DriftFinding[] AnalyzeKeywordDrift(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                // Extract keywords from documented scenario
                var documentedKeywords = ExtractKeywords(
                    string.Join(" ", scenario.Given.Concat(scenario.When).Concat(scenario.Then)));

                // Extract keywords from implementation
                var implementedKeywords = ExtractKeywords(
                    string.Join(" ", behavior.Steps.Concat(behavior.Outcomes)));

                // Find missing keywords
                var missingKeywords = documentedKeywords
                    .Except(implementedKeywords, StringComparer.OrdinalIgnoreCase)
                    .Where(kw => kw.Length > 3) // Ignore very short keywords
                    .ToArray();

                if (missingKeywords.Length > sensitivity * 5)
                {
                    findings.Add(new DriftFinding
                    {
                        Type = "missing-keywords",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = $"{missingKeywords.Length} key terms missing in implementation",
                        Severity = missingKeywords.Length > 5 ? "medium" : "low",
                        Evidence = missingKeywords.Take(5).ToArray(),
                        Impact = new[]
                        {
                        "Semantic gap",
                        "Potential misunderstanding of requirements"
                    }
                    });
                }

                return findings.ToArray();
            }

            #region Helper Methods

            private string GetScenarioIdentifier(BDDScenario scenario)
            {
                return scenario.Title.GetHashCode().ToString();
            }

            private string GetScenarioIdentifier(string scenarioTitle)
            {
                return scenarioTitle.GetHashCode().ToString();
            }

            private void ValidateSensitivity(double sensitivity)
            {
                if (sensitivity < 0 || sensitivity > 1)
                    throw new ArgumentException("Sensitivity must be between 0 and 1", nameof(sensitivity));
            }

            private string NormalizeStep(string step)
            {
                // Remove Given/When/Then/And prefixes and normalize
                return Regex.Replace(step, @"^(Given|When|Then|And)\s+", "", RegexOptions.IgnoreCase)
                    .ToLower()
                    .Trim();
            }

            private double CalculateSemanticSimilarity(string text1, string text2)
            {
                // Simple semantic similarity calculation
                // In production, this would use NLP techniques or embeddings

                var words1 = text1.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var words2 = text2.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var commonWords = words1.Intersect(words2, StringComparer.OrdinalIgnoreCase).Count();
                var totalWords = words1.Length + words2.Length;

                return totalWords > 0 ? (2.0 * commonWords) / totalWords : 0;
            }

            private string CalculateSeverity(string stepType, double matchScore)
            {
                return stepType.ToLower() switch
                {
                    "then" when matchScore < 0.3 => "high", // Missing outcomes are critical
                    "when" when matchScore < 0.3 => "high", // Missing actions are critical
                    "given" when matchScore < 0.3 => "medium", // Missing setup is important but less critical
                    _ => matchScore < 0.5 ? "medium" : "low"
                };
            }

            private List<string> FindSemanticContradictions(string text1, string text2)
            {
                var contradictions = new List<string>();

                // Simple contradiction detection based on negation patterns
                var negationWords = new[] { "not", "no ", "never", "cannot", "can't", "won't", "doesn't" };
                var affirmationWords = new[] { "always", "must", "should", "will", "does" };

                foreach (var negation in negationWords)
                {
                    if (text1.Contains(negation) && text2.Contains(negation))
                    {
                        // Both contain negation - check for contradictory affirmation
                        foreach (var affirmation in affirmationWords)
                        {
                            if ((text1.Contains(affirmation) && !text2.Contains(affirmation)) ||
                                (!text1.Contains(affirmation) && text2.Contains(affirmation)))
                            {
                                contradictions.Add($"Contradiction between '{negation}' and '{affirmation}'");
                            }
                        }
                    }
                }

                return contradictions;
            }

            private string[] ExtractStepSequence(ImplementedBehavior behavior)
            {
                var sequence = new List<string>();

                foreach (var step in behavior.Steps.Concat(behavior.Outcomes))
                {
                    if (step.StartsWith("Given", StringComparison.OrdinalIgnoreCase))
                        sequence.Add("Given");
                    else if (step.StartsWith("When", StringComparison.OrdinalIgnoreCase))
                        sequence.Add("When");
                    else if (step.StartsWith("Then", StringComparison.OrdinalIgnoreCase))
                        sequence.Add("Then");
                    else if (step.StartsWith("And", StringComparison.OrdinalIgnoreCase))
                        sequence.Add("And");
                }

                return sequence.ToArray();
            }

            private double CalculateOrderingSimilarity(string[] expected, string[] actual)
            {
                if (expected.Length == 0 || actual.Length == 0)
                    return 0;

                var matches = 0;
                for (int i = 0; i < Math.Min(expected.Length, actual.Length); i++)
                {
                    if (i < actual.Length && expected[i] == actual[i])
                        matches++;
                }

                return (double)matches / Math.Max(expected.Length, actual.Length);
            }

            private double CalculateAverageStepLength(IEnumerable<string> steps)
            {
                var stepList = steps.ToList();
                if (stepList.Count == 0)
                    return 0;

                return stepList.Average(s => s.Length);
            }

            private string[] ExtractEdgeCases(string[] examples)
            {
                if (examples == null)
                    return Array.Empty<string>();

                return examples
                    .Where(example =>
                        example.Contains("edge", StringComparison.OrdinalIgnoreCase) ||
                        example.Contains("corner", StringComparison.OrdinalIgnoreCase) ||
                        example.Contains("boundary", StringComparison.OrdinalIgnoreCase) ||
                        example.Contains("exception", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }

            private string[] ExtractKeywords(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return Array.Empty<string>();

                var stopWords = new HashSet<string>
            {
                "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for",
                "of", "with", "by", "is", "are", "was", "were", "be", "been", "being"
            };

                return text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word.Trim().ToLower())
                    .Where(word => word.Length > 3 && !stopWords.Contains(word))
                    .Distinct()
                    .ToArray();
            }

            private DriftFinding AnalyzeTagStructure(BDDScenario scenario, ImplementedBehavior behavior, double sensitivity)
            {
                // Check if tags from documentation are reflected in implementation
                // (This is a simplified version - in reality would check test metadata)

                var documentedTags = scenario.Tags ?? Array.Empty<string>();
                var criticalTags = documentedTags
                    .Where(tag => tag.StartsWith("critical", StringComparison.OrdinalIgnoreCase) ||
                                 tag.StartsWith("smoke", StringComparison.OrdinalIgnoreCase) ||
                                 tag.StartsWith("regression", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (criticalTags.Length > 0)
                {
                    // In a real implementation, we would check if these tags are present
                    // in test execution metadata or test attributes
                    return new DriftFinding
                    {
                        Type = "tag-implementation-gap",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = "Critical scenario tags may not be reflected in test execution",
                        Severity = "medium",
                        Evidence = criticalTags.Take(3).Select(t => $"Critical tag: {t}").ToArray(),
                        Impact = new[]
                        {
                        "Missing test categorization",
                        "Potential test execution gaps"
                    }
                    };
                }

                return null;
            }

            private DriftFinding[] AnalyzeOutcomeCompleteness(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                // Check if all documented outcomes have corresponding implementations
                foreach (var outcome in scenario.Then)
                {
                    var hasMatchingOutcome = behavior.Outcomes.Any(implOutcome =>
                        CalculateSemanticSimilarity(outcome, implOutcome) >= sensitivity);

                    if (!hasMatchingOutcome)
                    {
                        findings.Add(new DriftFinding
                        {
                            Type = "missing-outcome-validation",
                            ScenarioId = GetScenarioIdentifier(scenario),
                            Description = $"Documented outcome not validated: '{outcome}'",
                            Severity = "high",
                            Evidence = new[] { $"Missing validation for: {outcome}" },
                            Impact = new[] { "Incomplete verification", "Potential quality issues" }
                        });
                    }
                }

                return findings.ToArray();
            }

            private DriftFinding[] AnalyzeErrorHandling(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                // Check if error scenarios are properly handled
                var errorKeywords = new[] { "error", "exception", "fail", "invalid", "reject" };
                var documentedErrors = scenario.Then
                    .Where(outcome => errorKeywords.Any(kw => outcome.Contains(kw, StringComparison.OrdinalIgnoreCase)))
                    .ToArray();

                if (documentedErrors.Length > 0)
                {
                    var implementedErrors = behavior.Outcomes
                        .Count(outcome => errorKeywords.Any(kw => outcome.Contains(kw, StringComparison.OrdinalIgnoreCase)));

                    if (implementedErrors < documentedErrors.Length * sensitivity)
                    {
                        findings.Add(new DriftFinding
                        {
                            Type = "insufficient-error-handling",
                            ScenarioId = GetScenarioIdentifier(scenario),
                            Description = "Error handling scenarios may not be fully implemented",
                            Severity = "medium",
                            Evidence = documentedErrors.Take(2).Select(e => $"Documented error case: {e}").ToArray(),
                            Impact = new[] { "Incomplete error coverage", "Potential system fragility" }
                        });
                    }
                }

                return findings.ToArray();
            }

            private DriftFinding[] AnalyzePerformanceBehavior(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                // Check for performance-related requirements
                var perfKeywords = new[] { "fast", "quick", "seconds", "milliseconds", "performance", "response time" };
                var documentedPerf = scenario.Then
                    .Where(outcome => perfKeywords.Any(kw => outcome.Contains(kw, StringComparison.OrdinalIgnoreCase)))
                    .ToArray();

                if (documentedPerf.Length > 0)
                {
                    // Check if performance validations are implemented
                    var hasPerformanceChecks = behavior.Outcomes.Any(outcome =>
                        outcome.Contains("performance", StringComparison.OrdinalIgnoreCase) ||
                        outcome.Contains("response time", StringComparison.OrdinalIgnoreCase));

                    if (!hasPerformanceChecks)
                    {
                        findings.Add(new DriftFinding
                        {
                            Type = "missing-performance-validation",
                            ScenarioId = GetScenarioIdentifier(scenario),
                            Description = "Performance requirements may not be validated",
                            Severity = "medium",
                            Evidence = documentedPerf.Take(2).Select(p => $"Performance requirement: {p}").ToArray(),
                            Impact = new[] { "Unvalidated performance", "Potential scalability issues" }
                        });
                    }
                }

                return findings.ToArray();
            }

            private DriftFinding[] AnalyzeTerminologyConsistency(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                // Check for terminology consistency
                var businessTerms = ExtractBusinessTerms(
                    string.Join(" ", scenario.Given.Concat(scenario.When).Concat(scenario.Then)));

                var implementationTerms = ExtractBusinessTerms(
                    string.Join(" ", behavior.Steps.Concat(behavior.Outcomes)));

                var inconsistentTerms = businessTerms
                    .Where(term => !implementationTerms.Contains(term, StringComparer.OrdinalIgnoreCase))
                    .ToArray();

                if (inconsistentTerms.Length > sensitivity * 3)
                {
                    findings.Add(new DriftFinding
                    {
                        Type = "terminology-inconsistency",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = "Business terminology not consistently used in implementation",
                        Severity = "low",
                        Evidence = inconsistentTerms.Take(3).ToArray(),
                        Impact = new[] { "Communication gaps", "Domain model misalignment" }
                    });
                }

                return findings.ToArray();
            }

            private string[] ExtractBusinessTerms(string text)
            {
                // Extract domain-specific terms (simplified)
                var domainPatterns = new[]
                {
                @"\b(user|customer|client)\b",
                @"\b(order|purchase|transaction)\b",
                @"\b(payment|invoice|receipt)\b",
                @"\b(account|profile|settings)\b",
                @"\b(product|item|service)\b"
            };

                var terms = new List<string>();
                foreach (var pattern in domainPatterns)
                {
                    var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
                    terms.AddRange(matches.Select(m => m.Value.ToLower()));
                }

                return terms.Distinct().ToArray();
            }

            private DriftFinding[] AnalyzeBusinessVocabulary(
                BDDScenario scenario,
                ImplementedBehavior behavior,
                double sensitivity)
            {
                var findings = new List<DriftFinding>();

                // Check if business vocabulary is properly reflected
                var businessObjects = ExtractBusinessObjects(scenario);
                var technicalTerms = ExtractTechnicalTerms(behavior);

                var missingBusinessConcepts = businessObjects
                    .Except(technicalTerms, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (missingBusinessConcepts.Length > 0)
                {
                    findings.Add(new DriftFinding
                    {
                        Type = "business-vocabulary-gap",
                        ScenarioId = GetScenarioIdentifier(scenario),
                        Description = "Business concepts not reflected in technical implementation",
                        Severity = "medium",
                        Evidence = missingBusinessConcepts.Take(3).ToArray(),
                        Impact = new[] { "Domain-implementation mismatch", "Maintenance complexity" }
                    });
                }

                return findings.ToArray();
            }

            private string[] ExtractBusinessObjects(BDDScenario scenario)
            {
                // Extract potential business objects from scenario
                var text = string.Join(" ",
                    scenario.Given.Concat(scenario.When).Concat(scenario.Then));

                // Look for noun phrases that might represent business objects
                var nouns = new List<string>();
                var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Length > 3 && char.IsUpper(words[i][0]))
                    {
                        nouns.Add(words[i]);
                    }
                }

                return nouns.Distinct().ToArray();
            }

            private string[] ExtractTechnicalTerms(ImplementedBehavior behavior)
            {
                var text = string.Join(" ",
                    behavior.Steps.Concat(behavior.Outcomes));

                // Look for technical terms (simplified)
                var techKeywords = new[]
                {
                "api", "endpoint", "database", "repository", "service",
                "controller", "model", "view", "validation", "authentication"
            };

                return techKeywords
                    .Where(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }

            private List<DriftFinding> ConsolidateFindings(List<DriftFinding> findings)
            {
                // Group similar findings
                var grouped = findings
                    .GroupBy(f => new { f.Type, f.ScenarioId })
                    .Select(g => new DriftFinding
                    {
                        Type = g.Key.Type,
                        ScenarioId = g.Key.ScenarioId,
                        Description = $"{g.Count()} findings of type '{g.Key.Type}'",
                        Severity = g.Max(f => GetSeverityValue(f.Severity)) > 0.7 ? "high" :
                                  g.Max(f => GetSeverityValue(f.Severity)) > 0.4 ? "medium" : "low",
                        Evidence = g.SelectMany(f => f.Evidence).Take(5).ToArray(),
                        Impact = g.SelectMany(f => f.Impact).Distinct().ToArray()
                    })
                    .ToList();

                return grouped;
            }

            private double GetSeverityValue(string severity)
            {
                return severity.ToLower() switch
                {
                    "high" => 1.0,
                    "medium" => 0.5,
                    _ => 0.2
                };
            }

            #endregion
        }
    
}
