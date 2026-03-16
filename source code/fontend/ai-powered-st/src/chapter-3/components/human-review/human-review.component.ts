import { Component, OnInit, OnDestroy, signal, computed, effect } from '@angular/core';
import { CommonModule, JsonPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule,
         FormBuilder, FormGroup, 
         Validators, FormArray } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Subscription } from 'rxjs';

// Import the service
import { HumanReviewService } from '../../Services/human-review.service';
import { ClarificationRequest, ClarificationResponse,
         CollaborationError, CollaborationResponse, 
         CollaborativeEditRequest, HumanReviewError,
         JudgmentRequest, JudgmentResponse,
         ReviewRequest, ReviewSession,
         ReviewSessionResponse } from 'src/chapter-3/Models/human-review.models';


@Component({
  selector: 'app-human-review',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  templateUrl: './human-review.component.html',
  styleUrls: ['./human-review.component.css']
})
export class HumanReviewComponent implements OnInit, OnDestroy {
  // Forms
  submitForm: FormGroup;
  editForm: FormGroup;
  clarificationForm: FormGroup;
  judgmentForm: FormGroup;

  // State signals
  activeTab = signal<'submit' | 'session' | 'collaborate' | 'clarify' | 'judge'>('submit');
  currentSessionId = signal<string | null>(null);
  currentSession = signal<ReviewSession | null>(null);
  isLoading = signal<boolean>(false);
  error = signal<HumanReviewError | null>(null);
  success = signal<string | null>(null);

  // Response data signals
  submitResponse = signal<ReviewSessionResponse | null>(null);
  sessionResponse = signal<ReviewSession | null>(null);
  collaborateResponse = signal<CollaborationResponse | null>(null);
  clarifyResponse = signal<ClarificationResponse | null>(null);
  judgeResponse = signal<JudgmentResponse | null>(null);
  conflictError = signal<CollaborationError | null>(null);

  // Computed values
  isSessionActive = computed(() => !!this.currentSession() && this.currentSession()?.status !== 'closed');
  sessionProgress = computed(() => this.getSessionProgress(this.currentSession()));
  estimatedTimeFormatted = computed(() => {
    const time = this.submitResponse()?.estimatedReviewTime;
    return time ? this.formatTime(time) : 'N/A';
  });

  private subscriptions: Subscription[] = [];

  constructor(
    private fb: FormBuilder,
    private humanReviewService: HumanReviewService
  ) {
    // Initialize forms
    this.submitForm = this.createSubmitForm();
    this.editForm = this.createEditForm();
    this.clarificationForm = this.createClarificationForm();
    this.judgmentForm = this.createJudgmentForm();

    // React to session changes
    effect(() => {
      const sessionId = this.currentSessionId();
      if (sessionId) {
        this.loadSession(sessionId);
      }
    });
  }

