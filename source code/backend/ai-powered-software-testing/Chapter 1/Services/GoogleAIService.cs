using Chapter_1.Exceptions;
using Chapter_1.Models;
using Chapter_1.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Chapter_1.Services
{
    public class GoogleAIService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleAIService> _logger;
        private readonly string _apiKey;

        public string ProviderName => "gemini";

        public GoogleAIService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<GoogleAIService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("GoogleAI");
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["LLMProviders:GoogleAI:ApiKey"] ?? string.Empty;
        }

        public async Task<string> GenerateTestCodeAsync(string prompt, string context)
        {
            try
            {
                _logger.LogInformation("Generating test code with Google AI for prompt: {Prompt}", prompt.Substring(0, Math.Min(50, prompt.Length)));

                var model = _configuration["LLMProviders:GoogleAI:Model"] ?? "gemini-pro";
                var url = $"{model}:generateContent?key={_apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = $"{context}\n\n{prompt}" }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 2048
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseBody);

                var result = jsonResponse.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test code with Google AI");
                throw new LLMCoordinationException($"Google AI generation failed: {ex.Message}", ProviderName);
            }
        }

        public bool CanHandleProvider(string providerName)
        {
            return providerName?.ToLower() == "gemini" ||
                   providerName?.ToLower() == "google" ||
                   providerName?.ToLower() == "googleai";
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var model = _configuration["LLMProviders:GoogleAI:Model"] ?? "gemini-pro";
                var url = $"{model}?key={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}