using Chapter_11.Data;
using Chapter_11.Interfaces;
using Chapter_11.Middlewares;
using Chapter_11.Orchestrators;
using Chapter_11.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure API Explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo  // Changed from "full-spectrum" to "v1"
    {
        Title = "Full Spectrum API",
        Version = "v1",
        Description = "API for full spectrum testing and monitoring",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@fullspectrum.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Use camelCase for property names
    c.UseAllOfToExtendReferenceSchemas();
});

// Rest of your service registrations...
builder.Services.AddDbContext<SpectrumDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<ILLMService, LLMService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<ILLMService, LLMService>();
builder.Services.AddScoped<IShiftLeftOrchestrator, ShiftLeftOrchestrator>();
builder.Services.AddScoped<ITestabilityAnalyzer, TestabilityAnalyzer>();
builder.Services.AddScoped<IShiftRightOrchestrator, ShiftRightOrchestrator>();
builder.Services.AddScoped<ISpectrumPipelineBuilder, SpectrumPipelineBuilder>();
builder.Services.AddScoped<ICrossSpectrumOrchestrator, CrossSpectrumOrchestrator>();

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

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Full Spectrum API V1");  // Match the document name
        c.RoutePrefix = "swagger";  // This makes Swagger UI available at /swagger
        c.DocumentTitle = "Full Spectrum API Documentation";
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Map endpoints
app.MapControllers();

app.MapGet("/debug/swagger", () =>
{
    var swaggerPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "swagger");
    return Results.Ok(new
    {
        swaggerExists = Directory.Exists(swaggerPath),
        baseDirectory = AppContext.BaseDirectory
    });
});
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SpectrumDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

app.Run();