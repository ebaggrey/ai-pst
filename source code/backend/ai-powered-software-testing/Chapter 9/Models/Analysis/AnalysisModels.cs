
// Models/Analysis/TestAnalysis.cs
using Chapter_9.Models.Requests;

namespace Chapter_9.Models.Analysis
{
    /// <summary>
    /// Represents a comprehensive analysis of tests including value, cost, and impact metrics
    /// </summary>
    public class TestSuiteAnalysis
    {
        /// <summary>
        /// The original test suite being analyzed
        /// </summary>
        public TestSuite TestSuite { get; set; }

        /// <summary>
        /// Value analysis for each test case
        /// </summary>
        public TestValueAnalysis[] TestValues { get; set; }

        /// <summary>
        /// Cost analysis for each test case
        /// </summary>
        public TestCostAnalysis[] TestCosts { get; set; }

        /// <summary>
        /// Impact analysis of changes on tests
        /// </summary>
        public ImpactAnalysis ImpactAnalysis { get; set; }

        /// <summary>
        /// Redundancy analysis identifying duplicate or overlapping tests
        /// </summary>
        public RedundancyAnalysis RedundancyAnalysis { get; set; }

        /// <summary>
        /// Risk assessment for each test
        /// </summary>
        public TestRiskAssessment[] RiskAssessments { get; set; }

        /// <summary>
        /// Overall health score of the test suite (0-1)
        /// </summary>
        public double OverallHealthScore { get; set; }

        /// <summary>
        /// Recommendations based on the analysis
        /// </summary>
        public AnalysisRecommendation[] Recommendations { get; set; }

        /// <summary>
        /// Timestamp of when the analysis was performed
        /// </summary>
        public DateTime AnalyzedAt { get; set; }
    }
    /// <summary>
    /// Value analysis for a specific test case
    /// </summary>
    public class TestValueAnalysis
    {
        /// <summary>
        /// ID of the test case being analyzed
        /// </summary>
        public string TestId { get; set; }

        /// <summary>
        /// Business value score (0-1)
        /// </summary>
        public double BusinessValueScore { get; set; }

        /// <summary>
        /// Criticality score based on feature importance (0-1)
        /// </summary>
        public double CriticalityScore { get; set; }

        /// <summary>
        /// How many requirements this test covers
        /// </summary>
        public int RequirementsCovered { get; set; }

        /// <summary>
        /// How many defects this test has caught historically
        /// </summary>
        public int DefectsCaught { get; set; }

        /// <summary>
        /// Value density (value per unit of execution time)
        /// </summary>
        public double ValueDensity { get; set; }

        /// <summary>
        /// Whether this is a mandatory test (compliance/regulatory)
        /// </summary>
        public bool IsMandatory { get; set; }

        /// <summary>
        /// Tags or categories that add value
        /// </summary>
        public string[] ValueDrivers { get; set; }

        /// <summary>
        /// Calculated value score (weighted combination of factors)
        /// </summary>
        public double CalculatedValueScore { get; set; }
    }

    /// <summary>
    /// Cost analysis for a specific test case
    /// </summary>
    public class TestCostAnalysis
    {
        /// <summary>
        /// ID of the test case being analyzed
        /// </summary>
        public string TestId { get; set; }

        /// <summary>
        /// Time to execute the test in minutes
        /// </summary>
        public double ExecutionTimeMinutes { get; set; }

        /// <summary>
        /// Cost to execute once
        /// </summary>
        public double ExecutionCost { get; set; }

        /// <summary>
        /// Monthly maintenance cost
        /// </summary>
        public double MonthlyMaintenanceCost { get; set; }

        /// <summary>
        /// Time spent on maintenance per month
        /// </summary>
        public double MaintenanceHoursPerMonth { get; set; }

        /// <summary>
        /// Setup/teardown cost
        /// </summary>
        public double SetupCost { get; set; }

        /// <summary>
        /// Infrastructure/resources cost
        /// </summary>
        public double InfrastructureCost { get; set; }

        /// <summary>
        /// Historical flakiness rate (0-1)
        /// </summary>
        public double FlakinessRate { get; set; }

        /// <summary>
        /// Cost of false positives (investigation time)
        /// </summary>
        public double FalsePositiveCost { get; set; }

        /// <summary>
        /// Total cost over a specified period
        /// </summary>
        public double TotalCostOverPeriod { get; set; }

        /// <summary>
        /// Cost trend (increasing, stable, decreasing)
        /// </summary>
        public string CostTrend { get; set; }

        /// <summary>
        /// Detailed cost breakdown
        /// </summary>
        public CostBreakdown Breakdown { get; set; }
    }

    /// <summary>
    /// Detailed cost breakdown
    /// </summary>
    public class CostBreakdown
    {
        public double DirectLabor { get; set; }
        public double Infrastructure { get; set; }
        public double Tools { get; set; }
        public double Maintenance { get; set; }
        public double Opportunity { get; set; }
    }

