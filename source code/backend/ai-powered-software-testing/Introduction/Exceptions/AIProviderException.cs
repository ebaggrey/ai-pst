namespace Introduction.Exceptions
{
    public class AIProviderException : Exception
    {
        public AIProviderException(string message) : base(message) { }

        public AIProviderException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
