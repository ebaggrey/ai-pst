
// Services/Analysis/CurrentStateAnalyzerService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class CurrentStateAnalyzerService : ICurrentStateAnalyzer
    {
        private readonly ILogger<CurrentStateAnalyzerService> _logger;

        public CurrentStateAnalyzerService(ILogger<CurrentStateAnalyzerService> logger)
        {
            _logger = logger;
        }

        public async Task<CurrentStateAnalysis> AnalyzeCurrentStateAsync(
            MetricValue[] currentMetrics,
            HistoricalTrend[] trends)
        {
            _logger.LogInformation("Analyzing current state with {MetricCount} metrics",
                currentMetrics?.Length ?? 0);

            if (currentMetrics == null || currentMetrics.Length == 0)
            {
                return new CurrentStateAnalysis
                {
                    AnalysisDate = DateTime.UtcNow,
                    CurrentValues = new Dictionary<string, double>(),
                    Strengths = Array.Empty<string>(),
                    Weaknesses = Array.Empty<string>()
                };
            }

            var currentValues = currentMetrics.ToDictionary(m => m.MetricId ?? m.MetricName ?? "unknown", m => m.Value);

            var strengths = new List<string>();
            var weaknesses = new List<string>();

            foreach (var metric in currentMetrics)
            {
                var trend = trends?.FirstOrDefault(t =>
                    t.MetricValues != null && t.MetricValues.ContainsKey(metric.MetricId ?? metric.MetricName ?? ""));

                if (trend != null && trend.MetricValues != null)
                {
                    var historicalAvg = trend.MetricValues.Values.Average();
                    if (metric.Value > historicalAvg * 1.1)
                        strengths.Add($"{metric.MetricName} is performing above historical average");
                    else if (metric.Value < historicalAvg * 0.9)
                        weaknesses.Add($"{metric.MetricName} is below historical average");
                }
            }

            return await Task.FromResult(new CurrentStateAnalysis
            {
                AnalysisDate = DateTime.UtcNow,
                CurrentValues = currentValues,
                Strengths = strengths.ToArray(),
                Weaknesses = weaknesses.ToArray()
            });
        }
    }
}