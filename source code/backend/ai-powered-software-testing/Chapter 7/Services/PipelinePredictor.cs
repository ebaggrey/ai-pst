using Chapter_7.Interfaces;
using Chapter_7.Models.Responses;

namespace Chapter_7.Services
{
    public class PipelinePredictor : IPipelinePredictor
    {
        private readonly ILogger<PipelinePredictor> _logger;

        public PipelinePredictor(ILogger<PipelinePredictor> logger)
        {
            _logger = logger;
        }

        public async Task<Prediction[]> PredictIssuesAsync(
            ChangeAnalysis changeAnalysis,
            PatternMatch[] patternMatches,
            TimeSpan predictionHorizon,
            double confidenceThreshold)
        {
            _logger.LogInformation("Predicting issues for {ChangeCount} changes",
                changeAnalysis.Changes.Length);

            await Task.Delay(100);

            var predictions = new List<Prediction>();

            foreach (var change in changeAnalysis.Changes)
            {
                var probability = change.Type switch
                {
                    "database" => 0.7,
                    "api" => 0.5,
                    "ui" => 0.3,
                    _ => 0.4
                };

                if (probability >= confidenceThreshold)
                {
                    predictions.Add(new Prediction
                    {
                        Issue = $"Potential failure in {change.Type} component",
                        Probability = probability
                    });
                }
            }

            return predictions.ToArray();
        }
    }
}
