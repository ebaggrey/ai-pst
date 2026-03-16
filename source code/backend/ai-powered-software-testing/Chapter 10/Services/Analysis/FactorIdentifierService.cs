
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Responses;


namespace MetricsThatMatter.Services.Analysis
{
    public class FactorIdentifierService : IFactorIdentifier
    {
        private readonly ILogger<FactorIdentifierService> _logger;

        public FactorIdentifierService(ILogger<FactorIdentifierService> logger)
        {
            _logger = logger;
        }

        public async Task<ContributingFactor[]> IdentifyContributingFactorsAsync(
            HealthScore healthScore,
            WeightedMetric[] metrics)
        {
            _logger.LogInformation("Identifying contributing factors from {MetricCount} metrics",
                metrics?.Length ?? 0);

            if (healthScore == null || metrics == null || metrics.Length == 0)
                return Array.Empty<ContributingFactor>();

            var factors = new List<ContributingFactor>();

            foreach (var metric in metrics)
            {
                double impact = CalculateImpact(metric, healthScore.OverallScore);
                string direction = DetermineDirection(metric.Value, healthScore.OverallScore);

                factors.Add(new ContributingFactor
                {
                    Factor = metric.MetricName,
                    Impact = impact,
                    Direction = direction
                });
            }

            return await Task.FromResult(factors.OrderByDescending(f => Math.Abs(f.Impact)).ToArray());
        }

        private double CalculateImpact(WeightedMetric metric, double overallScore)
        {
            // Simplified impact calculation
            return (metric.Value - overallScore) * metric.Weight;
        }

        private string DetermineDirection(double metricValue, double overallScore)
        {
            return metricValue > overallScore ? "positive" : "negative";
        }
    }
}