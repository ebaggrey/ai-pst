// services/full-spectrum.service.ts (continued)
import { Injectable } from '@angular/core';
import { FullSpectrumModels, FullSpectrumModels as Model } from '../models/requests_reponses/full-spectrum.models';
import { catchError, Observable, retry, throwError, timeout } from 'rxjs';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class FullSpectrumService {
  // ... previous code ...

   private readonly baseUrl = `${environment.fullSpectrumAPIUrl}/api/full-spectrum`;
  private readonly defaultTimeout = 30000; // 30 seconds

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
      // Add auth headers if needed
      // 'Authorization': `Bearer ${this.authService.getToken()}`
    });
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorResponse: FullSpectrumModels.SpectrumErrorResponse;
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      console.error('Client error:', error.error.message);
      errorResponse = {
        context: 'client',
        errorType: 'client-error',
        spectrumLocation: 'unknown',
        message: error.error.message,
        recoverySteps: ['Check your internet connection', 'Try again later'],
        fallbackSuggestion: 'Please try again'
      };
    } else {
      // Server-side error
      console.error(`Server error: ${error.status} - ${error.message}`);
      errorResponse = error.error as FullSpectrumModels.SpectrumErrorResponse || {
        context: 'server',
        errorType: `http-${error.status}`,
        spectrumLocation: 'unknown',
        message: error.message,
        recoverySteps: ['Please try again', 'Contact support if issue persists'],
        fallbackSuggestion: 'Manual operation'
      };
    }
    
    return throwError(() => errorResponse);
  }

  private getOptions() {
    return {
      headers: this.getHeaders(),
      timeout: this.defaultTimeout
    };
  }

  /**
   * Generate shift-left tests from requirements
   * POST /api/full-spectrum/shift-left
   */
  generateShiftLeftTests(request: Model.ShiftLeftRequest): Observable<Model.ShiftLeftResponse> {
    return this.http.post<Model.ShiftLeftResponse>(
      `${this.baseUrl}/shift-left`,
      request,
      this.getOptions()
    ).pipe(
      retry(1),
      timeout(this.defaultTimeout),
      catchError(this.handleError)
    );
  }

  /**
   * Analyze code for testability
   * POST /api/full-spectrum/analyze-testability
   */
  analyzeTestability(request: Model.TestabilityRequest): Observable<Model.TestabilityResponse> {
    return this.http.post<Model.TestabilityResponse>(
      `${this.baseUrl}/analyze-testability`,
      request,
      this.getOptions()
    ).pipe(
      retry(1),
      timeout(this.defaultTimeout),
      catchError(this.handleError)
    );
  }

  /**
   * Generate shift-right monitors
   * POST /api/full-spectrum/shift-right
   */
  generateShiftRightMonitors(request: Model.ShiftRightRequest): Observable<Model.ShiftRightResponse> {
    return this.http.post<Model.ShiftRightResponse>(
      `${this.baseUrl}/shift-right`,
      request,
      this.getOptions()
    ).pipe(
      retry(1),
      timeout(this.defaultTimeout),
      catchError(this.handleError)
    );
  }

  /**
   * Create spectrum pipeline
   * POST /api/full-spectrum/create-pipeline
   */
  createSpectrumPipeline(request: Model.PipelineRequest): Observable<Model.PipelineResponse> {
    return this.http.post<Model.PipelineResponse>(
      `${this.baseUrl}/create-pipeline`,
      request,
      this.getOptions()
    ).pipe(
      retry(1),
      timeout(this.defaultTimeout),
      catchError(this.handleError)
    );
  }

  /**
   * Orchestrate cross-spectrum testing
   * POST /api/full-spectrum/orchestrate-testing
   */
  orchestrateTesting(request: Model.OrchestrationRequest): Observable<Model.OrchestrationResponse> {
    return this.http.post<Model.OrchestrationResponse>(
      `${this.baseUrl}/orchestrate-testing`,
      request,
      this.getOptions()
    ).pipe(
      retry(1),
      timeout(this.defaultTimeout),
      catchError(this.handleError)
    );
  }
}