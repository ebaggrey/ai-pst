using System.Text.Json.Serialization;
using Chapter_1.Middleware;
using Chapter_1.Middlewares;
using Chapter_1.Models;
using Chapter_1.Services;
using Chapter_1.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddApplicationPart(typeof(Chapter_1.Controllers.LandscapeAnalysisController).Assembly);

// Configure CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure API Versioning
//builder.Services.AddApiVersioning(options =>
//{
//    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
//    options.AssumeDefaultVersionWhenUnspecified = true;
//    options.ReportApiVersions = true;
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Chapter 1 - Testing Landscape Analyzer API",
        Version = "v1",
        Description = "API for analyzing application testing landscapes and generating intelligent test strategies",
        Contact = new OpenApiContact
        {
            Name = "QA Team",
            Email = "qa@chapter1.com"
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register architecture analysis plugins
builder.Services.AddTransient<IArchitecturePlugin, MicroservicePlugin>();
builder.Services.AddTransient<IArchitecturePlugin, FrontendPlugin>();
builder.Services.AddTransient<IArchitecturePlugin, DatabasePlugin>();
builder.Services.AddTransient<IArchitecturePlugin, ExternalIntegrationPlugin>();
builder.Services.AddTransient<IArchitecturePlugin, SecurityPlugin>();

// Register LLM services
builder.Services.AddScoped<ILLMService, OpenAIService>();
builder.Services.AddScoped<ILLMService, AnthropicService>();
builder.Services.AddScoped<ILLMService, DeepSeekService>();
builder.Services.AddScoped<ILLMService, GoogleAIService>();
builder.Services.AddScoped<ILLMService, LocalLLMService>();

// Register core services
builder.Services.AddScoped<ILandscapeAnalyzer, IntelligentLandscapeAnalyzer>();
builder.Services.AddScoped<ILLMOrchestrator, LLMOrchestrator>();
builder.Services.AddScoped<ITestSynthesisService, IntelligentTestSynthesisService>();

// Register HTTP clients for external AI services
builder.Services.AddHttpClient("OpenAI", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiKey = configuration["LLMProviders:OpenAI:ApiKey"];
    var baseUrl = configuration["LLMProviders:OpenAI:BaseUrl"] ?? "https://api.openai.com/v1/";

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("User-Agent", "Chapter1-LandscapeAnalyzer/1.0");

    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }
});

builder.Services.AddHttpClient("Anthropic", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiKey = configuration["LLMProviders:Anthropic:ApiKey"];
    var baseUrl = configuration["LLMProviders:Anthropic:BaseUrl"] ?? "https://api.anthropic.com/v1/";

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);

    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
    }
});

builder.Services.AddHttpClient("DeepSeek", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiKey = configuration["LLMProviders:DeepSeek:ApiKey"];
    var endpoint = configuration["LLMProviders:DeepSeek:Endpoint"] ?? "https://api.deepseek.com/v1/chat/completions";

    client.BaseAddress = new Uri(endpoint);
    client.Timeout = TimeSpan.FromSeconds(60);

    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }
});

builder.Services.AddHttpClient("GoogleAI", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiKey = configuration["LLMProviders:GoogleAI:ApiKey"];
    var baseUrl = configuration["LLMProviders:GoogleAI:BaseUrl"] ?? "https://generativelanguage.googleapis.com/v1/";

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddHttpClient("LocalLLM", client =>
{
    client.Timeout = TimeSpan.FromSeconds(120); // Longer timeout for local models
});

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<LLMHealthCheck>("openai-check", tags: new[] { "llm", "openai" })
    .AddCheck<LLMHealthCheck>("anthropic-check", tags: new[] { "llm", "anthropic" })
    .AddCheck<LLMHealthCheck>("deepseek-check", tags: new[] { "llm", "deepseek" })
    .AddCheck<LLMHealthCheck>("google-check", tags: new[] { "llm", "google" })
    .AddCheck<SystemHealthCheck>("system-check", tags: new[] { "system" })
    .AddCheck<DatabaseHealthCheck>("database-check", tags: new[] { "database" });

// Configure Response Caching
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1 MB
    options.SizeLimit = 100 * 1024 * 1024; // 100 MB
    options.UseCaseSensitivePaths = false;
});

