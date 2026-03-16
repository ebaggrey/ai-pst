
// Services/Analysis/PatternIdentifierService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;


namespace Chapter_10.Services.Analysis
{
    public class PatternIdentifierService : IPatternIdentifier
    {
        private readonly ILogger<PatternIdentifierService> _logger;
        private readonly IConfiguration _configuration;

        public PatternIdentifierService(
            ILogger<PatternIdentifierService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<DetectedPattern[]> IdentifyPatternsAsync(
            HistoricalTrend[] trends,
            int horizon)
        {
            _logger.LogInformation("Identifying patterns from {TrendCount} trends", trends?.Length ?? 0);

            if (trends == null || trends.Length == 0)
                return Array.Empty<DetectedPattern>();

            var patterns = new List<DetectedPattern>();

            foreach (var trend in trends)
            {
                if (trend.MetricValues == null) continue;

                var values = trend.MetricValues.Values.ToArray();

                // Detect trend pattern
                var trendPattern = DetectTrend(values);
                if (trendPattern != null)
                    patterns.Add(trendPattern);

                // Detect seasonality
                var seasonalPattern = DetectSeasonality(values, trend.Seasonality);
                if (seasonalPattern != null)
                    patterns.Add(seasonalPattern);
            }

            return await Task.FromResult(patterns.ToArray());
        }

        private DetectedPattern? DetectTrend(double[] values)
        {
            if (values.Length < 2) return null;

            var firstHalf = values.Take(values.Length / 2).Average();
            var secondHalf = values.Skip(values.Length / 2).Average();

            if (Math.Abs(secondHalf - firstHalf) < 0.01) return null;

            return new DetectedPattern
            {
                Type = secondHalf > firstHalf ? "upward_trend" : "downward_trend",
                Strength = Math.Abs(secondHalf - firstHalf) / firstHalf,
                Parameters = new Dictionary<string, object>
                {
                    ["slope"] = (secondHalf - firstHalf) / (values.Length / 2)
                }
            };
        }

        private DetectedPattern? DetectSeasonality(double[] values, string? seasonality)
        {
            if (string.IsNullOrEmpty(seasonality)) return null;

            return new DetectedPattern
            {
                Type = "seasonality",
                Strength = 0.6,
                Parameters = new Dictionary<string, object>
                {
                    ["period"] = seasonality,
                    ["amplitude"] = CalculateAmplitude(values)
                }
            };
        }

        private double CalculateAmplitude(double[] values)
        {
            if (values.Length == 0) return 0;
            return (values.Max() - values.Min()) / 2;
        }
    }
}
