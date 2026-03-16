namespace Chapter_7.Exceptions
{
    
    public class LogParseException : Exception
    {
        public string RawLogSnippet { get; set; }

        public LogParseException(string message, string rawLogSnippet)
            : base(message)
        {
            RawLogSnippet = rawLogSnippet;
        }
    }
}
