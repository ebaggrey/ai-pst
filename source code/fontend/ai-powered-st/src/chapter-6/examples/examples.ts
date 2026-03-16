// BDD Supercharged Service - HTTP Request Examples

// 1. POST /api/bdd-supercharged/co-create-scenarios - Co-create Scenarios
// typescript
// // Example request body
// {
//   "requirement": "As a user, I want to login to access my account",
//   "stakeholderPerspectives": [
//     {
//       "role": "End User",
//       "priorities": ["Ease of use", "Security"],
//       "concerns": ["Password recovery"]
//     }
//   ],
//   "conversationConstraints": {
//     "maxRounds": 3,
//     "consensusThreshold": 0.7,
//     "forbiddenAssumptions": []
//   },
//   "desiredOutcomes": ["Clear login scenarios"]
// }

// // Example response
// {
//   "sessionId": "123e4567-e89b-12d3-a456-426614174000",
//   "requirement": "As a user, I want to login to access my account",
//   "conversationRounds": [
//     {
//       "roundNumber": 1,
//       "stakeholderInputs": ["Round 1: End User perspective - prioritizing Ease of use, Security"],
//       "clarifications": ["Clarify: Round 1: End User perspective..."],
//       "decisions": ["Decision based on stakeholder input"],
//       "consensusScore": 0.85,
//       "updatedConversation": {
//         "id": "conv-001",
//         "participants": ["End User"],
//         "topics": ["login", "security"],
//         "decisions": ["Implement email/password login"],
//         "openQuestions": ["Should we support social login?"],
//         "createdAt": "2024-03-04T10:00:00Z"
//       }
//     }
//   ],
//   "generatedScenarios": [
//     {
//       "title": "Scenario for: login",
//       "given": ["the user is registered", "the user is on login page"],
//       "when": ["the user enters credentials", "the user clicks login"],
//       "then": ["the user is redirected to dashboard"],
//       "tags": ["End User", "generated"],
//       "description": "Generated from conversation on 2024-03-04",
//       "examples": []
//     }
//   ],
//   "assumptionsChallenged": ["Users always remember passwords"],
//   "consensusPoints": ["Implement email/password login"],
//   "unresolvedQuestions": ["Should we support social login?"],
//   "nextSteps": ["Review 1 generated scenarios", "Validate against business requirements"],
//   "conversationQualityScore": 0.85
// }
// 2. POST /api/bdd-supercharged/translate-to-automation - Translate Scenario to Automation
// typescript
// // Example request body
// {
//   "scenario": {
//     "title": "User Login",
//     "given": ["the user is on login page", "the user has valid credentials"],
//     "when": ["the user enters credentials", "the user clicks login"],
//     "then": ["the user is redirected to dashboard", "a session is created"],
//     "tags": ["authentication", "login"],
//     "description": "Successful user login",
//     "examples": []
//   },
//   "techContext": {
//     "stack": "dotnet",
//     "testFramework": "xunit",
//     "libraries": [],
//     "constraints": []
//   },
//   "translationStyle": "declarative",
//   "qualityTargets": {
//     "minCoverage": 0.8,
//     "maxComplexity": 10,
//     "maxDependencies": 5,
//     "validationRules": []
//   }
// }