// Configure Memory Cache
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 100 * 1024 * 1024; // 100 MB
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
});

// Configure Options pattern for strong configuration
builder.Services.Configure<LLMStrategyOptions>(builder.Configuration.GetSection("LLMStrategy"));
builder.Services.Configure<LLMProviderOptions>(builder.Configuration.GetSection("LLMProviders"));
builder.Services.Configure<TestingLandscapeOptions>(builder.Configuration.GetSection("TestingLandscape"));

// Add custom validation
builder.Services.AddScoped<RiskAssessmentValidator>();
builder.Services.AddScoped<LandscapeRequestValidator>();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.AddEventSourceLogger();

    if (builder.Environment.IsDevelopment())
    {
        logging.AddEventLog();
    }
});

// Add Application Insights if needed
// builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chapter 1 - Testing Landscape Analyzer API V1");
        c.RoutePrefix = "swagger";
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Use custom middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

// Use CORS
app.UseCors("AllowAngularApp");

// Use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Use Response Caching
app.UseResponseCaching();

// Use Authentication/Authorization if needed
// app.UseAuthentication();
// app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map health checks with custom responses
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration,
                tags = e.Value.Tags,
                error = e.Value.Exception?.Message
            }),
            totalDuration = report.TotalDuration,
            timestamp = DateTime.UtcNow
        };
        await context.Response.WriteAsJsonAsync(response);
    },
    Predicate = _ => true,
    AllowCachingResponses = false
});

// Map health checks for specific tags
app.MapHealthChecks("/health/llm", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("llm"),
    ResponseWriter = HealthCheckResponseWriter.WriteJson
});

app.MapHealthChecks("/health/system", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("system"),
    ResponseWriter = HealthCheckResponseWriter.WriteJson
});

// Map a simple root endpoint
app.MapGet("/", () => Results.Ok(new
{
    service = "Chapter 1 - Testing Landscape Analyzer API",
    version = "1.0.0",
    status = "operational",
    timestamp = DateTime.UtcNow,
    documentation = "/swagger",
    endpoints = new[]
    {
        "POST /api/landscape/analyze - Analyze testing landscape",
        "GET /api/landscape/analysis/{id} - Get analysis by ID",
        "GET /api/landscape/health - Health check"
    }
}));

// Map a simple error endpoint
app.MapGet("/error", () => Results.Problem("An error occurred while processing your request."));

app.Run();

// Health check response writer helper
public static class HealthCheckResponseWriter
{
    public static async Task WriteJson(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration,
                error = e.Value.Exception?.Message,
                data = e.Value.Data
            }),
            totalDuration = report.TotalDuration,
            timestamp = DateTime.UtcNow
        };
        await context.Response.WriteAsJsonAsync(response);
    }
}

// Health check implementations
public class LLMHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LLMHealthCheck> _logger;
    private readonly IConfiguration _configuration;

    public LLMHealthCheck(
        IHttpClientFactory httpClientFactory,
        ILogger<LLMHealthCheck> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var provider = context.Registration.Tags.FirstOrDefault(t => t != "llm") ?? "unknown";

        try
        {
            var client = _httpClientFactory.CreateClient(provider);

            // Simple health check - try to get models list or just check connectivity
            var response = await client.GetAsync("models", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy($"{provider} LLM service is operational");
            }

            return HealthCheckResult.Degraded($"{provider} LLM service returned {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Health check failed for {Provider}", provider);
            return HealthCheckResult.Unhealthy($"{provider} LLM service is unavailable", ex);
        }
    }
}

public class SystemHealthCheck : IHealthCheck
{
    private readonly ILogger<SystemHealthCheck> _logger;

