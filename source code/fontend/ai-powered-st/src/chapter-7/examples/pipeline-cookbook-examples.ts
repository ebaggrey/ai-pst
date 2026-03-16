
// Pipeline Cookbook Service - HTTP Request Examples
// --------------------------------------------------
// 1. POST /api/pipeline-cookbook/generate-pipeline - Generate Intelligent Pipeline
// typescript
// // Example request body
// {
//   "codebaseAnalysis": {
//     "language": "C#",
//     "testCoverage": 0.85,
//     "totalLines": 250000,
//     "dependencies": [
//       "Microsoft.NET.Sdk",
//       "EntityFrameworkCore",
//       "MassTransit",
//       "Serilog"
//     ]
//   },
//   "constraints": {
//     "maxDuration": "15 minutes",
//     "maxCostPerRun": 2.50
//   },
//   "teamPractices": {
//     "codeReviews": true,
//     "automatedTesting": true,
//     "deploymentStrategy": ["blue-green", "canary"]
//   },
//   "optimizationFocus": {
//     "speed": true,
//     "reliability": true,
//     "cost": false
//   }
// }

// // Example success response (200 OK)
// {
//   "pipelineId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "pipelineDefinition": {
//     "stages": [
//       {
//         "id": "build-001",
//         "name": "Build"
//       },
//       {
//         "id": "test-001",
//         "name": "Test"
//       },
//       {
//         "id": "deploy-001",
//         "name": "Deploy"
//       }
//     ]
//   },
//   "decisionPoints": [
//     {
//       "id": "dp-001",
//       "condition": "testCoverage > 80"
//     }
//   ],
//   "recoveryPaths": [
//     {
//       "id": "rp-001",
//       "steps": ["Retry build", "Clean cache"]
//     }
//   ],
//   "estimatedMetrics": {
//     "estimatedDuration": "00:15:00",
//     "estimatedCost": 2.25
//   },
//   "optimizationSuggestions": [
//     {
//       "id": "opt-001",
//       "description": "Parallelize test execution"
//     }
//   ],
//   "monitoringConfiguration": {
//     "metrics": ["duration", "successRate", "testCoverage"]
//   },
//   "adaptationGuidance": {
//     "recommendations": ["Consider splitting into microservices"]
//   }
// }

// // Example error response (400 Bad Request)
// {
//   "errorType": "unrealistic-constraints",
//   "message": "Pipeline constraints may not be achievable with current codebase",
//   "recoverySteps": [
//     "Relax duration or cost constraints",
//     "Consider splitting pipeline into stages",
//     "Reduce required quality gates"
//   ],
//   "fallbackSuggestion": "Generate pipeline without constraints first"
// }

// // Example error response (422 Unprocessable Entity)
// {
//   "errorType": "complexity-overload",
//   "message": "Codebase too complex for automated pipeline generation",
//   "recoverySteps": [
//     "Simplify codebase structure",
//     "Generate pipeline for subsystems separately",
//     "Provide more specific requirements"
//   ],
//   "fallbackSuggestion": "Manual pipeline design with template assistance"
// }
// 2. POST /api/pipeline-cookbook/diagnose-failure - Diagnose Pipeline Failure
// typescript
// // Example request body
// {
//   "failureLogs": {
//     "rawLogs": "[2024-01-15T10:30:45Z] [ERROR] Build failed: Project 'ApiGateway' \n[2024-01-15T10:30:46Z] [DETAIL] CS0012: The type 'DbContext' is defined in an assembly that is not referenced.",
//     "failureTime": "2024-01-15T10:30:45Z"
//   },
//   "recentChanges": {
//     "changes": [
//       {
//         "id": "change-001",
//         "type": "dependency",
//         "description": "Updated EntityFrameworkCore from 6.0.0 to 7.0.0"
//       }
//     ]
//   },
//   "diagnosisDepth": 2,
//   "includeRemediation": true,
//   "preventionStrategies": true,
//   "pipelineContext": {
//     "pipelineId": "pipeline-123",
//     "stage": "build"
//   }
// }

// // Example success response (200 OK)
// {
//   "diagnosisId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "failureDetails": {
//     "errorType": "CompilationError",
//     "message": "Missing assembly reference",
//     "stackTrace": "at Build.Compile() in Build.cs:line 45"
//   },
//   "rootCause": {
//     "summary": "EntityFrameworkCore version mismatch",
//     "component": "dependency"
//   },
//   "confidence": 0.92,
//   "remediationSteps": [
//     {
//       "stepNumber": 1,
//       "action": "Downgrade EntityFrameworkCore to 6.0.0"
//     },
//     {
//       "stepNumber": 2,
//       "action": "Clear NuGet cache and rebuild"
//     }
//   ],
//   "preventionStrategies": [
//     {
//       "id": "prev-001",
//       "description": "Lock dependency versions in CI pipeline"
//     }
//   ],
//   "similarHistoricalFailures": [
//     {
//       "failureId": "fail-001",
//       "occurredAt": "2024-01-10T08:15:00Z"
//     }
//   ],
//   "impactAssessment": {
//     "impactScore": 0.8,
//     "affectedComponents": ["build", "test"]
//   }
// }

