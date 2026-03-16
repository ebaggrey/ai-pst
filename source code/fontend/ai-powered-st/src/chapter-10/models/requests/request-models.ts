// models/requests/metric-design-request.ts
export interface MetricDesignRequest {
    businessObjectives: string[];
    testingActivities: string[];
    designPrinciples: string[];
    constraints: MetricConstraints;
}

export interface MetricConstraints {
    maxMetrics: number;
    minDataPoints: number;
    requiredDimensions?: string[];
    allowCompositeMetrics: boolean;
}

// models/requests/health-score-request.ts
export interface HealthScoreRequest {
    metricValues: MetricValue[];
    historicalBaselines: HistoricalBaseline[];
    normalizationMethod: string;
    weightingStrategy: string;
    confidenceThreshold: number;
}

export interface MetricValue {
    metricId: string;
    metricName: string;
    value: number;
    timestamp: Date;
    attributes?: { [key: string]: any };
}

export interface HistoricalBaseline {
    metricId: string;
    mean: number;
    standardDeviation: number;
    min: number;
    max: number;
    sampleSize: number;
    periodStart: Date;
    periodEnd: Date;
}

// models/requests/prediction-request.ts
export interface PredictionRequest {
    currentMetrics: MetricValue[];
    historicalTrends: HistoricalTrend[];
    predictionHorizon: number; // Days
    confidenceIntervals: number[];
    includeInterventions: boolean;
}

export interface HistoricalTrend {
    date: Date;
    metricValues: { [key: string]: number };
    seasonality?: string;
}

// models/requests/insight-request.ts
export interface InsightRequest {
    metrics: MetricValue[];
    insightTypes: string[];
    actionabilityThreshold: number;
    context: InsightContext;
}

export interface InsightContext {
    stakeholders?: string[];
    businessGoals?: string[];
    constraints?: { [key: string]: any };
    priorityLevel?: string;
}

// models/requests/optimization-request.ts
export interface OptimizationRequest {
    currentMetrics: MetricDefinition[];
    resourceConstraints: ResourceConstraint[];
    optimizationGoals?: string[];
    preservationRules?: PreservationRule[];
}

export interface MetricDefinition {
    metricId: string;
    name: string;
    category: string;
    collectionMethod: string;
    collectionCost: number;
    businessValue: number;
    dependencies?: string[];
}

export interface ResourceConstraint {
    resourceType: string;
    maxAllocation: number;
    unit: string;
    period: string;
}

export interface PreservationRule {
    metricId: string;
    rule: string;
    reason: string;
}