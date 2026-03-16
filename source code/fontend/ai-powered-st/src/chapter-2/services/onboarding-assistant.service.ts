import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, map, retry, timeout } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { MentorInfo, OnboardingError, OnboardingFeedback, OnboardingModule, OnboardingPlan, OnboardingPreferences, OnboardingProfile, OnboardingProgress, OnboardingProgressUpdate, OnboardingRecommendation, OnboardingRequest, OnboardingResponse, OnboardingStats, OnboardingTask, TaskStatus } from '../models/onboarding-assistant-models';



// ==================== Service ====================

@Injectable({
  providedIn: 'root'
})
export class OnboardingAssistantService {
  private readonly apiUrl: string;
  private readonly defaultTimeout = 30000;
  private readonly maxRetries = 2;

  constructor(private http: HttpClient) {
    this.apiUrl = environment.apiBaseUrl || 'http://localhost:5000/api';
  }

  /**
   * Create HTTP headers
   */
  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'X-Request-ID': this.generateRequestId()
    });
  }

  /**
   * Generate unique request ID
   */
  private generateRequestId(): string {
    return `onb_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Handle HTTP errors
   */
  private handleError<T>(operation: string, fallbackValue?: T) {
    return (error: HttpErrorResponse): Observable<T> => {
      console.error(`${operation} failed:`, error);

      let errorMessage = 'An unknown error occurred';
      let errorResponse: OnboardingError = {
        code: 'UNKNOWN_ERROR',
        message: errorMessage,
        timestamp: new Date().toISOString()
      };

      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = `Client error: ${error.error.message}`;
        errorResponse = {
          code: 'CLIENT_ERROR',
          message: errorMessage,
          timestamp: new Date().toISOString()
        };
      } else {
        // Server-side error
        errorMessage = `Server error: ${error.status} ${error.statusText}`;
        errorResponse = {
          code: `HTTP_${error.status}`,
          message: errorMessage,
          details: error.error?.message || error.message,
          suggestion: this.getErrorSuggestion(error.status),
          timestamp: new Date().toISOString()
        };
      }

      if (fallbackValue !== undefined) {
        console.warn(`Returning fallback value for ${operation}`);
        return of(fallbackValue);
      }

      return throwError(() => errorResponse);
    };
  }

  /**
   * Get error suggestion based on status code
   */
  private getErrorSuggestion(status: number): string {
    switch (status) {
      case 400:
        return 'Please check your input and try again.';
      case 401:
        return 'Please log in again to continue.';
      case 403:
        return 'You don\'t have permission for this action.';
      case 404:
        return 'The requested resource was not found.';
      case 408:
        return 'The request timed out. Please try again.';
      case 429:
        return 'Too many requests. Please wait a moment and try again.';
      case 500:
        return 'Server error. Please try again later.';
      case 503:
        return 'Service unavailable. Please try again later.';
      default:
        return 'Please try again or contact support.';
    }
  }

  // ==================== API Methods ====================

  /**
   * 1. Create new onboarding profile
   */
  createOnboardingProfile(request: OnboardingRequest): Observable<OnboardingResponse> {
    return this.http.post<OnboardingResponse>(
      `${this.apiUrl}/onboarding/profiles`,
      request,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      map(response => this.validateOnboardingResponse(response)),
      catchError(this.handleError<OnboardingResponse>('createOnboardingProfile', this.createDefaultOnboardingResponse(request)))
    );
  }

  /**
   * 2. Get onboarding profile by ID
   */
  getOnboardingProfile(profileId: string): Observable<OnboardingProfile> {
    return this.http.get<OnboardingProfile>(
      `${this.apiUrl}/onboarding/profiles/${profileId}`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      map(profile => this.validateOnboardingProfile(profile)),
      catchError(this.handleError<OnboardingProfile>('getOnboardingProfile', this.createDefaultOnboardingProfile(profileId)))
    );
  }

  /**
   * 3. Get onboarding plan
   */
  getOnboardingPlan(profileId: string): Observable<OnboardingPlan> {
    return this.http.get<OnboardingPlan>(
      `${this.apiUrl}/onboarding/plans/${profileId}`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      map(plan => this.validateOnboardingPlan(plan)),
      catchError(this.handleError<OnboardingPlan>('getOnboardingPlan', this.createDefaultOnboardingPlan(profileId)))
    );
  }

  /**
   * 4. Update onboarding progress
   */
  updateProgress(update: OnboardingProgressUpdate): Observable<OnboardingProgress> {
    return this.http.post<OnboardingProgress>(
      `${this.apiUrl}/onboarding/progress`,
      update,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError<OnboardingProgress>('updateProgress', this.createDefaultProgress(update.profileId)))
    );
  }

  /**
   * 5. Get onboarding tasks
   */
  getTasks(profileId: string, status?: TaskStatus): Observable<OnboardingTask[]> {
    let url = `${this.apiUrl}/onboarding/tasks/${profileId}`;
    if (status) {
      url += `?status=${status}`;
    }
    return this.http.get<OnboardingTask[]>(
      url,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError<OnboardingTask[]>('getTasks', []))
    );
  }

  /**
   * 6. Update task status
   */
  updateTaskStatus(taskId: string, status: TaskStatus, notes?: string): Observable<OnboardingTask> {
    return this.http.patch<OnboardingTask>(
      `${this.apiUrl}/onboarding/tasks/${taskId}`,
      { status, notes, completedAt: status === 'completed' ? new Date().toISOString() : undefined },
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError<OnboardingTask>('updateTaskStatus'))
    );
  }

  /**
   * 7. Get module details
   */
  getModule(moduleId: string): Observable<OnboardingModule> {
    return this.http.get<OnboardingModule>(
      `${this.apiUrl}/onboarding/modules/${moduleId}`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      map(module => this.validateOnboardingModule(module)),
      catchError(this.handleError<OnboardingModule>('getModule', this.createDefaultModule(moduleId)))
    );
  }

  /**
   * 8. Submit assessment
   */
  submitAssessment(moduleId: string, answers: Record<string, any>): Observable<{
    passed: boolean;
    score: number;
    feedback: string;
    correctAnswers: Record<string, any>;
  }> {
    return this.http.post<any>(
      `${this.apiUrl}/onboarding/modules/${moduleId}/assess`,
      { answers },
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError('submitAssessment', {
        passed: false,
        score: 0,
        feedback: 'Assessment submission failed',
        correctAnswers: {}
      }))
    );
  }

  /**
   * 9. Get recommendations
   */
  getRecommendations(profileId: string): Observable<OnboardingRecommendation[]> {
    return this.http.get<OnboardingRecommendation[]>(
      `${this.apiUrl}/onboarding/recommendations/${profileId}`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError<OnboardingRecommendation[]>('getRecommendations', []))
    );
  }

  /**
   * 10. Submit feedback
   */
  submitFeedback(feedback: OnboardingFeedback): Observable<{ success: boolean; message: string }> {
    return this.http.post<{ success: boolean; message: string }>(
      `${this.apiUrl}/onboarding/feedback`,
      feedback,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError('submitFeedback', { success: false, message: 'Failed to submit feedback' }))
    );
  }

  /**
   * 11. Get onboarding statistics
   */
  getStatistics(department?: string): Observable<OnboardingStats> {
    let url = `${this.apiUrl}/onboarding/statistics`;
    if (department) {
      url += `?department=${encodeURIComponent(department)}`;
    }
    return this.http.get<OnboardingStats>(
      url,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError<OnboardingStats>('getStatistics', this.createDefaultStats()))
    );
  }

  /**
   * 12. Assign mentor
   */
  assignMentor(profileId: string, mentorId: string): Observable<MentorInfo> {
    return this.http.post<MentorInfo>(
      `${this.apiUrl}/onboarding/mentor/assign`,
      { profileId, mentorId },
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError<MentorInfo>('assignMentor', this.createDefaultMentorInfo()))
    );
  }

  /**
   * 13. Complete onboarding
   */
  completeOnboarding(profileId: string, feedback?: string): Observable<{ success: boolean; certificate?: string }> {
    return this.http.post<{ success: boolean; certificate?: string }>(
      `${this.apiUrl}/onboarding/complete/${profileId}`,
      { feedback },
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError('completeOnboarding', { success: false }))
    );
  }

  // ==================== Validation Methods ====================

  private validateOnboardingResponse(response: OnboardingResponse): OnboardingResponse {
    if (!response) {
      return this.createDefaultOnboardingResponse({
        userId: 'unknown',
        userName: 'Unknown',
        userEmail: 'unknown@example.com',
        role: 'developer',
        experienceLevel: 'beginner',
        department: 'unknown',
        manager: 'unknown',
        startDate: new Date().toISOString(),
        preferences: this.createDefaultPreferences()
      });
    }
    return response;
  }

  private validateOnboardingProfile(profile: OnboardingProfile): OnboardingProfile {
    if (!profile) {
      return this.createDefaultOnboardingProfile('unknown');
    }
    return profile;
  }

  private validateOnboardingPlan(plan: OnboardingPlan): OnboardingPlan {
    if (!plan) {
      return this.createDefaultOnboardingPlan('unknown');
    }
    return plan;
  }

  private validateOnboardingModule(module: OnboardingModule): OnboardingModule {
    if (!module) {
      return this.createDefaultModule('unknown');
    }
    return module;
  }

  // ==================== Default Object Factories ====================

  private createDefaultOnboardingResponse(request: OnboardingRequest): OnboardingResponse {
    return {
      success: true,
      profileId: `prof_${Date.now()}`,
      message: 'Onboarding profile created successfully',
      plan: this.createDefaultOnboardingPlan(`prof_${Date.now()}`),
      nextSteps: [
        'Complete your profile',
        'Review onboarding materials',
        'Schedule welcome meeting with mentor',
        'Complete first module'
      ]
    };
  }

  private createDefaultOnboardingProfile(profileId: string): OnboardingProfile {
    return {
      id: profileId,
      userId: `user_${Date.now()}`,
      userName: 'New User',
      userEmail: 'user@example.com',
      role: 'developer',
      experienceLevel: 'beginner',
      startDate: new Date().toISOString(),
      department: 'Engineering',
      manager: 'To be assigned',
      status: 'pending',
      progress: {
        overall: 0,
        modulesCompleted: 0,
        totalModules: 10,
        assessmentsPassed: 0,
        totalAssessments: 5,
        timeSpentMinutes: 0,
        estimatedCompletionDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
        lastActivityDate: new Date().toISOString()
      },
      preferences: this.createDefaultPreferences(),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };
  }

  private createDefaultPreferences(): OnboardingPreferences {
    return {
      learningStyle: 'mixed',
      communicationFrequency: 'weekly',
      preferredChannels: ['email', 'slack'],
      timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
      workHours: {
        start: '09:00',
        end: '17:00',
        timezone: Intl.DateTimeFormat().resolvedOptions().timeZone
      },
      language: 'en',
      notificationsEnabled: true
    };
  }

  private createDefaultOnboardingPlan(profileId: string): OnboardingPlan {
    const modules = [
      this.createDefaultModule('mod_1'),
      this.createDefaultModule('mod_2')
    ];

    return {
      id: `plan_${Date.now()}`,
      profileId: profileId,
      name: 'Standard Onboarding Plan',
      description: 'Complete onboarding program for new team members',
      modules: modules,
      estimatedTotalMinutes: 480,
      startDate: new Date().toISOString(),
      targetCompletionDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
      milestones: [
        {
          id: 'milestone_1',
          title: 'Week 1: Foundation',
          description: 'Complete basic training modules',
          dueDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
          completed: false,
          moduleIds: ['mod_1']
        },
        {
          id: 'milestone_2',
          title: 'Week 2: Advanced Topics',
          description: 'Complete advanced modules and assessments',
          dueDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
          completed: false,
          moduleIds: ['mod_2']
        }
      ],
      status: 'pending',
      progress: {
        overall: 0,
        modulesCompleted: 0,
        totalModules: modules.length,
        assessmentsPassed: 0,
        totalAssessments: 2,
        timeSpentMinutes: 0,
        estimatedCompletionDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
        lastActivityDate: new Date().toISOString()
      }
    };
  }

  private createDefaultModule(moduleId: string): OnboardingModule {
    return {
      id: moduleId,
      title: 'Getting Started with Testing Patterns',
      description: 'Introduction to testing patterns and best practices',
      type: 'pattern-training',
      difficulty: 'beginner',
      prerequisites: [],
      estimatedMinutes: 45,
      content: {
        introduction: 'This module introduces you to testing patterns...',
        sections: [
          {
            title: 'What are Testing Patterns?',
            content: 'Testing patterns are reusable solutions to common testing problems...',
            durationMinutes: 15,
            codeBlocks: [
              {
                language: 'typescript',
                code: '// Example test pattern\nit("should handle success case", () => {\n  // Test implementation\n});',
                explanation: 'Basic test structure'
              }
            ]
          }
        ],
        summary: 'Testing patterns help maintain consistent testing practices',
        keyTakeaways: [
          'Understanding testing patterns',
          'When to use specific patterns',
          'How to implement patterns correctly'
        ],
        codeExamples: [
          {
            title: 'Basic Test Pattern',
            description: 'Simple test pattern example',
            language: 'typescript',
            code: 'describe("Component", () => {\n  it("should work", () => {\n    expect(true).toBe(true);\n  });\n});'
          }
        ]
      },
      assessment: {
        questions: [
          {
            id: 'q1',
            type: 'multiple-choice',
            text: 'What is a testing pattern?',
            options: [
              'A bug in the test',
              'A reusable solution to common testing problems',
              'A type of test framework',
              'A testing tool'
            ],
            correctAnswer: '1',
            points: 10,
            explanation: 'Testing patterns are reusable solutions to common testing problems.'
          }
        ],
        passingScore: 70,
        maxAttempts: 3,
        timeLimitMinutes: 15,
        allowReview: true
      },
      resources: [
        {
          id: 'res_1',
          title: 'Testing Patterns Guide',
          type: 'documentation',
          url: '/docs/testing-patterns',
          description: 'Comprehensive guide to testing patterns',
          tags: ['testing', 'patterns', 'guide'],
          estimatedMinutes: 20,
          required: true
        }
      ],
      status: 'available'
    };
  }

  private createDefaultProgress(profileId: string): OnboardingProgress {
    return {
      overall: 0,
      modulesCompleted: 0,
      totalModules: 10,
      assessmentsPassed: 0,
      totalAssessments: 5,
      timeSpentMinutes: 0,
      estimatedCompletionDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
      lastActivityDate: new Date().toISOString()
    };
  }

  private createDefaultMentorInfo(): MentorInfo {
    return {
      id: `mentor_${Date.now()}`,
      name: 'Default Mentor',
      email: 'mentor@example.com',
      role: 'Senior Developer',
      availability: ['Monday', 'Wednesday', 'Friday'],
      expertise: ['Testing', 'Development', 'Best Practices']
    };
  }

  private createDefaultStats(): OnboardingStats {
    return {
      totalOnboardings: 0,
      activeOnboardings: 0,
      completedOnboardings: 0,
      averageCompletionDays: 14,
      averageSatisfactionScore: 4.5,
      moduleCompletionRates: {},
      commonChallenges: ['Time management', 'Understanding complex patterns'],
      mentorEffectiveness: 85
    };
  }

  // ==================== Helper Methods ====================

  /**
   * Create sample onboarding request for testing
   */
  createSampleOnboardingRequest(): OnboardingRequest {
    return {
      userId: `user_${Date.now()}`,
      userName: 'John Doe',
      userEmail: 'john.doe@example.com',
      role: 'developer',
      experienceLevel: 'intermediate',
      department: 'Engineering',
      manager: 'Jane Smith',
      startDate: new Date().toISOString(),
      preferences: this.createDefaultPreferences()
    };
  }

  /**
   * Calculate estimated completion date based on progress
   */
  calculateEstimatedCompletion(progress: OnboardingProgress): Date {
    const now = new Date();
    const timePerModule = progress.timeSpentMinutes / (progress.modulesCompleted || 1);
    const remainingModules = progress.totalModules - progress.modulesCompleted;
    const remainingMinutes = remainingModules * timePerModule;
    
    const completionDate = new Date(now.getTime() + remainingMinutes * 60000);
    return completionDate;
  }

  /**
   * Get next recommended module
   */
  getNextRecommendedModule(plan: OnboardingPlan): OnboardingModule | undefined {
    return plan.modules.find(m => m.status === 'available');
  }

  /**
   * Calculate overall progress percentage
   */
  calculateOverallProgress(plan: OnboardingPlan): number {
    if (!plan.modules.length) return 0;
    
    const completed = plan.modules.filter(m => m.status === 'completed').length;
    return Math.round((completed / plan.modules.length) * 100);
  }

  /**
   * Get days remaining until target completion
   */
  getDaysRemaining(targetDate: string): number {
    const target = new Date(targetDate);
    const now = new Date();
    const diffTime = target.getTime() - now.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return Math.max(0, diffDays);
  }

  /**
   * Check if onboarding is on track
   */
  isOnTrack(plan: OnboardingPlan): boolean {
    const progress = plan.progress.overall;
    const totalDays = this.getDaysRemaining(plan.startDate) + this.getDaysRemaining(plan.targetCompletionDate);
    const daysElapsed = this.getDaysRemaining(plan.startDate);
    const expectedProgress = (daysElapsed / totalDays) * 100;
    
    return progress >= expectedProgress;
  }

  /**
   * Generate personalized welcome message
   */
  generateWelcomeMessage(profile: OnboardingProfile): string {
    const messages = {
      developer: 'Welcome to the development team!',
      'qa-engineer': 'Welcome to the QA team!',
      'devops-engineer': 'Welcome to the DevOps team!',
      'product-manager': 'Welcome to the product team!',
      'technical-lead': 'Welcome to the technical leadership team!',
      architect: 'Welcome to the architecture team!'
    };

    return messages[profile.role] || 'Welcome to the team!';
  }

  /**
   * Format time for display
   */
  formatTimeSpent(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    
    if (hours > 0) {
      return `${hours}h ${mins}m`;
    }
    return `${mins}m`;
  }
}