// components/legacy-analysis/legacy-analysis.component.ts

import { Component, OnInit } from '@angular/core';
import { LegacyConquestService } from '../../services/legacy-conquest.service';
import { CharacterizationRequest, HealthRequest, 
        LegacyAnalysisRequest, RoadmapRequest, 
        TelemetryDataPoint, 
        WrapperRequest} from 'src/chapter-8/models/legacy-conquest.models';


@Component({
    selector: 'app-legacy-analysis',
    templateUrl: './legacy-analysis.component.html'
})
export class LegacyAnalysisComponent implements OnInit {
    loading = false;
    error: string | null = null;

    constructor(private legacyService: LegacyConquestService) {}

    ngOnInit(): void {}

    // ============= EXAMPLE 1: Analyze Legacy Codebase =============
    async exampleAnalyzeLegacyCodebase() {
        this.loading = true;
        this.error = null;

        const request: LegacyAnalysisRequest = {
            codebase: {
                name: "LegacyOrderSystem",
                technologyStack: [".NET Framework 4.5", "ASP.NET WebForms", "SQL Server 2012"],
                ageYears: 10,
                totalLines: 500000,
                complexityScore: 8,
                dependencies: [
                    { name: "Newtonsoft.Json", version: "6.0.8", isExternal: true },
                    { name: "EnterpriseLibrary", version: "5.0", isExternal: true }
                ]
            },
            analysisDepth: "comprehensive",
            businessContext: {
                criticalFlows: [
                    {
                        id: "flow-001",
                        description: "Order processing workflow from cart to fulfillment",
                        businessValue: 10,
                        involvedSystems: ["Inventory", "Payment", "Shipping"]
                    },
                    {
                        id: "flow-002",
                        description: "Customer account management and authentication",
                        businessValue: 8,
                        involvedSystems: ["CRM", "Auth"]
                    }
                ],
                stakeholderConcerns: ["Performance", "Security", "Maintainability"],
                businessRules: {
                    "order-total": "Must calculate taxes correctly",
                    "inventory-check": "Must validate stock before order"
                }
            },
            safetyLevel: "balanced",
            focusAreas: ["payment-processing", "inventory-management"]
        };

        try {
            const response = await this.legacyService.analyzeLegacyCodebase(request).toPromise();
            console.log('Analysis Response:', response);
            
            // Handle the response
            console.log(`Found ${response?.riskHotspots.length} risk hotspots`);
            console.log(`Readiness Score: ${response?.modernizationReadiness?.readinessScore ?? 0 * 100}%`);
            
        } catch (error: any) {
            this.error = error.message;
            console.error('Analysis failed:', error);
        } finally {
            this.loading = false;
        }
    }

    // ============= EXAMPLE 2: Generate Wrappers =============
    async exampleGenerateWrappers() {
        this.loading = true;

        const request: WrapperRequest = {
            legacyModule: {
                name: "PaymentProcessor",
                version: "2.1.0",
                exposedFunctions: ["ProcessPayment", "ValidateCard", "RefundTransaction"],
                complexityScore: 7,
                configuration: {
                    "timeout": "30s",
                    "retryCount": "3"
                }
            },
            wrapperType: "strangler",
            safetyLevel: "conservative",
            safetyMeasures: [
                {
                    type: "circuit-breaker",
                    configuration: "failureThreshold=5, timeout=60"
                },
                {
                    type: "retry",
                    configuration: "maxAttempts=3, backoff=exponential"
                },
                {
                    type: "timeout",
                    configuration: "duration=5000"
                }
            ],
            validationRequirements: [
                {
                    name: "Input Validation",
                    validationType: "input",
                    isMandatory: true
                },
                {
                    name: "Payment Gateway Response",
                    validationType: "output",
                    isMandatory: true
                }
            ],
            modernizationStrategy: "strangler-fig"
        };

        try {
            const response = await this.legacyService.generateSafeWrappers(request).toPromise();
            console.log('Wrapper Response:', response);
            
            console.log(`Generated wrapper with ${response?.generatedWrapper.safetyFeatures.length} safety features`);
            console.log(`Safety Score: ${response?.safetyAssessment?.safetyScore ?? 0 * 100}%`);
            
        } catch (error) {
            console.error('Wrapper generation failed:', error);
        } finally {
            this.loading = false;
        }
    }

