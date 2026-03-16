namespace Chapter_7.Data.Entities
{

    // Statistics DTO
    public class PipelineStatistics
    {
        public int TotalRuns { get; set; }
        public int SuccessfulRuns { get; set; }
        public int FailedRuns { get; set; }
        public double? AverageDuration { get; set; }
        public DateTimeOffset? FirstRun { get; set; }
        public DateTimeOffset? LastRun { get; set; }
        public decimal SuccessRate => TotalRuns > 0 ? (decimal)SuccessfulRuns / TotalRuns : 0;
    }

    public class Pipeline
    {
        public Guid Id { get; set; }
        public string PipelineId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string PipelineDefinition { get; set; }
        public string Language { get; set; }
        public decimal TestCoverage { get; set; }
        public int TotalLines { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation properties
        public virtual ICollection<PipelineStage> Stages { get; set; }
        public virtual ICollection<PipelineRun> Runs { get; set; }
        public virtual ICollection<OptimizationOpportunity> OptimizationOpportunities { get; set; }
        public virtual ICollection<AdaptationEvent> AdaptationEvents { get; set; }
        public virtual ICollection<PredictionResult> PredictionResults { get; set; }
    }

    public class PipelineStage
    {
        public Guid Id { get; set; }
        public Guid PipelineId { get; set; }
        public string StageId { get; set; }
        public string StageName { get; set; }
        public int StageOrder { get; set; }
        public string? Configuration { get; set; }
        public int? AverageDuration { get; set; }
        public decimal? SuccessRate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }

        // Navigation properties
        public virtual Pipeline Pipeline { get; set; }
    }

    public class PipelineRun
    {
        public Guid Id { get; set; }
        public Guid PipelineId { get; set; }
        public string RunId { get; set; }
        public string Status { get; set; }
        public int? Duration { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorType { get; set; }
        public string? StackTrace { get; set; }
        public string TriggeredBy { get; set; }
        public DateTimeOffset TriggeredAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation properties
        public virtual Pipeline Pipeline { get; set; }
        public virtual ICollection<PipelineFailure> Failures { get; set; }
    }

    public class PipelineFailure
    {
        public Guid Id { get; set; }
        public Guid RunId { get; set; }
        public string FailureType { get; set; }
        public string FailureMessage { get; set; }
        public string FailureStage { get; set; }
        public string? RawLogs { get; set; }
        public string? RootCause { get; set; }
        public string? Resolution { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation properties
        public virtual PipelineRun Run { get; set; }
    }

    public class OptimizationOpportunity
    {
        public Guid Id { get; set; }
        public Guid PipelineId { get; set; }
        public string OpportunityId { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Impact { get; set; }
        public decimal Effort { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? ImplementedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation properties
        public virtual Pipeline Pipeline { get; set; }
    }

    public class AdaptationEvent
    {
        public Guid Id { get; set; }
        public Guid PipelineId { get; set; }
        public string AdaptationId { get; set; }
        public string ChangeType { get; set; }
        public string AdaptationPlan { get; set; }
        public decimal ImpactScore { get; set; }
        public string? ValidationResults { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? ImplementedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation properties
        public virtual Pipeline Pipeline { get; set; }
    }

    public class PredictionModel
    {
        public Guid Id { get; set; }
        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string ModelType { get; set; }
        public string ModelData { get; set; }
        public decimal? Accuracy { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? DeactivatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<PredictionResult> PredictionResults { get; set; }
    }

    public class PredictionResult
    {
        public Guid Id { get; set; }
        public Guid PipelineId { get; set; }
        public string PredictionId { get; set; }
        public Guid ModelId { get; set; }
        public string PredictionType { get; set; }
        public string PredictionData { get; set; }
        public decimal Confidence { get; set; }
        public string? ActualOutcome { get; set; }
        public DateTimeOffset? ValidatedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation properties
        public virtual Pipeline Pipeline { get; set; }
        public virtual PredictionModel Model { get; set; }
    }
}
