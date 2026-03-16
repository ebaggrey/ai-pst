
namespace Chapter_7.Models.Requests;

public class PipelineGenerationRequest
{
    public CodebaseAnalysis CodebaseAnalysis { get; set; }
    public PipelineConstraints Constraints { get; set; }
    public TeamPractices TeamPractices { get; set; }
    public OptimizationFocus OptimizationFocus { get; set; }
}

public class CodebaseAnalysis
{
    public string Language { get; set; }
    public double TestCoverage { get; set; }
    public int TotalLines { get; set; }
    public string[] Dependencies { get; set; }
}

public class PipelineConstraints
{
    public string MaxDuration { get; set; }
    public double MaxCostPerRun { get; set; }
}

public class TeamPractices
{
    public bool CodeReviews { get; set; }
    public bool AutomatedTesting { get; set; }
    public string[] DeploymentStrategy { get; set; }
}

public class OptimizationFocus
{
    public bool Speed { get; set; }
    public bool Reliability { get; set; }
    public bool Cost { get; set; }
}