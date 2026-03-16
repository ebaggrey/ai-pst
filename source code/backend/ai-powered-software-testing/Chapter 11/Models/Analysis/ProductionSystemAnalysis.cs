// Models/Analysis/ProductionSystemAnalysis.cs
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;

namespace Chapter_11.Models.Analysis
{
    public class ProductionSystemAnalysis
    {
        public string Id { get; set; }
        public ProductionComponent[] Components { get; set; }
        public DependencyGraph DependencyGraph { get; set; }
        public PerformanceMetrics PerformanceMetrics { get; set; }
        public string[] IdentifiedPatterns { get; set; }
    }

    public class DependencyGraph
    {
        public DependencyNode[] Nodes { get; set; }
        public DependencyEdge[] Edges { get; set; }
    }

    public class DependencyNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class DependencyEdge
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public string Relationship { get; set; }
    }
}
