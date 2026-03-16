

// Exceptions/RequirementAmbiguityException.cs
namespace Chapter_11.Exceptions
{
    public class RequirementAmbiguityException : Exception
    {
        public string[] AmbiguousRequirements { get; }
        public string[] ClarificationQuestions { get; }

        public RequirementAmbiguityException(
            string message,
            string[] ambiguousRequirements,
            string[] clarificationQuestions) : base(message)
        {
            AmbiguousRequirements = ambiguousRequirements;
            ClarificationQuestions = clarificationQuestions;
        }
    }
}