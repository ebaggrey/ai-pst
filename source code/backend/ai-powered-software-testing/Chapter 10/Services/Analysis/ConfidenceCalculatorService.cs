
// Services/Analysis/ConfidenceCalculatorService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class ConfidenceCalculatorService : IConfidenceCalculator
    {
        private readonly ILogger<ConfidenceCalculatorService> _logger;

        public ConfidenceCalculatorService(ILogger<ConfidenceCalculatorService> logger)
        {
            _logger = logger;
        }

        public async Task<PredictionConfidence> CalculatePredictionConfidenceAsync(
            MetricPrediction[] predictions,
            HistoricalTrend[] trends)
        {
            _logger.LogInformation("Calculating prediction confidence for {PredictionCount} predictions",
                predictions?.Length ?? 0);

            if (predictions == null || predictions.Length == 0)
            {
                return new PredictionConfidence
                {
                    AverageConfidence = 0,
                    MetricConfidence = new Dictionary<string, double>(),
                    ConfidenceFactors = new[] { "No predictions available" }
                };
            }

            var metricConfidence = new Dictionary<string, double>();
            var confidenceFactors = new List<string>();
            double totalConfidence = 0;

            foreach (var prediction in predictions)
            {
                double confidence = CalculateMetricConfidence(prediction, trends);
                metricConfidence[prediction.MetricId ?? prediction.MetricName ?? "unknown"] = confidence;
                totalConfidence += confidence;

                if (confidence < 0.5)
                    confidenceFactors.Add($"Low confidence for {prediction.MetricName} due to data variability");
            }

            confidenceFactors.Add("Confidence based on historical pattern strength");

            return await Task.FromResult(new PredictionConfidence
            {
                AverageConfidence = predictions.Length > 0 ? totalConfidence / predictions.Length : 0,
                MetricConfidence = metricConfidence,
                ConfidenceFactors = confidenceFactors.ToArray()
            });
        }

        private double CalculateMetricConfidence(MetricPrediction prediction, HistoricalTrend[] trends)
        {
            if (prediction.Values == null || prediction.Values.Length == 0)
                return 0.3;

            // Simplified confidence calculation
            var trend = trends?.FirstOrDefault(t =>
                t.MetricValues != null && t.MetricValues.ContainsKey(prediction.MetricId ?? prediction.MetricName ?? ""));

            if (trend?.MetricValues == null) return 0.5;

            var dataPoints = trend.MetricValues.Count;
            var variability = CalculateVariability(trend.MetricValues.Values.ToArray());

            // More data points and lower variability = higher confidence
            return Math.Min(0.95, (dataPoints / 100.0) * (1.0 - variability));
        }

        private double CalculateVariability(double[] values)
        {
            if (values.Length == 0) return 1;
            var avg = values.Average();
            var variance = values.Select(v => Math.Pow(v - avg, 2)).Average();
            return Math.Sqrt(variance) / avg;
        }
    }
}
