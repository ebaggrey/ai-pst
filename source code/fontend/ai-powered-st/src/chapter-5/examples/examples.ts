// 1. POST /api/tdd-reimagined/generate-test-first - Generate Test First
// typescript
// // Example request body
// {
//   "userStory": {
//     "id": "US-123",
//     "title": "User Registration",
//     "description": "As a new user, I want to register for an account",
//     "acceptanceCriteria": [
//       {
//         "id": "AC-1",
//         "description": "User can provide email and password",
//         "testConditions": ["Email is valid", "Password meets requirements"]
//       }
//     ],
//     "businessRules": ["Password must be at least 8 characters"],
//     "examples": [
//       {
//         "scenario": "Successful registration",
//         "given": "New user with valid credentials",
//         "when": "User submits form",
//         "then": "Account is created",
//         "expectedResult": "User can log in"
//       }
//     ]
//   },
//   "tddStyle": "classic",
//   "constraints": [
//     {
//       "type": "time",
//       "value": "2h",
//       "description": "Complete within 2 hours"
//     }
//   ],
//   "generateMultipleApproaches": true,
//   "maxComplexityLevel": 5
// }

// // Example response
// {
//   "cycleId": "123e4567-e89b-12d3-a456-426614174000",
//   "phase": "test-generation",
//   "generatedTest": {
//     "testCode": "[Fact]\npublic void User_Registration_Should_Create_Account()\n{\n    // Arrange\n    var service = new RegistrationService();\n    var user = new UserDto { Email = \"test@example.com\", Password = \"Password123!\" };\n    \n    // Act\n    var result = service.Register(user);\n    \n    // Assert\n    result.Should().NotBeNull();\n    result.Success.Should().BeTrue();\n}",
//     "testFramework": "xUnit",
//     "testName": "User_Registration_Should_Create_Account",
//     "dependencies": ["xunit", "Moq", "FluentAssertions"],
//     "isFailingByDesign": true,
//     "expectedFailure": {
//       "expected": "Registration succeeds",
//       "actual": "NotImplementedException",
//       "message": "Implementation missing"
//     }
//   },
//   "implementationSuggestions": [
//     {
//       "id": "impl-001",
//       "approach": "simplest-working",
//       "codeSnippet": {
//         "id": "code-001",
//         "language": "csharp",
//         "code": "public class RegistrationService {\n    public RegistrationResult Register(UserDto user) {\n        return new RegistrationResult { Success = true };\n    }\n}",
//         "complexityMetrics": {
//           "cyclomaticComplexity": 1,
//           "linesOfCode": 5
//         }
//       },
//       "explanation": "Simplest implementation to pass the test",
//       "tradeoffs": ["No validation", "No error handling"]
//     }
//   ],
//   "refactoringHints": [
//     {
//       "implementationId": "impl-001",
//       "suggestion": "Add input validation",
//       "reason": "No validation present",
//       "priority": "high"
//     }
//   ],
//   "estimatedTimeline": "00:30:00",
//   "confidenceMetrics": {
//     "testQuality": 0.85,
//     "implementationQuality": 0.7,
//     "refactoringSafety": 0.9,
//     "overallConfidence": 0.82
//   },
//   "learningPoints": [
//     {
//       "category": "tdd",
//       "title": "Test-First Approach",
//       "description": "Writing tests first ensures requirements are clear",
//       "impact": "high"
//     }
//   ],
//   "nextSteps": [
//     "Review failing test",
//     "Choose implementation approach",
//     "Run test to confirm failure"
//   ]
// }
// 2. POST /api/tdd-reimagined/implement-from-failing-test - Implement from Failing Test
// typescript
// // Example request body
// {
//   "failingTest": {
//     "testCode": "[Fact]\npublic void User_Registration_Should_Create_Account() {\n    // Test code here\n}",
//     "testFramework": "xUnit",
//     "testName": "User_Registration_Should_Create_Account",
//     "dependencies": ["xunit"],
//     "isFailingByDesign": true,
//     "expectedFailure": {
//       "expected": "Success result",
//       "actual": "NotImplementedException",
//       "message": "Method not implemented"
//     }
//   },
//   "failureDetails": {
//     "expected": "RegistrationResult with Success = true",
//     "actual": "NotImplementedException",
//     "message": "Service not implemented"
//   },
//   "implementationStrategy": "simplest-first",
//   "constraints": [
//     {
//       "type": "complexity",
//       "value": "5",
//       "description": "Max cyclomatic complexity of 5"
//     }
//   ],
//   "allowMultipleImplementations": true
// }

