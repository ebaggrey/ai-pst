// src/app/models/responses/api-response-wrapper.model.ts
import { BiasAuditResponse } from './bias-audit-response.model';
import { BiasAuditErrorResponse } from './bias-audit-error-response.model';
import { HealthCheckDetailedResponse, HealthCheckResponse } from './health-check-detailed-response.model';

// Generic API response wrapper
export interface ApiResponse<T> {
  data?: T;
  error?: BiasAuditErrorResponse;
  status: number;
  message?: string;
  timestamp: Date;
}

// Specific response types
export type BiasAuditApiResponse = ApiResponse<BiasAuditResponse>;
export type HealthCheckApiResponse = ApiResponse<HealthCheckResponse>;
export type HealthCheckDetailedApiResponse = ApiResponse<HealthCheckDetailedResponse>;

// Paginated response (if needed for future endpoints)
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// Example usage:
/*
const successResponse: BiasAuditApiResponse = {
  data: {
    datasetName: "hiring-data",
    auditId: "AUD-123",
    auditDate: new Date(),
    findings: [],
    suggestions: [],
    overallBiasScore: {
      overallScore: 0.3,
      genderBiasScore: 0.2,
      racialBiasScore: 0.4,
      ageBiasScore: 0.3,
      culturalBiasScore: 0.3,
      riskLevel: "Low"
    }
  },
  status: 200,
  message: "Audit completed successfully",
  timestamp: new Date()
};

const errorResponse: BiasAuditApiResponse = {
  error: {
    message: "Service unavailable",
    errorType: "AIServiceUnavailable",
    suggestedRemediation: "Try again later",
    errorId: "ERR-123",
    timestamp: new Date()
  },
  status: 503,
  timestamp: new Date()
};
*/