namespace Chapter_2.MiddleWares
{
    public class LearningPathMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LearningPathMiddleware> _logger;

        public LearningPathMiddleware(
            RequestDelegate next,
            ILogger<LearningPathMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if this is a learning path related request
            if (context.Request.Path.StartsWithSegments("/api/onboarding"))
            {
                context.Items["LearningPathContext"] = "active";

                // Add learning progress header
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers["X-Learning-Phase"] = "90-day-plan";
                    return Task.CompletedTask;
                });
            }

            await _next(context);
        }
    }
}
