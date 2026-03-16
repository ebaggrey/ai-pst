
// Services/Analysis/InterventionGeneratorService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class InterventionGeneratorService : IInterventionGenerator
    {
        private readonly ILogger<InterventionGeneratorService> _logger;

        public InterventionGeneratorService(ILogger<InterventionGeneratorService> logger)
        {
            _logger = logger;
        }

        public async Task<Intervention[]> GenerateInterventionsAsync(
            MetricPrediction[] predictions,
            MetricValue[] currentMetrics,
            HistoricalTrend[] trends)
        {
            _logger.LogInformation("Generating interventions based on {PredictionCount} predictions",
                predictions?.Length ?? 0);

            if (predictions == null || predictions.Length == 0)
                return Array.Empty<Intervention>();

            var interventions = new List<Intervention>();

            foreach (var prediction in predictions)
            {
                if (IsDecliningTrend(prediction))
                {
                    interventions.Add(new Intervention
                    {
                        MetricId = prediction.MetricId,
                        Type = "preventive",
                        Description = $"Intervene to prevent decline in {prediction.MetricName}",
                        RecommendedDate = DateTime.UtcNow.AddDays(7),
                        ExpectedImpact = 0.15
                    });
                }

                if (HasHighVolatility(prediction))
                {
                    interventions.Add(new Intervention
                    {
                        MetricId = prediction.MetricId,
                        Type = "stabilizing",
                        Description = $"Stabilize {prediction.MetricName} through process improvements",
                        RecommendedDate = DateTime.UtcNow.AddDays(14),
                        ExpectedImpact = 0.1
                    });
                }
            }

            return await Task.FromResult(interventions.ToArray());
        }

        private bool IsDecliningTrend(MetricPrediction prediction)
        {
            if (prediction.Values == null || prediction.Values.Length < 2)
                return false;

            return prediction.Values.Last() < prediction.Values.First() * 0.9;
        }

        private bool HasHighVolatility(MetricPrediction prediction)
        {
            if (prediction.Values == null || prediction.Values.Length == 0)
                return false;

            var avg = prediction.Values.Average();
            var maxDeviation = prediction.Values.Max(v => Math.Abs(v - avg)) / avg;
            return maxDeviation > 0.2;
        }
    }
}