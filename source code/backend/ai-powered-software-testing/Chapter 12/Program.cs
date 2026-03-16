// Program.cs

using Chapter_12.Configuration;
using Chapter_12.Data;
using Chapter_12.Interfaces;
using Chapter_12.Middlewares;
using Chapter_12.Orchestrators;
using Chapter_12.Repositories;
using Chapter_12.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add API Explorer for Swagger
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Powered Testing API",
        Version = "v1",
        Description = "API for bias auditing of test data using AI",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "support@aipoweredtesting.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configuration
builder.Services.Configure<AIServiceConfig>(
    builder.Configuration.GetSection("AIService"));
builder.Services.Configure<DatabaseConfig>(
    builder.Configuration.GetSection("Database"));

// Database Context
builder.Services.AddDbContext<AuditDbContext>((serviceProvider, options) =>
{
    var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfig>>().Value;
    options.UseSqlServer(dbConfig.ConnectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(dbConfig.CommandTimeout);
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });

    if (dbConfig.EnableSensitiveDataLogging)
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// HTTP Clients
builder.Services.AddHttpClient<ILLMService, LLMService>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IOptions<AIServiceConfig>>().Value;
    client.BaseAddress = new Uri(config.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    if (!string.IsNullOrEmpty(config.ApiKey))
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.ApiKey}");
    }
});

// Dependency Injection - Services
builder.Services.AddScoped<IBiasAuditService, BiasAuditService>();
builder.Services.AddScoped<ILLMService, LLMService>();

// Dependency Injection - Orchestrators
builder.Services.AddScoped<IBiasOrchestrator, BiasOrchestrator>();

// Dependency Injection - Repositories
builder.Services.AddScoped<IAuditRepository, AuditRepository>();

// Add custom middleware
builder.Services.AddTransient<GlobalExceptionMiddleware>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AuditDbContext>(
        name: "database",
        tags: new[] { "database", "sqlserver" });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (allowedOrigins != null && allowedOrigins.Any())
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

// Add response caching
builder.Services.AddResponseCaching();

// Add compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Powered Testing API V1");
        c.RoutePrefix = "swagger"; // This makes Swagger UI available at /swagger
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Powered Testing API V1");
        c.RoutePrefix = "swagger"; // This makes Swagger UI available at /swagger
    });
}

// Use custom middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Use compression
app.UseResponseCompression();

// Use CORS
app.UseCors("AllowFrontend");

// Use HTTPS redirection
app.UseHttpsRedirection();

// Use response caching
app.UseResponseCaching();

// Use routing
app.UseRouting();

// Use authorization (if needed)
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map health checks
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(e => new
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Duration = e.Value.Duration.ToString(),
                Tags = e.Value.Tags
            }),
            Duration = report.TotalDuration
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Lightweight check, just returns if the app is running
});

// Simple health check endpoint
app.MapGet("/health/ping", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AuditDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database ensured created successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while ensuring the database was created");
    }
}

app.Run();

public partial class Program { }
