
// Models/Requests/PipelineRequest.cs
namespace Chapter_11.Models.Requests
{
    public class PipelineRequest
    {
        public DevelopmentStage[] DevelopmentStages { get; set; }
        public QualityGate[] QualityGates { get; set; }
        public SpectrumCoverage SpectrumCoverage { get; set; }
        public FeedbackMechanism[] FeedbackMechanisms { get; set; }
    }

    public class DevelopmentStage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Activities { get; set; }
        public string[] Dependencies { get; set; }
    }

    public class QualityGate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Conditions { get; set; }
        public GateAction Action { get; set; }
    }

    public enum GateAction
    {
        Proceed,
        Warn,
        Block
    }

    public class FeedbackMechanism
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Channel { get; set; }
    }

    public enum SpectrumCoverage
    {
        LeftOnly,
        RightOnly,
        FullSpectrum,
        Custom
    }
}
