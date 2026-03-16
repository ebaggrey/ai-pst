
using Chapter_10.Interfaces;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class DecisionSupportGeneratorService : IDecisionSupportGenerator
    {
        private readonly ILogger<DecisionSupportGeneratorService> _logger;

        public DecisionSupportGeneratorService(ILogger<DecisionSupportGeneratorService> logger)
        {
            _logger = logger;
        }

        public async Task<DecisionSupport> GenerateDecisionSupportAsync(
            MetricPrediction[] predictions,
            Intervention[] interventions)
        {
            _logger.LogInformation("Generating decision support");

            var decisionPoints = new List<string>();
            var recommendedActions = new List<string>();
            var tradeoffs = new Dictionary<string, string>();

            if (predictions != null && predictions.Length > 0)
            {
                // Identify key decision points
                if (predictions.Any(p => IsDeclining(p)))
                {
                    decisionPoints.Add("Declining metrics detected - consider intervention");
                    recommendedActions.Add("Implement preventive measures for declining metrics");
                    tradeoffs["Early intervention"] = "May incur costs but prevents larger issues";
                }

                if (predictions.Any(p => IsImproving(p)))
                {
                    decisionPoints.Add("Improving metrics - consider scaling successful practices");
                    recommendedActions.Add("Analyze and replicate success factors");
                    tradeoffs["Scaling improvements"] = "Resource allocation vs. potential gains";
                }

                if (interventions != null && interventions.Length > 0)
                {
                    decisionPoints.Add("Multiple interventions available - prioritize by impact");

                    var topIntervention = interventions.OrderByDescending(i => i.ExpectedImpact).FirstOrDefault();
                    if (topIntervention != null)
                    {
                        recommendedActions.Add($"Prioritize: {topIntervention.Description}");
                    }
                }
            }

            return await Task.FromResult(new DecisionSupport
            {
                DecisionPoints = decisionPoints.ToArray(),
                RecommendedActions = recommendedActions.ToArray(),
                Tradeoffs = tradeoffs
            });
        }

        private bool IsDeclining(MetricPrediction prediction)
        {
            if (prediction.Values == null || prediction.Values.Length < 2)
                return false;
            return prediction.Values.Last() < prediction.Values.First();
        }

        private bool IsImproving(MetricPrediction prediction)
        {
            if (prediction.Values == null || prediction.Values.Length < 2)
                return false;
            return prediction.Values.Last() > prediction.Values.First();
        }
    }
}
