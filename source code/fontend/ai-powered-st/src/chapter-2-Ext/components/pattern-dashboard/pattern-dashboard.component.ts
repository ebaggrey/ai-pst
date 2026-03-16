import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, 
         FormGroup, FormArray, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { PatternEstablishmentService } 
         from 'src/chapter-2-Ext/Services/pattern-establishment.service';
import { PatternError, PatternEstablishmentRequest,
         PatternValidationResult, PipelineBlueprint, 
         PipelineRequest, TestExample, TestingPattern,
         TrainingGenerationRequest,
         TrainingMaterials } 
         from 'src/chapter-2-Ext/Models/pattern-models';


@Component({
  selector: 'app-pattern-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  templateUrl: './pattern-dashboard.component.html',
  styleUrls: ['./pattern-dashboard.component.css']
})
export class PatternDashboardComponent implements OnInit, OnDestroy {
  // Form groups
  patternForm!: FormGroup;
  exampleForm!: FormGroup;
  trainingForm!: FormGroup;
  pipelineForm!: FormGroup;

  // State management
  activeTab: 'establish' | 'training' | 'pipeline' | 'validate' = 'establish';
  isLoading = false;
  error: string | null = null;
  success: string | null = null;

  // Data containers with proper types
  establishedPattern: TestingPattern | null = null;
  trainingMaterials: TrainingMaterials | null = null;
  pipelineBlueprint: PipelineBlueprint | null = null;
  validationResult: PatternValidationResult | null = null;
  patternsByArea: TestingPattern[] = [];
  recentErrors: PatternError[] = [];

  // Examples array
  examples: TestExample[] = [];

  // Selected items
  selectedPattern: TestingPattern | null = null;

  // Subscriptions
  private subscriptions: Subscription[] = [];

  // Dropdown options
  automationLevels = ['manual', 'semi-automated', 'fully-automated'];
  consistencyLevels = ['low', 'medium', 'high'];
  experienceLevels = ['beginner', 'intermediate', 'advanced', 'expert'];
  audiences = ['developers', 'qa-engineers', 'team-leads', 'all-team-members'];
  triggerEvents = ['pr-created', 'schedule-daily', 'manual', 'push-to-main', 'tag-created'];
  taskTypes = [
    'validate-syntax', 'check-coverage', 'run-smoke-tests',
    'generate-variants', 'optimize-selectors', 'add-assertions',
    'code-review', 'approval-workflow', 'merge-to-main'
  ];

  constructor(
    private fb: FormBuilder,
    private patternService: PatternEstablishmentService
  ) {}

