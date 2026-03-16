
// src/app/components/pipeline-cookbook/pipeline-cookbook.component.ts
import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe, JsonPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { PipelineCookbookService } from '../../Services/pipeline-cookbook.service';
import { environment } from '../../../environments/environment';
import { AdaptationRequest, AdaptationStrategy, DiagnosisDepth, DiagnosisRequest, OptimizationRequest, PipelineGenerationRequest, PredictionRequest, Priority } from 'src/chapter-7/models/request/requests';



@Component({
    selector: 'app-pipeline-cookbook',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
    ],
    templateUrl: './pipeline-cookbook.component.html',
    styleUrls: ['./pipeline-cookbook.component.css']
})
export class PipelineCookbookComponent implements OnInit {
    private fb = inject(FormBuilder);
    private pipelineService = inject(PipelineCookbookService);
    
    environment = environment;

    // Signal state management
    selectedEndpoint = signal<string>('generate-pipeline');
    isLoading = signal<boolean>(false);
    responseData = signal<any>(null);
    errorMessage = signal<string | null>(null);
    responseTime = signal<number | null>(null);

    // Response visibility signals
    showGeneratePipelineResponse = signal<boolean>(false);
    showDiagnoseFailureResponse = signal<boolean>(false);
    showOptimizePerformanceResponse = signal<boolean>(false);
    showPredictIssuesResponse = signal<boolean>(false);
    showAdaptToChangeResponse = signal<boolean>(false);

    // Form groups
    generatePipelineForm!: FormGroup 
    diagnoseFailureForm!: FormGroup 
    optimizePerformanceForm!: FormGroup 
    predictIssuesForm!: FormGroup 
    adaptToChangeForm!: FormGroup 

    // Available options for selects
    diagnosisDepthOptions = [
        { value: DiagnosisDepth.Shallow, label: 'Shallow' },
        { value: DiagnosisDepth.Standard, label: 'Standard' },
        { value: DiagnosisDepth.Deep, label: 'Deep' }
    ];

    adaptationStrategyOptions = [
        { value: AdaptationStrategy.Conservative, label: 'Conservative' },
        { value: AdaptationStrategy.Balanced, label: 'Balanced' },
        { value: AdaptationStrategy.Aggressive, label: 'Aggressive' }
    ];

    priorityOptions = [
        { value: Priority.Low, label: 'Low' },
        { value: Priority.Medium, label: 'Medium' },
        { value: Priority.High, label: 'High' }
    ];

    endpointDescriptions: { [key: string]: string } = {
        'generate-pipeline': 'Generate an intelligent CI/CD pipeline based on codebase analysis',
        'diagnose-failure': 'Diagnose pipeline failures from logs and context',
        'optimize-performance': 'Optimize pipeline performance based on metrics and goals',
        'predict-issues': 'Predict potential pipeline issues based on proposed changes',
        'adapt-to-change': 'Adapt pipeline to accommodate changes'
    };

    constructor() {
        this.initializeForms();
    }

    ngOnInit(): void {
        this.loadExampleData();
    }

