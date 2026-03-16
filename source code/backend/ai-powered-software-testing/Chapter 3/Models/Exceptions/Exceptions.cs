using Chapter_3.Models.Requests;
using Chapter_3.Models.Supporting;

namespace Chapter_3.Models.Exceptions
{
    // Models/Exceptions/ReviewContextException.cs
    public class ReviewContextException : Exception
    {
        public string ErrorCode { get; }
        public string[] MissingContext { get; }
        public string[] EnhancementPrompts { get; }

        public ReviewContextException(string message, string errorCode, string[] missingContext = null, string[] enhancementPrompts = null)
            : base(message)
        {
            ErrorCode = errorCode;
            MissingContext = missingContext ?? Array.Empty<string>();
            EnhancementPrompts = enhancementPrompts ?? Array.Empty<string>();
        }

        public ReviewContextException(string message, Exception innerException, string errorCode, string[] missingContext = null, string[] enhancementPrompts = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            MissingContext = missingContext ?? Array.Empty<string>();
            EnhancementPrompts = enhancementPrompts ?? Array.Empty<string>();
        }
    }

    // Models/Exceptions/EditConflictException.cs
    public class EditConflictException : Exception
    {
        public string SessionId { get; }
        public UserEdit[] ConflictingEdits { get; }
        public string ConflictType { get; }

        public EditConflictException(string sessionId, UserEdit[] conflictingEdits, string conflictType = "edit-overlap")
            : base($"Edit conflict in session {sessionId}")
        {
            SessionId = sessionId;
            ConflictingEdits = conflictingEdits;
            ConflictType = conflictType;
        }
    }

    // Models/Exceptions/AmbiguousQuestionException.cs
    public class AmbiguousQuestionException : Exception
    {
        public string Question { get; }
        public string[] Interpretations { get; }

        public AmbiguousQuestionException(string question, string[] interpretations)
            : base($"Ambiguous question: {question}")
        {
            Question = question;
            Interpretations = interpretations;
        }
    }

    // Models/Exceptions/JudgmentProcessingException.cs
    public class JudgmentProcessingException : Exception
    {
        public string SessionId { get; }
        public string JudgmentId { get; }
        public string ErrorCode { get; }

        public JudgmentProcessingException(string sessionId, string judgmentId, string errorCode, Exception innerException = null)
            : base($"Failed to process judgment {judgmentId} for session {sessionId}", innerException)
        {
            SessionId = sessionId;
            JudgmentId = judgmentId;
            ErrorCode = errorCode;
        }
    }

    // Models/Exceptions/SessionNotFoundException.cs
    public class SessionNotFoundException : Exception
    {
        public string SessionId { get; }

        public SessionNotFoundException(string sessionId)
            : base($"Session {sessionId} not found")
        {
            SessionId = sessionId;
        }
    }

    // Models/Exceptions/InvalidEditException.cs
    public class InvalidEditException : Exception
    {
        public string EditId { get; }
        public string ValidationError { get; }

        public InvalidEditException(string editId, string validationError)
            : base($"Edit {editId} is invalid: {validationError}")
        {
            EditId = editId;
            ValidationError = validationError;
        }
    }
}
