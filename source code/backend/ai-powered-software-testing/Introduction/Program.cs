using Introduction.Controllers;
using Introduction.Services;
using Introduction.Services.Interfaces;
using Introduction.Services.Introduction.Services.Implementations;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test Generation API", Version = "v1" });
});

// Register HTTP clients for each LLM service
builder.Services.AddHttpClient<ChatGPTService>();
builder.Services.AddHttpClient<ClaudeService>();

// Register other services
builder.Services.AddScoped<ICodeTransformer, CodeTransformerService>();
builder.Services.AddScoped<ITestGenerationService, TestGenerationService>();

// Register all LLM services
builder.Services.AddScoped<ILLMService, ChatGPTService>();
builder.Services.AddScoped<ILLMService, ClaudeService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
