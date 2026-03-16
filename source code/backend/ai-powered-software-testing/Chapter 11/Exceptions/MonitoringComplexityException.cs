
// Exceptions/MonitoringComplexityException.cs
namespace Chapter_11.Exceptions
{
    public class MonitoringComplexityException : Exception
    {
        public string[] ComplexityIssues { get; }
        public string[] RecommendedSimplifications { get; }

        public MonitoringComplexityException(
            string message,
            string[] complexityIssues,
            string[] recommendedSimplifications) : base(message)
        {
            ComplexityIssues = complexityIssues;
            RecommendedSimplifications = recommendedSimplifications;
        }
    }
}