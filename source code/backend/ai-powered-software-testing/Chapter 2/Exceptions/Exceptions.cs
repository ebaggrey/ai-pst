namespace Chapter_2.Exceptions
{
    public class RepositoryAccessException : Exception
    {
        public string RepositoryUrl { get; }
        public string? ErrorCode { get; }

        public RepositoryAccessException(string repositoryUrl, string message, string? errorCode = null, Exception? innerException = null)
            : base(message, innerException)
        {
            RepositoryUrl = repositoryUrl;
            ErrorCode = errorCode;
        }
    }

    public class AnalysisTimeoutException : Exception
    {
        public TimeSpan Timeout { get; }

        public AnalysisTimeoutException(TimeSpan timeout, string? message = null, Exception? innerException = null)
            : base(message ?? $"Analysis timed out after {timeout.TotalSeconds} seconds", innerException)
        {
            Timeout = timeout;
        }
    }

    public class TestAnalysisException : Exception
    {
        public string TestPath { get; }
        public string? AnalysisPhase { get; }

        public TestAnalysisException(string testPath, string message, string? analysisPhase = null, Exception? innerException = null)
            : base(message, innerException)
        {
            TestPath = testPath;
            AnalysisPhase = analysisPhase;
        }
    }

    public class LLMServiceException : Exception
    {
        public string ProviderName { get; }
        public string? RequestId { get; }

        public LLMServiceException(string providerName, string message, string? requestId = null, Exception? innerException = null)
            : base(message, innerException)
        {
            ProviderName = providerName;
            RequestId = requestId;
        }
    }
}
