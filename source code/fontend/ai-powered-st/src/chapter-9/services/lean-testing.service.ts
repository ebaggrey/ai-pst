// services/lean-testing.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry, timeout } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { AutomationDecisionRequest, AutomationDecisionResponse, CoverageRequest, CoverageResponse, LeanErrorResponse, MaintenanceOptimizationResponse, MaintenanceRequest, PriorityRequest, ROIAnalysisResponse, ROIRequest, TestingPriorityResponse } from '../models/lean-testing-models';

// Import all models


@Injectable({
  providedIn: 'root'
})
export class LeanTestingService {
  private apiUrl = `${environment.leanTestingAPiUrl}/api/lean-testing`;
  private defaultTimeout = 30000; // 30 seconds

  constructor(private http: HttpClient) {}

  /**
   * Prioritize testing effort based on features and constraints
   * @param request PriorityRequest
   * @returns Observable<TestingPriorityResponse>
   */
  prioritizeTestingEffort(request: PriorityRequest): Observable<TestingPriorityResponse> {
    const headers = this.getHeaders();
    
    return this.http.post<TestingPriorityResponse>(
      `${this.apiUrl}/prioritize`,
      request,
      { headers }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(1),
      catchError(this.handleError)
    );
  }

  /**
   * Generate minimal test coverage for a feature
   * @param request CoverageRequest
   * @returns Observable<CoverageResponse>
   */
  generateMinimalTestCoverage(request: CoverageRequest): Observable<CoverageResponse> {
    const headers = this.getHeaders();
    
    return this.http.post<CoverageResponse>(
      `${this.apiUrl}/minimal-coverage`,
      request,
      { headers }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(1),
      catchError(this.handleError)
    );
  }

  /**
   * Decide whether to automate a test scenario
   * @param request AutomationDecisionRequest
   * @returns Observable<AutomationDecisionResponse>
   */
  decideAutomationThreshold(request: AutomationDecisionRequest): Observable<AutomationDecisionResponse> {
    const headers = this.getHeaders();
    
    return this.http.post<AutomationDecisionResponse>(
      `${this.apiUrl}/automation-threshold`,
      request,
      { headers }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(1),
      catchError(this.handleError)
    );
  }

  /**
   * Optimize test maintenance
   * @param request MaintenanceRequest
   * @returns Observable<MaintenanceOptimizationResponse>
   */
  optimizeTestMaintenance(request: MaintenanceRequest): Observable<MaintenanceOptimizationResponse> {
    const headers = this.getHeaders();
    
    return this.http.post<MaintenanceOptimizationResponse>(
      `${this.apiUrl}/optimize-maintenance`,
      request,
      { headers }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(1),
      catchError(this.handleError)
    );
  }

  /**
   * Measure testing ROI
   * @param request ROIRequest
   * @returns Observable<ROIAnalysisResponse>
   */
  measureTestingROI(request: ROIRequest): Observable<ROIAnalysisResponse> {
    const headers = this.getHeaders();
    
    return this.http.post<ROIAnalysisResponse>(
      `${this.apiUrl}/measure-roi`,
      request,
      { headers }
    ).pipe(
      timeout(this.defaultTimeout),
      retry(1),
      catchError(this.handleError)
    );
  }

  /**
   * Get default headers for API requests
   * @returns HttpHeaders
   */
  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    });
  }

  /**
   * Handle HTTP errors
   * @param error HttpErrorResponse
   * @returns Observable<never>
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An error occurred';
    let leanError: LeanErrorResponse | undefined;

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client Error: ${error.error.message}`;
      console.error('Client error:', error.error);
    } else {
      // Server-side error
      try {
        // Try to parse as LeanErrorResponse
        leanError = error.error as LeanErrorResponse;
        errorMessage = leanError.message || `Server Error: ${error.status} - ${error.message}`;
      } catch {
        errorMessage = `Server Error: ${error.status} - ${error.message}`;
      }
      
      console.error('Server error:', {
        status: error.status,
        message: error.message,
        body: error.error
      });
    }

    // Return observable with error
    return throwError(() => ({
      message: errorMessage,
      status: error.status,
      leanError: leanError,
      originalError: error
    }));
  }
}