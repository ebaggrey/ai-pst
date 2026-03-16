// src/app/services/data-bias-audit.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, map, retry, timeout } from 'rxjs/operators';
import { TestDataBiasAuditRequest } from '../models/requests/test-data-bias-audit-request.model';
import { BiasAuditResponse } from '../models/responses/bias-audit-response.model';
import { HealthCheckDetailedResponse, HealthCheckResponse } from '../models/responses/health-check-detailed-response.model';
import { AuditStatistics, BiasAuditFilter, BiasAuditSummary } from '../models/supporting/bias-audit-summary.model';
import { BiasAuditErrorResponse, CategorizedError, ErrorCategory, ErrorSeverity } from '../models/responses/bias-audit-error-response.model';

@Injectable({
  providedIn: 'root'
})
export class DataBiasAuditService {
  private apiUrl = 'https://localhost:5001/api'; // Base API URL
  private readonly DEFAULT_TIMEOUT = 30000; // 30 seconds
  private readonly MAX_RETRIES = 2;

  constructor(private http: HttpClient) { }

  /**
   * Get HTTP headers with proper content type
   */
  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    });
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorResponse: BiasAuditErrorResponse = {
      message: 'An unknown error occurred',
      errorType: 'UnknownError',
      suggestedRemediation: 'Please try again later',
      errorId: this.generateErrorId(),
      timestamp: new Date()
    };

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorResponse.message = `Client Error: ${error.error.message}`;
      errorResponse.errorType = 'ClientError';
      errorResponse.suggestedRemediation = 'Please check your network connection';
    } else {
      // Server-side error
      if (error.error && error.error.errorType) {
        // Our API error format
        errorResponse = error.error as BiasAuditErrorResponse;
      } else {
        errorResponse.message = `Server Error: ${error.status} - ${error.statusText}`;
        errorResponse.errorType = `HttpError${error.status}`;
        errorResponse.suggestedRemediation = this.getRemediationForStatus(error.status);
      }
    }

    console.error('API Error:', errorResponse);
    return throwError(() => errorResponse);
  }

  /**
   * Generate a unique error ID for tracking
   */
  private generateErrorId(): string {
    return 'ERR-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
  }

  /**
   * Get remediation suggestion based on HTTP status
   */
  private getRemediationForStatus(status: number): string {
    switch (status) {
      case 400: return 'Please check your request data and try again';
      case 401: return 'Please log in again';
      case 403: return 'You do not have permission to perform this action';
      case 404: return 'The requested resource was not found';
      case 408: return 'The request timed out. Please try again';
      case 429: return 'Too many requests. Please wait a moment and try again';
      case 500: return 'Internal server error. Please try again later';
      case 503: return 'Service unavailable. Please try again later';
      default: return 'Please try again later or contact support';
    }
  }

  /**
   * Perform bias audit on test data
   * POST /api/BiasAudit/audit-data
   */
  auditTestData(request: TestDataBiasAuditRequest): Observable<BiasAuditResponse> {
    return this.http.post<BiasAuditResponse>(
      `${this.apiUrl}/BiasAudit/audit-data`,
      request,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.DEFAULT_TIMEOUT),
      retry(this.MAX_RETRIES),
      map(response => {
        // Convert date strings to Date objects
        response.auditDate = new Date(response.auditDate);
        return response;
      }),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Check controller health
   * GET /api/BiasAudit/health
   */
  checkHealth(): Observable<HealthCheckResponse> {
    return this.http.get<HealthCheckResponse>(
      `${this.apiUrl}/BiasAudit/health`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(5000),
      map(response => {
        response.timestamp = new Date(response.timestamp);
        return response;
      }),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Check application liveness
   * GET /health/live
   */
  checkLiveness(): Observable<{ status: string }> {
    return this.http.get<{ status: string }>(
      `https://localhost:5001/health/live`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(3000),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Check application readiness with detailed status
   * GET /health/ready
   */
checkReadiness(): Observable<HealthCheckDetailedResponse> {
  return this.http.get<HealthCheckDetailedResponse>(
    `https://localhost:5001/health/ready`,
    { headers: this.getHeaders() }
  ).pipe(
    timeout(5000),
    catchError(this.handleError.bind(this))
  );
}

  /**
   * Simple ping health check
   * GET /health/ping
   */
  ping(): Observable<{ status: string; timestamp: Date }> {
    return this.http.get<{ status: string; timestamp: string }>(
      `https://localhost:5001/health/ping`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(3000),
      map(response => ({
        ...response,
        timestamp: new Date(response.timestamp)
      })),
      catchError(this.handleError.bind(this))
    );
  }

private categorizeError(error: any): CategorizedError {
  const baseError: BiasAuditErrorResponse = {
    message: error.message || 'An unknown error occurred',
    errorType: error.errorType || 'UnknownError',
    suggestedRemediation: error.suggestedRemediation || 'Please try again later',
    errorId: error.errorId || this.generateErrorId(),
    timestamp: error.timestamp ? new Date(error.timestamp) : new Date()
  };

  let category: ErrorCategory;
  let severity: ErrorSeverity;
  let retryable: boolean;
  let userAction: string;

  // Categorize based on error type
  switch (baseError.errorType) {
    case 'AIServiceUnavailable':
    case 'AIServiceError':
      category = ErrorCategory.ServiceUnavailable;
      severity = ErrorSeverity.High;
      retryable = true;
      userAction = 'Wait a few moments and try again';
      break;

    case 'DataFormatError':
    case 'InvalidData':
    case 'MissingRequest':
    case 'MissingDatasetName':
    case 'EmptyDataSample':
    case 'InvalidSuggestionCount':
      category = ErrorCategory.Validation;
      severity = ErrorSeverity.Medium;
      retryable = false;
      userAction = 'Check your input data and try again';
      break;

    case 'Unauthorized':
      category = ErrorCategory.Authentication;
      severity = ErrorSeverity.High;
      retryable = false;
      userAction = 'Please log in again';
      break;

    case 'HttpError404':
      category = ErrorCategory.NotFound;
      severity = ErrorSeverity.Low;
      retryable = false;
      userAction = 'The requested resource was not found';
      break;

    case 'HttpError408':
      category = ErrorCategory.Timeout;
      severity = ErrorSeverity.Medium;
      retryable = true;
      userAction = 'The request timed out. Please try again';
      break;

    case 'HttpError500':
    case 'InternalServerError':
      category = ErrorCategory.InternalServer;
      severity = ErrorSeverity.Critical;
      retryable = true;
      userAction = 'System error. Please try again later or contact support';
      break;

    default:
      category = ErrorCategory.Unknown;
      severity = ErrorSeverity.Medium;
      retryable = false;
      userAction = 'Please try again or contact support';
  }

  return {
    ...baseError,
    category,
    severity,
    retryable,
    userAction
  };
}


  /**
   * Get audit summaries with optional filtering
   */
  getAuditSummaries(filter?: BiasAuditFilter): Observable<BiasAuditSummary[]> {
    // This would need a backend endpoint - here's an example implementation
    let url = `${this.apiUrl}/BiasAudit/summaries`;
    if (filter) {
      const params = new URLSearchParams();
      if (filter.datasetName) params.set('datasetName', filter.datasetName);
      if (filter.fromDate) params.set('fromDate', filter.fromDate.toISOString());
      if (filter.toDate) params.set('toDate', filter.toDate.toISOString());
      if (filter.riskLevel) params.set('riskLevel', filter.riskLevel);
      if (filter.minScore) params.set('minScore', filter.minScore.toString());
      if (filter.maxScore) params.set('maxScore', filter.maxScore.toString());
      
      if (params.toString()) {
        url += `?${params.toString()}`;
      }
    }
    
    return this.http.get<BiasAuditSummary[]>(url, { headers: this.getHeaders() }).pipe(
      timeout(this.DEFAULT_TIMEOUT),
      map(summaries => summaries.map(s => ({
        ...s,
        auditDate: new Date(s.auditDate)
      }))),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Get audit statistics
   */
  getAuditStatistics(fromDate?: Date, toDate?: Date): Observable<AuditStatistics> {
    let url = `${this.apiUrl}/BiasAudit/statistics`;
    const params = new URLSearchParams();
    if (fromDate) params.set('fromDate', fromDate.toISOString());
    if (toDate) params.set('toDate', toDate.toISOString());
    
    if (params.toString()) {
      url += `?${params.toString()}`;
    }
    
    return this.http.get<AuditStatistics>(url, { headers: this.getHeaders() }).pipe(
      timeout(this.DEFAULT_TIMEOUT),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Get a specific audit by ID
   */
  getAuditById(auditId: string): Observable<BiasAuditResponse> {
    return this.http.get<BiasAuditResponse>(
      `${this.apiUrl}/BiasAudit/${auditId}`,
      { headers: this.getHeaders() }
    ).pipe(
      timeout(this.DEFAULT_TIMEOUT),
      map(response => ({
        ...response,
        auditDate: new Date(response.auditDate)
      })),
      catchError(this.handleError.bind(this))
    );
  }
}