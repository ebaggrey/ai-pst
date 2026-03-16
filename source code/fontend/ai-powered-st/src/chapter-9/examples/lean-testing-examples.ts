// //Lean Testing Service - HTTP Request Examples

// 1. POST /api/lean-testing/prioritize - Prioritize Testing Effort
// typescript
// // Example request body
// {
//   "features": [
//     {
//       "id": "feat-001",
//       "name": "User Authentication",
//       "description": "Implement OAuth2 authentication flow",
//       "businessValue": 95,
//       "riskLevel": 0.3,
//       "implementationComplexity": 0.6,
//       "testingEffort": 40,
//       "dependencies": [],
//       "attributes": {}
//     },
//     {
//       "id": "feat-002",
//       "name": "Payment Processing",
//       "description": "Handle credit card payments",
//       "businessValue": 100,
//       "riskLevel": 0.8,
//       "implementationComplexity": 0.9,
//       "testingEffort": 60,
//       "dependencies": ["feat-001"],
//       "attributes": {}
//     }
//   ],
//   "constraints": {
//     "maxTimeHours": 80,
//     "maxCost": 10000,
//     "maxParallelTests": 5,
//     "requiredEnvironments": ["staging", "production"],
//     "complianceRules": {}
//   },
//   "prioritizationMethod": "WeightedShortestJobFirst",
//   "maxTestingBudget": 80,
//   "costOfDelay": {
//     "dailyCost": 5000,
//     "delayType": "market-window",
//     "deadline": "2024-12-31T00:00:00.000Z"
//   }
// }

// // Example response
// {
//   "prioritizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "features": [
//     {
//       "id": "feat-002",
//       "name": "Payment Processing",
//       "description": "Handle credit card payments",
//       "businessValue": 100,
//       "riskLevel": 0.8,
//       "implementationComplexity": 0.9,
//       "testingEffort": 60,
//       "dependencies": ["feat-001"],
//       "attributes": {},
//       "priority": 1,
//       "priorityScore": 166.67,
//       "expectedValuePerHour": 1.67,
//       "riskAdjustedValue": 20,
//       "optimizationRationale": ["Score based on WeightedShortestJobFirst method", "Risk adjusted value: 20"]
//     },
//     {
//       "id": "feat-001",
//       "name": "User Authentication",
//       "description": "Implement OAuth2 authentication flow",
//       "businessValue": 95,
//       "riskLevel": 0.3,
//       "implementationComplexity": 0.6,
//       "testingEffort": 40,
//       "dependencies": [],
//       "attributes": {},
//       "priority": 2,
//       "priorityScore": 2.38,
//       "expectedValuePerHour": 2.38,
//       "riskAdjustedValue": 66.5,
//       "optimizationRationale": ["Score based on WeightedShortestJobFirst method", "Risk adjusted value: 66.5"]
//     }
//   ],
//   "reasoning": [
//     "Used WeightedShortestJobFirst prioritization method",
//     "Top feature: Payment Processing with score 166.67",
//     "Risk-adjusted values considered"
//   ],
//   "expectedROI": {
//     "expectedValue": 195,
//     "expectedCost": 80,
//     "ratio": 2.44,
//     "expectedPaybackPeriod": "30.00:00:00"
//   },
//   "confidenceScores": [
//     {
//       "featureId": "feat-001",
//       "score": 0.85,
//       "rationale": "Based on historical data and risk assessment"
//     },
//     {
//       "featureId": "feat-002",
//       "score": 0.85,
//       "rationale": "Based on historical data and risk assessment"
//     }
//   ],
//   "tradeOffs": [
//     {
//       "description": "Speed vs Quality",
//       "options": ["Prioritize quick wins", "Focus on high-value features"],
//       "recommended": "Focus on high-value features"
//     }
//   ],
//   "nextBestAlternatives": [
//     {
//       "name": "Risk-Based Alternative",
//       "features": [],
//       "rationale": "Focus on highest risk first"
//     }
//   ],
//   "resourceAllocation": {
//     "allocations": {
//       "Engineers": 5,
//       "Hours": 80
//     },
//     "timeline": {
//       "Start": "Day 1",
//       "End": "Day 10"
//     }
//   },
//   "riskAssessment": {
//     "overallRiskScore": 0.3,
//     "riskFactors": [
//       {
//         "name": "Technical Debt",
//         "probability": 0.3,
//         "impact": 0.5
//       },
//       {
//         "name": "Resource Constraints",
//         "probability": 0.2,
//         "impact": 0.7
//       }
//     ],
//     "mitigations": [
//       {
//         "risk": "Technical Debt",
//         "strategy": "Refactor incrementally"
//       }
//     ]
//   }
// }
// 2. POST /api/lean-testing/minimal-coverage - Generate Minimal Test Coverage
// typescript
// // Example request body
// {
//   "feature": {
//     "id": "feat-002",
//     "name": "Payment Processing",
//     "description": "Handle credit card payments",
//     "businessValue": 100,
//     "riskLevel": 0.8,
//     "implementationComplexity": 0.9,
//     "testingEffort": 60,
//     "dependencies": ["feat-001"],
//     "attributes": {}
//   },
//   "confidenceTarget": 0.95,
//   "riskProfile": {
//     "highRiskAreas": ["payment", "security", "compliance"],
//     "criticalFlows": ["charge", "refund", "void"],
//     "riskTolerance": 0.1
//   },
//   "constraints": {
//     "maxTestCases": 15,
//     "maxExecutionTimeMinutes": 30,
//     "requiredCoverageTypes": ["functional", "security", "compliance"]
//   },
//   "optimizationGoal": "BalanceRiskAndEffort"
// }

