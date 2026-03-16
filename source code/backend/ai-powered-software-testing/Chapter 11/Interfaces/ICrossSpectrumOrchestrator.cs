
// Interfaces/ICrossSpectrumOrchestrator.cs
using Chapter_11.Models.Analysis;
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;
using ExecutionContext = Chapter_11.Models.Requests.ExecutionContext;


namespace Chapter_11.Interfaces
{
    public interface ICrossSpectrumOrchestrator
    {
        Task<Orchestration> OrchestrateTestingAsync(
            TestSuiteAnalysis suiteAnalysis,
            OrchestrationStrategy strategy,
            ExecutionContext context);
    }
}