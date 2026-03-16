
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chapter2_Ext.Models
{
    public class TestingPatternDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Area { get; set; }
        public string ProblemStatement { get; set; }
        public string Solution { get; set; }
        public PatternImplementation Implementation { get; set; }
        public QualityIndicators QualityIndicators { get; set; }
        public AiAssistance AiAssistance { get; set; }
        public AdoptionMetrics AdoptionMetrics { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "draft";
    }

    public class PatternImplementation
    {
        public List<string> CodeExamples { get; set; }
        public Dictionary<string, object> Configuration { get; set; }
        public List<string> DosAndDonts { get; set; }
    }

    public class QualityIndicators
    {
        [Range(0, 100)]
        public double RepeatabilityScore { get; set; }

        public string LearningCurve { get; set; } // "easy", "medium", "steep"

        public string MaintenanceCost { get; set; } // "low", "medium", "high"
    }

    public class AiAssistance
    {
        public List<string> PromptTemplates { get; set; }
        public List<string> ValidationRules { get; set; }
        public List<string> CommonPitfalls { get; set; }
    }

    public class AdoptionMetrics
    {
        public string EstimatedTimeSave { get; set; }
        public string ErrorReduction { get; set; }

        [Range(1, 10)]
        public int TeamSatisfaction { get; set; }
    }

    public class TrainingMaterials
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PatternId { get; set; }
        public string Audience { get; set; }
        public string Title { get; set; }
        public string Format { get; set; }
        public int DurationMinutes { get; set; }
        public List<Module> Modules { get; set; }
        public List<string> Prerequisites { get; set; }
        public List<string> LearningObjectives { get; set; }
        public HandsOnSection HandsOn { get; set; }
        public Assessment Assessment { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class Module
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int DurationMinutes { get; set; }
        public List<string> KeyPoints { get; set; }
    }

    public class HandsOnSection
    {
        public bool Included { get; set; }
        public string SetupInstructions { get; set; }
        public List<Exercise> Exercises { get; set; }
        public string ExpectedOutcome { get; set; }
    }

    public class Exercise
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string SolutionHint { get; set; }
        public string Solution { get; set; }
    }

    public class Assessment
    {
        public List<Question> QuizQuestions { get; set; }
        public PracticalTask PracticalTask { get; set; }
        public double PassingScore { get; set; } = 80;
    }

    public class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
    }

    public class PracticalTask
    {
        public string Description { get; set; }
        public List<string> Requirements { get; set; }
        public List<string> SuccessCriteria { get; set; }
    }

    public class PipelineBlueprint
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PatternId { get; set; }
        public string Name { get; set; }
        public List<string> TriggerEvents { get; set; }
        public List<PipelineStage> Stages { get; set; }
        public List<QualityGate> QualityGates { get; set; }
        public DeploymentConfiguration DeploymentConfig { get; set; }
        public MonitoringConfiguration MonitoringConfig { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PipelineStage
    {
        public string Name { get; set; }
        public List<PipelineStageTask> Tasks { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string Executor { get; set; } = "default";
    }

    public class PipelineStageTask
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object> Configuration { get; set; }
        public int TimeoutMinutes { get; set; } = 30;
    }

    public class QualityGate
    {
        public string Metric { get; set; }
        public double Threshold { get; set; }
        public string Operator { get; set; } = ">=";
        public string ActionOnFail { get; set; } = "block";
    }

    public class DeploymentConfiguration
    {
        public string Environment { get; set; } = "development";
        public bool AutoDeploy { get; set; }
        public List<string> Approvers { get; set; }
        public RollbackStrategy RollbackStrategy { get; set; }
    }

    public class RollbackStrategy
    {
        public bool Enabled { get; set; }
        public string TriggerCondition { get; set; }
        public string Method { get; set; }
    }

    public class MonitoringConfiguration
    {
        public List<string> Metrics { get; set; }
        public List<AlertRule> AlertRules { get; set; }
        public string DashboardUrl { get; set; }
    }

    public class AlertRule
    {
        public string Name { get; set; }
        public string Condition { get; set; }
        public List<string> NotifyChannels { get; set; }
    }


}


namespace Chapter2_Ext.Models
{
    public class PatternError
    {
        public string PatternArea { get; set; }
        public string FailureType { get; set; } // "generation", "validation", "adoption"
        public List<string> Symptoms { get; set; }
        public string RootCause { get; set; }
        public List<string> MitigationSteps { get; set; }
        public string TemporaryWorkaround { get; set; }
        public DateTime ErrorTime { get; set; } = DateTime.UtcNow;
        public string CorrelationId { get; set; }
    }

    public class ApiErrorResponse
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string CorrelationId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public PatternError PatternError { get; set; }
    }

    public class CustomExceptions
    {
        public class PatternEstablishmentException : Exception
        {
            public string ErrorCode { get; }
            public PatternError PatternError { get; }

            public PatternEstablishmentException(string message, string errorCode, PatternError patternError = null)
                : base(message)
            {
                ErrorCode = errorCode;
                PatternError = patternError;
            }
        }

        public class PatternGenerationException : PatternEstablishmentException
        {
            public PatternGenerationException(string message, PatternError patternError = null)
                : base(message, "PATTERN_GENERATION_FAILED", patternError) { }
        }

        public class PatternValidationException : PatternEstablishmentException
        {
            public PatternValidationException(string message, PatternError patternError = null)
                : base(message, "PATTERN_VALIDATION_FAILED", patternError) { }
        }

        public class PatternAdoptionException : PatternEstablishmentException
        {
            public PatternAdoptionException(string message, PatternError patternError = null)
                : base(message, "PATTERN_ADOPTION_FAILED", patternError) { }
        }
    }
}