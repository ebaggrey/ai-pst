
// Services/Analysis/HealthImprovementGeneratorService.cs
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class HealthImprovementGeneratorService : IHealthImprovementGenerator
    {
        private readonly ILogger<HealthImprovementGeneratorService> _logger;

        public HealthImprovementGeneratorService(ILogger<HealthImprovementGeneratorService> logger)
        {
            _logger = logger;
        }

        public async Task<ImprovementRecommendation[]> GenerateHealthImprovementsAsync(
            HealthScore healthScore,
            WeightedMetric[] metrics,
            HistoricalBaseline[] baselines)
        {
            _logger.LogInformation("Generating health improvements for score {OverallScore}",
                healthScore?.OverallScore ?? 0);

            var recommendations = new List<ImprovementRecommendation>();

            if (healthScore == null || metrics == null)
                return Array.Empty<ImprovementRecommendation>();

            foreach (var metric in metrics.Where(m => m.Value < 70))
            {
                var baseline = baselines?.FirstOrDefault(b => b.MetricId == metric.MetricId);

                recommendations.Add(new ImprovementRecommendation
                {
                    MetricId = metric.MetricId,
                    Recommendation = $"Improve {metric.MetricName} by focusing on key drivers",
                    Impact = CalculatePotentialImpact(metric, baseline),
                    Priority = DeterminePriority(metric.Value),
                    Steps = GenerateImprovementSteps(metric.MetricName)
                });
            }

            return await Task.FromResult(recommendations.ToArray());
        }

        private double CalculatePotentialImpact(WeightedMetric metric, HistoricalBaseline? baseline)
        {
            double target = baseline?.Mean ?? 80;
            return (target - metric.Value) * metric.Weight;
        }

        private string DeterminePriority(double value)
        {
            return value switch
            {
                < 50 => "High",
                < 70 => "Medium",
                _ => "Low"
            };
        }

        private string[] GenerateImprovementSteps(string metricName)
        {
            return new[]
            {
                $"Analyze root causes for {metricName}",
                "Implement targeted improvements",
                "Monitor progress weekly",
                "Adjust strategy based on results"
            };
        }
    }
}