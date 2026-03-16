import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, map, retry, timeout } from 'rxjs/operators';
import { environment } from '../../environments/environment';

// ==================== Models ====================

export interface TestExample {
  testName: string;
  input: string;
  expectedOutput: string;
  actualOutput?: string;
  tags?: string[];
  complexity: 'low' | 'medium' | 'high';
}

export interface PatternEstablishmentRequest {
  area: string;
  examples: TestExample[];
  desiredConsistency: 'low' | 'medium' | 'high';
  automationLevel: 'manual' | 'semi-automated' | 'fully-automated';
  validationCriteria: string[];
  metadata?: {
    teamSize: number;
    experienceLevel: string;
    timeline: string;
  };
}

export interface PatternImplementation {
  codeExamples: string[];
  configuration: Record<string, any>;
  dosAndDonts: string[];
}

export interface QualityIndicators {
  repeatabilityScore: number;
  learningCurve: 'easy' | 'medium' | 'steep';
  maintenanceCost: 'low' | 'medium' | 'high';
}

export interface AiAssistance {
  promptTemplates: string[];
  validationRules: string[];
  commonPitfalls: string[];
}

export interface AdoptionMetrics {
  estimatedTimeSave: string;
  errorReduction: string;
  teamSatisfaction: number;
}

export interface TestingPattern {
  id: string;
  name: string;
  area: string;
  problemStatement: string;
  solution: string;
  implementation: PatternImplementation;
  qualityIndicators: QualityIndicators;
  aiAssistance: AiAssistance;
  adoptionMetrics: AdoptionMetrics;
  createdAt: string;
  status: 'draft' | 'active' | 'deprecated' | 'archived';
}

export interface TrainingGenerationRequest {
  pattern: TestingPattern;
  audience: string;
  format: 'workshop-ready' | 'quick-start' | 'comprehensive';
  durationMinutes: number;
  includeHandsOn: boolean;
  prerequisites: string[];
  learningObjectives: string[];
}

export interface TrainingModule {
  title: string;
  content: string;
  durationMinutes: number;
  keyPoints: string[];
}

export interface Exercise {
  title: string;
  description: string;
  solutionHint: string;
  solution: string;
}

export interface HandsOnSection {
  included: boolean;
  setupInstructions?: string;
  exercises?: Exercise[];
  expectedOutcome?: string;
}

export interface QuizQuestion {
  text: string;
  options: string[];
  correctAnswerIndex: number;
  explanation: string;
}

export interface PracticalTask {
  description: string;
  requirements: string[];
  successCriteria: string[];
}

export interface Assessment {
  quizQuestions: QuizQuestion[];
  practicalTask?: PracticalTask;
  passingScore: number;
}

export interface TrainingMaterials {
  id: string;
  patternId: string;
  audience: string;
  title: string;
  format: 'workshop-ready' | 'quick-start' | 'comprehensive';
  durationMinutes: number;
  modules: TrainingModule[];
  prerequisites: string[];
  learningObjectives: string[];
  handsOn: HandsOnSection;
  assessment: Assessment;
  generatedAt: string;
}

export interface PipelineStageRequest {
  name: string;
  tasks: string[];
}

export interface QualityGateRequest {
  metric: string;
  threshold: number;
}

export interface PipelineRequest {
  patternId: string;
  triggerEvents: string[];
  stages: PipelineStageRequest[];
  qualityGates: QualityGateRequest[];
}

export interface PipelineTask {
  name: string;
  type: string;
  configuration?: Record<string, any>;
  timeoutMinutes: number;
}

export interface PipelineStage {
  name: string;
  tasks: PipelineTask[];
  parameters?: Record<string, string>;
  executor: string;
}

export interface QualityGate {
  metric: string;
  threshold: number;
  operator: '>=' | '<=' | '>' | '<' | '==';
  actionOnFail: 'block' | 'warn' | 'continue';
}

export interface RollbackStrategy {
  enabled: boolean;
  triggerCondition: string;
  method: 'automatic' | 'manual';
}

