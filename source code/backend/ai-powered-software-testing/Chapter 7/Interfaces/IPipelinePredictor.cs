using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;

namespace Chapter_7.Interfaces
{
    public interface IPipelinePredictor
    {
        Task<Prediction[]> PredictIssuesAsync(
            ChangeAnalysis changeAnalysis,
            PatternMatch[] patternMatches,
            TimeSpan predictionHorizon,
            double confidenceThreshold);
    }

    public class ChangeAnalysis
    {
        public ProposedChange[] Changes { get; set; }
        public Dictionary<string, double> ImpactScores { get; set; }
    }
}
