
using System.Text;
using System.Text.Json;
using Chapter_9.Settings;
using Microsoft.Extensions.Options;

namespace Chapter_9.Services.LLM
{
    public interface ILLMService
    {
        Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
        Task<T> GenerateStructuredAsync<T>(string prompt, CancellationToken cancellationToken = default);
    }

    public class OllamaLLMService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly LLMSettings _settings;
        private readonly ILogger<OllamaLLMService> _logger;

        public OllamaLLMService(
            IHttpClientFactory httpClientFactory,
            IOptions<LLMSettings> settings,
            ILogger<OllamaLLMService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("LLMClient");
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
        {
            if (!_settings.Enabled)
            {
                _logger.LogWarning("LLM is disabled in configuration");
                return string.Empty;
            }

            try
            {
                var request = new
                {
                    model = _settings.ModelName,
                    prompt = prompt,
                    stream = false,
                    options = new
                    {
                        temperature = _settings.Temperature,
                        num_predict = _settings.MaxTokens
                    }
                };

                var response = await _httpClient.PostAsync(
                    "/api/generate",
                    new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"),
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(jsonResponse);

                return doc.RootElement.GetProperty("response").GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling LLM service");
                throw;
            }
        }

        public async Task<T> GenerateStructuredAsync<T>(string prompt, CancellationToken cancellationToken = default)
        {
            var jsonPrompt = $"{prompt}\n\nProvide the response as a valid JSON object.";
            var response = await GenerateAsync(jsonPrompt, cancellationToken);

            try
            {
                return JsonSerializer.Deserialize<T>(response);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse LLM response as {Type}", typeof(T).Name);
                throw;
            }
        }
    }
}