    private initializeForms(): void {
        // Generate Pipeline Form
        this.generatePipelineForm = this.fb.group({
            codebaseAnalysis: this.fb.group({
                language: ['C#', Validators.required],
                testCoverage: [0.75, [Validators.required, Validators.min(0), Validators.max(1)]],
                totalLines: [250000, [Validators.required, Validators.min(1)]],
                dependencies: [['Microsoft.NET.Sdk', 'EntityFrameworkCore']]
            }),
            constraints: this.fb.group({
                maxDuration: ['15 minutes', Validators.required],
                maxCostPerRun: [2.50, [Validators.required, Validators.min(0)]]
            }),
            teamPractices: this.fb.group({
                codeReviews: [true],
                automatedTesting: [true],
                deploymentStrategy: [['blue-green', 'canary']]
            }),
            optimizationFocus: this.fb.group({
                speed: [true],
                reliability: [true],
                cost: [false]
            })
        });

        // Diagnose Failure Form
        this.diagnoseFailureForm = this.fb.group({
            failureLogs: this.fb.group({
                rawLogs: ['', Validators.required],
                failureTime: [new Date().toISOString()]
            }),
            recentChanges: this.fb.group({
                changes: this.fb.array([
                    this.createChangeGroup()
                ])
            }),
            diagnosisDepth: [DiagnosisDepth.Deep],
            includeRemediation: [true],
            preventionStrategies: [true],
            pipelineContext: this.fb.group({
                pipelineId: ['pipeline-123'],
                stage: ['build']
            })
        });

        // Optimize Performance Form
        this.optimizePerformanceForm = this.fb.group({
            currentMetrics: this.fb.group({
                averageDuration: ['PT30M', Validators.required],
                successRate: [0.85, [Validators.required, Validators.min(0), Validators.max(1)]],
                resourceUtilization: [0.65, [Validators.required, Validators.min(0), Validators.max(1)]]
            }),
            identifiedBottlenecks: this.fb.array([
                this.createBottleneckGroup()
            ]),
            optimizationGoals: this.fb.array([
                this.createOptimizationGoalGroup()
            ]),
            constraints: this.fb.group({
                maxBudget: [500],
                maxDowntime: ['PT1H']
            })
        });

        // Predict Issues Form - Fixed to match PredictionRequest interface
        this.predictIssuesForm = this.fb.group({
            proposedChanges: this.fb.array([
                this.createProposedChangeGroup()
            ]),
            historicalData: this.fb.group({
                runs: this.fb.array(this.createHistoricalRuns())
            }),
            predictionHorizon: ['P7D', Validators.required],
            confidenceThreshold: [0.7, [Validators.required, Validators.min(0), Validators.max(1)]],
            includeMitigations: [true]
        });

        // Adapt To Change Form
        this.adaptToChangeForm = this.fb.group({
            changeType: ['compliance-requirement', Validators.required],
            impactAssessment: this.fb.group({
                impactScore: [0.9, [Validators.required, Validators.min(0), Validators.max(1)]],
                affectedComponents: [['security-scanning', 'dependency-check']]
            }),
            adaptationStrategy: [AdaptationStrategy.Balanced],
            validationRules: this.fb.array([
                this.createValidationRuleGroup()
            ])
        });
    }

    // Factory methods for form groups
    private createChangeGroup(): FormGroup {
        return this.fb.group({
            id: ['', Validators.required],
            type: ['', Validators.required],
            description: ['']
        });
    }

    private createBottleneckGroup(): FormGroup {
        return this.fb.group({
            stageId: ['', Validators.required],
            description: ['', Validators.required],
            impactScore: [0.5, [Validators.required, Validators.min(0), Validators.max(1)]]
        });
    }

    private createOptimizationGoalGroup(): FormGroup {
        return this.fb.group({
            type: ['', Validators.required],
            targetValue: [0, Validators.required],
            priority: [Priority.Medium, Validators.required]
        });
    }

    private createProposedChangeGroup(): FormGroup {
        return this.fb.group({
            id: ['', Validators.required],
            type: ['', Validators.required],
            description: ['']
        });
    }

    private createPipelineRunGroup(): FormGroup {
        return this.fb.group({
            runId: ['', Validators.required],
            timestamp: [new Date().toISOString(), Validators.required],
            succeeded: [true],
            errors: [[]]
        });
    }

    private createValidationRuleGroup(): FormGroup {
        return this.fb.group({
            ruleId: ['', Validators.required],
            condition: ['', Validators.required]
        });
    }

    private createHistoricalRuns(): FormGroup[] {
        const runs: FormGroup[] = [];
        for (let i = 0; i < 12; i++) {
            runs.push(
                this.fb.group({
                    runId: [`run-00${i + 1}`],
                    timestamp: [new Date(Date.now() - i * 86400000).toISOString()],
                    succeeded: [i % 3 !== 0],
                    errors: [[]]
                })
            );
        }
        return runs;
    }

    private loadExampleData(): void {
    // Set example values for proposed changes
    const proposedChangesArray = this.getProposedChangesArray();
    proposedChangesArray.clear();
    
    // Use setValue or patchValue instead of push with FormGroup
    proposedChangesArray.push(
        this.fb.control({
            id: 'db-change-001',
            type: 'database',
            description: 'Add new Users table with email verification column'
        })
    );
    
    proposedChangesArray.push(
        this.fb.control({
            id: 'api-change-001',
            type: 'api',
            description: 'Add new endpoint for user registration'
        })
    );
    
    // Alternative approach: set the entire array at once
    // proposedChangesArray.setValue([
    //     { id: 'db-change-001', type: 'database', description: 'Add new Users table with email verification column' },
    //     { id: 'api-change-001', type: 'api', description: 'Add new endpoint for user registration' }
    // ]);
}

