
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;

namespace Chapter_8.Interfaces
{
    public interface IModernizationPlanner
    {
        Task<ModernizationRoadmap> CreateRoadmapAsync(
            ModernizationOption[] modernizationOptions,
            BusinessPriority[] businessPriorities,
            PipelineConstraints constraints,
            SuccessMetric[] successMetrics);
    }

    public class ModernizationOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Effort { get; set; }
        public double Impact { get; set; }
        public double Risk { get; set; }
        public string[] Prerequisites { get; set; }
    }
}