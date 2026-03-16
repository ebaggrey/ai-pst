
using Chapter_10.Interfaces;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class MonitoringRecommenderService : IMonitoringRecommender
    {
        private readonly ILogger<MonitoringRecommenderService> _logger;

        public MonitoringRecommenderService(ILogger<MonitoringRecommenderService> logger)
        {
            _logger = logger;
        }

        public async Task<MonitoringRecommendation[]> GenerateMonitoringRecommendationsAsync(
            MetricPrediction[] predictions,
            int horizon)
        {
            _logger.LogInformation("Generating monitoring recommendations for {PredictionCount} predictions",
                predictions?.Length ?? 0);

            if (predictions == null || predictions.Length == 0)
                return Array.Empty<MonitoringRecommendation>();

            var recommendations = new List<MonitoringRecommendation>();

            foreach (var prediction in predictions)
            {
                string frequency = DetermineMonitoringFrequency(prediction);
                string threshold = DetermineAlertThreshold(prediction);

                recommendations.Add(new MonitoringRecommendation
                {
                    MetricId = prediction.MetricId,
                    Frequency = frequency,
                    Threshold = threshold
                });
            }

            return await Task.FromResult(recommendations.ToArray());
        }

        private string DetermineMonitoringFrequency(MetricPrediction prediction)
        {
            if (HasHighVolatility(prediction))
                return "daily";

            if (IsCriticalMetric(prediction))
                return "weekly";

            return "monthly";
        }

        private bool HasHighVolatility(MetricPrediction prediction)
        {
            if (prediction.Values == null || prediction.Values.Length == 0)
                return false;

            var avg = prediction.Values.Average();
            var stdDev = Math.Sqrt(prediction.Values.Select(v => Math.Pow(v - avg, 2)).Average());
            return stdDev / avg > 0.15;
        }

        private bool IsCriticalMetric(MetricPrediction prediction)
        {
            var criticalMetrics = new[] { "quality", "performance", "reliability" };
            return criticalMetrics.Any(m =>
                prediction.MetricName?.Contains(m, StringComparison.OrdinalIgnoreCase) == true);
        }

        private string DetermineAlertThreshold(MetricPrediction prediction)
        {
            if (prediction.Values == null || prediction.Values.Length == 0)
                return "±10%";

            var avg = prediction.Values.Average();
            var threshold = avg * 0.15; // 15% deviation threshold
            return $"±{threshold:F2} or {threshold / avg * 100:F0}%";
        }
    }
}