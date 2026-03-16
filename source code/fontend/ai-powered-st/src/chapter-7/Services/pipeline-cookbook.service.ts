// src/app/services/pipeline-cookbook.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, retry, timeout, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { AdaptationRequest, DiagnosisRequest, OptimizationRequest, PipelineGenerationRequest, PredictionRequest } from '../models/request/requests';
import { AdaptationResponse, DiagnosisResponse, IntelligentPipelineResponse, OptimizationResponse, PredictionResponse } from '../models/responses/responses';
import { PipelineErrorResponse } from '../models/errors/errors';



@Injectable({
    providedIn: 'root'
})
export class PipelineCookbookService {
    private readonly baseUrl = `${environment.pipelineCookbookApiUrl}/api/pipeline-cookbook`;
    private readonly defaultTimeout = 30000; // 30 seconds
    private readonly maxRetries = 2;

    constructor(private http: HttpClient) {}

    /**
     * Generate an intelligent pipeline based on codebase analysis and requirements
     * @param request Pipeline generation request
     * @returns Observable with intelligent pipeline response
     */
    generateIntelligentPipeline(request: PipelineGenerationRequest): Observable<IntelligentPipelineResponse> {
        const headers = this.createHeaders();
        
        return this.http.post<IntelligentPipelineResponse>(
            `${this.baseUrl}/generate-pipeline`,
            request,
            { headers }
        ).pipe(
            timeout(this.defaultTimeout),
            retry(this.maxRetries),
            catchError(this.handleError<IntelligentPipelineResponse>('generateIntelligentPipeline'))
        );
    }

    /**
     * Diagnose pipeline failure from logs and context
     * @param request Diagnosis request with failure logs
     * @returns Observable with diagnosis response
     */
    diagnosePipelineFailure(request: DiagnosisRequest): Observable<DiagnosisResponse> {
        const headers = this.createHeaders();
        
        return this.http.post<DiagnosisResponse>(
            `${this.baseUrl}/diagnose-failure`,
            request,
            { headers }
        ).pipe(
            timeout(this.defaultTimeout),
            retry(this.maxRetries),
            catchError(this.handleError<DiagnosisResponse>('diagnosePipelineFailure'))
        );
    }

    /**
     * Optimize pipeline performance based on metrics and goals
     * @param request Optimization request with current metrics and goals
     * @returns Observable with optimization response
     */
    optimizePipelinePerformance(request: OptimizationRequest): Observable<OptimizationResponse> {
        const headers = this.createHeaders();
        
        return this.http.post<OptimizationResponse>(
            `${this.baseUrl}/optimize-performance`,
            request,
            { headers }
        ).pipe(
            timeout(this.defaultTimeout),
            retry(this.maxRetries),
            catchError(this.handleError<OptimizationResponse>('optimizePipelinePerformance'))
        );
    }

    /**
     * Predict potential pipeline issues based on proposed changes
     * @param request Prediction request with proposed changes
     * @returns Observable with prediction response
     */
    predictPipelineIssues(request: PredictionRequest): Observable<PredictionResponse> {
        const headers = this.createHeaders();
        
        return this.http.post<PredictionResponse>(
            `${this.baseUrl}/predict-issues`,
            request,
            { headers }
        ).pipe(
            timeout(this.defaultTimeout),
            retry(this.maxRetries),
            catchError(this.handleError<PredictionResponse>('predictPipelineIssues'))
        );
    }

    /**
     * Adapt pipeline to accommodate changes
     * @param request Adaptation request with change details
     * @returns Observable with adaptation response
     */
    adaptPipelineToChange(request: AdaptationRequest): Observable<AdaptationResponse> {
        const headers = this.createHeaders();
        
        return this.http.post<AdaptationResponse>(
            `${this.baseUrl}/adapt-to-change`,
            request,
            { headers }
        ).pipe(
            timeout(this.defaultTimeout),
            retry(this.maxRetries),
            catchError(this.handleError<AdaptationResponse>('adaptPipelineToChange'))
        );
    }

    /**
     * Create HTTP headers with content type
     * @returns HttpHeaders object
     */
    private createHeaders(): HttpHeaders {
        return new HttpHeaders({
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        });
    }

    /**
     * Handle HTTP errors and transform to user-friendly format
     * @param operation Name of the operation that failed
     * @returns Function to handle the error
     */
    private handleError<T>(operation = 'operation') {
        return (error: HttpErrorResponse): Observable<T> => {
            let errorMessage = 'An error occurred';
            let pipelineError: PipelineErrorResponse | null = null;

            if (error.error instanceof ErrorEvent) {
                // Client-side error
                errorMessage = `Client Error: ${error.error.message}`;
                console.error(`${operation} failed:`, errorMessage);
            } else {
                // Server-side error
                try {
                    // Try to parse as PipelineErrorResponse
                    pipelineError = error.error as PipelineErrorResponse;
                    errorMessage = pipelineError.message || `Server Error: ${error.status}`;
                } catch {
                    errorMessage = `Server Error: ${error.status} - ${error.statusText}`;
                }
                
                console.error(
                    `${operation} failed: Backend returned code ${error.status}, ` +
                    `body was:`, error.error
                );
            }

            // Create a structured error object
            const enhancedError = {
                status: error.status,
                statusText: error.statusText,
                message: errorMessage,
                pipelineError: pipelineError,
                timestamp: new Date().toISOString(),
                operation: operation
            };

            return throwError(() => enhancedError);
        };
    }

    /**
     * Check if the API is healthy
     * @returns Observable with health status
     */
    healthCheck(): Observable<{ status: string }> {
        return this.http.get<{ status: string }>(`${this.baseUrl.replace('/api/pipeline-cookbook', '')}/health`)
            .pipe(
                timeout(5000),
                catchError(() => of({ status: 'unhealthy' }))
            );
    }
}