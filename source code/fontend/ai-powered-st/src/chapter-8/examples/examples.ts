// //Legacy Conquest Service - HTTP Request Examples

// 1. POST /api/legacy-conquest/analyze - Analyze Legacy Codebase
// typescript
// // Example request body
// {
//   "codebase": {
//     "name": "LegacyOrderSystem",
//     "technologyStack": [".NET Framework 4.5", "ASP.NET WebForms", "SQL Server 2012"],
//     "ageYears": 10,
//     "totalLines": 500000,
//     "complexityScore": 8,
//     "dependencies": [
//       { "name": "Newtonsoft.Json", "version": "6.0.8", "isExternal": true },
//       { "name": "EnterpriseLibrary", "version": "5.0", "isExternal": true }
//     ]
//   },
//   "analysisDepth": "comprehensive",
//   "businessContext": {
//     "criticalFlows": [
//       {
//         "id": "flow-001",
//         "description": "Order processing workflow from cart to fulfillment",
//         "businessValue": 10,
//         "involvedSystems": ["Inventory", "Payment", "Shipping"]
//       }
//     ],
//     "stakeholderConcerns": ["Performance", "Security", "Maintainability"],
//     "businessRules": {
//       "order-total": "Must calculate taxes correctly",
//       "inventory-check": "Must validate stock before order"
//     }
//   },
//   "safetyLevel": "balanced",
//   "focusAreas": ["payment-processing", "inventory-management"]
// }

// // Example response
// {
//   "analysisId": "ana-12345",
//   "codebaseSummary": {
//     "name": "LegacyOrderSystem",
//     "totalLines": 500000,
//     "complexityScore": 8,
//     "primaryTechnologies": [".NET Framework 4.5", "ASP.NET WebForms"],
//     "dependencyCount": 42,
//     "technicalDebtEstimate": 0.35
//   },
//   "businessLogicMap": [
//     {
//       "businessFlowId": "flow-001",
//       "businessFlowDescription": "Order processing workflow",
//       "codeLocations": ["OrderController.cs", "OrderService.cs"],
//       "mappingConfidence": 0.85
//     }
//   ],
//   "riskHotspots": [
//     {
//       "id": "risk-001",
//       "location": "PaymentProcessor.cs",
//       "riskType": "Security",
//       "severity": 9,
//       "description": "SQL injection vulnerability in payment processing"
//     }
//   ],
//   "hiddenAssumptions": [
//     {
//       "id": "assume-001",
//       "description": "Payment gateway always available",
//       "location": "PaymentService.cs:45",
//       "impact": "System failure if gateway down",
//       "isValidated": false
//     }
//   ],
//   "modernizationReadiness": {
//     "readinessScore": 0.45,
//     "strengths": ["Modular design", "Clear separation of concerns"],
//     "weaknesses": ["No unit tests", "High coupling"],
//     "opportunities": ["Cloud migration", "Microservices"],
//     "threats": ["Business critical", "Limited documentation"]
//   },
//   "recommendedActions": [
//     {
//       "id": "act-001",
//       "title": "Add characterization tests",
//       "description": "Create tests for payment flow",
//       "priority": 1,
//       "estimatedEffort": "2 weeks"
//     }
//   ],
//   "confidenceScores": [
//     {
//       "metric": "Business Logic Mapping",
//       "score": 0.85,
//       "explanation": "Good match with provided flows"
//     }
//   ],
//   "nextSteps": [
//     {
//       "step": "Review analysis with team",
//       "owner": "Tech Lead",
//       "timeline": "1 week"
//     }
//   ]
// }
// 2. POST /api/legacy-conquest/generate-wrappers - Generate Safe Wrappers
// typescript
// // Example request body
// {
//   "legacyModule": {
//     "name": "PaymentProcessor",
//     "version": "2.1.0",
//     "exposedFunctions": ["ProcessPayment", "ValidateCard", "RefundTransaction"],
//     "complexityScore": 7,
//     "configuration": {
//       "timeout": "30s",
//       "retryCount": "3"
//     }
//   },
//   "wrapperType": "strangler",
//   "safetyLevel": "conservative",
//   "safetyMeasures": [
//     {
//       "type": "circuit-breaker",
//       "configuration": "failureThreshold=5, timeout=60"
//     },
//     {
//       "type": "retry",
//       "configuration": "maxAttempts=3, backoff=exponential"
//     }
//   ],
//   "validationRequirements": [
//     {
//       "name": "Input Validation",
//       "validationType": "input",
//       "isMandatory": true
//     }
//   ],
//   "modernizationStrategy": "strangler-fig"
// }

