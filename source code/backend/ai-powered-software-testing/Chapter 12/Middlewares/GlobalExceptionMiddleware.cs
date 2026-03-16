
// Middleware/GlobalExceptionMiddleware.cs
using Chapter_12.Exceptions;
using Chapter_12.Models.Errors;
using System.Net;
using System.Text.Json;
using InvalidDataException = Chapter_12.Exceptions.InvalidDataException;

namespace Chapter_12.Middlewares
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
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

            var errorResponse = new BiasAuditErrorResponse
            {
                Message = "An error occurred while processing your request.",
                ErrorType = "InternalServerError",
                SuggestedRemediation = "Please try again later or contact support.",
                Timestamp = DateTime.UtcNow,
                ErrorId = Guid.NewGuid().ToString()
            };

            switch (exception)
            {
                case AIServiceException aiEx:
                    response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    errorResponse.Message = aiEx.Message;
                    errorResponse.ErrorType = "AIServiceError";
                    errorResponse.SuggestedRemediation = "The AI service is currently unavailable. Please try again later.";
                    break;

                case InvalidDataException invalidEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = invalidEx.Message;
                    errorResponse.ErrorType = "InvalidData";
                    errorResponse.SuggestedRemediation = "Please check your data format and try again.";
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = "You are not authorized to perform this action.";
                    errorResponse.ErrorType = "Unauthorized";
                    errorResponse.SuggestedRemediation = "Please provide valid credentials.";
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    // Add more details in development
                    if (_env.IsDevelopment())
                    {
                        errorResponse.Message = exception.Message;
                        errorResponse.SuggestedRemediation = exception.StackTrace;
                    }
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }
    }
}
