
// services/landscape-testing.service.ts (update with additional methods)
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, map, timeout } from 'rxjs/operators';
import { environment } from '../../environments/environment';

import {
  LandscapeTestRequest,
  TestLandscapeResponse,
  LandscapeError,
  RiskAssessment,
  UserScale,
  AppProfile
} from '../models/landscape.models'

@Injectable({
  providedIn: 'root'
})
export class LandscapeTestingService {
  private readonly baseUrl = environment.testingApiUrl;
  private readonly defaultTimeout = 30000; // 30 seconds

  constructor(private http: HttpClient) {}

  // generateLandscapeTests(
  //   applicationProfile: AppProfile,
  //   testingFocus: string[],
  //   riskAssessment?: RiskAssessment,
  //   requestedArtifacts: string[] = ['testScenarios', 'automationScripts', 'monitoringSuggestions']
  // ): Observable<TestLandscapeResponse> {
    
  //   // Validate required fields as per backend validation
  //   if (!applicationProfile?.name || applicationProfile.name.length < 2) {
  //     return throwError(() => new Error('Application name must be at least 2 characters long'));
  //   }

  //   if (!testingFocus || testingFocus.length === 0) {
  //     return throwError(() => new Error('Testing focus areas are required'));
  //   }

  //   const request: LandscapeTestRequest = {
  //     applicationProfile: {
  //       ...applicationProfile,
  //       backendServicesCount: applicationProfile.backendServices?.length || 0,
  //       backendServicesCount: Math.max(1, Math.min(1000, applicationProfile.backendServices?.length || 1))
  //     },
  //     testingFocus: testingFocus,
  //     riskAssessment: riskAssessment || this.createDefaultRiskAssessment(applicationProfile),
  //     promptVersion: '1.2',
  //     requestedArtifacts: requestedArtifacts
  //   };

  //   return this.http.post<TestLandscapeResponse>(
  //     `${this.baseUrl}/api/landscape/analyze`,
  //     request
  //   ).pipe(
  //     timeout(this.defaultTimeout),
  //     map(response => ({
  //       ...response,
  //       generatedAt: new Date(response.generatedAt)
  //     })),
  //     catchError((error: HttpErrorResponse) => this.handleLandscapeError(error, applicationProfile))
  //   );
  // }

  // services/landscape-testing.service.ts - Fixed section

generateLandscapeTests(
  applicationProfile: AppProfile,
  testingFocus: string[],
  riskAssessment?: RiskAssessment,
  requestedArtifacts: string[] = ['testScenarios', 'automationScripts', 'monitoringSuggestions']
): Observable<TestLandscapeResponse> {
  
  // Validate required fields as per backend validation
  if (!applicationProfile?.name || applicationProfile.name.length < 2) {
    return throwError(() => new Error('Application name must be at least 2 characters long'));
  }

  if (!testingFocus || testingFocus.length === 0) {
    return throwError(() => new Error('Testing focus areas are required'));
  }

  // Calculate backend services count once
  const servicesCount = applicationProfile.backendServices?.length || 1;
  const validatedServicesCount = Math.max(1, Math.min(1000, servicesCount));

  const request: LandscapeTestRequest = {
    applicationProfile: {
      ...applicationProfile,
      backendServicesCount: validatedServicesCount,  // Set it once
      // Remove the duplicate line below
    },
    testingFocus: testingFocus,
    riskAssessment: riskAssessment || this.createDefaultRiskAssessment(applicationProfile),
    promptVersion: '1.2',
    requestedArtifacts: requestedArtifacts
  };

  return this.http.post<TestLandscapeResponse>(
    `${this.baseUrl}/api/landscape/analyze`,
    request
  ).pipe(
    timeout(this.defaultTimeout),
    map(response => ({
      ...response,
      generatedAt: new Date(response.generatedAt)
    })),
    catchError((error: HttpErrorResponse) => this.handleLandscapeError(error, applicationProfile))
  );
}

  // 2. GET /api/landscape/analysis/{id} - Get analysis by ID
  getLandscapeAnalysis(analysisId: string): Observable<TestLandscapeResponse> {
    if (!analysisId) {
      return throwError(() => new Error('Analysis ID is required'));
    }

    return this.http.get<TestLandscapeResponse>(
      `${this.baseUrl}/api/landscape/analysis/${analysisId}`
    ).pipe(
      timeout(this.defaultTimeout),
      map(response => ({
        ...response,
        generatedAt: new Date(response.generatedAt)
      })),
      catchError(error => this.handleError(error, { name: 'Unknown' } as AppProfile))
    );
  }

  // 3. GET /api/landscape/health - Health check
  healthCheck(): Observable<any> {
    return this.http.get(`${this.baseUrl}/api/landscape/health`).pipe(
      timeout(5000), // Shorter timeout for health check
      catchError(error => {
        console.error('Health check failed:', error);
        return of({ status: 'unhealthy', error: error.message });
      })
    );
  }

