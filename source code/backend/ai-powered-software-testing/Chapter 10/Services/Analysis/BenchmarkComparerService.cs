//namespace Chapter_10.Services.Analysis
//{
//    public class BenchmarkComparerService
//    {
//    }
//}

// Services/Analysis/BenchmarkComparerService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class BenchmarkComparerService : IBenchmarkComparer
    {
        private readonly ILogger<BenchmarkComparerService> _logger;

        public BenchmarkComparerService(ILogger<BenchmarkComparerService> logger)
        {
            _logger = logger;
        }

        public async Task<BenchmarkComparison> CompareToBenchmarksAsync(
            HealthScore healthScore,
            HistoricalBaseline[] baselines)
        {
            _logger.LogInformation("Comparing to benchmarks");

            if (healthScore == null || baselines == null || baselines.Length == 0)
            {
                return new BenchmarkComparison
                {
                    Percentile = 50,
                    GapToAverage = 0,
                    GapToBest = 0
                };
            }

            var avgScore = baselines.Average(b => b.Mean);
            var bestScore = baselines.Max(b => b.Mean);

            return await Task.FromResult(new BenchmarkComparison
            {
                Percentile = CalculatePercentile(healthScore.OverallScore, baselines),
                GapToAverage = healthScore.OverallScore - avgScore,
                GapToBest = bestScore - healthScore.OverallScore
            });
        }

        private double CalculatePercentile(double score, HistoricalBaseline[] baselines)
        {
            var scores = baselines.Select(b => b.Mean).ToList();
            scores.Add(score);
            scores.Sort();

            int index = scores.IndexOf(score);
            return (double)index / scores.Count * 100;
        }

        Task<BenchmarkComparison> IBenchmarkComparer.CompareToBenchmarksAsync(HealthScore healthScore, HistoricalBaseline[] baselines)
        {
            throw new NotImplementedException();
        }
    }
}