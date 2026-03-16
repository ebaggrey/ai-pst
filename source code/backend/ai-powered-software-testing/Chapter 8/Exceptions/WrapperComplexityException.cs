
namespace Chapter_8.Exceptions
{
    public class WrapperComplexityException : Exception
    {
        public string ModuleName { get; set; }
        public string[] ComplexityIssues { get; set; }

        public WrapperComplexityException() : base() { }

        public WrapperComplexityException(string message) : base(message) { }

        public WrapperComplexityException(string message, Exception innerException)
            : base(message, innerException) { }

        public WrapperComplexityException(string message, string moduleName, string[] complexityIssues)
            : base(message)
        {
            ModuleName = moduleName;
            ComplexityIssues = complexityIssues;
        }
    }
}