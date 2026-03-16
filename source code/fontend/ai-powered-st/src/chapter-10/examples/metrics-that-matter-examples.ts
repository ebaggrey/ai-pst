
//Metrics That Matter Service - HTTP Request Examples
/*
1. POST /api/metrics-that-matter/design-metrics - Design Impactful Metrics

// Example request body
{
  "businessObjectives": [
    "Increase code coverage to 80% by Q3",
    "Reduce production defects by 50%",
    "Improve deployment frequency to weekly"
  ],
  "testingActivities": [
    "Unit testing",
    "Integration testing",
    "End-to-end testing",
    "Performance testing"
  ],
  "designPrinciples": [
    "Actionable",
    "Measurable",
    "Timely"
  ],
  "constraints": {
    "maxMetrics": 5,
    "minDataPoints": 30,
    "allowCompositeMetrics": true
  }
}

// Example response
{
  "designId": "550e8400-e29b-41d4-a716-446655440000",
  "metrics": [
    {
      "metricId": "code-coverage-001",
      "name": "Code Coverage",
      "description": "Percentage of code covered by tests",
      "formula": "(covered_lines / total_lines) * 100",
      "dataSource": "CI Pipeline",
      "businessObjectives": ["Increase code coverage to 80% by Q3"],
      "targetValue": 80,
      "unit": "%",
      "dimensions": ["team", "time"],
      "weight": 0.4,
      "category": "Quality"
    }
  ],
  "relationships": [],
  "collectionPlan": {
    "collectionSteps": ["Configure CI pipeline to report coverage"],
    "tools": ["SonarQube", "Jenkins"],
    "frequency": "daily",
    "responsibleTeam": "DevOps"
  },
  "interpretationFramework": {
    "interpretationRules": ["Values above 80% indicate good coverage"],
    "thresholds": [
      {
        "metricId": "code-coverage-001",
        "green": 80,
        "yellow": 60,
        "red": 40
      }
    ],
    "outlierHandling": ["Remove values beyond 3 standard deviations"]
  },
  "validationRules": [],
  "implementationGuidance": {
    "prerequisites": ["Set up data collection pipelines"],
    "steps": ["Phase 1: Deploy collection mechanisms"],
    "pitfalls": ["Avoid measuring too many metrics"],
    "successFactors": ["Clear ownership"]
  },
  "successCriteria": {
    "quantitativeCriteria": ["95% data completeness"],
    "qualitativeCriteria": ["Stakeholder satisfaction"],
    "reviewPeriod": "Quarterly"
  }
}
*/

/*
2. POST /api/metrics-that-matter/calculate-health - Calculate Testing Health Score

// Example request body
{
  "metricValues": [
    {
      "metricId": "code-coverage-001",
      "metricName": "Code Coverage",
      "value": 75.5,
      "timestamp": "2024-01-15T10:00:00Z",
      "attributes": { "team": "backend" }
    },
    {
      "metricId": "defect-rate-001",
      "metricName": "Defect Rate",
      "value": 12.3,
      "timestamp": "2024-01-15T10:00:00Z",
      "attributes": { "severity": "critical" }
    }
  ],
  "historicalBaselines": [
    {
      "metricId": "code-coverage-001",
      "mean": 70.2,
      "standardDeviation": 5.1,
      "min": 60.0,
      "max": 82.0,
      "sampleSize": 90,
      "periodStart": "2023-10-01T00:00:00Z",
      "periodEnd": "2023-12-31T00:00:00Z"
    }
  ],
  "normalizationMethod": "z-score",
  "weightingStrategy": "statistical",
  "confidenceThreshold": 0.85
}

// Example response
{
  "healthScoreId": "660e8400-e29b-41d4-a716-446655440001",
  "overallScore": 78.5,
  "componentScores": [
    {
      "componentName": "Code Coverage",
      "score": 75.5,
      "weight": 0.4,
      "status": "warning"
    },
    {
      "componentName": "Defect Rate",
      "score": 82.1,
      "weight": 0.6,
      "status": "healthy"
    }
  ],
  "confidence": 0.85,
  "contributingFactors": [
    {
      "factor": "Code Coverage",
      "impact": -2.5,
      "direction": "negative"
    }
  ],
  "recommendations": [
    {
      "metricId": "code-coverage-001",
      "recommendation": "Improve Code Coverage by focusing on critical paths",
      "impact": 5.2,
      "priority": "medium",
      "steps": ["Add tests for error handling", "Cover edge cases"]
    }
  ],
  "trendAnalysis": {
    "direction": "improving",
    "rate": 0.05,
    "observations": ["Code coverage increasing by 2% per month"]
  },
  "benchmarkComparison": {
    "percentile": 65,
    "gapToAverage": 3.2,
    "gapToBest": -8.5
  },
  "alertStatus": {
    "level": "info",
    "alerts": ["Code coverage below target"],
    "recommendedAction": "Review testing strategy"
  }
}
*/

