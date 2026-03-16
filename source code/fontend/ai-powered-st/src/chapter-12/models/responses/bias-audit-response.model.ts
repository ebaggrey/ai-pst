// src/app/models/responses/bias-audit-response.model.ts

// Main response model
export interface BiasAuditResponse {
  datasetName: string;
  auditId: string;
  auditDate: Date;
  findings?: BiasFinding[];
  suggestions?: InclusiveSuggestion[];
  overallBiasScore: BiasScore;
  metadata?: Record<string, any>;
}

// Bias finding model
export interface BiasFinding {
  fieldName: string;
  biasType: string;
  description: string;
  severityScore: number;
  examples?: string[];
}

// Inclusive suggestion model
export interface InclusiveSuggestion {
  originalValue: string;
  suggestedValue: string;
  fieldName: string;
  rationale: string;
  confidenceScore: number;
}

// Bias score model
export interface BiasScore {
  overallScore: number;
  genderBiasScore: number;
  racialBiasScore: number;
  ageBiasScore: number;
  culturalBiasScore: number;
  riskLevel: string; // 'Low' | 'Medium' | 'High' | 'Critical'
}