// Program.cs
using Chapter_3.Middlewares;
using Chapter_3.Services;
using Chapter_3.Services.Interfaces;
using Chapter_3.Settings;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Load settings
builder.Configuration.AddJsonFile("human-loop-settings.json", optional: true);
builder.Configuration.AddJsonFile("collaboration-settings.json", optional: true);
builder.Configuration.AddJsonFile("learning-settings.json", optional: true);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("human-in-the-loop", new OpenApiInfo
    {
        Title = "Human Review API",
        Version = "v1",
        Description = "API for human-in-the-loop review of AI-generated tests"
    });

    c.TagActionsBy(api => new[] { api.GroupName });
});

// Register human review services
builder.Services.AddScoped<IHumanReviewOrchestrator, HumanReviewOrchestrator>();
builder.Services.AddScoped<ICollaborationSessionManager, CollaborationSessionManager>();
builder.Services.AddScoped<IJudgmentProcessor, JudgmentProcessor>();
builder.Services.AddScoped<IJudgmentAnalyzer, LearningFocusedJudgmentAnalyzer>();

// Session management
builder.Services.AddSingleton<IReviewSessionStore, InMemorySessionStore>();
builder.Services.AddHostedService<SessionCleanupService>();

// LLM services optimized for human collaboration
builder.Services.AddTransient<DialogueOptimizedChatGPTService>();
builder.Services.AddTransient<ClarificationSpecialistClaudeService>();
builder.Services.AddTransient<EditAnalysisDeepSeekService>();
builder.Services.AddTransient<ILLMService>(sp => sp.GetService<DialogueOptimizedChatGPTService>()!);
builder.Services.AddSingleton<ILLMServiceFactory, LLMServiceFactory>();

// Collaboration tools
builder.Services.AddScoped<ICollaborationTools, RealTimeCollaborationTools>();
builder.Services.AddScoped<IDiffService, IntelligentDiffService>();
builder.Services.AddScoped<ISuggestionEngine, ContextAwareSuggestionEngine>();

// SignalR for real-time collaboration
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

// Configure settings
builder.Services.Configure<HumanLoopSettings>(builder.Configuration.GetSection("HumanLoop"));
builder.Services.Configure<CollaborationSettings>(builder.Configuration.GetSection("Collaboration"));
builder.Services.Configure<LearningFromJudgmentsSettings>(builder.Configuration.GetSection("LearningFromJudgments"));

// Add health checks for collaboration services
builder.Services.AddHealthChecks()
    .AddCheck<SessionHealthCheck>("session-management")
    .AddCheck<CollaborationHealthCheck>("real-time-collaboration")
    .AddCheck<LearningHealthCheck>("judgment-learning");

// Add CORS for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}

// Add logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/human-in-the-loop/swagger.json", "Human Review API v1");
        c.RoutePrefix = "swagger";
    });

    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Middleware
app.UseMiddleware<CollaborationTrackingMiddleware>();
app.UseMiddleware<HumanJudgmentLoggingMiddleware>();

// Map SignalR hub
app.MapHub<CollaborationHub>("/collaboration-hub");

// Map health checks
app.MapHealthChecks("/health");

// Map controllers
app.MapControllers();

app.Run();

