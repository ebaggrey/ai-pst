using Chapter_2.MiddleWares;
using Chapter_2.Models;
using Chapter_2.Services;
using Chapter_2.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

// Load onboarding-specific configuration
builder.Configuration.AddJsonFile("onboarding-settings.json", optional: true);

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Register onboarding services
builder.Services.AddScoped<IOnboardingOrchestrator, OnboardingOrchestrator>();
builder.Services.AddScoped<ICodebaseAnalyzer, SmartCodebaseAnalyzer>();
builder.Services.AddScoped<ITestPatternRecognizer, MLPatternRecognizer>();
builder.Services.AddScoped<ICodeAnalyzer, DefaultCodeAnalyzer>();
builder.Services.AddScoped<ILearningPathGenerator, AILearningPathGenerator>();

// LLM services
builder.Services.AddTransient<ILLMService, OnboardingChatGPTService>();
builder.Services.AddTransient<ILLMService, OnboardingClaudeService>();
builder.Services.AddTransient<ILLMService, OnboardingDeepSeekService>();

// Factory pattern for strategy-based LLM selection
builder.Services.AddSingleton<ILLMServiceFactory, StrategyBasedLLMServiceFactory>();

// HTTP clients with retry policies
builder.Services.AddHttpClient("OnboardingOpenAI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["LLMProviders:OpenAI:BaseUrl"] ?? "https://api.openai.com/v1/");
    client.Timeout = TimeSpan.FromSeconds(90);
    client.DefaultRequestHeaders.Add("X-Onboarding-Phase", "90-day-journey");
});
//.AddPolicyHandler(GetRetryPolicy())
//.AddPolicyHandler(GetCircuitBreakerPolicy());

// Configure onboarding-specific settings
builder.Services.Configure<OnboardingSettings>(builder.Configuration.GetSection("Onboarding"));
builder.Services.Configure<First90DaysSettings>(builder.Configuration.GetSection("First90DaysSettings"));

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<OnboardingHealthCheck>("onboarding-readiness");

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("90-day-plan", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Onboarding 90-Day Plan API",
        Version = "1.0",
        Description = "APIs for managing the 90-day onboarding journey"
    });
});

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/90-day-plan/swagger.json", "90-Day Plan API");
    });
}

app.UseMiddleware<OnboardingProgressTrackerMiddleware>();
app.UseMiddleware<LearningPathMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Special onboarding endpoints
app.MapGet("/api/onboarding/progress/{userId}", async (string userId, IOnboardingOrchestrator orchestrator) =>
{
    return Results.Ok(new
    {
        UserId = userId,
        Progress = "25%",
        CurrentPhase = "Phase 1: Discovery",
        NextCheckpoint = "Complete codebase analysis"
    });
});



app.Run();






// Add this method to Program.cs after the GetCircuitBreakerPolicy method
static async Task<IResult> RecordCheckpointAsync(CheckpointRequest request, ILogger<Program> logger)
{
try
{
logger.LogInformation("Recording checkpoint: {CheckpointName} for user {UserId}",
    request.CheckpointName, request.UserId);

// In a real application, you would:
// 1. Validate the request
// 2. Store in database
// 3. Update user's progress
// 4. Trigger any related workflows

var checkpoint = new Checkpoint
{
Id = Guid.NewGuid().ToString(),
UserId = request.UserId,
CheckpointName = request.CheckpointName,
CompletedAt = DateTime.UtcNow,
Metadata = request.Metadata ?? new Dictionary<string, object>(),
Status = "completed",
DurationMinutes = CalculateCheckpointDuration(request)
};

// Simulate async processing (database save, event publishing, etc.)
await Task.Delay(100);

logger.LogInformation("Checkpoint recorded successfully: {CheckpointId}", checkpoint.Id);

return Results.Ok(new CheckpointResponse
{
CheckpointId = checkpoint.Id,
Timestamp = checkpoint.CompletedAt,
Message = "Checkpoint recorded successfully",
NextCheckpoint = GetNextCheckpoint(checkpoint.CheckpointName),
Progress = CalculateOverallProgress(request.UserId, checkpoint.CheckpointName),
Checkpoint = checkpoint
});
}
catch (Exception ex)
{
logger.LogError(ex, "Failed to record checkpoint: {CheckpointName}", request.CheckpointName);

return Results.Problem(
    title: "Failed to record checkpoint",
    detail: ex.Message,
    statusCode: StatusCodes.Status500InternalServerError,
    extensions: new Dictionary<string, object?>
{
["checkpoint"] = request.CheckpointName,
["userId"] = request.UserId,
["suggestion"] = "Try again or contact support if the issue persists"
});
}
}

// Helper methods for RecordCheckpointAsync
static int CalculateCheckpointDuration(CheckpointRequest request)
{
// Calculate duration based on checkpoint type or metadata
return request.Metadata?.TryGetValue("durationMinutes", out var duration) == true
    ? Convert.ToInt32(duration)
    : 60; // Default 60 minutes
}

static string GetNextCheckpoint(string currentCheckpoint)
{
// Map to next checkpoint in the 90-day journey
var checkpointMap = new Dictionary<string, string>
{
["Day7"] = "Day14",
["Day14"] = "Day30",
["Day30"] = "Day60",
["Day60"] = "Day90",
["Day90"] = "Complete"
};

return checkpointMap.TryGetValue(currentCheckpoint, out var next)
    ? next
    : "Continue to next phase";
}

static decimal CalculateOverallProgress(string userId, string checkpointName)
{
// Calculate overall progress percentage
var checkpointProgress = new Dictionary<string, decimal>
{
["Day7"] = 0.10m,
["Day14"] = 0.20m,
["Day30"] = 0.35m,
["Day60"] = 0.65m,
["Day90"] = 1.00m
};

return checkpointProgress.TryGetValue(checkpointName, out var progress)
    ? progress
    : 0.50m; // Default 50% if unknown checkpoint
}

// Update the endpoint registration in Program.cs to include dependency injection
app.MapPost("/api/onboarding/checkpoint", async (CheckpointRequest request, ILogger<Program> logger) =>
{
return await RecordCheckpointAsync(request, logger);
});

// Required supporting classes (add these to Program.cs or a separate file)
public class CheckpointRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string CheckpointName { get; set; } = string.Empty;

    public Dictionary<string, object> Metadata { get; set; } = new();

    public DateTime? CompletedAt { get; set; }
}

public class CheckpointResponse
{
    public string CheckpointId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
    public string NextCheckpoint { get; set; } = string.Empty;
    public decimal Progress { get; set; }
    public Checkpoint? Checkpoint { get; set; }
}

public class Checkpoint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string CheckpointName { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "completed";
    public int DurationMinutes { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

// Required attribute (if not already present)
public class RequiredAttribute : Attribute
{
    public bool AllowEmptyStrings { get; set; }
}




// Supporting classes


public class OnboardingHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("Onboarding system is operational"));
    }
}

public class DefaultCodeAnalyzer : ICodeAnalyzer
{
    public Task<CodeMetrics> AnalyzeFileAsync(string filePath)
    {
        return Task.FromResult(new CodeMetrics());
    }

    public Task<CodeStructure> AnalyzeStructureAsync(string directoryPath)
    {
        return Task.FromResult(new CodeStructure());
    }

    public Task<DependencyGraph> AnalyzeDependenciesAsync(string filePath)
    {
        return Task.FromResult(new DependencyGraph());
    }
}








