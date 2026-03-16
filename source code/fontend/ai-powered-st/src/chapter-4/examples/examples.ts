// AI Testing Service - HTTP Request Examples
// 1. POST /api/ai-testing/assess-capabilities - Assess AI Capabilities
// typescript
// // Example request body
// {
//   "provider": "openai",
//   "rigorLevel": "standard",
//   "dimensions": ["accuracy", "speed", "consistency", "cost-efficiency"],
//   "modelName": "gpt-4",
//   "maxTokens": 1000,
//   "includeBenchmarks": true
// }

// // Example response
// {
//   "provider": "openai",
//   "modelName": "gpt-4",
//   "overallScore": 87.5,
//   "dimensionScores": {
//     "accuracy": 92.3,
//     "speed": 78.9,
//     "consistency": 85.7,
//     "cost-efficiency": 93.1
//   },
//   "metrics": [
//     {
//       "name": "Response Accuracy",
//       "category": "Quality",
//       "score": 92.3,
//       "weight": 1.0
//     }
//   ],
//   "recommendations": [
//     {
//       "area": "Speed Optimization",
//       "suggestion": "Consider using gpt-4-turbo for faster responses",
//       "priority": "medium",
//       "impact": "35% faster response times"
//     }
//   ],
//   "assessmentDate": "2024-03-04T10:30:00.000Z"
// }
// 2. POST /api/ai-testing/test-robustness - Test Prompt Robustness
// typescript
// // Example request body
// {
//   "basePrompt": "Explain quantum computing",
//   "variations": [
//     "Explain quantum computing to a 5-year-old",
//     "Explain quantum computing to a college student",
//     "Explain quantum computing with an analogy"
//   ],
//   "numberOfRuns": 10,
//   "provider": "openai",
//   "modelName": "gpt-4"
// }

// // Example response
// {
//   "basePrompt": "Explain quantum computing",
//   "variationCount": 3,
//   "runCount": 10,
//   "averageConsistencyScore": 82.4,
//   "variationResults": [
//     {
//       "variation": "Explain quantum computing to a 5-year-old",
//       "consistencyScore": 78.5,
//       "responses": ["Response 1...", "Response 2..."],
//       "passed": true
//     }
//   ],
//   "varianceAnalysis": {
//     "overallVariance": 0.15,
//     "highVarianceVariations": ["Explain to an expert"],
//     "stableVariations": ["Explain with an analogy"]
//   },
//   "antipatterns": [
//     {
//       "pattern": "Technical jargon overload",
//       "description": "Too much technical terminology",
//       "severity": "medium",
//       "fix": "Simplify language for general audience"
//     }
//   ],
//   "optimizationSuggestions": [
//     {
//       "area": "Prompt Structure",
//       "suggestion": "Add more specific examples",
//       "expectedImprovement": 12.5
//     }
//   ]
// }
// 3. POST /api/ai-testing/detect-bias - Detect AI Bias
// typescript
// // Example request body
// {
//   "contextAreas": ["hiring", "lending", "healthcare"],
//   "detectionMethods": ["statistical", "content-analysis", "demographic-parity"],
//   "sensitivityThreshold": 0.75,
//   "requireStatisticalSignificance": true,
//   "demographicData": {
//     "ageGroups": "18-30,31-50,51+",
//     "genders": "male,female,non-binary",
//     "ethnicities": "white,black,asian,hispanic"
//   }
// }

// // Example response
// {
//   "findings": [
//     {
//       "context": "hiring",
//       "biasType": "gender",
//       "confidence": 0.82,
//       "evidence": "80% of recommended candidates were male",
//       "severity": "high"
//     }
//   ],
//   "overallBiasScore": 24.5,
//   "contextBiasScores": {
//     "hiring": 32.1,
//     "lending": 18.3,
//     "healthcare": 23.1
//   },
//   "mitigationStrategies": [
//     {
//       "findingId": "hiring-gender",
//       "strategy": "Balance training data with more female examples",
//       "implementation": "Add 500 female candidate profiles to dataset",
//       "timeline": "short-term"
//     }
//   ],
//   "longTermMonitoringPlan": {
//     "metrics": ["gender-parity", "ethnicity-distribution"],
//     "frequency": "weekly",
//     "triggers": ["new-data", "model-update"],
//     "reportingFormat": "dashboard"
//   },
//   "statisticalSignificanceValidated": true
// }
// 4. POST /api/ai-testing/test-hallucinations - Test for Hallucinations
// typescript
// // Example request body
// {
//   "provider": "openai",
//   "knownFacts": [
//     "The Earth orbits the Sun",
//     "Water boils at 100°C at sea level",
//     "Shakespeare wrote Hamlet",
//     "The capital of France is Paris",
//     "Python is a programming language"
//   ],
//   "maxAllowedHallucinations": 3,
//   "verificationSources": ["wikipedia", "britannica", "google-scholar"],
//   "testIterations": 20
// }

