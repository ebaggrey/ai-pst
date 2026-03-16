
// Interfaces/IBiasAuditService.cs

using Chapter_12.Models.Database;
using Chapter_12.Models.Requests;
using Chapter_12.Models.Responses;

namespace Chapter_12.Interfaces
{
    public interface IBiasAuditService
    {
        Task<BiasAuditResponse> PerformBiasAuditAsync(TestDataBiasAuditRequest request, string prompt);
    }
}

// Interfaces/ILLMService.cs
namespace Chapter_12.Interfaces
{
    public interface ILLMService
    {
        Task<string> GenerateCompletionAsync(string prompt, int maxTokens, Dictionary<string, object> parameters = null);
        Task<bool> HealthCheckAsync();
    }
}

// Interfaces/IAuditRepository.cs


namespace Chapter_12.Interfaces
{
    public interface IAuditRepository
    {
        Task<AuditRecord> SaveAuditRecordAsync(BiasAuditResponse auditResponse, string status);
        Task<List<AuditRecord>> GetAuditRecordsByDatasetAsync(string datasetName, DateTime? fromDate = null);
        Task<AuditRecord> GetAuditRecordByIdAsync(string auditId);
    }
}

// Interfaces/IBiasOrchestrator.cs


namespace Chapter_12.Interfaces
{
    public interface IBiasOrchestrator
    {
        Task<BiasAuditResponse> AuditDataAsync(TestDataBiasAuditRequest request, string prompt);
    }
}