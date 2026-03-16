import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, DecimalPipe, JsonPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Subscription } from 'rxjs';

import { PatternEstablishmentService } from '../../services/pattern-establishment.service';
import { 
  TestingPattern, 
  TrainingMaterials, 
  PipelineBlueprint,
  HealthStatus,
  PatternError,
  TestExample
} from '../../services/pattern-establishment.service';

// Component Models
export interface ComponentState {
  isLoading: boolean;
  error: PatternError | null;
  successMessage: string;
  activeTab: TabType;
}

export type TabType = 'establish' | 'training' | 'pipeline' | 'patterns' | 'health';

export interface TrainingAudience {
  id: string;
  name: string;
  description: string;
}

export interface PipelineTemplate {
  id: string;
  name: string;
  description: string;
  stages: string[];
}

@Component({
  selector: 'app-pattern-establishment',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,

  ],
  templateUrl: '../../components/pattern-establishment/pattern-establishment.component.html',
  styleUrls: ['../../components/pattern-establishment/pattern-establishment.component.css']
})
export class PatternEstablishmentComponent implements OnInit, OnDestroy {
  // State
  public state: ComponentState = {
    isLoading: false,
    error: null,
    successMessage: '',
    activeTab: 'establish'
  };

  // Forms
  public establishForm: FormGroup;
  public trainingForm: FormGroup;
  public pipelineForm: FormGroup;
  public patternSearchForm: FormGroup;

  // Data
  public createdPattern: TestingPattern | null = null;
  public trainingMaterials: TrainingMaterials | null = null;
  public pipelineBlueprint: PipelineBlueprint | null = null;
  public patterns: TestingPattern[] = [];
  public healthStatus: HealthStatus | null = null;

  // Options for dropdowns
  public audiences: TrainingAudience[];
  public pipelineTemplates: PipelineTemplate[];
  public consistencyOptions: string[];
  public automationOptions: string[];
  public triggerEvents: string[];

  private subscriptions: Subscription;

  constructor(
    private fb: FormBuilder,
    private patternService: PatternEstablishmentService
  ) {
    // Initialize arrays
    this.audiences = [
      { id: 'developers', name: 'Developers', description: 'Focus on implementation' },
      { id: 'qa', name: 'QA Engineers', description: 'Focus on testing strategies' },
      { id: 'devops', name: 'DevOps', description: 'Focus on automation' },
      { id: 'managers', name: 'Managers', description: 'Focus on metrics and adoption' },
      { id: 'all', name: 'All Teams', description: 'Comprehensive training' }
    ];

    this.pipelineTemplates = [
      { 
        id: 'basic', 
        name: 'Basic Pipeline', 
        description: 'Simple validation and testing',
        stages: ['validate', 'test', 'report']
      },
      { 
        id: 'ai-enhanced', 
        name: 'AI-Enhanced Pipeline', 
        description: 'Includes AI generation and optimization',
        stages: ['validate', 'ai-generate', 'human-review', 'deploy']
      },
      { 
        id: 'enterprise', 
        name: 'Enterprise Pipeline', 
        description: 'Full pipeline with quality gates',
        stages: ['validate', 'ai-generate', 'security-scan', 'performance-test', 'human-review', 'deploy', 'monitor']
      }
    ];

    this.consistencyOptions = ['low', 'medium', 'high'];
    this.automationOptions = ['manual', 'semi-automated', 'fully-automated'];
    this.triggerEvents = ['pr-created', 'schedule-daily', 'schedule-weekly', 'manual', 'push-to-main'];

    this.subscriptions = new Subscription();

    // Initialize forms
    this.establishForm = this.initializeEstablishForm();
    this.trainingForm = this.initializeTrainingForm();
    this.pipelineForm = this.initializePipelineForm();
    this.patternSearchForm = this.initializeSearchForm();
  }

