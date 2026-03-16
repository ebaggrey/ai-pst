using Chapter_5.Exceptions;
using Chapter_5.Interfaces;
using Chapter_5.Models.Domain;
using Chapter_5.Models.Requests;
using System.Text;

namespace Chapter_5.Services.Implementations
{
    // Services/TDDOrchestrator.cs
    public class TDDOrchestrator : ITDDOrchestrator
    {
        private readonly ILogger<TDDOrchestrator> _logger;

        public TDDOrchestrator(ILogger<TDDOrchestrator> logger)
        {
            _logger = logger;
        }

        public async Task<TestResult[]> RunVirtualTestsAsync(CodeSnippet implementation, GeneratedTest test)
        {
            _logger.LogInformation("Running virtual tests for implementation {ImplementationId}", implementation.Id);

            // Simulate test execution
            var results = new List<TestResult>
        {
            new TestResult
            {
                TestName = test.TestName,
                Passed = implementation.Code.Contains("throw new NotImplementedException()") ? false : true,
                Duration = TimeSpan.FromMilliseconds(new Random().Next(50, 500)),
                ErrorMessage = implementation.Code.Contains("throw new NotImplementedException()")
                    ? "NotImplementedException thrown"
                    : null
            }
        };

            await Task.Delay(100); // Simulate async work

            return results.ToArray();
        }

        public async Task<TDDCycle> CreateNextCycleAsync(TestedImplementation implementation, ImplementationRequest request)
        {
            return new TDDCycle
            {
                Id = Guid.NewGuid().ToString(),
                Phase = "refactor",
                Description = "Refactor the working implementation",
                EstimatedDuration = TimeSpan.FromMinutes(15),
                FocusAreas = new[] { "code-smells", "duplication" }
            };
        }

        public async Task<TestValidation> ValidateTestIsActuallyFailingAsync(GeneratedTest test, TDDRequest request)
        {
            var validation = new TestValidation
            {
                Test = test,
                IsFailingByDesign = test.IsFailingByDesign,
                FailureDetails = test.ExpectedFailure,
                Issues = new List<string>()
            };

            if (string.IsNullOrEmpty(test.TestCode))
            {
                validation.Issues.Add("Test code is empty");
                validation.IsFailingByDesign = false;
            }

            if (!test.TestCode.Contains("Assert"))
            {
                validation.Issues.Add("Test has no assertions");
                validation.IsFailingByDesign = false;
            }

            return await Task.FromResult(validation);
        }
    }

    // Services/TestFirstGenerator.cs
    public class TestFirstGenerator : ITestFirstGenerator
    {
        private readonly ILogger<TestFirstGenerator> _logger;

