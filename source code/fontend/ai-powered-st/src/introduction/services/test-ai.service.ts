import { Injectable } from "@angular/core";
import { catchError, Observable, throwError } from "rxjs";
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AIError, AnalysisRequest, FlakyPrediction, TestCaseRequest, TestCaseResponse, TestRun } from "../models/test-case.models";
import { environment } from "src/environments/environment";


@Injectable({
  providedIn: 'root'
})
export class TestAIService {
  private apiUrl = environment.apiBaseUrl + '/api/testgen';
  
  constructor(private http: HttpClient) {}

  generateTestCase(prompt: string, llmProvider: string = 'chatgpt', urgency: number = 2): Observable<TestCaseResponse> {
    const request: TestCaseRequest = {
      prompt: prompt.trim(),
      llmProvider: llmProvider,
      context: 'Generate a complete, executable test case with assertions.',
      urgency: urgency // Changed from 'standard' to numeric value
    };

    return this.http.post<TestCaseResponse>(this.apiUrl, request).pipe(
      catchError(this.handleAIMisstep)
    );
  }

  checkForFlakyTests(testRunData: TestRun[]): Observable<FlakyPrediction[]> {
    const analysisRequest: AnalysisRequest = {
      testRuns: testRunData,
      analysisType: 'flaky-pattern',
      confidenceThreshold: 0.75
    };
    
    return this.http.post<FlakyPrediction[]>(
      `${this.apiUrl}/analyze/flaky`, 
      analysisRequest
    );
  }

  private handleAIMisstep(error: HttpErrorResponse): Observable<never> {
    let aiError: AIError;
    
    // Check if the error response matches our AIError structure from backend
    if (error.error && typeof error.error === 'object') {
      const backendError = error.error as AIError;
      aiError = {
        errorId: backendError.errorId,
        message: backendError.message || `The AI hit a snag: ${error.message}`,
        suggestion: backendError.suggestion || 'Try rephrasing your prompt or switching LLM providers.',
        recoverable: backendError.recoverable !== undefined ? backendError.recoverable : error.status !== 500,
        provider: backendError.provider || 'unknown'
      };
    } else {
      // Fallback for non-AIError responses
      aiError = {
        message: `The AI hit a snag: ${error.message}`,
        suggestion: 'Try rephrasing your prompt or switching LLM providers.',
        recoverable: error.status !== 500,
        provider: 'unknown'
      };
    }
    
    return throwError(() => aiError);
  }
}

export { TestCaseResponse, AIError, FlakyPrediction, TestRun };