    // ============= EXAMPLE 3: Create Characterization Tests =============
    async exampleCreateCharacterizationTests() {
        this.loading = true;

        const request: CharacterizationRequest = {
            observedOutputs: [
                {
                    input: "ProcessPayment(amount: 100, currency: 'USD')",
                    output: "Success: Transaction ID: TX123",
                    timestamp: new Date('2024-01-15T10:30:00'),
                    context: "Normal payment flow"
                },
                {
                    input: "ProcessPayment(amount: -50, currency: 'USD')",
                    output: "Error: Invalid amount",
                    timestamp: new Date('2024-01-15T10:31:00'),
                    context: "Negative amount test"
                },
                {
                    input: "ProcessPayment(amount: 999999, currency: 'USD')",
                    output: "Success: Transaction ID: TX999",
                    timestamp: new Date('2024-01-15T10:32:00'),
                    context: "Large amount"
                },
                {
                    input: "ProcessPayment(amount: 0, currency: 'USD')",
                    output: "Success: Transaction ID: TX000",
                    timestamp: new Date('2024-01-15T10:33:00'),
                    context: "Zero amount"
                },
                {
                    input: "ProcessPayment(amount: 100, currency: 'INVALID')",
                    output: "Error: Unsupported currency",
                    timestamp: new Date('2024-01-15T10:34:00'),
                    context: "Invalid currency"
                }
            ],
            legacyBehavior: {
                id: "behavior-001",
                description: "Payment processing with validation and error handling",
                category: "Financial",
                knownVariations: ["Credit Card", "PayPal", "Bank Transfer"]
            },
            coverageGoal: 0.85,
            includeEdgeCases: true,
            testStrategy: "property-based",
            generateDocumentation: true
        };

        try {
            const response = await this.legacyService.createCharacterizationTests(request).toPromise();
            console.log('Characterization Response:', response);
            
            console.log(`Created ${response?.characterizationTests.length} tests`);
            console.log(`Coverage: ${response?.coverageReport?.coveragePercentage ?? 0 * 100}%`);
            
        } catch (error) {
            console.error('Test creation failed:', error);
        } finally {
            this.loading = false;
        }
    }

    // ============= EXAMPLE 4: Plan Modernization =============
    async examplePlanModernization() {
        this.loading = true;

        const request: RoadmapRequest = {
            legacyAnalysis: {
                analysisId: "analysis-123",
                analysisDate: new Date(),
                codebaseInfo: {
                    name: "LegacyOrderSystem",
                    technologyStack: [".NET Framework 4.5"],
                    ageYears: 10,
                    totalLines: 500000,
                    complexityScore: 8
                },
                metrics: {
                    totalFiles: 1500,
                    totalLinesOfCode: 500000,
                    totalClasses: 1200,
                    totalMethods: 8000,
                    averageComplexity: 12,
                    maxComplexity: 45,
                    totalDependencies: 85,
                    circularDependencies: 5,
                    codeSmellsCount: 230,
                    technicalDebtRatio: 0.35,
                    testCoverage: 0.15,
                    securityVulnerabilities: 12
                },
                findings: [
                    "High coupling in payment module",
                    "No unit tests for business logic",
                    "SQL injection vulnerabilities"
                ],
                recommendations: [
                    "Implement repository pattern",
                    "Add integration tests",
                    "Upgrade to .NET 6"
                ]
            },
            businessPriorities: [
                {
                    id: "prio-001",
                    name: "Payment Processing Reliability",
                    priority: 10,
                    dependentSystems: ["Payment", "Inventory"]
                },
                {
                    id: "prio-002",
                    name: "Order Management Performance",
                    priority: 8,
                    dependentSystems: ["Order", "Inventory"]
                }
            ],
            modernizationApproach: "incremental",
            constraints: {
                maxDuration: "6 months",
                maxCostPerRun: 5000,
                maxParallelWorkers: 3,
                allowedDowntimeWindows: ["Sunday 2am-4am", "Wednesday 3am-5am"]
            },
            successMetrics: [
                {
                    name: "Migration Completion",
                    targetValue: "100%",
                    measurementMethod: "Component count"
                },
                {
                    name: "System Uptime",
                    targetValue: "99.9%",
                    measurementMethod: "Monitoring"
                }
            ]
        };

        try {
            const response = await this.legacyService.planIncrementalModernization(request).toPromise();
            console.log('Modernization Response:', response);
            
            console.log(`Roadmap has ${response?.roadmap.phases.length} phases`);
            console.log(`Total Duration: ${response?.roadmap.totalDuration}`);
            
        } catch (error) {
            console.error('Modernization planning failed:', error);
        } finally {
            this.loading = false;
        }
    }

