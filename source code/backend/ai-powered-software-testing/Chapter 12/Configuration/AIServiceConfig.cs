
// Configuration/AIServiceConfig.cs
namespace Chapter_12.Configuration
{
    public class AIServiceConfig
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string ModelName { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
        public int MaxRetries { get; set; } = 3;
        public Dictionary<string, object> DefaultParameters { get; set; }
    }

    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; } = 30;
        public bool EnableSensitiveDataLogging { get; set; } = false;
    }
}