/*
3. POST /api/metrics-that-matter/predict-trends - Predict Quality Trends

// Example request body
{
  "currentMetrics": [
    {
      "metricId": "code-coverage-001",
      "metricName": "Code Coverage",
      "value": 75.5,
      "timestamp": "2024-01-15T10:00:00Z",
      "attributes": {}
    }
  ],
  "historicalTrends": [
    {
      "date": "2024-01-01T00:00:00Z",
      "metricValues": {
        "code-coverage-001": 72.1
      },
      "seasonality": "weekly"
    },
    {
      "date": "2024-01-08T00:00:00Z",
      "metricValues": {
        "code-coverage-001": 73.8
      },
      "seasonality": "weekly"
    },
    {
      "date": "2024-01-15T00:00:00Z",
      "metricValues": {
        "code-coverage-001": 75.5
      },
      "seasonality": "weekly"
    }
  ],
  "predictionHorizon": 30,
  "confidenceIntervals": [0.8, 0.9, 0.95],
  "includeInterventions": true
}

// Example response
{
  "predictionId": "770e8400-e29b-41d4-a716-446655440002",
  "currentState": {
    "analysisDate": "2024-01-15T10:00:00Z",
    "currentValues": {
      "code-coverage-001": 75.5
    },
    "strengths": ["Consistent weekly improvement"],
    "weaknesses": ["Still below 80% target"]
  },
  "predictions": [
    {
      "metricId": "code-coverage-001",
      "metricName": "Code Coverage",
      "dates": ["2024-01-22T00:00:00Z", "2024-01-29T00:00:00Z"],
      "values": [77.2, 79.1],
      "lowerBound": [75.1, 76.8],
      "upperBound": [79.3, 81.4],
      "trend": "upward"
    }
  ],
  "predictionConfidence": {
    "averageConfidence": 0.87,
    "metricConfidence": {
      "code-coverage-001": 0.87
    },
    "confidenceFactors": ["Strong historical pattern", "Low variability"]
  },
  "interventions": [
    {
      "metricId": "code-coverage-001",
      "type": "accelerator",
      "description": "Add automated test generation",
      "recommendedDate": "2024-02-01T00:00:00Z",
      "expectedImpact": 5.0
    }
  ],
  "riskAssessment": {
    "overallRisk": "low",
    "identifiedRisks": ["Prediction accuracy decreases beyond 30 days"],
    "mitigationStrategies": ["Update predictions weekly"]
  },
  "monitoringRecommendations": [
    {
      "metricId": "code-coverage-001",
      "frequency": "weekly",
      "threshold": "±5%"
    }
  ],
  "decisionSupport": {
    "decisionPoints": ["Consider intervention to accelerate coverage"],
    "recommendedActions": ["Implement automated test generation"],
    "tradeoffs": {
      "cost": "$10,000 investment vs 5% improvement"
    }
  }
}
*/

