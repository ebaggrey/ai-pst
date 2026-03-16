// Program.cs
using Chapter_7.Extensions;
using Chapter_7.Interfaces;
using Chapter_7.Orchestrators;
using Chapter_7.Services;
using Chapter_7.Services.LLM;
using Chapter_7.Settings;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog if required
//builder.Host.UseSerilog((context, config) =>
//{
//    config.ReadFrom.Configuration(context.Configuration)
//          .Enrich.FromLogContext()
//          .WriteTo.Console()
//          .WriteTo.File("logs/pipeline-cookbook-.txt", rollingInterval: RollingInterval.Day);
//});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("pipeline-cookbook", new OpenApiInfo
    {
        Title = "Pipeline Cookbook API",
        Version = "v1",
        Description = "Intelligent CI/CD Pipeline Management API"
    });
});

// Configure Settings
builder.Services.Configure<LLMSettings>(builder.Configuration.GetSection("LLM"));
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

// Register HTTP Clients
builder.Services.AddHttpClient<ILLMService, OpenAILLMService>();

// Register Core Services
builder.Services.AddScoped<IPipelineGenerator, PipelineGenerator>();
builder.Services.AddScoped<IPipelineDiagnostician, PipelineDiagnostician>();
builder.Services.AddScoped<IPipelineOptimizer, PipelineOptimizer>();
builder.Services.AddScoped<IPipelinePredictor, PipelinePredictor>();
builder.Services.AddScoped<IPipelineAdapter, PipelineAdapter>();

// Register LLM Service
builder.Services.AddScoped<ILLMService, OpenAILLMService>();

// Register Orchestrators
builder.Services.AddScoped<IPipelineOrchestrator, PipelineOrchestrator>();
builder.Services.AddScoped<IDiagnosisOrchestrator, DiagnosisOrchestrator>();
builder.Services.AddScoped<IOptimizationOrchestrator, OptimizationOrchestrator>();
builder.Services.AddScoped<IPredictionOrchestrator, PredictionOrchestrator>();
builder.Services.AddScoped<IAdaptationOrchestrator, AdaptationOrchestrator>();

// Register Database Services
 builder.Services.AddDatabaseServices();

// Register AutoMapper if needed
// builder.Services.AddAutoMapper(typeof(Program));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/pipeline-cookbook/swagger.json", "Pipeline Cookbook API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Configure if required: Serilog request logging
//app.UseSerilogRequestLogging();

app.Run();