// // Example response
// {
//   "translationId": "123e4567-e89b-12d3-a456-426614174001",
//   "originalScenario": {
//     "title": "User Login",
//     "given": ["the user is on login page", "the user has valid credentials"],
//     "when": ["the user enters credentials", "the user clicks login"],
//     "then": ["the user is redirected to dashboard", "a session is created"],
//     "tags": ["authentication", "login"],
//     "description": "Successful user login",
//     "examples": []
//   },
//   "automationSteps": [
//     {
//       "originalStep": "the user is on login page",
//       "automationCode": "[StepDefinition(\"the user is on login page\")]\npublic void Step_the_user_is_on_login_page()\n{\n    // SETUP: the user is on login page\n    // Auto-generated implementation\n}",
//       "validationRules": [
//         {
//           "name": "SetupComplete",
//           "condition": "All dependencies initialized",
//           "errorMessage": "Setup failed to initialize dependencies"
//         }
//       ],
//       "dependencies": ["TestFramework", "MockingLibrary"],
//       "qualityMetrics": {
//         "complexityScore": 0.8,
//         "maintainabilityScore": 0.7,
//         "readabilityScore": 0.9,
//         "cyclomaticComplexity": 3,
//         "linesOfCode": 15
//       }
//     }
//   ],
//   "frameworkIntegration": {
//     "frameworkSetup": "[Collection(\"BDD Tests\")]\npublic class BDDTestCollection : IClassFixture<TestFixture>\n{\n    // xUnit setup for BDD tests\n}",
//     "testStructure": "// Test structure for dotnet using xunit\n// Generated from BDD scenarios",
//     "requiredPackages": ["xunit", "BDDFramework", "AssertionLibrary"],
//     "configuration": ["Stack: dotnet", "TestFramework: xunit", "StepCount: 3", "Generated: 2024-03-04 10:00:00"]
//   },
//   "livingTest": {
//     "id": "test-001",
//     "steps": ["[StepDefinition(\"the user is on login page\")]..."],
//     "adaptations": ["auto-retry", "dynamic-data"],
//     "monitoringPoints": ["execution-time", "success-rate"],
//     "createdAt": "2024-03-04T10:00:00Z"
//   },
//   "qualityReport": {
//     "coverageScore": 0.85,
//     "maintainabilityScore": 0.75,
//     "readabilityScore": 0.88,
//     "issues": ["Complex step validation"],
//     "recommendations": ["Simplify validation logic"]
//   },
//   "maintenanceGuidance": {
//     "commonIssues": ["Data setup failures", "Timing issues"],
//     "fixPatterns": ["Retry logic", "Mock external services"],
//     "updateTriggers": ["API changes", "Business rule updates"],
//     "bestPractices": ["Use page objects", "Implement wait strategies"]
//   },
//   "evolutionHooks": [
//     {
//       "hookType": "monitoring",
//       "trigger": "test-failure-rate > 0.1",
//       "action": "notify-maintainers",
//       "dependencies": ["monitoring-service"]
//     }
//   ]
// }
// 3. POST /api/bdd-supercharged/evolve-scenarios - Evolve Scenarios
// typescript
// // Example request body
// {
//   "existingScenarios": [
//     {
//       "title": "User Registration",
//       "given": ["the user is on registration page"],
//       "when": ["the user enters valid details", "the user submits form"],
//       "then": ["account is created", "confirmation email is sent"],
//       "tags": ["registration"],
//       "description": "User registration flow",
//       "examples": []
//     }
//   ],
//   "newInformation": "Add GDPR consent checkbox",
//   "breakingChanges": [
//     {
//       "type": "addition",
//       "description": "Add consent checkbox to form",
//       "impactLevel": "medium",
//       "affectedAreas": ["registration-form"]
//     }
//   ],
//   "validationRules": [],
//   "evolutionStrategy": "preserve-intent"
// }

