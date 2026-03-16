

using Chapter_6.Interfaces.BDDSupercharged.Interfaces;
using Chapter_6.Interfaces.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
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
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BDD Supercharged API",
        Version = "v1",
        Description = "API for BDD scenario co-creation, automation translation, evolution, drift detection, and living documentation generation."
    });

    c.SwaggerDoc("bdd-supercharged", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BDD Supercharged",
        Version = "v1",
        Description = "BDD-specific operations"
    });

    c.TagActionsBy(api => new[] { api.GroupName });
    c.DocInclusionPredicate((name, api) => true);
});

// Register logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

// Register custom services
builder.Services.AddScoped<IBDDConversationOrchestrator, BDDConversationOrchestrator>();
builder.Services.AddScoped<IScenarioTranslator, ScenarioTranslator>();
builder.Services.AddScoped<IScenarioEvolver, ScenarioEvolver>();
builder.Services.AddScoped<IDriftDetector, DriftDetector>();
builder.Services.AddScoped<ILivingDocumentationGenerator, LivingDocumentationGenerator>();

// Register HttpClient for external calls if needed
builder.Services.AddHttpClient();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BDD Supercharged API v1");
        c.SwaggerEndpoint("/swagger/bdd-supercharged/swagger.json", "BDD Supercharged");
        c.RoutePrefix = "swagger";
    });

    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Add global error handling
app.UseExceptionHandler("/error");

app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500));

app.Run();

