using Chapter_5.Models.Domain;

namespace Chapter_5.Helpers
{
    // Add this to the TDDControllerHelpers or create a separate service
    public static class RoadmapAnalyzer
    {
        public static async Task<RoadmapAnalysis> AnalyzeRoadmapAsync(ProductRoadmap roadmap)
        {
            if (roadmap == null)
                return null;

            var analysis = new RoadmapAnalysis
            {
                Roadmap = roadmap,
                Features = roadmap.Features,
                HighPriorityFeatures = roadmap.Features?
                    .Where(f => f.Priority == "high" || f.Priority == "critical")
                    .Select(f => f.Title)
                    .ToArray() ?? Array.Empty<string>(),
                TechnicalRequirements = ExtractTechnicalRequirements(roadmap),
                Dependencies = ExtractDependencies(roadmap),
                EstimatedCompletion = roadmap.EndDate,
                RiskAreas = IdentifyRiskAreas(roadmap)
            };

            return await Task.FromResult(analysis);
        }

        private static string[] ExtractTechnicalRequirements(ProductRoadmap roadmap)
        {
            var requirements = new List<string>();

            foreach (var feature in roadmap.Features ?? Array.Empty<RoadmapFeature>())
            {
                if (feature.Description.Contains("API", StringComparison.OrdinalIgnoreCase))
                    requirements.Add("API development");

                if (feature.Description.Contains("database", StringComparison.OrdinalIgnoreCase) ||
                    feature.Description.Contains("DB", StringComparison.OrdinalIgnoreCase))
                    requirements.Add("Database changes");

                if (feature.Description.Contains("performance", StringComparison.OrdinalIgnoreCase))
                    requirements.Add("Performance optimization");

                if (feature.Description.Contains("security", StringComparison.OrdinalIgnoreCase))
                    requirements.Add("Security implementation");
            }

            return requirements.Distinct().ToArray();
        }

        private static string[] ExtractDependencies(ProductRoadmap roadmap)
        {
            var dependencies = new List<string>();

            foreach (var feature in roadmap.Features ?? Array.Empty<RoadmapFeature>())
            {
                if (feature.Dependencies != null)
                {
                    dependencies.AddRange(feature.Dependencies);
                }
            }

            return dependencies.Distinct().ToArray();
        }

        private static string[] IdentifyRiskAreas(ProductRoadmap roadmap)
        {
            var risks = new List<string>();

            var highPriorityCount = roadmap.Features?.Count(f => f.Priority == "high" || f.Priority == "critical") ?? 0;
            if (highPriorityCount > 5)
                risks.Add("Too many high priority features");

            var longDurationFeatures = roadmap.Features?.Where(f =>
                (f.TargetDate - roadmap.StartDate).TotalDays > 90).Count() ?? 0;
            if (longDurationFeatures > 3)
                risks.Add("Multiple long-running features");

            var featuresWithoutDeps = roadmap.Features?.Where(f =>
                f.Dependencies == null || f.Dependencies.Length == 0).Count() ?? 0;
            if (featuresWithoutDeps < roadmap.Features?.Length / 2)
                risks.Add("High feature interdependency");

            return risks.ToArray();
        }
}
}