export interface DeploymentConfiguration {
  environment: 'development' | 'staging' | 'production';
  autoDeploy: boolean;
  approvers?: string[];
  rollbackStrategy: RollbackStrategy;
}

export interface AlertRule {
  name: string;
  condition: string;
  notifyChannels: ('email' | 'slack' | 'teams' | 'pagerduty')[];
}

export interface MonitoringConfiguration {
  metrics: string[];
  alertRules: AlertRule[];
  dashboardUrl: string;
}

export interface PipelineBlueprint {
  id: string;
  patternId: string;
  name: string;
  triggerEvents: string[];
  stages: PipelineStage[];
  qualityGates: QualityGate[];
  deploymentConfig: DeploymentConfiguration;
  monitoringConfig: MonitoringConfiguration;
  createdAt: string;
}

export interface PatternError {
  patternArea: string;
  failureType: 'generation' | 'validation' | 'adoption';
  symptoms: string[];
  rootCause: string;
  mitigationSteps: string[];
  temporaryWorkaround?: string;
  errorTime: string;
  correlationId: string;
}

export interface ApiErrorResponse {
  errorCode: string;
  message: string;
  details?: string;
  correlationId: string;
  timestamp: string;
  patternError?: PatternError;
}

export interface HealthStatus {
  status: 'healthy' | 'degraded' | 'unhealthy';
  timestamp: string;
  service: string;
  version?: string;
}

export interface HealthDependency {
  name: string;
  status: 'healthy' | 'degraded' | 'unhealthy';
  message?: string;
}

export interface HealthStatusResponse extends HealthStatus {
  dependencies?: HealthDependency[];
}

// ==================== Service ====================

@Injectable({
  providedIn: 'root'
})
export class PatternEstablishmentService {
  private readonly apiUrl: string;
  private readonly defaultTimeout = 30000; // 30 seconds
  private readonly maxRetries = 2;

  constructor(private http: HttpClient) {
    this.apiUrl = environment.apiBaseUrl || 'http://localhost:5000/api';
  }

