using Chapter_3.Models.Supporting;
using System.ComponentModel.DataAnnotations;

namespace Chapter_3.Models.Requests
{
    // Models/Requests/CollaborativeEditRequest.cs
    public class CollaborativeEditRequest
    {
        public UserEdit UserEdit { get; set; } = new();
        public string EditContext { get; set; } = string.Empty;
        public string[] RelatedIssues { get; set; } = Array.Empty<string>();
        public bool RequestAiAnalysis { get; set; } = true;
    }

   

    // Models/Requests/ClarificationRequest.cs
    public class ClarificationRequest
    {
        [Required]
        [MinLength(10)]
        [MaxLength(1000)]
        public string HumanQuestion { get; set; } = string.Empty;

        public string QuestionType { get; set; } = "general";
        public string[] ContextTags { get; set; } = Array.Empty<string>();
        public string Urgency { get; set; } = "normal"; // low, normal, high
        public Dictionary<string, string> AdditionalContext { get; set; } = new();
    }

    // Models/Requests/JudgmentRequest.cs
    public class JudgmentRequest
    {
        [Required]
        public HumanJudgment Judgment { get; set; } = new();

        public string[] AreasReviewed { get; set; } = Array.Empty<string>();
        public string FeedbackForAi { get; set; } = string.Empty;
        public bool StoreForTraining { get; set; } = true;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    // Models/Requests/HumanJudgment.cs
    public class HumanJudgment
    {
        [Required]
        public string Decision { get; set; } = string.Empty; // approve, request-revision, reject

        [Required]
        [MinLength(10)]
        public string Reasoning { get; set; } = string.Empty;

        public string[] SuggestedImprovements { get; set; } = Array.Empty<string>();
        public string[] AreasOfConcern { get; set; } = Array.Empty<string>();
        public double ConfidenceInJudgment { get; set; } = 1.0;
        public string[] SupportingEvidence { get; set; } = Array.Empty<string>();
        public Dictionary<string, string> SpecificFeedback { get; set; } = new();
    }

    // Models/Requests/ReviewChecklistItem.cs
    public class ReviewChecklistItem
    {
        public string Item { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public string Guidance { get; set; } = string.Empty;
        public string[] Examples { get; set; } = Array.Empty<string>();
    }
}
