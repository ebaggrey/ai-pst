namespace Chapter_7.Models.Requests
{
 
    public class PredictionRequest
    {
        public ProposedChange[] ProposedChanges { get; set; }
        public HistoricalData HistoricalData { get; set; }
        public TimeSpan PredictionHorizon { get; set; }
        public double ConfidenceThreshold { get; set; }
        public bool IncludeMitigations { get; set; }
    }

    public class ProposedChange
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }

    public class HistoricalData
    {
        public PipelineRun[] Runs { get; set; }
    }

    public class PipelineRun
    {
        public string RunId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
    }
}
