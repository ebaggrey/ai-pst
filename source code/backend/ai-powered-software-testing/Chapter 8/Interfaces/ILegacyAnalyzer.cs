
using Chapter_8.Models.Requests;

namespace Chapter_8.Interfaces
{
    public interface ILegacyAnalyzer
    {
        Task<CodebaseAnalysis> AnalyzeCodebaseAsync(LegacyAnalysisRequest request);
    }

    // Supporting models for analysis
    public class CodebaseAnalysis
    {
        public string AnalysisId { get; set; }
        public CodeStructure Structure { get; set; }
        public ComplexityMetrics Complexity { get; set; }
        public DependencyGraph Dependencies { get; set; }
        public CodeSmell[] CodeSmells { get; set; }
        public Dictionary<string, object> RawAnalysis { get; set; }
    }

    public class CodeStructure
    {
        public Module[] Modules { get; set; }
        public Class[] Classes { get; set; }
        public Method[] Methods { get; set; }
    }

    public class Module
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string[] Classes { get; set; }
    }

    public class Class
    {
        public string Name { get; set; }
        public string Module { get; set; }
        public int MethodCount { get; set; }
        public int ComplexityScore { get; set; }
    }

    public class Method
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public int LinesOfCode { get; set; }
        public int CyclomaticComplexity { get; set; }
    }

    public class ComplexityMetrics
    {
        public double AverageCyclomaticComplexity { get; set; }
        public int MaxCyclomaticComplexity { get; set; }
        public double CouplingScore { get; set; }
        public double CohesionScore { get; set; }
        public Dictionary<string, double> Metrics { get; set; }
    }

    public class DependencyGraph
    {
        public Dependency[] InternalDependencies { get; set; }
        public Dependency[] ExternalDependencies { get; set; }
        public string[] CircularDependencies { get; set; }
    }

    public class CodeSmell
    {
        public string Type { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int Severity { get; set; }
    }
}
