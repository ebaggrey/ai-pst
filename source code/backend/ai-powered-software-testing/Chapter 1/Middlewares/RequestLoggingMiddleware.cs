

using System.Diagnostics;
using System.Text;

namespace Chapter_1.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            // Add request ID to context for tracking
            context.Items["RequestId"] = requestId;
            context.Response.Headers.Add("X-Request-Id", requestId);

            try
            {
                // Log request details
                await LogRequest(context, requestId);

                // Capture response body
                var originalBodyStream = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // Call the next middleware
                await _next(context);

                // Log response details
                await LogResponse(context, requestId, stopwatch.ElapsedMilliseconds, responseBody);

                // Copy the response body back to the original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request {RequestId} failed", requestId);
                throw;
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private async Task LogRequest(HttpContext context, string requestId)
        {
            var request = context.Request;

            // Enable request body buffering to allow multiple reads
            request.EnableBuffering();

            // Read request body
            string requestBody = string.Empty;
            if (request.ContentLength > 0 && request.Body.CanRead)
            {
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin); // Reset position for next middleware
            }

            // Truncate request body if too long
            if (requestBody.Length > 1000)
            {
                requestBody = requestBody.Substring(0, 1000) + "... [truncated]";
            }

            var headers = request.Headers
                .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                .Select(h => $"{h.Key}: {h.Value}");

            _logger.LogInformation(
                "Request {RequestId}: {Method} {Path} {QueryString} - Headers: [{Headers}] - Body: {Body}",
                requestId,
                request.Method,
                request.Path,
                request.QueryString,
                string.Join(", ", headers),
                requestBody
            );
        }

        private async Task LogResponse(HttpContext context, string requestId, long elapsedMs, MemoryStream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            string responseBodyText = string.Empty;

            // Read response body for logging if it's not too large
            if (responseBody.Length < 10000) // Only log responses under 10KB
            {
                using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
                responseBodyText = await reader.ReadToEndAsync();

                // Truncate if still too long
                if (responseBodyText.Length > 1000)
                {
                    responseBodyText = responseBodyText.Substring(0, 1000) + "... [truncated]";
                }
            }

            _logger.LogInformation(
                "Response {RequestId}: Status {StatusCode} - Duration {ElapsedMs}ms - Body: {Body}",
                requestId,
                context.Response.StatusCode,
                elapsedMs,
                string.IsNullOrEmpty(responseBodyText) ? "[body too large]" : responseBodyText
            );

            // Log performance warning for slow requests
            if (elapsedMs > 1000)
            {
                _logger.LogWarning("Slow request detected: {RequestId} took {ElapsedMs}ms", requestId, elapsedMs);
            }
        }
    }
}