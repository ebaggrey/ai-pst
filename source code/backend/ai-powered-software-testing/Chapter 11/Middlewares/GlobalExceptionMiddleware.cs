
using Chapter_11.Exceptions;
using FullSpectrumApp.Models.Error;
using System.Net;
using System.Text.Json;

namespace Chapter_11.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
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
            _logger.LogError(exception, "An unhandled exception occurred");

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new SpectrumErrorResponse
            {
                Context = GetContextFromPath(context.Request.Path),
                ErrorType = GetErrorType(exception),
                SpectrumLocation = GetSpectrumLocationFromPath(context.Request.Path),
                Message = GetUserFriendlyMessage(exception),
                RecoverySteps = GetRecoverySteps(exception),
                FallbackSuggestion = GetFallbackSuggestion(exception)
            };

            // Add diagnostic data in development
            if (_env.IsDevelopment())
            {
                errorResponse.DiagnosticData = new SpectrumDiagnosticData
                {
                    ClarificationQuestions = new[] { exception.StackTrace ?? "No stack trace available" }
                };
            }

            response.StatusCode = exception switch
            {
                RequirementAmbiguityException => (int)HttpStatusCode.UnprocessableEntity,
                TestabilityFrameworkException => (int)HttpStatusCode.UnprocessableEntity,
                MonitoringComplexityException => (int)HttpStatusCode.UnprocessableEntity,
                PipelineConflictException => (int)HttpStatusCode.UnprocessableEntity,
                OrchestrationComplexityException => (int)HttpStatusCode.UnprocessableEntity,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _env.IsDevelopment()
            };

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
        }

        private string GetContextFromPath(string path)
        {
            return path.ToString().ToLower() switch
            {
                var p when p.Contains("shift-left") => "shift-left",
                var p when p.Contains("analyze-testability") => "testability-analysis",
                var p when p.Contains("shift-right") => "shift-right",
                var p when p.Contains("create-pipeline") => "pipeline-creation",
                var p when p.Contains("orchestrate-testing") => "test-orchestration",
                _ => "unknown"
            };
        }

        private string GetSpectrumLocationFromPath(string path)
        {
            return path.ToString().ToLower() switch
            {
                var p when p.Contains("shift-left") => "far-left",
                var p when p.Contains("analyze-testability") => "left",
                var p when p.Contains("shift-right") => "far-right",
                var p when p.Contains("create-pipeline") => "center",
                var p when p.Contains("orchestrate-testing") => "full-spectrum",
                _ => "unknown"
            };
        }

        private string GetErrorType(Exception exception)
        {
            return exception switch
            {
                RequirementAmbiguityException => "requirement-ambiguity",
                TestabilityFrameworkException => "framework-incompatibility",
                MonitoringComplexityException => "monitoring-complexity",
                PipelineConflictException => "pipeline-conflict",
                OrchestrationComplexityException => "orchestration-complexity",
                ArgumentException => "invalid-input",
                KeyNotFoundException => "not-found",
                UnauthorizedAccessException => "unauthorized",
                TimeoutException => "timeout",
                _ => "internal-error"
            };
        }

        private string GetUserFriendlyMessage(Exception exception)
        {
            return exception switch
            {
                RequirementAmbiguityException => "The requirements are too ambiguous for automated processing.",
                TestabilityFrameworkException => "The selected testability framework is not compatible with your codebase.",
                MonitoringComplexityException => "The production system is too complex for automated monitoring setup.",
                PipelineConflictException => "Conflicts detected in the pipeline configuration.",
                OrchestrationComplexityException => "The test suite is too complex for automated orchestration.",
                ArgumentException => "Invalid input provided. Please check your request.",
                KeyNotFoundException => "The requested resource was not found.",
                UnauthorizedAccessException => "You are not authorized to perform this action.",
                TimeoutException => "The operation timed out. Please try again.",
                _ => "An unexpected error occurred. Please try again later."
            };
        }

        private string[] GetRecoverySteps(Exception exception)
        {
            return exception switch
            {
                RequirementAmbiguityException raex => raex.ClarificationQuestions ?? new[]
                {
                    "Review and clarify the requirements",
                    "Add more specific details",
                    "Break down complex requirements"
                },
                TestabilityFrameworkException tfex => tfex.FrameworkIssues ?? new[]
                {
                    "Choose a different testability framework",
                    "Update your codebase to match framework requirements",
                    "Use a technology-specific analysis tool"
                },
                MonitoringComplexityException mcex => mcex.RecommendedSimplifications ?? new[]
                {
                    "Start with monitoring critical components only",
                    "Simplify monitoring objectives",
                    "Implement monitoring in phases"
                },
                PipelineConflictException pcex => pcex.ConflictingStages?.Select(s => $"Resolve conflict in stage: {s}").ToArray() ?? new[]
                {
                    "Review stage dependencies",
                    "Re-order development stages",
                    "Remove circular dependencies"
                },
                OrchestrationComplexityException ocex => ocex.SimplificationSuggestions ?? new[]
                {
                    "Break the test suite into smaller batches",
                    "Use a simpler orchestration strategy",
                    "Remove redundant tests"
                },
                _ => new[]
                {
                    "Check your input and try again",
                    "Contact support if the issue persists",
                    "Review the API documentation"
                }
            };
        }

        private string GetFallbackSuggestion(Exception exception)
        {
            return exception switch
            {
                RequirementAmbiguityException => "Manual requirement refinement with stakeholders",
                TestabilityFrameworkException => "Manual code review with testability checklist",
                MonitoringComplexityException => "Manual monitoring design with expert consultation",
                PipelineConflictException => "Manual pipeline design with dependency analysis",
                OrchestrationComplexityException => "Manual test execution with phased approach",
                _ => "Please try again with simplified input"
            };
        }
    }
}