// // Example response
// {
//   "suiteId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "feature": {
//     "id": "feat-002",
//     "name": "Payment Processing",
//     "description": "Handle credit card payments",
//     "businessValue": 100,
//     "riskLevel": 0.8,
//     "implementationComplexity": 0.9,
//     "testingEffort": 60,
//     "dependencies": ["feat-001"],
//     "attributes": {}
//   },
//   "testCases": [
//     {
//       "id": "test-001",
//       "name": "Valid Credit Card Charge",
//       "steps": ["Login", "Enter card", "Submit payment", "Verify success"],
//       "coveredRequirements": ["REQ-PAY-001", "REQ-PAY-002"],
//       "executionFrequency": 50,
//       "lastExecution": "2024-01-15T00:00:00.000Z",
//       "failureRate": 0.05,
//       "maintenanceCost": 2.5,
//       "businessCriticality": 0.9
//     }
//   ],
//   "coverageMetrics": {
//     "functionalCoverage": 0.85,
//     "riskCoverage": 0.75,
//     "valueCoverage": 0.8,
//     "requirementCoverage": 0.9,
//     "coverageGaps": {
//       "untestedRequirements": ["REQ-101", "REQ-102"],
//       "lowCoverageAreas": ["Edge Cases"],
//       "recommendations": ["Add tests for edge cases"]
//     }
//   },
//   "executionPlan": {
//     "executions": [
//       {
//         "testId": "test-001",
//         "scheduledTime": "2025-03-05T00:28:02.749Z",
//         "environment": "Staging",
//         "prerequisites": ["Database seeded"]
//       }
//     ],
//     "totalDurationMinutes": 5,
//     "parallelizationStrategy": "Run in parallel where possible"
//   },
//   "maintenanceGuidance": [
//     "Review tests quarterly",
//     "Update when feature changes",
//     "Monitor flaky tests"
//   ],
//   "confidenceAchieved": 0.8,
//   "efficiencyScore": 0.85,
//   "simplificationOpportunities": [
//     {
//       "testId": "test-001",
//       "suggestion": "Combine similar steps",
//       "impactScore": 0.3,
//       "implementation": "Use test data parameterization"
//     }
//   ]
// }
// 3. POST /api/lean-testing/automation-threshold - Decide Automation Threshold
// typescript
// // Example request body
// {
//   "testScenario": {
//     "id": "test-regression-001",
//     "name": "Payment Flow Regression",
//     "description": "Complete payment processing regression suite",
//     "executionFrequency": 50,
//     "averageDurationMinutes": 45,
//     "testDataRequirements": ["valid_credit_cards", "test_merchant_accounts"]
//   },
//   "automationCost": {
//     "initialCost": 15000,
//     "maintenanceCostPerMonth": 2000,
//     "infrastructureCost": 500,
//     "trainingCost": 3000
//   },
//   "manualCost": {
//     "executionCost": 200,
//     "setupCost": 100,
//     "reportingCost": 50
//   },
//   "roiThreshold": 2.5,
//   "decisionFactors": [
//     { "name": "Test Stability", "weight": 0.3, "score": 8 },
//     { "name": "Execution Frequency", "weight": 0.25, "score": 9 },
//     { "name": "Complexity", "weight": 0.2, "score": 7 },
//     { "name": "Team Expertise", "weight": 0.25, "score": 8 }
//   ]
// }