// // Example error response (422 Unprocessable Entity)
// {
//   "errorType": "log-parsing-error",
//   "message": "Cannot parse failure logs for diagnosis",
//   "recoverySteps": [
//     "Provide logs in standard format",
//     "Include more context around failure",
//     "Try manual diagnosis with error snippets"
//   ],
//   "fallbackSuggestion": "Manual log analysis with pattern matching"
// }
// 3. POST /api/pipeline-cookbook/optimize-performance - Optimize Pipeline Performance
// typescript
// // Example request body
// {
//   "currentMetrics": {
//     "averageDuration": "00:30:00",
//     "successRate": 0.85,
//     "resourceUtilization": 0.65
//   },
//   "identifiedBottlenecks": [
//     {
//       "stageId": "test-stage",
//       "description": "Unit tests taking 15 minutes",
//       "impactScore": 0.8
//     },
//     {
//       "stageId": "build-stage",
//       "description": "NuGet package restore slow",
//       "impactScore": 0.6
//     }
//   ],
//   "optimizationGoals": [
//     {
//       "type": "duration",
//       "targetValue": 15,
//       "priority": 2
//     },
//     {
//       "type": "success-rate",
//       "targetValue": 0.95,
//       "priority": 1
//     }
//   ],
//   "constraints": {
//     "maxBudget": 500,
//     "maxDowntime": "01:00:00"
//   }
// }

// // Example success response (200 OK)
// {
//   "optimizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "currentPerformance": {
//     "averageDuration": "00:30:00",
//     "successRate": 0.85,
//     "resourceUtilization": 0.65
//   },
//   "optimizationOpportunities": [
//     {
//       "id": "opp-001",
//       "description": "Optimize test-stage: Unit tests taking 15 minutes"
//     },
//     {
//       "id": "opp-002",
//       "description": "Optimize build-stage: NuGet package restore slow"
//     }
//   ],
//   "proposedStrategies": [
//     {
//       "id": "strat-001",
//       "name": "duration-optimization-123456"
//     }
//   ],
//   "tradeOffAnalysis": {
//     "tradeOffs": [
//       {
//         "strategyA": "strat-001",
//         "strategyB": "strat-002"
//       }
//     ]
//   },
//   "implementationPlan": {
//     "steps": [
//       {
//         "order": 1,
//         "action": "Implement parallel test execution"
//       },
//       {
//         "order": 2,
//         "action": "Configure NuGet cache"
//       }
//     ]
//   },
//   "expectedImprovements": [
//     {
//       "metric": "duration",
//       "improvement": 45.5
//     }
//   ],
//   "riskAssessment": {
//     "risks": [
//       {
//         "description": "Test instability",
//         "probability": 0.3
//       }
//     ]
//   },
//   "validationPlan": {
//     "testCases": ["Performance benchmark", "Load test"]
//   }
// }

// // Example error response (422 Unprocessable Entity)
// {
//   "errorType": "goal-conflict",
//   "message": "Optimization goals conflict with each other",
//   "recoverySteps": [
//     "Prioritize optimization goals",
//     "Relax some constraints",
//     "Optimize in phases"
//   ],
//   "fallbackSuggestion": "Manual optimization with goal balancing"
// }
// 4. POST /api/pipeline-cookbook/predict-issues - Predict Pipeline Issues
// typescript
// // Example request body
// {
//   "proposedChanges": [
//     {
//       "id": "db-change-001",
//       "type": "database",
//       "description": "Add new Users table with email verification column"
//     },
//     {
//       "id": "api-change-001",
//       "type": "api",
//       "description": "Add new endpoint for user registration"
//     }
//   ],
//   "historicalData": {
//     "runs": [
//       {
//         "runId": "run-001",
//         "timestamp": "2024-01-14T10:00:00Z",
//         "succeeded": false,
//         "errors": ["Migration failed: Duplicate key violation"]
//       },
//       {
//         "runId": "run-002",
//         "timestamp": "2024-01-13T09:30:00Z",
//         "succeeded": true,
//         "errors": []
//       }
//     ]
//   },
//   "predictionHorizon": "7.00:00:00",
//   "confidenceThreshold": 0.7,
//   "includeMitigations": true
// }

