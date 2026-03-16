namespace Chapter_8.Services.LLM
{
    public interface ILLMService
    {
        Task<string> GenerateContentAsync(string prompt);
        Task<T> GenerateStructuredContentAsync<T>(string prompt) where T : class;
    }
}


