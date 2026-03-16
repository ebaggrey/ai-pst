// services/legacy-conquest.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry, timeout } from 'rxjs/operators';
import { environment } from '../../environments/environment';

import {
    LegacyAnalysisRequest,
    LegacyAnalysisResponse,
    WrapperRequest,
    WrapperGenerationResponse,
    CharacterizationRequest,
    CharacterizationResponse,
    RoadmapRequest,
    ModernizationResponse,
    HealthRequest,
    HealthResponse,
    LegacyErrorResponse
} from '../models/legacy-conquest.models';

@Injectable({
    providedIn: 'root'
})
export class LegacyConquestService {
    private readonly baseUrl = `${environment.legacyConquestAPiUrl}/api/legacy-conquest`;
    private readonly defaultTimeout = 30000; // 30 seconds

    constructor(private http: HttpClient) {}

    /**
     * Analyze legacy codebase
     * POST /api/legacy-conquest/analyze
     */
    analyzeLegacyCodebase(request: LegacyAnalysisRequest): Observable<LegacyAnalysisResponse> {
        return this.http.post<LegacyAnalysisResponse>(`${this.baseUrl}/analyze`, request)
            .pipe(
                timeout(this.defaultTimeout),
                retry(1),
                catchError(this.handleError)
            );
    }

    /**
     * Generate safe wrappers for legacy module
     * POST /api/legacy-conquest/generate-wrappers
     */
    generateSafeWrappers(request: WrapperRequest): Observable<WrapperGenerationResponse> {
        return this.http.post<WrapperGenerationResponse>(`${this.baseUrl}/generate-wrappers`, request)
            .pipe(
                timeout(this.defaultTimeout),
                retry(1),
                catchError(this.handleError)
            );
    }

    /**
     * Create characterization tests
     * POST /api/legacy-conquest/create-characterization-tests
     */
    createCharacterizationTests(request: CharacterizationRequest): Observable<CharacterizationResponse> {
        return this.http.post<CharacterizationResponse>(`${this.baseUrl}/create-characterization-tests`, request)
            .pipe(
                timeout(this.defaultTimeout),
                retry(1),
                catchError(this.handleError)
            );
    }

    /**
     * Plan incremental modernization
     * POST /api/legacy-conquest/plan-modernization
     */
    planIncrementalModernization(request: RoadmapRequest): Observable<ModernizationResponse> {
        return this.http.post<ModernizationResponse>(`${this.baseUrl}/plan-modernization`, request)
            .pipe(
                timeout(this.defaultTimeout),
                retry(1),
                catchError(this.handleError)
            );
    }

    /**
     * Monitor legacy health
     * POST /api/legacy-conquest/monitor-health
     */
    monitorLegacyHealth(request: HealthRequest): Observable<HealthResponse> {
        return this.http.post<HealthResponse>(`${this.baseUrl}/monitor-health`, request)
            .pipe(
                timeout(this.defaultTimeout),
                retry(1),
                catchError(this.handleError)
            );
    }

    /**
     * Error handler
     */
    private handleError(error: HttpErrorResponse) {
        let errorMessage = 'An error occurred';

        if (error.error instanceof ErrorEvent) {
            // Client-side error
            errorMessage = `Client Error: ${error.error.message}`;
        } else {
            // Server-side error
            if (error.error && error.error.message) {
                // Our API error format
                errorMessage = error.error.message;
                
                // Log diagnostic info if available
                if (error.error.diagnosticData) {
                    console.error('Diagnostic Data:', error.error.diagnosticData);
                }
            } else {
                errorMessage = `Server Error: ${error.status} - ${error.message}`;
            }
        }

        console.error('Legacy Conquest API Error:', errorMessage);
        return throwError(() => ({
            error: error.error as LegacyErrorResponse,
            message: errorMessage,
            status: error.status
        }));
    }

    /**
     * Helper method to create headers with authentication if needed
     */
    private getHeaders(): HttpHeaders {
        let headers = new HttpHeaders({
            'Content-Type': 'application/json'
        });

        // Add auth token if available
        const token = localStorage.getItem('auth_token');
        if (token) {
            headers = headers.set('Authorization', `Bearer ${token}`);
        }

        return headers;
    }
}