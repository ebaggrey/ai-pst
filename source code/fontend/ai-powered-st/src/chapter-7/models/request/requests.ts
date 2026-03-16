
// Pipeline Generation Models
export interface CodebaseAnalysis {
    language: string;
    testCoverage: number;
    totalLines: number;
    dependencies: string[];
}

export interface PipelineConstraints {
    maxDuration: string;
    maxCostPerRun: number;
}

export interface TeamPractices {
    codeReviews: boolean;
    automatedTesting: boolean;
    deploymentStrategy: string[];
}

export interface OptimizationFocus {
    speed: boolean;
    reliability: boolean;
    cost: boolean;
}

export interface PipelineGenerationRequest {
    codebaseAnalysis: CodebaseAnalysis;
    constraints: PipelineConstraints;
    teamPractices: TeamPractices;
    optimizationFocus: OptimizationFocus;
}

// Diagnosis Models
export interface FailureLogs {
    rawLogs: string;
    failureTime: Date;
}

export interface Change {
    id: string;
    type: string;
    description: string;
}

export interface RecentChanges {
    changes: Change[];
}

export enum DiagnosisDepth {
    Shallow = 0,
    Standard = 1,
    Deep = 2
}

export interface PipelineContext {
    pipelineId: string;
    stage: string;
}

export interface DiagnosisRequest {
    failureLogs: FailureLogs;
    recentChanges: RecentChanges;
    diagnosisDepth: DiagnosisDepth;
    includeRemediation: boolean;
    preventionStrategies: boolean;
    pipelineContext: PipelineContext;
}

// Optimization Models
export interface PipelineMetrics {
    averageDuration: string; // ISO 8601 duration format
    successRate: number;
    resourceUtilization: number;
}

export interface Bottleneck {
    stageId: string;
    description: string;
    impactScore: number;
}

export enum Priority {
    Low = 0,
    Medium = 1,
    High = 2
}

export interface OptimizationGoal {
    type: string;
    targetValue: number;
    priority: Priority;
}

export interface OptimizationConstraints {
    maxBudget: number;
    maxDowntime: string; // ISO 8601 duration format
}

export interface OptimizationRequest {
    currentMetrics: PipelineMetrics;
    identifiedBottlenecks: Bottleneck[];
    optimizationGoals: OptimizationGoal[];
    constraints: OptimizationConstraints;
}

// Prediction Models
export interface ProposedChange {
    id: string;
    type: string;
    description: string;
}

export interface PipelineRun {
    runId: string;
    timestamp: Date;
    succeeded: boolean;
    errors: string[];
}

export interface HistoricalData {
    runs: PipelineRun[];
}

export interface PredictionRequest {
    proposedChanges: ProposedChange[];
    historicalData: HistoricalData;
    predictionHorizon: string; // ISO 8601 duration format
    confidenceThreshold: number;
    includeMitigations: boolean;
}

// Adaptation Models
export enum AdaptationStrategy {
    Conservative = 0,
    Balanced = 1,
    Aggressive = 2
}

export interface ValidationRule {
    ruleId: string;
    condition: string;
}

export interface ImpactAssessment {
    impactScore: number;
    affectedComponents: string[];
}

export interface AdaptationRequest {
    changeType: string;
    impactAssessment: ImpactAssessment;
    adaptationStrategy: AdaptationStrategy;
    validationRules: ValidationRule[];
}