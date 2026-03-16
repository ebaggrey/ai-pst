//Full Spectrum Service - HTTP Request Examples
/*
1. POST /api/full-spectrum/shift-left - Generate Shift-Left Tests

// Example request body
{
  "requirements": {
    "items": [
      {
        "id": "REQ-001",
        "description": "User must be able to login with email and password",
        "testability": 4,
        "acceptanceCriteria": ["Login form exists", "Validation works"]
      }
    ],
    "stakeholders": [
      {
        "id": "STK-001",
        "name": "John Product",
        "role": "Product Owner"
      }
    ]
  },
  "designDocuments": [
    {
      "id": "DES-001",
      "name": "Auth Design",
      "content": "JWT authentication",
      "documentType": "Technical"
    }
  ],
  "shiftDepth": 3,
  "collaborationMode": "Hybrid"
}

// Example response
{
  "artifactsId": "3f2e1d5a-8b7c-4a9d-8e6f-2c1b3a4d5e6f",
  "requirements": {
    "items": [...],
    "stakeholders": [...]
  },
  "acceptanceCriteria": [
    {
      "id": "AC-001",
      "requirementId": "REQ-001",
      "criterion": "Login form should validate email format",
      "isAutomated": true
    }
  ],
  "testScenarios": [
    {
      "id": "TS-001",
      "name": "Valid Login Test",
      "steps": ["Enter email", "Enter password", "Click login"],
      "expectedOutcome": "User redirected to dashboard",
      "tags": ["smoke", "critical"]
    }
  ],
  "testDataRequirements": [
    {
      "id": "TD-001",
      "testScenarioId": "TS-001",
      "dataType": "UserCredentials",
      "sampleData": { "email": "test@example.com", "password": "Test@123" }
    }
  ],
  "riskAssessment": {
    "id": "RA-001",
    "highRisks": [],
    "mediumRisks": [],
    "lowRisks": []
  },
  "collaborationHistory": {
    "mode": "Hybrid",
    "entries": [
      {
        "timestamp": "2024-03-04T10:30:00Z",
        "participant": "John Product",
        "action": "Joined collaboration"
      }
    ]
  },
  "coverageMetrics": {
    "requirementCoverage": 0.85,
    "scenarioCoverage": 0.78,
    "riskCoverage": 0.92
  },
  "implementationGuidance": {
    "steps": ["Implement unit tests first", "Add integration tests"],
    "bestPractices": ["Follow AAA pattern", "Use meaningful test names"],
    "warnings": ["Avoid test interdependence"]
  },
  "validationChecklist": {
    "items": [
      {
        "description": "All acceptance criteria covered",
        "isRequired": true
      }
    ]
  }
}
*/

/*
2. POST /api/full-spectrum/analyze-testability - Analyze Code Testability

// Example request body
{
  "codebase": {
    "repositoryUrl": "https://github.com/company/product.git",
    "branch": "main",
    "totalLines": 15000,
    "files": [
      {
        "path": "src/services/auth.service.ts",
        "content": "export class AuthService { ... }",
        "language": "TypeScript"
      }
    ]
  },
  "testabilityFramework": {
    "name": "SonarQube",
    "version": "9.0",
    "supportedLanguages": ["TypeScript", "C#", "Java"]
  },
  "analysisDepth": "Comprehensive",
  "improvementSuggestions": true,
  "refactoringRecommendations": true
}

// Example response
{
  "analysisId": "7a8b9c0d-1e2f-3a4b-5c6d-7e8f9a0b1c2d",
  "codebase": {
    "repositoryUrl": "https://github.com/company/product.git",
    "branch": "main",
    "totalLines": 15000,
    "files": [...]
  },
  "analysis": {
    "id": "AN-001",
    "codeSmells": [
      {
        "type": "Long Method",
        "location": "auth.service.ts:45",
        "description": "Method has 50+ lines",
        "severity": "Medium"
      }
    ],
    "dependencyIssues": [
      {
        "dependency": "external-lib",
        "issue": "Outdated version",
        "recommendation": "Update to latest"
      }
    ],
    "complexityMetrics": [
      {
        "name": "Cyclomatic Complexity",
        "value": 45,
        "threshold": 50,
        "exceedsThreshold": false
      }
    ]
  },
  "testabilityScore": {
    "score": 75,
    "componentScores": [
      {
        "componentName": "Controllers",
        "score": 80
      }
    ]
  },
  "improvements": [
    {
      "id": "IMP-001",
      "description": "Add unit tests for core logic",
      "category": "Testing",
      "estimatedEffort": 5,
      "impact": 8
    }
  ],
  "refactoringRecommendations": [
    {
      "id": "REF-001",
      "location": "BusinessLogic.cs",
      "currentPattern": "Long method",
      "recommendedPattern": "Extract methods",
      "steps": ["Identify logical blocks", "Extract to private methods"]
    }
  ],
  "impactAssessment": {
    "assessment": "Medium impact on development velocity",
    "areas": [
      {
        "name": "Development Speed",
        "impact": "Positive",
        "confidence": 0.85
      }
    ]
  },
  "implementationRoadmap": {
    "phases": [
      {
        "name": "Quick Wins",
        "tasks": ["Fix code smells", "Add missing tests"],
        "duration": "2 weeks"
      }
    ]
  },
  "monitoringPlan": {
    "metrics": [
      {
        "name": "Code Coverage",
        "collectionMethod": "Automated",
        "target": 80
      }
    ]
  }
}
*/

