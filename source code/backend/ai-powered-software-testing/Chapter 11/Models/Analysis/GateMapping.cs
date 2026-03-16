

// Models/Analysis/GateMapping.cs
using Chapter_11.Models.Requests;

namespace Chapter_11.Models.Analysis
{
    public class GateMapping
    {
        public QualityGate[] Gates { get; set; }
        public GateStageMapping[] StageMappings { get; set; }
    }

    public class GateStageMapping
    {
        public string GateId { get; set; }
        public string StageId { get; set; }
        public int ExecutionOrder { get; set; }
    }
}