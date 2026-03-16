
// Exceptions/TestabilityFrameworkException.cs
namespace Chapter_11.Exceptions
{
    public class TestabilityFrameworkException : Exception
    {
        public string[] FrameworkIssues { get; }
        public string[] TechnologyMismatches { get; }

        public TestabilityFrameworkException(
            string message,
            string[] frameworkIssues,
            string[] technologyMismatches) : base(message)
        {
            FrameworkIssues = frameworkIssues;
            TechnologyMismatches = technologyMismatches;
        }
    }
}