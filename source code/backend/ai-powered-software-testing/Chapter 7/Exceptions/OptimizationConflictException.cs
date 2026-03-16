namespace Chapter_7.Exceptions
{
   
    public class OptimizationConflictException : Exception
    {
        public string[] ConflictingGoals { get; set; }

        public OptimizationConflictException(string message, string[] conflictingGoals)
            : base(message)
        {
            ConflictingGoals = conflictingGoals;
        }
    }
}