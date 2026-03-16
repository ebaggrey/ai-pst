
// Services/Analysis/RiskAssessorService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class RiskAssessorService : IRiskAssessor
    {
        private readonly ILogger<RiskAssessorService> _logger;

        public RiskAssessorService(ILogger<RiskAssessorService> logger)
        {
            _logger = logger;
        }

        public async Task<RiskAssessment> AssessPredictionRisksAsync(
            MetricPrediction[] predictions,
            HistoricalTrend[] trends)
        {
            _logger.LogInformation("Assessing prediction risks");

            var risks = new List<string>();
            var mitigations = new List<string>();

            if (predictions == null || predictions.Length == 0)
            {
                return new RiskAssessment
                {
                    OverallRisk = "high",
                    IdentifiedRisks = new[] { "No predictions available" },
                    MitigationStrategies = new[] { "Collect more data" }
                };
            }

            // Assess various risks
            if (HasDownwardTrendRisk(predictions))
            {
                risks.Add("Multiple metrics showing declining trends");
                mitigations.Add("Implement preventive interventions");
            }

            if (HasVolatilityRisk(predictions))
            {
                risks.Add("High volatility in key metrics");
                mitigations.Add("Stabilize measurement process");
            }

            if (HasDataGapRisk(trends))
            {
                risks.Add("Insufficient historical data for reliable predictions");
                mitigations.Add("Continue data collection before acting on predictions");
            }

            string overallRisk = DetermineOverallRisk(risks.Count);

            return await Task.FromResult(new RiskAssessment
            {
                OverallRisk = overallRisk,
                IdentifiedRisks = risks.ToArray(),
                MitigationStrategies = mitigations.ToArray()
            });
        }

        private bool HasDownwardTrendRisk(MetricPrediction[] predictions)
        {
            return predictions.Count(p =>
                p.Values != null &&
                p.Values.Length > 1 &&
                p.Values.Last() < p.Values.First()) > predictions.Length / 2;
        }

        private bool HasVolatilityRisk(MetricPrediction[] predictions)
        {
            return predictions.Any(p =>
                p.Values != null &&
                CalculateVolatility(p.Values) > 0.3);
        }

        private double CalculateVolatility(double[] values)
        {
            if (values.Length < 2) return 0;
            var changes = new List<double>();
            for (int i = 1; i < values.Length; i++)
            {
                changes.Add(Math.Abs(values[i] - values[i - 1]) / values[i - 1]);
            }
            return changes.Average();
        }

        private bool HasDataGapRisk(HistoricalTrend[] trends)
        {
            return trends == null || trends.Length < 10;
        }

        private string DetermineOverallRisk(int riskCount)
        {
            return riskCount switch
            {
                0 => "low",
                1 => "medium",
                _ => "high"
            };
        }
    }
}