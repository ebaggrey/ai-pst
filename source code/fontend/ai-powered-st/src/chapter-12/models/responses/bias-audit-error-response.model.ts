// src/app/models/responses/bias-audit-error-response.model.ts
export interface BiasAuditErrorResponse {
  message: string;
  datasetName?: string;
  errorType: string;
  suggestedRemediation: string;
  errorId: string;
  timestamp: Date;
}

// Extended error response with additional fields for detailed error handling
export interface ExtendedErrorResponse extends BiasAuditErrorResponse {
  statusCode?: number;
  details?: any;
  stackTrace?: string; // Only in development mode
  validationErrors?: ValidationError[];
}

export interface ValidationError {
  field: string;
  message: string;
  attemptedValue?: any;
}

// Error severity levels
export enum ErrorSeverity {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical'
}

// Error category for better error handling
export enum ErrorCategory {
  Validation = 'Validation',
  Authentication = 'Authentication',
  Authorization = 'Authorization',
  NotFound = 'NotFound',
  Timeout = 'Timeout',
  ServiceUnavailable = 'ServiceUnavailable',
  InternalServer = 'InternalServer',
  Network = 'Network',
  Unknown = 'Unknown'
}

// Error response with category for better error handling in UI
export interface CategorizedError extends BiasAuditErrorResponse {
  category: ErrorCategory;
  severity: ErrorSeverity;
  retryable: boolean;
  userAction?: string;
}

// Error statistics for monitoring
export interface ErrorStatistics {
  totalErrors: number;
  errorsByType: Record<string, number>;
  errorsByCategory: Record<ErrorCategory, number>;
  recentErrors: BiasAuditErrorResponse[];
  mostCommonError: string;
  errorRate: number; // errors per minute
}

// Example usage:
/*
const errorResponse: BiasAuditErrorResponse = {
  message: "The AI insight service is having trouble right now.",
  datasetName: "hiring-data-2024",
  errorType: "AIServiceUnavailable",
  suggestedRemediation: "Please try again in a few moments. If it persists, contact the AI ops team.",
  errorId: "ERR-20240225-abc123def",
  timestamp: new Date()
};

const categorizedError: CategorizedError = {
  ...errorResponse,
  category: ErrorCategory.ServiceUnavailable,
  severity: ErrorSeverity.High,
  retryable: true,
  userAction: "Wait a few moments and try your request again"
};
*/