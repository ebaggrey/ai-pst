
using Chapter_1.Exceptions;
using Chapter_1.Models;
using Chapter_1.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Chapter_1.Services
{
    public class OpenAIService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIService> _logger;

        public string ProviderName => "chatgpt";

        public OpenAIService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<OpenAIService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("OpenAI");
            _configuration = configuration;
            _logger = logger;
        }

        // OpenAIService.cs - Fixed
        public async Task<string> GenerateTestCodeAsync(string prompt, string context)
        {
            try
            {
                _logger.LogInformation("Generating test code with OpenAI for prompt: {Prompt}", prompt.Substring(0, Math.Min(50, prompt.Length)));

                var model = _configuration["LLMProviders:OpenAI:DefaultModel"] ?? "gpt-4-turbo-preview";

                // Fix: Parse string to int with default value
                var maxTokensString = _configuration["LLMProviders:OpenAI:MaxTokens"];
                var maxTokens = string.IsNullOrEmpty(maxTokensString) ? 4096 : int.Parse(maxTokensString);

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                new { role = "system", content = "You are a testing expert. Generate high-quality test code." },
                new { role = "user", content = $"{context}\n\n{prompt}" }
            },
                    max_tokens = maxTokens,
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("chat/completions", content);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseBody);
                var result = jsonResponse.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test code with OpenAI");
                throw new LLMCoordinationException($"OpenAI generation failed: {ex.Message}", ProviderName);
            }
        }
      
        public bool CanHandleProvider(string providerName)
        {
            return providerName?.ToLower() == "chatgpt" ||
                   providerName?.ToLower() == "openai" ||
                   providerName?.ToLower() == "gpt";
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("models");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}