        public TestFirstGenerator(ILogger<TestFirstGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<GeneratedTest> GenerateFailingTestAsync(TDDRequest request)
        {
            if (request.UserStory == null)
                throw new TestGenerationException("User story is required");

            _logger.LogInformation("Generating failing test for story: {StoryTitle}", request.UserStory.Title);

            var testCode = GenerateTestCode(request);

            return await Task.FromResult(new GeneratedTest
            {
                TestCode = testCode,
                TestFramework = "xUnit",
                TestName = $"Test_{request.UserStory.Id.Replace("-", "_")}",
                Dependencies = new[] { "xunit", "Moq", "FluentAssertions" },
                IsFailingByDesign = true,
                ExpectedFailure = new FailureDetails
                {
                    Expected = "Expected value from acceptance criteria",
                    Actual = "null or NotImplementedException",
                    Message = "Test should fail because implementation doesn't exist"
                }
            });
        }

        //public async Task<GeneratedTest> AdjustTestToFailAsync(GeneratedTest test, TestValidation validation)
        //{
        //    _logger.LogDebug("Adjusting test to ensure it fails properly");

        //    var adjustedTest = test with
        //    {
        //        TestCode = EnsureTestFails(test.TestCode),
        //        IsFailingByDesign = true
        //    };

        //    return await Task.FromResult(adjustedTest);
        //}

        public async Task<GeneratedTest> AdjustTestToFailAsync(GeneratedTest test, TestValidation validation)
        {
            _logger.LogDebug("Adjusting test to ensure it fails properly");

            // Create a new instance with adjusted properties
            var adjustedTest = new GeneratedTest
            {
                TestCode = EnsureTestFails(test.TestCode),
                TestFramework = test.TestFramework,
                TestName = test.TestName,
                Dependencies = test.Dependencies,
                IsFailingByDesign = true,
                ExpectedFailure = test.ExpectedFailure
            };

            return await Task.FromResult(adjustedTest);
        }

        public async Task<TestValidation> ValidateTestAsync(GeneratedTest test)
        {
            var validation = new TestValidation
            {
                Test = test,
                IsFailingByDesign = test.IsFailingByDesign,
                Issues = new List<string>()
            };

            if (!test.TestCode.Contains("[Fact]") && !test.TestCode.Contains("[Test]"))
            {
                validation.Issues.Add("Test method attribute missing");
            }

            return await Task.FromResult(validation);
        }

        private string GenerateTestCode(TDDRequest request)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using Xunit;");
            sb.AppendLine("using FluentAssertions;");
            sb.AppendLine();
            sb.AppendLine("namespace GeneratedTests;");
            sb.AppendLine();
            sb.AppendLine($"public class {request.UserStory.Id.Replace("-", "")}Tests");
            sb.AppendLine("{");
            sb.AppendLine("    [Fact]");
            sb.AppendLine($"    public void {request.UserStory.Title.Replace(" ", "_")}_Should_Work_Correctly()");
            sb.AppendLine("    {");
            sb.AppendLine("        // Arrange");
            sb.AppendLine("        var sut = new SystemUnderTest();");
            sb.AppendLine();
            sb.AppendLine("        // Act");
            sb.AppendLine("        var result = sut.DoSomething();");
            sb.AppendLine();
            sb.AppendLine("        // Assert");
            sb.AppendLine("        result.Should().NotBeNull();");
            sb.AppendLine("        // This test will fail until implementation is complete");
            sb.AppendLine("        throw new System.NotImplementedException();");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string EnsureTestFails(string testCode)
        {
            if (!testCode.Contains("throw new") && !testCode.Contains("Assert.Fail"))
            {
                return testCode.Replace("// Assert", "// Assert\n        throw new System.NotImplementedException(\"Test must fail initially\");");
            }

            return testCode;
        }
    }

    // Services/ImplementationGenerator.cs
    public class ImplementationGenerator : IImplementationGenerator
    {
        private readonly ILogger<ImplementationGenerator> _logger;

        public ImplementationGenerator(ILogger<ImplementationGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<ImplementationSuggestion[]> GenerateImplementationSuggestionsAsync(GeneratedTest failingTest, TDDRequest request)
        {
            _logger.LogInformation("Generating implementation suggestions for test: {TestName}", failingTest.TestName);

            var suggestions = new List<ImplementationSuggestion>();

            // Generate simple implementation
            suggestions.Add(new ImplementationSuggestion
            {
                Id = Guid.NewGuid().ToString(),
                Approach = "simplest-working",
                CodeSnippet = new CodeSnippet
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = GenerateSimpleImplementation(failingTest),
                    ComplexityMetrics = new ComplexityMetrics { CyclomaticComplexity = 1, LinesOfCode = 10 }
                },
                Explanation = "Simplest possible implementation to make the test pass",
                Tradeoffs = new[] { "Simple but may not handle edge cases" }
            });

            // Generate robust implementation
            suggestions.Add(new ImplementationSuggestion
            {
                Id = Guid.NewGuid().ToString(),
                Approach = "robust-extensible",
                CodeSnippet = new CodeSnippet
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = GenerateRobustImplementation(failingTest),
                    ComplexityMetrics = new ComplexityMetrics { CyclomaticComplexity = 3, LinesOfCode = 25 }
                },
                Explanation = "More robust implementation with error handling and extensibility",
                Tradeoffs = new[] { "More complex but handles edge cases better" }
            });

            return await Task.FromResult(suggestions.ToArray());
        }

        public async Task<CodeSnippet[]> GenerateImplementationsAsync(ImplementationRequest request)
        {
            if (string.IsNullOrEmpty(request.FailingTest.TestCode))
                throw new ArgumentException("Failing test is required");

            _logger.LogInformation("Generating implementations using strategy: {Strategy}", request.ImplementationStrategy);

            var implementations = new List<CodeSnippet>();

            switch (request.ImplementationStrategy.ToLower())
            {
                case "simplest-first":
                    implementations.Add(CreateSimplestImplementation(request));
                    break;
                case "most-readable":
                    implementations.Add(CreateReadableImplementation(request));
                    break;
                default:
                    implementations.Add(CreateSimplestImplementation(request));
                    implementations.Add(CreateReadableImplementation(request));
                    break;
            }

            return await Task.FromResult(implementations.ToArray());
        }

