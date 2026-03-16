
using Chapter_11.Models.Analysis;
using Chapter_11.Models.Requests;

namespace Chapter_11.Interfaces
{
    public interface IShiftRightOrchestrator
    {
        Task<Models.Responses.Monitor[]> GenerateMonitorsAsync(
            ProductionSystemAnalysis systemAnalysis,
            UserBehaviorAnalysis behaviorAnalysis,
            MonitoringObjective[] objectives);
    }
}
