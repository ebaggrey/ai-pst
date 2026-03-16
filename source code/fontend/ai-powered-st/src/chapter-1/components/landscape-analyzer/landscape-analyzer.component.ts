// components/landscape-analyzer/landscape-analyzer.component.ts (fixed section)
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Subscription } from 'rxjs';

import { LandscapeTestingService } from '../../services/landscape-testing.service';
import {
  ApplicationProfile,
  UserScale,
  TestLandscapeResponse,
  LandscapeError,
  BackendService,
  RiskAssessment
} from '../../models/landscape.models';

@Component({
  selector: 'app-landscape-analyzer',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  templateUrl: './landscape-analyzer.component.html',
  styleUrls: ['./landscape-analyzer.component.css']
})
export class LandscapeAnalyzerComponent implements OnInit, OnDestroy {
  // Form
  analysisForm!: FormGroup;
  
  // State
  isLoading = false;
  isSubmitting = false;
  analysisResult: TestLandscapeResponse | null = null;
  error: LandscapeError | null = null;
  selectedAnalysisId: string | null = null;
  
  // Subscriptions
  private subscriptions = new Subscription();
  
  // Constants for dropdowns - RENAME THIS to avoid conflict with form getter
  architectureTypeOptions = [
    { value: 'microservices', label: 'Microservices' },
    { value: 'monolith', label: 'Monolith' },
    { value: 'serverless', label: 'Serverless' },
    { value: 'hybrid', label: 'Hybrid' }
  ];
  
  focusAreaOptions = [
    { value: 'integration', label: 'Integration Testing' },
    { value: 'performance', label: 'Performance Testing' },
    { value: 'security', label: 'Security Testing' },
    { value: 'ui', label: 'UI Testing' },
    { value: 'api', label: 'API Testing' },
    { value: 'database', label: 'Database Testing' },
    { value: 'e2e', label: 'End-to-End Testing' }
  ];
  
  userScaleOptions = [
    { value: UserScale.Small, label: 'Small (< 1,000 users)' },
    { value: UserScale.Medium, label: 'Medium (1,000 - 10,000 users)' },
    { value: UserScale.Large, label: 'Large (10,000 - 100,000 users)' },
    { value: UserScale.Enterprise, label: 'Enterprise (> 100,000 users)' }
  ];
  
  artifactOptions = [
    { value: 'testScenarios', label: 'Test Scenarios' },
    { value: 'automationScripts', label: 'Automation Scripts' },
    { value: 'monitoringSuggestions', label: 'Monitoring Suggestions' },
    { value: 'riskAnalysis', label: 'Risk Analysis' }
  ];
  
  dataSourceOptions = [
    { value: 'database', label: 'Database' },
    { value: 'external-apis', label: 'External APIs' },
    { value: 'message-queues', label: 'Message Queues' },
    { value: 'file-storage', label: 'File Storage' },
    { value: 'cache', label: 'Cache' }
  ];
  
  constructor(
    private fb: FormBuilder,
    private landscapeService: LandscapeTestingService
  ) {}
  
  ngOnInit(): void {
    this.initializeForm();
    this.loadInitialData();
  }
  
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
  