        public async Task<ImplementationAnalysis> AnalyzeImplementationQualityAsync(CodeSnippet implementation, TestResult[] testResults)
        {
            var analysis = new ImplementationAnalysis
            {
                ImplementationId = implementation.Id,
                PassesTests = testResults.All(r => r.Passed),
                CodeQuality = CalculateCodeQuality(implementation),
                MaintainabilityScore = CalculateMaintainabilityScore(implementation),
                Issues = IdentifyIssues(implementation)
            };

            return await Task.FromResult(analysis);
        }

        private string GenerateSimpleImplementation(GeneratedTest test)
        {
            return """
            public class SystemUnderTest
            {
                public object DoSomething()
                {
                    return new object(); // Simplest implementation
                }
            }
            """;
        }

        private string GenerateRobustImplementation(GeneratedTest test)
        {
            return """
            public class SystemUnderTest
            {
                private readonly ILogger _logger;
                
                public SystemUnderTest(ILogger logger = null)
                {
                    _logger = logger;
                }
                
                public object DoSomething()
                {
                    try
                    {
                        // Implementation with error handling
                        var result = ProcessData();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error in DoSomething");
                        throw;
                    }
                }
                
                private object ProcessData()
                {
                    return new object();
                }
            }
            """;
        }

        private CodeSnippet CreateSimplestImplementation(ImplementationRequest request)
        {
            return new CodeSnippet
            {
                Id = Guid.NewGuid().ToString(),
                Code = "public class Implementation { public object Execute() { return new object(); } }",
                ComplexityMetrics = new ComplexityMetrics { CyclomaticComplexity = 1, LinesOfCode = 5 }
            };
        }

        private CodeSnippet CreateReadableImplementation(ImplementationRequest request)
        {
            return new CodeSnippet
            {
                Id = Guid.NewGuid().ToString(),
                Code = """
                public class Implementation 
                {
                    public object Execute() 
                    {
                        // Clear, readable implementation
                        var result = CalculateResult();
                        return FormatResult(result);
                    }
                    
                    private object CalculateResult() => new object();
                    private object FormatResult(object input) => input;
                }
                """,
                ComplexityMetrics = new ComplexityMetrics { CyclomaticComplexity = 3, LinesOfCode = 12 }
            };
        }

        private double CalculateCodeQuality(CodeSnippet implementation)
        {
            // Simplified quality calculation
            var lines = implementation.Code.Split('\n').Length;
            var complexity = implementation.ComplexityMetrics?.CyclomaticComplexity ?? 1;

            return Math.Max(0.1, Math.Min(1.0, 10.0 / (complexity * lines / 10.0)));
        }

        private double CalculateMaintainabilityScore(CodeSnippet implementation)
        {
            // Simplified maintainability calculation
            var quality = CalculateCodeQuality(implementation);
            var hasComments = implementation.Code.Contains("//") || implementation.Code.Contains("/*");

            return hasComments ? quality * 1.1 : quality * 0.9;
        }

        private string[] IdentifyIssues(CodeSnippet implementation)
        {
            var issues = new List<string>();

            if (implementation.Code.Contains("magic string"))
                issues.Add("Contains magic strings");

            if (implementation.Code.Length > 500)
                issues.Add("Method might be too long");

            return issues.ToArray();
        }
    }

    // Services/RefactoringAdvisor.cs
    public class RefactoringAdvisor : IRefactoringAdvisor
    {
        private readonly ILogger<RefactoringAdvisor> _logger;

        public RefactoringAdvisor(ILogger<RefactoringAdvisor> logger)
        {
            _logger = logger;
        }