  ngOnInit(): void {
    this.checkHealth();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private initializeEstablishForm(): FormGroup {
    return this.fb.group({
      area: ['', [Validators.required, Validators.minLength(3)]],
      desiredConsistency: ['high', Validators.required],
      automationLevel: ['semi-automated', Validators.required],
      validationCriteria: this.fb.array([
        this.fb.control('Pattern should be repeatable', Validators.required),
        this.fb.control('Easy for other team members to follow', Validators.required),
        this.fb.control('Produces consistent results', Validators.required),
        this.fb.control('Documented with examples', Validators.required)
      ]),
      metadata: this.fb.group({
        teamSize: [5, [Validators.required, Validators.min(1), Validators.max(100)]],
        experienceLevel: ['intermediate', Validators.required],
        timeline: ['2 weeks', Validators.required]
      }),
      examples: this.fb.array([this.createExampleFormGroup()])
    });
  }

  private initializeTrainingForm(): FormGroup {
    return this.fb.group({
      audience: ['developers', Validators.required],
      format: ['workshop-ready', Validators.required],
      durationMinutes: [60, [Validators.required, Validators.min(15), Validators.max(480)]],
      includeHandsOn: [true],
      prerequisites: this.fb.array([
        this.fb.control('basic testing knowledge'),
        this.fb.control('familiarity with our app')
      ]),
      learningObjectives: this.fb.array([
        this.fb.control('Understand when to use the pattern', Validators.required),
        this.fb.control('Implement the pattern correctly', Validators.required),
        this.fb.control('Troubleshoot common issues', Validators.required),
        this.fb.control('Extend the pattern for edge cases', Validators.required)
      ])
    });
  }

  private initializePipelineForm(): FormGroup {
    return this.fb.group({
      patternId: ['', Validators.required],
      template: ['ai-enhanced'],
      triggerEvents: this.fb.array([
        this.fb.control('pr-created'),
        this.fb.control('schedule-daily'),
        this.fb.control('manual')
      ]),
      customStages: this.fb.array([]),
      qualityGates: this.fb.array([
        this.fb.group({
          metric: ['test-pass-rate', Validators.required],
          threshold: [95, [Validators.required, Validators.min(0), Validators.max(100)]]
        }),
        this.fb.group({
          metric: ['generation-confidence', Validators.required],
          threshold: [80, [Validators.required, Validators.min(0), Validators.max(100)]]
        }),
        this.fb.group({
          metric: ['reviewer-approval', Validators.required],
          threshold: [2, [Validators.required, Validators.min(1), Validators.max(10)]]
        })
      ])
    });
  }

  private initializeSearchForm(): FormGroup {
    return this.fb.group({
      searchType: ['id', Validators.required],
      searchValue: ['', Validators.required]
    });
  }

  private createExampleFormGroup(): FormGroup {
    return this.fb.group({
      testName: ['', Validators.required],
      input: ['', Validators.required],
      expectedOutput: ['', Validators.required],
      actualOutput: [''],
      tags: this.fb.array([]),
      complexity: ['medium', Validators.required]
    });
  }

  // Form Array Getters
  get examples(): FormArray {
    return this.establishForm.get('examples') as FormArray;
  }

  get validationCriteria(): FormArray {
    return this.establishForm.get('validationCriteria') as FormArray;
  }

  get prerequisites(): FormArray {
    return this.trainingForm.get('prerequisites') as FormArray;
  }

  get learningObjectives(): FormArray {
    return this.trainingForm.get('learningObjectives') as FormArray;
  }

  get triggerEventsArray(): FormArray {
    return this.pipelineForm.get('triggerEvents') as FormArray;
  }

  get qualityGates(): FormArray {
    return this.pipelineForm.get('qualityGates') as FormArray;
  }

  // Form Array Methods
  addExample(): void {
    this.examples.push(this.createExampleFormGroup());
  }

  removeExample(index: number): void {
    if (this.examples.length > 1) {
      this.examples.removeAt(index);
    }
  }

  addValidationCriteria(criteria: string = ''): void {
    this.validationCriteria.push(this.fb.control(criteria, Validators.required));
  }

  removeValidationCriteria(index: number): void {
    if (this.validationCriteria.length > 1) {
      this.validationCriteria.removeAt(index);
    }
  }

  addPrerequisite(prereq: string = ''): void {
    this.prerequisites.push(this.fb.control(prereq, Validators.required));
  }

  removePrerequisite(index: number): void {
    if (this.prerequisites.length > 1) {
      this.prerequisites.removeAt(index);
    }
  }

  addLearningObjective(objective: string = ''): void {
    this.learningObjectives.push(this.fb.control(objective, Validators.required));
  }

  removeLearningObjective(index: number): void {
    if (this.learningObjectives.length > 1) {
      this.learningObjectives.removeAt(index);
    }
  }

  addTriggerEvent(event: string = ''): void {
    this.triggerEventsArray.push(this.fb.control(event, Validators.required));
  }

  removeTriggerEvent(index: number): void {
    if (this.triggerEventsArray.length > 1) {
      this.triggerEventsArray.removeAt(index);
    }
  }

  addQualityGate(): void {
    this.qualityGates.push(
      this.fb.group({
        metric: ['', Validators.required],
        threshold: [80, [Validators.required, Validators.min(0), Validators.max(100)]]
      })
    );
  }

  removeQualityGate(index: number): void {
    if (this.qualityGates.length > 1) {
      this.qualityGates.removeAt(index);
    }
  }

  // Tab Management
  setActiveTab(tab: TabType): void {
    this.state.activeTab = tab;
    this.clearMessages();
  }

  // Clear messages
  clearMessages(): void {
    this.state.error = null;
    this.state.successMessage = '';
  }

  // ==================== API METHODS ====================

  establishPattern(): void {
    if (this.establishForm.invalid) {
      this.markFormGroupTouched(this.establishForm);
      const defaultError: PatternError = {
        patternArea: this.establishForm.get('area')?.value || 'unknown',
        failureType: 'validation',
        symptoms: ['Form validation failed'],
        rootCause: 'Please fill in all required fields correctly',
        mitigationSteps: ['Check form fields', 'Review validation errors'],
        temporaryWorkaround: 'Complete all required fields',
        errorTime: new Date().toISOString(),
        correlationId: 'client-validation'
      };
      this.state.error = defaultError;
      return;
    }

    this.state.isLoading = true;
    this.clearMessages();

    const formValue = this.establishForm.value;
    
    const examples: TestExample[] = (formValue.examples || []).map((ex: any) => ({
      testName: ex.testName || '',
      input: ex.input || '',
      expectedOutput: ex.expectedOutput || '',
      actualOutput: ex.actualOutput || '',
      tags: ex.tags || [],
      complexity: ex.complexity || 'medium'
    }));

    this.subscriptions.add(
      this.patternService.establishTestingPattern(formValue.area, examples).subscribe({
        next: (pattern) => {
          this.createdPattern = pattern;
          this.state.isLoading = false;
          this.state.successMessage = `Pattern "${pattern.name}" created successfully!`;
          
          if (this.trainingForm) {
            this.trainingForm.patchValue({ patternId: pattern.id });
          }
          if (this.pipelineForm) {
            this.pipelineForm.patchValue({ patternId: pattern.id });
          }
          
          this.getPatternsByArea(formValue.area);
        },
        error: (error) => {
          this.state.isLoading = false;
          this.state.error = error;
        }
      })
    );
  }

  generateTrainingMaterials(): void {
    if (!this.createdPattern) {
      const defaultError: PatternError = {
        patternArea: 'unknown',
        failureType: 'validation',
        symptoms: ['No pattern selected'],
        rootCause: 'Please create a pattern first',
        mitigationSteps: ['Create a pattern', 'Select an existing pattern'],
        temporaryWorkaround: 'Use an existing pattern',
        errorTime: new Date().toISOString(),
        correlationId: 'client-validation'
      };
      this.state.error = defaultError;
      return;
    }

    if (this.trainingForm.invalid) {
      this.markFormGroupTouched(this.trainingForm);
      return;
    }

    this.state.isLoading = true;
    this.clearMessages();

    const formValue = this.trainingForm.value;

    this.subscriptions.add(
      this.patternService.generateTrainingMaterials(this.createdPattern, formValue.audience).subscribe({
        next: (materials) => {
          this.trainingMaterials = materials;
          this.state.isLoading = false;
          this.state.successMessage = `Training materials generated for ${formValue.audience}!`;
        },
        error: (error) => {
          this.state.isLoading = false;
          this.state.error = error;
        }
      })
    );
  }

  createPipeline(): void {
    if (!this.createdPattern) {
      const defaultError: PatternError = {
        patternArea: 'unknown',
        failureType: 'validation',
        symptoms: ['No pattern selected'],
        rootCause: 'Please create a pattern first',
        mitigationSteps: ['Create a pattern', 'Select an existing pattern'],
        temporaryWorkaround: 'Use an existing pattern',
        errorTime: new Date().toISOString(),
        correlationId: 'client-validation'
      };
      this.state.error = defaultError;
      return;
    }

    if (this.pipelineForm.invalid) {
      this.markFormGroupTouched(this.pipelineForm);
      return;
    }

    this.state.isLoading = true;
    this.clearMessages();

    this.subscriptions.add(
      this.patternService.createAutomationPipeline(this.createdPattern).subscribe({
        next: (pipeline) => {
          this.pipelineBlueprint = pipeline;
          this.state.isLoading = false;
          this.state.successMessage = `Pipeline "${pipeline.name}" created successfully!`;
        },
        error: (error) => {
          this.state.isLoading = false;
          this.state.error = error;
        }
      })
    );
  }

  getPatternById(): void {
    const searchValue = this.patternSearchForm.get('searchValue')?.value;
    
    if (!searchValue) {
      return;
    }

    this.state.isLoading = true;
    this.clearMessages();

    this.subscriptions.add(
      this.patternService.getPatternById(searchValue).subscribe({
        next: (pattern) => {
          this.createdPattern = pattern;
          this.patterns = [pattern];
          this.state.isLoading = false;
          this.state.successMessage = `Pattern found: ${pattern.name}`;
          
          if (this.trainingForm) {
            this.trainingForm.patchValue({ patternId: pattern.id });
          }
          if (this.pipelineForm) {
            this.pipelineForm.patchValue({ patternId: pattern.id });
          }
        },
        error: (error) => {
          this.state.isLoading = false;
          this.state.error = error;
        }
      })
    );
  }

  getPatternsByArea(area?: string): void {
    const searchArea = area || this.patternSearchForm.get('searchValue')?.value;
    
    if (!searchArea) {
      return;
    }

    this.state.isLoading = true;
    this.clearMessages();

    this.subscriptions.add(
      this.patternService.getPatternsByArea(searchArea).subscribe({
        next: (patterns) => {
          this.patterns = patterns;
          this.state.isLoading = false;
          this.state.successMessage = `Found ${patterns.length} pattern(s)`;
        },
        error: (error) => {
          this.state.isLoading = false;
          this.state.error = error;
        }
      })
    );
  }

  checkHealth(): void {
    this.subscriptions.add(
      this.patternService.checkHealth().subscribe({
        next: (health) => {
          this.healthStatus = health;
          console.log('Service health:', health);
        },
        error: (error) => {
          console.warn('Health check failed:', error);
          this.healthStatus = {
            status: 'degraded',
            timestamp: new Date().toISOString(),
            service: 'PatternEstablishmentAPI',
            version: 'unknown'
          };
        }
      })
    );
  }

  onSearch(): void {
    const searchType = this.patternSearchForm.get('searchType')?.value;
    
    if (searchType === 'id') {
      this.getPatternById();
    } else {
      this.getPatternsByArea();
    }
  }

  selectPattern(pattern: TestingPattern): void {
    this.createdPattern = pattern;
    if (this.trainingForm) {
      this.trainingForm.patchValue({ patternId: pattern.id });
    }
    if (this.pipelineForm) {
      this.pipelineForm.patchValue({ patternId: pattern.id });
    }
    this.state.successMessage = `Selected pattern: ${pattern.name}`;
  }

  resetEstablishForm(): void {
    this.establishForm.reset({
      desiredConsistency: 'high',
      automationLevel: 'semi-automated'
    });
    this.createdPattern = null;
  }

  resetTrainingForm(): void {
    this.trainingForm.reset({
      audience: 'developers',
      format: 'workshop-ready',
      durationMinutes: 60,
      includeHandsOn: true
    });
    this.trainingMaterials = null;
  }

  resetPipelineForm(): void {
    this.pipelineForm.reset({
      template: 'ai-enhanced'
    });
    this.pipelineBlueprint = null;
  }

  private markFormGroupTouched(formGroup: FormGroup | FormArray): void {
    Object.values(formGroup.controls).forEach(control => {
      if (control instanceof FormGroup || control instanceof FormArray) {
        this.markFormGroupTouched(control);
      } else {
        control.markAsTouched();
      }
    });
  }

// Test endpoint methods
testEstablishEndpoint(event: Event): void {
  event.preventDefault();
  this.setActiveTab('establish');
  // Pre-fill with sample data for quick testing
  const sampleExamples = this.patternService.createSampleTestExamples(2);
  this.establishForm.patchValue({
    area: 'API Testing',
    examples: sampleExamples
  });
}

testTrainingEndpoint(event: Event): void {
  event.preventDefault();
  if (!this.createdPattern) {
    alert('Please create a pattern first or use the View Patterns tab to select one');
    this.setActiveTab('patterns');
  } else {
    this.setActiveTab('training');
  }
}


// Update these methods to accept the input values
testGetPatternByIdEndpoint(event: Event, patternId: string): void {
  event.preventDefault();
  if (patternId) {
    this.patternSearchForm.patchValue({
      searchType: 'id',
      searchValue: patternId
    });
    this.getPatternById();
    this.setActiveTab('patterns');
  } else {
    alert('Please enter a pattern ID');
  }
}

testGetPatternsByAreaEndpoint(event: Event, area: string): void {
  event.preventDefault();
  if (area) {
    this.patternSearchForm.patchValue({
      searchType: 'area',
      searchValue: area
    });
    this.getPatternsByArea();
    this.setActiveTab('patterns');
  } else {
    alert('Please enter an area');
  }
}


/**
 * Test the pipeline creation endpoint
 * Checks if a pattern exists and navigates to the pipeline tab
 */
testPipelineEndpoint(event: Event): void {
  event.preventDefault();
  
  if (!this.createdPattern) {
    // Try to find any pattern in the patterns list
    if (this.patterns && this.patterns.length > 0) {
      // Use the first available pattern
      this.createdPattern = this.patterns[0];
      this.pipelineForm.patchValue({ patternId: this.createdPattern.id });
      this.setActiveTab('pipeline');
      this.state.successMessage = `Using pattern: ${this.createdPattern.name}`;
    } else {
      // No pattern available, prompt user to create or search for one
      const userChoice = confirm(
        'No pattern found. Would you like to:\n' +
        '• Click OK to go to the Establish tab and create a new pattern\n' +
        '• Click Cancel to go to the View Patterns tab and search for an existing pattern'
      );
      
      if (userChoice) {
        // Go to establish tab
        this.setActiveTab('establish');
        // Pre-fill with sample data for quick testing
        const sampleExamples = this.patternService.createSampleTestExamples(2);
        this.establishForm.patchValue({
          area: 'API Testing',
          examples: sampleExamples
        });
      } else {
        // Go to patterns tab
        this.setActiveTab('patterns');
        // Pre-fill a common search
        this.patternSearchForm.patchValue({
          searchType: 'area',
          searchValue: 'API'
        });
      }
    }
  } else {
    // Pattern exists, go directly to pipeline tab
    this.setActiveTab('pipeline');
    this.state.successMessage = `Ready to create pipeline for pattern: ${this.createdPattern.name}`;
  }
}

/**
 * Alternative implementation that creates a default pipeline without requiring a pattern
 * Use this if you want to demonstrate the pipeline endpoint with mock data
 */
testPipelineEndpointWithMock(event: Event): void {
  event.preventDefault();
  
  // Create a mock pattern for demonstration
  const mockPattern: TestingPattern = {
    id: `mock_${Date.now()}`,
    name: 'Demo Pattern',
    area: 'API Testing',
    problemStatement: 'Testing APIs consistently is challenging',
    solution: 'Use this pattern to standardize API testing',
    implementation: {
      codeExamples: ['// Example test code'],
      configuration: { timeout: 30, retryCount: 3 },
      dosAndDonts: ['DO: Use async/await', 'DON\'T: Forget error handling']
    },
    qualityIndicators: {
      repeatabilityScore: 85,
      learningCurve: 'medium',
      maintenanceCost: 'low'
    },
    aiAssistance: {
      promptTemplates: ['Generate API test for {endpoint}'],
      validationRules: ['Check status codes', 'Validate response schema'],
      commonPitfalls: ['Not handling timeouts', 'Missing assertions']
    },
    adoptionMetrics: {
      estimatedTimeSave: '40-60%',
      errorReduction: '50-70%',
      teamSatisfaction: 8
    },
    createdAt: new Date().toISOString(),
    status: 'draft'
  };
  
  this.createdPattern = mockPattern;
  this.pipelineForm.patchValue({ patternId: mockPattern.id });
  this.setActiveTab('pipeline');
  this.state.successMessage = 'Demo pattern created for pipeline testing';
}



/**
 * Test the health check endpoint
 * Calls the health API and displays the result
 */
testHealthEndpoint(event: Event): void {
  event.preventDefault();
  
  // Set loading state
  this.state.isLoading = true;
  this.clearMessages();
  
  // Call the health check service
  this.subscriptions.add(
    this.patternService.checkHealth().subscribe({
      next: (health) => {
        this.healthStatus = health;
        this.state.isLoading = false;
        this.state.successMessage = `Health check successful: ${health.status}`;
        this.setActiveTab('health');
        
        // Log detailed health info to console
        console.log('Health Check Details:', {
          status: health.status,
          service: health.service,
          timestamp: health.timestamp,
          version: health.version
        });
      },
      error: (error) => {
        this.state.isLoading = false;
        this.state.error = {
          patternArea: 'system',
          failureType: 'adoption',
          symptoms: ['Health check failed'],
          rootCause: error.message || 'Unable to reach health endpoint',
          mitigationSteps: [
            'Check if the backend server is running',
            'Verify API URL configuration',
            'Check network connectivity'
          ],
          temporaryWorkaround: 'Refresh the page or try again later',
          errorTime: new Date().toISOString(),
          correlationId: error.correlationId || 'unknown'
        };
        
        // Set degraded health status
        this.healthStatus = {
          status: 'unhealthy',
          timestamp: new Date().toISOString(),
          service: 'PatternEstablishmentAPI',
          version: 'unknown'
        };
        
        this.setActiveTab('health');
      }
    })
  );
}

/**
 * Advanced health check with dependency status
 * Shows more detailed health information
 */
testHealthEndpointWithDetails(event: Event): void {
  event.preventDefault();
  
  this.state.isLoading = true;
  this.clearMessages();
  
  // Simulate a more detailed health check with dependencies
  setTimeout(() => {
    this.healthStatus = {
      status: 'healthy',
      timestamp: new Date().toISOString(),
      service: 'PatternEstablishmentAPI',
      version: '1.2.3'
    };
    
    // Add custom property for demo purposes
    const detailedStatus = {
      ...this.healthStatus,
      dependencies: [
        { name: 'Database', status: 'healthy', responseTime: '45ms' },
        { name: 'Redis Cache', status: 'healthy', responseTime: '12ms' },
        { name: 'Message Queue', status: 'degraded', responseTime: '230ms', message: 'High latency' },
        { name: 'AI Service', status: 'healthy', responseTime: '156ms' }
      ],
      uptime: '99.97%',
      memoryUsage: '42%',
      cpuLoad: '18%'
    };
    
    this.healthStatus = detailedStatus as any;
    this.state.isLoading = false;
    this.state.successMessage = 'Detailed health check completed';
    this.setActiveTab('health');
  }, 800);
}

/**
 * Continuous health monitoring
 * Checks health every 30 seconds
 */
startHealthMonitoring(): void {
  this.testHealthEndpoint(new Event('click'));
  
  // Set up interval for continuous monitoring
  const intervalId = setInterval(() => {
    this.subscriptions.add(
      this.patternService.checkHealth().subscribe({
        next: (health) => {
          this.healthStatus = health;
          console.log('Health monitoring:', health.status, new Date().toLocaleTimeString());
        },
        error: (error) => {
          console.warn('Health monitoring failed:', error);
        }
      })
    );
  }, 30000); // Check every 30 seconds
  
  // Store interval ID to clear on component destroy
  (this as any).healthInterval = intervalId;
}

// Add this to ngOnDestroy to clean up the interval
// In ngOnDestroy method, add:
// if ((this as any).healthInterval) {
//   clearInterval((this as any).healthInterval);
// }



  getFormErrors(form: FormGroup): { [key: string]: string } {
    const errors: { [key: string]: string } = {};
    
    Object.keys(form.controls).forEach(key => {
      const control = form.get(key);
      if (control?.errors) {
        if (control.errors['required']) {
          errors[key] = `${key} is required`;
        } else if (control.errors['minlength']) {
          errors[key] = `${key} must be at least ${control.errors['minlength'].requiredLength} characters`;
        } else if (control.errors['min']) {
          errors[key] = `${key} must be at least ${control.errors['min'].min}`;
        } else if (control.errors['max']) {
          errors[key] = `${key} cannot exceed ${control.errors['max'].max}`;
        }
      }
    });
    
    return errors;
  }
}