// // Example response
// {
//   "cycleId": "223e4567-e89b-12d3-a456-426614174001",
//   "implementations": [
//     {
//       "implementation": {
//         "id": "impl-002",
//         "language": "csharp",
//         "code": "public class RegistrationService {\n    public RegistrationResult Register(UserDto user) {\n        if (user == null) throw new ArgumentNullException();\n        return new RegistrationResult { Success = true, UserId = Guid.NewGuid() };\n    }\n}",
//         "complexityMetrics": {
//           "cyclomaticComplexity": 2,
//           "linesOfCode": 7
//         }
//       },
//       "testResults": [
//         {
//           "testName": "User_Registration_Should_Create_Account",
//           "passed": true,
//           "duration": "00:00:00.150"
//         }
//       ],
//       "analysis": {
//         "codeQuality": 0.85,
//         "maintainabilityScore": 0.8,
//         "issues": []
//       },
//       "passesAllTests": true,
//       "qualityScore": 92.5
//     }
//   ],
//   "recommendedImplementation": {
//     "implementation": {
//       "id": "impl-002",
//       "code": "public class RegistrationService { ... }"
//     },
//     "qualityScore": 92.5
//   },
//   "codeSmellAnalysis": {
//     "overallSmellScore": 0.2,
//     "severity": "none",
//     "recommendations": ["Code looks clean"]
//   },
//   "refactoringOpportunities": [
//     {
//       "area": "validation",
//       "suggestion": "Add email format validation",
//       "effort": "low"
//     }
//   ],
//   "nextTDDCycle": {
//     "id": "cycle-003",
//     "phase": "refactor",
//     "description": "Refactor to add validation"
//   }
// }
// 3. POST /api/tdd-reimagined/refactor-with-confidence - Refactor with Confidence
// typescript
// // Example request body
// {
//   "workingCode": {
//     "id": "code-002",
//     "language": "csharp",
//     "code": "public class RegistrationService {\n    public RegistrationResult Register(UserDto user) {\n        var result = new RegistrationResult();\n        if (user.Email.Contains(\"@\")) {\n            result.Success = true;\n            result.UserId = Guid.NewGuid();\n        }\n        return result;\n    }\n}",
//     "complexityMetrics": {
//       "cyclomaticComplexity": 3,
//       "linesOfCode": 10
//     }
//   },
//   "testSuite": {
//     "name": "RegistrationTests",
//     "tests": [
//       {
//         "testName": "ValidEmail_ShouldSucceed",
//         "testCode": "[Fact] public void ValidEmail_ShouldSucceed() { ... }"
//       },
//       {
//         "testName": "InvalidEmail_ShouldFail",
//         "testCode": "[Fact] public void InvalidEmail_ShouldFail() { ... }"
//       },
//       {
//         "testName": "NullUser_ShouldThrow",
//         "testCode": "[Fact] public void NullUser_ShouldThrow() { ... }"
//       }
//     ],
//     "passRate": 1.0
//   },
//   "refactoringGoals": [
//     {
//       "type": "extract-method",
//       "description": "Extract email validation",
//       "priority": 1
//     }
//   ],
//   "safetyMeasures": {
//     "preserveBehavior": true,
//     "createCheckpoints": true,
//     "suggestRollbackPoints": true,
//     "maxStepsWithoutCommit": 5
//   },
//   "constraints": []
// }

// // Example response
// {
//   "cycleId": "323e4567-e89b-12d3-a456-426614174002",
//   "completedSteps": [
//     {
//       "step": {
//         "type": "extract-method",
//         "description": "Extract email validation logic"
//       },
//       "successful": true,
//       "result": "Successfully extracted validation method",
//       "codeAfterStep": {
//         "code": "public class RegistrationService {\n    public RegistrationResult Register(UserDto user) {\n        if (!IsValidEmail(user.Email)) return new RegistrationResult { Success = false };\n        return new RegistrationResult { Success = true, UserId = Guid.NewGuid() };\n    }\n    private bool IsValidEmail(string email) {\n        return email.Contains(\"@\");\n    }\n}",
//         "complexityMetrics": {
//           "cyclomaticComplexity": 2,
//           "linesOfCode": 12
//         }
//       },
//       "duration": "00:00:02",
//       "testResults": [
//         { "testName": "ValidEmail_ShouldSucceed", "passed": true },
//         { "testName": "InvalidEmail_ShouldFail", "passed": true }
//       ]
//     }
//   ],
//   "originalCode": {
//     "code": "public class RegistrationService { ... }"
//   },
//   "refactoredCode": {
//     "code": "public class RegistrationService {\n    public RegistrationResult Register(UserDto user) {\n        if (!IsValidEmail(user.Email)) return new RegistrationResult { Success = false };\n        return new RegistrationResult { Success = true, UserId = Guid.NewGuid() };\n    }\n    private bool IsValidEmail(string email) {\n        return email.Contains(\"@\");\n    }\n}"
//   },
//   "improvementMetrics": {
//     "overallImprovement": 0.15,
//     "maintainabilityGain": 0.2,
//     "readabilityGain": 0.3,
//     "complexityReduction": 1
//   },
//   "testSafetyReport": {
//     "totalTests": 3,
//     "passingTests": 3,
//     "allTestsPass": true,
//     "safetyIssues": []
//   },
//   "futureMaintenanceImpact": {
//     "estimatedMaintenanceCost": 0.3,
//     "longTermSustainability": "excellent"
//   }
// }
// 4. POST /api/tdd-reimagined/predict-future-tests - Predict Future Tests
// typescript
// // Example request body
// {
//   "currentCode": {
//     "id": "code-003",
//     "language": "csharp",
//     "code": "public class RegistrationService {\n    public RegistrationResult Register(UserDto user) {\n        // Implementation\n    }\n    public UserDto GetUser(Guid id) {\n        // Implementation\n    }\n}",
//     "complexityMetrics": {
//       "cyclomaticComplexity": 4,
//       "linesOfCode": 25
//     }
//   },
//   "productRoadmap": {
//     "id": "roadmap-2024",
//     "name": "2024 Roadmap",
//     "features": [
//       {
//         "id": "feat-001",
//         "title": "Social Authentication",
//         "description": "Add Google login",
//         "priority": "high",
//         "targetDate": "2024-06-30T00:00:00Z"
//       },
//       {
//         "id": "feat-002",
//         "title": "Two-Factor Authentication",
//         "description": "Add 2FA",
//         "priority": "medium",
//         "targetDate": "2024-09-30T00:00:00Z"
//       }
//     ],
//     "milestones": [
//       {
//         "id": "ms-001",
//         "name": "Q2 Release",
//         "date": "2024-06-30T00:00:00Z"
//       }
//     ],
//     "startDate": "2024-01-01T00:00:00Z",
//     "endDate": "2024-12-31T00:00:00Z"
//   },
//   "timeHorizon": "quarterly",
//   "confidenceThreshold": 0.7,
//   "focusAreas": ["authentication", "security"]
// }