        public async Task<RefactoringHint[]> PredictRefactoringNeedsAsync(ImplementationSuggestion[] implementations, TDDRequest request)
        {
            _logger.LogInformation("Predicting refactoring needs for {Count} implementations", implementations.Length);

            var hints = new List<RefactoringHint>();

            foreach (var impl in implementations)
            {
                if (impl.CodeSnippet.ComplexityMetrics.CyclomaticComplexity > 5)
                {
                    hints.Add(new RefactoringHint
                    {
                        ImplementationId = impl.Id,
                        Suggestion = "Consider extracting complex logic into smaller methods",
                        Reason = "High cyclomatic complexity",
                        Priority = "medium"
                    });
                }

                if (impl.CodeSnippet.Code.Contains("new object()") && impl.CodeSnippet.Code.Contains("new object()", StringComparison.Ordinal))
                {
                    var count = CountOccurrences(impl.CodeSnippet.Code, "new object()");
                    if (count > 1)
                    {
                        hints.Add(new RefactoringHint
                        {
                            ImplementationId = impl.Id,
                            Suggestion = "Consider using factory method or dependency injection",
                            Reason = $"Multiple object creations ({count})",
                            Priority = "low"
                        });
                    }
                }
            }

            return await Task.FromResult(hints.ToArray());
        }

        public async Task<RefactoringPlan> CreateRefactoringPlanAsync(CodeSnippet workingCode, RefactoringGoal[] goals, Constraint[] constraints)
        {
            _logger.LogInformation("Creating refactoring plan for code snippet: {SnippetId}", workingCode.Id);

            var steps = new List<RefactoringStep>();

            // Analyze code and create steps based on goals
            foreach (var goal in goals.OrderBy(g => g.Priority))
            {
                steps.AddRange(CreateStepsForGoal(workingCode, goal));
            }

            return await Task.FromResult(new RefactoringPlan
            {
                Id = Guid.NewGuid().ToString(),
                Steps = steps.ToArray(),
                EstimatedDuration = TimeSpan.FromMinutes(steps.Count * 5),
                RiskLevel = steps.Count > 10 ? "high" : "medium"
            });
        }

        public async Task<RefactoringOpportunity[]> IdentifyRefactoringOpportunitiesAsync(TestedImplementation[] implementations)
        {
            var opportunities = new List<RefactoringOpportunity>();

            foreach (var impl in implementations)
            {
                if (impl.Analysis.MaintainabilityScore < 0.7)
                {
                    opportunities.Add(new RefactoringOpportunity
                    {
                        ImplementationId = impl.Implementation.Id,
                        Area = "maintainability",
                        Suggestion = "Improve code structure for better maintainability",
                        ExpectedImprovement = 0.2,
                        Effort = "low"
                    });
                }

                if (impl.Implementation.Code.Contains("// TODO") || impl.Implementation.Code.Contains("// FIXME"))
                {
                    opportunities.Add(new RefactoringOpportunity
                    {
                        ImplementationId = impl.Implementation.Id,
                        Area = "code-cleanup",
                        Suggestion = "Address TODO/FIXME comments",
                        ExpectedImprovement = 0.1,
                        Effort = "very-low"
                    });
                }
            }

            return await Task.FromResult(opportunities.ToArray());
        }

