
// Interfaces/ITestabilityAnalyzer.cs
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;

namespace Chapter_11.Interfaces
{
    public interface ITestabilityAnalyzer
    {
        Task<TestabilityAnalysis> AnalyzeTestabilityAsync(
            Codebase codebase,
            TestabilityFramework framework,
            AnalysisDepth depth);
    }
}