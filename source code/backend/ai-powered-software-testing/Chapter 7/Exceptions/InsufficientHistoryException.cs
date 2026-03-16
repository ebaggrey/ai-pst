namespace Chapter_7.Exceptions
{
    
    public class InsufficientHistoryException : Exception
    {
        public int AvailableRuns { get; set; }
        public int RequiredRuns { get; set; }

        public InsufficientHistoryException(string message, int availableRuns, int requiredRuns)
            : base(message)
        {
            AvailableRuns = availableRuns;
            RequiredRuns = requiredRuns;
        }
    }
}