        private RefactoringStep[] CreateStepsForGoal(CodeSnippet code, RefactoringGoal goal)
        {
            return goal.Type.ToLower() switch
            {
                "extract-method" => new[]
                {
                new RefactoringStep
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "extract-method",
                    Description = $"Extract method for: {goal.Description}",
                    Before = "Long method with mixed responsibilities",
                    After = "Shorter method with single responsibility",
                    SafetyChecks = new[] { "Run all tests", "Verify behavior unchanged" }
                }
            },
                "rename" => new[]
                {
                new RefactoringStep
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "rename",
                    Description = $"Rename for clarity: {goal.Description}",
                    Before = "Poorly named variables/methods",
                    After = "Clear, descriptive names",
                    SafetyChecks = new[] { "Run tests", "Check references" }
                }
            },
                _ => new[]
                {
                new RefactoringStep
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "general",
                    Description = $"Improve: {goal.Description}",
                    Before = "Current implementation",
                    After = "Improved implementation",
                    SafetyChecks = new[] { "Run tests" }
                }
            }
            };
        }

        private int CountOccurrences(string source, string pattern)
        {
            int count = 0;
            int i = 0;
            while ((i = source.IndexOf(pattern, i, StringComparison.Ordinal)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }
    }

    // Services/FuturePredictor.cs
    //public class FuturePredictor : IFuturePredictor
    //{
    //    private readonly ILogger<FuturePredictor> _logger;

    //    public FuturePredictor(ILogger<FuturePredictor> logger)
    //    {
    //        _logger = logger;
    //    }

    //    public async Task<ChangePrediction[]> PredictChangesAsync(CodeAnalysis analysis, RoadmapAnalysis roadmapAnalysis, TimeHorizon horizon)
    //    {
    //        _logger.LogInformation("Predicting changes for {Horizon} horizon", horizon);

    //        var predictions = new List<ChangePrediction>();

    //        // Simple prediction logic based on code complexity and roadmap
    //        if (analysis.Complexity > 0.7)
    //        {
    //            predictions.Add(new ChangePrediction
    //            {
    //                Id = Guid.NewGuid().ToString(),
    //                Area = "refactoring",
    //                Description = "High complexity suggests need for refactoring",
    //                Probability = 0.8,
    //                Confidence = 0.7,
    //                Timeline = GetTimelineForHorizon(horizon, 0.5)
    //            });
    //        }

    //        if (roadmapAnalysis?.Features?.Any(f => f.Contains("scale")) == true)
    //        {
    //            predictions.Add(new ChangePrediction
    //            {
    //                Id = Guid.NewGuid().ToString(),
    //                Area = "scalability",
    //                Description = "Roadmap indicates scaling requirements",
    //                Probability = 0.6,
    //                Confidence = 0.8,
    //                Timeline = GetTimelineForHorizon(horizon, 0.8)
    //            });
    //        }

    //        return await Task.FromResult(predictions.ToArray());
    //    }

    //    public async Task<FutureTestRecommendation[]> GenerateTestsForPredictedChangeAsync(ChangePrediction prediction, FuturePredictionRequest request)
    //    {
    //        var recommendations = new List<FutureTestRecommendation>();

    //        recommendations.Add(new FutureTestRecommendation
    //        {
    //            Id = Guid.NewGuid().ToString(),
    //            PredictionId = prediction.Id,
    //            TestType = "unit",
    //            Description = $"Test for predicted change: {prediction.Description}",
    //            Priority = prediction.Probability > 0.7 ? "high" : "medium",
    //            ImplementationEffort = "medium"
    //        });

    //        // Add integration test recommendation for major changes
    //        if (prediction.Probability > 0.8)
    //        {
    //            recommendations.Add(new FutureTestRecommendation
    //            {
    //                Id = Guid.NewGuid().ToString(),
    //                PredictionId = prediction.Id,
    //                TestType = "integration",
    //                Description = $"Integration test for {prediction.Area} changes",
    //                Priority = "medium",
    //                ImplementationEffort = "high"
    //            });
    //        }

    //        return await Task.FromResult(recommendations.ToArray());
    //    }

    //    public async Task<RiskMitigationStrategy[]> GenerateRiskMitigationStrategiesAsync(ChangePrediction[] predictions, List<FutureTestRecommendation> futureTests)
    //    {
    //        var strategies = new List<RiskMitigationStrategy>();

    //        var highRiskPredictions = predictions.Where(p => p.Probability > 0.7 && p.Confidence > 0.7);

    //        foreach (var prediction in highRiskPredictions)
    //        {
    //            strategies.Add(new RiskMitigationStrategy
    //            {
    //                PredictionId = prediction.Id,
    //                Strategy = "Write characterization tests before changes",
    //                Rationale = "Tests will document current behavior",
    //                ImplementationSteps = new[] { "Identify key behaviors", "Write tests", "Verify tests pass" }
    //            });
    //        }

    //        return await Task.FromResult(strategies.ToArray());
    //    }

    //    private DateTimeOffset GetTimelineForHorizon(TimeHorizon horizon, double modifier = 1.0)
    //    {
    //        var now = DateTimeOffset.UtcNow;

    //        return horizon switch
    //        {
    //            TimeHorizon.Immediate => now.AddDays(7 * modifier),
    //            TimeHorizon.Weekly => now.AddDays(14 * modifier),
    //            TimeHorizon.Monthly => now.AddDays(30 * modifier),
    //            TimeHorizon.Quarterly => now.AddDays(90 * modifier),
    //            TimeHorizon.Yearly => now.AddDays(365 * modifier),
    //            _ => now.AddDays(30 * modifier)
    //        };
    //    }
    //}


   

    // Update the FuturePredictor service to use the Features property
    public class FuturePredictor : IFuturePredictor
    {
        private readonly ILogger<FuturePredictor> _logger;

        public FuturePredictor(ILogger<FuturePredictor> logger)
        {
            _logger = logger;
        }

        public async Task<ChangePrediction[]> PredictChangesAsync(CodeAnalysis analysis, RoadmapAnalysis roadmapAnalysis, TimeHorizon horizon)
        {
            _logger.LogInformation("Predicting changes for {Horizon} horizon", horizon);

            var predictions = new List<ChangePrediction>();

            // Simple prediction logic based on code complexity and roadmap
            if (analysis.Complexity > 0.7)
            {
                predictions.Add(new ChangePrediction
                {
                    Id = Guid.NewGuid().ToString(),
                    Area = "refactoring",
                    Description = "High complexity suggests need for refactoring",
                    Probability = 0.8,
                    Confidence = 0.7,
                    Timeline = GetTimelineForHorizon(horizon, 0.5)
                });
            }

            // Use the Features property from RoadmapAnalysis
            if (roadmapAnalysis?.Features != null && roadmapAnalysis.Features.Any(f => f.Title.Contains("scale", StringComparison.OrdinalIgnoreCase)))
            {
                predictions.Add(new ChangePrediction
                {
                    Id = Guid.NewGuid().ToString(),
                    Area = "scalability",
                    Description = "Roadmap indicates scaling requirements",
                    Probability = 0.6,
                    Confidence = 0.8,
                    Timeline = GetTimelineForHorizon(horizon, 0.8)
                });
            }

            // Also check HighPriorityFeatures if Features is null
            if (roadmapAnalysis?.HighPriorityFeatures != null &&
                roadmapAnalysis.HighPriorityFeatures.Any(f => f.Contains("scale", StringComparison.OrdinalIgnoreCase)))
            {
                predictions.Add(new ChangePrediction
                {
                    Id = Guid.NewGuid().ToString(),
                    Area = "scalability",
                    Description = "High priority scaling requirements identified",
                    Probability = 0.7,
                    Confidence = 0.9,
                    Timeline = GetTimelineForHorizon(horizon, 0.6)
                });
            }

            return await Task.FromResult(predictions.ToArray());
        }

        public async Task<FutureTestRecommendation[]> GenerateTestsForPredictedChangeAsync(ChangePrediction prediction, FuturePredictionRequest request)
        {
            var recommendations = new List<FutureTestRecommendation>();

            recommendations.Add(new FutureTestRecommendation
            {
                Id = Guid.NewGuid().ToString(),
                PredictionId = prediction.Id,
                TestType = "unit",
                Description = $"Test for predicted change: {prediction.Description}",
                Priority = prediction.Probability > 0.7 ? "high" : "medium",
                ImplementationEffort = "medium"
            });

            // Add integration test recommendation for major changes
            if (prediction.Probability > 0.8)
            {
                recommendations.Add(new FutureTestRecommendation
                {
                    Id = Guid.NewGuid().ToString(),
                    PredictionId = prediction.Id,
                    TestType = "integration",
                    Description = $"Integration test for {prediction.Area} changes",
                    Priority = "medium",
                    ImplementationEffort = "high"
                });
            }

            return await Task.FromResult(recommendations.ToArray());
        }

        public async Task<RiskMitigationStrategy[]> GenerateRiskMitigationStrategiesAsync(ChangePrediction[] predictions, List<FutureTestRecommendation> futureTests)
        {
            var strategies = new List<RiskMitigationStrategy>();

            var highRiskPredictions = predictions.Where(p => p.Probability > 0.7 && p.Confidence > 0.7);

            foreach (var prediction in highRiskPredictions)
            {
                strategies.Add(new RiskMitigationStrategy
                {
                    PredictionId = prediction.Id,
                    Strategy = "Write characterization tests before changes",
                    Rationale = "Tests will document current behavior",
                    ImplementationSteps = new[] { "Identify key behaviors", "Write tests", "Verify tests pass" }
                });
            }

            return await Task.FromResult(strategies.ToArray());
        }

        private DateTimeOffset GetTimelineForHorizon(TimeHorizon horizon, double modifier = 1.0)
        {
            var now = DateTimeOffset.UtcNow;

            return horizon switch
            {
                TimeHorizon.Immediate => now.AddDays(7 * modifier),
                TimeHorizon.Weekly => now.AddDays(14 * modifier),
                TimeHorizon.Monthly => now.AddDays(30 * modifier),
                TimeHorizon.Quarterly => now.AddDays(90 * modifier),
                TimeHorizon.Yearly => now.AddDays(365 * modifier),
                _ => now.AddDays(30 * modifier)
            };
        }
    }



}
