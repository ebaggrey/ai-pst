namespace Chapter_5.Helpers
{
    using Chapter_5.Models.Domain;
    using Chapter_5.Models.Requests;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class TDDControllerHelpers
    {
        public static TimeSpan EstimateTDDCycleTime(GeneratedTest failingTest, ImplementationSuggestion[] implementations, TDDRequest request)
        {
            // Simple estimation logic
            var baseTime = TimeSpan.FromMinutes(30);
            var testComplexity = failingTest?.TestCode?.Split('\n').Length / 10.0 ?? 1.0;
            var implementationCount = implementations?.Length ?? 1;

            return TimeSpan.FromMinutes(baseTime.TotalMinutes * testComplexity * (1 + implementationCount * 0.5));
        }

        public static ConfidenceMetrics CalculateConfidenceMetrics(GeneratedTest failingTest, ImplementationSuggestion[] implementations)
        {
            var metrics = new ConfidenceMetrics
            {
                TestQuality = CalculateTestQuality(failingTest),
                ImplementationQuality = CalculateImplementationQuality(implementations),
                OverallConfidence = 0.7 // Default value
            };

            metrics.RefactoringSafety = metrics.OverallConfidence * 0.9;
            metrics.OverallConfidence = (metrics.TestQuality + metrics.ImplementationQuality + metrics.RefactoringSafety) / 3.0;

            metrics.ConfidenceFactors = new[]
            {
            $"Test quality: {metrics.TestQuality:P0}",
            $"Implementation quality: {metrics.ImplementationQuality:P0}",
            $"Refactoring safety: {metrics.RefactoringSafety:P0}"
        };

            metrics.RiskFactors = new[] { "New implementation", "No production history" };

            return metrics;
        }

        public static LearningPoint[] ExtractLearningPoints(TDDRequest request, GeneratedTest failingTest)
        {
            var points = new List<LearningPoint>();

            points.Add(new LearningPoint
            {
                Category = "tdd",
                Title = "Test-First Approach",
                Description = "Writing tests before implementation ensures requirements are clear",
                Impact = "high"
            });

            if (failingTest?.TestCode?.Length > 100)
            {
                points.Add(new LearningPoint
                {
                    Category = "testing",
                    Title = "Test Complexity",
                    Description = "Complex tests may indicate need for smaller units",
                    Impact = "medium"
                });
            }

            if (request?.UserStory?.AcceptanceCriteria?.Length > 3)
            {
                points.Add(new LearningPoint
                {
                    Category = "requirements",
                    Title = "Multiple Acceptance Criteria",
                    Description = "Consider breaking down into smaller user stories",
                    Impact = "medium"
                });
            }

            return points.ToArray();
        }

        public static double CalculateTestQuality(GeneratedTest test)
        {
            if (test == null || string.IsNullOrEmpty(test.TestCode))
                return 0.1;

            var hasAssertions = test.TestCode.Contains("Assert") || test.TestCode.Contains("Should()");
            var hasArrangeActAssert = test.TestCode.Contains("// Arrange") && test.TestCode.Contains("// Act") && test.TestCode.Contains("// Assert");
            var hasClearName = !string.IsNullOrEmpty(test.TestName) && test.TestName.Length > 5;

            var score = 0.0;
            if (hasAssertions) score += 0.4;
            if (hasArrangeActAssert) score += 0.3;
            if (hasClearName) score += 0.3;

            return Math.Min(1.0, score);
        }

        public static double CalculateImplementationQuality(ImplementationSuggestion[] implementations)
        {
            if (implementations == null || implementations.Length == 0)
                return 0.1;

            var avgComplexity = implementations.Average(i => i.CodeSnippet?.ComplexityMetrics?.CyclomaticComplexity ?? 1);
            var quality = Math.Max(0.1, 1.0 / (avgComplexity * 0.5));

            return Math.Min(1.0, quality);
        }

        public static double CalculateQualityScore(CodeSnippet implementation, ImplementationAnalysis analysis, TestResult[] testResults)
        {
            var testScore = testResults?.All(r => r.Passed) == true ? 1.0 : 0.3;
            var analysisScore = analysis?.CodeQuality ?? 0.5;
            var complexityPenalty = implementation?.ComplexityMetrics?.CyclomaticComplexity > 5 ? 0.2 : 1.0;

            return (testScore * 0.4 + analysisScore * 0.4 + (complexityPenalty * 0.2)) * 100;
        }

        public static ImprovementMetrics CalculateImprovementMetrics(CodeAnalysis currentAnalysis, CodeAnalysis finalAnalysis)
        {
            return new ImprovementMetrics
            {
                OverallImprovement = finalAnalysis.MaintainabilityIndex - currentAnalysis.MaintainabilityIndex,
                MaintainabilityGain = finalAnalysis.MaintainabilityIndex - currentAnalysis.MaintainabilityIndex,
                ReadabilityGain = 0.1,
                PerformanceChange = 0.05,
                ComplexityReduction = currentAnalysis.CyclomaticComplexity - finalAnalysis.CyclomaticComplexity,
                LinesOfCodeChange = currentAnalysis.LinesOfCode - finalAnalysis.LinesOfCode
            };
        }

        public static ConfidenceSummary CalculateConfidenceSummary(ChangePrediction[] predictions, double threshold)
        {
            var highConfidence = predictions?.Count(p => p.Confidence >= 0.8) ?? 0;
            var mediumConfidence = predictions?.Count(p => p.Confidence >= 0.6 && p.Confidence < 0.8) ?? 0;
            var lowConfidence = predictions?.Count(p => p.Confidence < 0.6) ?? 0;
            var average = predictions?.Any() == true ? predictions.Average(p => p.Confidence) : 0;

            return new ConfidenceSummary
            {
                AverageConfidence = average,
                HighConfidencePredictions = highConfidence,
                MediumConfidencePredictions = mediumConfidence,
                LowConfidencePredictions = lowConfidence,
                OverallReliability = average >= 0.8 ? "high" : average >= 0.6 ? "medium" : "low"
            };
        }

        public static ImplementationTimeline CreateImplementationTimeline(List<FutureTestRecommendation> futureTests, TimeHorizon horizon)
        {
            var phases = new[]
            {
            new Phase
            {
                Name = "Analysis",
                Description = "Analyze requirements and design tests",
                StartDate = DateTimeOffset.UtcNow,
                EndDate = DateTimeOffset.UtcNow.AddDays(7),
                Status = "planned"
            },
            new Phase
            {
                Name = "Implementation",
                Description = "Implement core functionality",
                StartDate = DateTimeOffset.UtcNow.AddDays(7),
                EndDate = DateTimeOffset.UtcNow.AddDays(21),
                Status = "planned"
            },
            new Phase
            {
                Name = "Testing",
                Description = "Execute and refine tests",
                StartDate = DateTimeOffset.UtcNow.AddDays(21),
                EndDate = DateTimeOffset.UtcNow.AddDays(28),
                Status = "planned"
            }
        };

            return new ImplementationTimeline
            {
                Phases = phases,
                StartDate = phases.First().StartDate,
                EndDate = phases.Last().EndDate,
                TotalDuration = phases.Last().EndDate - phases.First().StartDate,
                Dependencies = Array.Empty<Dependency>(),
                Risks = new[]
                {
                new Risk
                {
                    Description = "Requirements may change during implementation",
                    Impact = "medium",
                    Probability = "medium",
                    MitigationStrategy = "Regular stakeholder reviews",
                    Owner = "Product Owner"
                }
            }
            };
        }
    }
}
