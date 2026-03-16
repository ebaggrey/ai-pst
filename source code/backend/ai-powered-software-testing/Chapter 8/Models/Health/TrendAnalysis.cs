
using Chapter_8.Models.Responses;

namespace Chapter_8.Models.Health
{
    public class TrendAnalysis
    {
        public Trend[] IncreasingTrends { get; set; }
        public Trend[] DecreasingTrends { get; set; }
        public Trend[] StableTrends { get; set; }
        public SeasonalPattern[] SeasonalPatterns { get; set; }
        public AnomalyTrend[] AnomalyTrends { get; set; }
        public Correlation[] Correlations { get; set; }
    }

    public class SeasonalPattern
    {
        public string Metric { get; set; }
        public string Pattern { get; set; } // "daily", "weekly", "monthly", "quarterly"
        public double Confidence { get; set; }
        public Dictionary<string, double> TypicalRanges { get; set; }
    }

    public class AnomalyTrend
    {
        public string Metric { get; set; }
        public int AnomalyCount { get; set; }
        public string[] CommonTypes { get; set; }
        public string[] AffectedSystems { get; set; }
        public double FrequencyTrend { get; set; } // positive means increasing
    }

    public class Correlation
    {
        public string Metric1 { get; set; }
        public string Metric2 { get; set; }
        public double CorrelationCoefficient { get; set; } // -1 to 1
        public string Relationship { get; set; } // "positive", "negative", "none"
        public string Description { get; set; }
    }
}