// // Example response
// {
//   "decisionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "testScenario": {
//     "id": "test-regression-001",
//     "name": "Payment Flow Regression",
//     "description": "Complete payment processing regression suite",
//     "executionFrequency": 50,
//     "averageDurationMinutes": 45,
//     "testDataRequirements": ["valid_credit_cards", "test_merchant_accounts"]
//   },
//   "decision": {
//     "decision": "automate",
//     "confidence": 3.33,
//     "rationale": [
//       "ROI: 3.33x vs threshold 2.5x",
//       "Weighted factor score: 8.05",
//       "Payback period: 8.42 months"
//     ],
//     "factors": [
//       { "name": "Test Stability", "weight": 0.3, "score": 8 },
//       { "name": "Execution Frequency", "weight": 0.25, "score": 9 },
//       { "name": "Complexity", "weight": 0.2, "score": 7 },
//       { "name": "Team Expertise", "weight": 0.25, "score": 8 }
//     ]
//   },
//   "costAnalysis": {
//     "totalAutomationCost": 20500,
//     "totalManualCost": 192000,
//     "breakEvenPoint": 0.11,
//     "automationBreakdown": {
//       "initial": 15000,
//       "recurring": 24000,
//       "maintenance": 2000
//     },
//     "manualBreakdown": {
//       "initial": 100,
//       "recurring": 120000,
//       "maintenance": 600
//     }
//   },
//   "roi": {
//     "roiValue": 3.33,
//     "paybackPeriod": 8.42,
//     "netPresentValue": 171500,
//     "assumptions": [
//       "3-year timeframe",
//       "Linear cost projection",
//       "No discount rate applied"
//     ]
//   },
//   "implementationPlan": {
//     "phases": ["Setup", "Script Development", "Validation"],
//     "resources": [
//       {
//         "resourceType": "Engineer",
//         "quantity": 2,
//         "duration": "10.00:00:00"
//       }
//     ],
//     "timeline": {
//       "startDate": "2025-03-04T00:29:24.494Z",
//       "endDate": "2025-04-03T00:29:24.494Z",
//       "milestones": [
//         {
//           "name": "POC Complete",
//           "date": "2025-03-09T00:29:24.494Z",
//           "deliverables": ["POC Complete"]
//         }
//       ]
//     }
//   },
//   "monitoringPlan": {
//     "metrics": [
//       {
//         "name": "Execution Time",
//         "collectionMethod": "Automated",
//         "frequency": "Daily"
//       }
//     ],
//     "alerts": [
//       {
//         "metric": "Execution Time",
//         "warningThreshold": 10,
//         "criticalThreshold": 20
//       }
//     ],
//     "reviews": {
//       "frequency": "Monthly",
//       "responsible": "QA Lead"
//     }
//   },
//   "alternativeOptions": [
//     {
//       "name": "Partial Automation",
//       "decision": {
//         "decision": "hybrid",
//         "confidence": 0.7,
//         "rationale": [],
//         "factors": []
//       },
//       "rationale": "Automate critical paths only"
//     }
//   ],
//   "reviewTimeline": {
//     "startDate": "2025-03-04T00:29:24.494Z",
//     "endDate": "2025-06-02T00:29:24.494Z",
//     "milestones": [
//       {
//         "name": "First Review",
//         "date": "2025-04-03T00:29:24.494Z",
//         "deliverables": ["First Review"]
//       }
//     ]
//   }
// }
// 4. POST /api/lean-testing/optimize-maintenance - Optimize Test Maintenance
// typescript
// // Example request body
// {
//   "existingTests": {
//     "suiteId": "suite-regression-001",
//     "applicationArea": "Payment Processing",
//     "testCases": [
//       {
//         "id": "test-001",
//         "name": "Valid Credit Card Charge",
//         "steps": ["Login", "Enter card", "Submit payment", "Verify success"],
//         "coveredRequirements": ["REQ-PAY-001", "REQ-PAY-002"],
//         "executionFrequency": 50,
//         "lastExecution": "2024-01-15T00:00:00.000Z",
//         "failureRate": 0.05,
//         "maintenanceCost": 2.5,
//         "businessCriticality": 0.9
//       },
//       {
//         "id": "test-002",
//         "name": "Invalid Credit Card Charge",
//         "steps": ["Login", "Enter invalid card", "Submit payment", "Verify error"],
//         "coveredRequirements": ["REQ-PAY-003"],
//         "executionFrequency": 50,
//         "lastExecution": "2024-01-15T00:00:00.000Z",
//         "failureRate": 0.02,
//         "maintenanceCost": 2.0,
//         "businessCriticality": 0.7
//       }
//     ]
//   },
//   "changeImpact": {
//     "changedComponents": ["payment-gateway", "fraud-detection"],
//     "affectedFeatures": ["Payment Processing"],
//     "impactRadius": 0.6
//   },
//   "optimizationStrategy": "ConsolidateSimilarTests",
//   "allowedActions": ["remove", "consolidate", "simplify"],
//   "preservationRules": [
//     { "ruleId": "COMP-PCI-001", "description": "PCI compliance tests must be preserved", "mustPreserve": true },
//     { "ruleId": "CRIT-PAY-001", "description": "Critical payment flow tests", "mustPreserve": true }
//   ]
// }

