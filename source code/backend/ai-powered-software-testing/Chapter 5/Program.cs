// Program.cs
using Chapter_5.Configuration;
using Chapter_5.Interfaces;
using Chapter_5.Models.Requests;
using Chapter_5.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Configure services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TDD Reimagined API",
        Version = "v1",
        Description = "A reimagined TDD workflow API for modern development",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@tddreimagined.com"
        }
    });

    c.SwaggerDoc("tdd-reimagined", new OpenApiInfo
    {
        Title = "TDD Reimagined",
        Version = "v1",
        Description = "TDD-specific endpoints for test-driven development workflows"
    });

    c.TagActionsBy(api => new[] { api.GroupName });
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;

        var attributes = methodInfo.DeclaringType?.GetCustomAttributes(true)
            .Union(methodInfo.GetCustomAttributes(true))
            .OfType<ApiExplorerSettingsAttribute>();

        return attributes?.Any(a => a.GroupName == docName) == true || docName == "v1";
    });
});

// Add logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
    loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

// Register configuration
builder.Services.Configure<TDDConfiguration>(builder.Configuration.GetSection("TDD"));
builder.Services.AddSingleton<TDDConfiguration>(sp =>
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<TDDConfiguration>>().Value);

// Register services
builder.Services.AddScoped<ITDDOrchestrator, TDDOrchestrator>();
builder.Services.AddScoped<ITestFirstGenerator, TestFirstGenerator>();
builder.Services.AddScoped<IImplementationGenerator, ImplementationGenerator>();
builder.Services.AddScoped<IRefactoringAdvisor, RefactoringAdvisor>();
builder.Services.AddScoped<IFuturePredictor, FuturePredictor>();

// Register helpers as singletons
//builder.Services.AddSingleton<TDDControllerHelpers>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TDD Reimagined API v1");
        c.SwaggerEndpoint("/swagger/tdd-reimagined/swagger.json", "TDD Reimagined");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Error handling endpoint
app.Map("/error", () => Results.Problem("An error occurred.", statusCode: 500));

// API documentation endpoint
app.MapGet("/", () => Results.Redirect("/swagger"));

// Global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception occurred");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var errorResponse = new TDDErrorResponse
        {
            Phase = "global",
            ErrorType = "unhandled-exception",
            Message = "An unexpected error occurred",
            RecoveryStrategy = new[] { "Check logs for details", "Retry the operation" },
            SuggestedFallback = "Contact system administrator"
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
});

app.Run();

// Make Program class accessible for testing
public partial class Program { }

