// Models/Landscape/ApplicationProfile.cs (enhanced)
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Chapter_1.Models
{
   

    public class ApplicationProfile
    {
        [Required(ErrorMessage = "Application name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Architecture type is required")]
        public string ArchitectureType { get; set; } = "microservices"; // "monolith", "serverless", "hybrid"

        public string[] FrontendFrameworks { get; set; } = Array.Empty<string>();

        [Range(1, 1000, ErrorMessage = "Backend services count must be between 1 and 1000")]
        public int BackendServicesCount { get; set; }

        public BackendService[] BackendServices { get; set; } = Array.Empty<BackendService>();

        public string[] DataSources { get; set; } = new[] { "database", "external-apis" };

        [Required(ErrorMessage = "Expected user scale is required")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserScale ExpectedUsers { get; set; } = UserScale.Medium;

        [Required(ErrorMessage = "At least one critical user journey is required")]
        [MinLength(1, ErrorMessage = "Specify at least one critical user journey")]
        public string[] CriticalUserJourneys { get; set; } = Array.Empty<string>();

        public Dictionary<string, string> TechDebtAreas { get; set; } = new();

        // Additional properties
        public string DeploymentEnvironment { get; set; } = "cloud"; // cloud, on-premise, hybrid
        public string[] ComplianceRequirements { get; set; } = Array.Empty<string>(); // HIPAA, GDPR, PCI-DSS, etc.
        public decimal AvailabilityRequirement { get; set; } = 99.9m; // as percentage
        public Dictionary<string, string> PerformanceSLAs { get; set; } = new();
        public string[] SecurityRequirements { get; set; } = Array.Empty<string>();
    }

}