// // Example response
// {
//   "optimizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "originalTests": {
//     "suiteId": "suite-regression-001",
//     "applicationArea": "Payment Processing",
//     "testCases": [
//       {
//         "id": "test-001",
//         "name": "Valid Credit Card Charge",
//         "steps": ["Login", "Enter card", "Submit payment", "Verify success"],
//         "coveredRequirements": ["REQ-PAY-001", "REQ-PAY-002"],
//         "executionFrequency": 50,
//         "lastExecution": "2024-01-15T00:00:00.000Z",
//         "failureRate": 0.05,
//         "maintenanceCost": 2.5,
//         "businessCriticality": 0.9
//       },
//       {
//         "id": "test-002",
//         "name": "Invalid Credit Card Charge",
//         "steps": ["Login", "Enter invalid card", "Submit payment", "Verify error"],
//         "coveredRequirements": ["REQ-PAY-003"],
//         "executionFrequency": 50,
//         "lastExecution": "2024-01-15T00:00:00.000Z",
//         "failureRate": 0.02,
//         "maintenanceCost": 2.0,
//         "businessCriticality": 0.7
//       }
//     ]
//   },
//   "optimization": {
//     "actions": [],
//     "optimizedSuite": {
//       "suiteId": "suite-regression-001",
//       "applicationArea": "Payment Processing",
//       "testCases": [
//         {
//           "id": "test-001",
//           "name": "Valid Credit Card Charge",
//           "steps": ["Login", "Enter card", "Submit payment", "Verify success"],
//           "coveredRequirements": ["REQ-PAY-001", "REQ-PAY-002"],
//           "executionFrequency": 50,
//           "lastExecution": "2024-01-15T00:00:00.000Z",
//           "failureRate": 0.05,
//           "maintenanceCost": 2.5,
//           "businessCriticality": 0.9
//         },
//         {
//           "id": "test-002",
//           "name": "Invalid Credit Card Charge",
//           "steps": ["Login", "Enter invalid card", "Submit payment", "Verify error"],
//           "coveredRequirements": ["REQ-PAY-003"],
//           "executionFrequency": 50,
//           "lastExecution": "2024-01-15T00:00:00.000Z",
//           "failureRate": 0.02,
//           "maintenanceCost": 2.0,
//           "businessCriticality": 0.7
//         }
//       ]
//     },
//     "metrics": {
//       "TestCount": 2,
//       "ActionsTaken": 0,
//       "Reduction": 0,
//       "HealthScore": 0
//     }
//   },
//   "savings": {
//     "maintenanceReduction": 0,
//     "costSavings": 0,
//     "timeSavingsHours": 0,
//     "breakdown": {
//       "executionSavings": 0,
//       "maintenanceSavings": 0,
//       "reportingSavings": 0
//     }
//   },
//   "implementationGuidance": {
//     "steps": [],
//     "risks": [],
//     "prerequisites": []
//   },
//   "preservationValidation": {
//     "isValid": true,
//     "violations": [],
//     "warnings": []
//   },
//   "riskAssessment": {
//     "overallRiskScore": 0,
//     "riskFactors": [],
//     "mitigations": []
//   },
//   "monitoringPlan": {
//     "metrics": [],
//     "alerts": [],
//     "reviews": {
//       "frequency": "",
//       "responsible": ""
//     }
//   },
//   "continuousImprovement": {
//     "recommendations": [],
//     "timeline": {
//       "startDate": "0001-01-01T00:00:00",
//       "endDate": "0001-01-01T00:00:00",
//       "milestones": []
//     },
//     "successCriteria": []
//   }
// }
// 5. POST /api/lean-testing/measure-roi - Measure Testing ROI
// typescript
// // Example request body
// {
//   "testInvestments": [
//     { "category": "Automation Setup", "cost": 25000, "date": "2024-01-01T00:00:00.000Z" },
//     { "category": "Test Maintenance", "cost": 5000, "date": "2024-02-01T00:00:00.000Z" },
//     { "category": "Test Execution", "cost": 3000, "date": "2024-02-01T00:00:00.000Z" },
//     { "category": "Tooling", "cost": 2000, "date": "2024-01-15T00:00:00.000Z" }
//   ],
//   "outcomes": [
//     { "category": "Defects Found", "value": 45000, "date": "2024-02-15T00:00:00.000Z", "type": "tangible" },
//     { "category": "Time Saved", "value": 15000, "date": "2024-02-15T00:00:00.000Z", "type": "tangible" },
//     { "category": "Quality Improvement", "value": 10000, "date": "2024-02-15T00:00:00.000Z", "type": "intangible" }
//   ],
//   "measurementPeriod": "P3M",
//   "costCategories": ["Automation", "Maintenance", "Execution", "Tools"],
//   "valueCategories": ["Defect Prevention", "Efficiency", "Quality"],
//   "includeIntangibles": true
// }