// // Example response
// {
//   "wrapperId": "wrap-67890",
//   "originalModule": {
//     "name": "PaymentProcessor",
//     "version": "2.1.0",
//     "exposedFunctions": ["ProcessPayment", "ValidateCard", "RefundTransaction"],
//     "complexityScore": 7
//   },
//   "generatedWrapper": {
//     "name": "PaymentProcessorWrapper",
//     "code": "public class PaymentProcessorWrapper { ... }",
//     "language": "C#",
//     "safetyFeatures": [
//       {
//         "name": "CircuitBreaker",
//         "type": "circuit-breaker",
//         "configuration": "failureThreshold=5"
//       }
//     ],
//     "exposedInterfaces": ["IPaymentProcessor"]
//   },
//   "validationTests": [
//     {
//       "id": "test-001",
//       "name": "ProcessPayment_ValidInput_Success",
//       "testCode": "[Fact] public void ProcessPayment_ValidInput_Success() { ... }",
//       "expectedResult": "Success"
//     }
//   ],
//   "migrationPlan": {
//     "strategy": "strangler-fig",
//     "phases": [
//       {
//         "name": "Phase 1",
//         "steps": ["Deploy wrapper", "Monitor"],
//         "successCriteria": "No errors"
//       }
//     ],
//     "estimatedDuration": "3 months"
//   },
//   "safetyAssessment": {
//     "safetyScore": 0.85,
//     "coveredRisks": ["Input validation", "Timeout handling"],
//     "uncoveredRisks": ["State corruption"],
//     "recommendations": ["Add bulkhead pattern"]
//   },
//   "performanceImpact": {
//     "latencyIncrease": 15.5,
//     "memoryOverhead": 25.0,
//     "throughputImpact": 5.0,
//     "bottlenecks": ["Serialization"]
//   },
//   "rollbackStrategy": {
//     "trigger": "Error rate > 5%",
//     "steps": ["Stop traffic", "Revert to direct calls"],
//     "estimatedRevertTime": "10 minutes",
//     "dataPreservationSteps": ["Backup state"]
//   }
// }
// 3. POST /api/legacy-conquest/create-characterization-tests - Create Characterization Tests
// typescript
// // Example request body
// {
//   "observedOutputs": [
//     {
//       "input": "ProcessPayment(amount: 100, currency: 'USD')",
//       "output": "Success: TX123",
//       "timestamp": "2024-01-15T10:30:00Z",
//       "context": "Normal payment flow"
//     },
//     {
//       "input": "ProcessPayment(amount: -50, currency: 'USD')",
//       "output": "Error: Invalid amount",
//       "timestamp": "2024-01-15T10:31:00Z"
//     },
//     {
//       "input": "ProcessPayment(amount: 999999, currency: 'USD')",
//       "output": "Success: TX999",
//       "timestamp": "2024-01-15T10:32:00Z"
//     },
//     {
//       "input": "ProcessPayment(amount: 0, currency: 'USD')",
//       "output": "Success: TX000",
//       "timestamp": "2024-01-15T10:33:00Z"
//     },
//     {
//       "input": "ProcessPayment(amount: 100, currency: 'INVALID')",
//       "output": "Error: Unsupported currency",
//       "timestamp": "2024-01-15T10:34:00Z"
//     }
//   ],
//   "legacyBehavior": {
//     "id": "behavior-001",
//     "description": "Payment processing with validation",
//     "category": "Financial",
//     "knownVariations": ["Credit Card", "PayPal"]
//   },
//   "coverageGoal": 0.85,
//   "includeEdgeCases": true,
//   "testStrategy": "property-based",
//   "generateDocumentation": true
// }

