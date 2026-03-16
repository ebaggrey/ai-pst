using Chapter_3.Models.Domain;
using Chapter_3.Models.Requests;
using Chapter_3.Models.Responses;
using Chapter_3.Models.Supporting;

namespace Chapter_3.Services.Interfaces
{
    // Services/Interfaces/IHumanReviewOrchestrator.cs
    public interface IHumanReviewOrchestrator
    {
        Task<ReviewSession> CreateReviewSessionAsync(ReviewRequest request);
        Task<AiEditAnalysis> AnalyzeHumanEditAsync(ReviewSession session, CollaborativeEditRequest request);
        Task<AiClarification> GenerateClarificationAsync(ReviewSession session, ClarificationRequest request, QuestionAnalysis questionAnalysis);
        Task<QuestionAnalysis> AnalyzeHumanQuestionAsync(ClarificationRequest request);
        Task LearnFromJudgmentAsync(JudgmentRequest request);
        //Task<AiClarification> GenerateClarificationAsync(ReviewSession session, ClarificationRequest request, QuestionAnalysis questionAnalysis);
    }

    // Services/Interfaces/ICollaborationSessionManager.cs
    public interface ICollaborationSessionManager
    {
        Task<ReviewSession> GetSessionAsync(string sessionId);
        Task<ReviewSession> ApplyHumanEditAsync(string sessionId, CollaborativeEditRequest request);
        Task<ClarificationThread> AddClarificationRoundAsync(string sessionId, ClarificationRequest request, ClarificationResponse response);
        Task CloseSessionAsync(string sessionId, ReviewOutcome outcome);
    }

    // Services/Interfaces/IJudgmentProcessor.cs
    public interface IJudgmentProcessor
    {
        Task<ReviewOutcome> ProcessJudgmentAsync(ReviewSession session, JudgmentRequest request);
        Task<ReviewInsight[]> ExtractInsightsAsync(ReviewSession session, JudgmentRequest request, ReviewOutcome outcome);
    }

    // Services/Interfaces/IJudgmentAnalyzer.cs
    public interface IJudgmentAnalyzer
    {
        Task<LearningPoint[]> ExtractLearningPointsAsync(JudgmentRequest request);
        Task<ModelUpdateSummary[]> AnalyzeForModelUpdatesAsync(JudgmentRequest request);
    }

    // Services/Interfaces/IReviewSessionStore.cs
    public interface IReviewSessionStore
    {
        Task<ReviewSession> GetSessionAsync(string sessionId);
        Task StoreSessionAsync(ReviewSession session);
        Task UpdateSessionAsync(ReviewSession session);
        Task DeleteSessionAsync(string sessionId);
        Task<IEnumerable<ReviewSession>> GetSessionsByStatusAsync(ReviewSessionStatus status);
    }

    // Services/Interfaces/ILLMServiceFactory.cs
    public interface ILLMServiceFactory
    {
        ILLMService GetService(string serviceName);
        ILLMService GetServiceForTask(string task, string strategy, string context);
    }

    // Services/Interfaces/ILLMService.cs
    public interface ILLMService
    {
        Task<string> GenerateTestCodeAsync(string prompt, string context);
        Task<string> AnalyzeContentAsync(string content, string analysisType);
        Task<string> AnswerQuestionAsync(string question, string context);
    }

    // Services/Interfaces/ICollaborationTools.cs
    public interface ICollaborationTools
    {
        Task<string> CreateWorkspaceAsync(ReviewSession session);
        Task<DiffResult> CalculateDiffAsync(string original, string modified);
        Task<SuggestionResult[]> GenerateSuggestionsAsync(ReviewSession session, string context);
    }

    // Services/Interfaces/IDiffService.cs
    public interface IDiffService
    {
        Task<DiffResult> CalculateDiffAsync(string original, string modified, DiffOptions options);
        Task<MergeResult> MergeChangesAsync(string baseContent, string[] variations);
    }

    // Services/Interfaces/ISuggestionEngine.cs
    public interface ISuggestionEngine
    {
        Task<SuggestionResult[]> GenerateSuggestionsAsync(ReviewSession session, string context);
        Task<EditValidationResult> ValidateEditAsync(GeneratedTest currentContent, UserEdit edit);
    }

    // Services/Interfaces/ICollaborationHub.cs
    public interface ICollaborationHub
    {
        Task JoinSession(string sessionId);
        Task LeaveSession(string sessionId);
        Task SendEdit(string sessionId, UserEdit edit);
        Task SendMessage(string sessionId, string message);
    }

    // Additional helper classes
    public class DiffResult
    {
        public string Original { get; set; } = string.Empty;
        public string Modified { get; set; } = string.Empty;
        public DiffChange[] Changes { get; set; } = Array.Empty<DiffChange>();
        public string Summary { get; set; } = string.Empty;
    }

    public class DiffChange
    {
        public string Type { get; set; } = string.Empty; // add, remove, modify
        public int LineNumber { get; set; }
        public string Content { get; set; } = string.Empty;
        public string OldContent { get; set; } = string.Empty;
    }

    public class MergeResult
    {
        public string MergedContent { get; set; } = string.Empty;
        public MergeConflict[] Conflicts { get; set; } = Array.Empty<MergeConflict>();
        public bool HasConflicts => Conflicts.Any();
    }

    public class MergeConflict
    {
        public int LineNumber { get; set; }
        public string[] Options { get; set; } = Array.Empty<string>();
        public string Suggestion { get; set; } = string.Empty;
    }

    public class SuggestionResult
    {
        public string Suggestion { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string Reasoning { get; set; } = string.Empty;
        public string[] AffectedAreas { get; set; } = Array.Empty<string>();
    }

    public class EditValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public EditImpact Impact { get; set; } = new();

        public static EditValidationResult Valid(EditImpact impact) => new() { IsValid = true, Impact = impact };
        public static EditValidationResult Invalid(string errorMessage) => new() { IsValid = false, ErrorMessage = errorMessage };
    }

    public class DiffOptions
    {
        public bool WordLevel { get; set; }
        public bool IgnoreWhitespace { get; set; }
        public bool ContextLines { get; set; } = true;
        public int ContextSize { get; set; } = 3;
    }
}
