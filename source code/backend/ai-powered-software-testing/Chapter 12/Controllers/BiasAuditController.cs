

// Controllers/BiasAuditController.cs
using Chapter_12.Configuration;
using Chapter_12.Exceptions;
using Chapter_12.Interfaces;
using Chapter_12.Models.Errors;
using Chapter_12.Models.Requests;
using Chapter_12.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using InvalidDataException = Chapter_12.Exceptions.InvalidDataException;

namespace Chapter_12.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BiasAuditController : ControllerBase
    {
        private readonly IBiasAuditService _auditService;
        private readonly ILogger<BiasAuditController> _logger;
        private readonly AIServiceConfig _aiConfig;

        public BiasAuditController(
            IBiasAuditService auditService,
            IOptions<AIServiceConfig> aiOptions,
            ILogger<BiasAuditController> logger)
        {
            _auditService = auditService;
            _logger = logger;
            _aiConfig = aiOptions.Value;
        }

        /// <summary>
        /// Performs a bias audit on test data
        /// </summary>
        /// <param name="request">The test data to audit</param>
        /// <returns>Bias audit results with suggestions</returns>
        /// <response code="200">Returns the audit results</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="503">If the AI service is unavailable</response>
        [HttpPost("audit-data")]
        [ProducesResponseType(typeof(BiasAuditResponse), 200)]
        [ProducesResponseType(typeof(BiasAuditErrorResponse), 400)]
        [ProducesResponseType(typeof(BiasAuditErrorResponse), 503)]
        public async Task<IActionResult> AuditTestData([FromBody] TestDataBiasAuditRequest request)
        {
            // Input validation - keeping it simple but effective
            if (request == null)
            {
                return BadRequest(new BiasAuditErrorResponse
                {
                    Message = "Request body is missing. Send us your data to audit.",
                    ErrorType = "MissingRequest",
                    SuggestedRemediation = "Please provide a valid request body.",
                    Timestamp = DateTime.UtcNow,
                    ErrorId = Guid.NewGuid().ToString()
                });
            }

            if (string.IsNullOrWhiteSpace(request.DatasetName))
            {
                return BadRequest(new BiasAuditErrorResponse
                {
                    Message = "We need a name for this dataset to track the audit.",
                    ErrorType = "MissingDatasetName",
                    SuggestedRemediation = "Please provide a DatasetName in your request.",
                    Timestamp = DateTime.UtcNow,
                    ErrorId = Guid.NewGuid().ToString()
                });
            }

            if (request.DataSample == null || !request.DataSample.Any())
            {
                return BadRequest(new BiasAuditErrorResponse
                {
                    Message = "The data sample is empty. Send us a few rows to look at.",
                    DatasetName = request.DatasetName,
                    ErrorType = "EmptyDataSample",
                    SuggestedRemediation = "Please provide at least one row of data in the DataSample array.",
                    Timestamp = DateTime.UtcNow,
                    ErrorId = Guid.NewGuid().ToString()
                });
            }

            if (request.SuggestionCount <= 0 || request.SuggestionCount > 50)
            {
                return BadRequest(new BiasAuditErrorResponse
                {
                    Message = "Please ask for between 1 and 50 suggestions.",
                    DatasetName = request.DatasetName,
                    ErrorType = "InvalidSuggestionCount",
                    SuggestedRemediation = "Set SuggestionCount to a value between 1 and 50.",
                    Timestamp = DateTime.UtcNow,
                    ErrorId = Guid.NewGuid().ToString()
                });
            }

            // Set default values if not provided
            request.DataContext = string.IsNullOrEmpty(request.DataContext) ? "general" : request.DataContext;

            // We'll use the prompt from the frontend, or create a default one
            var promptToUse = string.IsNullOrEmpty(request.AIPrompt)
                ? $"Analyze this {request.DataContext} data for bias. Suggest more inclusive data points. Provide {request.SuggestionCount} suggestions."
                : request.AIPrompt;

            try
            {
                _logger.LogInformation("Starting bias audit for dataset {DatasetName} with {SuggestionCount} suggestions",
                    request.DatasetName, request.SuggestionCount);

                var auditResult = await _auditService.PerformBiasAuditAsync(request, promptToUse);

                _logger.LogInformation("Completed bias audit for dataset {DatasetName} with AuditId {AuditId}",
                    request.DatasetName, auditResult.AuditId);

                return Ok(auditResult);
            }
            catch (AIServiceException ex)
            {
                _logger.LogError(ex, "AI service error during bias audit for dataset {DatasetName}", request.DatasetName);

                return StatusCode(503, new BiasAuditErrorResponse
                {
                    Message = "The AI insight service is having trouble right now.",
                    DatasetName = request.DatasetName,
                    ErrorType = "AIServiceUnavailable",
                    SuggestedRemediation = "Please try again in a few moments. If it persists, contact the AI ops team.",
                    ErrorId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidDataException ex)
            {
                _logger.LogWarning(ex, "Invalid data format for dataset {DatasetName}", request.DatasetName);

                return BadRequest(new BiasAuditErrorResponse
                {
                    Message = "We couldn't make sense of the data format.",
                    DatasetName = request.DatasetName,
                    ErrorType = "DataFormatError",
                    SuggestedRemediation = "Check that your data sample is a valid JSON array of objects.",
                    ErrorId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during bias audit for dataset {DatasetName}", request.DatasetName);

                return StatusCode(500, new BiasAuditErrorResponse
                {
                    Message = "An unexpected error occurred while processing your request.",
                    DatasetName = request.DatasetName,
                    ErrorType = "InternalServerError",
                    SuggestedRemediation = "Please try again later or contact support.",
                    ErrorId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>Health status</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult Health()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "BiasAuditController"
            });
        }
    }
}