/*
4. POST /api/metrics-that-matter/generate-insights - Generate Actionable Insights

// Example request body
{
  "metrics": [
    {
      "metricId": "code-coverage-001",
      "metricName": "Code Coverage",
      "value": 75.5,
      "timestamp": "2024-01-15T10:00:00Z",
      "attributes": { "team": "backend" }
    },
    {
      "metricId": "defect-rate-001",
      "metricName": "Defect Rate",
      "value": 12.3,
      "timestamp": "2024-01-15T10:00:00Z",
      "attributes": { "severity": "critical" }
    }
  ],
  "insightTypes": ["trend", "anomaly", "correlation", "opportunity"],
  "actionabilityThreshold": 0.75,
  "context": {
    "stakeholders": ["Engineering Manager", "QA Lead"],
    "businessGoals": ["Quality Improvement", "Faster Delivery"],
    "priorityLevel": "high"
  }
}

// Example response
{
  "insightId": "880e8400-e29b-41d4-a716-446655440003",
  "generatedInsights": [
    {
      "insightId": "insight-001",
      "title": "Correlation between Code Coverage and Defect Rate",
      "description": "Teams with >70% coverage have 40% fewer critical defects",
      "type": "correlation",
      "actionabilityScore": 0.85,
      "impact": 0.75,
      "affectedMetrics": ["code-coverage-001", "defect-rate-001"],
      "recommendedActions": [
        "Focus on coverage for critical paths",
        "Set team-specific coverage targets"
      ],
      "priority": "high"
    },
    {
      "insightId": "insight-002",
      "title": "Defect Rate Anomaly Detected",
      "description": "Spike in defects on Wednesdays after deployments",
      "type": "anomaly",
      "actionabilityScore": 0.82,
      "impact": 0.65,
      "affectedMetrics": ["defect-rate-001"],
      "recommendedActions": [
        "Review deployment process",
        "Add pre-deployment validation"
      ],
      "priority": "medium"
    }
  ],
  "implementationGuidance": {
    "prerequisites": ["Team alignment", "Data validation"],
    "steps": ["Review insights with team", "Create action items"],
    "resources": ["Analytics tools", "Implementation team"]
  },
  "insightQuality": {
    "accuracy": 0.88,
    "relevance": 0.92,
    "timeliness": 0.95,
    "limitations": ["Based on 3 months of data"]
  },
  "validationSteps": [
    {
      "step": "Data Quality Check",
      "method": "Verify data sources",
      "expectedOutcome": "Data is complete"
    }
  ],
  "communicationPlan": {
    "stakeholders": ["Engineering Manager", "QA Lead"],
    "message": "Identified 2 actionable insights with high impact potential",
    "channel": "Weekly Review",
    "timeline": "2024-01-22T15:00:00Z"
  },
  "monitoringPlan": {
    "metrics": ["insight-001 - action taken", "insight-002 - action taken"],
    "reviewFrequency": "Weekly",
    "successCriteria": "Implement both insights within 4 weeks"
  }
}
*/

