
namespace Chapter_9.Middleware
{
    public class LeanPrincipleValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LeanPrincipleValidationMiddleware> _logger;

        public LeanPrincipleValidationMiddleware(RequestDelegate next, ILogger<LeanPrincipleValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request for lean principle tracking
            if (context.Request.Path.StartsWithSegments("/api/lean-testing"))
            {
                _logger.LogInformation("Lean Testing API call: {Method} {Path}",
                    context.Request.Method, context.Request.Path);
            }

            await _next(context);
        }
    }
}