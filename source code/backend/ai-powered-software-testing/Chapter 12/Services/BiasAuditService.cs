
// Services/BiasAuditService.cs
using Chapter_12.Interfaces;
using Chapter_12.Models.Requests;
using Chapter_12.Models.Responses;

namespace Chapter_12.Services
{
    public class BiasAuditService : IBiasAuditService
    {
        private readonly IBiasOrchestrator _orchestrator;
        private readonly ILogger<BiasAuditService> _logger;

        public BiasAuditService(
            IBiasOrchestrator orchestrator,
            ILogger<BiasAuditService> logger)
        {
            _orchestrator = orchestrator;
            _logger = logger;
        }

        public async Task<BiasAuditResponse> PerformBiasAuditAsync(
            TestDataBiasAuditRequest request,
            string prompt)
        {
            _logger.LogInformation("Starting bias audit for dataset {DatasetName}",
                request.DatasetName);

            try
            {
                var result = await _orchestrator.AuditDataAsync(request, prompt);

                _logger.LogInformation("Completed bias audit for dataset {DatasetName}, AuditId: {AuditId}",
                    request.DatasetName, result.AuditId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete bias audit for dataset {DatasetName}",
                    request.DatasetName);
                throw;
            }
        }
    }
}
