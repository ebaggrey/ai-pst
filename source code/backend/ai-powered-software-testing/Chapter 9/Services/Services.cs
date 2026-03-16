
// Services/PriorityOptimizer.cs
using Chapter_9.Exceptions;
using Chapter_9.Interfaces;
using Chapter_9.Models.Analysis;
using Chapter_9.Models.Requests;
using Chapter_9.Models.Responses;


namespace Chapter_9.Services
{
    public class PriorityOptimizer : IPriorityOptimizer
    {
        private readonly ILogger<PriorityOptimizer> _logger;

        public PriorityOptimizer(ILogger<PriorityOptimizer> logger)
        {
            _logger = logger;
        }

        public async Task<OptimizedFeature[]> OptimizePrioritiesAsync(
            Feature[] features,
            TestingConstraints constraints,
            PrioritizationMethod method,
            CostOfDelay costOfDelay)
        {
            // Validate constraints feasibility
            var totalEffort = features.Sum(f => f.TestingEffort);
            if (totalEffort > constraints.MaxTimeHours)
            {
                var minAdditionalTime = totalEffort - constraints.MaxTimeHours;
                var featuresToRemove = CalculateFeaturesToRemove(features, constraints.MaxTimeHours);

                throw new ConstraintImpossibleException(
                    "Cannot satisfy time constraints",
                    minAdditionalTime,
                    featuresToRemove,
                    new { TotalEffort = totalEffort, MaxAllowed = constraints.MaxTimeHours },
                    new { ReduceFeaturesBy = featuresToRemove, IncreaseTimeBy = minAdditionalTime }
                );
            }

            // Calculate priority scores based on method
            var prioritized = features.Select(f => new OptimizedFeature
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                BusinessValue = f.BusinessValue,
                RiskLevel = f.RiskLevel,
                ImplementationComplexity = f.ImplementationComplexity,
                TestingEffort = f.TestingEffort,
                Dependencies = f.Dependencies,
                Attributes = f.Attributes,
                PriorityScore = CalculatePriorityScore(f, method, costOfDelay),
                ExpectedValuePerHour = f.BusinessValue / f.TestingEffort,
                RiskAdjustedValue = f.BusinessValue * (1 - f.RiskLevel),
                OptimizationRationale = new[]
                {
                    $"Score based on {method} method",
                    $"Risk adjusted value: {f.BusinessValue * (1 - f.RiskLevel)}"
                }
            }).OrderByDescending(f => f.PriorityScore).ToArray();

            // Assign priority numbers
            for (int i = 0; i < prioritized.Length; i++)
            {
                prioritized[i].Priority = i + 1;
            }

            return await Task.FromResult(prioritized);
        }

        private double CalculatePriorityScore(Feature feature,
            PrioritizationMethod method,
            CostOfDelay costOfDelay)
        {
            return method switch
            {
                PrioritizationMethod.WeightedShortestJobFirst =>
                    feature.BusinessValue / feature.TestingEffort,
                PrioritizationMethod.CostOfDelayDividedByDuration =>
                    (costOfDelay?.DailyCost ?? 1) * feature.BusinessValue / feature.TestingEffort,
                PrioritizationMethod.BusinessValueFirst =>
                    feature.BusinessValue,
                PrioritizationMethod.RiskBased =>
                    feature.RiskLevel * feature.BusinessValue,
                PrioritizationMethod.MoSCoW =>
                    feature.BusinessValue * 100,
                _ => feature.BusinessValue
            };
        }

        private int CalculateFeaturesToRemove(Feature[] features, double maxTime)
        {
            var sorted = features.OrderBy(f => f.BusinessValue / f.TestingEffort);
            double currentTime = features.Sum(f => f.TestingEffort);
            int removeCount = 0;

            foreach (var feature in sorted)
            {
                if (currentTime <= maxTime) break;
                currentTime -= feature.TestingEffort;
                removeCount++;
            }

            return removeCount;
        }

