namespace Chapter_2.MiddleWares
{
    public class OnboardingProgressTrackerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OnboardingProgressTrackerMiddleware> _logger;

        public OnboardingProgressTrackerMiddleware(
            RequestDelegate next,
            ILogger<OnboardingProgressTrackerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;

            try
            {
                // Add onboarding context to request
                context.Items["OnboardingPhase"] = "90-day-journey";

                await _next(context);

                // Log successful completion
                var duration = DateTime.UtcNow - startTime;
                _logger.LogInformation(
                    "Onboarding request completed: {Method} {Path} - {StatusCode} in {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                var duration = DateTime.UtcNow - startTime;
                _logger.LogError(ex,
                    "Onboarding request failed: {Method} {Path} - {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    duration.TotalMilliseconds);

                throw;
            }
        }
    }
}