/*
3. POST /api/full-spectrum/shift-right - Generate Shift-Right Monitors

// Example request body
{
  "productionSystem": {
    "id": "SYS-PROD-001",
    "name": "E-commerce Platform",
    "components": [
      {
        "id": "COMP-API-001",
        "name": "Product API",
        "type": "REST API",
        "dependencies": ["COMP-DB-001"]
      }
    ],
    "environment": "Production"
  },
  "userBehavior": {
    "patterns": [
      {
        "name": "Morning Peak",
        "description": "High traffic 9-11 AM",
        "frequency": 0.8
      }
    ],
    "segments": [
      {
        "id": "SEG-001",
        "criteria": "new_users"
      }
    ]
  },
  "monitoringObjectives": [
    {
      "id": "MON-001",
      "name": "API Response Time",
      "metric": "response_time_ms",
      "threshold": 200
    }
  ],
  "feedbackLoops": [
    {
      "id": "LOOP-001",
      "name": "Auto-scaling",
      "sourceComponent": "COMP-API-001",
      "targetComponent": "COMP-CACHE-001"
    }
  ]
}

// Example response
{
  "monitorsId": "9b8c7d6e-5f4a-3b2c-1d0e-9f8a7b6c5d4e",
  "productionSystem": {
    "id": "SYS-PROD-001",
    "name": "E-commerce Platform",
    "components": [...],
    "environment": "Production"
  },
  "monitors": [
    {
      "id": "MON-001",
      "name": "API Response Time Monitor",
      "type": "Performance",
      "configuration": "{\"threshold\":200,\"interval\":60}",
      "targets": ["COMP-API-001"]
    }
  ],
  "feedbackLoops": [
    {
      "id": "LOOP-001",
      "name": "Auto-scaling",
      "sourceComponent": "COMP-API-001",
      "targetComponent": "COMP-CACHE-001"
    }
  ],
  "incidentResponse": {
    "rules": [
      {
        "condition": "Error rate > 5%",
        "severity": "High",
        "action": "Page on-call"
      }
    ],
    "actionPlans": [
      {
        "name": "High Severity Response",
        "steps": ["Alert team", "Investigate", "Mitigate"]
      }
    ]
  },
  "coverageAssessment": {
    "overallCoverage": 0.85,
    "componentCoverage": [
      {
        "componentName": "Product API",
        "coverage": 0.8
      }
    ]
  },
  "alertConfiguration": {
    "channels": [
      {
        "type": "Email",
        "destination": "team@example.com"
      }
    ],
    "rules": [
      {
        "name": "Critical Alert",
        "condition": "severity == 'High'",
        "severity": "High"
      }
    ]
  },
  "integrationPlan": {
    "steps": [
      {
        "order": 1,
        "description": "Deploy monitoring agents"
      }
    ]
  },
  "costBenefitAnalysis": {
    "estimatedCost": 5000,
    "estimatedBenefit": 15000,
    "roi": 3.0
  }
}
*/

