using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace Chapter_1.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly IConfiguration _configuration;

        // Rate limiting storage
        private static readonly ConcurrentDictionary<string, ClientRequestTracker> _clientTrackers = new();

        // Configuration defaults
        private readonly bool _enabled;
        private readonly int _maxRequestsPerMinute;
        private readonly int _maxRequestsPerHour;
        private readonly int _burstSize;

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;

            // Load configuration
            var rateLimitConfig = configuration.GetSection("RateLimiting");
            _enabled = rateLimitConfig.GetValue<bool>("Enabled", true);
            _maxRequestsPerMinute = rateLimitConfig.GetValue<int>("MaxRequestsPerMinute", 60);
            _maxRequestsPerHour = rateLimitConfig.GetValue<int>("MaxRequestsPerHour", 1000);
            _burstSize = rateLimitConfig.GetValue<int>("BurstSize", 10);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_enabled)
            {
                await _next(context);
                return;
            }

            // Get client identifier (IP address or API key)
            var clientId = GetClientIdentifier(context);

            // Check rate limits
            var tracker = _clientTrackers.GetOrAdd(clientId, _ => new ClientRequestTracker());

            if (!IsRequestAllowed(tracker))
            {
                await WriteRateLimitResponse(context, tracker);
                return;
            }

            // Track the request
            tracker.TrackRequest();

            // Add rate limit headers
            context.Response.Headers.Add("X-RateLimit-Limit", _maxRequestsPerMinute.ToString());
            context.Response.Headers.Add("X-RateLimit-Remaining", (_maxRequestsPerMinute - tracker.RequestsLastMinute).ToString());
            context.Response.Headers.Add("X-RateLimit-Reset", tracker.ResetTime.ToString("o"));

            await _next(context);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            // Try to get API key from header first
            if (context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
            {
                return $"apikey:{apiKey}";
            }

            // Fall back to IP address
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return $"ip:{ipAddress}";
        }

        private bool IsRequestAllowed(ClientRequestTracker tracker)
        {
            // Clean up old requests
            tracker.Cleanup();

            // Check burst limit
            if (tracker.RequestsLastSecond > _burstSize)
            {
                _logger.LogWarning("Rate limit exceeded - burst limit: {BurstSize}", _burstSize);
                return false;
            }

            // Check minute limit
            if (tracker.RequestsLastMinute >= _maxRequestsPerMinute)
            {
                _logger.LogWarning("Rate limit exceeded - minute limit: {MaxRequestsPerMinute}", _maxRequestsPerMinute);
                return false;
            }

            // Check hour limit
            if (tracker.RequestsLastHour >= _maxRequestsPerHour)
            {
                _logger.LogWarning("Rate limit exceeded - hour limit: {MaxRequestsPerHour}", _maxRequestsPerHour);
                return false;
            }

            return true;
        }

        private async Task WriteRateLimitResponse(HttpContext context, ClientRequestTracker tracker)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";

            var retryAfter = (int)Math.Ceiling((tracker.ResetTime - DateTime.UtcNow).TotalSeconds);
            context.Response.Headers.Add("Retry-After", retryAfter.ToString());

            var error = new
            {
                errorCode = "RATE_LIMIT_EXCEEDED",
                message = "Rate limit exceeded. Please slow down your requests.",
                retryAfterSeconds = retryAfter,
                limits = new
                {
                    perSecond = _burstSize,
                    perMinute = _maxRequestsPerMinute,
                    perHour = _maxRequestsPerHour
                },
                currentUsage = new
                {
                    lastSecond = tracker.RequestsLastSecond,
                    lastMinute = tracker.RequestsLastMinute,
                    lastHour = tracker.RequestsLastHour
                },
                timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        // Client request tracker class
        private class ClientRequestTracker
        {
            private readonly object _lock = new();
            private readonly List<DateTime> _requestTimestamps = new();

            public int RequestsLastSecond => GetRequestCount(TimeSpan.FromSeconds(1));
            public int RequestsLastMinute => GetRequestCount(TimeSpan.FromMinutes(1));
            public int RequestsLastHour => GetRequestCount(TimeSpan.FromHours(1));

            public DateTime ResetTime => DateTime.UtcNow.AddMinutes(1);

            public void TrackRequest()
            {
                lock (_lock)
                {
                    _requestTimestamps.Add(DateTime.UtcNow);

                    // Keep only last hour of timestamps
                    var cutoff = DateTime.UtcNow.AddHours(-1);
                    _requestTimestamps.RemoveAll(t => t < cutoff);
                }
            }

            public void Cleanup()
            {
                lock (_lock)
                {
                    var cutoff = DateTime.UtcNow.AddHours(-1);
                    _requestTimestamps.RemoveAll(t => t < cutoff);
                }
            }

            private int GetRequestCount(TimeSpan window)
            {
                lock (_lock)
                {
                    var cutoff = DateTime.UtcNow.Subtract(window);
                    return _requestTimestamps.Count(t => t >= cutoff);
                }
            }
        }
    }
}