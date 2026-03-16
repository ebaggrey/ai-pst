using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chapter2_Ext.Models
{
    public class PatternEstablishmentRequest
    {
        [Required]
        public string? Area { get; set; }

        [Required]
        public List<TestExampleDto> Examples { get; set; }

        [Required]
        [RegularExpression("low|medium|high")]
        public string DesiredConsistency { get; set; } = "high";

        [Required]
        [RegularExpression("manual|semi-automated|fully-automated")]
        public string AutomationLevel { get; set; } = "semi-automated";

        [Required]
        public List<string> ValidationCriteria { get; set; }

        public RequestMetadata Metadata { get; set; }
    }

    public class TestExampleDto
    {
        public string TestName { get; set; }
        public string Input { get; set; }
        public string ExpectedOutput { get; set; }
        public string ActualOutput { get; set; }
        public List<string> Tags { get; set; }
        public string Complexity { get; set; }
    }

    public class RequestMetadata
    {
        public int TeamSize { get; set; }
        public string ExperienceLevel { get; set; }
        public string Timeline { get; set; }
    }

    public class TrainingGenerationRequest
    {
        [Required]
        public TestingPatternDto Pattern { get; set; }

        [Required]
        public string Audience { get; set; }

        [Required]
        public string Format { get; set; }

        [Range(15, 480)]
        public int DurationMinutes { get; set; } = 60;

        public bool IncludeHandsOn { get; set; } = true;

        public List<string> Prerequisites { get; set; }

        [Required]
        public List<string> LearningObjectives { get; set; }
    }

    public class PipelineRequest
    {
        [Required]
        public string PatternId { get; set; }

        public List<string> TriggerEvents { get; set; }

        [Required]
        public List<PipelineStageDto> Stages { get; set; }

        [Required]
        public List<QualityGateDto> QualityGates { get; set; }
    }

    public class PipelineStageDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public List<string> PipelineStageTasks { get; set; }
    }

    public class QualityGateDto
    {
        [Required]
        public string Metric { get; set; }

        [Range(0, 100)]
        public double Threshold { get; set; }
    }
}
