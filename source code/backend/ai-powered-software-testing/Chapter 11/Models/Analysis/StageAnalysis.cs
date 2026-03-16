
// Models/Analysis/StageAnalysis.cs
using Chapter_11.Models.Requests;

namespace Chapter_11.Models.Analysis
{
    public class StageAnalysis
    {
        public DevelopmentStage[] Stages { get; set; }
        public StageDependency[] Dependencies { get; set; }
        public StageMetric[] Metrics { get; set; }
    }

    public class StageDependency
    {
        public string SourceStageId { get; set; }
        public string TargetStageId { get; set; }
        public string DependencyType { get; set; }
    }

    public class StageMetric
    {
        public string StageId { get; set; }
        public string MetricName { get; set; }
        public double Value { get; set; }
    }
}