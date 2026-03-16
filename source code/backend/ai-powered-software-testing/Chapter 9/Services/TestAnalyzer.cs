
// Services/TestAnalyzer.cs
using Chapter_9.Interfaces;
using Chapter_9.Models.Analysis;
using Chapter_9.Models.Requests;

namespace Chapter_9.Services
{
    /// <summary>
    /// Analyzes test suites to provide value, cost, and optimization insights
    /// </summary>
    public class TestAnalyzer : ITestAnalyzer
    {
        private readonly ILogger<TestAnalyzer> _logger;
        private const double HIGH_VALUE_THRESHOLD = 0.7;
        private const double MEDIUM_VALUE_THRESHOLD = 0.4;
        private const double HIGH_RISK_THRESHOLD = 0.7;
        private const double OBSOLESCENCE_DAYS = 90;

        public TestAnalyzer(ILogger<TestAnalyzer> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Analyzes test suite value, costs, risks, and provides recommendations
        /// </summary>
        public async Task<TestSuiteAnalysis> AnalyzeTestValueAsync(
            TestSuite testSuite,
            ChangeImpact changeImpact)
        {
            _logger.LogInformation("Starting comprehensive analysis for test suite: {SuiteId} with {TestCount} tests",
                testSuite?.SuiteId, testSuite?.TestCases?.Length ?? 0);

            if (testSuite?.TestCases == null || !testSuite.TestCases.Any())
            {
                _logger.LogWarning("Empty test suite provided for analysis");
                return CreateEmptyAnalysis(testSuite);
            }

            try
            {
                // Parallel analysis for performance
                var testValuesTask = Task.Run(() => AnalyzeTestValues(testSuite.TestCases));
                var testCostsTask = Task.Run(() => AnalyzeTestCosts(testSuite.TestCases));
                var riskAssessmentsTask = Task.Run(() => AssessTestRisks(testSuite.TestCases));

                await Task.WhenAll(testValuesTask, testCostsTask, riskAssessmentsTask);

                var testValues = await testValuesTask;
                var testCosts = await testCostsTask;
                var riskAssessments = await riskAssessmentsTask;

                // Analyze redundancies
                var redundancyAnalysis = await FindRedundantTestsAsync(testSuite.TestCases, testValues);

                // Determine impacted tests
                var impactedTests = DetermineImpactedTests(testSuite, changeImpact);

                // Calculate overall health score
                var overallHealthScore = CalculateOverallHealthScore(testValues, testCosts, riskAssessments);

                // Generate recommendations
                var recommendations = GenerateRecommendations(
                    testSuite,
                    testValues,
                    testCosts,
                    riskAssessments,
                    redundancyAnalysis);

                var analysis = new TestSuiteAnalysis
                {
                    TestSuite = testSuite,
                    TestValues = testValues,
                    TestCosts = testCosts,
                    ImpactAnalysis = new ImpactAnalysis
                    {
                        ImpactedTests = impactedTests,
                        ImpactRadius = changeImpact?.ImpactRadius ?? CalculateDefaultImpactRadius(testSuite, changeImpact),
                        HighChurnAreas = changeImpact?.ChangedComponents ?? IdentifyHighChurnAreas(testSuite),
                        ChangeFailureCorrelation = CalculateChangeFailureCorrelation(testSuite),
                        PredictedStabilityScore = CalculatePredictedStability(testSuite, changeImpact)
                    },
                    RedundancyAnalysis = redundancyAnalysis,
                    RiskAssessments = riskAssessments,
                    OverallHealthScore = overallHealthScore,
                    Recommendations = recommendations,
                    AnalyzedAt = DateTime.UtcNow
                };

                _logger.LogInformation(
                    "Analysis complete: Health Score: {HealthScore:P2}, {RecommendationCount} recommendations generated",
                    overallHealthScore,
                    recommendations.Length);

                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing test suite {SuiteId}", testSuite.SuiteId);
                throw;
            }
        }

        /// <summary>
        /// Analyzes the business value of each test case
        /// </summary>
        private TestValueAnalysis[] AnalyzeTestValues(TestCase[] tests)
        {
            _logger.LogDebug("Analyzing values for {TestCount} tests", tests.Length);

            return tests.Select(test =>
            {
                // Calculate business value based on criticality and requirements
                var businessValueScore = CalculateBusinessValueScore(test);

                // Criticality score based on business criticality and failure impact
                var criticalityScore = CalculateCriticalityScore(test);

                // Requirements covered count
                var requirementsCovered = test.CoveredRequirements?.Length ?? 0;

                // Defects caught (estimate from failure rate - simplified)
                var defectsCaught = EstimateDefectsCaught(test);

                // Value density (value per unit of maintenance cost)
                var valueDensity = CalculateValueDensity(test, businessValueScore);

                // Determine if mandatory (compliance/regulatory)
                var isMandatory = IsMandatoryTest(test);

                // Calculate overall value score
                var calculatedValueScore = CalculateCompositeValueScore(
                    businessValueScore,
                    criticalityScore,
                    valueDensity,
                    isMandatory);

                return new TestValueAnalysis
                {
                    TestId = test.Id,
                    BusinessValueScore = businessValueScore,
                    CriticalityScore = criticalityScore,
                    RequirementsCovered = requirementsCovered,
                    DefectsCaught = defectsCaught,
                    ValueDensity = valueDensity,
                    IsMandatory = isMandatory,
                    ValueDrivers = IdentifyValueDrivers(test),
                    CalculatedValueScore = calculatedValueScore
                };
            }).ToArray();
        }

        /// <summary>
        /// Analyzes the costs associated with each test case
        /// </summary>
        private TestCostAnalysis[] AnalyzeTestCosts(TestCase[] tests)
        {
            _logger.LogDebug("Analyzing costs for {TestCount} tests", tests.Length);

            return tests.Select(test =>
            {
                // Calculate execution costs
                var executionTimeMinutes = EstimateExecutionTime(test);
                var executionCost = CalculateExecutionCost(test, executionTimeMinutes);

                // Calculate maintenance costs
                var monthlyMaintenanceCost = CalculateMonthlyMaintenanceCost(test);
                var maintenanceHoursPerMonth = monthlyMaintenanceCost / 100; // Assuming $100/hour

                // Infrastructure and setup costs
                var setupCost = CalculateSetupCost(test);
                var infrastructureCost = CalculateInfrastructureCost(test);

                // Flakiness and false positive costs
                var flakinessRate = test.FailureRate;
                var falsePositiveCost = CalculateFalsePositiveCost(test, flakinessRate);

                // Total cost over 3 months (typical review period)
                var totalCostOverPeriod = (executionCost * 3) + (monthlyMaintenanceCost * 3) +
                                          setupCost + infrastructureCost + falsePositiveCost;

                // Determine cost trend
                var costTrend = DetermineCostTrend(test);

                return new TestCostAnalysis
                {
                    TestId = test.Id,
                    ExecutionTimeMinutes = executionTimeMinutes,
                    ExecutionCost = executionCost,
                    MonthlyMaintenanceCost = monthlyMaintenanceCost,
                    MaintenanceHoursPerMonth = maintenanceHoursPerMonth,
                    SetupCost = setupCost,
                    InfrastructureCost = infrastructureCost,
                    FlakinessRate = flakinessRate,
                    FalsePositiveCost = falsePositiveCost,
                    TotalCostOverPeriod = totalCostOverPeriod,
                    CostTrend = costTrend,
                    Breakdown = new CostBreakdown
                    {
                        DirectLabor = executionCost * 0.7,
                        Infrastructure = infrastructureCost,
                        Tools = monthlyMaintenanceCost * 0.1,
                        Maintenance = monthlyMaintenanceCost,
                        Opportunity = monthlyMaintenanceCost * 0.3
                    }
                };
            }).ToArray();
        }

        /// <summary>
        /// Assesses risks for each test case
        /// </summary>
        private TestRiskAssessment[] AssessTestRisks(TestCase[] tests)
        {
            _logger.LogDebug("Assessing risks for {TestCount} tests", tests.Length);

            return tests.Select(test =>
            {
                // Calculate various risk factors
                var obsolescenceRisk = CalculateObsolescenceRisk(test);
                var flakinessRisk = test.FailureRate;
                var falseNegativeRisk = CalculateFalseNegativeRisk(test);
                var maintenanceRisk = CalculateMaintenanceRisk(test);

                // Calculate overall risk score (weighted average)
                var overallRiskScore = (obsolescenceRisk * 0.25) +
                                      (flakinessRisk * 0.3) +
                                      (falseNegativeRisk * 0.2) +
                                      (maintenanceRisk * 0.25);

                // Identify contributing factors
                var contributingFactors = new List<RiskFactorDetail>();

                if (obsolescenceRisk > HIGH_RISK_THRESHOLD)
                {
                    contributingFactors.Add(new RiskFactorDetail
                    {
                        FactorName = "Obsolescence",
                        Contribution = obsolescenceRisk,
                        Description = "Test not executed recently or feature changed significantly"
                    });
                }

                if (flakinessRisk > HIGH_RISK_THRESHOLD)
                {
                    contributingFactors.Add(new RiskFactorDetail
                    {
                        FactorName = "Flakiness",
                        Contribution = flakinessRisk,
                        Description = "Test has high failure rate due to instability"
                    });
                }

                if (falseNegativeRisk > HIGH_RISK_THRESHOLD)
                {
                    contributingFactors.Add(new RiskFactorDetail
                    {
                        FactorName = "False Negative",
                        Contribution = falseNegativeRisk,
                        Description = "Test may miss actual defects"
                    });
                }

                if (maintenanceRisk > HIGH_RISK_THRESHOLD)
                {
                    contributingFactors.Add(new RiskFactorDetail
                    {
                        FactorName = "Maintenance Burden",
                        Contribution = maintenanceRisk,
                        Description = "Test requires frequent updates or high effort"
                    });
                }

                // Generate mitigation steps based on risks
                var mitigationSteps = GenerateRiskMitigations(obsolescenceRisk, flakinessRisk,
                                                              falseNegativeRisk, maintenanceRisk);

                return new TestRiskAssessment
                {
                    TestId = test.Id,
                    ObsolescenceRisk = obsolescenceRisk,
                    FlakinessRisk = flakinessRisk,
                    FalseNegativeRisk = falseNegativeRisk,
                    MaintenanceRisk = maintenanceRisk,
                    OverallRiskScore = overallRiskScore,
                    ContributingFactors = contributingFactors.ToArray(),
                    MitigationSteps = mitigationSteps
                };
            }).ToArray();
        }

        /// <summary>
        /// Finds redundant or overlapping tests
        /// </summary>
        private async Task<RedundancyAnalysis> FindRedundantTestsAsync(TestCase[] tests, TestValueAnalysis[] valueAnalyses)
        {
            _logger.LogDebug("Analyzing test redundancies");

            var redundantGroups = new List<TestRedundancyGroup>();
            var consolidationCandidates = new List<ConsolidationCandidate>();

            // Create a similarity matrix
            var similarityMatrix = new double[tests.Length, tests.Length];
            var processedTests = new HashSet<string>();

            for (int i = 0; i < tests.Length; i++)
            {
                for (int j = i + 1; j < tests.Length; j++)
                {
                    var similarity = CalculateTestSimilarity(tests[i], tests[j]);
                    similarityMatrix[i, j] = similarity;
                    similarityMatrix[j, i] = similarity;

                    // If tests are highly similar, consider them redundant
                    if (similarity > 0.8 && !processedTests.Contains(tests[i].Id) && !processedTests.Contains(tests[j].Id))
                    {
                        var groupId = Guid.NewGuid().ToString();

                        redundantGroups.Add(new TestRedundancyGroup
                        {
                            GroupId = groupId,
                            TestIds = new[] { tests[i].Id, tests[j].Id },
                            RedundancyType = similarity > 0.95 ? "identical" : "overlapping",
                            SimilarityScore = similarity,
                            Recommendation = similarity > 0.95 ? "Remove duplicate" : "Consolidate tests"
                        });

                        // Add to consolidation candidates
                        consolidationCandidates.Add(new ConsolidationCandidate
                        {
                            TestIds = new[] { tests[i].Id, tests[j].Id },
                            ConsolidationStrategy = similarity > 0.95 ? "Remove one test" : "Merge test cases",
                            ExpectedReduction = similarity * 0.5,
                            Rationale = $"Tests are {similarity:P0} similar and cover same functionality"
                        });

                        processedTests.Add(tests[i].Id);
                        processedTests.Add(tests[j].Id);
                    }
                }
            }

            // Calculate redundancy percentage
            var redundantTestCount = redundantGroups.Sum(g => g.TestIds.Length);
            var redundancyPercentage = tests.Length > 0 ? (double)redundantTestCount / tests.Length : 0;

            // Calculate potential savings
            var estimatedSavings = new RedundancySavings
            {
                TimeSavingsHours = redundantGroups.Sum(g => g.TestIds.Length * 2), // 2 hours per redundant test
                CostSavings = redundantGroups.Sum(g => g.TestIds.Length * 100), // $100 per redundant test
                MaintenanceReduction = redundancyPercentage * 0.5 // 50% of redundancy percentage
            };

            return await Task.FromResult(new RedundancyAnalysis
            {
                RedundantGroups = redundantGroups.ToArray(),
                CoverageOverlapMatrix = similarityMatrix,
                ConsolidationCandidates = consolidationCandidates.ToArray(),
                RedundancyPercentage = redundancyPercentage,
                EstimatedSavings = estimatedSavings
            });
        }

        /// <summary>
        /// Determines which tests are impacted by changes
        /// </summary>
        private ImpactedTest[] DetermineImpactedTests(TestSuite testSuite, ChangeImpact changeImpact)
        {
            if (changeImpact?.ChangedComponents == null || !changeImpact.ChangedComponents.Any())
                return Array.Empty<ImpactedTest>();

            var impacted = new List<ImpactedTest>();

            foreach (var test in testSuite?.TestCases ?? Array.Empty<TestCase>())
            {
                // Check if test covers any changed components
                var matchingComponents = test.CoveredRequirements?
                    .Intersect(changeImpact.ChangedComponents)
                    .ToArray() ?? Array.Empty<string>();

                if (matchingComponents.Any())
                {
                    // Determine impact severity based on number of matches and test criticality
                    var impactSeverity = Math.Min(1.0, matchingComponents.Length * 0.3 + test.BusinessCriticality * 0.5);

                    // Determine impact type
                    var impactType = impactSeverity > 0.7 ? "direct" :
                                    impactSeverity > 0.3 ? "indirect" : "ripple";

                    impacted.Add(new ImpactedTest
                    {
                        TestId = test.Id,
                        ImpactType = impactType,
                        ImpactSeverity = impactSeverity,
                        ImpactedComponents = matchingComponents,
                        EstimatedReviewDate = DateTime.UtcNow.AddDays(impactSeverity > 0.7 ? 3 : 7)
                    });
                }
            }

            return impacted.ToArray();
        }

        /// <summary>
        /// Calculates overall health score of the test suite
        /// </summary>
        private double CalculateOverallHealthScore(
            TestValueAnalysis[] values,
            TestCostAnalysis[] costs,
            TestRiskAssessment[] risks)
        {
            if (!values.Any() || !costs.Any() || !risks.Any())
                return 0.5; // Default middle value

            // Average value score (higher is better)
            var avgValue = values.Average(v => v.CalculatedValueScore);

            // Cost efficiency (lower cost is better, normalized to 0-1)
            var avgCost = costs.Average(c => c.TotalCostOverPeriod);
            var maxCost = costs.Max(c => c.TotalCostOverPeriod);
            var costEfficiency = maxCost > 0 ? 1 - (avgCost / maxCost) : 0.5;

            // Inverse of risk (lower risk is better)
            var avgRisk = risks.Average(r => r.OverallRiskScore);
            var riskHealth = 1 - avgRisk;

            // Redundancy health (less redundancy is better) - approximated from costs
            var redundancyHealth = 1 - (costs.Count(c => c.FlakinessRate > 0.3) / (double)costs.Length);

            // Weighted composite score
            var healthScore = (avgValue * 0.35) +
                             (costEfficiency * 0.25) +
                             (riskHealth * 0.25) +
                             (redundancyHealth * 0.15);

            return Math.Max(0, Math.Min(1, healthScore));
        }

        /// <summary>
        /// Generates recommendations based on analysis
        /// </summary>
        private AnalysisRecommendation[] GenerateRecommendations(
            TestSuite testSuite,
            TestValueAnalysis[] values,
            TestCostAnalysis[] costs,
            TestRiskAssessment[] risks,
            RedundancyAnalysis redundancy)
        {
            var recommendations = new List<AnalysisRecommendation>();

            // 1. Recommendations for low-value tests
            var lowValueTests = values
                .Where(v => v.CalculatedValueScore < MEDIUM_VALUE_THRESHOLD && !v.IsMandatory)
                .ToArray();

            if (lowValueTests.Any())
            {
                recommendations.Add(new AnalysisRecommendation
                {
                    RecommendationId = Guid.NewGuid().ToString(),
                    Title = "Remove Low-Value Tests",
                    Description = $"Found {lowValueTests.Length} tests with low business value that could be removed",
                    Category = "remove",
                    AffectedTestIds = lowValueTests.Select(v => v.TestId).ToArray(),
                    ExpectedImpact = lowValueTests.Length * 0.15, // 15% reduction per test
                    Confidence = 0.85,
                    Priority = lowValueTests.Length > 5 ? "high" : "medium",
                    Prerequisites = new[] { "Review with product owner", "Verify coverage not impacted" },
                    ImplementationGuide = "Remove tests and update test suite configuration",
                    Risks = new[]
                    {
                        new RiskFactorDetail
                        {
                            FactorName = "Coverage Loss",
                            Contribution = 0.2,
                            Description = "Removing tests might reduce coverage in some areas"
                        }
                    }
                });
            }

            // 2. Recommendations for redundant tests
            if (redundancy.ConsolidationCandidates?.Any() == true)
            {
                recommendations.Add(new AnalysisRecommendation
                {
                    RecommendationId = Guid.NewGuid().ToString(),
                    Title = "Consolidate Redundant Tests",
                    Description = $"Found {redundancy.ConsolidationCandidates.Length} groups of tests that can be consolidated",
                    Category = "consolidate",
                    AffectedTestIds = redundancy.ConsolidationCandidates
                        .SelectMany(c => c.TestIds)
                        .Distinct()
                        .ToArray(),
                    ExpectedImpact = redundancy.EstimatedSavings?.MaintenanceReduction ?? 0.3,
                    Confidence = 0.9,
                    Priority = redundancy.RedundancyPercentage > 0.3 ? "high" : "medium",
                    Prerequisites = new[] { "Analyze coverage overlap", "Create consolidated test plan" },
                    ImplementationGuide = "Merge similar test cases into parameterized tests",
                    Risks = new[]
                    {
                        new RiskFactorDetail
                        {
                            FactorName = "Missing Edge Cases",
                            Contribution = 0.15,
                            Description = "Consolidation might miss specific edge cases"
                        }
                    }
                });
            }

            // 3. Recommendations for high-risk tests
            var highRiskTests = risks
                .Where(r => r.OverallRiskScore > HIGH_RISK_THRESHOLD)
                .ToArray();

            if (highRiskTests.Any())
            {
                recommendations.Add(new AnalysisRecommendation
                {
                    RecommendationId = Guid.NewGuid().ToString(),
                    Title = "Address High-Risk Tests",
                    Description = $"Found {highRiskTests.Length} tests with high risk scores requiring attention",
                    Category = "fix",
                    AffectedTestIds = highRiskTests.Select(r => r.TestId).ToArray(),
                    ExpectedImpact = highRiskTests.Length * 0.1,
                    Confidence = 0.75,
                    Priority = "high",
                    Prerequisites = new[] { "Review test failures", "Analyze root causes" },
                    ImplementationGuide = "Stabilize flaky tests, update test data, or redesign tests",
                    Risks = new[]
                    {
                        new RiskFactorDetail
                        {
                            FactorName = "Implementation Time",
                            Contribution = 0.25,
                            Description = "Fixing high-risk tests may require significant effort"
                        }
                    }
                });
            }

            // 4. Recommendations for cost optimization
            var highCostTests = costs
                .Where(c => c.TotalCostOverPeriod > costs.Average(ct => ct.TotalCostOverPeriod) * 1.5)
                .ToArray();

            if (highCostTests.Any())
            {
                recommendations.Add(new AnalysisRecommendation
                {
                    RecommendationId = Guid.NewGuid().ToString(),
                    Title = "Optimize High-Cost Tests",
                    Description = $"Found {highCostTests.Length} tests with above-average maintenance costs",
                    Category = "optimize",
                    AffectedTestIds = highCostTests.Select(c => c.TestId).ToArray(),
                    ExpectedImpact = 0.2,
                    Confidence = 0.8,
                    Priority = "medium",
                    Prerequisites = new[] { "Analyze cost drivers", "Review test efficiency" },
                    ImplementationGuide = "Refactor tests, reduce execution frequency, or simplify steps",
                    Risks = new[]
                    {
                        new RiskFactorDetail
                        {
                            FactorName = "Performance Impact",
                            Contribution = 0.1,
                            Description = "Optimization might affect test coverage"
                        }
                    }
                });
            }

            // 5. Recommendations for obsolete tests
            var obsoleteTests = risks
                .Where(r => r.ObsolescenceRisk > HIGH_RISK_THRESHOLD)
                .Select(r => r.TestId)
                .ToArray();

            if (obsoleteTests.Any())
            {
                recommendations.Add(new AnalysisRecommendation
                {
                    RecommendationId = Guid.NewGuid().ToString(),
                    Title = "Review Obsolete Tests",
                    Description = $"Found {obsoleteTests.Length} tests that may be obsolete",
                    Category = "review",
                    AffectedTestIds = obsoleteTests,
                    ExpectedImpact = 0.1,
                    Confidence = 0.7,
                    Priority = "medium",
                    Prerequisites = new[] { "Verify feature still exists", "Check last execution date" },
                    ImplementationGuide = "Remove or update obsolete tests",
                    Risks = new[]
                    {
                        new RiskFactorDetail
                        {
                            FactorName = "Regression Risk",
                            Contribution = 0.2,
                            Description = "Removing tests might miss regressions"
                        }
                    }
                });
            }

            return recommendations.ToArray();
        }

        #region Private Helper Methods

        private double CalculateBusinessValueScore(TestCase test)
        {
            // Business value based on criticality and requirement coverage
            var criticalityWeight = test.BusinessCriticality;
            var requirementWeight = Math.Min(1.0, (test.CoveredRequirements?.Length ?? 0) / 10.0);

            return (criticalityWeight * 0.7) + (requirementWeight * 0.3);
        }

        private double CalculateCriticalityScore(TestCase test)
        {
            // Criticality based on business criticality and failure impact
            return test.BusinessCriticality * 0.8 + (test.FailureRate * 0.2);
        }

        private int EstimateDefectsCaught(TestCase test)
        {
            // Estimate defects caught based on failure rate and execution frequency
            return (int)(test.FailureRate * test.ExecutionFrequency * 10);
        }

        private double CalculateValueDensity(TestCase test, double businessValue)
        {
            // Value per unit of maintenance cost
            var maintenanceCost = Math.Max(0.1, test.MaintenanceCost);
            return businessValue / maintenanceCost;
        }

        private bool IsMandatoryTest(TestCase test)
        {
            // Check if test covers compliance/regulatory requirements
            return test.CoveredRequirements?.Any(r =>
                r.StartsWith("COMP-") ||
                r.StartsWith("REG-") ||
                r.StartsWith("SEC-")) ?? false;
        }

        private string[] IdentifyValueDrivers(TestCase test)
        {
            var drivers = new List<string>();

            if (test.BusinessCriticality > 0.7)
                drivers.Add("High Business Criticality");

            if (test.CoveredRequirements?.Length > 5)
                drivers.Add("Broad Coverage");

            if (test.FailureRate < 0.1)
                drivers.Add("Stable Test");

            if (IsMandatoryTest(test))
                drivers.Add("Compliance Required");

            return drivers.ToArray();
        }

        private double CalculateCompositeValueScore(double businessValue, double criticality, double valueDensity, bool isMandatory)
        {
            var baseScore = (businessValue * 0.4) + (criticality * 0.3) + (valueDensity * 0.3);

            // Boost score for mandatory tests
            if (isMandatory)
                baseScore = Math.Min(1.0, baseScore + 0.2);

            return baseScore;
        }

        private double EstimateExecutionTime(TestCase test)
        {
            // Estimate execution time based on steps count (assuming 2 minutes per step)
            return (test.Steps?.Length ?? 1) * 2.0;
        }

        private double CalculateExecutionCost(TestCase test, double executionTime)
        {
            // Assume $100/hour labor cost
            var laborCost = (executionTime / 60) * 100;
            return laborCost * test.ExecutionFrequency;
        }

        private double CalculateMonthlyMaintenanceCost(TestCase test)
        {
            // Maintenance cost based on failure rate and complexity
            var baseMaintenance = test.MaintenanceCost * 100;
            var flakinessFactor = 1 + (test.FailureRate * 2);

            return baseMaintenance * flakinessFactor;
        }

        private double CalculateSetupCost(TestCase test)
        {
            // One-time setup cost
            return test.Steps?.Length * 50 ?? 100;
        }

        private double CalculateInfrastructureCost(TestCase test)
        {
            // Infrastructure/resources cost
            return 50; // Flat rate for simplicity
        }

        private double CalculateFalsePositiveCost(TestCase test, double flakinessRate)
        {
            // Cost of investigating false positives
            return flakinessRate * test.ExecutionFrequency * 100;
        }

        private string DetermineCostTrend(TestCase test)
        {
            if (test.FailureRate > 0.3) return "increasing";
            if (test.FailureRate < 0.1) return "decreasing";
            return "stable";
        }

        private double CalculateObsolescenceRisk(TestCase test)
        {
            // Risk increases if test hasn't been executed recently
            var daysSinceExecution = (DateTime.UtcNow - test.LastExecution).TotalDays;

            if (daysSinceExecution > OBSOLESCENCE_DAYS)
                return Math.Min(1.0, daysSinceExecution / (OBSOLESCENCE_DAYS * 2));

            return 0.1; // Base risk
        }

        private double CalculateFalseNegativeRisk(TestCase test)
        {
            // Risk of missing defects (inverse of effectiveness)
            return 1 - (test.BusinessCriticality * (1 - test.FailureRate));
        }

        private double CalculateMaintenanceRisk(TestCase test)
        {
            // Risk based on maintenance cost relative to average
            var avgMaintenance = 500; // Assume average maintenance cost
            return Math.Min(1.0, test.MaintenanceCost / avgMaintenance);
        }

        private string[] GenerateRiskMitigations(double obsolescence, double flakiness, double falseNegative, double maintenance)
        {
            var mitigations = new List<string>();

            if (obsolescence > HIGH_RISK_THRESHOLD)
                mitigations.Add("Review and update test for current feature set");

            if (flakiness > HIGH_RISK_THRESHOLD)
                mitigations.Add("Stabilize test by fixing race conditions or improving test data");

            if (falseNegative > HIGH_RISK_THRESHOLD)
                mitigations.Add("Enhance test assertions to catch more defects");

            if (maintenance > HIGH_RISK_THRESHOLD)
                mitigations.Add("Refactor test to reduce maintenance overhead");

            if (!mitigations.Any())
                mitigations.Add("Continue monitoring test health");

            return mitigations.ToArray();
        }

        private double CalculateTestSimilarity(TestCase test1, TestCase test2)
        {
            var similarity = 0.0;
            var factors = 0;

            // Compare names (fuzzy match - simplified)
            if (test1.Name == test2.Name)
            {
                similarity += 0.3;
                factors++;
            }

            // Compare covered requirements
            if (test1.CoveredRequirements != null && test2.CoveredRequirements != null)
            {
                var commonReqs = test1.CoveredRequirements.Intersect(test2.CoveredRequirements).Count();
                var totalReqs = test1.CoveredRequirements.Union(test2.CoveredRequirements).Count();

                if (totalReqs > 0)
                {
                    similarity += (commonReqs / (double)totalReqs) * 0.4;
                    factors++;
                }
            }

            // Compare steps (simplified - just count similarity)
            if (test1.Steps != null && test2.Steps != null)
            {
                var stepsSimilarity = Math.Min(test1.Steps.Length, test2.Steps.Length) /
                                     (double)Math.Max(test1.Steps.Length, test2.Steps.Length);
                similarity += stepsSimilarity * 0.3;
                factors++;
            }

            return factors > 0 ? similarity / factors : 0;
        }

        private double CalculateDefaultImpactRadius(TestSuite testSuite, ChangeImpact changeImpact)
        {
            // Calculate how far changes typically propagate
            if (testSuite?.TestCases == null || !testSuite.TestCases.Any())
                return 0.5;

            var impactedCount = DetermineImpactedTests(testSuite, changeImpact).Length;
            return (double)impactedCount / testSuite.TestCases.Length;
        }

        private string[] IdentifyHighChurnAreas(TestSuite testSuite)
        {
            // Identify areas with frequent changes based on test failure rates
            return testSuite?.TestCases
                .Where(t => t.FailureRate > 0.2)
                .SelectMany(t => t.CoveredRequirements ?? Array.Empty<string>())
                .Distinct()
                .Take(5)
                .ToArray() ?? Array.Empty<string>();
        }

        private Dictionary<string, double> CalculateChangeFailureCorrelation(TestSuite testSuite)
        {
            // Simplified correlation between components and failures
            var correlation = new Dictionary<string, double>();

            foreach (var test in testSuite?.TestCases ?? Array.Empty<TestCase>())
            {
                foreach (var req in test.CoveredRequirements ?? Array.Empty<string>())
                {
                    if (!correlation.ContainsKey(req))
                        correlation[req] = 0;

                    correlation[req] = Math.Max(correlation[req], test.FailureRate);
                }
            }

            return correlation;
        }

        private double CalculatePredictedStability(TestSuite testSuite, ChangeImpact changeImpact)
        {
            // Predict how stable the test suite will be after changes
            var impactedTests = DetermineImpactedTests(testSuite, changeImpact);

            if (!impactedTests.Any())
                return 0.95; // Highly stable

            var avgImpactSeverity = impactedTests.Average(i => i.ImpactSeverity);
            var stabilityScore = 1 - (avgImpactSeverity * 0.5);

            return Math.Max(0.5, Math.Min(1.0, stabilityScore));
        }

        private TestSuiteAnalysis CreateEmptyAnalysis(TestSuite testSuite)
        {
            return new TestSuiteAnalysis
            {
                TestSuite = testSuite,
                TestValues = Array.Empty<TestValueAnalysis>(),
                TestCosts = Array.Empty<TestCostAnalysis>(),
                ImpactAnalysis = new ImpactAnalysis
                {
                    ImpactedTests = Array.Empty<ImpactedTest>(),
                    ImpactRadius = 0,
                    HighChurnAreas = Array.Empty<string>(),
                    ChangeFailureCorrelation = new Dictionary<string, double>(),
                    PredictedStabilityScore = 1.0
                },
                RedundancyAnalysis = new RedundancyAnalysis
                {
                    RedundantGroups = Array.Empty<TestRedundancyGroup>(),
                    CoverageOverlapMatrix = new double[0, 0],
                    ConsolidationCandidates = Array.Empty<ConsolidationCandidate>(),
                    RedundancyPercentage = 0,
                    EstimatedSavings = new RedundancySavings()
                },
                RiskAssessments = Array.Empty<TestRiskAssessment>(),
                OverallHealthScore = 0.5,
                Recommendations = Array.Empty<AnalysisRecommendation>(),
                AnalyzedAt = DateTime.UtcNow
            };
        }

        #endregion
    }
}