    /// <summary>
    /// Impact analysis of changes on the test suite
    /// </summary>
    public class ImpactAnalysis
    {
        /// <summary>
        /// Tests affected by recent changes
        /// </summary>
        public ImpactedTest[] ImpactedTests { get; set; }

        /// <summary>
        /// Impact radius (how far changes propagate)
        /// </summary>
        public double ImpactRadius { get; set; }

        /// <summary>
        /// Areas with high change frequency
        /// </summary>
        public string[] HighChurnAreas { get; set; }

        /// <summary>
        /// Correlation between changes and test failures
        /// </summary>
        public Dictionary<string, double> ChangeFailureCorrelation { get; set; }

        /// <summary>
        /// Predicted stability score after changes (0-1)
        /// </summary>
        public double PredictedStabilityScore { get; set; }
    }

    /// <summary>
    /// Test impacted by changes
    /// </summary>
    public class ImpactedTest
    {
        public string TestId { get; set; }
        public string ImpactType { get; set; } // "direct", "indirect", "ripple"
        public double ImpactSeverity { get; set; } // 0-1
        public string[] ImpactedComponents { get; set; }
        public DateTime EstimatedReviewDate { get; set; }
    }

    /// <summary>
    /// Redundancy analysis
    /// </summary>
    public class RedundancyAnalysis
    {
        /// <summary>
        /// Groups of tests that appear to be redundant
        /// </summary>
        public TestRedundancyGroup[] RedundantGroups { get; set; }

        /// <summary>
        /// Overlap matrix showing coverage similarity
        /// </summary>
        public double[,] CoverageOverlapMatrix { get; set; }

        /// <summary>
        /// Tests that could potentially be consolidated
        /// </summary>
        public ConsolidationCandidate[] ConsolidationCandidates { get; set; }

        /// <summary>
        /// Overall redundancy percentage
        /// </summary>
        public double RedundancyPercentage { get; set; }

        /// <summary>
        /// Estimated savings if redundancies are removed
        /// </summary>
        public RedundancySavings EstimatedSavings { get; set; }
    }

    /// <summary>
    /// Group of redundant tests
    /// </summary>
    public class TestRedundancyGroup
    {
        public string GroupId { get; set; }
        public string[] TestIds { get; set; }
        public string RedundancyType { get; set; } // "identical", "overlapping", "subset"
        public double SimilarityScore { get; set; }
        public string Recommendation { get; set; }
    }

    /// <summary>
    /// Candidate for test consolidation
    /// </summary>
    public class ConsolidationCandidate
    {
        public string[] TestIds { get; set; }
        public string ConsolidationStrategy { get; set; }
        public double ExpectedReduction { get; set; }
        public string Rationale { get; set; }
    }

    /// <summary>
    /// Savings from removing redundancy
    /// </summary>
    public class RedundancySavings
    {
        public double TimeSavingsHours { get; set; }
        public double CostSavings { get; set; }
        public double MaintenanceReduction { get; set; }
    }

    /// <summary>
    /// Risk assessment for a test
    /// </summary>
    public class TestRiskAssessment
    {
        public string TestId { get; set; }

        /// <summary>
        /// Risk of test being obsolete
        /// </summary>
        public double ObsolescenceRisk { get; set; }

        /// <summary>
        /// Risk of test failing incorrectly (flakiness)
        /// </summary>
        public double FlakinessRisk { get; set; }

        /// <summary>
        /// Risk of missing defects (false negative)
        /// </summary>
        public double FalseNegativeRisk { get; set; }

        /// <summary>
        /// Risk of high maintenance burden
        /// </summary>
        public double MaintenanceRisk { get; set; }

        /// <summary>
        /// Overall risk score
        /// </summary>
        public double OverallRiskScore { get; set; }

        /// <summary>
        /// Risk factors contributing to the score
        /// </summary>
        public RiskFactorDetail[] ContributingFactors { get; set; }

        /// <summary>
        /// Recommended risk mitigation
        /// </summary>
        public string[] MitigationSteps { get; set; }
    }

    /// <summary>
    /// Risk factor detail
    /// </summary>
    public class RiskFactorDetail
    {
        public string FactorName { get; set; }
        public double Contribution { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Analysis recommendation
    /// </summary>
    public class AnalysisRecommendation
    {
        public string RecommendationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } // "remove", "consolidate", "simplify", "monitor"
        public string[] AffectedTestIds { get; set; }
        public double ExpectedImpact { get; set; }
        public double Confidence { get; set; }
        public string Priority { get; set; } // "high", "medium", "low"
        public string[] Prerequisites { get; set; }
        public string ImplementationGuide { get; set; }
        public RiskFactorDetail[] Risks { get; set; }
    }
}
