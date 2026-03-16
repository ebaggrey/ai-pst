
// Services/Analysis/TrendAnalyzerService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;

namespace Chapter_10.Services.Analysis
{
    public class TrendAnalyzerService : ITrendAnalyzer
    {
        private readonly ILogger<TrendAnalyzerService> _logger;

        public TrendAnalyzerService(ILogger<TrendAnalyzerService> logger)
        {
            _logger = logger;
        }

        public async Task<TrendAnalysis> AnalyzeHealthTrendsAsync(
            HealthScore healthScore,
            HistoricalBaseline[] baselines)
        {
            _logger.LogInformation("Analyzing health trends");

            if (healthScore == null)
                return new TrendAnalysis
                {
                    Direction = "stable",
                    Rate = 0,
                    Observations = Array.Empty<string>()
                };

            // Simplified trend analysis
            string direction = DetermineDirection(healthScore);
            double rate = CalculateTrendRate(healthScore, baselines);

            return await Task.FromResult(new TrendAnalysis
            {
                Direction = direction,
                Rate = rate,
                Observations = GenerateObservations(healthScore, direction, rate)
            });
        }

        private string DetermineDirection(HealthScore healthScore)
        {
            if (healthScore.OverallScore > 80) return "improving";
            if (healthScore.OverallScore > 60) return "stable";
            return "declining";
        }

        private double CalculateTrendRate(HealthScore healthScore, HistoricalBaseline[] baselines)
        {
            // Simplified rate calculation
            return (healthScore.OverallScore - 50) / 100;
        }

        private string[] GenerateObservations(HealthScore healthScore, string direction, double rate)
        {
            return new[]
            {
                $"Overall health is {direction} at {rate:P1} per month",
                $"Highest scoring component: {healthScore.ComponentScores?.OrderByDescending(c => c.Score).FirstOrDefault()?.ComponentName}",
                $"Lowest scoring component: {healthScore.ComponentScores?.OrderBy(c => c.Score).FirstOrDefault()?.ComponentName}"
            };
        }

        Task<TrendAnalysis> ITrendAnalyzer.AnalyzeHealthTrendsAsync(HealthScore healthScore, HistoricalBaseline[] baselines)
        {
            throw new NotImplementedException();
        }
    }
}