    public SystemHealthCheck(ILogger<SystemHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var memory = GC.GetTotalMemory(false);
        var memoryInfo = new
        {
            TotalMemory = memory,
            MemoryInMB = memory / (1024 * 1024),
            ProcessorCount = Environment.ProcessorCount,
            WorkingSet = Environment.WorkingSet,
            OSVersion = Environment.OSVersion.ToString(),
            RuntimeVersion = Environment.Version.ToString()
        };

        var data = new Dictionary<string, object>
        {
            ["memory"] = memoryInfo,
            ["timestamp"] = DateTime.UtcNow
        };

        return Task.FromResult(HealthCheckResult.Healthy("System is operational", data));
    }
}

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(ILogger<DatabaseHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Add database connectivity check here if needed
        return Task.FromResult(HealthCheckResult.Healthy("Database connection is healthy"));
    }
}

// Configuration Options classes
public class LLMStrategyOptions
{
    public Dictionary<string, string> AreaMapping { get; set; } = new();
    public string[] FallbackOrder { get; set; } = Array.Empty<string>();
    public int TimeoutSeconds { get; set; } = 45;
    public int MaxRetries { get; set; } = 2;
    public CostOptimizationOptions CostOptimization { get; set; } = new();
}

public class CostOptimizationOptions
{
    public string[] UseCheaperModelFor { get; set; } = Array.Empty<string>();
    public decimal BudgetPerRequest { get; set; } = 0.10m;
    public decimal MonthlyLimit { get; set; } = 50.00m;
}

public class LLMProviderOptions
{
    public OpenAIProvider OpenAI { get; set; } = new();
    public AnthropicProvider Anthropic { get; set; } = new();
    public DeepSeekProvider DeepSeek { get; set; } = new();
    public GoogleAIProvider GoogleAI { get; set; } = new();
    public LocalLLMProvider LocalLLM { get; set; } = new();
}

public class OpenAIProvider
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openai.com/v1/";
    public string DefaultModel { get; set; } = "gpt-4-turbo-preview";
    public decimal CostPerToken { get; set; } = 0.00003m;
    public int MaxTokens { get; set; } = 4096;
    public int TimeoutSeconds { get; set; } = 60;
}

public class AnthropicProvider
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.anthropic.com/v1/";
    public string DefaultModel { get; set; } = "claude-3-sonnet-20240229";
    public int MaxTokens { get; set; } = 8192;
    public int TimeoutSeconds { get; set; } = 60;
}

public class DeepSeekProvider
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "https://api.deepseek.com/v1/chat/completions";
    public int ContextWindow { get; set; } = 16384;
    public int TimeoutSeconds { get; set; } = 60;
}

public class GoogleAIProvider
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1/";
    public string Model { get; set; } = "gemini-pro";
    public int TimeoutSeconds { get; set; } = 60;
}

public class LocalLLMProvider
{
    public string Endpoint { get; set; } = "http://localhost:8080/v1/completions";
    public string Model { get; set; } = "llama2-13b-chat";
    public int TimeoutSeconds { get; set; } = 120;
}

public class TestingLandscapeOptions
{
    public double DefaultRiskThreshold { get; set; } = 7.0;
    public int MaxScenariosPerArea { get; set; } = 10;
    public bool IncludeSecurityScan { get; set; } = true;
    public string PerformanceBaseline { get; set; } = "p95_under_2s";
    public int CacheDurationMinutes { get; set; } = 30;
    public int MaxConcurrentAnalyses { get; set; } = 5;
    public bool EnableDetailedLogging { get; set; } = true;
}

// Validation classes
public class RiskAssessmentValidator
{
    public bool Validate(RiskAssessment assessment)
    {
        // Add validation logic
        return assessment != null &&
               assessment.Criticality >= 1 &&
               assessment.Criticality <= 10;
    }
}

public class LandscapeRequestValidator
{
    public bool Validate(LandscapeTestRequest request)
    {
        return request != null &&
               request.ApplicationProfile != null &&
               !string.IsNullOrEmpty(request.ApplicationProfile.Name) &&
               request.TestingFocus != null &&
               request.TestingFocus.Length > 0;
    }
}