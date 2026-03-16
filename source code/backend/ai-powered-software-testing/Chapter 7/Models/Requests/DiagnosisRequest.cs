
namespace Chapter_7.Models.Requests
{
    public class DiagnosisRequest
    {
        public FailureLogs FailureLogs { get; set; }
        public RecentChanges RecentChanges { get; set; }
        public DiagnosisDepth DiagnosisDepth { get; set; }
        public bool IncludeRemediation { get; set; }
        public bool PreventionStrategies { get; set; }
        public PipelineContext PipelineContext { get; set; }
    }

    public class FailureLogs
    {
        public string RawLogs { get; set; }
        public DateTime FailureTime { get; set; }
    }

    public class RecentChanges
    {
        public Change[] Changes { get; set; }
    }

    public class Change
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }

    public enum DiagnosisDepth
    {
        Shallow,
        Standard,
        Deep
    }

    public class PipelineContext
    {
        public string PipelineId { get; set; }
        public string Stage { get; set; }
    }
}