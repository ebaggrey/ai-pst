using Chapter_4.Models.Domain;

namespace Chapter_4.Services.Interfaces
{
    // Interfaces/IAICapabilityAssessor.cs
    public interface IAICapabilityAssessor
    {
        Task<AICapabilityReport> AssessCapabilitiesAsync(AIAssessmentRequest request);
    }

    // Interfaces/IAIRobustnessTester.cs
    public interface IAIRobustnessTester
    {
        Task<RobustnessTestReport> TestRobustnessAsync(RobustnessTestRequest request);
    }

    // Interfaces/IAIBiasDetector.cs
    public interface IAIBiasDetector
    {
        Task<BiasDetectionReport> DetectBiasAsync(BiasDetectionRequest request);
    }

    // Interfaces/IAIHallucinationDetector.cs
    public interface IAIHallucinationDetector
    {
        Task<HallucinationDetectionReport> DetectHallucinationsAsync(HallucinationTestRequest request);
    }

    // Interfaces/IAIDriftMonitor.cs
    public interface IAIDriftMonitor
    {
        Task<DriftDetectionReport> MonitorDriftAsync(DriftDetectionRequest request);
    }
}
