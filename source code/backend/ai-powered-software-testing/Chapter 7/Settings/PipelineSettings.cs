namespace Chapter_7.Settings
{
    public class PipelineSettings
    {
        public int MaxStagesPerPipeline { get; set; }
        public int DefaultTimeoutMinutes { get; set; }
        public bool EnableTelemetry { get; set; }
        public int RetryCount { get; set; }
        public int RetryDelaySeconds { get; set; }
    }
}