// // Example response
// {
//   "testSuiteId": "suite-54321",
//   "legacyBehavior": {
//     "id": "behavior-001",
//     "description": "Payment processing with validation",
//     "category": "Financial"
//   },
//   "characterizationTests": [
//     {
//       "id": "test-001",
//       "name": "ProcessPayment_ValidAmount_Success",
//       "testCode": "[Fact] public void ProcessPayment_ValidAmount_Success() { ... }",
//       "inputs": ["amount: 100", "currency: USD"],
//       "expectedOutput": "Success",
//       "category": "Happy Path"
//     }
//   ],
//   "documentation": {
//     "overview": "Tests for payment processing behavior",
//     "scenarios": [
//       {
//         "name": "Valid payments",
//         "description": "Tests with valid amounts",
//         "coveredBehaviors": ["Success path"]
//       }
//     ],
//     "assumptions": ["System state is consistent"],
//     "limitations": ["Does not test performance"]
//   },
//   "testHarness": {
//     "setupCode": "// Initialize test environment",
//     "teardownCode": "// Clean up",
//     "requiredMocks": ["ILogger", "IDatabase"],
//     "configuration": { "environment": "test" }
//   },
//   "validationSuite": {
//     "validationRules": ["All tests must pass"],
//     "validationTests": ["test-001", "test-002"],
//     "validationCoverage": 0.85
//   },
//   "coverageReport": {
//     "coveragePercentage": 0.85,
//     "coveredBehaviors": ["Success path", "Error handling"],
//     "uncoveredBehaviors": ["Timeout scenarios"],
//     "recommendations": ["Add timeout tests"]
//   },
//   "confidenceMetrics": [
//     {
//       "metric": "Behavioral Accuracy",
//       "value": 0.85,
//       "justification": "Matches observed outputs"
//     }
//   ],
//   "maintenanceGuide": {
//     "commonIssues": ["Flaky tests"],
//     "troubleshootingSteps": ["Check test data"],
//     "updateProcedures": ["Add new observed outputs"]
//   }
// }
// 4. POST /api/legacy-conquest/plan-modernization - Plan Modernization
// typescript
// // Example request body
// {
//   "legacyAnalysis": {
//     "analysisId": "analysis-123",
//     "analysisDate": "2024-01-15T00:00:00Z",
//     "codebaseInfo": {
//       "name": "LegacyOrderSystem",
//       "technologyStack": [".NET Framework 4.5"],
//       "ageYears": 10,
//       "totalLines": 500000,
//       "complexityScore": 8
//     },
//     "metrics": {
//       "totalFiles": 1500,
//       "totalLinesOfCode": 500000,
//       "totalClasses": 1200,
//       "totalMethods": 8000,
//       "averageComplexity": 12,
//       "maxComplexity": 45,
//       "totalDependencies": 85,
//       "circularDependencies": 5,
//       "codeSmellsCount": 230,
//       "technicalDebtRatio": 0.35,
//       "testCoverage": 0.15,
//       "securityVulnerabilities": 12
//     },
//     "findings": ["High coupling", "No unit tests"],
//     "recommendations": ["Implement repository pattern"]
//   },
//   "businessPriorities": [
//     {
//       "id": "prio-001",
//       "name": "Payment Processing Reliability",
//       "priority": 10,
//       "dependentSystems": ["Payment", "Inventory"]
//     }
//   ],
//   "modernizationApproach": "incremental",
//   "constraints": {
//     "maxDuration": "6 months",
//     "maxCostPerRun": 5000,
//     "maxParallelWorkers": 3,
//     "allowedDowntimeWindows": ["Sunday 2am-4am"]
//   },
//   "successMetrics": [
//     {
//       "name": "Migration Completion",
//       "targetValue": "100%",
//       "measurementMethod": "Component count"
//     }
//   ]
// }