  // 4. Custom analysis request with additional parameters
  customAnalysisRequest(customRequest: any): Observable<TestLandscapeResponse> {
    return this.http.post<TestLandscapeResponse>(
      `${this.baseUrl}/api/landscape/custom-analyze`,
      customRequest
    ).pipe(
      timeout(this.defaultTimeout),
      map(response => ({
        ...response,
        generatedAt: new Date(response.generatedAt)
      })),
      catchError(error => this.handleError(error, customRequest.applicationProfile))
    );
  }

  // 5. GET /api/landscape/analyses - List all analyses (if endpoint exists)
  getAllAnalyses(params?: { page?: number; limit?: number; }): Observable<TestLandscapeResponse[]> {
    let httpParams = new HttpParams();
    if (params?.page) httpParams = httpParams.set('page', params.page.toString());
    if (params?.limit) httpParams = httpParams.set('limit', params.limit.toString());

    return this.http.get<TestLandscapeResponse[]>(
      `${this.baseUrl}/api/landscape/analyses`,
      { params: httpParams }
    ).pipe(
      timeout(this.defaultTimeout),
      map(responses => responses.map(r => ({ ...r, generatedAt: new Date(r.generatedAt) }))),
      catchError(error => this.handleError(error, { name: 'List' } as AppProfile))
    );
  }

  // 6. DELETE /api/landscape/analysis/{id} - Delete analysis (if endpoint exists)
  deleteAnalysis(analysisId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}/api/landscape/analysis/${analysisId}`
    ).pipe(
      timeout(this.defaultTimeout),
      catchError(error => this.handleError(error, { name: 'Delete' } as AppProfile))
    );
  }

  private createDefaultRiskAssessment(profile: AppProfile): RiskAssessment {
    return {
      criticality: profile.expectedUsers === UserScale.Enterprise ? 8 : 5,
      complianceRequirements: [],
      dataSensitivity: profile.dataSources?.includes('database') ? ['user-data'] : [],
      riskFactors: [
        {
          area: 'integration',
          likelihood: profile.backendServices?.length > 3 ? 8 : 4,
          impact: 7,
          description: 'Multiple backend services integration'
        }
      ]
    };
  }

  private handleLandscapeError(error: HttpErrorResponse, profile: AppProfile): Observable<never> {
    let landscapeError: LandscapeError;
    
    if (error.status === 400) {
      // Validation error from backend
      const validationErrors = error.error?.context?.validationErrors || {};
      landscapeError = {
        errorCode: 'VALIDATION_FAILED',
        message: error.error?.message || 'Invalid request parameters',
        recoverySteps: error.error?.recoverySteps || [
          'Check required fields',
          'Verify architecture details',
          'Ensure testing focus areas are specified'
        ],
        fallbackSuggestion: this.generateFallbackStrategy(profile),
        timestamp: new Date(),
        context: { validationErrors },
        severity: 'error'
      };
    } else if (error.status === 422) {
      // Architecture analysis failed
      landscapeError = {
        errorCode: 'ARCHITECTURE_UNPROCESSABLE',
        message: error.error?.message || `Couldn't analyze ${profile.name}'s architecture`,
        recoverySteps: error.error?.recoverySteps || [
          'Simplify the architecture description',
          'Focus on one component at a time',
          'Provide more details about integration points'
        ],
        fallbackSuggestion: error.error?.fallbackSuggestion || this.generateFallbackStrategy(profile),
        timestamp: new Date(),
        severity: 'error'
      };
    } else if (error.status === 503) {
      // LLM services unavailable
      landscapeError = {
        errorCode: 'LLM_UNAVAILABLE',
        message: error.error?.message || 'AI analysis services are currently unavailable',
        recoverySteps: error.error?.recoverySteps || [
          'Try again in a few minutes',
          'Use manual analysis mode',
          'Check service status dashboard'
        ],
        fallbackSuggestion: error.error?.fallbackSuggestion || 'Focus on smoke tests for critical paths only',
        timestamp: new Date(),
        severity: 'warning'
      };
    } else {
      // Generic error
      landscapeError = {
        errorCode: `LANDSCAPE_${error.status || 'ERROR'}`,
        message: error.error?.message || `Failed to analyze ${profile.name}'s testing landscape`,
        recoverySteps: error.error?.recoverySteps || [
          'Try analyzing individual components first',
          'Check if API dependencies are available',
          'Simplify the architecture description'
        ],
        fallbackSuggestion: this.generateFallbackStrategy(profile),
        timestamp: new Date(),
        context: {
          status: error.status,
          statusText: error.statusText,
          url: error.url
        },
        severity: 'error'
      };
    }
    
    return throwError(() => landscapeError);
  }

  private handleError(error: HttpErrorResponse, profile: AppProfile): Observable<never> {
    console.error('API Error:', error);
    return this.handleLandscapeError(error, profile);
  }

  private generateFallbackStrategy(profile: AppProfile): string {
    return `Basic testing strategy for ${profile.name}:
1. Smoke tests for critical user journeys: ${profile.criticalUserJourneys?.slice(0, 3).join(', ')}
2. API integration tests for ${profile.backendServices?.length || 0} services
3. UI tests for ${profile.frontendFrameworks?.join(', ') || 'frontend'}`;
  }
}