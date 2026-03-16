
// Middleware/MetricsExceptionMiddleware.cs
using Chapter_10.Exceptions;
using Chapter_10.Models.Errors;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Chapter_10.Middlewares
{
    public class MetricsExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MetricsExceptionMiddleware> _logger;

        public MetricsExceptionMiddleware(RequestDelegate next,
            ILogger<MetricsExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
            _logger.LogError(exception, "An error occurred in MetricsThatMatter API");

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new MetricErrorResponse
            {
                ErrorType = "unexpected-error",
                Message = "An unexpected error occurred. Please try again later.",
                RecoverySteps = new[]
                {
                    "Retry the operation",
                    "Check your input data",
                    "Contact support if issue persists"
                },
                FallbackSuggestion = "Use manual alternative"
            };

            switch (exception)
            {
                case ObjectiveAmbiguityException oaex:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    errorResponse.ErrorType = "objective-ambiguity";
                    errorResponse.Message = exception.Message;
                    errorResponse.DiagnosticData = new MetricDiagnosticData
                    {
                        AmbiguousObjectives = oaex.AmbiguousObjectives,
                        ClarificationQuestions = oaex.ClarificationQuestions
                    };
                    break;

                case BaselineInconsistencyException biex:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    errorResponse.ErrorType = "baseline-inconsistency";
                    errorResponse.Message = exception.Message;
                    errorResponse.DiagnosticData = new MetricDiagnosticData
                    {
                        InconsistencyDetails = biex.InconsistencyDetails,
                        DataQualityIssues = biex.DataQualityIssues
                    };
                    break;

                case PatternDetectionException pdex:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    errorResponse.ErrorType = "pattern-detection-error";
                    errorResponse.Message = exception.Message;
                    errorResponse.DiagnosticData = new MetricDiagnosticData
                    {
                        PatternDetectionChallenges = pdex.DetectionChallenges,
                        DataRequirements = pdex.DataRequirements
                    };
                    break;

                case InsightGenerationException igex:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    errorResponse.ErrorType = "insight-generation-error";
                    errorResponse.Message = exception.Message;
                    errorResponse.DiagnosticData = new MetricDiagnosticData
                    {
                        InsightGenerationChallenges = igex.GenerationChallenges,
                        MetricLimitations = igex.MetricLimitations
                    };
                    break;

                case OptimizationConflictException ocex:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    errorResponse.ErrorType = "optimization-conflict";
                    errorResponse.Message = exception.Message;
                    errorResponse.DiagnosticData = new MetricDiagnosticData
                    {
                        ConflictingGoals = ocex.ConflictingGoals,
                        TradeOffAnalysis = ocex.TradeOffAnalysis
                    };
                    break;

                case ArgumentException:
                case ValidationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorType = "validation-error";
                    errorResponse.Message = exception.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(jsonResponse);
        }
    }

    // Extension method for middleware
    public static class MetricsExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseMetricsExceptionMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MetricsExceptionMiddleware>();
        }
    }
}