// // Example response
// {
//   "roadmapId": "roadmap-98765",
//   "modernizationApproach": "incremental",
//   "roadmap": {
//     "phases": [
//       {
//         "name": "Foundation",
//         "description": "Establish modernization foundation",
//         "duration": "3 months",
//         "steps": ["Set up CI/CD", "Create characterization tests"],
//         "deliverables": ["Working CI/CD"]
//       }
//     ],
//     "totalDuration": "12 months",
//     "milestones": ["M1: Foundation complete"],
//     "dependencies": { "Phase2": "Phase1" }
//   },
//   "riskAssessment": {
//     "identifiedRisks": [
//       {
//         "name": "Data Migration",
//         "description": "Data loss during migration",
//         "likelihood": "Medium",
//         "impact": "High"
//       }
//     ],
//     "overallRiskLevel": "Medium",
//     "mitigationPlans": [
//       {
//         "riskName": "Data Migration",
//         "strategy": "Backup and verify",
//         "actions": ["Full backup"]
//       }
//     ]
//   },
//   "implementationPlan": {
//     "tasks": [
//       {
//         "id": "T1",
//         "description": "Setup CI/CD",
//         "estimatedHours": "40"
//       }
//     ],
//     "resourceRequirements": ["2 Developers"],
//     "dependencies": { "T2": "T1" }
//   },
//   "successMetrics": {
//     "id": "metrics-123",
//     "name": "Modernization Success Metrics",
//     "description": "Track modernization progress",
//     "createdAt": "2024-01-15T00:00:00Z",
//     "metrics": [
//       {
//         "id": "m1",
//         "name": "Code Quality Index",
//         "category": "technical",
//         "description": "Code quality score",
//         "measurementMethod": "SonarQube",
//         "targetValue": "85",
//         "currentValue": "62",
//         "unit": "percentage",
//         "status": "at-risk",
//         "trend": {
//           "direction": "improving",
//           "changeRate": 0.15,
//           "interpretation": "Steady improvement",
//           "periodStart": "2023-12-15T00:00:00Z",
//           "periodEnd": "2024-01-15T00:00:00Z"
//         }
//       }
//     ],
//     "overallAssessment": {
//       "overallScore": 68,
//       "grade": "C",
//       "summary": "Progressing but needs attention",
//       "keyAchievements": ["Established CI/CD"],
//       "areasForImprovement": ["Increase test coverage"],
//       "riskAssessment": {
//         "overallRiskLevel": "medium",
//         "riskFactors": []
//       },
//       "categoryScores": { "technical": 62 }
//     },
//     "recommendations": ["Improve test coverage"]
//   },
//   "stakeholderCommunication": {
//     "stakeholders": ["Business Owners"],
//     "communicationPlans": [
//       {
//         "audience": "Business Owners",
//         "frequency": "Monthly",
//         "format": "Executive Summary",
//         "content": "Progress report"
//       }
//     ],
//     "keyMessages": ["Modernization reduces debt"]
//   },
//   "monitoringPlan": {
//     "metrics": [
//       {
//         "name": "System Health",
//         "target": ">95%",
//         "measurementMethod": "Health checks"
//       }
//     ],
//     "alertRules": ["Alert if health < 90%"],
//     "reviewFrequency": "Weekly"
//   },
//   "contingencyPlans": [
//     {
//       "trigger": "Critical failure",
//       "actions": ["Rollback"],
//       "owner": "Ops Team"
//     }
//   ]
// }
// 5. POST /api/legacy-conquest/monitor-health - Monitor Legacy Health
// typescript
// // Example request body
// {
//   "monitoredSystems": [
//     {
//       "id": "sys-001",
//       "name": "PaymentProcessor",
//       "type": "LegacyService",
//       "dependencies": ["Database", "PaymentGateway"]
//     },
//     {
//       "id": "sys-002",
//       "name": "InventoryService",
//       "type": "ModernService",
//       "dependencies": ["Database"]
//     }
//   ],
//   "telemetryData": [
//     {
//       "systemId": "sys-001",
//       "timestamp": "2024-01-15T10:30:00Z",
//       "metricName": "CPU Usage",
//       "value": 65.5,
//       "tags": { "environment": "production" }
//     }
//     // ... 100+ data points
//   ],
//   "healthIndicators": [
//     {
//       "name": "CPU Usage",
//       "metricSource": "system.cpu.usage",
//       "warningThreshold": 70,
//       "criticalThreshold": 85
//     }
//   ],
//   "alertThresholds": [
//     {
//       "indicator": "CPU Usage",
//       "level": "warning",
//       "consecutiveOccurrences": 3,
//       "timeWindow": "PT5M"
//     }
//   ]
// }