/*
4. POST /api/full-spectrum/create-pipeline - Create Spectrum Pipeline

// Example request body
{
  "developmentStages": [
    {
      "id": "STAGE-001",
      "name": "Development",
      "activities": ["Code", "Unit Tests"],
      "dependencies": []
    },
    {
      "id": "STAGE-002",
      "name": "Integration",
      "activities": ["Integration Tests"],
      "dependencies": ["STAGE-001"]
    }
  ],
  "qualityGates": [
    {
      "id": "GATE-001",
      "name": "Code Quality Gate",
      "conditions": ["Coverage > 80%", "No critical bugs"],
      "action": "Block"
    },
    {
      "id": "GATE-002",
      "name": "Security Gate",
      "conditions": ["No high vulnerabilities"],
      "action": "Block"
    },
    {
      "id": "GATE-003",
      "name": "Performance Gate",
      "conditions": ["Response time < 200ms"],
      "action": "Warn"
    }
  ],
  "spectrumCoverage": "FullSpectrum",
  "feedbackMechanisms": [
    {
      "id": "FEED-001",
      "type": "Slack",
      "channel": "#ci-cd-alerts"
    }
  ]
}

// Example response
{
  "pipelineId": "2c3d4e5f-6a7b-8c9d-0e1f-2a3b4c5d6e7f",
  "pipeline": {
    "stages": [
      {
        "id": "STAGE-001",
        "name": "Development",
        "activities": ["Code", "Unit Tests"],
        "metrics": ["Duration", "Success Rate"]
      }
    ],
    "qualityGates": [
      {
        "id": "GATE-001",
        "name": "Code Quality Gate",
        "conditions": ["Coverage > 80%"],
        "action": "Block"
      }
    ]
  },
  "feedbackConfiguration": {
    "channels": [
      {
        "type": "Slack",
        "configuration": "{}"
      }
    ]
  },
  "implementationPlan": {
    "tasks": [
      {
        "id": "TASK-001",
        "description": "Implement Development stage",
        "order": 1
      }
    ]
  },
  "spectrumCoverage": "FullSpectrum",
  "performanceProjections": {
    "expectedThroughput": 100,
    "expectedDuration": "00:30:00",
    "successRate": 0.95
  },
  "riskAssessment": {
    "risks": [
      {
        "description": "Pipeline bottleneck at integration",
        "probability": 0.3,
        "impact": 0.7,
        "mitigation": "Add parallel execution"
      }
    ]
  },
  "optimizationRecommendations": [
    {
      "area": "Build Time",
      "recommendation": "Parallelize test execution",
      "impact": 40
    }
  ]
}
*/

/*
5. POST /api/full-spectrum/orchestrate-testing - Orchestrate Cross-Spectrum Testing

// Example request body
{
  "testSuite": {
    "id": "SUITE-001",
    "name": "Full Regression Suite",
    "tests": [
      {
        "id": "TEST-001",
        "name": "User Login Test",
        "type": "Integration",
        "tags": ["smoke", "critical"]
      }
    ]
  },
  "executionContext": {
    "environment": "staging",
    "capabilities": {
      "maxParallelTests": 5,
      "supportedEnvironments": ["staging"]
    },
    "timeout": "PT30M"
  },
  "orchestrationStrategy": "Adaptive",
  "failureResponse": "Continue"
}

// Example response
{
  "orchestrationId": "3d4e5f6a-7b8c-9d0e-1f2a-3b4c5d6e7f8a",
  "testSuite": {
    "id": "SUITE-001",
    "name": "Full Regression Suite",
    "tests": [...]
  },
  "orchestration": {
    "id": "ORCH-001",
    "strategy": "Adaptive",
    "executionPlans": [
      {
        "testId": "TEST-001",
        "order": 1,
        "dependencies": []
      }
    ]
  },
  "executionResults": [
    {
      "testId": "TEST-001",
      "status": "passed",
      "duration": "00:00:05",
      "errorMessage": null
    }
  ],
  "processedResults": [
    {
      "testId": "TEST-001",
      "output": { "summary": "Test completed successfully" },
      "issues": []
    }
  ],
  "feedback": {
    "summary": "Executed 1 tests with 1 passing",
    "items": [
      {
        "type": "Success",
        "message": "Test TEST-001: passed"
      }
    ]
  },
  "performanceMetrics": {
    "totalDuration": "00:00:05",
    "totalTests": 1,
    "passedTests": 1,
    "failedTests": 0
  },
  "improvementRecommendations": [
    {
      "category": "Performance",
      "recommendation": "Increase parallel execution",
      "impact": 30
    }
  ],
  "documentationUpdates": [
    {
      "testId": "TEST-001",
      "field": "LastExecutionStatus",
      "update": "passed"
    }
  ]
}
*/

