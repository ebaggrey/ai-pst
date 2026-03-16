
// Models/Requests/ShiftRightRequest.cs
namespace Chapter_11.Models.Requests
{
    public class ShiftRightRequest
    {
        public ProductionSystem ProductionSystem { get; set; }
        public UserBehavior UserBehavior { get; set; }
        public MonitoringObjective[] MonitoringObjectives { get; set; }
        public FeedbackLoop[] FeedbackLoops { get; set; }
    }

    public class ProductionSystem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ProductionComponent[] Components { get; set; }
        public DeploymentEnvironment Environment { get; set; }
    }

    public class ProductionComponent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string[] Dependencies { get; set; }
    }

    public class UserBehavior
    {
        public UserPattern[] Patterns { get; set; }
        public UserSegment[] Segments { get; set; }
    }

    public class UserPattern
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Frequency { get; set; }
    }

    public class UserSegment
    {
        public string Id { get; set; }
        public string Criteria { get; set; }
    }

    public class MonitoringObjective
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Metric { get; set; }
        public double Threshold { get; set; }
    }

    public class FeedbackLoop
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SourceComponent { get; set; }
        public string TargetComponent { get; set; }
    }

    public enum DeploymentEnvironment
    {
        Development,
        Staging,
        Production,
        Canary
    }
}
