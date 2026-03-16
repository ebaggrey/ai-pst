// src/app/models/supporting/bias-audit-summary.model.ts
export interface BiasAuditSummary {
  auditId: string;
  datasetName: string;
  auditDate: Date;
  riskLevel: string;
  overallScore: number;
  findingCount: number;
  suggestionCount: number;
}

// src/app/models/supporting/bias-audit-filter.model.ts
export interface BiasAuditFilter {
  datasetName?: string;
  fromDate?: Date;
  toDate?: Date;
  riskLevel?: string;
  minScore?: number;
  maxScore?: number;
}

// src/app/models/supporting/audit-statistics.model.ts
export interface AuditStatistics {
  totalAudits: number;
  averageBiasScore: number;
  riskLevelDistribution: Record<string, number>;
  commonBiasTypes: Array<{ type: string; count: number }>;
  topPerformingDatasets: Array<{ name: string; score: number }>;
}

// src/app/models/supporting/api-response.model.ts
export interface ApiResponse<T> {
  data?: T;
  error?: BiasAuditErrorResponse;
  status: number;
  message?: string;
}