namespace Chapter_7.Models.Responses
{
   
    public class IntelligentPipelineResponse
    {
        public string PipelineId { get; set; }
        public PipelineDefinition PipelineDefinition { get; set; }
        public DecisionPoint[] DecisionPoints { get; set; }
        public RecoveryPath[] RecoveryPaths { get; set; }
        public EstimatedMetrics EstimatedMetrics { get; set; }
        public OptimizationSuggestion[] OptimizationSuggestions { get; set; }
        public MonitoringConfiguration MonitoringConfiguration { get; set; }
        public AdaptationGuidance AdaptationGuidance { get; set; }
        public ConflictingConstraint[] ConstraintConflicts { get; set; }
        public ConstraintSuggestion[] ConstraintSuggestions { get; set; }
        public PartialPipeline PartialPipeline { get; set; }
    }

    public class PipelineDefinition
    {
        public Stage[] Stages { get; set; }
    }

    public class Stage
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DecisionPoint
    {
        public string Id { get; set; }
        public string Condition { get; set; }
    }

    public class RecoveryPath
    {
        public string Id { get; set; }
        public string[] Steps { get; set; }
    }

    public class EstimatedMetrics
    {
        public TimeSpan EstimatedDuration { get; set; }
        public double EstimatedCost { get; set; }
    }

    public class OptimizationSuggestion
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class MonitoringConfiguration
    {
        public string[] Metrics { get; set; }
    }

    public class AdaptationGuidance
    {
        public string[] Recommendations { get; set; }
    }

    public class ConflictingConstraint
    {
        public string ConstraintA { get; set; }
        public string ConstraintB { get; set; }
    }

    public class ConstraintSuggestion
    {
        public string Constraint { get; set; }
        public string SuggestedValue { get; set; }
    }

    public class PartialPipeline
    {
        public Stage[] Stages { get; set; }
    }
}
