using Chapter_3.Models.Domain;
using Chapter_3.Models.Requests;
using Chapter_3.Models.Supporting;
using System.ComponentModel.DataAnnotations;

namespace Chapter_3.Models.Responses
{
    // Models/Responses/AiEditAnalysis.cs
    public class AiEditAnalysis
    {
        public string EditId { get; set; } = Guid.NewGuid().ToString();
        public bool AlignsWithIntent { get; set; }
        public string[] Improvements { get; set; } = Array.Empty<string>();
        public string[] PotentialIssues { get; set; } = Array.Empty<string>();
        public string[] AlternativeSuggestions { get; set; } = Array.Empty<string>();
        public EditImpact EditImpact { get; set; } = new();
        public bool ShouldLearnFromEdit { get; set; }
        public DateTime AnalyzedAt { get; set; }
        public double ConfidenceInAnalysis { get; set; }
    }

    // Models/Responses/EditImpact.cs
    public class EditImpact
    {
        [Required]
        public string ImpactLevel { get; set; } = "low"; // low, medium, high, critical

        public string[] AffectedAreas { get; set; } = Array.Empty<string>();
        public string[] RisksIntroduced { get; set; } = Array.Empty<string>();
        public string[] Benefits { get; set; } = Array.Empty<string>();
        public string Summary { get; set; } = string.Empty;

        // Quantitative impact
        public int LinesChanged { get; set; }
        public int DependenciesAffected { get; set; }
        public double ComplexityChange { get; set; } // -1.0 to 1.0

        // Quality impact
        public double TestCoverageImpact { get; set; }
        public double MaintainabilityImpact { get; set; }
        public double PerformanceImpact { get; set; }

        // Business impact
        public string BusinessRiskLevel { get; set; } = "low";
        public string[] BusinessAreasAffected { get; set; } = Array.Empty<string>();

        public Dictionary<string, object> Metrics { get; set; } = new();
    }

    // Models/Responses/CollaborationResponse.cs
    public class CollaborationResponse
    {
        public ReviewSession Session { get; set; } = new();
        public AiEditAnalysis AiPerspective { get; set; } = new();
        public EditImpact ImpactAnalysis { get; set; } = new();
        public AiSuggestion[] SuggestedNextEdits { get; set; } = Array.Empty<AiSuggestion>();
        public LearningOpportunity[] LearningOpportunities { get; set; } = Array.Empty<LearningOpportunity>();
    }

    // Models/Responses/LearningOpportunity.cs
    public class LearningOpportunity
    {
        public string Opportunity { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string WhyImportant { get; set; } = string.Empty;
        public string[] Examples { get; set; } = Array.Empty<string>();
        public bool ShouldPrioritize { get; set; }
    }

    // Models/Responses/CollaborationError.cs
    public class CollaborationError
    {
        public string SessionId { get; set; } = string.Empty;
        public string ConflictType { get; set; } = string.Empty;
        public UserEdit[] ConflictingEdits { get; set; } = Array.Empty<UserEdit>();
        public string[] ResolutionOptions { get; set; } = Array.Empty<string>();
        public string AiMergeSuggestion { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }

    // Models/Responses/QuestionAnalysis.cs
    public class QuestionAnalysis
    {
        public bool IsAmbiguous { get; set; }
        public string[] Interpretations { get; set; } = Array.Empty<string>();
        public string QuestionType { get; set; } = string.Empty;
        public string[] KeyTopics { get; set; } = Array.Empty<string>();
        public string UnderlyingNeed { get; set; } = string.Empty;
        public double ClarityScore { get; set; }
        public string[] MissingContext { get; set; } = Array.Empty<string>();
    }

    // Models/Responses/AiClarification.cs
    //public class AiClarification
    //{
    //    public string DirectAnswer { get; set; } = string.Empty;
    //    public string[] Alternatives { get; set; } = Array.Empty<string>();
    //    public double Confidence { get; set; }
    //    public string[] Assumptions { get; set; } = Array.Empty<string>();
    //    public string RecommendedAction { get; set; } = string.Empty;
    //    public string WhenToAskHuman { get; set; } = string.Empty;
    //    public string[] References { get; set; } = Array.Empty<string>();
    //    public string[] RelatedQuestions { get; set; } = Array.Empty<string>();
    //}

    // Models/Responses/ClarificationResponse.cs
    public class ClarificationResponse
    {
        public string RoundId { get; set; } = Guid.NewGuid().ToString();
        public QuestionAnalysis QuestionAnalysis { get; set; } = new();
        public AiClarification AiAnswer { get; set; } = new();
        public double RelevanceScore { get; set; }
        public string[] SuggestedFollowUps { get; set; } = Array.Empty<string>();
        public string ConfidenceStatement { get; set; } = string.Empty;
        public string WhenToAskHuman { get; set; } = string.Empty;
        public string[] RephrasingSuggestions { get; set; } = Array.Empty<string>();
    }

    // Models/Responses/JudgmentResponse.cs
    public class JudgmentResponse
    {
        public ReviewOutcome Outcome { get; set; } = new();
        public ReviewInsight[] Insights { get; set; } = Array.Empty<ReviewInsight>();
        public string[] NextSteps { get; set; } = Array.Empty<string>();
        public string FeedbackForHuman { get; set; } = string.Empty;
        public ModelUpdateSummary[] ModelUpdatesApplied { get; set; } = Array.Empty<ModelUpdateSummary>();
    }

    // Models/Responses/ReviewInsight.cs
    public class ReviewInsight
    {
        public string Insight { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public string[] Evidence { get; set; } = Array.Empty<string>();
        public bool Actionable { get; set; }
    }

    // Models/Responses/ModelUpdateSummary.cs
    public class ModelUpdateSummary
    {
        public string ModelName { get; set; } = string.Empty;
        public string UpdateType { get; set; } = string.Empty;
        public string[] AreasUpdated { get; set; } = Array.Empty<string>();
        public double ConfidenceImpact { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
