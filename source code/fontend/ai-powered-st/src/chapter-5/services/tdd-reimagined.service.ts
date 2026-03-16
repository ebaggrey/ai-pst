// services/tdd-reimagined.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { FuturePredictionRequest, FuturePredictionResponse, ImplementationRequest, ImplementationResponse, RefactorRequest, RefactorResponse, TDDCycleResponse, TDDErrorResponse, TDDRequest } from '../models/tdd-models';

@Injectable({
  providedIn: 'root'
})
export class TDDReimaginedService {
  private apiUrl = 'https://localhost:5001/api/tdd-reimagined';

  constructor(private http: HttpClient) { }

  // Generate test-first implementation
  generateTestFirst(request: TDDRequest): Observable<TDDCycleResponse> {
    return this.http.post<TDDCycleResponse>(`${this.apiUrl}/generate-test-first`, request)
      .pipe(
        catchError(this.handleError<TDDCycleResponse>('generateTestFirst'))
      );
  }

  // Implement from failing test
  implementFromFailingTest(request: ImplementationRequest): Observable<ImplementationResponse> {
    return this.http.post<ImplementationResponse>(`${this.apiUrl}/implement-from-failing-test`, request)
      .pipe(
        catchError(this.handleError<ImplementationResponse>('implementFromFailingTest'))
      );
  }

  // Refactor with confidence
  refactorWithConfidence(request: RefactorRequest): Observable<RefactorResponse> {
    return this.http.post<RefactorResponse>(`${this.apiUrl}/refactor-with-confidence`, request)
      .pipe(
        catchError(this.handleError<RefactorResponse>('refactorWithConfidence'))
      );
  }

  // Predict future tests
  predictFutureTests(request: FuturePredictionRequest): Observable<FuturePredictionResponse> {
    return this.http.post<FuturePredictionResponse>(`${this.apiUrl}/predict-future-tests`, request)
      .pipe(
        catchError(this.handleError<FuturePredictionResponse>('predictFutureTests'))
      );
  }

  // Health check
  checkHealth(): Observable<{ status: string; timestamp: string }> {
    return this.http.get<{ status: string; timestamp: string }>('https://localhost:5001/health')
      .pipe(
        catchError(this.handleError<{ status: string; timestamp: string }>('checkHealth'))
      );
  }

  // Error handling
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
      
      // Create a TDDErrorResponse from the error if available
      if (error.error) {
        const tddError: TDDErrorResponse = {
          phase: error.error.phase || 'unknown',
          errorType: error.error.errorType || 'unknown-error',
          message: error.error.message || error.message,
          recoveryStrategy: error.error.recoveryStrategy || ['Check console for details'],
          suggestedFallback: error.error.suggestedFallback || 'Retry the operation',
          learningOpportunity: error.error.learningOpportunity
        };
        
        // You could also emit this to a separate error stream
        console.error('TDD Error Response:', tddError);
      }
      
      // Let the app keep running by returning an empty result
      return of(result as T);
    };
  }
}