        Task<OptimizedFeature[]> IPriorityOptimizer.OptimizePrioritiesAsync(Feature[] features, TestingConstraints constraints, PrioritizationMethod method, CostOfDelay costOfDelay)
        {
            throw new NotImplementedException();
        }
    }

    // Services/MinimalCoverageGenerator.cs
    public class MinimalCoverageGenerator : IMinimalCoverageGenerator
    {
        private readonly ILogger<MinimalCoverageGenerator> _logger;

        public MinimalCoverageGenerator(ILogger<MinimalCoverageGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<TestCase[]> SelectMinimalCoverageAsync(
            TestScenario[] testScenarios,
            double confidenceTarget,
            CoverageConstraints constraints,
            OptimizationGoal goal)
        {
            // Calculate if achievable
            if (testScenarios.Length > constraints.MaxTestCases)
            {
                var minTestsRequired = (int)Math.Ceiling(testScenarios.Length * confidenceTarget);
                var achievableConfidence = (double)constraints.MaxTestCases / testScenarios.Length;

                throw new CoverageImpossibleException(
                    "Cannot achieve confidence target with given constraints",
                    minTestsRequired,
                    achievableConfidence,
                    new { TotalScenarios = testScenarios.Length, Covered = constraints.MaxTestCases },
                    new { Constraint = $"Max {constraints.MaxTestCases} tests", Achievable = achievableConfidence }
                );
            }

            // Select tests based on optimization goal
            var selectedTests = goal switch
            {
                OptimizationGoal.MaximizeCoverage =>
                    testScenarios.OrderByDescending(s => s.Description.Length).Take(constraints.MaxTestCases),
                OptimizationGoal.MinimizeTests =>
                    testScenarios.Take(constraints.MaxTestCases),
                OptimizationGoal.BalanceRiskAndEffort =>
                    testScenarios.OrderBy(s => s.AverageDurationMinutes).Take(constraints.MaxTestCases),
                _ => testScenarios.Take(constraints.MaxTestCases)
            };

            return await Task.FromResult(selectedTests.Select(s => new TestCase
            {
                Id = Guid.NewGuid().ToString(),
                Name = s.Name,
                CoveredRequirements = new[] { "REQ-" + s.Name },
                ExecutionFrequency = s.ExecutionFrequency,
                FailureRate = 0.05,
                MaintenanceCost = 1.0,
                BusinessCriticality = 0.8
            }).ToArray());
        }
    }

    // Services/AutomationDecider.cs
    public class AutomationDecider : IAutomationDecider
    {
        private readonly ILogger<AutomationDecider> _logger;

        public AutomationDecider(ILogger<AutomationDecider> logger)
        {
            _logger = logger;
        }

        public async Task<AutomationDecision> MakeDecisionAsync(
            ROIAnalysis roi,
            double roiThreshold,
            DecisionFactor[] factors,
            TestScenario scenario)
        {
            var decision = roi.ROIValue >= roiThreshold ? "automate" : "manual";

            // Consider additional factors
            var weightedScore = factors?.Sum(f => f.Weight * f.Score) ?? 0;
            if (weightedScore > 7.5) decision = "automate";
            else if (weightedScore < 3.0) decision = "manual";

            return await Task.FromResult(new AutomationDecision
            {
                Decision = decision,
                Confidence = roi.ROIValue / roiThreshold,
                Rationale = new[]
                {
                    $"ROI: {roi.ROIValue}x vs threshold {roiThreshold}x",
                    $"Weighted factor score: {weightedScore}",
                    $"Payback period: {roi.PaybackPeriod} months"
                },
                Factors = factors
            });
        }
    }

    // Services/MaintenanceOptimizer.cs
    //public class MaintenanceOptimizer : IMaintenanceOptimizer
    //{
    //    private readonly ILogger<MaintenanceOptimizer> _logger;

    //    public MaintenanceOptimizer(ILogger<MaintenanceOptimizer> logger)
    //    {
    //        _logger = logger;
    //    }

    //    public async Task<OptimizationResult> OptimizeAsync(
    //        TestAnalysis testAnalysis,
    //        OptimizationStrategy strategy,
    //        string[] allowedActions,
    //        PreservationRule[] preservationRules)
    //    {
    //        var actions = new List<OptimizationAction>();
    //        var optimizedTests = testAnalysis?.TestSuite?.TestCases ?? Array.Empty<TestCase>();

    //        // Check preservation rules
    //        var violations = CheckPreservationRules(optimizedTests, preservationRules);
    //        if (violations.Any())
    //        {
    //            throw new PreservationViolationException(
    //                "Optimization would violate preservation rules",
    //                violations,
    //                optimizedTests.Select(t => t.Id).ToArray()
    //            );
    //        }

    //        // Apply optimization actions
    //        if (allowedActions.Contains("remove"))
    //        {
    //            actions.Add(new OptimizationAction
    //            {
    //                Type = "remove",
    //                AffectedTests = optimizedTests.Where(t => t.FailureRate > 0.3).Select(t => t.Id).ToArray(),
    //                Rationale = "High failure rate indicates flaky tests",
    //                Impact = 0.15
    //            });
    //        }

    //        if (allowedActions.Contains("consolidate"))
    //        {
    //            actions.Add(new OptimizationAction
    //            {
    //                Type = "consolidate",
    //                AffectedTests = optimizedTests.Take(2).Select(t => t.Id).ToArray(),
    //                Rationale = "Similar test coverage detected",
    //                Impact = 0.10
    //            });
    //        }

    //        return await Task.FromResult(new OptimizationResult
    //        {
    //            Actions = actions.ToArray(),
    //            OptimizedSuite = new TestSuite
    //            {
    //                SuiteId = testAnalysis?.TestSuite?.SuiteId ?? Guid.NewGuid().ToString(),
    //                TestCases = optimizedTests,
    //                ApplicationArea = testAnalysis?.TestSuite?.ApplicationArea
    //            },
    //            Metrics = new Dictionary<string, double>
    //            {
    //                ["TestCount"] = optimizedTests.Length,
    //                ["ActionsTaken"] = actions.Count,
    //                ["Reduction"] = actions.Sum(a => a.Impact)
    //            }
    //        });
    //    }

    //    private string[] CheckPreservationRules(TestCase[] tests, PreservationRule[] rules)
    //    {
    //        var violations = new List<string>();

    //        foreach (var rule in rules)
    //        {
    //            if (rule.MustPreserve && !tests.Any(t => t.CoveredRequirements.Contains(rule.RuleId)))
    //            {
    //                violations.Add($"Rule {rule.RuleId}: {rule.Description}");
    //            }
    //        }

    //        return violations.ToArray();
    //    }
    //}

    // Services/ROIAnalyzer.cs
    public class ROIAnalyzer : IROIAnalyzer
    {
        private readonly ILogger<ROIAnalyzer> _logger;

        public ROIAnalyzer(ILogger<ROIAnalyzer> logger)
        {
            _logger = logger;
        }

        public async Task<TangibleAnalysis> CalculateTangibleROIAsync(
            TestInvestment[] investments,
            TestOutcome[] outcomes,
            string[] costCategories,
            string[] valueCategories)
        {
            var totalCost = investments.Sum(i => i.Cost);
            var totalBenefit = outcomes.Where(o => o.Type == "tangible").Sum(o => o.Value);

            if (totalCost == 0 || totalBenefit == 0)
            {
                throw new ROICalculationException(
                    "Cannot calculate ROI with zero values",
                    new { MissingCostData = totalCost == 0, MissingBenefitData = totalBenefit == 0 },
                    new { Limitations = "Zero values prevent meaningful ROI calculation" }
                );
            }

            return await Task.FromResult(new TangibleAnalysis
            {
                TotalCost = totalCost,
                TotalBenefit = totalBenefit,
                ROI = (totalBenefit - totalCost) / totalCost,
                CostBreakdown = investments
                    .GroupBy(i => i.Category)
                    .Select(g => new CostBenefitItem
                    {
                        Category = g.Key,
                        Amount = g.Sum(i => i.Cost),
                        Percentage = g.Sum(i => i.Cost) / totalCost * 100
                    }).ToArray(),
                BenefitBreakdown = outcomes
                    .Where(o => o.Type == "tangible")
                    .GroupBy(o => o.Category)
                    .Select(g => new CostBenefitItem
                    {
                        Category = g.Key,
                        Amount = g.Sum(o => o.Value),
                        Percentage = g.Sum(o => o.Value) / totalBenefit * 100
                    }).ToArray()
            });
        }

        public async Task<IntangibleAnalysis> CalculateIntangibleValueAsync(
            TestInvestment[] investments,
            TestOutcome[] outcomes)
        {
            var intangibleOutcomes = outcomes.Where(o => o.Type == "intangible").ToArray();

            return await Task.FromResult(new IntangibleAnalysis
            {
                QualityScore = 0.85,
                TeamMorale = 0.75,
                CustomerSatisfaction = 0.90,
                QualitativeBenefits = intangibleOutcomes.Select(o => o.Category).ToArray()
            });
        }
    }
}
