using Chapter_2.Services.Interfaces;

namespace Chapter_2.Services
{
    public class StrategyBasedLLMServiceFactory : ILLMServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _config;
        private readonly ILogger<StrategyBasedLLMServiceFactory> _logger;
        private readonly Dictionary<string, ILLMService> _services;

        public StrategyBasedLLMServiceFactory(
            IServiceProvider serviceProvider,
            IConfiguration config,
            ILogger<StrategyBasedLLMServiceFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _logger = logger;
            _services = InitializeServices();
        }

        public ILLMService GetService(string providerName)
        {
            if (_services.TryGetValue(providerName.ToLower(), out var service))
            {
                return service;
            }

            _logger.LogWarning("Requested unknown LLM provider: {Provider}", providerName);

            // Fallback to default
            return _services["chatgpt"];
        }

        public ILLMService GetServiceForTask(string task, string strategy, string context)
        {
            var mapping = _config.GetSection("Onboarding:TaskToLLMMapping")
                .Get<Dictionary<string, string>>() ?? new Dictionary<string, string>();

            // Check for exact task match
            if (mapping.TryGetValue(task.ToLower(), out var provider))
            {
                return GetService(provider);
            }

            // Strategy-based selection
            return (task.ToLower(), strategy.ToLower()) switch
            {
                ("fix-flaky-test", "conservative") => GetService("claude"),
                ("fix-flaky-test", "aggressive") => GetService("deepseek"),
                ("learning-path", _) => GetService("chatgpt"),
                ("generate-questions", "collaborative") => GetService("claude"),
                ("generate-questions", "technical") => GetService("deepseek"),
                ("code-analysis", _) => GetService("gemini"),
                _ => GetService("chatgpt") // Default fallback
            };
        }

        public IEnumerable<ILLMService> GetAllServices()
        {
            return _services.Values;
        }

        public async Task<ProviderHealth> CheckProviderHealthAsync(string providerName)
        {
            var service = GetService(providerName);

            try
            {
                // Simple health check - try to generate a small test
                var test = await service.GenerateTestCodeAsync("health check", "health");
                return new ProviderHealth
                {
                    ProviderName = providerName,
                    IsHealthy = !string.IsNullOrEmpty(test),
                    ResponseTime = 100, // ms
                    LastChecked = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for {Provider}", providerName);
                return new ProviderHealth
                {
                    ProviderName = providerName,
                    IsHealthy = false,
                    Error = ex.Message,
                    LastChecked = DateTime.UtcNow
                };
            }
        }

        private Dictionary<string, ILLMService> InitializeServices()
        {
            var services = new Dictionary<string, ILLMService>();
            var allServices = _serviceProvider.GetServices<ILLMService>();

            foreach (var service in allServices)
            {
                services[service.ProviderName.ToLower()] = service;
            }

            return services;
        }
    }

    public class ProviderHealth
    {
        public string ProviderName { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public long ResponseTime { get; set; } // ms
        public string? Error { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
