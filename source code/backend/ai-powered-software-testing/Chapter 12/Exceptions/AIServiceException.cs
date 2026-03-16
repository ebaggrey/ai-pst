

// Exceptions/AIServiceException.cs
namespace Chapter_12.Exceptions
{
    public class AIServiceException : Exception
    {
        public string ServiceName { get; set; }
        public int StatusCode { get; set; }

        public AIServiceException() : base() { }

        public AIServiceException(string message) : base(message) { }

        public AIServiceException(string message, Exception innerException)
            : base(message, innerException) { }

        public AIServiceException(string message, string serviceName, int statusCode)
            : base(message)
        {
            ServiceName = serviceName;
            StatusCode = statusCode;
        }
    }

    public class InvalidDataException : Exception
    {
        public string FieldName { get; set; }
        public string ExpectedFormat { get; set; }

        public InvalidDataException() : base() { }

        public InvalidDataException(string message) : base(message) { }

        public InvalidDataException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidDataException(string message, string fieldName, string expectedFormat)
            : base(message)
        {
            FieldName = fieldName;
            ExpectedFormat = expectedFormat;
        }
    }
}