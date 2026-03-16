
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;

namespace Chapter_8.Orchestrators
{
    public interface IHealthMonitoringOrchestrator
    {
        Task<HealthResponse> MonitorHealthAsync(HealthRequest request);
    }
}