// // Example response
// {
//   "analysisId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//   "measurementPeriod": "P3M",
//   "tangibleAnalysis": {
//     "totalCost": 35000,
//     "totalBenefit": 60000,
//     "roi": 0.7142857142857143,
//     "costBreakdown": [
//       {
//         "category": "Automation Setup",
//         "amount": 25000,
//         "percentage": 71.42857142857143
//       },
//       {
//         "category": "Test Maintenance",
//         "amount": 5000,
//         "percentage": 14.285714285714286
//       },
//       {
//         "category": "Test Execution",
//         "amount": 3000,
//         "percentage": 8.571428571428571
//       },
//       {
//         "category": "Tooling",
//         "amount": 2000,
//         "percentage": 5.714285714285714
//       }
//     ],
//     "benefitBreakdown": [
//       {
//         "category": "Defects Found",
//         "amount": 45000,
//         "percentage": 75
//       },
//       {
//         "category": "Time Saved",
//         "amount": 15000,
//         "percentage": 25
//       }
//     ]
//   },
//   "intangibleAnalysis": {
//     "qualityScore": 0,
//     "teamMorale": 0,
//     "customerSatisfaction": 0,
//     "qualitativeBenefits": ["Quality Improvement"]
//   },
//   "overallROI": {
//     "roiValue": 2,
//     "paybackPeriod": 6,
//     "netPresentValue": 30000,
//     "confidence": "High"
//   },
//   "insights": [
//     {
//       "title": "Strong ROI",
//       "description": "ROI of 2x exceeds industry average",
//       "category": "Financial",
//       "impact": 0.9
//     }
//   ],
//   "recommendations": [
//     {
//       "title": "Increase automation",
//       "description": "Automate regression tests to improve ROI",
//       "expectedImpact": 0.25,
//       "priority": "High"
//     }
//   ],
//   "benchmarkComparison": {
//     "industryAverage": 2.5,
//     "bestInClass": 5,
//     "percentile": 75
//   },
//   "forecasting": {
//     "projectedROI": 2.2,
//     "trends": ["Increasing efficiency", "Lower maintenance costs"],
//     "recommendations": ["Scale successful patterns"]
//   },
//   "visualizationData": {
//     "timeSeries": {
//       "ROI": [1, 1.5, 2, 2.5]
//     },
//     "distribution": {
//       "Cost": 35000,
//       "Benefit": 60000
//     },
//     "chartConfig": "{type: 'bar'}"
//   }
// }
// Error Responses Examples
// typescript
// // 400 Bad Request - Validation Error
// {
//   "context": "prioritization",
//   "errorType": "scope-too-large",
//   "leanPrincipleViolated": "Limit Work in Progress",
//   "message": "Cannot effectively prioritize more than 50 features at once",
//   "recoverySteps": [
//     "Prioritize in batches of 10-20 features",
//     "Group related features first",
//     "Use higher-level categories initially"
//   ],
//   "fallbackSuggestion": "Manual prioritization with AI-assisted scoring"
// }

