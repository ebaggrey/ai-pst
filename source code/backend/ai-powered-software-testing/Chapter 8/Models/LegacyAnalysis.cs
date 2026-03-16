

using Chapter_8.Models.Requests;

namespace Chapter_8.Models
{
    public class LegacyAnalysis
    {
        public string AnalysisId { get; set; }
        public DateTime AnalysisDate { get; set; }
        public CodebaseInfo CodebaseInfo { get; set; }
        public AnalysisMetrics Metrics { get; set; }
        public string[] Findings { get; set; }
        public string[] Recommendations { get; set; }
        public Dictionary<string, object> RawData { get; set; }
    }

    public class AnalysisMetrics
    {
        public int TotalFiles { get; set; }
        public long TotalLinesOfCode { get; set; }
        public int TotalClasses { get; set; }
        public int TotalMethods { get; set; }
        public double AverageComplexity { get; set; }
        public double MaxComplexity { get; set; }
        public int TotalDependencies { get; set; }
        public int CircularDependencies { get; set; }
        public int CodeSmellsCount { get; set; }
        public double TechnicalDebtRatio { get; set; }
        public double TestCoverage { get; set; }
        public int SecurityVulnerabilities { get; set; }
    }
}
