
// Services/Analysis/OptimizationMonitoringPlannerService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class OptimizationMonitoringPlannerService : IOptimizationMonitoringPlanner
    {
        private readonly ILogger<OptimizationMonitoringPlannerService> _logger;

        public OptimizationMonitoringPlannerService(ILogger<OptimizationMonitoringPlannerService> logger)
        {
            _logger = logger;
        }

        public async Task<OptimizationMonitoringPlan> CreateOptimizationMonitoringPlanAsync(
            OptimizationRecommendation optimization)
        {
            _logger.LogInformation("Creating optimization monitoring plan");

            var keyIndicators = new List<string>();

            // Add indicators based on optimization actions
            if (optimization.DeprecatedMetrics?.Any() == true)
            {
                keyIndicators.Add("Number of metrics actively collected");
                keyIndicators.Add("Cost savings from deprecations");
            }

            if (optimization.Consolidations?.Any() == true)
            {
                keyIndicators.Add("Consolidation accuracy");
                keyIndicators.Add("Query performance improvement");
            }

            if (optimization.NewMetrics?.Any() == true)
            {
                keyIndicators.Add("New metric adoption rate");
                keyIndicators.Add("Value generated from new metrics");
            }

            // Always include basic indicators
            keyIndicators.Add("Overall collection cost");
            keyIndicators.Add("Data quality score");
            keyIndicators.Add("Stakeholder satisfaction");

            return await Task.FromResult(new OptimizationMonitoringPlan
            {
                KeyIndicators = keyIndicators.ToArray(),
                ReviewFrequency = DetermineReviewFrequency(optimization)
            });
        }

        private string DetermineReviewFrequency(OptimizationRecommendation optimization)
        {
            if (optimization.DeprecatedMetrics?.Length > 5 ||
                optimization.Consolidations?.Length > 3)
            {
                return "Weekly for first month, then monthly";
            }

            return "Monthly";
        }
    }
}