/*
5. POST /api/metrics-that-matter/optimize-collection - Optimize Metric Collection

// Example request body
{
  "currentMetrics": [
    {
      "metricId": "code-coverage-001",
      "name": "Code Coverage",
      "category": "Quality",
      "collectionMethod": "Automated CI Pipeline",
      "collectionCost": 25.0,
      "businessValue": 85.0,
      "dependencies": []
    },
    {
      "metricId": "defect-rate-001",
      "name": "Defect Rate",
      "category": "Quality",
      "collectionMethod": "Manual Tracking",
      "collectionCost": 75.0,
      "businessValue": 90.0,
      "dependencies": []
    },
    {
      "metricId": "test-execution-001",
      "name": "Test Execution Time",
      "category": "Performance",
      "collectionMethod": "Automated Monitoring",
      "collectionCost": 15.0,
      "businessValue": 60.0,
      "dependencies": []
    }
  ],
  "resourceConstraints": [
    {
      "resourceType": "Manual Tracking",
      "maxAllocation": 50.0,
      "unit": "hours/week",
      "period": "monthly"
    }
  ],
  "optimizationGoals": ["cost-reduction", "efficiency"],
  "preservationRules": [
    {
      "metricId": "defect-rate-001",
      "rule": "Must track critical defects",
      "reason": "Regulatory requirement"
    }
  ]
}

// Example response
{
  "optimizationId": "990e8400-e29b-41d4-a716-446655440004",
  "currentMetrics": [
    {
      "metricId": "code-coverage-001",
      "name": "Code Coverage",
      "category": "Quality",
      "collectionMethod": "Automated CI Pipeline",
      "collectionCost": 25.0,
      "businessValue": 85.0,
      "dependencies": []
    }
  ],
  "optimization": {
    "recommendedActions": [
      {
        "metricId": "defect-rate-001",
        "action": "modify",
        "rationale": "High cost, consider automation",
        "costSaving": 40.0
      },
      {
        "metricId": "test-execution-001",
        "action": "keep",
        "rationale": "Low cost, reasonable value",
        "costSaving": 0
      }
    ],
    "consolidations": [
      {
        "sourceMetricIds": ["code-coverage-001"],
        "targetMetricId": "quality-index-001",
        "consolidationMethod": "weighted-average"
      }
    ],
    "deprecatedMetrics": [],
    "newMetrics": ["automation-potential-index"]
  },
  "expectedBenefits": {
    "costReduction": 0.25,
    "efficiencyGain": 0.15,
    "qualityImprovement": 0.05,
    "additionalBenefits": ["Reduced manual effort"]
  },
  "implementationPlan": {
    "phases": [
      "Phase 1: Automate defect tracking",
      "Phase 2: Consolidate quality metrics"
    ],
    "timeline": "4 weeks",
    "dependencies": ["Stakeholder approval"],
    "rollbackSteps": ["Revert to manual tracking"]
  },
  "preservationValidation": {
    "isPreserved": true,
    "validatedRules": ["defect-rate-001 - preserved"],
    "warnings": []
  },
  "riskAssessment": {
    "risks": ["Automation may miss some defects"],
    "mitigations": ["Run parallel manual/automated for 2 weeks"]
  },
  "monitoringPlan": {
    "keyIndicators": ["Cost savings", "Defect detection rate"],
    "reviewFrequency": "Weekly for first month"
  },
  "continuousImprovement": {
    "reviewCycles": ["Weekly review", "Monthly optimization review"],
    "feedbackLoops": ["Automated alerts", "Quarterly surveys"]
  }
}
*/

/*
Error Responses

// 400 Bad Request - Validation Error
{
  "errorType": "insufficient-metrics",
  "message": "Need at least 3 metrics for meaningful measurement",
  "recoverySteps": [
    "Increase max metrics to 5-7",
    "Accept limited coverage",
    "Focus on critical areas only"
  ],
  "fallbackSuggestion": "Use minimum viable metric set"
}

// 422 Unprocessable Entity - Business Logic Error
{
  "errorType": "objective-ambiguity",
  "message": "Business objectives are too vague for measurable metric design",
  "recoverySteps": [
    "Make objectives SMART",
    "Provide concrete success criteria",
    "Add quantitative targets"
  ],
  "fallbackSuggestion": "Qualitative assessment with subjective metrics",
  "diagnosticData": {
    "ambiguousObjectives": ["Improve quality"],
    "clarificationQuestions": [
      "What specific metric would indicate success?",
      "What is the target value or range?"
    ]
  }
}

// 500 Internal Server Error - Unexpected Error
{
  "errorType": "unexpected-error",
  "message": "An unexpected error occurred. Please try again later.",
  "recoverySteps": [
    "Retry the operation",
    "Check your input data",
    "Contact support if issue persists"
  ],
  "fallbackSuggestion": "Use manual alternative"
}
*/