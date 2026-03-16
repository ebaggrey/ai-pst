using Chapter_3.Models.Domain;
using Chapter_3.Models.Requests;
using Chapter_3.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Chapter_3.Middlewares
{
    // Middleware/CollaborationTrackingMiddleware.cs
    public class CollaborationTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CollaborationTrackingMiddleware> _logger;

        public CollaborationTrackingMiddleware(RequestDelegate next, ILogger<CollaborationTrackingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sessionId = context.Request.RouteValues["sessionId"]?.ToString();
            var action = context.Request.Path.ToString();
            var userId = context.User?.Identity?.Name ?? "anonymous";

            _logger.LogInformation("Collaboration request: {Action} for session {SessionId} by {UserId}",
                action, sessionId ?? "none", userId);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Collaboration completed: {Action} took {ElapsedMs}ms",
                    action, stopwatch.ElapsedMilliseconds);
            }
        }
    }

    // Middleware/HumanJudgmentLoggingMiddleware.cs
    public class HumanJudgmentLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HumanJudgmentLoggingMiddleware> _logger;

        public HumanJudgmentLoggingMiddleware(RequestDelegate next, ILogger<HumanJudgmentLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.ToString().Contains("/judge"))
            {
                context.Request.EnableBuffering();
                var originalBodyStream = context.Response.Body;

                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);

                    if (context.Response.StatusCode == StatusCodes.Status200OK)
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                        responseBody.Seek(0, SeekOrigin.Begin);

                        _logger.LogInformation("Human judgment recorded successfully");
                    }
                }
                finally
                {
                    context.Response.Body = originalBodyStream;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }

    // Middleware/SessionCleanupService.cs
    public class SessionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SessionCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(30);

        public SessionCleanupService(IServiceProvider serviceProvider, ILogger<SessionCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Session cleanup service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupOldSessionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during session cleanup");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }

        private async Task CleanupOldSessionsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var sessionStore = scope.ServiceProvider.GetRequiredService<IReviewSessionStore>();

            var oldSessions = await sessionStore.GetSessionsByStatusAsync(ReviewSessionStatus.Closed);

            foreach (var session in oldSessions)
            {
                if (session.ClosedAt.HasValue &&
                    session.ClosedAt.Value < DateTime.UtcNow.AddDays(-7))
                {
                    await sessionStore.DeleteSessionAsync(session.Id);
                    _logger.LogInformation("Cleaned up old session: {SessionId}", session.Id);
                }
            }
        }
    }

    // Middleware/HealthChecks
    public class SessionHealthCheck : IHealthCheck
    {
        private readonly ICollaborationSessionManager _sessionManager;
        private readonly ILogger<SessionHealthCheck> _logger;

        public SessionHealthCheck(ICollaborationSessionManager sessionManager, ILogger<SessionHealthCheck> logger)
        {
            _sessionManager = sessionManager;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Try to access session manager
                _logger.LogDebug("Health check: Testing session manager");
                return Task.FromResult(HealthCheckResult.Healthy("Session management is healthy"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for session management");
                return Task.FromResult(HealthCheckResult.Unhealthy("Session management is unhealthy", ex));
            }
        }
    }

    public class CollaborationHealthCheck : IHealthCheck
    {
        private readonly ILogger<CollaborationHealthCheck> _logger;

        public CollaborationHealthCheck(ILogger<CollaborationHealthCheck> logger)
        {
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Health check: Testing collaboration services");
                return Task.FromResult(HealthCheckResult.Healthy("Collaboration services are healthy"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for collaboration services");
                return Task.FromResult(HealthCheckResult.Unhealthy("Collaboration services are unhealthy", ex));
            }
        }
    }

    public class LearningHealthCheck : IHealthCheck
    {
        private readonly IJudgmentAnalyzer _judgmentAnalyzer;
        private readonly ILogger<LearningHealthCheck> _logger;

        public LearningHealthCheck(IJudgmentAnalyzer judgmentAnalyzer, ILogger<LearningHealthCheck> logger)
        {
            _judgmentAnalyzer = judgmentAnalyzer;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Health check: Testing judgment learning");

                var testRequest = new JudgmentRequest
                {
                    Judgment = new HumanJudgment
                    {
                        Decision = "approve",
                        Reasoning = "Test health check"
                    }
                };

                var learningPoints = await _judgmentAnalyzer.ExtractLearningPointsAsync(testRequest);

                return HealthCheckResult.Healthy($"Learning system is healthy. Can extract {learningPoints.Length} learning points.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for learning system");
                return HealthCheckResult.Unhealthy("Learning system is unhealthy", ex);
            }
        }
    }
}
