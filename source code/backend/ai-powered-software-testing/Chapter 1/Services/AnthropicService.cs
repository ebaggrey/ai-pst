using Chapter_1.Exceptions;
using Chapter_1.Models;
using Chapter_1.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Chapter_1.Services
{
    public class AnthropicService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AnthropicService> _logger;

        public string ProviderName => "claude";

        public AnthropicService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AnthropicService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("Anthropic");
            _configuration = configuration;
            _logger = logger;
        }

        // AnthropicService.cs - Fixed
        public async Task<string> GenerateTestCodeAsync(string prompt, string context)
        {
            try
            {
                _logger.LogInformation("Generating test code with Anthropic for prompt: {Prompt}", prompt.Substring(0, Math.Min(50, prompt.Length)));

                var model = _configuration["LLMProviders:Anthropic:DefaultModel"] ?? "claude-3-sonnet-20240229";

                // Fix: Parse string to int with default value
                var maxTokensString = _configuration["LLMProviders:Anthropic:MaxTokens"];
                var maxTokens = string.IsNullOrEmpty(maxTokensString) ? 8192 : int.Parse(maxTokensString);

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                new { role = "user", content = $"{context}\n\n{prompt}" }
            },
                    max_tokens = maxTokens,
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("messages", content);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseBody);
                var result = jsonResponse.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString();

                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test code with Anthropic");
                throw new LLMCoordinationException($"Anthropic generation failed: {ex.Message}", ProviderName);
            }
        }

     
        public bool CanHandleProvider(string providerName)
        {
            return providerName?.ToLower() == "claude" ||
                   providerName?.ToLower() == "anthropic";
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