// // Example response
// {
//   "predictionId": "423e4567-e89b-12d3-a456-426614174003",
//   "timeHorizon": "quarterly",
//   "changePredictions": [
//     {
//       "id": "pred-001",
//       "area": "authentication",
//       "description": "Social login features will be added",
//       "probability": 0.85,
//       "confidence": 0.8,
//       "timeline": "2024-06-15T00:00:00Z"
//     },
//     {
//       "id": "pred-002",
//       "area": "security",
//       "description": "2FA implementation required",
//       "probability": 0.7,
//       "confidence": 0.75,
//       "timeline": "2024-08-30T00:00:00Z"
//     }
//   ],
//   "futureTestRecommendations": [
//     {
//       "id": "test-rec-001",
//       "predictionId": "pred-001",
//       "testType": "integration",
//       "description": "Test Google OAuth login flow",
//       "priority": "high",
//       "implementationEffort": "medium"
//     },
//     {
//       "id": "test-rec-002",
//       "predictionId": "pred-002",
//       "testType": "unit",
//       "description": "Test TOTP code generation and validation",
//       "priority": "medium",
//       "implementationEffort": "low"
//     }
//   ],
//   "confidenceSummary": {
//     "averageConfidence": 0.78,
//     "highConfidencePredictions": 1,
//     "mediumConfidencePredictions": 1,
//     "lowConfidencePredictions": 0,
//     "overallReliability": "medium"
//   },
//   "riskMitigationStrategies": [
//     {
//       "predictionId": "pred-001",
//       "strategy": "Write characterization tests before implementing social login",
//       "rationale": "Ensure existing auth behavior is preserved",
//       "implementationSteps": [
//         "Document current auth flows",
//         "Create integration tests",
//         "Verify tests pass"
//       ]
//     }
//   ]
// }
// 5. GET /health - Health Check
// typescript
// // Example request
// // No request body needed

// // Example response
// {
//   "status": "healthy",
//   "timestamp": "2024-01-15T10:30:00Z"
// }
// Error Responses
// 400 Bad Request (Validation Error)
// typescript
// {
//   "phase": "validation",
//   "errorType": "constraint-violation",
//   "message": "TDD constraints need adjustment",
//   "recoveryStrategy": [
//     "Fix: User Story ID is required",
//     "Fix: TDD style must be one of: classic, outside-in, inside-out"
//   ],
//   "suggestedFallback": "Use simpler constraints or different TDD style"
// }
// 422 Unprocessable Entity (Test Generation Error)
// typescript
// {
//   "phase": "test-generation",
//   "errorType": "cannot-generate-failing-test",
//   "message": "Struggling to create a proper failing test",
//   "recoveryStrategy": [
//     "Simplify the user story",
//     "Provide more concrete examples",
//     "Specify acceptance criteria more clearly"
//   ],
//   "suggestedFallback": "Write the first failing test manually",
//   "learningOpportunity": "Complex stories may need decomposition before TDD"
// }
// 500 Internal Server Error
// typescript
// {
//   "phase": "refactoring",
//   "errorType": "safety-violation",
//   "message": "Refactoring would break tests or behavior",
//   "recoveryStrategy": [
//     "Add more comprehensive tests",
//     "Refactor in smaller increments",
//     "Disable behavior preservation for risky refactors"
//   ],
//   "suggestedFallback": "Manual refactoring with careful test monitoring"
// }