    // ============= EXAMPLE 5: Monitor Health =============
    async exampleMonitorHealth() {
        this.loading = true;

        const request: HealthRequest = {
            monitoredSystems: [
                {
                    id: "sys-001",
                    name: "PaymentProcessor",
                    type: "LegacyService",
                    dependencies: ["Database", "PaymentGateway"]
                },
                {
                    id: "sys-002",
                    name: "InventoryService",
                    type: "ModernService",
                    dependencies: ["Database"]
                }
            ],
            telemetryData: this.generateTelemetryData(150), // Generate 150 data points
            healthIndicators: [
                {
                    name: "CPU Usage",
                    metricSource: "system.cpu.usage",
                    warningThreshold: 70,
                    criticalThreshold: 85
                },
                {
                    name: "Memory Usage",
                    metricSource: "system.memory.usage",
                    warningThreshold: 80,
                    criticalThreshold: 90
                },
                {
                    name: "Error Rate",
                    metricSource: "application.errors.count",
                    warningThreshold: 5,
                    criticalThreshold: 10
                }
            ],
            alertThresholds: [
                {
                    indicator: "CPU Usage",
                    level: "warning",
                    consecutiveOccurrences: 3,
                    timeWindow: "PT5M" // 5 minutes
                },
                {
                    indicator: "Error Rate",
                    level: "critical",
                    consecutiveOccurrences: 2,
                    timeWindow: "PT1M" // 1 minute
                }
            ]
        };

        try {
            const response = await this.legacyService.monitorLegacyHealth(request).toPromise();
            console.log('Health Response:', response);
            
            const healthy = response?.healthScores.filter(s => s.status === 'healthy').length;
            const warning = response?.healthScores.filter(s => s.status === 'warning').length;
            const critical = response?.healthScores.filter(s => s.status === 'critical').length;
            
            console.log(`Health Status - Healthy: ${healthy}, Warning: ${warning}, Critical: ${critical}`);
            console.log(`Alert Level: ${response?.alertStatus.level}`);
            
        } catch (error) {
            console.error('Health monitoring failed:', error);
        } finally {
            this.loading = false;
        }
    }

    // Helper method to generate sample telemetry data
    private generateTelemetryData(count: number): TelemetryDataPoint[] {
        const data: TelemetryDataPoint[] = [];
        const now = new Date();
        
        for (let i = 0; i < count; i++) {
            data.push({
                systemId: i % 2 === 0 ? "sys-001" : "sys-002",
                timestamp: new Date(now.getTime() - (i * 60000)), // Every minute
                metricName: ["CPU Usage", "Memory Usage", "Error Rate"][Math.floor(Math.random() * 3)],
                value: Math.random() * 100,
                tags: {
                    environment: "production",
                    region: "us-east-1"
                }
            });
        }
        
        return data;
    }
}