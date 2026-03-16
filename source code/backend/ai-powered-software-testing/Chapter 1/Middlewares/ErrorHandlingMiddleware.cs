using System.Net;
using System.Text.Json;
using Chapter_1.Exceptions;
using Chapter_1.Models;

namespace Chapter_1.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var requestId = context.Items["RequestId"]?.ToString() ?? Guid.NewGuid().ToString();
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new LandscapeError
            {
                Timestamp = DateTime.UtcNow,
                CorrelationId = requestId,
                Context = new Dictionary<string, object>()
            };

            // Add request details to context for debugging
            if (_environment.IsDevelopment())
            {
                errorResponse.Context["path"] = context.Request.Path;
                errorResponse.Context["method"] = context.Request.Method;
                errorResponse.Context["query"] = context.Request.QueryString.ToString();
            }

            switch (exception)
            {
                case ArchitectureAnalysisException aex:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    errorResponse.ErrorCode = "ARCHITECTURE_UNPROCESSABLE";
                    errorResponse.Message = aex.Message;
                    errorResponse.RecoverySteps = new[]
                    {
                        "Simplify the architecture description",
                        "Focus on one component at a time",
                        "Provide more details about integration points"
                    };
                    errorResponse.FallbackSuggestion = "Start with smoke tests for critical paths";
                    errorResponse.Severity = "error";

                    _logger.LogWarning(aex, "Architecture analysis failed for {AppName}", aex.ApplicationName);
                    break;

                case LLMCoordinationException lex:
                    response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    errorResponse.ErrorCode = "LLM_COORDINATION_FAILED";
                    errorResponse.Message = lex.Message;
                    errorResponse.RecoverySteps = new[]
                    {
                        "Try again in a few minutes",
                        "Check if LLM services are operational",
                        "Use manual analysis mode as fallback"
                    };
                    errorResponse.FallbackSuggestion = "Focus on smoke tests for critical paths only";
                    errorResponse.Severity = "warning";

                    if (!string.IsNullOrEmpty(lex.Provider))
                    {
                        errorResponse.Context["provider"] = lex.Provider;
                    }
                    if (!string.IsNullOrEmpty(lex.Area))
                    {
                        errorResponse.Context["area"] = lex.Area;
                    }

                    _logger.LogError(lex, "LLM coordination failed for provider {Provider}", lex.Provider);
                    break;

                case ValidationException vex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorCode = "VALIDATION_ERROR";
                    errorResponse.Message = vex.Message;
                    errorResponse.RecoverySteps = new[]
                    {
                        "Review the validation errors",
                        "Correct the invalid fields",
                        "Ensure all required data is provided"
                    };
                    errorResponse.Severity = "error";

                    if (vex.Data.Contains("Errors"))
                    {
                        errorResponse.Context["validationErrors"] = vex.Data["Errors"];
                    }

                    _logger.LogWarning(vex, "Validation error: {Message}", vex.Message);
                    break;

                case UnauthorizedAccessException uex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.ErrorCode = "UNAUTHORIZED";
                    errorResponse.Message = "You are not authorized to perform this action";
                    errorResponse.RecoverySteps = new[]
                    {
                        "Check your authentication credentials",
                        "Ensure you have the required permissions",
                        "Contact your administrator if access is needed"
                    };
                    errorResponse.Severity = "warning";

                    _logger.LogWarning(uex, "Unauthorized access attempt");
                    break;

                case TimeoutException tex:
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    errorResponse.ErrorCode = "REQUEST_TIMEOUT";
                    errorResponse.Message = "The request timed out while processing";
                    errorResponse.RecoverySteps = new[]
                    {
                        "Try again with a smaller request",
                        "Check if the service is under heavy load",
                        "Consider simplifying the analysis request"
                    };
                    errorResponse.Severity = "warning";

                    _logger.LogWarning(tex, "Request timeout");
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.ErrorCode = "INTERNAL_SERVER_ERROR";
                    errorResponse.Message = _environment.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred while processing your request";
                    errorResponse.RecoverySteps = new[]
                    {
                        "Try again later",
                        "Contact support if the problem persists"
                    };
                    errorResponse.Severity = "error";

                    // Add stack trace in development
                    if (_environment.IsDevelopment())
                    {
                        errorResponse.Context["stackTrace"] = exception.StackTrace;
                    }

                    _logger.LogCritical(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }

            // Add common context
            errorResponse.Context["requestId"] = requestId;
            errorResponse.Context["timestamp"] = DateTime.UtcNow.ToString("o");

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            });

            await response.WriteAsync(jsonResponse);
        }
    }

    // Custom validation exception
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }

        public ValidationException(string message, IDictionary<string, string[]> errors) : base(message)
        {
            Data["Errors"] = errors;
        }
    }
}