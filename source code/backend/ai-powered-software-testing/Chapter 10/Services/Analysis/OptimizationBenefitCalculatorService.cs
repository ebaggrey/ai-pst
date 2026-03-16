
// Services/Analysis/OptimizationBenefitCalculatorService.cs
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Responses;

namespace Chapter_10.Services.Analysis
{
    public class OptimizationBenefitCalculatorService : IOptimizationBenefitCalculator
    {
        private readonly ILogger<OptimizationBenefitCalculatorService> _logger;

        public OptimizationBenefitCalculatorService(ILogger<OptimizationBenefitCalculatorService> logger)
        {
            _logger = logger;
        }

        public async Task<ExpectedBenefits> CalculateOptimizationBenefitsAsync(
            OptimizationRecommendation optimization,
            MetricValueAnalysis valueAnalysis,
            CollectionCostAnalysis costAnalysis)
        {
            _logger.LogInformation("Calculating optimization benefits");

            double costReduction = CalculateCostReduction(optimization, costAnalysis);
            double efficiencyGain = CalculateEfficiencyGain(optimization);
            double qualityImprovement = CalculateQualityImprovement(optimization, valueAnalysis);

            var additionalBenefits = new List<string>();

            if (optimization.Consolidations?.Any() == true)
                additionalBenefits.Add($"Consolidated {optimization.Consolidations.Length} metric groups");

            if (optimization.DeprecatedMetrics?.Any() == true)
                additionalBenefits.Add($"Deprecated {optimization.DeprecatedMetrics.Length} low-value metrics");

            if (optimization.NewMetrics?.Any() == true)
                additionalBenefits.Add($"Added {optimization.NewMetrics.Length} high-value metrics");

            return await Task.FromResult(new ExpectedBenefits
            {
                CostReduction = costReduction,
                EfficiencyGain = efficiencyGain,
                QualityImprovement = qualityImprovement,
                AdditionalBenefits = additionalBenefits.ToArray()
            });
        }

        private double CalculateCostReduction(
            OptimizationRecommendation optimization,
            CollectionCostAnalysis costAnalysis)
        {
            double totalSavings = 0;

            // Savings from removals
            if (optimization.DeprecatedMetrics != null)
            {
                totalSavings += optimization.DeprecatedMetrics
                    .Select(m => costAnalysis.MetricCosts?.FirstOrDefault(c => c.MetricId == m)?.Cost ?? 0)
                    .Sum();
            }

            // Savings from consolidations
            if (optimization.Consolidations != null)
            {
                foreach (var consolidation in optimization.Consolidations)
                {
                    var sourceCosts = consolidation.SourceMetricIds
                        .Select(m => costAnalysis.MetricCosts?.FirstOrDefault(c => c.MetricId == m)?.Cost ?? 0)
                        .Sum();

                    // Assume consolidated metric costs 40% of sum of sources
                    totalSavings += sourceCosts * 0.6;
                }
            }

            return costAnalysis.TotalCost > 0
                ? totalSavings / costAnalysis.TotalCost
                : 0;
        }

        private double CalculateEfficiencyGain(OptimizationRecommendation optimization)
        {
            // Simplified efficiency calculation
            double gain = 0;

            if (optimization.Consolidations?.Any() == true)
                gain += 0.1;

            if (optimization.DeprecatedMetrics?.Any() == true)
                gain += 0.05 * Math.Min(5, optimization.DeprecatedMetrics.Length);

            return Math.Min(0.5, gain);
        }

        private double CalculateQualityImprovement(
            OptimizationRecommendation optimization,
            MetricValueAnalysis valueAnalysis)
        {
            // Simplified quality improvement
            double improvement = 0.05; // Base improvement

            if (optimization.NewMetrics?.Any() == true)
                improvement += 0.03 * optimization.NewMetrics.Length;

            return Math.Min(0.3, improvement);
        }
    }
}