// // Example response
// {
//   "evolutionId": "123e4567-e89b-12d3-a456-426614174002",
//   "originalScenarios": [
//     {
//       "title": "User Registration",
//       "given": ["the user is on registration page"],
//       "when": ["the user enters valid details", "the user submits form"],
//       "then": ["account is created", "confirmation email is sent"],
//       "tags": ["registration"],
//       "description": "User registration flow",
//       "examples": []
//     }
//   ],
//   "evolvedScenarios": [
//     {
//       "title": "User Registration (Evolved)",
//       "given": ["the user is on registration page"],
//       "when": ["the user enters valid details", "the user checks consent checkbox", "the user submits form"],
//       "then": ["account is created", "confirmation email is sent", "consent is recorded"],
//       "tags": ["registration", "evolved-preserved", "updated-20240304", "change-addition"],
//       "description": "User registration flow\n\nEvolution Notes: Add GDPR consent checkbox",
//       "examples": []
//     }
//   ],
//   "evolutionRecords": [
//     {
//       "originalScenarioId": "user-registration",
//       "evolvedScenarioId": "user-registration-evolved",
//       "changes": ["addition: Add consent checkbox to form"],
//       "rationale": ["Applied addition change with medium impact: Add consent checkbox to form"],
//       "preservationScore": 0.85,
//       "evolvedAt": "2024-03-04T10:00:00Z"
//     }
//   ],
//   "impactAnalysis": {
//     "highImpactChanges": [],
//     "mediumImpactChanges": ["Add consent checkbox to form"],
//     "lowImpactChanges": [],
//     "affectedAreas": ["registration-form"],
//     "risks": ["Data loss", "Integration failures"]
//   },
//   "preservationMetrics": {
//     "averagePreservation": 0.85,
//     "minPreservation": 0.85,
//     "maxPreservation": 0.85,
//     "wellPreservedAreas": ["Business logic", "User flows"],
//     "poorlyPreservedAreas": []
//   },
//   "breakingChangeHandling": {
//     "successfullyHandled": ["Add consent checkbox to form"],
//     "partiallyHandled": [],
//     "notHandled": [],
//     "workarounds": []
//   },
//   "futureCompatibility": {
//     "compatibilityScore": 0.8,
//     "futureProofAreas": ["Modular design"],
//     "vulnerableAreas": ["Tight coupling"],
//     "recommendations": ["Add abstraction layers"]
//   }
// }
// 4. POST /api/bdd-supercharged/detect-drift - Detect Scenario Drift
// typescript
// // Example request body
// {
//   "DocumentedScenarios": [
//     {
//       "title": "User Login",
//       "given": ["the user is on login page"],
//       "when": ["the user enters credentials", "the user clicks login"],
//       "then": ["the user is redirected to dashboard"],
//       "tags": ["login"],
//       "description": "Login scenario",
//       "examples": []
//     }
//   ],
//   "ImplementedBehavior": [
//     {
//       "scenarioId": "user-login",
//       "steps": ["Navigate to login", "Enter credentials", "Click login"],
//       "outcomes": ["Check redirect to dashboard"],
//       "edgeCases": [],
//       "lastUpdated": "2024-03-04T10:00:00Z"
//     }
//   ],
//   "DetectionMethods": ["semantic", "structural"],
//   "Sensitivity": 0.7,
//   "AutoSuggestFixes": true
// }

// // Example response
// {
//   "detectionId": "123e4567-e89b-12d3-a456-426614174003",
//   "documentedScenarios": [
//     {
//       "title": "User Login",
//       "given": ["the user is on login page"],
//       "when": ["the user enters credentials", "the user clicks login"],
//       "then": ["the user is redirected to dashboard"],
//       "tags": ["login"],
//       "description": "Login scenario",
//       "examples": []
//     }
//   ],
//   "implementedBehavior": [
//     {
//       "scenarioId": "user-login",
//       "steps": ["Navigate to login", "Enter credentials", "Click login"],
//       "outcomes": ["Check redirect to dashboard"],
//       "edgeCases": [],
//       "lastUpdated": "2024-03-04T10:00:00Z"
//     }
//   ],
//   "driftFindings": [
//     {
//       "type": "missing-edge-cases",
//       "scenarioId": "user-login",
//       "description": "1 documented edge cases not implemented",
//       "severity": "medium",
//       "evidence": ["Example: Valid credentials"],
//       "impact": ["Incomplete testing", "Potential undiscovered bugs"]
//     }
//   ],
//   "driftSeverity": "medium",
//   "coverageGaps": [
//     {
//       "area": "Error handling",
//       "description": "Missing error scenarios in implementation",
//       "riskLevel": "medium",
//       "affectedScenarios": ["User Login"]
//     }
//   ],
//   "suggestedFixes": [
//     {
//       "driftFindingId": "missing-edge-cases",
//       "fixType": "automated-refactoring",
//       "description": "Fix for missing-edge-cases drift",
//       "steps": ["Analyze current implementation", "Generate corrected code"],
//       "verification": ["Run existing tests", "Validate with stakeholders"]
//     }
//   ],
//   "prioritizedActions": [
//     {
//       "action": "Fix missing-edge-cases drift",
//       "priority": "medium",
//       "dependencies": ["Analyze current implementation"],
//       "resources": ["Development team", "Testing resources"]
//     }
//   ],
//   "monitoringRecommendations": [
//     {
//       "area": "missing-edge-cases",
//       "metric": "drift-missing-edge-cases-count",
//       "threshold": "5",
//       "triggers": ["scenario-update", "scheduled-check"]
//     }
//   ]
// }
// 5. POST /api/bdd-supercharged/generate-documentation - Generate Living Documentation
// typescript
// // Example request body
// {
//   "scenarios": [
//     {
//       "title": "Search Products",
//       "given": ["the user is on homepage"],
//       "when": ["the user enters search term", "the user clicks search"],
//       "then": ["products are displayed", "results are paginated"],
//       "tags": ["search", "product"],
//       "description": "Product search functionality",
//       "examples": []
//     }
//   ],
//   "testResults": [
//     {
//       "scenarioId": "search-products",
//       "passed": true,
//       "errors": [],
//       "executionTime": "2024-03-04T10:00:00Z",
//       "duration": 1.5
//     }
//   ],
//   "audience": {
//     "role": "developer",
//     "technicalLevel": "intermediate",
//     "interests": [],
//     "constraints": []
//   },
//   "format": "html",
//   "include": ["test-results"],
//   "updateStrategy": {
//     "trigger": "scenario-change",
//     "autoUpdate": true,
//     "notifyRoles": [],
//     "versioning": "semantic"
//   }
// }

