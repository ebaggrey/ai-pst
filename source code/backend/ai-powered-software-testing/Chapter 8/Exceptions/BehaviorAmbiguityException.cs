
namespace Chapter_8.Exceptions
{
    public class BehaviorAmbiguityException : Exception
    {
        public string[] AmbiguityAreas { get; set; }
        public string[] ClarificationQuestions { get; set; }

        public BehaviorAmbiguityException() : base() { }

        public BehaviorAmbiguityException(string message) : base(message) { }

        public BehaviorAmbiguityException(string message, Exception innerException)
            : base(message, innerException) { }

        public BehaviorAmbiguityException(string message, string[] ambiguityAreas, string[] clarificationQuestions)
            : base(message)
        {
            AmbiguityAreas = ambiguityAreas;
            ClarificationQuestions = clarificationQuestions;
        }
    }
}
