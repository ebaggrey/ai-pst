
namespace Chapter_8.Exceptions
{
    public class BusinessContextMismatchException : Exception
    {
        public string[] MismatchedFlows { get; set; }
        public string[] SuggestedContexts { get; set; }

        public BusinessContextMismatchException() : base() { }

        public BusinessContextMismatchException(string message) : base(message) { }

        public BusinessContextMismatchException(string message, Exception innerException)
            : base(message, innerException) { }

        public BusinessContextMismatchException(string message, string[] mismatchedFlows, string[] suggestedContexts)
            : base(message)
        {
            MismatchedFlows = mismatchedFlows;
            SuggestedContexts = suggestedContexts;
        }
    }
}
