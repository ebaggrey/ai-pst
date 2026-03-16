// services/ai-testing.service.ts
import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, timer } from 'rxjs';
import { catchError, timeout, retryWhen, delayWhen, map } from 'rxjs/operators';
import { AITestingConfig, DEFAULT_AITESTING_CONFIG } from '../configuration';
import { AIAssessmentRequest, BiasDetectionRequest, DriftDetectionRequest, HallucinationTestRequest, RobustnessTestRequest } from '../models/core-models';
import { AICapabilityReport, BiasDetectionReport, DriftDetectionReport, HallucinationDetectionReport, RobustnessTestReport } from '../models/response-models';
import { AITestingError } from '../models/supporting-models';


@Injectable({
  providedIn: 'root'
})
export class AITestingService {
  private config: AITestingConfig;
  private cache = new Map<string, { data: any; timestamp: number }>();

  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    })
  };

  constructor(
    private http: HttpClient,
    @Inject('AITESTING_CONFIG') config?: Partial<AITestingConfig>
  ) {
    this.config = { ...DEFAULT_AITESTING_CONFIG, ...config };
  }

  // ========== Public Methods ==========

  assessAICapabilities(request: AIAssessmentRequest): Observable<AICapabilityReport> {
    const url = `${this.config.apiBaseUrl}/assess-capabilities`;
    return this.post<AICapabilityReport>(url, request);
  }

  testPromptRobustness(request: RobustnessTestRequest): Observable<RobustnessTestReport> {
    const url = `${this.config.apiBaseUrl}/test-robustness`;
    return this.post<RobustnessTestReport>(url, request);
  }

  detectAIBias(request: BiasDetectionRequest): Observable<BiasDetectionReport> {
    const url = `${this.config.apiBaseUrl}/detect-bias`;
    return this.post<BiasDetectionReport>(url, request);
  }

  testForHallucinations(request: HallucinationTestRequest): Observable<HallucinationDetectionReport> {
    const url = `${this.config.apiBaseUrl}/test-hallucinations`;
    return this.post<HallucinationDetectionReport>(url, request);
  }

  monitorAIDrift(request: DriftDetectionRequest): Observable<DriftDetectionReport> {
    const url = `${this.config.apiBaseUrl}/monitor-drift`;
    return this.post<DriftDetectionReport>(url, request);
  }

  getTestHistory(provider?: string, startDate?: Date, endDate?: Date): Observable<any[]> {
    const url = `${this.config.apiBaseUrl}/test-history`;
    let params: any = {};
    
    if (provider) params.provider = provider;
    if (startDate) params.startDate = startDate.toISOString();
    if (endDate) params.endDate = endDate.toISOString();
    
    return this.get<any[]>(url, params);
  }

  getTestStatus(testId: string): Observable<any> {
    const url = `${this.config.apiBaseUrl}/test-status/${testId}`;
    return this.get<any>(url);
  }

  // ========== Private HTTP Methods ==========

  private get<T>(url: string, params?: any): Observable<T> {
    const cacheKey = this.generateCacheKey('GET', url, params);
    
    // Check cache first
    if (this.config.enableCaching) {
      const cached = this.getFromCache<T>(cacheKey);
      if (cached) {
        return new Observable<T>(observer => {
          observer.next(cached);
          observer.complete();
        });
      }
    }

    const options = {
      ...this.httpOptions,
      params: params
    };

    return this.http.get<T>(url, options).pipe(
      timeout(this.config.requestTimeout),
      retryWhen(errors => this.handleRetry(errors)),
      map(response => {
        if (this.config.enableCaching) {
          this.saveToCache(cacheKey, response);
        }
        return response;
      }),
      catchError(error => this.handleError(error))
    );
  }

  private post<T>(url: string, body: any): Observable<T> {
    const cacheKey = this.generateCacheKey('POST', url, body);
    
    // Check cache for idempotent requests
    if (this.config.enableCaching && this.isCacheableRequest(body)) {
      const cached = this.getFromCache<T>(cacheKey);
      if (cached) {
        return new Observable<T>(observer => {
          observer.next(cached);
          observer.complete();
        });
      }
    }

    return this.http.post<T>(url, body, this.httpOptions).pipe(
      timeout(this.config.requestTimeout),
      retryWhen(errors => this.handleRetry(errors)),
      map(response => {
        if (this.config.enableCaching && this.isCacheableRequest(body)) {
          this.saveToCache(cacheKey, response);
        }
        return response;
      }),
      catchError(error => this.handleError(error))
    );
  }

  // ========== Cache Management ==========

  private generateCacheKey(method: string, url: string, data: any): string {
    const dataStr = JSON.stringify(data || {});
    return `${method}:${url}:${this.hashString(dataStr)}`;
  }

  private hashString(str: string): string {
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
      const char = str.charCodeAt(i);
      hash = ((hash << 5) - hash) + char;
      hash = hash & hash;
    }
    return hash.toString(36);
  }

  private getFromCache<T>(key: string): T | null {
    const cached = this.cache.get(key);
    if (!cached) return null;
    
    const now = Date.now();
    if (now - cached.timestamp > this.config.cacheDuration) {
      this.cache.delete(key);
      return null;
    }
    
    return cached.data;
  }

  private saveToCache(key: string, data: any): void {
    this.cache.set(key, {
      data: data,
      timestamp: Date.now()
    });
    
    // Simple cache cleanup - keep only last 100 items
    if (this.cache.size > 100) {
      const keys = Array.from(this.cache.keys());
      for (let i = 0; i < 20; i++) {
        this.cache.delete(keys[i]);
      }
    }
  }

  private isCacheableRequest(body: any): boolean {
    // Only cache idempotent requests (GET-equivalent POSTs)
    // Add logic based on your API design
    return body?.rigorLevel === 'standard' || 
           body?.testIterations === 10; // Example conditions
  }

  clearCache(): void {
    this.cache.clear();
  }

  // ========== Error Handling ==========

  private handleRetry(errors: Observable<any>): Observable<any> {
    return errors.pipe(
      delayWhen((error, retryCount) => {
        if (retryCount < this.config.maxRetryAttempts && this.shouldRetry(error)) {
          return timer(this.config.retryDelay * Math.pow(2, retryCount));
        }
        return throwError(() => error);
      })
    );
  }

  private shouldRetry(error: HttpErrorResponse): boolean {
    // Retry on network errors, timeouts, or server errors
    return error.status === 0 || 
           error.status === 408 || 
           error.status === 429 || 
           error.status >= 500;
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('AITestingService error:', error);
    
    const testingError: AITestingError = {
      testType: 'api-call',
      provider: '',
      failureMode: this.getFailureMode(error),
      diagnosticInfo: {
        errorCode: error.status || 500,
        errorMessage: this.getErrorMessage(error),
        suggestedInvestigation: this.getSuggestions(error)
      },
      fallbackAction: this.getFallbackAction(error)
    };
    
    return throwError(() => testingError);
  }

  private getFailureMode(error: HttpErrorResponse): string {
    if (error.status === 0) return 'network-error';
    if (error.status === 400) return 'bad-request';
    if (error.status === 401) return 'unauthorized';
    if (error.status === 403) return 'forbidden';
    if (error.status === 404) return 'not-found';
    if (error.status === 408) return 'timeout';
    if (error.status === 429) return 'rate-limited';
    if (error.status >= 500) return 'server-error';
    return 'unknown-error';
  }

  private getErrorMessage(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'Network error: Unable to connect to the server';
    }
    
    if (error.error?.message) {
      return error.error.message;
    }
    
    if (error.message) {
      return error.message;
    }
    
    return `HTTP Error ${error.status}: ${error.statusText}`;
  }

  private getSuggestions(error: HttpErrorResponse): string[] {
    const suggestions: string[] = [];
    
    if (error.status === 0) {
      suggestions.push('Check your internet connection');
      suggestions.push('Verify the server is running');
      suggestions.push('Check if the API URL is correct');
    } else if (error.status === 400) {
      suggestions.push('Review your request parameters');
      suggestions.push('Check data types and formats');
    } else if (error.status === 429) {
      suggestions.push('Wait a few moments before retrying');
      suggestions.push('Reduce the frequency of requests');
    } else if (error.status >= 500) {
      suggestions.push('Try again in a few minutes');
      suggestions.push('Contact support if issue persists');
    }
    
    suggestions.push('Check browser console for details');
    
    return suggestions;
  }

  private getFallbackAction(error: HttpErrorResponse): string {
    if (error.status === 429) return 'wait-and-retry';
    if (error.status >= 500) return 'retry-later';
    return 'check-parameters';
  }

  // ========== Configuration ==========

  getConfig(): AITestingConfig {
    return { ...this.config };
  }

  updateConfig(updates: Partial<AITestingConfig>): void {
    this.config = { ...this.config, ...updates };
  }
}