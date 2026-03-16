
using Chapter2_Ext.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Chapter2_Ext.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PatternestablishmentController : ControllerBase
    {
        private readonly IPatternService _patternService;
        private readonly ILogger<PatternestablishmentController> _logger;

        public PatternestablishmentController(
            IPatternService patternService,
            ILogger<PatternestablishmentController> logger)
        {
            _patternService = patternService;
            _logger = logger;
        }

        /// <summary>
        /// Establish a new testing pattern
        /// </summary>
        [HttpPost("establish")]
        [ProducesResponseType(typeof(TestingPatternDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EstablishPattern([FromBody] PatternEstablishmentRequest request)
        {
            try
            {
                _logger.LogInformation("Received pattern establishment request for area: {Area}",
                    request.Area);

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        ErrorCode = "VALIDATION_ERROR",
                        Message = "Invalid request data",
                        Details = string.Join("; ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)),
                        CorrelationId = Guid.NewGuid().ToString()
                    });
                }

                var pattern = await _patternService.EstablishPatternAsync(request);

                return CreatedAtAction(
                    nameof(GetPattern),
                    new { id = pattern.Id },
                    pattern);
            }
            catch (CustomExceptions.PatternValidationException ex)
            {
                _logger.LogWarning(ex, "Pattern validation failed");
                return BadRequest(CreateErrorResponse(ex));
            }
            catch (CustomExceptions.PatternEstablishmentException ex)
            {
                _logger.LogError(ex, "Pattern establishment failed");
                return StatusCode(500, CreateErrorResponse(ex));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error establishing pattern");
                return StatusCode(500, new ApiErrorResponse
                {
                    ErrorCode = "INTERNAL_ERROR",
                    Message = "An unexpected error occurred",
                    Details = ex.Message,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Generate training materials for a pattern
        /// </summary>
        [HttpPost("training/generate")]
        [Route("training/generate")]
        [ProducesResponseType(typeof(TrainingMaterials), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateTraining([FromBody] TrainingGenerationRequest request)
        {
            try
            {
                _logger.LogInformation("Generating training materials for audience: {Audience}",
                    request.Audience);

                var materials = await _patternService.GenerateTrainingMaterialsAsync(request);
                return Ok(materials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating training materials");
                return StatusCode(500, new ApiErrorResponse
                {
                    ErrorCode = "TRAINING_GENERATION_FAILED",
                    Message = "Failed to generate training materials",
                    Details = ex.Message,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Create automation pipeline for a pattern
        /// </summary>
        [HttpPost("pipelines/create")]
        [Route("pipelines/create")]
        [ProducesResponseType(typeof(PipelineBlueprint), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePipeline([FromBody] PipelineRequest request)
        {
            try
            {
                _logger.LogInformation("Creating pipeline for pattern: {PatternId}",
                    request.PatternId);

                var pipeline = await _patternService.CreateAutomationPipelineAsync(request);
                return Ok(pipeline);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorCode = "PATTERN_NOT_FOUND",
                    Message = ex.Message,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pipeline");
                return StatusCode(500, new ApiErrorResponse
                {
                    ErrorCode = "PIPELINE_CREATION_FAILED",
                    Message = "Failed to create automation pipeline",
                    Details = ex.Message,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Get pattern by ID
        /// </summary>
        [HttpGet("patterns/{id}")]
        [ProducesResponseType(typeof(TestingPatternDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPattern([Required] string id)
        {
            var pattern = await _patternService.GetPatternByIdAsync(id);
            if (pattern == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    ErrorCode = "PATTERN_NOT_FOUND",
                    Message = $"Pattern with ID {id} not found",
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }

            return Ok(pattern);
        }

        /// <summary>
        /// Get patterns by area
        /// </summary>
        [HttpGet("patterns/area/{area}")]
        [ProducesResponseType(typeof(List<TestingPatternDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatternsByArea([Required] string area)
        {
            var patterns = await _patternService.GetPatternsByAreaAsync(area);
            return Ok(patterns);
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "PatternEstablishmentAPI"
            });
        }

        private ApiErrorResponse CreateErrorResponse(CustomExceptions.PatternEstablishmentException ex)
        {
            return new ApiErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                Message = ex.Message,
                PatternError = ex.PatternError,
                CorrelationId = Guid.NewGuid().ToString()
            };
        }
    }
}