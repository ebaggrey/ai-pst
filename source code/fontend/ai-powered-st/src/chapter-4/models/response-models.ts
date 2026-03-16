import { Alert, Antipattern, BaselineComparison, BiasFinding, CapabilityMetric, ConfidenceAdjustment, HallucinationFinding, MetricDrift, MitigationStrategy, MonitoringPlan, OptimizationSuggestion, Recommendation, RecommendedAction, VarianceAnalysis, VariationResult, VerificationRule } from "./supporting-models";

// models/responses/ai-capability-report.model.ts
export interface AICapabilityReport {
  provider: string;
  modelName: string;
  overallScore: number;
  dimensionScores: Record<string, number>;
  metrics: CapabilityMetric[];
  recommendations: Recommendation[];
  assessmentDate: Date;
}

// models/responses/robustness-test-report.model.ts
export interface RobustnessTestReport {
  basePrompt: string;
  variationCount: number;
  runCount: number;
  averageConsistencyScore: number;
  variationResults: VariationResult[];
  varianceAnalysis: VarianceAnalysis;
  antipatterns: Antipattern[];
  optimizationSuggestions: OptimizationSuggestion[];
}

// models/responses/bias-detection-report.model.ts
export interface BiasDetectionReport {
  findings: BiasFinding[];
  overallBiasScore: number;
  contextBiasScores: Record<string, number>;
  mitigationStrategies: MitigationStrategy[];
  longTermMonitoringPlan: MonitoringPlan;
  statisticalSignificanceValidated: boolean;
}

// models/responses/hallucination-detection-report.model.ts
export interface HallucinationDetectionReport {
  provider: string;
  hallucinations: HallucinationFinding[];
  hallucinationRate: number;
  confidenceAdjustments: ConfidenceAdjustment[];
  verificationRules: VerificationRule[];
  totalTests: number;
  hallucinationCount: number;
}

// models/responses/drift-detection-report.model.ts
export interface DriftDetectionReport {
  startDate: Date;
  endDate: Date;
  driftSignificance: number;
  metricDrifts: Record<string, MetricDrift>;
  alerts: Alert[];
  recommendedActions: RecommendedAction[];
  baselineComparison: BaselineComparison;
}