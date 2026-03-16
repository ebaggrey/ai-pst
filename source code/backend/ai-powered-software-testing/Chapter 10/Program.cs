using Chapter_10.Data;
using Chapter_10.Extensions;
using Chapter_10.Interfaces;
using Chapter_10.Interfaces.LLM;
using Chapter_10.Middlewares;
using Chapter_10.Services;
using MetricsThatMatter.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Metrics That Matter API",
        Version = "v1",
        Description = "API for designing, calculating, predicting, and optimizing testing metrics",
        Contact = new OpenApiContact
        {
            Name = "Metrics That Matter Team",
            Email = "support@metricsthatmatter.com"
        }
    });

    c.DocInclusionPredicate((docName, apiDesc) => true);
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default" });
});

builder.Services.AddScoped<ILLMService, LLMService>();

// Register Repository
builder.Services.AddScoped<IMetricsRepository, MetricsRepository>();

// Register all services using extension method
builder.Services.AddMetricsThatMatterServices(builder.Configuration);

// Add DbContext
builder.Services.AddDbContext<MetricsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add HttpClient for LLM service
builder.Services.AddHttpClient("LLMService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["LLM:BaseUrl"] ?? "http://localhost:5000");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["LLM:ApiKey"]}");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configure CORS using fixed extension methods
builder.Services.AddCors(options =>
{
    options.AddPolicy("MetricsThatMatterPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        var allowedMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>();
        var allowedHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>();

        bool hasSpecificOrigins;

        policy.WithOriginsIfProvided(allowedOrigins, out hasSpecificOrigins)
              .WithMethodsIfProvided(allowedMethods)
              .WithHeadersIfProvided(allowedHeaders)
              .ConfigureCredentials(hasSpecificOrigins);
    });
});

// Configure Health Checks
builder.Services.AddHealthChecks();

// Configure Response Caching
builder.Services.AddResponseCaching();

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register all services using extension method
builder.Services.AddMetricsThatMatterServices(builder.Configuration);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Register Repository
builder.Services.AddScoped<IMetricsRepository, MetricsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Metrics That Matter API v1");
        c.RoutePrefix = "swagger";
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

// Use custom exception middleware
app.UseMetricsExceptionMiddleware();

// Use CORS
app.UseCors("MetricsThatMatterPolicy");

// Use HTTPS redirection
app.UseHttpsRedirection();

// Use response caching
app.UseResponseCaching();

// Use routing
app.UseRouting();

// Use authorization
app.UseAuthorization();

// Map endpoints
app.MapControllers();
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok(new { message = "Metrics That Matter API is running" }));

app.Run();

