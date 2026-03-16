
// Models/Requests/ShiftLeftRequest.cs
namespace Chapter_11.Models.Requests
{
    public class ShiftLeftRequest
    {
        public RequirementCollection Requirements { get; set; }
        public DesignDocument[] DesignDocuments { get; set; }
        public int ShiftDepth { get; set; }
        public CollaborationMode CollaborationMode { get; set; }
    }

    public class RequirementCollection
    {
        public Requirement[] Items { get; set; }
        public Stakeholder[] Stakeholders { get; set; }
    }

    public class Requirement
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Testability { get; set; }
        public string[] AcceptanceCriteria { get; set; }
    }

    public class Stakeholder
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class DesignDocument
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string DocumentType { get; set; }
    }

    public enum CollaborationMode
    {
        Synchronous,
        Asynchronous,
        Hybrid
    }
}