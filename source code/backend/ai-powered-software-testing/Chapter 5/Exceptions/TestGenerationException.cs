using Chapter_5.Models.Requests;

namespace Chapter_5.Exceptions
{
    // Exceptions/TestGenerationException.cs
    public class TestGenerationException : Exception
    {
        public string UserStoryId { get; }
        public string TddStyle { get; }

        public TestGenerationException(string message) : base(message)
        {
        }

        public TestGenerationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public TestGenerationException(string message, string userStoryId, string tddStyle)
            : base(message)
        {
            UserStoryId = userStoryId;
            TddStyle = tddStyle;
        }
    }

    // Exceptions/ImplementationComplexityException.cs
    public class ImplementationComplexityException : Exception
    {
        public int MaxAllowedComplexity { get; }
        public int ActualComplexity { get; }

        public ImplementationComplexityException(string message, int maxAllowed, int actual)
            : base(message)
        {
            MaxAllowedComplexity = maxAllowed;
            ActualComplexity = actual;
        }

        public ImplementationComplexityException(string message, int maxAllowed, int actual, Exception innerException)
            : base(message, innerException)
        {
            MaxAllowedComplexity = maxAllowed;
            ActualComplexity = actual;
        }
    }

    // Exceptions/RefactoringSafetyException.cs
    public class RefactoringSafetyException : Exception
    {
        public string RefactoringStep { get; }
        public string SafetyCheck { get; }

        public RefactoringSafetyException(string message, string step, string safetyCheck)
            : base(message)
        {
            RefactoringStep = step;
            SafetyCheck = safetyCheck;
        }

        public RefactoringSafetyException(string message, string step, string safetyCheck, Exception innerException)
            : base(message, innerException)
        {
            RefactoringStep = step;
            SafetyCheck = safetyCheck;
        }
    }

    // Exceptions/PredictionComplexityException.cs
    public class PredictionComplexityException : Exception
    {
        public TimeHorizon TimeHorizon { get; }
        public double ConfidenceThreshold { get; }

        public PredictionComplexityException(string message, TimeHorizon horizon, double threshold)
            : base(message)
        {
            TimeHorizon = horizon;
            ConfidenceThreshold = threshold;
        }

        public PredictionComplexityException(string message, TimeHorizon horizon, double threshold, Exception innerException)
            : base(message, innerException)
        {
            TimeHorizon = horizon;
            ConfidenceThreshold = threshold;
        }
    }
}
