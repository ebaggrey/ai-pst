// Program.cs
using Chapter_9.Interfaces;
using Chapter_9.Middleware;
using Chapter_9.Orchestrators;
using Chapter_9.Services;
using Chapter_9.Services.LLM;
using Chapter_9.Settings;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lean Testing API",
        Version = "v1",
        Description = "API for Lean Testing principles and optimization"
    });

    c.SwaggerDoc("lean-testing", new OpenApiInfo
    {
        Title = "Lean Testing",
        Version = "v1",
        Description = "Lean Testing optimization endpoints"
    });

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (docName == "v1") return true;
        return apiDesc.GroupName == docName;
    });
});

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register configuration
builder.Services.Configure<LeanTestingSettings>(builder.Configuration.GetSection("LeanTesting"));
builder.Services.Configure<LLMSettings>(builder.Configuration.GetSection("LLM"));

// FIX: Change orchestrator to Scoped lifetime to match its dependencies
builder.Services.AddScoped<ILeanTestingOrchestrator, LeanTestingOrchestrator>();

// Register Services - All as Scoped (consistent lifetime)
builder.Services.AddScoped<IPriorityOptimizer, PriorityOptimizer>();
builder.Services.AddScoped<IMinimalCoverageGenerator, MinimalCoverageGenerator>();
builder.Services.AddScoped<IAutomationDecider, AutomationDecider>();
builder.Services.AddScoped<IMaintenanceOptimizer, MaintenanceOptimizer>();
builder.Services.AddScoped<IROIAnalyzer, ROIAnalyzer>();
builder.Services.AddScoped<ITestScenarioGenerator, TestScenarioGenerator>();
builder.Services.AddScoped<ITestOptimizer, TestOptimizer>();
builder.Services.AddScoped<ICostCalculator, CostCalculator>();
builder.Services.AddScoped<IROICalculator, ROICalculator>();
builder.Services.AddScoped<ITestAnalyzer, TestAnalyzer>();
builder.Services.AddScoped<IValueCalculator, ValueCalculator>();

// Register LLM Service
builder.Services.AddScoped<ILLMService, OllamaLLMService>();

// Register HttpClient - Transient is fine for HttpClient
builder.Services.AddHttpClient("LLMClient", client =>
{
    var llmSettings = builder.Configuration.GetSection("LLM").Get<LLMSettings>();
    client.BaseAddress = new Uri(llmSettings?.BaseUrl ?? "http://localhost:11434");
    client.Timeout = TimeSpan.FromSeconds(llmSettings?.TimeoutSeconds ?? 30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    if (!string.IsNullOrEmpty(llmSettings?.ApiKey))
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {llmSettings.ApiKey}");
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                new[] { "http://localhost:3000", "https://localhost:5001" })
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

// Add health checks
builder.Services.AddHealthChecks();

// Add response caching
builder.Services.AddResponseCaching();

// Add memory cache
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lean Testing API V1");
        c.SwaggerEndpoint("/swagger/lean-testing/swagger.json", "Lean Testing");
    });
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseResponseCaching();

// Add Serilog request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

// Custom middleware for lean principles validation
app.UseMiddleware<LeanPrincipleValidationMiddleware>();

app.UseRouting();
app.UseAuthorization();

// Map endpoints
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