// // Example success response (200 OK)
// {
//   "predictionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "proposedChanges": [
//     {
//       "id": "db-change-001",
//       "type": "database",
//       "description": "Add new Users table with email verification column"
//     }
//   ],
//   "predictions": [
//     {
//       "issue": "Potential failure in database component",
//       "probability": 0.7
//     },
//     {
//       "issue": "Potential failure in api component",
//       "probability": 0.5
//     }
//   ],
//   "confidenceScores": [0.85, 0.72],
//   "riskScores": [
//     {
//       "category": "database",
//       "score": 0.8
//     },
//     {
//       "category": "api",
//       "score": 0.5
//     }
//   ],
//   "mitigations": [
//     {
//       "predictionId": "pred-001",
//       "action": "Add database migration rollback plan"
//     }
//   ],
//   "recommendedActions": [
//     {
//       "action": "Run database migration in staging first",
//       "priority": 0.9
//     }
//   ],
//   "monitoringRecommendations": [
//     {
//       "metric": "database-errors",
//       "threshold": 5
//     }
//   ],
//   "historicalEvidence": [
//     {
//       "patternId": "pattern-001",
//       "similarity": 0.85
//     }
//   ]
// }

// // Example error response (422 Unprocessable Entity)
// {
//   "errorType": "insufficient-history",
//   "message": "Not enough historical data for accurate predictions",
//   "recoverySteps": [
//     "Collect more pipeline run data",
//     "Reduce confidence threshold",
//     "Use simpler prediction models"
//   ],
//   "fallbackSuggestion": "Manual risk assessment based on change type"
// }
// 5. POST /api/pipeline-cookbook/adapt-to-change - Adapt Pipeline to Change
// typescript
// // Example request body
// {
//   "changeType": "compliance-requirement",
//   "impactAssessment": {
//     "impactScore": 0.9,
//     "affectedComponents": [
//       "security-scanning",
//       "dependency-check",
//       "compliance-reporting"
//     ]
//   },
//   "adaptationStrategy": 1,
//   "validationRules": [
//     {
//       "ruleId": "soc2-compliance",
//       "condition": "securityScan.passed == true && vulnerabilityCount == 0"
//     },
//     {
//       "ruleId": "license-compliance",
//       "condition": "licenseCheck.criticalViolations == 0"
//     }
//   ]
// }

// // Example success response (200 OK)
// {
//   "adaptationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "changeType": "compliance-requirement",
//   "adaptationPlan": {
//     "steps": [
//       {
//         "id": "step-001",
//         "description": "Modify security-scanning for soc2-compliance"
//       },
//       {
//         "id": "step-002",
//         "description": "Update dependency-check for license-compliance"
//       }
//     ]
//   },
//   "validationResults": [
//     {
//       "ruleId": "soc2-compliance",
//       "passed": true
//     },
//     {
//       "ruleId": "license-compliance",
//       "passed": true
//     }
//   ],
//   "rollbackPlan": {
//     "steps": [
//       {
//         "order": 1,
//         "action": "Revert security scanning configuration"
//       },
//       {
//         "order": 2,
//         "action": "Restore dependency check settings"
//       }
//     ]
//   },
//   "effortEstimate": {
//     "duration": "04:00:00",
//     "complexity": 5
//   },
//   "implementationSteps": [
//     {
//       "order": 1,
//       "action": "Update security scanning rules"
//     },
//     {
//       "order": 2,
//       "action": "Configure license compliance checks"
//     },
//     {
//       "order": 3,
//       "action": "Test compliance requirements"
//     }
//   ],
//   "testingStrategy": {
//     "testTypes": ["security", "compliance", "regression"]
//   },
//   "communicationPlan": {
//     "stakeholders": ["security-team", "compliance-officer", "devops"]
//   }
// }

// // Example error response (422 Unprocessable Entity)
// {
//   "errorType": "adaptation-complexity",
//   "message": "Change too complex for automated adaptation",
//   "recoverySteps": [
//     "Break change into smaller increments",
//     "Manual adaptation with guided steps",
//     "Temporary workarounds while adapting"
//   ],
//   "fallbackSuggestion": "Manual pipeline redesign with expert review"
// }
// Error Response Types
// The API handles the following error types:

// 400 Bad Request (Validation errors)
// typescript
// {
//   "errorType": "missing-logs",
//   "message": "Failure logs are required for diagnosis",
//   "recoverySteps": ["Provide complete failure logs"],
//   "fallbackSuggestion": "Manual log inspection"
// }
// 422 Unprocessable Entity (Business logic errors)
// typescript
// {
//   "errorType": "complexity-overload",
//   "message": "Codebase too complex for automated pipeline generation",
//   "recoverySteps": [
//     "Simplify codebase structure",
//     "Generate pipeline for subsystems separately"
//   ],
//   "fallbackSuggestion": "Manual pipeline design with template assistance"
// }
// 500 Internal Server Error (Unexpected errors)
// typescript
// {
//   "errorType": "internal-server-error",
//   "message": "An unexpected error occurred",
//   "recoverySteps": ["Try again later", "Contact support if issue persists"],
//   "fallbackSuggestion": "Use alternative method"
// }