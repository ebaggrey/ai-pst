// src/app/models/pipeline-cookbook/responses.ts

import { ImpactAssessment, PipelineMetrics, ProposedChange } from "../request/requests";

// Pipeline Generation Responses
export interface Stage {
    id: string;
    name: string;
}

export interface PipelineDefinition {
    stages: Stage[];
}

export interface DecisionPoint {
    id: string;
    condition: string;
}

export interface RecoveryPath {
    id: string;
    steps: string[];
}

export interface EstimatedMetrics {
    estimatedDuration: string; // ISO 8601 duration format
    estimatedCost: number;
}

export interface OptimizationSuggestion {
    id: string;
    description: string;
}

export interface MonitoringConfiguration {
    metrics: string[];
}

export interface AdaptationGuidance {
    recommendations: string[];
}

export interface ConflictingConstraint {
    constraintA: string;
    constraintB: string;
}

export interface ConstraintSuggestion {
    constraint: string;
    suggestedValue: string;
}

export interface PartialPipeline {
    stages: Stage[];
}

export interface IntelligentPipelineResponse {
    pipelineId: string;
    pipelineDefinition: PipelineDefinition;
    decisionPoints: DecisionPoint[];
    recoveryPaths: RecoveryPath[];
    estimatedMetrics: EstimatedMetrics;
    optimizationSuggestions: OptimizationSuggestion[];
    monitoringConfiguration: MonitoringConfiguration;
    adaptationGuidance: AdaptationGuidance;
    constraintConflicts?: ConflictingConstraint[];
    constraintSuggestions?: ConstraintSuggestion[];
    partialPipeline?: PartialPipeline;
}

// Diagnosis Responses
export interface ParsedFailure {
    errorType: string;
    message: string;
    stackTrace: string;
}

export interface RootCause {
    summary: string;
    component: string;
}

export interface RemediationStep {
    stepNumber: number;
    action: string;
}

export interface PreventionStrategy {
    id: string;
    description: string;
}

export interface HistoricalFailure {
    failureId: string;
    occurredAt: Date;
}

export interface DiagnosisResponse {
    diagnosisId: string;
    failureDetails: ParsedFailure;
    rootCause: RootCause;
    confidence: number;
    remediationSteps: RemediationStep[];
    preventionStrategies: PreventionStrategy[];
    similarHistoricalFailures: HistoricalFailure[];
    impactAssessment: ImpactAssessment;
}

// Optimization Responses
export interface OptimizationOpportunity {
    id: string;
    description: string;
}

export interface OptimizationStrategy {
    id: string;
    name: string;
}

export interface TradeOff {
    strategyA: string;
    strategyB: string;
}

export interface TradeOffAnalysis {
    tradeOffs: TradeOff[];
}

export interface ImplementationStep {
    order: number;
    action: string;
}

export interface ImplementationPlan {
    steps: ImplementationStep[];
}

export interface ExpectedImprovement {
    metric: string;
    improvement: number;
}

export interface Risk {
    description: string;
    probability: number;
}

export interface RiskAssessment {
    risks: Risk[];
}

export interface ValidationPlan {
    testCases: string[];
}

export interface OptimizationResponse {
    optimizationId: string;
    currentPerformance: PipelineMetrics;
    optimizationOpportunities: OptimizationOpportunity[];
    proposedStrategies: OptimizationStrategy[];
    tradeOffAnalysis: TradeOffAnalysis;
    implementationPlan: ImplementationPlan;
    expectedImprovements: ExpectedImprovement[];
    riskAssessment: RiskAssessment;
    validationPlan: ValidationPlan;
}

// Prediction Responses
export interface Prediction {
    issue: string;
    probability: number;
}

export interface RiskScore {
    category: string;
    score: number;
}

export interface Mitigation {
    predictionId: string;
    action: string;
}

export interface RecommendedAction {
    action: string;
    priority: number;
}

export interface MonitoringRecommendation {
    metric: string;
    threshold: number;
}

export interface PatternMatch {
    patternId: string;
    similarity: number;
}

export interface PredictionResponse {
    predictionId: string;
    proposedChanges: ProposedChange[];
    predictions: Prediction[];
    confidenceScores: number[];
    riskScores: RiskScore[];
    mitigations: Mitigation[];
    recommendedActions: RecommendedAction[];
    monitoringRecommendations: MonitoringRecommendation[];
    historicalEvidence: PatternMatch[];
}

// Adaptation Responses
export interface AdaptationStep {
    id: string;
    description: string;
}

export interface AdaptationPlan {
    steps: AdaptationStep[];
}

export interface ValidationResult {
    ruleId: string;
    passed: boolean;
}

export interface RollbackStep {
    order: number;
    action: string;
}

export interface RollbackPlan {
    steps: RollbackStep[];
}

export interface EffortEstimate {
    duration: string; // ISO 8601 duration format
    complexity: number;
}

export interface TestingStrategy {
    testTypes: string[];
}

export interface CommunicationPlan {
    stakeholders: string[];
}

export interface AdaptationResponse {
    adaptationId: string;
    changeType: string;
    adaptationPlan: AdaptationPlan;
    validationResults: ValidationResult[];
    rollbackPlan: RollbackPlan;
    effortEstimate: EffortEstimate;
    implementationSteps: ImplementationStep[];
    testingStrategy: TestingStrategy;
    communicationPlan: CommunicationPlan;
}