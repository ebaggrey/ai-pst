import { MetricDefinition } from "../requests/request-models";

// models/responses/metric-design-response.ts
export interface MetricDesignResponse {
    designId: string;
    metrics: DesignedMetric[];
    relationships: MetricRelationship[];
    collectionPlan: CollectionPlan;
    interpretationFramework: InterpretationFramework;
    validationRules: ValidationRule[];
    implementationGuidance: ImplementationGuidance;
    successCriteria: SuccessCriteria;
}

export interface DesignedMetric {
    metricId: string;
    name: string;
    description: string;
    formula: string;
    dataSource: string;
    businessObjectives: string[];
    targetValue: number;
    unit: string;
    dimensions: string[];
    weight: number;
    category: string;
}

export interface MetricRelationship {
    sourceMetricId: string;
    targetMetricId: string;
    relationshipType: string; // "correlation", "causation", "composition"
    strength: number;
}

export interface CollectionPlan {
    collectionSteps: string[];
    tools: string[];
    frequency: string;
    responsibleTeam: string;
}

export interface InterpretationFramework {
    interpretationRules: string[];
    thresholds: ThresholdDefinition[];
    outlierHandling: string[];
}

export interface ThresholdDefinition {
    metricId: string;
    green: number;
    yellow: number;
    red: number;
}

export interface ValidationRule {
    metricId: string;
    rule: string;
    errorMessage: string;
}

export interface ImplementationGuidance {
    prerequisites: string[];
    steps: string[];
    pitfalls: string[];
    successFactors: string[];
}

export interface SuccessCriteria {
    quantitativeCriteria: string[];
    qualitativeCriteria: string[];
    reviewPeriod: string;
}

// models/responses/health-score-response.ts
export interface HealthScoreResponse {
    healthScoreId: string;
    overallScore: number;
    componentScores: ComponentScore[];
    confidence: number;
    contributingFactors: ContributingFactor[];
    recommendations: ImprovementRecommendation[];
    trendAnalysis: TrendAnalysis;
    benchmarkComparison: BenchmarkComparison;
    alertStatus: AlertStatus;
}

export interface ComponentScore {
    componentName: string;
    score: number;
    weight: number;
    status: string;
}

export interface ContributingFactor {
    factor: string;
    impact: number;
    direction: string; // "positive", "negative"
}

export interface ImprovementRecommendation {
    metricId: string;
    recommendation: string;
    impact: number;
    priority: string;
    steps: string[];
}

export interface TrendAnalysis {
    direction: string;
    rate: number;
    observations: string[];
}

export interface BenchmarkComparison {
    percentile: number;
    gapToAverage: number;
    gapToBest: number;
}

export interface AlertStatus {
    level: string;
    alerts: string[];
    recommendedAction: string;
}

// models/responses/prediction-response.ts
export interface PredictionResponse {
    predictionId: string;
    currentState: CurrentStateAnalysis;
    predictions: MetricPrediction[];
    predictionConfidence: PredictionConfidence;
    interventions: Intervention[];
    riskAssessment: RiskAssessment;
    monitoringRecommendations: MonitoringRecommendation[];
    decisionSupport: DecisionSupport;
}

export interface CurrentStateAnalysis {
    analysisDate: Date;
    currentValues: { [key: string]: number };
    strengths: string[];
    weaknesses: string[];
}

export interface MetricPrediction {
    metricId: string;
    metricName: string;
    dates: Date[];
    values: number[];
    lowerBound: number[];
    upperBound: number[];
    trend: string;
}

export interface PredictionConfidence {
    averageConfidence: number;
    metricConfidence: { [key: string]: number };
    confidenceFactors: string[];
}

export interface Intervention {
    metricId: string;
    type: string;
    description: string;
    recommendedDate: Date;
    expectedImpact: number;
}

export interface RiskAssessment {
    overallRisk: string;
    identifiedRisks: string[];
    mitigationStrategies: string[];
}

export interface MonitoringRecommendation {
    metricId: string;
    frequency: string;
    threshold: string;
}

