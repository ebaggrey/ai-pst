// services/human-review.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, Subject } from 'rxjs';
import { catchError, map, tap, shareReplay } from 'rxjs/operators';
//import { v4 as uuidv4 } from 'uuid';

import {
  ReviewRequest,
  ReviewSessionResponse,
  ReviewSession,
  CollaborativeEditRequest,
  CollaborationResponse,
  CollaborationError,
  ClarificationRequest,
  ClarificationResponse,
  JudgmentRequest,
  JudgmentResponse,
  HumanReviewError,
  GeneratedTest,
  ReviewContext,
  SubmissionMetadata,
  AiClarification,
  ReviewOutcome,
  RiskLevel,
  Priority,
  ReviewDecision
} from '../Models/human-review.models';
import { InitialQuestion, ReviewChecklistItem } from '../Models/human-review.models';

@Injectable({
  providedIn: 'root'
})
export class HumanReviewService {
  private readonly baseUrl = '/api/human-review';
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    })
  };

  // Subjects for real-time updates
  private sessionUpdates = new Subject<ReviewSession>();
  private editConflicts = new Subject<CollaborationError>();
  private sessionClosed = new Subject<{ sessionId: string, outcome: ReviewOutcome }>();

  // Observables for components to subscribe to
  public sessionUpdates$ = this.sessionUpdates.asObservable();
  public editConflicts$ = this.editConflicts.asObservable();
  public sessionClosed$ = this.sessionClosed.asObservable();

  // Active sessions cache
  private activeSessionsCache = new Map<string, Observable<ReviewSession>>();

  constructor(private http: HttpClient) {}

  //#region Public API Methods

  /**
   * Submit AI-generated test for human review
   */
  submitForHumanReview(request: ReviewRequest): Observable<ReviewSessionResponse> {
    // Validate required fields
    const validationError = this.validateReviewRequest(request);
    if (validationError) {
      return throwError(() => validationError);
    }

    return this.http.post<ReviewSessionResponse>(
      `${this.baseUrl}/submit`,
      this.serializeRequest(request),
      this.httpOptions
    ).pipe(
      catchError(this.handleError),
      tap(response => {
        console.log(`Review session created: ${response.sessionId}`);
        this.clearSessionCache(response.sessionId);
      })
    );
  }

  /**
   * Get review session details
   */
  getReviewSession(sessionId: string, forceRefresh = false): Observable<ReviewSession> {
    if (!forceRefresh && this.activeSessionsCache.has(sessionId)) {
      return this.activeSessionsCache.get(sessionId)!;
    }

    const session$ = this.http.get<ReviewSession>(
      `${this.baseUrl}/${sessionId}`,
      this.httpOptions
    ).pipe(
      catchError(this.handleError),
      shareReplay(1)
    );

    this.activeSessionsCache.set(sessionId, session$);
    return session$;
  }

  /**
   * Collaborate on test with real-time editing
   */
  collaborateOnTest(sessionId: string, request: CollaborativeEditRequest): Observable<CollaborationResponse> {
    // Validate edit request
    const validationError = this.validateEditRequest(request);
    if (validationError) {
      return throwError(() => validationError);
    }

    return this.http.post<CollaborationResponse>(
      `${this.baseUrl}/${sessionId}/collaborate`,
      this.serializeRequest(request),
      this.httpOptions
    ).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 409 && error.error) {
          const conflictError = error.error as CollaborationError;
          this.editConflicts.next(conflictError);
          return throwError(() => conflictError);
        }
        return this.handleError(error);
      }),
      tap(response => {
        this.clearSessionCache(sessionId);
        this.sessionUpdates.next(response.session);
      })
    );
  }

  /**
   * Request clarification from AI about the test
   */
  requestClarification(sessionId: string, request: ClarificationRequest): Observable<ClarificationResponse> {
    // Validate clarification request
    const validationError = this.validateClarificationRequest(request);
    if (validationError) {
      return throwError(() => validationError);
    }

    return this.http.post<ClarificationResponse>(
      `${this.baseUrl}/${sessionId}/clarify`,
      this.serializeRequest(request),
      this.httpOptions
    ).pipe(
      catchError(this.handleError),
      tap(() => {
        this.clearSessionCache(sessionId);
      })
    );
  }

  /**
   * Provide final human judgment on the test
   */
  provideHumanJudgment(sessionId: string, request: JudgmentRequest): Observable<JudgmentResponse> {
    // Validate judgment request
    const validationError = this.validateJudgmentRequest(request);
    if (validationError) {
      return throwError(() => validationError);
    }

    return this.http.post<JudgmentResponse>(
      `${this.baseUrl}/${sessionId}/judge`,
      this.serializeRequest(request),
      this.httpOptions
    ).pipe(
      catchError(this.handleError),
      tap(response => {
        this.clearSessionCache(sessionId);
        this.sessionClosed.next({ sessionId, outcome: response.outcome });
      })
    );
  }

  //#endregion

  //#region Helper Methods

  /**
   * Create a new review request with default values
   */
  createReviewRequest(
    generatedTest: GeneratedTest,
    context: ReviewContext,
    reviewerGuidance: string,
    metadata: SubmissionMetadata
  ): ReviewRequest {
    return {
      generatedContent: generatedTest,
      context: context,
      reviewerGuidance: reviewerGuidance,
      submissionMetadata: metadata,
      preferredReviewers: [],
      priority: Priority.MEDIUM,
      enableRealTimeCollaboration: true,
      autoSuggestImprovements: true,
      reviewCategories: [],
      additionalSettings: {}
    };
  }

  /**
   * Create a collaborative edit request
   */
  createCollaborativeEditRequest(
    content: string,
    intent: string,
    editorId: string,
    editType = 'modification'
  ): CollaborativeEditRequest {
    return {
      userEdit: {
        content: content,
        intent: intent,
        editorId: editorId,
        editType: editType,
        affectedLines: [],
        metadata: {},
        createdAt: new Date(),
        priority: 5
      },
      editContext: '',
      relatedIssues: [],
      requestAiAnalysis: true
    };
  }

  /**
   * Create a clarification request
   */
  createClarificationRequest(
    question: string,
    questionType = 'general',
    contextTags: string[] = [],
    urgency = 'normal'
  ): ClarificationRequest {
    return {
      humanQuestion: question,
      questionType: questionType,
      contextTags: contextTags,
      urgency: urgency,
      additionalContext: {}
    };
  }

  /**
   * Create a judgment request
   */
  createJudgmentRequest(
    decision: ReviewDecision,
    reasoning: string,
    suggestedImprovements: string[] = [],
    areasOfConcern: string[] = [],
    confidence = 1.0
  ): JudgmentRequest {
    return {
      judgment: {
        decision: decision,
        reasoning: reasoning,
        suggestedImprovements: suggestedImprovements,
        areasOfConcern: areasOfConcern,
        confidenceInJudgment: confidence,
        supportingEvidence: [],
        specificFeedback: {}
      },
      areasReviewed: [],
      feedbackForAi: '',
      storeForTraining: true,
      metadata: {}
    };
  }

  /**
   * Estimate review time based on content and risk level
   */
  estimateReviewTime(contentLength: number, riskLevel: RiskLevel): number {
    let baseTime = 15 * 60 * 1000; // 15 minutes in milliseconds
    
    if (riskLevel === RiskLevel.HIGH || riskLevel === RiskLevel.CRITICAL) {
      baseTime += 30 * 60 * 1000; // Add 30 minutes
    }
    
    if (contentLength > 1000) {
      baseTime += 10 * 60 * 1000; // Add 10 minutes
    }
    
    return baseTime;
  }

  /**
   * Generate default review checklist
   */
  generateReviewChecklist(): ReviewChecklistItem[] {
    return [
      {
        item: 'Test Purpose Alignment',
        category: 'business',
        description: 'Does the test align with the stated purpose?',
        isRequired: true,
        guidance: 'Check if test covers the intended functionality',
        priority: 'high'
      },
      {
        item: 'Edge Cases',
        category: 'technical',
        description: 'Are important edge cases covered?',
        isRequired: true,
        guidance: 'Look for boundary conditions and error cases',
        priority: 'high'
      },
      {
        item: 'Code Quality',
        category: 'technical',
        description: 'Is the code readable and maintainable?',
        isRequired: true,
        guidance: 'Check naming, structure, and comments',
        priority: 'medium'
      },
      {
        item: 'Assertions',
        category: 'technical',
        description: 'Are assertions meaningful and complete?',
        isRequired: true,
        guidance: 'Verify all important conditions are tested',
        priority: 'medium'
      },
      {
        item: 'Performance',
        category: 'technical',
        description: 'Is the test efficient and non-blocking?',
        isRequired: false,
        guidance: 'Check for long-running operations or memory issues',
        priority: 'low'
      }
    ];
  }

  /**
   * Generate initial questions for review session
   */
  generateInitialQuestions(): InitialQuestion[] {
    return [
      {
        question: 'Does this test adequately cover the main business requirement?',
        type: 'validating',
        whyImportant: 'Ensures test serves its purpose',
        isRequired: true,
        priority: 1,
        category: 'business'
      },
      {
        question: 'What edge cases might be missing?',
        type: 'probing',
        whyImportant: 'Improves test robustness',
        isRequired: false,
        priority: 2,
        category: 'technical'
      },
      {
        question: 'Are there any security concerns with this test?',
        type: 'security',
        whyImportant: 'Prevents security vulnerabilities',
        isRequired: false,
        priority: 3,
        category: 'security'
      }
    ];
  }

  /**
   * Get missing elements from review request
   */
  getMissingElements(request: ReviewRequest): string[] {
    const missing: string[] = [];
    
    if (!request.generatedContent) missing.push('generatedContent');
    if (!request.context?.testPurpose) missing.push('testPurpose');
    if (!request.context?.riskLevel) missing.push('riskLevel');
    if (!request.reviewerGuidance) missing.push('reviewerGuidance');
    if (!request.submissionMetadata) missing.push('submissionMetadata');
    
    return missing;
  }

  /**
   * Generate context enhancement prompts
   */
  generateContextPrompts(): string[] {
    return [
      'What specific behavior are you testing?',
      'What are the acceptance criteria?',
      'Are there any special constraints or requirements?',
      'What edge cases should be considered?',
      'What is the expected outcome?'
    ];
  }

  //#endregion

  //#region Validation Methods

  private validateReviewRequest(request: ReviewRequest): HumanReviewError | null {
    const missingElements = this.getMissingElements(request);
    
    if (missingElements.length > 0) {
      return {
        errorCode: 'INSUFFICIENT_CONTEXT',
        message: 'Cannot start review without proper context',
        requiredElements: ['generatedContent', 'testPurpose', 'riskLevel', 'reviewerGuidance', 'submissionMetadata'],
        missingElements: missingElements,
        suggestion: 'Please provide all required fields',
        reportedAt: new Date()
      };
    }

    if (request.generatedContent.content.length < 10) {
      return {
        errorCode: 'CONTENT_TOO_SHORT',
        message: 'Test content is too short',
        suggestion: 'Please provide more substantial test content',
        reportedAt: new Date()
      };
    }

    if (request.generatedContent.confidenceScore < 0 || request.generatedContent.confidenceScore > 1) {
      return {
        errorCode: 'INVALID_CONFIDENCE',
        message: 'Confidence score must be between 0 and 1',
        suggestion: 'Please provide a valid confidence score',
        reportedAt: new Date()
      };
    }

    return null;
  }

  private validateEditRequest(request: CollaborativeEditRequest): HumanReviewError | null {
    if (!request.userEdit?.content) {
      return {
        errorCode: 'EMPTY_EDIT',
        message: 'Edit content cannot be empty',
        suggestion: 'Please provide content for the edit',
        reportedAt: new Date()
      };
    }

    if (request.userEdit.content.length > 10000) {
      return {
        errorCode: 'EDIT_TOO_LARGE',
        message: 'Edit content is too large',
        suggestion: 'Please break the edit into smaller changes',
        reportedAt: new Date()
      };
    }

    if (!request.userEdit.intent) {
      return {
        errorCode: 'MISSING_INTENT',
        message: 'Edit intent is required',
        suggestion: 'Please describe why you are making this change',
        reportedAt: new Date()
      };
    }

    return null;
  }

  private validateClarificationRequest(request: ClarificationRequest): HumanReviewError | null {
    if (!request.humanQuestion || request.humanQuestion.length < 10) {
      return {
        errorCode: 'QUESTION_TOO_SHORT',
        message: 'Question is too short',
        suggestion: 'Please ask a more detailed question',
        reportedAt: new Date()
      };
    }

    if (request.humanQuestion.length > 1000) {
      return {
        errorCode: 'QUESTION_TOO_LONG',
        message: 'Question is too long',
        suggestion: 'Please break your question into multiple questions',
        reportedAt: new Date()
      };
    }

    return null;
  }

  private validateJudgmentRequest(request: JudgmentRequest): HumanReviewError | null {
    if (!request.judgment?.decision) {
      return {
        errorCode: 'MISSING_DECISION',
        message: 'Judgment decision is required',
        suggestion: 'Please provide a decision (approve, request-revision, or reject)',
        reportedAt: new Date()
      };
    }

    if (!request.judgment.reasoning || request.judgment.reasoning.length < 10) {
      return {
        errorCode: 'INSUFFICIENT_REASONING',
        message: 'Reasoning is required and must be detailed',
        suggestion: 'Please provide detailed reasoning for your judgment',
        reportedAt: new Date()
      };
    }

    if (request.judgment.decision === ReviewDecision.REQUEST_REVISION && 
        (!request.judgment.suggestedImprovements || request.judgment.suggestedImprovements.length === 0)) {
      return {
        errorCode: 'MISSING_IMPROVEMENTS',
        message: 'Suggested improvements are required when requesting revision',
        suggestion: 'Please provide at least one suggested improvement',
        reportedAt: new Date()
      };
    }

    return null;
  }

  //#endregion

  //#region Private Methods

  private serializeRequest(request: any): any {
    // Convert dates to ISO strings
    return JSON.parse(JSON.stringify(request, (key, value) => {
      if (value instanceof Date) {
        return value.toISOString();
      }
      return value;
    }));
  }

  private clearSessionCache(sessionId: string): void {
    this.activeSessionsCache.delete(sessionId);
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An error occurred';
    let errorCode = 'UNKNOWN_ERROR';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else {
      // Server-side error
      errorCode = error.status.toString();
      if (error.error && error.error.message) {
        errorMessage = error.error.message;
      } else {
        errorMessage = `Server returned ${error.status}: ${error.statusText}`;
      }
    }
    
    const humanReviewError: HumanReviewError = {
      errorCode: errorCode,
      message: errorMessage,
      reportedAt: new Date(),
      recoveryActions: ['retry', 'contact-support']
    };
    
    return throwError(() => humanReviewError);
  }

  //#endregion

  //#region Utility Methods for Components

  /**
   * Check if a session is active
   */
  isSessionActive(session: ReviewSession): boolean {
    return session.status !== 'closed';
  }

  /**
   * Get session progress percentage
   */
  getSessionProgress(session: ReviewSession): number {
    if (session.status === 'closed') return 100;
    if (session.status === 'awaiting_review') return 0;
    if (session.status === 'in_progress') return 30;
    if (session.status === 'under_clarification') return 60;
    if (session.status === 'pending_judgment') return 90;
    return 0;
  }

  /**
   * Format time for display
   */
  formatTime(milliseconds: number): string {
    const minutes = Math.floor(milliseconds / 60000);
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    
    if (hours > 0) {
      return `${hours}h ${remainingMinutes}m`;
    }
    return `${minutes}m`;
  }

  /**
   * Calculate answer relevance score
   */
  calculateAnswerRelevance(question: string, answer: string): number {
    const questionWords = question.toLowerCase().split(/\s+/).filter(word => word.length > 2);
    const answerWords = answer.toLowerCase().split(/\s+/).filter(word => word.length > 2);
    
    if (questionWords.length === 0) return 0;
    
    const matchingWords = questionWords.filter(word => 
      answerWords.some(answerWord => answerWord.includes(word) || word.includes(answerWord))
    );
    
    return matchingWords.length / questionWords.length;
  }

  /**
   * Generate follow-up questions
   */
  generateFollowUpQuestions(question: string, answer: AiClarification): string[] {
    const followUps: string[] = [];
    
    if (answer.confidence < 0.7) {
      followUps.push('Could you elaborate on that?');
    }
    
    if (answer.alternatives && answer.alternatives.length > 0) {
      followUps.push('What about alternative approaches?');
    }
    
    if (question.toLowerCase().includes('how') || question.toLowerCase().includes('why')) {
      followUps.push('Could you provide an example?');
    }
    
    return followUps.length > 0 ? followUps : [
      'Could you provide more details?',
      'How does this apply to my specific case?',
      'Are there any exceptions to this rule?'
    ];
  }

  //#endregion
}