
// Services/Analysis/CollectionCostAnalyzerService.cs
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;


namespace Chapter_10.Services.Analysis
{
    public class CollectionCostAnalyzerService : ICollectionCostAnalyzer
    {
        private readonly ILogger<CollectionCostAnalyzerService> _logger;

        public CollectionCostAnalyzerService(ILogger<CollectionCostAnalyzerService> logger)
        {
            _logger = logger;
        }

        public async Task<CollectionCostAnalysis> AnalyzeCollectionCostsAsync(
            MetricDefinition[] metrics,
            ResourceConstraint[] constraints)
        {
            _logger.LogInformation("Analyzing collection costs for {MetricCount} metrics",
                metrics?.Length ?? 0);

            if (metrics == null || metrics.Length == 0)
            {
                return new CollectionCostAnalysis
                {
                    MetricCosts = Array.Empty<MetricCost>(),
                    TotalCost = 0
                };
            }

            var metricCosts = new List<MetricCost>();
            double totalCost = 0;

            foreach (var metric in metrics)
            {
                double cost = CalculateCost(metric, constraints);

                metricCosts.Add(new MetricCost
                {
                    MetricId = metric.MetricId,
                    Cost = cost,
                    CostDriver = DetermineCostDriver(metric)
                });

                totalCost += cost;
            }

            return await Task.FromResult(new CollectionCostAnalysis
            {
                MetricCosts = metricCosts.ToArray(),
                TotalCost = totalCost
            });
        }

        private double CalculateCost(MetricDefinition metric, ResourceConstraint[] constraints)
        {
            // Base cost from metric definition
            double cost = metric.CollectionCost;

            // Adjust based on constraints
            if (constraints != null)
            {
                var resourceConstraint = constraints.FirstOrDefault(c =>
                    c.ResourceType?.Equals(metric.CollectionMethod, StringComparison.OrdinalIgnoreCase) == true);

                if (resourceConstraint != null && cost > resourceConstraint.MaxAllocation)
                {
                    cost = resourceConstraint.MaxAllocation;
                }
            }

            return cost;
        }

        private string DetermineCostDriver(MetricDefinition metric)
        {
            if (metric.CollectionCost > 80) return "high_complexity";
            if (metric.CollectionCost > 50) return "manual_effort";
            if (metric.CollectionCost > 20) return "automated";
            return "minimal";
        }
    }
}
