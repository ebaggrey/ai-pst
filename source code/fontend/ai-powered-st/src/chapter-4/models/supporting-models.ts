// models/supporting/diagnostic-info.model.ts
export interface DiagnosticInfo {
  errorCode: number;
  errorMessage: string;
  suggestedInvestigation: string[];
}

// models/supporting/ai-testing-error.model.ts
export interface AITestingError {
  testType: string;
  provider: string;
  failureMode: string;
  diagnosticInfo: DiagnosticInfo;
  fallbackAction: string;
  metadata?: TestMetadata;
}

// models/supporting/test-metadata.model.ts
export interface TestMetadata {
  testCaseId: string;
  inputPrompt: string;
  expectedBehavior: string;
}

// models/supporting/baseline-data.model.ts
export interface BaselineData {
  testResults: TestResult[];
  collectedOn: Date;
  environment: string;
}

// models/supporting/test-result.model.ts
export interface TestResult {
  testId: string;
  timestamp: Date;
  metrics: Record<string, number>;
  status: 'completed' | 'failed' | 'pending';
}

// models/supporting/capability-metric.model.ts
export interface CapabilityMetric {
  name: string;
  score: number;
  weight: number;
  category: string;
}

// models/supporting/recommendation.model.ts
export interface Recommendation {
  area: string;
  suggestion: string;
  priority: 'low' | 'medium' | 'high' | 'critical';
  impact: string;
}

// models/supporting/variation-result.model.ts
export interface VariationResult {
  variation: string;
  consistencyScore: number;
  responses: string[];
  passed: boolean;
}

// models/supporting/variance-analysis.model.ts
export interface VarianceAnalysis {
  overallVariance: number;
  variationVariances: Record<string, number>;
  highVarianceVariations: string[];
  stableVariations: string[];
}

// models/supporting/antipattern.model.ts
export interface Antipattern {
  pattern: string;
  description: string;
  severity: 'low' | 'medium' | 'high' | 'critical';
  fix: string;
}

// models/supporting/optimization-suggestion.model.ts
export interface OptimizationSuggestion {
  area: string;
  suggestion: string;
  expectedImprovement: number;
}

// models/supporting/bias-finding.model.ts
export interface BiasFinding {
  context: string;
  biasType: string;
  confidence: number;
  evidence: string;
  severity: 'low' | 'medium' | 'high' | 'critical';
}

// models/supporting/mitigation-strategy.model.ts
export interface MitigationStrategy {
  findingId: string;
  strategy: string;
  implementation: string;
  timeline: 'immediate' | 'short-term' | 'medium-term' | 'long-term';
}

// models/supporting/monitoring-plan.model.ts
export interface MonitoringPlan {
  metrics: string[];
  frequency: 'hourly' | 'daily' | 'weekly' | 'monthly';
  triggers: string[];
  reportingFormat: string;
}

// models/supporting/hallucination-finding.model.ts
export interface HallucinationFinding {
  fact: string;
  aiResponse: string;
  severity: 'low' | 'medium' | 'high' | 'critical';
  category: string;
  correction: string;
}

// models/supporting/confidence-adjustment.model.ts
export interface ConfidenceAdjustment {
  context: string;
  adjustmentFactor: number;
  reason: string;
}

// models/supporting/verification-rule.model.ts
export interface VerificationRule {
  pattern: string;
  verificationMethod: string;
  confidenceLevel: 'low' | 'medium' | 'high';
  autoVerify: boolean;
}

// models/supporting/metric-drift.model.ts
export interface MetricDrift {
  metricName: string;
  baselineValue: number;
  currentValue: number;
  driftAmount: number;
  direction: 'positive' | 'negative' | 'stable';
  significant: boolean;
}

// models/supporting/alert.model.ts
export interface Alert {
  alertId: string;
  metric: string;
  severity: 'info' | 'warning' | 'error' | 'critical';
  message: string;
  triggeredAt: Date;
}

// models/supporting/recommended-action.model.ts
export interface RecommendedAction {
  action: string;
  priority: 'low' | 'medium' | 'high' | 'critical';
  impact: string;
  effort: 'low' | 'medium' | 'high';
}

// models/supporting/baseline-comparison.model.ts
export interface BaselineComparison {
  dataPointsCompared: number;
  similarityScore: number;
  comparisonDate: Date;
}