export interface DecisionSupport {
    decisionPoints: string[];
    recommendedActions: string[];
    tradeoffs: { [key: string]: string };
}

// models/responses/insight-response.ts
export interface InsightResponse {
    insightId: string;
    generatedInsights: ActionableInsight[];
    implementationGuidance: InsightImplementationGuidance;
    insightQuality: InsightQuality;
    validationSteps: ValidationStep[];
    communicationPlan: CommunicationPlan;
    monitoringPlan: InsightMonitoringPlan;
}

export interface ActionableInsight {
    insightId: string;
    title: string;
    description: string;
    type: string;
    actionabilityScore: number;
    impact: number;
    affectedMetrics: string[];
    recommendedActions: string[];
    priority: string;
}

export interface InsightImplementationGuidance {
    prerequisites: string[];
    steps: string[];
    resources: string[];
}

export interface InsightQuality {
    accuracy: number;
    relevance: number;
    timeliness: number;
    limitations: string[];
}

export interface ValidationStep {
    step: string;
    method: string;
    expectedOutcome: string;
}

export interface CommunicationPlan {
    stakeholders: string[];
    message: string;
    channel: string;
    timeline: Date;
}

export interface InsightMonitoringPlan {
    metrics: string[];
    reviewFrequency: string;
    successCriteria: string;
}

// models/responses/optimization-response.ts
export interface OptimizationResponse {
    optimizationId: string;
    currentMetrics: MetricDefinition[];
    optimization: OptimizationRecommendation;
    expectedBenefits: ExpectedBenefits;
    implementationPlan: ImplementationPlan;
    preservationValidation: PreservationValidation;
    riskAssessment: OptimizationRiskAssessment;
    monitoringPlan: OptimizationMonitoringPlan;
    continuousImprovement: ContinuousImprovementPlan;
}

export interface OptimizationRecommendation {
    recommendedActions: RecommendedAction[];
    consolidations: MetricConsolidation[];
    deprecatedMetrics: string[];
    newMetrics: string[];
}

export interface RecommendedAction {
    metricId: string;
    action: string; // "keep", "modify", "remove", "add"
    rationale: string;
    costSaving: number;
}

export interface MetricConsolidation {
    sourceMetricIds: string[];
    targetMetricId: string;
    consolidationMethod: string;
}

export interface ExpectedBenefits {
    costReduction: number;
    efficiencyGain: number;
    qualityImprovement: number;
    additionalBenefits: string[];
}

export interface ImplementationPlan {
    phases: string[];
    timeline: string;
    dependencies: string[];
    rollbackSteps: string[];
}

export interface PreservationValidation {
    isPreserved: boolean;
    validatedRules: string[];
    warnings: string[];
}

export interface OptimizationRiskAssessment {
    risks: string[];
    mitigations: string[];
}

export interface OptimizationMonitoringPlan {
    keyIndicators: string[];
    reviewFrequency: string;
}

export interface ContinuousImprovementPlan {
    reviewCycles: string[];
    feedbackLoops: string[];
}

// models/responses/error-response.ts
export interface MetricErrorResponse {
    errorType: string;
    message: string;
    recoverySteps: string[];
    fallbackSuggestion: string;
    diagnosticData?: MetricDiagnosticData;
}

export interface MetricDiagnosticData {
    ambiguousObjectives?: string[];
    clarificationQuestions?: string[];
    inconsistencyDetails?: InconsistencyDetails;
    dataQualityIssues?: DataQualityIssue[];
    patternDetectionChallenges?: string[];
    dataRequirements?: string[];
    insightGenerationChallenges?: string[];
    metricLimitations?: string[];
    conflictingGoals?: string[];
    tradeOffAnalysis?: TradeOffAnalysis;
}

export interface InconsistencyDetails {
    inconsistentMetrics?: string[];
    timePeriodIssues?: string[];
    unitMismatches?: string[];
}

export interface DataQualityIssue {
    metricId: string;
    issue: string;
    severity: string;
}

export interface TradeOffAnalysis {
    tradeoffs?: string[];
    impacts?: { [key: string]: number };
}