  ngOnInit(): void {
    this.initializeForms();
    this.loadInitialData();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private initializeForms(): void {
    // Pattern Establishment Form
    this.patternForm = this.fb.group({
      area: ['', [Validators.required, Validators.minLength(3)]],
      desiredConsistency: ['high', Validators.required],
      automationLevel: ['semi-automated', Validators.required],
      validationCriteria: this.fb.array([
        this.fb.control('Pattern should be repeatable'),
        this.fb.control('Easy for other team members to follow'),
        this.fb.control('Produces consistent results'),
        this.fb.control('Documented with examples')
      ]),
      metadata: this.fb.group({
        teamSize: [5, [Validators.required, Validators.min(1)]],
        experienceLevel: ['intermediate'],
        timeline: ['2 weeks'],
        tools: this.fb.array([]),
        constraints: this.fb.group({})
      })
    });

    // Example Form (for adding test examples)
    this.exampleForm = this.fb.group({
      name: ['', Validators.required],
      code: ['', Validators.required],
      description: [''],
      tags: [''],
      context: ['']
    });

    // Training Generation Form
    this.trainingForm = this.fb.group({
      patternId: ['', Validators.required],
      audience: ['team-members', Validators.required],
      format: ['workshop-ready'],
      durationMinutes: [60, [Validators.required, Validators.min(30)]],
      includeHandsOn: [true],
      prerequisites: this.fb.array([]),
      learningObjectives: this.fb.array([
        this.fb.control('Understand when to use the pattern'),
        this.fb.control('Implement the pattern correctly'),
        this.fb.control('Troubleshoot common issues'),
        this.fb.control('Extend the pattern for edge cases')
      ])
    });

    // Pipeline Creation Form
    this.pipelineForm = this.fb.group({
      patternId: ['', Validators.required],
      triggerEvents: this.fb.array([this.fb.control('pr-created')]),
      stages: this.fb.array([this.createStage()]),
      qualityGates: this.fb.array([
        this.createQualityGate('test-pass-rate', 95, '>=', 'fail'),
        this.createQualityGate('generation-confidence', 80, '>=', 'fail'),
        this.createQualityGate('reviewer-approval', 2, '>=', 'fail')
      ])
    });
  }

  private loadInitialData(): void {
    this.getPatternsByArea('testing');
  }

  // ==================== HTTP Request Methods ====================

  establishPattern(): void {
    if (this.patternForm.invalid) {
      this.showError('Please fill all required fields correctly');
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const request: PatternEstablishmentRequest = {
      area: this.patternForm.get('area')?.value,
      examples: this.examples,
      desiredConsistency: this.patternForm.get('desiredConsistency')?.value,
      automationLevel: this.patternForm.get('automationLevel')?.value,
      validationCriteria: this.validationCriteria.value,
      metadata: this.patternForm.get('metadata')?.value
    };

    const sub = this.patternService.establishTestingPattern(request).subscribe({
      next: (pattern) => {
        this.establishedPattern = pattern;
        this.selectedPattern = pattern;
        this.isLoading = false;
        this.showSuccess(`Pattern "${pattern.name}" established successfully!`);
        this.getPatternsByArea('testing'); // Refresh the list
      },
      error: (err) => {
        this.handleError(err, 'establishing pattern');
      }
    });

    this.subscriptions.push(sub);
  }

  validatePattern(): void {
    if (!this.selectedPattern) {
      this.showError('Please select a pattern to validate');
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const sub = this.patternService.validatePattern(this.selectedPattern).subscribe({
      next: (result) => {
        this.validationResult = result;
        this.isLoading = false;
        this.showSuccess(`Pattern validation ${result.isValid ? 'passed' : 'failed'}`);
        console.log('Validation result:', result);
      },
      error: (err) => {
        this.handleError(err, 'validating pattern');
      }
    });

    this.subscriptions.push(sub);
  }

  generateTrainingMaterials(): void {
    if (this.trainingForm.invalid || !this.selectedPattern) {
      this.showError('Please fill all required fields and select a pattern');
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const request: TrainingGenerationRequest = {
      pattern: this.selectedPattern,
      audience: this.trainingForm.get('audience')?.value,
      format: this.trainingForm.get('format')?.value,
      durationMinutes: this.trainingForm.get('durationMinutes')?.value,
      includeHandsOn: this.trainingForm.get('includeHandsOn')?.value || false,
      prerequisites: this.prerequisites.value,
      learningObjectives: this.learningObjectives.value
    };

    const sub = this.patternService.generateTeamTrainingMaterials(request).subscribe({
      next: (materials) => {
        this.trainingMaterials = materials;
        this.isLoading = false;
        this.showSuccess('Training materials generated successfully!');
      },
      error: (err) => {
        this.handleError(err, 'generating training materials');
      }
    });

    this.subscriptions.push(sub);
  }

  createAutomationPipeline(): void {
    if (this.pipelineForm.invalid) {
      this.showError('Please fill all required fields correctly');
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const request: PipelineRequest = {
      patternId: this.pipelineForm.get('patternId')?.value,
      triggerEvents: this.triggerEventsArray.value,
      stages: this.stages.value,
      qualityGates: this.qualityGates.value
    };

    const sub = this.patternService.createAutomationPipeline(request).subscribe({
      next: (pipeline) => {
        this.pipelineBlueprint = pipeline;
        this.isLoading = false;
        this.showSuccess(`Pipeline "${pipeline.pipelineName}" created successfully!`);
      },
      error: (err) => {
        this.handleError(err, 'creating pipeline');
      }
    });

    this.subscriptions.push(sub);
  }

  updatePattern(): void {
    if (!this.selectedPattern) {
      this.showError('Please select a pattern to update');
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const sub = this.patternService.updatePattern(
      this.selectedPattern.id,
      this.selectedPattern
    ).subscribe({
      next: (updated) => {
        this.selectedPattern = updated;
        this.establishedPattern = updated;
        this.isLoading = false;
        this.showSuccess('Pattern updated successfully!');
      },
      error: (err) => {
        this.handleError(err, 'updating pattern');
      }
    });

    this.subscriptions.push(sub);
  }

  deletePattern(patternId: string): void {
    if (!confirm('Are you sure you want to delete this pattern?')) {
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const sub = this.patternService.deletePattern(patternId).subscribe({
      next: () => {
        if (this.selectedPattern?.id === patternId) {
          this.selectedPattern = null;
          this.establishedPattern = null;
        }
        this.getPatternsByArea('testing'); // Refresh list
        this.isLoading = false;
        this.showSuccess('Pattern deleted successfully!');
      },
      error: (err) => {
        this.handleError(err, 'deleting pattern');
      }
    });

    this.subscriptions.push(sub);
  }

  getPatternsByArea(area: string): void {
    this.isLoading = true;

    const sub = this.patternService.getPatternsByArea(area).subscribe({
      next: (patterns) => {
        this.patternsByArea = patterns;
        this.isLoading = false;
      },
      error: (err) => {
        this.handleError(err, 'fetching patterns');
      }
    });

    this.subscriptions.push(sub);
  }

  batchValidatePatterns(): void {
    if (this.patternsByArea.length === 0) {
      this.showError('No patterns to validate');
      return;
    }

    this.isLoading = true;

    const sub = this.patternService.batchValidatePatterns(this.patternsByArea).subscribe({
      next: (results) => {
        console.log('Batch validation results:', results);
        const passedCount = results.filter(r => r.isValid).length;
        this.showSuccess(`Validated ${results.length} patterns. ${passedCount} passed.`);
        this.isLoading = false;
      },
      error: (err) => {
        this.handleError(err, 'batch validation');
      }
    });

    this.subscriptions.push(sub);
  }

  checkHealth(): void {
    const sub = this.patternService.checkHealth().subscribe({
      next: (health) => {
        console.log('Service health:', health);
        this.showSuccess('Service is healthy');
      },
      error: (err) => {
        this.handleError(err, 'health check');
      }
    });

    this.subscriptions.push(sub);
  }

  // ==================== Example Management ====================

  addExample(): void {
    if (this.exampleForm.invalid) {
      this.showError('Please fill all required fields for the example');
      return;
    }

    const metrics = this.patternService.generateExampleMetrics(
      this.exampleForm.get('code')?.value
    );

    const example: TestExample = {
      id: this.patternService.generateGuid(),
      name: this.exampleForm.get('name')?.value,
      code: this.exampleForm.get('code')?.value,
      description: this.exampleForm.get('description')?.value || '',
      tags: this.exampleForm.get('tags')?.value ? 
            this.exampleForm.get('tags')?.value.split(',').map((t: string) => t.trim()) : [],
      context: this.exampleForm.get('context')?.value || '',
      metrics: metrics
    };

    this.examples.push(example);
    this.exampleForm.reset();
    this.showSuccess('Example added successfully');
  }

  removeExample(index: number): void {
    this.examples.splice(index, 1);
  }

  // ==================== Form Array Getters ====================

  get validationCriteria() {
    return this.patternForm.get('validationCriteria') as FormArray;
  }

  get prerequisites() {
    return this.trainingForm.get('prerequisites') as FormArray;
  }

  get learningObjectives() {
    return this.trainingForm.get('learningObjectives') as FormArray;
  }

  get triggerEventsArray() {
    return this.pipelineForm.get('triggerEvents') as FormArray;
  }

  get stages() {
    return this.pipelineForm.get('stages') as FormArray;
  }

  get qualityGates() {
    return this.pipelineForm.get('qualityGates') as FormArray;
  }

  // ==================== Form Array Helpers ====================

  addValidationCriterion(criterion: string = ''): void {
    this.validationCriteria.push(this.fb.control(criterion));
  }

  removeValidationCriterion(index: number): void {
    this.validationCriteria.removeAt(index);
  }

  addPrerequisite(prereq: string = ''): void {
    this.prerequisites.push(this.fb.control(prereq));
  }

  removePrerequisite(index: number): void {
    this.prerequisites.removeAt(index);
  }

  addLearningObjective(objective: string = ''): void {
    this.learningObjectives.push(this.fb.control(objective));
  }

  removeLearningObjective(index: number): void {
    this.learningObjectives.removeAt(index);
  }

  addTriggerEvent(event: string = 'pr-created'): void {
    this.triggerEventsArray.push(this.fb.control(event));
  }

  removeTriggerEvent(index: number): void {
    this.triggerEventsArray.removeAt(index);
  }

  createStage(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      tasks: this.fb.array([this.fb.control('validate-syntax')]),
      dependencies: this.fb.array([]),
      timeoutMinutes: [30],
      requiredTools: this.fb.array([])
    });
  }

  addStage(): void {
    this.stages.push(this.createStage());
  }

  removeStage(index: number): void {
    this.stages.removeAt(index);
  }

  getStageTasks(stageIndex: number): FormArray {
    return this.stages.at(stageIndex).get('tasks') as FormArray;
  }

  addStageTask(stageIndex: number, task: string = ''): void {
    this.getStageTasks(stageIndex).push(this.fb.control(task || 'validate-syntax'));
  }

  removeStageTask(stageIndex: number, taskIndex: number): void {
    this.getStageTasks(stageIndex).removeAt(taskIndex);
  }

  createQualityGate(metric: string = '', threshold: number = 80, operator: string = '>=', action: string = 'fail'): FormGroup {
    return this.fb.group({
      metric: [metric, Validators.required],
      threshold: [threshold, [Validators.required, Validators.min(0), Validators.max(100)]],
      operator: [operator],
      action: [action]
    });
  }

  addQualityGate(): void {
    this.qualityGates.push(this.createQualityGate());
  }

  removeQualityGate(index: number): void {
    this.qualityGates.removeAt(index);
  }

  // ==================== Selection Methods ====================

  selectPattern(pattern: TestingPattern): void {
    this.selectedPattern = pattern;
    this.trainingForm.patchValue({ patternId: pattern.id });
    this.pipelineForm.patchValue({ patternId: pattern.id });
    this.showSuccess(`Selected pattern: ${pattern.name}`);
  }

  clearSelectedPattern(): void {
    this.selectedPattern = null;
    this.validationResult = null;
  }

  // ==================== Utility Methods ====================

  private handleError(err: any, operation: string): void {
    this.isLoading = false;
    const errorMessage = err.message || `Failed ${operation}`;
    this.showError(errorMessage);
    
    // Log error to service
    const patternError: PatternError = {
      errorId: this.patternService.generateGuid(),
      patternArea: this.patternForm?.get('area')?.value || 'unknown',
      failureType: 'generation',
      symptoms: [errorMessage],
      rootCause: err.details?.message || 'Unknown error',
      mitigationSteps: ['Check form inputs', 'Verify pattern requirements', 'Try again with different parameters'],
      temporaryWorkaround: 'Try with simpler examples or different area',
      occurredAt: new Date().toISOString()
    };
    
    this.patternService.logPatternError(patternError).subscribe({
      error: (logError) => console.error('Failed to log error:', logError)
    });
  }

  private showError(message: string): void {
    this.error = message;
    setTimeout(() => this.error = null, 5000);
  }

  private showSuccess(message: string): void {
    this.success = message;
    setTimeout(() => this.success = null, 5000);
  }

  private clearMessages(): void {
    this.error = null;
    this.success = null;
  }
}