// // Example response
// {
//   "healthReportId": "health-45678",
//   "monitoredSystems": [
//     {
//       "id": "sys-001",
//       "name": "PaymentProcessor",
//       "type": "LegacyService"
//     }
//   ],
//   "healthScores": [
//     {
//       "systemId": "sys-001",
//       "systemName": "PaymentProcessor",
//       "score": 72.5,
//       "status": "warning",
//       "componentScores": { "CPU": 65, "Memory": 80 }
//     }
//   ],
//   "detectedAnomalies": [
//     {
//       "id": "anom-001",
//       "systemId": "sys-001",
//       "detectedAt": "2024-01-15T10:35:00Z",
//       "metric": "CPU Usage",
//       "expectedValue": 50,
//       "actualValue": 85,
//       "severity": "critical",
//       "description": "CPU spike detected"
//     }
//   ],
//   "predictions": [
//     {
//       "id": "pred-001",
//       "systemId": "sys-001",
//       "predictedIssue": "Memory leak",
//       "predictedTimeframe": "2024-01-16T00:00:00Z",
//       "confidence": 0.75,
//       "suggestedActions": ["Restart service"]
//     }
//   ],
//   "recommendations": [
//     {
//       "id": "rec-001",
//       "title": "Investigate CPU spike",
//       "description": "Review recent deployments",
//       "impact": "High",
//       "effort": "Medium"
//     }
//   ],
//   "alertStatus": {
//     "level": "warning",
//     "activeAlerts": [
//       {
//         "id": "alert-001",
//         "systemId": "sys-001",
//         "type": "CPU Usage",
//         "message": "High CPU usage detected",
//         "triggeredAt": "2024-01-15T10:35:00Z"
//       }
//     ],
//     "recommendedResponses": ["Investigate immediately"]
//   },
//   "trendAnalysis": {
//     "increasingTrends": [
//       {
//         "metric": "CPU Usage",
//         "systemId": "sys-001",
//         "changeRate": 0.23,
//         "interpretation": "Gradual increase",
//         "periodStart": "2024-01-08T00:00:00Z",
//         "periodEnd": "2024-01-15T00:00:00Z"
//       }
//     ],
//     "decreasingTrends": [],
//     "stableTrends": [
//       {
//         "metric": "Memory Usage",
//         "systemId": "sys-002",
//         "changeRate": 0.02,
//         "interpretation": "Stable",
//         "periodStart": "2024-01-08T00:00:00Z",
//         "periodEnd": "2024-01-15T00:00:00Z"
//       }
//     ]
//   },
//   "actionableInsights": [
//     {
//       "category": "Performance",
//       "insight": "CPU usage trending up",
//       "recommendedAction": "Scale resources",
//       "priority": "High"
//     }
//   ]
// }
// Error Responses
// typescript
// // 400 Bad Request - Validation Error
// {
//   "errorType": "analysis-scale",
//   "message": "Codebase too large for comprehensive analysis",
//   "recoverySteps": [
//     "Use 'standard' or 'quick' analysis depth",
//     "Analyze subsystems separately"
//   ],
//   "fallbackSuggestion": "Incremental analysis starting with highest-risk areas"
// }

// // 422 Unprocessable Entity - Business Logic Error
// {
//   "errorType": "complexity-overload",
//   "message": "Codebase too complex for automated analysis",
//   "recoverySteps": [
//     "Simplify analysis focus areas",
//     "Provide more specific code samples"
//   ],
//   "fallbackSuggestion": "Human-led code archaeology with AI assistance",
//   "diagnosticData": {
//     "problematicComplexity": 9.5,
//     "suggestedSimplification": "Break down into smaller modules"
//   }
// }

// // 500 Internal Server Error
// {
//   "errorType": "internal-server-error",
//   "message": "An unexpected error occurred",
//   "recoverySteps": ["Try again later", "Contact support"],
//   "fallbackSuggestion": "Use manual analysis temporarily"
// }
