using Chapter_3.Models.Responses;
using Chapter_3.Models.Supporting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Chapter_3.Models.Domain
{
    //// Models/Domain/GeneratedTest.cs
    //public class GeneratedTest
    //{
    //    public string Content { get; set; } = string.Empty;
    //    public double ConfidenceScore { get; set; }
    //    public DateTime GeneratedAt { get; set; }
    //    public string TestType { get; set; } = string.Empty;
    //    public string Framework { get; set; } = string.Empty;
    //    public string[] Tags { get; set; } = Array.Empty<string>();
    //    public string[] Dependencies { get; set; } = Array.Empty<string>();
    //    public Dictionary<string, object> Metadata { get; set; } = new();
    //}

    // Models/Domain/ReviewContext.cs
    public class ReviewContext
    {
        public string TestPurpose { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = "medium"; // low, medium, high, critical
        public string[] BusinessDomains { get; set; } = Array.Empty<string>();
        public string[] TechnicalDomains { get; set; } = Array.Empty<string>();
        public Dictionary<string, string> Constraints { get; set; } = new();
        public string Priority { get; set; } = "normal";
        public string[] Stakeholders { get; set; } = Array.Empty<string>();
    }

    //// Models/Domain/SubmissionMetadata.cs
    public class SubmissionMetadata
    {
        public string SubmittedBy { get; set; } = string.Empty;
        public string SubmittedFrom { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string RequestId { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalInfo { get; set; } = new();
    }

    // Models/Domain/ReviewSession.cs
    public class ReviewSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public GeneratedTest OriginalContent { get; set; } = new();
        public GeneratedTest CurrentContent { get; set; } = new();
        public ReviewContext Context { get; set; } = new();
        public ReviewAnalysis Analysis { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public ReviewSessionStatus Status { get; set; }
        public string AiConfidenceStatement { get; set; } = string.Empty;
        public JudgmentArea[] AreasNeedingHumanJudgment { get; set; } = Array.Empty<JudgmentArea>();
        public string[] SuggestedReviewFocus { get; set; } = Array.Empty<string>();
        public InitialQuestion[] InitialQuestions { get; set; } = Array.Empty<InitialQuestion>();
        public EditRecord[] EditHistory { get; set; } = Array.Empty<EditRecord>();
        public AiSuggestion[] AiSuggestions { get; set; } = Array.Empty<AiSuggestion>();
        public ClarificationThread[] ClarificationThreads { get; set; } = Array.Empty<ClarificationThread>();
        public DateTime? LastModified { get; set; }
        public DateTime? ClosedAt { get; set; }
        public ReviewOutcome? Outcome { get; set; }
        public SessionSummary? Summary { get; set; }

        public GeneratedContent GeneratedContent { get; set; } = new();
    }


    // Models/Responses/AiClarification.cs - UPDATED with ConfidenceStatement
    public class AiClarification
    {
        [Required]
        public string DirectAnswer { get; set; } = string.Empty;

        public string[] Alternatives { get; set; } = Array.Empty<string>();

        [Required]
        [Range(0, 1)]
        public double Confidence { get; set; }

        public string ConfidenceStatement { get; set; } = string.Empty;
        public string[] Assumptions { get; set; } = Array.Empty<string>();
        public string RecommendedAction { get; set; } = string.Empty;
        public string WhenToAskHuman { get; set; } = string.Empty;
        public string[] References { get; set; } = Array.Empty<string>();
        public string[] RelatedQuestions { get; set; } = Array.Empty<string>();

        // Additional metadata
        public string AnswerType { get; set; } = "direct"; // direct, alternative, cautionary
        public string[] KeyPoints { get; set; } = Array.Empty<string>();
        public string[] Limitations { get; set; } = Array.Empty<string>();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string ModelVersion { get; set; } = string.Empty;

        public Dictionary<string, object> Metadata { get; set; } = new();
    }


    // Models/Domain/ReviewAnalysis.cs
    public class ReviewAnalysis
    {
        public string[] NeedsHumanReview { get; set; } = Array.Empty<string>();
        public string[] LikelyCorrect { get; set; } = Array.Empty<string>();
        public string[] MissingEdgeCases { get; set; } = Array.Empty<string>();
        public string[] BusinessContextConcerns { get; set; } = Array.Empty<string>();
        public string[] FlakinessRisks { get; set; } = Array.Empty<string>();
        public double OverallConfidence { get; set; }
        public Dictionary<string, double> AreaConfidenceScores { get; set; } = new();
    }

    // Models/Domain/JudgmentArea.cs
    public class JudgmentArea
    {
        public string Area { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string WhyHumanNeeded { get; set; } = string.Empty;
        public string Guidance { get; set; } = string.Empty;
        public string[] Examples { get; set; } = Array.Empty<string>();
        public string[] CommonPitfalls { get; set; } = Array.Empty<string>();
    }

    // Models/Domain/InitialQuestion.cs
    public class InitialQuestion
    {
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // clarifying, probing, validating
        public string WhyImportant { get; set; } = string.Empty;
        public string[] Options { get; set; } = Array.Empty<string>();
        public bool IsRequired { get; set; }
        public int Priority { get; set; }
    }

    // Models/Domain/EditRecord.cs
    public class EditRecord
    {
        [Required]
        public UserEdit Edit { get; set; } = new();

        [Required]
        public DateTime AppliedAt { get; set; }

        [Required]
        public string AppliedBy { get; set; } = string.Empty;

        public EditImpact Impact { get; set; } = new();
        public string EditId { get; set; } = Guid.NewGuid().ToString();
        public int Version { get; set; } = 1;
        public string[] Dependencies { get; set; } = Array.Empty<string>();
        public bool ApprovedByAi { get; set; }
        public string[] ReviewComments { get; set; } = Array.Empty<string>();

        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    // Models/Domain/AiSuggestion.cs
    public class AiSuggestion:ICloneable
    {
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Suggestion { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = "improvement"; // completion, improvement, alternative, refactor

        [Required]
        [Range(0, 1)]
        public double Confidence { get; set; }

        public string Reasoning { get; set; } = string.Empty;
        public bool AutoApply { get; set; }
        public DateTime GeneratedAt { get; set; }

        // Additional properties
        public string Category { get; set; } = "general"; // security, performance, readability, coverage
        public string Priority { get; set; } = "medium"; // low, medium, high
        public string[] AffectedLines { get; set; } = Array.Empty<string>();
        public string[] Prerequisites { get; set; } = Array.Empty<string>();
        public string EstimatedEffort { get; set; } = "low"; // low, medium, high
        public string[] RelatedSuggestions { get; set; } = Array.Empty<string>();

        public Dictionary<string, object> Metadata { get; set; } = new();

        public object Clone()
        {
            return new AiSuggestion
            {
                Id = this.Id,
                Suggestion = this.Suggestion,
                Type = this.Type,
                Confidence = this.Confidence,
                Reasoning = this.Reasoning,
                AutoApply = this.AutoApply,
                GeneratedAt = this.GeneratedAt,
                Category = this.Category,
                Priority = this.Priority,
                AffectedLines = this.AffectedLines.ToArray(),
                Prerequisites = this.Prerequisites.ToArray(),
                EstimatedEffort = this.EstimatedEffort,
                RelatedSuggestions = this.RelatedSuggestions.ToArray(),
                Metadata = new Dictionary<string, object>(this.Metadata)
            };
        }
    }

    // Models/Domain/ClarificationThread.cs
    public class ClarificationThread
    {
        [Required]
        public string ThreadId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public DateTime StartedAt { get; set; }

        public DateTime? LastActivity { get; set; }
        public bool Resolved { get; set; }
        public DateTime? ResolvedAt { get; set; }

        [Required]
        public ClarificationRound[] Rounds { get; set; } = Array.Empty<ClarificationRound>();

        // Thread metadata
        public string Topic { get; set; } = string.Empty;
        public string[] Tags { get; set; } = Array.Empty<string>();
        public string Priority { get; set; } = "normal";
        public string[] Participants { get; set; } = Array.Empty<string>();

        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    // Models/Domain/ClarificationRound.cs
    public class ClarificationRound
    {
        [Required]
        public string RoundId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string HumanQuestion { get; set; } = string.Empty;

        [Required]
        public AiClarification AiResponse { get; set; } = new();

        [Required]
        public DateTime AskedAt { get; set; }

        public string QuestionType { get; set; } = "general"; // technical, business, clarification, suggestion

        // Additional round metadata
        public double QuestionClarityScore { get; set; }
        public double AnswerRelevanceScore { get; set; }
        public bool RequiresFollowUp { get; set; }
        public string[] Keywords { get; set; } = Array.Empty<string>();

        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    // Models/Domain/SessionSummary.cs
    public class SessionSummary
    {
        public string SessionId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public int EditCount { get; set; }
        public int ClarificationCount { get; set; }
        public string FinalDecision { get; set; } = string.Empty;
        public string[] KeyInsights { get; set; } = Array.Empty<string>();
        public LearningPoint[] LearningPoints { get; set; } = Array.Empty<LearningPoint>();
        public Dictionary<string, object> Metrics { get; set; } = new();
    }

    // Models/Domain/ReviewOutcome.cs
    public class ReviewOutcome
    {
        public string Decision { get; set; } = string.Empty; // approved, revised, rejected
        public string DecisionSummary { get; set; } = string.Empty;
        public string[] AppliedEdits { get; set; } = Array.Empty<string>();
        public string[] AcceptedSuggestions { get; set; } = Array.Empty<string>();
        public DateTime DecidedAt { get; set; }
        public string DecidedBy { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    // Models/Domain/LearningPoint.cs
    public class LearningPoint
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public string[] Examples { get; set; } = Array.Empty<string>();
        public bool AppliedToModel { get; set; }
        public DateTime? AppliedAt { get; set; }
    }

    // Models/Domain/CollaborationTools.cs
    public class CollaborationTools
    {
        public bool RealTimeEditing { get; set; }
        public bool Comments { get; set; }
        public bool Suggestions { get; set; }
        public bool Chat { get; set; }
        public bool VersionHistory { get; set; }
        public bool SideBySideDiff { get; set; }
    }

    // Enums
    public enum ReviewSessionStatus
    {
        AwaitingReview,
        InProgress,
        UnderClarification,
        PendingJudgment,
        Closed
    }
}
