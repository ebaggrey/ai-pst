
// Exceptions/OrchestrationComplexityException.cs
namespace Chapter_11.Exceptions
{
    public class OrchestrationComplexityException : Exception
    {
        public string[] ComplexityFactors { get; }
        public string[] SimplificationSuggestions { get; }

        public OrchestrationComplexityException(
            string message,
            string[] complexityFactors,
            string[] simplificationSuggestions) : base(message)
        {
            ComplexityFactors = complexityFactors;
            SimplificationSuggestions = simplificationSuggestions;
        }
    }
}