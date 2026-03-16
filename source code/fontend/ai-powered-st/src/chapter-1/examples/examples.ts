/*

1. POST /api/landscape/analyze - Analyze Testing Landscape

// Example request body
{
  "applicationProfile": {
    "name": "E-Commerce Platform",
    "architectureType": "microservices",
    "frontendFrameworks": ["Angular", "React"],
    "backendServicesCount": 5,
    "backendServices": [
      {
        "name": "user-service",
        "technology": "Node.js",
        "endpoints": ["/api/users", "/api/auth"],
        "hasDatabase": true
      },
      {
        "name": "product-service",
        "technology": "Java Spring",
        "endpoints": ["/api/products", "/api/inventory"],
        "hasDatabase": true
      }
    ],
    "dataSources": ["database", "external-apis"],
    "expectedUsers": "large",
    "criticalUserJourneys": ["User Login", "Product Checkout", "Payment Processing"],
    "techDebtAreas": {
      "auth": "Legacy authentication needs update"
    }
  },
  "testingFocus": ["integration", "performance", "security"],
  "riskAssessment": {
    "criticality": 7,
    "complianceRequirements": ["GDPR", "PCI-DSS"],
    "dataSensitivity": ["PII", "payment-info"],
    "riskFactors": [
      {
        "area": "payment-integration",
        "likelihood": 6,
        "impact": 9,
        "description": "Third-party payment gateway integration"
      }
    ]
  },
  "promptVersion": "1.2",
  "requestedArtifacts": ["testScenarios", "automationScripts", "monitoringSuggestions"]
}

// Example response
{
  "analysisId": "3f7d1a2e-8b5c-4f6d-9a1e-2c3b4d5e6f7a",
  "highPriorityScenarios": [
    {
      "id": "scenario-1",
      "title": "Concurrent User Checkout Flow",
      "description": "Test multiple users checking out simultaneously",
      "priority": "high",
      "riskAreas": ["concurrency", "payment", "inventory"],
      "steps": [
        "Load test with 100 concurrent users",
        "Add items to cart simultaneously",
        "Process payments in parallel",
        "Verify inventory accuracy"
      ],
      "expectedOutcome": "All transactions complete successfully with correct inventory updates"
    }
  ],
  "recommendedAutomation": [
    {
      "id": "auto-1",
      "name": "Checkout Flow Automation",
      "technologyStack": ["Playwright", "TypeScript", "Jest"],
      "codeSnippet": "test('checkout flow', async () => { ... });",
      "coverage": ["UI interactions", "API calls", "Payment processing"],
      "prerequisites": ["Test accounts", "Payment sandbox access"]
    }
  ],
  "identifiedRisks": [
    {
      "area": "Payment Gateway",
      "riskLevel": "high",
      "description": "Third-party payment API has 99.5% uptime SLA",
      "mitigation": ["Implement circuit breaker", "Add retry logic with exponential backoff"],
      "testApproach": "Test failure scenarios and fallback mechanisms"
    }
  ],
  "flakyPredictions": [
    {
      "testType": "E2E",
      "area": "Checkout",
      "flakinessScore": 7.5,
      "reasons": ["Timing issues with payment confirmation", "Network latency variability"],
      "stabilizationTips": ["Add explicit waits", "Mock payment responses in tests"]
    }
  ],
  "monitoringSuggestions": {
    "metrics": [
      {
        "name": "checkout_success_rate",
        "description": "Percentage of successful checkouts",
        "threshold": 99.5,
        "collectionFrequency": "1 minute"
      }
    ],
    "alerts": [
      {
        "condition": "checkout_success_rate < 98% for 5 minutes",
        "severity": "critical",
        "action": "Page on-call engineer"
      }
    ],
    "dashboards": [
      {
        "name": "Checkout Health",
        "metrics": ["checkout_success_rate", "payment_latency", "cart_abandonment_rate"],
        "refreshInterval": "30 seconds"
      }
    ]
  },
  "summary": "Critical testing strategy focuses on payment integration, concurrency, and third-party API resilience",
  "generatedAt": "2024-01-15T10:30:00Z",
  "estimatedEffort": "02:00:00",
  "confidenceScores": {
    "scenarios": 0.92,
    "automation": 0.88,
    "risks": 0.95,
    "flaky_predictions": 0.78
  },
  "usedLLMProviders": ["claude", "deepseek", "gemini"],
  "nextSteps": "Implement payment integration tests first, then concurrency tests"
}
*/

/*
2. GET /api/landscape/analysis/{id} - Get Analysis by ID

// Example request URL
GET /api/landscape/analysis/3f7d1a2e-8b5c-4f6d-9a1e-2c3b4d5e6f7a

// Example response
{
  "analysisId": "3f7d1a2e-8b5c-4f6d-9a1e-2c3b4d5e6f7a",
  "highPriorityScenarios": [...], // Same structure as POST response
  "summary": "Analysis from 2024-01-15",
  "generatedAt": "2024-01-15T10:30:00Z",
  "estimatedEffort": "02:00:00"
}
*/