  private initializeForm(): void {
    this.analysisForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      architectureType: ['microservices', Validators.required],
      frontendFrameworks: this.fb.array([
        this.fb.control('Angular')
      ]),
      backendServices: this.fb.array([
        this.createBackendServiceFormGroup()
      ]),
      dataSources: this.fb.array([
        this.fb.control('database')
      ]),
      expectedUsers: [UserScale.Medium, Validators.required],
      criticalUserJourneys: this.fb.array([
        this.fb.control('User Login', Validators.required)
      ]),
      testingFocus: this.fb.array([
        this.fb.control('integration', Validators.required)
      ], [Validators.required, Validators.minLength(1)]),
      requestedArtifacts: this.fb.array([
        this.fb.control('testScenarios'),
        this.fb.control('automationScripts')
      ]),
      promptVersion: ['1.2', [Validators.required, Validators.pattern(/^\d+\.\d+$/)]],
      includeDetailedAnalysis: [true],
      analysisDepth: ['comprehensive'],
      maxRecommendationsPerArea: [10, [Validators.min(1), Validators.max(100)]],
      riskCriticality: [5, [Validators.min(1), Validators.max(10)]],
      complianceRequirements: this.fb.array([]),
      dataSensitivity: this.fb.array([])
    });
  }
  
  private createBackendServiceFormGroup(): FormGroup {
    return this.fb.group({
      name: ['main-api', Validators.required],
      technology: ['Node.js', Validators.required],
      endpoints: this.fb.array([this.fb.control('/api')]),
      hasDatabase: [true],
      requestRatePerSecond: [100],
      dependencies: this.fb.array([])
    });
  }
  
  // Form array helpers - FIXED: Ensure these return FormArray, not the constant arrays
  get frontendFrameworksArray(): FormArray {
    return this.analysisForm.get('frontendFrameworks') as FormArray;
  }
  
  get backendServicesArray(): FormArray {
    return this.analysisForm.get('backendServices') as FormArray;
  }
  
  get dataSourcesArray(): FormArray {
    return this.analysisForm.get('dataSources') as FormArray;
  }
  
  get criticalUserJourneysArray(): FormArray {
    return this.analysisForm.get('criticalUserJourneys') as FormArray;
  }
  
  get testingFocusArray(): FormArray {
    return this.analysisForm.get('testingFocus') as FormArray;
  }
  
  get requestedArtifactsArray(): FormArray {
    return this.analysisForm.get('requestedArtifacts') as FormArray;
  }
  
  get complianceRequirementsArray(): FormArray {
    return this.analysisForm.get('complianceRequirements') as FormArray;
  }
  
  get dataSensitivityArray(): FormArray {
    return this.analysisForm.get('dataSensitivity') as FormArray;
  }
  
  // Add/remove form array items - UPDATED to use the renamed getters
  addFrontendFramework(): void {
    this.frontendFrameworksArray.push(this.fb.control(''));
  }
  
  removeFrontendFramework(index: number): void {
    this.frontendFrameworksArray.removeAt(index);
  }
  
  addBackendService(): void {
    this.backendServicesArray.push(this.createBackendServiceFormGroup());
  }
  
  removeBackendService(index: number): void {
    if (this.backendServicesArray.length > 1) {
      this.backendServicesArray.removeAt(index);
    }
  }
  
  addDataSource(): void {
    this.dataSourcesArray.push(this.fb.control(''));
  }
  
  removeDataSource(index: number): void {
    this.dataSourcesArray.removeAt(index);
  }
  
  addCriticalUserJourney(): void {
    this.criticalUserJourneysArray.push(this.fb.control('', Validators.required));
  }
  
  removeCriticalUserJourney(index: number): void {
    if (this.criticalUserJourneysArray.length > 1) {
      this.criticalUserJourneysArray.removeAt(index);
    }
  }
  
  addTestingFocus(): void {
    this.testingFocusArray.push(this.fb.control('', Validators.required));
  }
  
  removeTestingFocus(index: number): void {
    if (this.testingFocusArray.length > 1) {
      this.testingFocusArray.removeAt(index);
    }
  }
  
  addComplianceRequirement(): void {
    this.complianceRequirementsArray.push(this.fb.control(''));
  }
  
  removeComplianceRequirement(index: number): void {
    this.complianceRequirementsArray.removeAt(index);
  }
  
  addDataSensitivity(): void {
    this.dataSensitivityArray.push(this.fb.control(''));
  }
  
  removeDataSensitivity(index: number): void {
    this.dataSensitivityArray.removeAt(index);
  }
  
  // Backend service endpoints helpers - UPDATED
  getBackendServiceEndpoints(serviceIndex: number): FormArray {
    return this.backendServicesArray.at(serviceIndex).get('endpoints') as FormArray;
  }
  
  addEndpoint(serviceIndex: number): void {
    this.getBackendServiceEndpoints(serviceIndex).push(this.fb.control(''));
  }
  
  removeEndpoint(serviceIndex: number, endpointIndex: number): void {
    this.getBackendServiceEndpoints(serviceIndex).removeAt(endpointIndex);
  }
  
  // Backend service dependencies helpers - UPDATED
  getBackendServiceDependencies(serviceIndex: number): FormArray {
    return this.backendServicesArray.at(serviceIndex).get('dependencies') as FormArray;
  }
  
  addDependency(serviceIndex: number): void {
    this.getBackendServiceDependencies(serviceIndex).push(this.fb.control(''));
  }
  
  removeDependency(serviceIndex: number, dependencyIndex: number): void {
    this.getBackendServiceDependencies(serviceIndex).removeAt(dependencyIndex);
  }
  
  // Helper method to get label for artifact - ADD THIS
  getArtifactLabel(artifactValue: string): string {
    const artifact = this.artifactOptions.find(a => a.value === artifactValue);
    return artifact ? artifact.label : artifactValue;
  }
  
  private loadInitialData(): void {
    // Optionally load saved data or default values
  }
  
  // HTTP Request Examples for each endpoint
  
  // 1. POST /api/landscape/analyze - Analyze testing landscape
  analyzeLandscape(): void {
    if (this.analysisForm.invalid) {
      this.markFormGroupTouched(this.analysisForm);
      return;
    }
    
    this.isSubmitting = true;
    this.error = null;
    
    const formValue = this.analysisForm.value;
    
    // Build the request object
    const request: any = {
      applicationProfile: {
        name: formValue.name,
        architectureType: formValue.architectureType,
        frontendFrameworks: formValue.frontendFrameworks.filter((f: string) => f && f.trim()),
        backendServicesCount: formValue.backendServices.length,
        backendServices: formValue.backendServices.map((service: any) => ({
          ...service,
          endpoints: service.endpoints?.filter((e: string) => e && e.trim()) || [],
          dependencies: service.dependencies?.filter((d: string) => d && d.trim()) || []
        })),
        dataSources: formValue.dataSources.filter((d: string) => d && d.trim()),
        expectedUsers: formValue.expectedUsers,
        criticalUserJourneys: formValue.criticalUserJourneys.filter((j: string) => j && j.trim()),
        deploymentEnvironment: 'cloud',
        complianceRequirements: formValue.complianceRequirements?.filter((c: string) => c && c.trim()) || [],
        availabilityRequirement: 99.9
      },
      testingFocus: formValue.testingFocus.filter((f: string) => f && f.trim()),
      riskAssessment: {
        criticality: formValue.riskCriticality,
        complianceRequirements: formValue.complianceRequirements?.filter((c: string) => c && c.trim()) || [],
        dataSensitivity: formValue.dataSensitivity?.filter((d: string) => d && d.trim()) || [],
        riskFactors: []
      },
      promptVersion: formValue.promptVersion,
      requestedArtifacts: formValue.requestedArtifacts.filter((a: string) => a && a.trim()),
      includeDetailedAnalysis: formValue.includeDetailedAnalysis,
      analysisDepth: formValue.analysisDepth,
      maxRecommendationsPerArea: formValue.maxRecommendationsPerArea
    };
    
    // HTTP Request Example #1: POST /api/landscape/analyze
    this.subscriptions.add(
      this.landscapeService.generateLandscapeTests(
        request.applicationProfile,
        request.testingFocus,
        request.riskAssessment,
        request.requestedArtifacts
      ).subscribe({
        next: (response) => {
          this.analysisResult = response;
          this.isSubmitting = false;
          this.selectedAnalysisId = response.analysisId;
        },
        error: (error: LandscapeError) => {
          this.error = error;
          this.isSubmitting = false;
          console.error('Analysis error:', error);
        }
      })
    );
  }
  
  // 2. GET /api/landscape/analysis/{id} - Get analysis by ID
  getAnalysisById(analysisId: string): void {
    if (!analysisId) return;
    
    this.isLoading = true;
    this.error = null;
    
    // HTTP Request Example #2: GET /api/landscape/analysis/{id}
    this.subscriptions.add(
      this.landscapeService.getLandscapeAnalysis(analysisId).subscribe({
        next: (response) => {
          this.analysisResult = response;
          this.isLoading = false;
        },
        error: (error: LandscapeError) => {
          this.error = error;
          this.isLoading = false;
          console.error('Failed to fetch analysis:', error);
        }
      })
    );
  }
  
  // 3. GET /api/landscape/health - Health check
  checkHealth(): void {
    // HTTP Request Example #3: GET /api/landscape/health
    this.landscapeService.healthCheck().subscribe({
      next: (response) => {
        console.log('Health check response:', response);
        // You can show a toast notification or update UI
        alert('System is healthy! Check console for details.');
      },
      error: (error) => {
        console.error('Health check failed:', error);
        alert('Health check failed. Check console for details.');
      }
    });
  }
  
  // 4. Example of custom request with additional parameters
  analyzeWithCustomParameters(): void {
    // HTTP Request Example #4: Customized POST request
    const customRequest = {
      ...this.buildRequestFromForm(),
      customParameters: {
        priority: 'high',
        team: 'qa-team',
        project: this.analysisForm.value.name
      },
      includeDetailedAnalysis: true
    };
    
    // This would need a custom endpoint or extended service method
    this.subscriptions.add(
      this.landscapeService.customAnalysisRequest(customRequest).subscribe({
        next: (response) => {
          console.log('Custom analysis response:', response);
          this.analysisResult = response;
        },
        error: (error) => {
          console.error('Custom analysis failed:', error);
          this.error = error;
        }
      })
    );
  }
  
  // Utility method to build request from form
  private buildRequestFromForm(): any {
    const formValue = this.analysisForm.value;
    return {
      applicationProfile: {
        name: formValue.name,
        architectureType: formValue.architectureType,
        frontendFrameworks: formValue.frontendFrameworks,
        backendServices: formValue.backendServices,
        dataSources: formValue.dataSources,
        expectedUsers: formValue.expectedUsers,
        criticalUserJourneys: formValue.criticalUserJourneys
      },
      testingFocus: formValue.testingFocus,
      promptVersion: formValue.promptVersion,
      requestedArtifacts: formValue.requestedArtifacts
    };
  }
  
  // Utility method to mark all form controls as touched
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      } else if (control instanceof FormArray) {
        control.controls.forEach(c => {
          if (c instanceof FormGroup) {
            this.markFormGroupTouched(c);
          } else {
            c.markAsTouched();
          }
        });
      }
    });
  }
  
  // Reset form and results
  resetForm(): void {
    this.analysisForm.reset({
      architectureType: 'microservices',
      expectedUsers: UserScale.Medium,
      promptVersion: '1.2',
      includeDetailedAnalysis: true,
      analysisDepth: 'comprehensive',
      maxRecommendationsPerArea: 10,
      riskCriticality: 5
    });
    
    // Reset form arrays
    this.frontendFrameworksArray.clear();
    this.frontendFrameworksArray.push(this.fb.control('Angular'));
    
    this.backendServicesArray.clear();
    this.backendServicesArray.push(this.createBackendServiceFormGroup());
    
    this.dataSourcesArray.clear();
    this.dataSourcesArray.push(this.fb.control('database'));
    
    this.criticalUserJourneysArray.clear();
    this.criticalUserJourneysArray.push(this.fb.control('User Login', Validators.required));
    
    this.testingFocusArray.clear();
    this.testingFocusArray.push(this.fb.control('integration', Validators.required));
    
    this.requestedArtifactsArray.clear();
    this.requestedArtifactsArray.push(this.fb.control('testScenarios'));
    this.requestedArtifactsArray.push(this.fb.control('automationScripts'));
    
    this.complianceRequirementsArray.clear();
    this.dataSensitivityArray.clear();
    
    this.analysisResult = null;
    this.error = null;
  }
  
  // Get risk level color
  getRiskLevelColor(level: string): string {
    switch (level.toLowerCase()) {
      case 'critical': return 'critical';
      case 'high': return 'high';
      case 'medium': return 'medium';
      case 'low': return 'low';
      default: return 'info';
    }
  }
  
  // Format date
  formatDate(date: Date): string {
    return new Date(date).toLocaleString();
  }
  
  // Format time span
  formatTimeSpan(timeSpan: string): string {
    return timeSpan || 'Not specified';
  }
  
  // Check if there are any backend services
  get hasBackendServices(): boolean {
    return this.backendServicesArray.length > 0;
  }
  
  // Check if form has critical errors
  get hasFormErrors(): boolean {
    return this.analysisForm?.invalid && this.analysisForm?.touched;
  }
}