// // 422 Unprocessable Entity - Constraint Impossible
// {
//   "context": "prioritization",
//   "errorType": "constraint-impossible",
//   "leanPrincipleViolated": "Set Realistic Constraints",
//   "message": "Cannot satisfy all constraints with given features",
//   "recoverySteps": [
//     "Increase time budget by 20 hours",
//     "Reduce feature count by 3 features",
//     "Accept lower confidence levels"
//   ],
//   "fallbackSuggestion": "Manual triage with constraint relaxation",
//   "diagnosticData": {
//     "constraintAnalysis": {
//       "totalEffort": 100,
//       "maxAllowed": 80
//     },
//     "suggestedAdjustments": {
//       "reduceFeaturesBy": 3,
//       "increaseTimeBy": 20
//     }
//   }
// }

// // 422 Unprocessable Entity - Coverage Impossible
// {
//   "context": "coverage-generation",
//   "errorType": "coverage-impossible",
//   "leanPrincipleViolated": "Set Achievable Goals",
//   "message": "Cannot achieve confidence target with given constraints",
//   "recoverySteps": [
//     "Increase max test cases to 25",
//     "Reduce confidence target to 75%",
//     "Accept higher risk in untested areas"
//   ],
//   "fallbackSuggestion": "Manual test design with constraint awareness",
//   "diagnosticData": {
//     "coverageGapAnalysis": {
//       "totalScenarios": 30,
//       "covered": 15
//     },
//     "constraintImpact": {
//       "constraint": "Max 15 tests",
//       "achievable": 0.5
//     }
//   }
// }
