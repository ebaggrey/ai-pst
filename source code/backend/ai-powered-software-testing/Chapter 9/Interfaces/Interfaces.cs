
// Interfaces/IPriorityOptimizer.cs
using Chapter_9.Models.Analysis;
using Chapter_9.Models.Requests;
using Chapter_9.Models.Responses;

namespace Chapter_9.Interfaces
{
    public interface IPriorityOptimizer
    {
        Task<OptimizedFeature[]> OptimizePrioritiesAsync(
            Feature[] features,
            TestingConstraints constraints,
            PrioritizationMethod method,
            CostOfDelay costOfDelay);
    }

    // Interfaces/IMinimalCoverageGenerator.cs
    public interface IMinimalCoverageGenerator
    {
        Task<TestCase[]> SelectMinimalCoverageAsync(
            TestScenario[] testScenarios,
            double confidenceTarget,
            CoverageConstraints constraints,
            OptimizationGoal goal);
    }

    // Interfaces/IAutomationDecider.cs
    public interface IAutomationDecider
    {
        Task<AutomationDecision> MakeDecisionAsync(
            ROIAnalysis roi,
            double roiThreshold,
            DecisionFactor[] factors,
            TestScenario scenario);
    }

    // Interfaces/IMaintenanceOptimizer.cs
    public interface IMaintenanceOptimizer
    {
        Task<OptimizationResult> OptimizeAsync(
            TestSuiteAnalysis testAnalysis,
            OptimizationStrategy strategy,
            string[] allowedActions,
            PreservationRule[] preservationRules);
    }

    // Interfaces/IROIAnalyzer.cs
    public interface IROIAnalyzer
    {
        Task<TangibleAnalysis> CalculateTangibleROIAsync(
            TestInvestment[] investments,
            TestOutcome[] outcomes,
            string[] costCategories,
            string[] valueCategories);

        Task<IntangibleAnalysis> CalculateIntangibleValueAsync(
            TestInvestment[] investments,
            TestOutcome[] outcomes);
    }

    // Supporting interfaces for internal methods
    public interface ITestScenarioGenerator
    {
        Task<TestScenario[]> GenerateTestScenariosAsync(
            Feature feature,
            RiskProfile riskProfile);
    }

    public interface ITestOptimizer
    {
        Task<TestCase[]> OptimizeTestDesignAsync(
            TestCase[] tests,
            CoverageConstraints constraints);
    }

    public interface ICostCalculator
    {
        Task<CostAnalysis> CalculateAutomationCostsAsync(
            AutomationCost automationCost,
            ManualCost manualCost,
            TestScenario scenario);
    }

    public interface IROICalculator
    {
        Task<ROIAnalysis> CalculateAutomationROIAsync(
            CostAnalysis costAnalysis,
            TestScenario scenario);
    }

    public interface ITestAnalyzer
    {
        /// <summary>
        /// Analyzes test value and cost metrics
        /// </summary>
        Task<TestSuiteAnalysis> AnalyzeTestValueAsync(
            TestSuite testSuite,
            ChangeImpact changeImpact);
    }
    public interface IValueCalculator
    {
        Task<OverallROI> CalculateOverallROIAsync(
            TangibleAnalysis tangibleAnalysis,
            IntangibleAnalysis intangibleAnalysis);
    }

}
