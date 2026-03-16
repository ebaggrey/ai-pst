
namespace Chapter_11.Models.Requests
{
    public class TestabilityRequest
    {
        public Codebase Codebase { get; set; }
        public TestabilityFramework TestabilityFramework { get; set; }
        public AnalysisDepth AnalysisDepth { get; set; }
        public bool ImprovementSuggestions { get; set; }
        public bool RefactoringRecommendations { get; set; }
    }

    public class Codebase
    {
        public string RepositoryUrl { get; set; }
        public string Branch { get; set; }
        public int TotalLines { get; set; }
        public CodeFile[] Files { get; set; }
    }

    public class CodeFile
    {
        public string Path { get; set; }
        public string Content { get; set; }
        public string Language { get; set; }
    }

    public class TestabilityFramework
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string[] SupportedLanguages { get; set; }
    }

    public enum AnalysisDepth
    {
        Basic,
        Detailed,
        Comprehensive
    }
}