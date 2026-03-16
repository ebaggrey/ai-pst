using Chapter_7.Models.Requests;

namespace Chapter_7.Models.Responses
{
    public class PredictionResponse
    {
        public string PredictionId { get; set; }
        public ProposedChange[] ProposedChanges { get; set; }
        public Prediction[] Predictions { get; set; }
        public double[] ConfidenceScores { get; set; }
        public RiskScore[] RiskScores { get; set; }
        public Mitigation[] Mitigations { get; set; }
        public RecommendedAction[] RecommendedActions { get; set; }
        public MonitoringRecommendation[] MonitoringRecommendations { get; set; }
        public PatternMatch[] HistoricalEvidence { get; set; }
    }

    public class Prediction
    {
        public string Issue { get; set; }
        public double Probability { get; set; }
    }

    public class RiskScore
    {
        public string Category { get; set; }
        public double Score { get; set; }
    }

    public class Mitigation
    {
        public string PredictionId { get; set; }
        public string Action { get; set; }
    }

    public class RecommendedAction
    {
        public string Action { get; set; }
        public double Priority { get; set; }
    }

    public class MonitoringRecommendation
    {
        public string Metric { get; set; }
        public double Threshold { get; set; }
    }

    public class PatternMatch
    {
        public string PatternId { get; set; }
        public double Similarity { get; set; }
    }
}
