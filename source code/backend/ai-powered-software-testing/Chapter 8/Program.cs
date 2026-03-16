

using Chapter_8.Interfaces;
using Chapter_8.Orchestrators;
using Chapter_8.Services;
using Chapter_8.Services.LLM;
using LegacyConquest.Orchestrators;
using LegacyConquest.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Legacy Conquest API",
        Version = "v1"
    });
});

// HTTP Client for LLM
builder.Services.AddHttpClient<ILLMService, LLMService>();

// Core Services
builder.Services.AddScoped<ILegacyAnalyzer, LegacyAnalyzer>();
builder.Services.AddScoped<IWrapperGenerator, WrapperGenerator>();
builder.Services.AddScoped<ICharacterizationTestCreator, CharacterizationTestCreator>();
builder.Services.AddScoped<IModernizationPlanner, ModernizationPlanner>();
builder.Services.AddScoped<ILegacyHealthMonitor, LegacyHealthMonitor>();

// LLM Service
builder.Services.AddScoped<ILLMService, LLMService>();

// Orchestrators
builder.Services.AddScoped<ILegacyAnalysisOrchestrator, LegacyAnalysisOrchestrator>();
builder.Services.AddScoped<IWrapperGenerationOrchestrator, WrapperGenerationOrchestrator>();
builder.Services.AddScoped<ITestCreationOrchestrator, TestCreationOrchestrator>();
builder.Services.AddScoped<IModernizationPlanningOrchestrator, ModernizationPlanningOrchestrator>();
builder.Services.AddScoped<IHealthMonitoringOrchestrator, HealthMonitoringOrchestrator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Development pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Legacy Conquest API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Simple root endpoint
app.MapGet("/", () => "Legacy Conquest API is running");

app.Run();