    // Helper methods for form arrays
    getChangesArray(): FormArray {
        return this.diagnoseFailureForm?.get('recentChanges.changes') as FormArray;
    }

    getBottlenecksArray(): FormArray {
        return this.optimizePerformanceForm?.get('identifiedBottlenecks') as FormArray;
    }

    getOptimizationGoalsArray(): FormArray {
        return this.optimizePerformanceForm?.get('optimizationGoals') as FormArray;
    }

    getProposedChangesArray(): FormArray {
        return this.predictIssuesForm?.get('proposedChanges') as FormArray;
    }

    getHistoricalRunsArray(): FormArray {
        return this.predictIssuesForm?.get('historicalData.runs') as FormArray;
    }

    getValidationRulesArray(): FormArray {
        return this.adaptToChangeForm?.get('validationRules') as FormArray;
    }

    // Add methods
    addChange(): void {
        this.getChangesArray().push(this.createChangeGroup());
    }

    addBottleneck(): void {
        this.getBottlenecksArray().push(this.createBottleneckGroup());
    }

    addOptimizationGoal(): void {
        this.getOptimizationGoalsArray().push(this.createOptimizationGoalGroup());
    }

    addProposedChange(): void {
        this.getProposedChangesArray().push(this.createProposedChangeGroup());
    }

    addHistoricalRun(): void {
        this.getHistoricalRunsArray().push(this.createPipelineRunGroup());
    }

    addValidationRule(): void {
        this.getValidationRulesArray().push(this.createValidationRuleGroup());
    }

    // Remove methods
    removeChange(index: number): void {
        this.getChangesArray().removeAt(index);
    }

    removeBottleneck(index: number): void {
        this.getBottlenecksArray().removeAt(index);
    }

    removeOptimizationGoal(index: number): void {
        this.getOptimizationGoalsArray().removeAt(index);
    }

    removeProposedChange(index: number): void {
        this.getProposedChangesArray().removeAt(index);
    }

    removeHistoricalRun(index: number): void {
        this.getHistoricalRunsArray().removeAt(index);
    }

    removeValidationRule(index: number): void {
        this.getValidationRulesArray().removeAt(index);
    }

    // Form submission methods
    onSubmitGeneratePipeline(): void {
        if (this.generatePipelineForm?.invalid) {
            this.markFormGroupTouched(this.generatePipelineForm);
            return;
        }
        
        this.isLoading.set(true);
        this.errorMessage.set(null);
        this.showGeneratePipelineResponse.set(true);
        
        const startTime = performance.now();
        
        const request: PipelineGenerationRequest = this.generatePipelineForm?.value;
        
        this.pipelineService.generateIntelligentPipeline(request).subscribe({
            next: (response) => {
                this.responseData.set(response);
                this.responseTime.set(performance.now() - startTime);
                this.isLoading.set(false);
            },
            error: (error) => {
                this.errorMessage.set(error.message || 'Failed to generate pipeline');
                this.responseData.set(error);
                this.isLoading.set(false);
            }
        });
    }

    onSubmitDiagnoseFailure(): void {
        if (this.diagnoseFailureForm?.invalid) {
            this.markFormGroupTouched(this.diagnoseFailureForm);
            return;
        }
        
        this.isLoading.set(true);
        this.errorMessage.set(null);
        this.showDiagnoseFailureResponse.set(true);
        
        const startTime = performance.now();
        
        const request: DiagnosisRequest = this.diagnoseFailureForm?.value;
        
        this.pipelineService.diagnosePipelineFailure(request).subscribe({
            next: (response) => {
                this.responseData.set(response);
                this.responseTime.set(performance.now() - startTime);
                this.isLoading.set(false);
            },
            error: (error) => {
                this.errorMessage.set(error.message || 'Failed to diagnose failure');
                this.responseData.set(error);
                this.isLoading.set(false);
            }
        });
    }

