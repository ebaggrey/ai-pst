
// Exceptions/PipelineConflictException.cs
namespace Chapter_11.Exceptions
{
    public class PipelineConflictException : Exception
    {
        public string[] ConflictingStages { get; }
        public string[] DependencyIssues { get; }

        public PipelineConflictException(
            string message,
            string[] conflictingStages,
            string[] dependencyIssues) : base(message)
        {
            ConflictingStages = conflictingStages;
            DependencyIssues = dependencyIssues;
        }
    }
}