  ngOnInit(): void {
    // Subscribe to service observables
    this.subscriptions.push(
      this.humanReviewService.sessionUpdates$.subscribe(session => {
        this.currentSession.set(session);
        this.sessionResponse.set(session);
      })
    );

    this.subscriptions.push(
      this.humanReviewService.editConflicts$.subscribe(conflict => {
        this.conflictError.set(conflict);
        this.error.set({
          errorCode: 'EDIT_CONFLICT',
          message: 'Edit conflict detected',
          suggestion: conflict.aiMergeSuggestion,
          recoveryActions: conflict.resolutionOptions,
          reportedAt: new Date()
        });
      })
    );

    this.subscriptions.push(
      this.humanReviewService.sessionClosed$.subscribe(event => {
        this.success.set(`Session ${event.sessionId} closed successfully`);
        this.currentSessionId.set(null);
        this.currentSession.set(null);
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  //#region Form Creation

  private createSubmitForm(): FormGroup {
    return this.fb.group({
      // Generated Test
      testContent: ['', [Validators.required, Validators.minLength(10)]],
      confidenceScore: [0.85, [Validators.required, Validators.min(0), Validators.max(1)]],
      testType: ['unit', Validators.required],
      framework: ['xUnit', Validators.required],
      language: ['C#'],
      
      // Review Context
      testPurpose: ['', [Validators.required, Validators.minLength(10)]],
      riskLevel: ['medium', Validators.required],
      technicalDomains: [''],
      
      // Reviewer Guidance
      reviewerGuidance: ['', [Validators.required, Validators.minLength(20)]],
      
      // Submission Metadata
      submittedBy: ['current-user', Validators.required],
      
      // Options
      priority: ['medium'],
      enableRealTimeCollaboration: [true],
      autoSuggestImprovements: [true]
    });
  }

  private createEditForm(): FormGroup {
    return this.fb.group({
      editContent: ['', [Validators.required, Validators.minLength(1)]],
      editIntent: ['', [Validators.required, Validators.minLength(10)]],
      editType: ['modification'],
      editorId: ['current-user', Validators.required],
      affectedLines: [''],
      requestAiAnalysis: [true]
    });
  }

  private createClarificationForm(): FormGroup {
    return this.fb.group({
      humanQuestion: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(1000)]],
      questionType: ['general'],
      urgency: ['normal'],
      contextTags: ['']
    });
  }

  private createJudgmentForm(): FormGroup {
    return this.fb.group({
      decision: ['approve', Validators.required],
      reasoning: ['', [Validators.required, Validators.minLength(10)]],
      suggestedImprovements: this.fb.array([]),
      areasOfConcern: this.fb.array([]),
      confidenceInJudgment: [1.0, [Validators.min(0), Validators.max(1)]],
      storeForTraining: [true]
    });
  }

  // Helper for form arrays
  get suggestedImprovements(): FormArray {
    return this.judgmentForm.get('suggestedImprovements') as FormArray;
  }

  get areasOfConcern(): FormArray {
    return this.judgmentForm.get('areasOfConcern') as FormArray;
  }

  addSuggestion(value: string = ''): void {
    this.suggestedImprovements.push(this.fb.control(value, Validators.required));
  }

  removeSuggestion(index: number): void {
    this.suggestedImprovements.removeAt(index);
  }

  addConcern(value: string = ''): void {
    this.areasOfConcern.push(this.fb.control(value, Validators.required));
  }

  removeConcern(index: number): void {
    this.areasOfConcern.removeAt(index);
  }

  //#endregion

  //#region API Endpoint Examples

  /**
   * Endpoint: POST /api/human-review/submit
   * Example HTTP Request: Submit AI-generated test for human review
   */
  onSubmitForReview(): void {
    if (this.submitForm.invalid) {
      this.markFormGroupTouched(this.submitForm);
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);
    this.success.set(null);

    const formValue = this.submitForm.value;
    
    // Parse technical domains
    const technicalDomains = formValue.technicalDomains
      ? formValue.technicalDomains.split(',').map((s: string) => s.trim()).filter((s: string) => s)
      : [];

    // Create the request object with type assertion to handle any mismatches
    const request: ReviewRequest = {
      generatedContent: {
        content: formValue.testContent,
        confidenceScore: formValue.confidenceScore,
        generatedAt: new Date(),
        testType: formValue.testType,
        framework: formValue.framework,
        language: formValue.language,
        tags: ['generated', 'ai', formValue.testType],
        metadata: {
          source: 'human-review-component'
        }
      },
      context: {
        testPurpose: formValue.testPurpose,
        riskLevel: formValue.riskLevel,
        technicalDomains: technicalDomains.length ? technicalDomains : undefined,
        priority: formValue.priority
      },
      reviewerGuidance: formValue.reviewerGuidance,
      submissionMetadata: {
        submittedBy: formValue.submittedBy,
        submittedAt: new Date(),
        requestId: `req_${Date.now()}`,
        submittedFrom: window.location.href
      },
      preferredReviewers: [],
      priority: formValue.priority,
      enableRealTimeCollaboration: formValue.enableRealTimeCollaboration,
      autoSuggestImprovements: formValue.autoSuggestImprovements,
      additionalSettings: {
        source: 'web-ui',
        timestamp: Date.now()
      }
    };

    this.humanReviewService.submitForHumanReview(request).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        this.submitResponse.set(response);
        this.currentSessionId.set(response.sessionId);
        this.success.set(`Review session created successfully: ${response.sessionId}`);
        this.activeTab.set('session');
      },
      error: (error: HumanReviewError) => {
        this.isLoading.set(false);
        this.error.set(error);
      }
    });
  }

  /**
   * Endpoint: GET /api/human-review/{sessionId}
   * Example HTTP Request: Get review session details
   */
  loadSession(sessionId: string): void {
    if (!sessionId) return;

    this.isLoading.set(true);
    this.error.set(null);

    this.humanReviewService.getReviewSession(sessionId).subscribe({
      next: (session) => {
        this.isLoading.set(false);
        // Type assertion to handle any type mismatches
        const typedSession = session as ReviewSession;
        this.sessionResponse.set(typedSession);
        this.currentSession.set(typedSession);
      },
      error: (error: HumanReviewError) => {
        this.isLoading.set(false);
        this.error.set(error);
      }
    });
  }

  /**
   * Endpoint: POST /api/human-review/{sessionId}/collaborate
   * Example HTTP Request: Collaborate on test with real-time editing
   */
  onCollaborate(): void {
    const sessionId = this.currentSessionId();
    if (!sessionId) {
      this.error.set({
        errorCode: 'NO_SESSION',
        message: 'No active session',
        suggestion: 'Create a review session first',
        reportedAt: new Date()
      });
      return;
    }

    if (this.editForm.invalid) {
      this.markFormGroupTouched(this.editForm);
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);
    this.conflictError.set(null);

    const formValue = this.editForm.value;
    
    // Parse affected lines
    const affectedLines = formValue.affectedLines
      ? formValue.affectedLines.split(',').map((s: string) => s.trim()).filter((s: string) => s)
      : [];

    // Create the request object
    const request: CollaborativeEditRequest = {
      userEdit: {
        content: formValue.editContent,
        intent: formValue.editIntent,
        editorId: formValue.editorId,
        editType: formValue.editType,
        affectedLines: affectedLines,
        createdAt: new Date()
      },
      editContext: 'Human review collaboration',
      relatedIssues: [],
      requestAiAnalysis: formValue.requestAiAnalysis
    };

    this.humanReviewService.collaborateOnTest(sessionId, request).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        // Type assertion to handle any type mismatches
        const typedResponse = response as CollaborationResponse;
        this.collaborateResponse.set(typedResponse);
        this.currentSession.set(typedResponse.session);
        this.success.set('Edit applied successfully');
        
        // Clear edit form for next edit
        this.editForm.patchValue({
          editContent: '',
          editIntent: ''
        });
      },
      error: (error) => {
        this.isLoading.set(false);
        if (this.isCollaborationError(error)) {
          this.conflictError.set(error);
        } else {
          this.error.set(error);
        }
      }
    });
  }

  /**
   * Endpoint: POST /api/human-review/{sessionId}/clarify
   * Example HTTP Request: Request clarification from AI
   */
  onRequestClarification(): void {
    const sessionId = this.currentSessionId();
    if (!sessionId) {
      this.error.set({
        errorCode: 'NO_SESSION',
        message: 'No active session',
        suggestion: 'Create a review session first',
        reportedAt: new Date()
      });
      return;
    }

    if (this.clarificationForm.invalid) {
      this.markFormGroupTouched(this.clarificationForm);
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);

    const formValue = this.clarificationForm.value;
    
    // Parse context tags
    const contextTags = formValue.contextTags
      ? formValue.contextTags.split(',').map((s: string) => s.trim()).filter((s: string) => s)
      : [];

    // Create the request object
    const request: ClarificationRequest = {
      humanQuestion: formValue.humanQuestion,
      questionType: formValue.questionType,
      contextTags: contextTags,
      urgency: formValue.urgency,
      additionalContext: {
        source: 'human-review-ui',
        sessionId: sessionId
      }
    };

    this.humanReviewService.requestClarification(sessionId, request).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        // Type assertion to handle any type mismatches
        const typedResponse = response as ClarificationResponse;
        this.clarifyResponse.set(typedResponse);
        this.success.set('Clarification received');
        
        // Clear clarification form
        this.clarificationForm.patchValue({
          humanQuestion: ''
        });
      },
      error: (error: HumanReviewError) => {
        this.isLoading.set(false);
        this.error.set(error);
      }
    });
  }

  /**
   * Endpoint: POST /api/human-review/{sessionId}/judge
   * Example HTTP Request: Provide final human judgment
   */
  onProvideJudgment(): void {
    const sessionId = this.currentSessionId();
    if (!sessionId) {
      this.error.set({
        errorCode: 'NO_SESSION',
        message: 'No active session',
        suggestion: 'Create a review session first',
        reportedAt: new Date()
      });
      return;
    }

    if (this.judgmentForm.invalid) {
      this.markFormGroupTouched(this.judgmentForm);
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);

    const formValue = this.judgmentForm.value;

    // Create the request object
    const request: JudgmentRequest = {
      judgment: {
        decision: formValue.decision,
        reasoning: formValue.reasoning,
        suggestedImprovements: this.suggestedImprovements.value.filter((v: string) => v),
        areasOfConcern: this.areasOfConcern.value.filter((v: string) => v),
        confidenceInJudgment: formValue.confidenceInJudgment,
        specificFeedback: {
          source: 'human-review-ui',
          timestamp: Date.now().toString()
        }
      },
      areasReviewed: ['all'],
      feedbackForAi: formValue.reasoning,
      storeForTraining: formValue.storeForTraining
    };

    this.humanReviewService.provideHumanJudgment(sessionId, request).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        // Type assertion to handle any type mismatches
        const typedResponse = response as JudgmentResponse;
        this.judgeResponse.set(typedResponse);
        this.success.set('Judgment provided successfully');
        this.activeTab.set('submit');
        this.currentSessionId.set(null);
        this.currentSession.set(null);
      },
      error: (error: HumanReviewError) => {
        this.isLoading.set(false);
        this.error.set(error);
      }
    });
  }

  //#endregion

  //#region Helper Methods

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  private isCollaborationError(error: any): error is CollaborationError {
    return error && error.conflictType !== undefined;
  }

  private getSessionProgress(session: ReviewSession | null): number {
    if (!session) return 0;
    if (session.status === 'closed') return 100;
    if (session.status === 'awaiting_review') return 0;
    if (session.status === 'in_progress') return 30;
    if (session.status === 'under_clarification') return 60;
    if (session.status === 'pending_judgment') return 90;
    return 0;
  }

  private formatTime(milliseconds: number): string {
    const minutes = Math.floor(milliseconds / 60000);
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    
    if (hours > 0) {
      return `${hours}h ${remainingMinutes}m`;
    }
    return `${minutes}m`;
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleString();
  }

  // Safe number formatter
  formatNumber(value: number | undefined, digits: number = 0): string {
    if (value === undefined || value === null) return '0';
    return value.toFixed(digits);
  }

  clearError(): void {
    this.error.set(null);
  }

  clearSuccess(): void {
    this.success.set(null);
  }

  setActiveTab(tab: 'submit' | 'session' | 'collaborate' | 'clarify' | 'judge'): void {
    this.activeTab.set(tab);
    this.error.set(null);
    this.success.set(null);
  }

  resetAll(): void {
    this.submitForm.reset({
      testContent: '',
      confidenceScore: 0.85,
      testType: 'unit',
      framework: 'xUnit',
      language: 'C#',
      testPurpose: '',
      riskLevel: 'medium',
      technicalDomains: '',
      reviewerGuidance: '',
      submittedBy: 'current-user',
      priority: 'medium',
      enableRealTimeCollaboration: true,
      autoSuggestImprovements: true
    });

    this.editForm.reset({
      editContent: '',
      editIntent: '',
      editType: 'modification',
      editorId: 'current-user',
      affectedLines: '',
      requestAiAnalysis: true
    });

    this.clarificationForm.reset({
      humanQuestion: '',
      questionType: 'general',
      urgency: 'normal',
      contextTags: ''
    });

    this.judgmentForm.reset({
      decision: 'approve',
      reasoning: '',
      confidenceInJudgment: 1.0,
      storeForTraining: true
    });

    // Clear form arrays
    while (this.suggestedImprovements.length) {
      this.suggestedImprovements.removeAt(0);
    }

    while (this.areasOfConcern.length) {
      this.areasOfConcern.removeAt(0);
    }

    this.currentSessionId.set(null);
    this.currentSession.set(null);
    this.submitResponse.set(null);
    this.sessionResponse.set(null);
    this.collaborateResponse.set(null);
    this.clarifyResponse.set(null);
    this.judgeResponse.set(null);
    this.conflictError.set(null);
    this.error.set(null);
    this.success.set(null);
    this.activeTab.set('submit');
  }

  // Safe array access for templates
  safeArray<T>(arr: T[] | undefined | null): T[] {
    return arr || [];
  }

  //#endregion
}