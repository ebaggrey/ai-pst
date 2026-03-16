using Chapter_1.Models;

namespace Chapter_1.Services.Interfaces
{
    // Services/Interfaces/ILandscapeAnalyzer.cs - Complete
    public interface ILandscapeAnalyzer
    {
        Task<ArchitectureAnalysis> AnalyzeAsync(ApplicationProfile profile);
        Task<TestabilityReport> AssessTestabilityAsync(ArchitectureAnalysis analysis);
        Task<ComplexityAssessment> CalculateComplexityAsync(ApplicationProfile profile);
        Task<RiskAssessment> AssessRisksAsync(ApplicationProfile profile);
    }

    // Services/Interfaces/ILLMOrchestrator.cs
    public interface ILLMOrchestrator
    {
        Task<Dictionary<string, LLMResult>> OrchestrateAnalysisAsync(Dictionary<string, string> prompts);
        Task<LLMResult> ProcessSinglePromptAsync(string area, string prompt, string? providerName = null);
    }
    // Services/Interfaces/ILLMService.cs
    public interface ILLMService
    {
        Task<string> GenerateTestCodeAsync(string prompt, string context);
        bool CanHandleProvider(string providerName);
        string ProviderName { get; }
        Task<bool> IsAvailableAsync();
    }

    // Services/Interfaces/IArchitecturePlugin.cs
    public interface IArchitecturePlugin
    {
        Task<PluginAnalysisResult> AnalyzeAsync(ApplicationProfile profile);
        bool CanAnalyze(ApplicationProfile profile);
        string PluginName { get; }
    }

    // Models/Landscape/PluginAnalysisResult.cs
    public class PluginAnalysisResult
    {
        public string PluginName { get; set; } = string.Empty;
        public List<IntegrationPoint> IntegrationPoints { get; set; } = new();
        public int ExternalDependencies { get; set; }
        public decimal ComplexityContribution { get; set; }
        public List<string> Findings { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    // Models/Landscape/LLMResult.cs
    public class LLMResult
    {
        public string Content { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public int TokenCount { get; set; }
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    // Services/Interfaces/ITestSynthesisService.cs
    public interface ITestSynthesisService
    {
        Task<TestLandscapeResponse> SynthesizeAsync(
            ArchitectureAnalysis analysis,
            Dictionary<string, LLMResult> llmResults,
            LandscapeTestRequest request);
    }
}
