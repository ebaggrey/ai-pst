
// Services/Analysis/InsightMonitoringPlannerService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class InsightMonitoringPlannerService : IInsightMonitoringPlanner
    {
        private readonly ILogger<InsightMonitoringPlannerService> _logger;

        public InsightMonitoringPlannerService(ILogger<InsightMonitoringPlannerService> logger)
        {
            _logger = logger;
        }

        public async Task<InsightMonitoringPlan> CreateInsightMonitoringPlanAsync(
            ActionableInsight[] insights)
        {
            _logger.LogInformation("Creating monitoring plan for {InsightCount} insights",
                insights?.Length ?? 0);

            var metrics = new List<string>();
            string reviewFrequency;
            string successCriteria;

            if (insights == null || insights.Length == 0)
            {
                metrics.Add("Insight generation rate");
                reviewFrequency = "Monthly";
                successCriteria = "At least 5 insights generated per quarter";
            }
            else
            {
                // Add metrics for each insight
                foreach (var insight in insights)
                {
                    metrics.Add($"{insight.Title} - action taken");
                    metrics.Add($"{insight.Title} - impact measured");
                }

                // Determine review frequency based on priority
                reviewFrequency = insights.Any(i => i.Priority == "high") ? "Weekly" : "Bi-weekly";

                // Define success criteria
                var highPriorityCount = insights.Count(i => i.Priority == "high");
                successCriteria = $"Implement {highPriorityCount} high-priority insights with >50% impact achievement";
            }

            return await Task.FromResult(new InsightMonitoringPlan
            {
                Metrics = metrics.ToArray(),
                ReviewFrequency = reviewFrequency ?? "Monthly",
                SuccessCriteria = successCriteria
            });
        }
    }
}