// // Example response
// {
//   "provider": "openai",
//   "hallucinations": [
//     {
//       "fact": "The Earth orbits the Sun",
//       "aiResponse": "The Earth orbits around the Moon", 
//       "severity": "critical",
//       "category": "factual",
//       "correction": "The Earth orbits the Sun (verified)"
//     }
//   ],
//   "hallucinationRate": 0.15,
//   "confidenceAdjustments": [
//     {
//       "context": "astronomy",
//       "adjustmentFactor": 0.6,
//       "reason": "High hallucination rate in astronomy topics"
//     }
//   ],
//   "verificationRules": [
//     {
//       "pattern": "*astronomy*",
//       "verificationMethod": "cross-reference",
//       "confidenceLevel": "high",
//       "autoVerify": true
//     }
//   ],
//   "totalTests": 20,
//   "hallucinationCount": 3
// }
// 5. POST /api/ai-testing/monitor-drift - Monitor AI Drift
// typescript
// // Example request body
// {
//   "baseline": {
//     "testResults": [
//       {
//         "testId": "test_001",
//         "timestamp": "2024-02-04T10:30:00.000Z",
//         "metrics": {
//           "accuracy": 0.88,
//           "latency": 450,
//           "consistency": 0.92
//         },
//         "status": "completed"
//       }
//     ],
//     "collectedOn": "2024-02-04T00:00:00.000Z",
//     "environment": "production"
//   },
//   "timeframe": "30d",
//   "metricsToMonitor": ["accuracy", "latency", "consistency"],
//   "driftThreshold": 0.15,
//   "minimumDataPoints": 100,
//   "documentedScenarios": ["normal-load", "peak-load", "error-recovery"],
//   "implementedBehavior": [
//     {
//       "scenario": "normal-load",
//       "expectedBehavior": "accuracy > 85%",
//       "actualBehavior": "accuracy 82-88%"
//     }
//   ],
//   "detectionMethods": ["statistical", "threshold-based"],
//   "sensitivity": 0.75,
//   "autoSuggestFixes": true
// }

// // Example response
// {
//   "startDate": "2024-02-04T00:00:00.000Z",
//   "endDate": "2024-03-04T00:00:00.000Z",
//   "driftSignificance": 0.23,
//   "metricDrifts": {
//     "accuracy": {
//       "metricName": "accuracy",
//       "baselineValue": 0.88,
//       "currentValue": 0.82,
//       "driftAmount": -0.06,
//       "direction": "negative",
//       "significant": true
//     }
//   },
//   "alerts": [
//     {
//       "alertId": "alert_001",
//       "metric": "accuracy",
//       "severity": "warning",
//       "message": "Accuracy has dropped 6% below baseline",
//       "triggeredAt": "2024-03-04T10:30:00.000Z"
//     }
//   ],
//   "recommendedActions": [
//     {
//       "action": "Retrain model with recent data",
//       "priority": "high",
//       "impact": "High - should restore accuracy",
//       "effort": "medium"
//     }
//   ],
//   "baselineComparison": {
//     "dataPointsCompared": 150,
//     "similarityScore": 0.77,
//     "comparisonDate": "2024-03-04T10:30:00.000Z"
//   }
// }
// 6. GET /api/ai-testing/test-history - Get Test History
// typescript
// // Example request parameters
// {
//   "provider": "openai",
//   "startDate": "2024-02-01T00:00:00.000Z",
//   "endDate": "2024-03-01T00:00:00.000Z",
//   "limit": 10
// }

// // Example response
// [
//   {
//     "testId": "test_001",
//     "testType": "capability-assessment",
//     "provider": "openai",
//     "timestamp": "2024-03-01T10:30:00.000Z",
//     "status": "completed",
//     "summary": "Overall score: 87.5"
//   },
//   {
//     "testId": "test_002",
//     "testType": "bias-detection",
//     "provider": "openai",
//     "timestamp": "2024-03-02T14:20:00.000Z",
//     "status": "completed",
//     "summary": "Found 2 bias findings"
//   }
// ]
// 7. GET /api/ai-testing/test-status/{testId} - Get Test Status
// typescript
// // Example request URL
// GET /api/ai-testing/test-status/test_001

// // Example response for in-progress test
// {
//   "testId": "test_001",
//   "status": "in-progress",
//   "progress": 65,
//   "estimatedTimeRemaining": 120,
//   "currentPhase": "analyzing-results"
// }

// // Example response for completed test
// {
//   "testId": "test_001",
//   "status": "completed",
//   "progress": 100,
//   "completedAt": "2024-03-04T10:30:00.000Z",
//   "result": {
//     "overallScore": 87.5,
//     "summary": "Assessment completed successfully"
//   }
// }
// Error Responses Examples
// typescript
// // 400 Bad Request - Validation Error
// {
//   "testType": "capability-assessment",
//   "provider": "openai",
//   "failureMode": "validation",
//   "diagnosticInfo": {
//     "errorCode": 400,
//     "errorMessage": "Too many dimensions for thorough assessment",
//     "suggestedInvestigation": [
//       "Reduce dimensions to 10 or fewer",
//       "Use standard rigor level"
//     ]
//   },
//   "fallbackAction": "auto-reduce-dimensions"
// }

// // 503 Service Unavailable - AI Provider Overload
// {
//   "testType": "capability-assessment",
//   "provider": "openai",
//   "failureMode": "timeout",
//   "diagnosticInfo": {
//     "errorCode": 503,
//     "errorMessage": "AI service cannot handle assessment load",
//     "suggestedInvestigation": [
//       "Try during off-peak hours",
//       "Reduce test rigor",
//       "Use cached results"
//     ]
//   },
//   "fallbackAction": "schedule-for-later",
//   "metadata": {
//     "testCaseId": "550e8400-e29b-41d4-a716-446655440000",
//     "inputPrompt": "Capability assessment suite",
//     "expectedBehavior": "Complete assessment without service overload"
//   }
// }

// // 500 Internal Server Error
// {
//   "testType": "drift-monitoring",
//   "failureMode": "unexpected",
//   "diagnosticInfo": {
//     "errorCode": 500,
//     "errorMessage": "An unexpected error occurred",
//     "suggestedInvestigation": [
//       "Check application logs",
//       "Contact support"
//     ]
//   },
//   "fallbackAction": "retry-or-contact-support"
// }
