namespace Chapter_4.Exceptions
{
    // Exceptions/AIOverloadException.cs
    public class AIOverloadException : Exception
    {
        public string Provider { get; }
        public string Service { get; }
        public DateTime Timestamp { get; }

        public AIOverloadException(string provider, string service)
            : base($"AI service overloaded: {provider}/{service}")
        {
            Provider = provider;
            Service = service;
            Timestamp = DateTime.UtcNow;
        }

        public AIOverloadException(string provider, string service, Exception innerException)
            : base($"AI service overloaded: {provider}/{service}", innerException)
        {
            Provider = provider;
            Service = service;
            Timestamp = DateTime.UtcNow;
        }
    }

    // Exceptions/InconsistentAIResponseException.cs
    public class InconsistentAIResponseException : Exception
    {
        public string Prompt { get; }
        public string[] Responses { get; }
        public decimal Variance { get; }

        public InconsistentAIResponseException(string prompt, string[] responses, decimal variance)
            : base($"Inconsistent AI responses for prompt: {prompt}")
        {
            Prompt = prompt;
            Responses = responses;
            Variance = variance;
        }
    }

    // Exceptions/PromptInjectionException.cs
    public class PromptInjectionException : Exception
    {
        public string MaliciousPattern { get; }
        public string DetectionMethod { get; }

        public PromptInjectionException(string maliciousPattern, string detectionMethod)
            : base($"Prompt injection detected: {maliciousPattern}")
        {
            MaliciousPattern = maliciousPattern;
            DetectionMethod = detectionMethod;
        }
    }

    // Exceptions/BiasDetectionComplexityException.cs
    public class BiasDetectionComplexityException : Exception
    {
        public int ContextCount { get; }
        public int MethodCount { get; }

        public BiasDetectionComplexityException(string message)
            : base(message)
        {
        }

        public BiasDetectionComplexityException(string message, int contextCount, int methodCount)
            : base(message)
        {
            ContextCount = contextCount;
            MethodCount = methodCount;
        }
    }

    // Exceptions/FactVerificationException.cs
    public class FactVerificationException : Exception
    {
        public string Fact { get; }
        public string Source { get; }

        public FactVerificationException(string fact, string source, string message)
            : base($"Failed to verify fact '{fact}' against source '{source}': {message}")
        {
            Fact = fact;
            Source = source;
        }

        public FactVerificationException(string message)
            : base(message)
        {
        }
    }

    // Exceptions/InsufficientDataException.cs
    public class InsufficientDataException : Exception
    {
        public int RequiredDataPoints { get; }
        public int AvailableDataPoints { get; }

        public InsufficientDataException(string message)
            : base(message)
        {
        }

        public InsufficientDataException(int required, int available)
            : base($"Insufficient data: required {required}, available {available}")
        {
            RequiredDataPoints = required;
            AvailableDataPoints = available;
        }
    }
}