/*
3. GET /api/landscape/health - Health Check

// Example request URL
GET /api/landscape/health

// Example response
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "services": ["LandscapeAnalyzer", "LLMOrchestrator", "TestSynthesis"]
}
*/

/*
4. POST /api/landscape/custom-analyze - Custom Analysis with Parameters

// Example request body
{
  "applicationProfile": {
    "name": "Mobile Banking App",
    "architectureType": "hybrid",
    "frontendFrameworks": ["React Native"],
    "backendServicesCount": 3,
    "backendServices": [
      {
        "name": "account-service",
        "technology": ".NET Core",
        "endpoints": ["/api/accounts", "/api/transactions"],
        "hasDatabase": true
      }
    ],
    "dataSources": ["database", "external-apis"],
    "expectedUsers": "enterprise",
    "criticalUserJourneys": ["Login", "Transfer Money", "View Balance"]
  },
  "testingFocus": ["security", "performance"],
  "promptVersion": "1.2",
  "requestedArtifacts": ["testScenarios", "automationScripts"],
  "customParameters": {
    "priority": "high",
    "team": "qa-team",
    "project": "mobile-banking"
  },
  "includeDetailedAnalysis": true
}

// Example response (same structure as POST /analyze)
{
  "analysisId": "8c9d2b3f-4e5a-6f7b-8c9d-0a1b2c3d4e5f",
  "highPriorityScenarios": [...],
  "summary": "Mobile banking security-focused analysis",
  "generatedAt": "2024-01-15T10:35:00Z",
  "estimatedEffort": "03:30:00"
}
*/

/*
5. GET /api/landscape/analyses - List All Analyses (with pagination)

// Example request URL with query parameters
GET /api/landscape/analyses?page=1&limit=10

// Example response
[
  {
    "analysisId": "3f7d1a2e-8b5c-4f6d-9a1e-2c3b4d5e6f7a",
    "summary": "E-Commerce Platform analysis",
    "generatedAt": "2024-01-15T10:30:00Z",
    "estimatedEffort": "02:00:00"
  },
  {
    "analysisId": "8c9d2b3f-4e5a-6f7b-8c9d-0a1b2c3d4e5f",
    "summary": "Mobile Banking App analysis",
    "generatedAt": "2024-01-15T09:15:00Z",
    "estimatedEffort": "03:30:00"
  }
]
*/

/*
6. DELETE /api/landscape/analysis/{id} - Delete Analysis

// Example request URL
DELETE /api/landscape/analysis/3f7d1a2e-8b5c-4f6d-9a1e-2c3b4d5e6f7a

// Example response (204 No Content)
(empty response body)
*/

/*
Error Responses Examples

// 400 Bad Request - Validation Error
{
  "errorCode": "VALIDATION_FAILED",
  "message": "The landscape analysis request has some issues",
  "recoverySteps": [
    "Check required fields",
    "Verify architecture details"
  ],
  "fallbackSuggestion": "Focus on smoke tests for critical paths",
  "timestamp": "2024-01-15T10:30:00Z",
  "context": {
    "validationErrors": {
      "ApplicationProfile.Name": ["Name must be between 2 and 100 characters"],
      "TestingFocus": ["At least one testing focus area is required"]
    }
  },
  "correlationId": "err-123-abc",
  "severity": "error"
}

// 422 Unprocessable Entity - Architecture Analysis Failed
{
  "errorCode": "ARCHITECTURE_UNPROCESSABLE",
  "message": "Couldn't make sense of the application architecture",
  "recoverySteps": [
    "Simplify the architecture description",
    "Focus on one component at a time",
    "Provide more details about integration points"
  ],
  "fallbackSuggestion": "Start with smoke tests for critical paths",
  "timestamp": "2024-01-15T10:30:00Z",
  "severity": "error"
}

// 503 Service Unavailable - LLM Providers Down
{
  "errorCode": "LLM_UNAVAILABLE",
  "message": "All AI services are currently unavailable",
  "recoverySteps": [
    "Try again in a few minutes",
    "Use manual analysis mode"
  ],
  "fallbackSuggestion": "Focus on smoke tests for critical paths only",
  "timestamp": "2024-01-15T10:30:00Z",
  "severity": "warning"
}

// 500 Internal Server Error
{
  "errorCode": "INTERNAL_ANALYSIS_FAILURE",
  "message": "Something went wrong during analysis",
  "recoverySteps": [
    "Contact support with the error code"
  ],
  "timestamp": "2024-01-15T10:30:00Z",
  "context": {
    "errorId": "abc-123-def",
    "timestamp": "2024-01-15T10:30:00Z"
  },
  "severity": "error"
}
*/