/*
Error Responses

// 400 Bad Request - Validation Error
{
  "context": "shift-left",
  "errorType": "invalid-request",
  "spectrumLocation": "far-left",
  "message": "Requirements are required",
  "recoverySteps": ["Provide valid requirements"],
  "fallbackSuggestion": "Ensure requirements are properly formatted"
}

// 422 Unprocessable Entity - Requirement Ambiguity
{
  "context": "shift-left",
  "errorType": "requirement-ambiguity",
  "spectrumLocation": "far-left",
  "message": "Requirements are too ambiguous for automated test generation",
  "recoverySteps": [
    "Clarify ambiguous requirements: REQ-001, REQ-002",
    "Add concrete examples"
  ],
  "fallbackSuggestion": "Manual test design with requirement workshops",
  "diagnosticData": {
    "ambiguousRequirements": ["REQ-001", "REQ-002"],
    "clarificationQuestions": ["What is the expected format?", "When should this happen?"]
  }
}

// 422 Unprocessable Entity - Framework Incompatibility
{
  "context": "testability-analysis",
  "errorType": "framework-incompatibility",
  "spectrumLocation": "left",
  "message": "Testability framework not suitable for this codebase",
  "recoverySteps": [
    "Use different testability framework",
    "Customize framework for this technology stack"
  ],
  "fallbackSuggestion": "Manual code review with testability checklist",
  "diagnosticData": {
    "frameworkIssues": ["Language not supported"],
    "technologyMismatches": ["Python not in supported languages"]
  }
}

// 422 Unprocessable Entity - Monitoring Complexity
{
  "context": "shift-right",
  "errorType": "monitoring-complexity",
  "spectrumLocation": "far-right",
  "message": "Production system too complex for automated monitoring generation",
  "recoverySteps": [
    "Focus on critical components first",
    "Simplify monitoring objectives"
  ],
  "fallbackSuggestion": "Manual monitoring design with expert consultation",
  "diagnosticData": {
    "complexityIssues": ["Too many components: 75", "Complex dependencies detected"],
    "recommendedSimplifications": ["Focus on critical components first"]
  }
}

// 422 Unprocessable Entity - Pipeline Conflict
{
  "context": "pipeline-creation",
  "errorType": "pipeline-conflict",
  "spectrumLocation": "center",
  "message": "Conflicts detected in pipeline configuration",
  "recoverySteps": [
    "Resolve stage dependencies: STAGE-002, STAGE-003",
    "Adjust quality gate requirements"
  ],
  "fallbackSuggestion": "Manual pipeline design with dependency analysis",
  "diagnosticData": {
    "conflictingStages": ["STAGE-002", "STAGE-003"],
    "dependencyIssues": ["Circular dependency detected"]
  }
}

// 422 Unprocessable Entity - Orchestration Complexity
{
  "context": "test-orchestration",
  "errorType": "orchestration-complexity",
  "spectrumLocation": "full-spectrum",
  "message": "Test suite too complex for automated orchestration",
  "recoverySteps": [
    "Simplify test suite structure",
    "Break into smaller orchestration batches"
  ],
  "fallbackSuggestion": "Manual test execution with phased approach",
  "diagnosticData": {
    "complexityFactors": ["Too many tests: 150", "Complex test dependencies"],
    "simplificationSuggestions": ["Batch tests into smaller groups"]
  }
}

// 500 Internal Server Error
{
  "context": "shift-left",
  "errorType": "internal-error",
  "spectrumLocation": "far-left",
  "message": "An unexpected error occurred",
  "recoverySteps": ["Please try again", "Contact support if issue persists"],
  "fallbackSuggestion": "Manual test generation"
}
*/