    onSubmitOptimizePerformance(): void {
        if (this.optimizePerformanceForm?.invalid) {
            this.markFormGroupTouched(this.optimizePerformanceForm);
            return;
        }
        
        this.isLoading.set(true);
        this.errorMessage.set(null);
        this.showOptimizePerformanceResponse.set(true);
        
        const startTime = performance.now();
        
        const request: OptimizationRequest = this.optimizePerformanceForm?.value;
        
        this.pipelineService.optimizePipelinePerformance(request).subscribe({
            next: (response) => {
                this.responseData.set(response);
                this.responseTime.set(performance.now() - startTime);
                this.isLoading.set(false);
            },
            error: (error) => {
                this.errorMessage.set(error.message || 'Failed to optimize performance');
                this.responseData.set(error);
                this.isLoading.set(false);
            }
        });
    }

    onSubmitPredictIssues(): void {
        if (this.predictIssuesForm?.invalid) {
            this.markFormGroupTouched(this.predictIssuesForm);
            return;
        }
        
        this.isLoading.set(true);
        this.errorMessage.set(null);
        this.showPredictIssuesResponse.set(true);
        
        const startTime = performance.now();
        
        const request: PredictionRequest = this.predictIssuesForm?.value;
        
        this.pipelineService.predictPipelineIssues(request).subscribe({
            next: (response) => {
                this.responseData.set(response);
                this.responseTime.set(performance.now() - startTime);
                this.isLoading.set(false);
            },
            error: (error) => {
                this.errorMessage.set(error.message || 'Failed to predict issues');
                this.responseData.set(error);
                this.isLoading.set(false);
            }
        });
    }

    onSubmitAdaptToChange(): void {
        if (this.adaptToChangeForm?.invalid) {
            this.markFormGroupTouched(this.adaptToChangeForm);
            return;
        }
        
        this.isLoading.set(true);
        this.errorMessage.set(null);
        this.showAdaptToChangeResponse.set(true);
        
        const startTime = performance.now();
        
        const request: AdaptationRequest = this.adaptToChangeForm?.value;
        
        this.pipelineService.adaptPipelineToChange(request).subscribe({
            next: (response) => {
                this.responseData.set(response);
                this.responseTime.set(performance.now() - startTime);
                this.isLoading.set(false);
            },
            error: (error) => {
                this.errorMessage.set(error.message || 'Failed to adapt to change');
                this.responseData.set(error);
                this.isLoading.set(false);
            }
        });
    }

    // Helper methods
    setEndpoint(endpoint: string): void {
        this.selectedEndpoint.set(endpoint);
        this.clearResponse();
    }

    clearResponse(): void {
        this.responseData.set(null);
        this.errorMessage.set(null);
        this.responseTime.set(null);
        this.showGeneratePipelineResponse.set(false);
        this.showDiagnoseFailureResponse.set(false);
        this.showOptimizePerformanceResponse.set(false);
        this.showPredictIssuesResponse.set(false);
        this.showAdaptToChangeResponse.set(false);
    }

    formatJson(data: any): string {
        return JSON.stringify(data, null, 2);
    }

    getEndpointDescription(): string {
        return this.endpointDescriptions[this.selectedEndpoint()] || '';
    }

    private markFormGroupTouched(formGroup: FormGroup): void {
        Object.values(formGroup.controls).forEach(control => {
            control.markAsTouched();
            if (control instanceof FormGroup) {
                this.markFormGroupTouched(control);
            } else if (control instanceof FormArray) {
                control.controls.forEach(c => {
                    if (c instanceof FormGroup) {
                        this.markFormGroupTouched(c);
                    }
                });
            }
        });
    }

    // Event handlers for array inputs
    onDependenciesInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const dependencies = input.value.split(',').map(d => d.trim());
        this.generatePipelineForm?.get('codebaseAnalysis.dependencies')?.setValue(dependencies);
    }

    onDeploymentStrategyInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const strategies = input.value.split(',').map(s => s.trim());
        this.generatePipelineForm?.get('teamPractices.deploymentStrategy')?.setValue(strategies);
    }

    onAffectedComponentsInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const components = input.value.split(',').map(c => c.trim());
        this.adaptToChangeForm?.get('impactAssessment.affectedComponents')?.setValue(components);
    }

    onErrorsInput(event: Event, index: number): void {
        const input = event.target as HTMLInputElement;
        const errors = input.value.split(',').map(e => e.trim());
        this.getHistoricalRunsArray().at(index).get('errors')?.setValue(errors);
    }
}