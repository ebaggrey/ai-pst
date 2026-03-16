// services/metrics-that-matter.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

// Import all request models
import { MetricDesignRequest } from '../models/requests/request-models';
import { HealthScoreRequest } from '../models/requests/request-models';
import { PredictionRequest } from '../models/requests/request-models';
import { InsightRequest } from '../models/requests/request-models';
import { OptimizationRequest } from '../models/requests/request-models';

// Import all response models
import { MetricDesignResponse } from '../models/responses/response-models';
import { HealthScoreResponse } from '../models/responses/response-models';
import { PredictionResponse } from '../models/responses/response-models';
import { InsightResponse } from '../models/responses/response-models';
import { OptimizationResponse } from '../models/responses/response-models';
import { MetricErrorResponse } from '../models/responses/response-models';

@Injectable({
    providedIn: 'root'
})
export class MetricsThatMatterService {
    private readonly baseUrl = `${environment.metricsThatMatterAPIUrl}/api/metrics-that-matter`;

    constructor(private http: HttpClient) {}

    /**
     * Design impactful metrics based on business objectives
     * @param request The metric design request
     * @returns Observable of MetricDesignResponse
     */
    designImpactfulMetrics(request: MetricDesignRequest): Observable<MetricDesignResponse> {
        return this.http.post<MetricDesignResponse>(
            `${this.baseUrl}/design-metrics`, 
            request
        );
    }

    /**
     * Calculate testing health score from metrics
     * @param request The health score request
     * @returns Observable of HealthScoreResponse
     */
    calculateTestingHealthScore(request: HealthScoreRequest): Observable<HealthScoreResponse> {
        return this.http.post<HealthScoreResponse>(
            `${this.baseUrl}/calculate-health`, 
            request
        );
    }

    /**
     * Predict quality trends based on historical data
     * @param request The prediction request
     * @returns Observable of PredictionResponse
     */
    predictQualityTrends(request: PredictionRequest): Observable<PredictionResponse> {
        return this.http.post<PredictionResponse>(
            `${this.baseUrl}/predict-trends`, 
            request
        );
    }

    /**
     * Generate actionable insights from metrics
     * @param request The insight request
     * @returns Observable of InsightResponse
     */
    generateActionableInsights(request: InsightRequest): Observable<InsightResponse> {
        return this.http.post<InsightResponse>(
            `${this.baseUrl}/generate-insights`, 
            request
        );
    }

    /**
     * Optimize metric collection based on constraints
     * @param request The optimization request
     * @returns Observable of OptimizationResponse
     */
    optimizeMetricCollection(request: OptimizationRequest): Observable<OptimizationResponse> {
        return this.http.post<OptimizationResponse>(
            `${this.baseUrl}/optimize-collection`, 
            request
        );
    }

    /**
     * Handle error responses
     * @param error The error response
     * @returns Formatted error message
     */
    private handleError(error: any): string {
        if (error.error && error.error.message) {
            const errorResponse = error.error as MetricErrorResponse;
            return `${errorResponse.errorType}: ${errorResponse.message}`;
        }
        return 'An unexpected error occurred';
    }
}