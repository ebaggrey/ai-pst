using Chapter_9.Exceptions;
using Chapter_9.Interfaces;
using Chapter_9.Models.Analysis;
using Chapter_9.Models.Requests;
using Chapter_9.Models.Responses;
namespace Chapter_9.Services
{

    public class MaintenanceOptimizer : IMaintenanceOptimizer
    {
        private readonly ILogger<MaintenanceOptimizer> _logger;

        public MaintenanceOptimizer(ILogger<MaintenanceOptimizer> logger)
        {
            _logger = logger;
        }

        public async Task<OptimizationResult> OptimizeAsync(
            TestSuiteAnalysis testAnalysis,  // Changed from TestAnalysis to TestSuiteAnalysis
            OptimizationStrategy strategy,
            string[] allowedActions,
            PreservationRule[] preservationRules)
        {
            var actions = new List<OptimizationAction>();
            var optimizedTests = testAnalysis?.TestSuite?.TestCases ?? Array.Empty<TestCase>();

            // Check preservation rules
            var violations = CheckPreservationRules(optimizedTests, preservationRules);
            if (violations.Any())
            {
                throw new PreservationViolationException(
                    "Optimization would violate preservation rules",
                    violations,
                    optimizedTests.Select(t => t.Id).ToArray()
                );
            }

            // Apply optimization actions based on analysis
            if (allowedActions.Contains("remove") && testAnalysis?.TestValues != null)
            {
                var lowValueTests = testAnalysis.TestValues
                    .Where(tv => tv.CalculatedValueScore < 0.3)
                    .Select(tv => tv.TestId)
                    .ToArray();

                if (lowValueTests.Any())
                {
                    actions.Add(new OptimizationAction
                    {
                        Type = "remove",
                        AffectedTests = lowValueTests,
                        Rationale = "Low business value or high maintenance cost",
                        Impact = 0.15
                    });
                }
            }

            if (allowedActions.Contains("consolidate") && testAnalysis?.RedundancyAnalysis != null)
            {
                foreach (var group in testAnalysis.RedundancyAnalysis.ConsolidationCandidates ?? Array.Empty<ConsolidationCandidate>())
                {
                    actions.Add(new OptimizationAction
                    {
                        Type = "consolidate",
                        AffectedTests = group.TestIds,
                        Rationale = group.Rationale,
                        Impact = group.ExpectedReduction
                    });
                }
            }

            if (allowedActions.Contains("simplify") && testAnalysis?.RiskAssessments != null)
            {
                var highRiskTests = testAnalysis.RiskAssessments
                    .Where(r => r.MaintenanceRisk > 0.7)
                    .Select(r => r.TestId)
                    .ToArray();

                if (highRiskTests.Any())
                {
                    actions.Add(new OptimizationAction
                    {
                        Type = "simplify",
                        AffectedTests = highRiskTests,
                        Rationale = "High maintenance risk detected",
                        Impact = 0.1
                    });
                }
            }

            return await Task.FromResult(new OptimizationResult
            {
                Actions = actions.ToArray(),
                OptimizedSuite = new TestSuite
                {
                    SuiteId = testAnalysis?.TestSuite?.SuiteId ?? Guid.NewGuid().ToString(),
                    TestCases = optimizedTests,
                    ApplicationArea = testAnalysis?.TestSuite?.ApplicationArea
                },
                Metrics = new Dictionary<string, double>
                {
                    ["TestCount"] = optimizedTests.Length,
                    ["ActionsTaken"] = actions.Count,
                    ["Reduction"] = actions.Sum(a => a.Impact),
                    ["HealthScore"] = testAnalysis?.OverallHealthScore ?? 0
                }
            });
        }

      
        private string[] CheckPreservationRules(TestCase[] tests, PreservationRule[] rules)
        {
            var violations = new List<string>();

            foreach (var rule in rules ?? Array.Empty<PreservationRule>())
            {
                if (rule.MustPreserve && !tests.Any(t => t.CoveredRequirements?.Contains(rule.RuleId) == true))
                {
                    violations.Add($"Rule {rule.RuleId}: {rule.Description}");
                }
            }

            return violations.ToArray();
        }
    }
}
