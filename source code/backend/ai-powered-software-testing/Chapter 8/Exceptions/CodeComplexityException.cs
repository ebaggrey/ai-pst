
namespace Chapter_8.Exceptions
{
    public class CodeComplexityException : Exception
    {
        public int ComplexityScore { get; set; }
        public string SuggestedApproach { get; set; }

        public CodeComplexityException() : base() { }

        public CodeComplexityException(string message) : base(message) { }

        public CodeComplexityException(string message, Exception innerException)
            : base(message, innerException) { }

        public CodeComplexityException(string message, int complexityScore, string suggestedApproach)
            : base(message)
        {
            ComplexityScore = complexityScore;
            SuggestedApproach = suggestedApproach;
        }
    }
}