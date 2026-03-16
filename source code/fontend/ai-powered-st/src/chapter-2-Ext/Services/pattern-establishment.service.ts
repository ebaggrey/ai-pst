import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap, timeout } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { ExampleMetrics, PatternError, PatternEstablishmentRequest, PatternValidationResult, PipelineBlueprint, PipelineRequest, TestingPattern, TrainingGenerationRequest, TrainingMaterials } from '../Models/pattern-models';

// ==================== Service ====================

@Injectable({
  providedIn: 'root'
})
export class PatternEstablishmentService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/patterns`;
  private readonly defaultTimeout = 180000; // 3 minutes for pattern operations

  constructor(private http: HttpClient) {}

  /**
   * Establish a new testing pattern
   */
  establishTestingPattern(request: PatternEstablishmentRequest): Observable<TestingPattern> {
    return this.http.post<TestingPattern>(
      `${this.baseUrl}/establish`,
      request
    ).pipe(
      timeout(this.defaultTimeout),
      tap(pattern => this.logPatternInfo('Pattern established', pattern)),
      catchError(error => this.handlePatternError(error, 'establish'))
    );
  }

  /**
   * Validate an existing pattern
   */
  validatePattern(pattern: TestingPattern): Observable<PatternValidationResult> {
    return this.http.post<PatternValidationResult>(
      `${this.baseUrl}/validate`,
      pattern
    ).pipe(
      tap(result => this.logValidationResult(result)),
      catchError(error => this.handlePatternError(error, 'validate'))
    );
  }

  /**
   * Generate training materials for a pattern
   */
  generateTeamTrainingMaterials(request: TrainingGenerationRequest): Observable<TrainingMaterials> {
    return this.http.post<TrainingMaterials>(
      `${this.baseUrl}/training/generate`,
      request
    ).pipe(
      timeout(this.defaultTimeout),
      tap(materials => this.logPatternInfo('Training materials generated', materials)),
      catchError(error => this.handlePatternError(error, 'training-generate'))
    );
  }

  /**
   * Create an automation pipeline for a pattern
   */
  createAutomationPipeline(request: PipelineRequest): Observable<PipelineBlueprint> {
    return this.http.post<PipelineBlueprint>(
      `${this.baseUrl}/pipelines/create`,
      request
    ).pipe(
      timeout(this.defaultTimeout),
      tap(pipeline => this.logPatternInfo('Pipeline created', pipeline)),
      catchError(error => this.handlePatternError(error, 'pipeline-create'))
    );
  }

  /**
   * Update an existing pattern
   */
  updatePattern(patternId: string, updates: TestingPattern): Observable<TestingPattern> {
    return this.http.put<TestingPattern>(
      `${this.baseUrl}/${patternId}`,
      updates
    ).pipe(
      tap(pattern => this.logPatternInfo('Pattern updated', pattern)),
      catchError(error => this.handlePatternError(error, 'update'))
    );
  }

  /**
   * Delete a pattern
   */
  deletePattern(patternId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}/${patternId}`
    ).pipe(
      tap(() => console.log(`Pattern ${patternId} deleted`)),
      catchError(error => this.handlePatternError(error, 'delete'))
    );
  }

  /**
   * Get patterns by area
   */
  getPatternsByArea(area: string): Observable<TestingPattern[]> {
    return this.http.get<TestingPattern[]>(
      `${this.baseUrl}/area/${area}`
    ).pipe(
      tap(patterns => console.log(`Retrieved ${patterns.length} patterns for area: ${area}`)),
      catchError(error => this.handlePatternError(error, 'get-by-area'))
    );
  }

  /**
   * Log a pattern error
   */
  logPatternError(error: PatternError): Observable<PatternError> {
    return this.http.post<PatternError>(
      `${this.baseUrl}/errors`,
      error
    ).pipe(
      tap(loggedError => console.log('Error logged:', loggedError)),
      catchError(err => this.handlePatternError(err, 'log-error'))
    );
  }

  /**
   * Batch validate multiple patterns
   */
  batchValidatePatterns(patterns: TestingPattern[]): Observable<PatternValidationResult[]> {
    return this.http.post<PatternValidationResult[]>(
      `${this.baseUrl}/batch-validate`,
      patterns
    ).pipe(
      tap(results => console.log(`Batch validated ${results.length} patterns`)),
      catchError(error => this.handlePatternError(error, 'batch-validate'))
    );
  }

  /**
   * Check service health
   */
  checkHealth(): Observable<any> {
    return this.http.get<any>(
      `${this.baseUrl}/health`
    ).pipe(
      tap(health => console.log('Health check:', health)),
      catchError(error => this.handlePatternError(error, 'health-check'))
    );
  }

  // ==================== Utility Methods ====================

  /**
   * Generate example metrics from code
   */
  generateExampleMetrics(code: string): ExampleMetrics {
    const lines = code.split('\n');
    const complexity = this.calculateComplexity(code);
    const readability = this.calculateReadability(code);
    const dependencies = this.countDependencies(code);

    return {
      linesOfCode: lines.length,
      complexityScore: complexity,
      readabilityScore: readability,
      dependencies: dependencies
    };
  }

  /**
   * Generate a GUID
   */
  generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  // ==================== Private Helper Methods ====================

  private calculateComplexity(code: string): number {
    const keywords = ['if', 'for', 'while', 'switch', 'case', 'catch', '&&', '||', '?', ':'];
    return keywords.reduce((count, keyword) => {
      const regex = new RegExp(`\\b${keyword}\\b`, 'g');
      const matches = code.match(regex);
      return count + (matches ? matches.length : 0);
    }, 0);
  }

  private calculateReadability(code: string): number {
    const lines = code.split('\n');
    const avgLineLength = lines.reduce((sum, line) => sum + line.length, 0) / lines.length;
    const longLines = lines.filter(line => line.length > 80).length;
    
    let score = 100;
    if (avgLineLength > 100) score -= 30;
    else if (avgLineLength > 80) score -= 20;
    
    if (longLines / lines.length > 0.2) score -= 20;
    
    return Math.max(0, score);
  }

  private countDependencies(code: string): number {
    const dependencyKeywords = ['import', 'from', 'require', 'using'];
    return dependencyKeywords.reduce((count, keyword) => {
      const regex = new RegExp(`\\b${keyword}\\b`, 'g');
      const matches = code.match(regex);
      return count + (matches ? matches.length : 0);
    }, 0);
  }

  private logPatternInfo(message: string, data: any): void {
    console.log(`[Pattern Service] ${message}:`, data);
  }

  private logValidationResult(result: PatternValidationResult): void {
    console.log('[Pattern Service] Validation result:', {
      isValid: result.isValid,
      errors: result.errors.length,
      warnings: result.warnings.length,
      metrics: result.metrics.length
    });
  }

  private handlePatternError(error: any, context: string): Observable<never> {
    let errorMessage = 'An error occurred during pattern operation';
    let errorDetails: any = null;

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client error: ${error.error.message}`;
    } else if (error.status === 0) {
      // Network error
      errorMessage = 'Network error: Unable to connect to the pattern service';
    } else if (error.status === 400) {
      // Bad request
      errorDetails = error.error;
      errorMessage = `Validation error: ${errorDetails?.message || 'Invalid request'}`;
    } else if (error.status === 404) {
      // Not found
      errorMessage = `Pattern not found: ${error.message}`;
    } else if (error.status === 408 || error.name === 'TimeoutError') {
      // Timeout
      errorMessage = `Pattern operation timed out for ${context}`;
    } else if (error.status === 422) {
      // Unprocessable entity
      errorDetails = error.error;
      errorMessage = `Pattern generation failed: ${errorDetails?.message || 'Unable to generate pattern'}`;
    } else if (error.status >= 500) {
      // Server error
      errorDetails = error.error;
      errorMessage = `Pattern service error: ${errorDetails?.message || 'Something went wrong on the server'}`;
    }

    console.error(`[Pattern Service Error] ${context}:`, {
      message: errorMessage,
      status: error.status,
      error: error,
      details: errorDetails
    });

    return throwError(() => ({
      message: errorMessage,
      details: errorDetails,
      context: context,
      status: error.status,
      timestamp: new Date().toISOString()
    }));
  }
}