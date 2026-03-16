namespace Chapter_1.Exceptions
{
    // Exceptions/ArchitectureAnalysisException.cs
    public class ArchitectureAnalysisException : Exception
    {
        public string ApplicationName { get; }
        public string ArchitectureType { get; }

        public ArchitectureAnalysisException(string message, string applicationName, string architectureType)
            : base(message)
        {
            ApplicationName = applicationName;
            ArchitectureType = architectureType;
        }

        public ArchitectureAnalysisException(string message, string applicationName, string architectureType, Exception innerException)
            : base(message, innerException)
        {
            ApplicationName = applicationName;
            ArchitectureType = architectureType;
        }
    }

    // Exceptions/LLMCoordinationException.cs
    public class LLMCoordinationException : Exception
    {
        public string Provider { get; }
        public string Area { get; }

        public LLMCoordinationException(string message, string provider = null, string area = null)
            : base(message)
        {
            Provider = provider;
            Area = area;
        }
    }
}