  /**
   * Create HTTP headers with proper content type
   */
  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'X-Request-ID': this.generateRequestId()
    });
  }

  /**
   * Generate unique request ID for tracing
   */
  private generateRequestId(): string {
    return `req_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Handle HTTP errors consistently
   */
  private handleError<T>(operation: string, fallbackValue?: T) {
    return (error: HttpErrorResponse): Observable<T> => {
      console.error(`${operation} failed:`, error);

      let errorMessage = 'An unknown error occurred';
      let patternError: PatternError | undefined;

      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = `Client error: ${error.error.message}`;
      } else {
        // Server-side error
        const apiError = error.error as ApiErrorResponse;
        errorMessage = apiError?.message || `Server error: ${error.status} ${error.statusText}`;
        patternError = apiError?.patternError;

        // Log detailed error information
        console.error(`Error details:`, {
          status: error.status,
          statusText: error.statusText,
          errorCode: apiError?.errorCode,
          correlationId: apiError?.correlationId,
          details: apiError?.details
        });
      }

      // Create a standardized error object
      const errorResponse: PatternError = patternError || {
        patternArea: 'unknown',
        failureType: 'generation',
        symptoms: [errorMessage],
        rootCause: error.message || 'Unknown cause',
        mitigationSteps: ['Check network connection', 'Verify API endpoint', 'Contact support'],
        temporaryWorkaround: 'Try again later',
        errorTime: new Date().toISOString(),
        correlationId: error.headers?.get('X-Correlation-ID') || this.generateRequestId()
      };

      // If fallback value is provided, return it instead of throwing
      if (fallbackValue !== undefined) {
        console.warn(`Returning fallback value for ${operation}`);
        return of(fallbackValue);
      }

      return throwError(() => errorResponse);
    };
  }

  /**
   * 1. Establish a new testing pattern
   */
  establishTestingPattern(area: string, examples: TestExample[]): Observable<TestingPattern> {
    const request: PatternEstablishmentRequest = {
      area,
      examples,
      desiredConsistency: 'high',
      automationLevel: 'semi-automated',
      validationCriteria: [
        'Pattern should be repeatable',
        'Easy for other team members to follow',
        'Produces consistent results',
        'Documented with examples'
      ],
      metadata: {
        teamSize: 5,
        experienceLevel: 'intermediate',
        timeline: '2 weeks'
      }
    };

    return this.http.post<TestingPattern>(
      `${this.apiUrl}/patternestablishment/establish`,
      request,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      map(pattern => this.validatePatternResponse(pattern)),
      catchError(this.handleError<TestingPattern>('establishTestingPattern', this.createDefaultTestingPattern(area)))
    );
  }

  /**
   * 2. Generate training materials
   */
  generateTrainingMaterials(pattern: TestingPattern, audience: string): Observable<TrainingMaterials> {
    const request: TrainingGenerationRequest = {
      pattern,
      audience,
      format: 'workshop-ready',
      durationMinutes: 60,
      includeHandsOn: true,
      prerequisites: ['basic testing knowledge', 'familiarity with our app'],
      learningObjectives: [
        `Understand when to use the ${pattern.name} pattern`,
        'Implement the pattern correctly',
        'Troubleshoot common issues',
        'Extend the pattern for edge cases'
      ]
    };

    return this.http.post<TrainingMaterials>(
      `${this.apiUrl}/patternestablishment/training/generate`,
      request,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError<TrainingMaterials>('generateTrainingMaterials', this.createDefaultTrainingMaterials(pattern.id, audience)))
    );
  }

  /**
   * 3. Create automation pipeline
   */
  createAutomationPipeline(pattern: TestingPattern): Observable<PipelineBlueprint> {
    const request: PipelineRequest = {
      patternId: pattern.id,
      triggerEvents: ['pr-created', 'schedule-daily', 'manual'],
      stages: [
        {
          name: 'pattern-validation',
          tasks: ['validate-syntax', 'check-coverage', 'run-smoke-tests']
        },
        {
          name: 'ai-generation',
          tasks: ['generate-variants', 'optimize-selectors', 'add-assertions']
        },
        {
          name: 'human-review',
          tasks: ['code-review', 'approval-workflow', 'merge-to-main']
        }
      ],
      qualityGates: [
        { metric: 'test-pass-rate', threshold: 95 },
        { metric: 'generation-confidence', threshold: 80 },
        { metric: 'reviewer-approval', threshold: 2 }
      ]
    };

    return this.http.post<PipelineBlueprint>(
      `${this.apiUrl}/patternestablishment/pipelines/create`,
      request,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      catchError(this.handleError<PipelineBlueprint>('createAutomationPipeline', this.createDefaultPipelineBlueprint(pattern.id)))
    );
  }

  /**
   * 4. Get pattern by ID
   */
  getPatternById(id: string): Observable<TestingPattern> {
    return this.http.get<TestingPattern>(
      `${this.apiUrl}/patternestablishment/patterns/${id}`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      map(pattern => this.validatePatternResponse(pattern)),
      catchError(this.handleError<TestingPattern>('getPatternById'))
    );
  }

  /**
   * 5. Get patterns by area
   */
  getPatternsByArea(area: string): Observable<TestingPattern[]> {
    return this.http.get<TestingPattern[]>(
      `${this.apiUrl}/patternestablishment/patterns/area/${encodeURIComponent(area)}`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(this.maxRetries),
      map(patterns => patterns.map(p => this.validatePatternResponse(p))),
      catchError(this.handleError<TestingPattern[]>('getPatternsByArea', []))
    );
  }

  /**
   * 6. Health check
   */
  checkHealth(): Observable<HealthStatusResponse> {
    return this.http.get<HealthStatusResponse>(
      `${this.apiUrl}/patternestablishment/health`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(5000), // Shorter timeout for health checks
      retry(1),
      catchError(this.handleError<HealthStatusResponse>('checkHealth', {
        status: 'degraded',
        timestamp: new Date().toISOString(),
        service: 'PatternEstablishmentAPI',
        version: 'unknown'
      }))
    );
  }

  /**
   * Validate pattern response - FIXED: No spread operator conflicts
   */
  private validatePatternResponse(pattern: TestingPattern): TestingPattern {
    if (!pattern) {
      throw new Error('Invalid pattern response: pattern is null');
    }

    // Create a new object with default values, then override with actual pattern values
    const defaultPattern = this.createDefaultTestingPattern(pattern.area || 'unknown');
    
    // Manually merge properties to avoid spread operator conflicts
    const validatedPattern: TestingPattern = {
      // Start with default values
      id: defaultPattern.id,
      name: defaultPattern.name,
      area: defaultPattern.area,
      problemStatement: defaultPattern.problemStatement,
      solution: defaultPattern.solution,
      //implementation: defaultPattern.implementation,
      qualityIndicators: defaultPattern.qualityIndicators,
      aiAssistance: defaultPattern.aiAssistance,
      adoptionMetrics: defaultPattern.adoptionMetrics,
      createdAt: defaultPattern.createdAt,
     // status: defaultPattern.status,
      
      // Override with actual pattern values where they exist
      ...(pattern.id && { id: pattern.id }),
      ...(pattern.name && { name: pattern.name }),
      ...(pattern.area && { area: pattern.area }),
      ...(pattern.problemStatement && { problemStatement: pattern.problemStatement }),
      ...(pattern.solution && { solution: pattern.solution }),
      ...(pattern.implementation && { implementation: pattern.implementation }),
      ...(pattern.createdAt && { createdAt: pattern.createdAt }),
      ...(pattern.status && { status: pattern.status })
    };

    // Handle nested objects separately to avoid overwriting
    if (pattern.qualityIndicators) {
      validatedPattern.qualityIndicators = {
        repeatabilityScore: pattern.qualityIndicators.repeatabilityScore ?? defaultPattern.qualityIndicators.repeatabilityScore,
        learningCurve: pattern.qualityIndicators.learningCurve ?? defaultPattern.qualityIndicators.learningCurve,
        maintenanceCost: pattern.qualityIndicators.maintenanceCost ?? defaultPattern.qualityIndicators.maintenanceCost
      };
    }

    if (pattern.aiAssistance) {
      validatedPattern.aiAssistance = {
        promptTemplates: pattern.aiAssistance.promptTemplates ?? defaultPattern.aiAssistance.promptTemplates,
        validationRules: pattern.aiAssistance.validationRules ?? defaultPattern.aiAssistance.validationRules,
        commonPitfalls: pattern.aiAssistance.commonPitfalls ?? defaultPattern.aiAssistance.commonPitfalls
      };
    }

    if (pattern.adoptionMetrics) {
      validatedPattern.adoptionMetrics = {
        estimatedTimeSave: pattern.adoptionMetrics.estimatedTimeSave ?? defaultPattern.adoptionMetrics.estimatedTimeSave,
        errorReduction: pattern.adoptionMetrics.errorReduction ?? defaultPattern.adoptionMetrics.errorReduction,
        teamSatisfaction: pattern.adoptionMetrics.teamSatisfaction ?? defaultPattern.adoptionMetrics.teamSatisfaction
      };
    }

    return validatedPattern;
  }

  /**
   * Create default testing pattern for fallback
   */
  private createDefaultTestingPattern(area: string = 'unknown'): TestingPattern {
    return {
      id: `temp_${Date.now()}`,
      name: `Default Pattern for ${area}`,
      area: area,
      problemStatement: 'Default problem statement',
      solution: 'Default solution',
      implementation: {
        codeExamples: ['// Default code example'],
        configuration: { timeout: 30, retryCount: 3 },
        dosAndDonts: ['DO: Write clear tests', 'DON\'T: Skip error handling']
      },
      qualityIndicators: {
        repeatabilityScore: 75,
        learningCurve: 'medium',
        maintenanceCost: 'medium'
      },
      aiAssistance: {
        promptTemplates: ['Default prompt template'],
        validationRules: ['Default validation rule'],
        commonPitfalls: ['Default pitfall']
      },
      adoptionMetrics: {
        estimatedTimeSave: '30-50%',
        errorReduction: '40-60%',
        teamSatisfaction: 7
      },
      createdAt: new Date().toISOString(),
      status: 'draft'
    };
  }

  /**
   * Create default training materials for fallback - FIXED: No spread operator conflicts
   */
  private createDefaultTrainingMaterials(patternId: string, audience: string): TrainingMaterials {
    return {
      id: `temp_${Date.now()}`,
      patternId: patternId,
      audience: audience,
      title: `Training Materials for Pattern ${patternId}`,
      format: 'workshop-ready',
      durationMinutes: 60,
      modules: [
        {
          title: 'Introduction',
          content: 'Introduction to the pattern',
          durationMinutes: 15,
          keyPoints: ['Point 1', 'Point 2']
        },
        {
          title: 'Implementation',
          content: 'How to implement',
          durationMinutes: 30,
          keyPoints: ['Step 1', 'Step 2', 'Step 3']
        }
      ],
      prerequisites: ['Basic knowledge'],
      learningObjectives: ['Objective 1', 'Objective 2'],
      handsOn: {
        included: false
      },
      assessment: {
        quizQuestions: [],
        passingScore: 80
      },
      generatedAt: new Date().toISOString()
    };
  }

  /**
   * Create default pipeline blueprint for fallback - FIXED: No spread operator conflicts
   */
  private createDefaultPipelineBlueprint(patternId: string): PipelineBlueprint {
    return {
      id: `temp_${Date.now()}`,
      patternId: patternId,
      name: `Default Pipeline for ${patternId}`,
      triggerEvents: ['manual'],
      stages: [
        {
          name: 'validation',
          tasks: [
            {
              name: 'validate',
              type: 'validation',
              timeoutMinutes: 10
            }
          ],
          executor: 'default'
        }
      ],
      qualityGates: [
        {
          metric: 'test-pass-rate',
          threshold: 80,
          operator: '>=',
          actionOnFail: 'warn'
        }
      ],
      deploymentConfig: {
        environment: 'development',
        autoDeploy: false,
        rollbackStrategy: {
          enabled: true,
          triggerCondition: 'failureRate > 10%',
          method: 'automatic'
        }
      },
      monitoringConfig: {
        metrics: ['pipeline_duration'],
        alertRules: [],
        dashboardUrl: ''
      },
      createdAt: new Date().toISOString()
    };
  }

  /**
   * Create a sample test example for testing
   */
  createSampleTestExample(): TestExample {
    return {
      testName: 'Sample Test',
      input: 'Sample input data',
      expectedOutput: 'Expected result',
      actualOutput: '',
      tags: ['sample'],
      complexity: 'medium'
    };
  }

  /**
   * Create multiple sample test examples
   */
  createSampleTestExamples(count: number = 3): TestExample[] {
    return Array.from({ length: count }, (_, i) => ({
      testName: `Sample Test ${i + 1}`,
      input: `Input data for test ${i + 1}`,
      expectedOutput: `Expected result for test ${i + 1}`,
      actualOutput: '',
      tags: ['sample', `test-${i + 1}`],
      complexity: i % 2 === 0 ? 'low' : i % 3 === 0 ? 'high' : 'medium'
    }));
  }

  /**
   * Validate if a pattern is valid
   */
  isValidPattern(pattern: TestingPattern | null): boolean {
    if (!pattern) return false;
    return !!(pattern.id && pattern.name && pattern.area);
  }

  /**
   * Extract error message from error object
   */
  getErrorMessage(error: unknown): string {
    if (typeof error === 'string') return error;
    if (error instanceof Error) return error.message;
    if (error && typeof error === 'object') {
      if ('message' in error && typeof error.message === 'string') return error.message;
      if ('rootCause' in error && typeof error.rootCause === 'string') return error.rootCause;
    }
    return 'An unknown error occurred';
  }
}