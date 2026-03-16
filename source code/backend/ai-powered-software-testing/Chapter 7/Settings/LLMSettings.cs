namespace Chapter_7.Settings
{
    
    public class LLMSettings
    {
        public string Provider { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string Model { get; set; }
        public string SystemPrompt { get; set; }
        public double Temperature { get; set; }
        public int MaxTokens { get; set; }
    }
}