// // Example response (HTML format - simplified)
// {
//   "documentationId": "123e4567-e89b-12d3-a456-426614174004",
//   "generatedDocumentation": {
//     "format": "html",
//     "content": "<!DOCTYPE html><html><body><h1>BDD Documentation</h1>...</body></html>",
//     "sections": ["header", "summary", "scenarios", "test-results", "footer"],
//     "navigation": ["#summary", "#scenarios", "#test-results"],
//     "interactiveElements": ["responsive-layout"]
//   },
//   "scenarioAnalysis": {
//     "healthScore": 1.0,
//     "coverageScore": 1.0,
//     "healthyScenarios": ["Search Products"],
//     "problematicScenarios": [],
//     "recommendations": ["Add missing tests", "Improve documentation"]
//   },
//   "audienceAppropriatenessScore": 0.9,
//   "interactiveFeatures": [
//     {
//       "featureType": "responsive-layout",
//       "description": "responsive-layout interactive feature",
//       "capabilities": ["user-interaction", "dynamic-content"],
//       "requirements": ["javascript-enabled", "modern-browser"]
//     }
//   ],
//   "updateMechanism": {
//     "type": "automatic",
//     "trigger": "scenario-change",
//     "processes": ["scenario-change-detection", "documentation-generation"],
//     "notifications": []
//   },
//   "accessPatterns": [
//     {
//       "pattern": "browse-by-tag",
//       "description": "Browse scenarios by their tags",
//       "useCases": ["Finding related scenarios"],
//       "bestPractices": ["Use consistent tagging"]
//     }
//   ],
//   "qualityMetrics": {
//     "clarityScore": 0.85,
//     "completenessScore": 1.0,
//     "accuracyScore": 0.9,
//     "maintainabilityScore": 0.75,
//     "issues": ["Could use more examples"]
//   }
// }
// Error Responses
// typescript
// // 400 Bad Request - Validation Error
// {
//   "errorType": "vague-requirement",
//   "phase": "co-creation",
//   "message": "Requirement is too vague for meaningful scenario creation",
//   "recoveryPath": ["Add more detail to requirement", "Provide concrete examples"],
//   "fallbackSuggestion": "Start with user story format: As a... I want... So that..."
// }

// // 422 Unprocessable Entity - Business Logic Error
// {
//   "errorType": "conversation-stalled",
//   "phase": "co-creation",
//   "message": "Conversation isn't converging on useful scenarios",
//   "recoveryPath": ["Change collaboration mode", "Add more specific constraints"],
//   "fallbackSuggestion": "Manual scenario workshop with guided templates",
//   "conflictDetails": {
//     "conflictingScenarios": ["User Login"],
//     "ambiguityAreas": ["Authentication method"],
//     "stakeholderConflicts": ["End User", "Security Team"]
//   }
// }

// // 500 Internal Server Error
// {
//   "errorType": "drift-analysis-failed",
//   "phase": "drift-detection",
//   "message": "Could not analyze drift between scenarios and implementation",
//   "recoveryPath": ["Simplify scenario descriptions", "Use simpler detection methods"],
//   "fallbackSuggestion": "Manual comparison with sample behaviors"
// }