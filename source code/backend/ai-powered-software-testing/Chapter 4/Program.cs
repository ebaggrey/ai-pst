// Program.cs - Complete working version
using Chapter_4.Services.Implementations;
using Chapter_4.Services.Interfaces;
using Chapter_4.Settings;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Configure AITestingConfiguration
builder.Services.Configure<AITestingConfiguration>(builder.Configuration.GetSection("AITesting"));

// Add services
builder.Services.AddControllers();

// Add CORS with specific policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5000",
                "https://localhost:5001",
                "http://localhost:3000",
                "https://localhost:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("https://yourapp.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("ai-testing", new OpenApiInfo
    {
        Title = "AI Testing API",
        Version = "v1",
        Description = "API for testing and monitoring AI models"
    });

    // Add CORS support to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

// Register services
builder.Services.AddScoped<IAICapabilityAssessor, AICapabilityAssessor>();
builder.Services.AddScoped<IAIRobustnessTester, AIRobustnessTester>();
builder.Services.AddScoped<IAIBiasDetector, AIBiasDetector>();
builder.Services.AddScoped<IAIHallucinationDetector, AIHallucinationDetector>();
builder.Services.AddScoped<IAIDriftMonitor, AIDriftMonitor>();

// Register configuration
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return config.GetSection("AITesting").Get<AITestingConfiguration>() ?? new AITestingConfiguration();
});

// Add HttpClient
builder.Services.AddHttpClient("AIService", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configure middleware pipeline - ORDER IS CRITICAL
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/ai-testing/swagger.json", "AI Testing API v1");
        c.RoutePrefix = "swagger";
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// CORS must come after HTTPS redirection and before routing
app.UseCors("DevelopmentPolicy");

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .RequireCors("DevelopmentPolicy");

// Root redirect to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
   .RequireCors("DevelopmentPolicy");

app.Run();
