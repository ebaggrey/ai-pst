
using global::Introduction.Controllers;
using global::Introduction.Exceptions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Introduction.Services
{
        public class ClaudeService : ILLMService
        {
            private readonly HttpClient _httpClient;
            private readonly IConfiguration _config;
            private readonly ILogger<ClaudeService> _logger;

            public ClaudeService(
                HttpClient httpClient,
                IConfiguration config,
                ILogger<ClaudeService> logger)
            {
                _httpClient = httpClient;
                _config = config;
                _logger = logger;
            }

            public string ProviderName => "claude";

            public bool CanHandleProvider(string providerName) =>
                providerName.Equals("claude", StringComparison.OrdinalIgnoreCase);

            public async Task<string> GenerateTestCodeAsync(string prompt, string context)
            {
                var apiKey = _config["LLMProviders:Claude:ApiKey"];
                var endpoint = _config["LLMProviders:Claude:Endpoint"];

                var request = new
                {
                    model = "claude-3-opus-20240229",
                    max_tokens = 4000,
                    messages = new[]
                    {
                    new {
                        role = "system",
                        content = "You are an expert test automation engineer. Write clean, maintainable test code."
                    },
                    new {
                        role = "user",
                        content = $"{context}\n\n{prompt}"
                    }
                }
                };

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);
                _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                var response = await _httpClient.PostAsJsonAsync(endpoint, request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Claude API failed with status {StatusCode}", response.StatusCode);
                    throw new AIProviderException($"Claude service unavailable: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ClaudeResponse>(content);

                return result?.Content?.FirstOrDefault()?.Text?.Trim()
                       ?? "// Sorry, the AI returned an empty response. Try again?";
            }

            private class ClaudeResponse
            {
                public List<Content> Content { get; set; } = new();
            }

            private class Content
            {
                public string Text { get; set; } = string.Empty;
            }
        }
    
}
