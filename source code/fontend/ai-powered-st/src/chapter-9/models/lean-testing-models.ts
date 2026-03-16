// models/feature.model.ts
export interface Feature {
    id: string;
    name: string;
    description: string;
    businessValue: number;
    riskLevel: number;
    implementationComplexity: number;
    testingEffort: number;
    dependencies: string[];
    attributes?: { [key: string]: any };
}

// models/testing-constraints.model.ts
export interface TestingConstraints {
    maxTimeHours: number;
    maxCost: number;
    maxParallelTests: number;
    requiredEnvironments: string[];
    complianceRules?: { [key: string]: string };
}

// models/priority-request.model.ts
export enum PrioritizationMethod {
    WeightedShortestJobFirst = 'WeightedShortestJobFirst',
    CostOfDelayDividedByDuration = 'CostOfDelayDividedByDuration',
    BusinessValueFirst = 'BusinessValueFirst',
    RiskBased = 'RiskBased',
    MoSCoW = 'MoSCoW'
}

export interface CostOfDelay {
    dailyCost: number;
    delayType: 'user-adoption' | 'market-window' | 'compliance';
    deadline?: Date;
}

export interface PriorityRequest {
    features: Feature[];
    constraints: TestingConstraints;
    prioritizationMethod: PrioritizationMethod;
    maxTestingBudget: number;
    costOfDelay: CostOfDelay;
}

// models/coverage-request.model.ts
export interface RiskProfile {
    highRiskAreas: string[];
    criticalFlows: string[];
    riskTolerance: number;
}

export interface CoverageConstraints {
    maxTestCases: number;
    maxExecutionTimeMinutes: number;
    requiredCoverageTypes: string[];
}

export enum OptimizationGoal {
    MaximizeCoverage = 'MaximizeCoverage',
    MinimizeTests = 'MinimizeTests',
    BalanceRiskAndEffort = 'BalanceRiskAndEffort'
}

export interface CoverageRequest {
    feature: Feature;
    confidenceTarget: number;
    riskProfile: RiskProfile;
    constraints: CoverageConstraints;
    optimizationGoal: OptimizationGoal;
}

// models/automation-request.model.ts
export interface TestScenario {
    id: string;
    name: string;
    description: string;
    executionFrequency: number;
    averageDurationMinutes: number;
    testDataRequirements: string[];
}

export interface AutomationCost {
    initialCost: number;
    maintenanceCostPerMonth: number;
    infrastructureCost: number;
    trainingCost: number;
}

export interface ManualCost {
    executionCost: number;
    setupCost: number;
    reportingCost: number;
}

export interface DecisionFactor {
    name: string;
    weight: number;
    score: number;
}

export interface AutomationDecisionRequest {
    testScenario: TestScenario;
    automationCost: AutomationCost;
    manualCost: ManualCost;
    roiThreshold: number;
    decisionFactors: DecisionFactor[];
}

// models/maintenance-request.model.ts
export interface TestCase {
    id: string;
    name: string;
    steps: string[];
    coveredRequirements: string[];
    executionFrequency: number;
    lastExecution: Date;
    failureRate: number;
    maintenanceCost: number;
    businessCriticality: number;
}

export interface TestSuite {
    suiteId: string;
    testCases: TestCase[];
    applicationArea: string;
}

export interface ChangeImpact {
    changedComponents: string[];
    affectedFeatures: string[];
    impactRadius: number;
}

export enum OptimizationStrategy {
    ConsolidateSimilarTests = 'ConsolidateSimilarTests',
    RemoveRedundantTests = 'RemoveRedundantTests',
    SimplifyTestSteps = 'SimplifyTestSteps',
    RetireObsoleteTests = 'RetireObsoleteTests'
}

export interface PreservationRule {
    ruleId: string;
    description: string;
    mustPreserve: boolean;
}

export interface MaintenanceRequest {
    existingTests: TestSuite;
    changeImpact: ChangeImpact;
    optimizationStrategy: OptimizationStrategy;
    allowedActions: string[];
    preservationRules: PreservationRule[];
}

// models/roi-request.model.ts
export interface TestInvestment {
    category: string;
    cost: number;
    date: Date;
}

export interface TestOutcome {
    category: string;
    value: number;
    date: Date;
    type: 'tangible' | 'intangible';
}

export interface ROIRequest {
    testInvestments: TestInvestment[];
    outcomes: TestOutcome[];
    measurementPeriod: string; // ISO 8601 duration format
    costCategories: string[];
    valueCategories: string[];
    includeIntangibles: boolean;
}

// models/responses.model.ts
export interface OptimizedFeature extends Feature {
    priority: number;
    priorityScore: number;
    expectedValuePerHour: number;
    riskAdjustedValue: number;
    optimizationRationale: string[];
}

export interface ExpectedROI {
    expectedValue: number;
    expectedCost: number;
    ratio: number;
    expectedPaybackPeriod: string; // ISO 8601 duration
}

export interface ConfidenceScore {
    featureId: string;
    score: number;
    rationale: string;
}

export interface TradeOff {
    description: string;
    options: string[];
    recommended: string;
}

export interface AlternativePrioritization {
    name: string;
    features: OptimizedFeature[];
    rationale: string;
}

export interface ResourceAllocation {
    allocations: { [key: string]: number };
    timeline: { [key: string]: string };
}

export interface RiskFactor {
    name: string;
    probability: number;
    impact: number;
}

export interface MitigationStrategy {
    risk: string;
    strategy: string;
}

export interface RiskAssessment {
    overallRiskScore: number;
    riskFactors: RiskFactor[];
    mitigations: MitigationStrategy[];
}

export interface TestingPriorityResponse {
    prioritizationId: string;
    features: OptimizedFeature[];
    reasoning: string[];
    expectedROI: ExpectedROI;
    confidenceScores: ConfidenceScore[];
    tradeOffs: TradeOff[];
    nextBestAlternatives: AlternativePrioritization[];
    resourceAllocation: ResourceAllocation;
    riskAssessment: RiskAssessment;
}

export interface GapAnalysis {
    untestedRequirements: string[];
    lowCoverageAreas: string[];
    recommendations: string[];
}

export interface CoverageMetrics {
    functionalCoverage: number;
    riskCoverage: number;
    valueCoverage: number;
    requirementCoverage: number;
    coverageGaps: GapAnalysis;
}

export interface TestExecution {
    testId: string;
    scheduledTime: Date;
    environment: string;
    prerequisites: string[];
}

export interface ExecutionPlan {
    executions: TestExecution[];
    totalDurationMinutes: number;
    parallelizationStrategy: string;
}

export interface SimplificationOpportunity {
    testId: string;
    suggestion: string;
    impactScore: number;
    implementation: string;
}

export interface CoverageResponse {
    suiteId: string;
    feature: Feature;
    testCases: TestCase[];
    coverageMetrics: CoverageMetrics;
    executionPlan: ExecutionPlan;
    maintenanceGuidance: string[];
    confidenceAchieved: number;
    efficiencyScore: number;
    simplificationOpportunities: SimplificationOpportunity[];
}

export interface CostBreakdown {
    initial: number;
    recurring: number;
    maintenance: number;
}

export interface CostAnalysis {
    totalAutomationCost: number;
    totalManualCost: number;
    breakEvenPoint: number;
    automationBreakdown: CostBreakdown;
    manualBreakdown: CostBreakdown;
}

export interface ROIAnalysis {
    roiValue: number;
    paybackPeriod: number;
    netPresentValue: number;
    assumptions: string[];
}

export interface AutomationDecision {
    decision: string;
    confidence: number;
    rationale: string[];
    factors: DecisionFactor[];
}

export interface ResourceRequirement {
    resourceType: string;
    quantity: number;
    duration: string; // ISO 8601 duration
}

export interface Milestone {
    name: string;
    date: Date;
    deliverables: string[];
}

export interface Timeline {
    startDate: Date;
    endDate: Date;
    milestones: Milestone[];
}

export interface AutomationPlan {
    phases: string[];
    resources: ResourceRequirement[];
    timeline: Timeline;
}

export interface MetricDefinition {
    name: string;
    collectionMethod: string;
    frequency: string;
}

export interface AlertThreshold {
    metric: string;
    warningThreshold: number;
    criticalThreshold: number;
}

export interface ReviewSchedule {
    frequency: string;
    responsible: string;
}

export interface MonitoringPlan {
    metrics: MetricDefinition[];
    alerts: AlertThreshold[];
    reviews: ReviewSchedule;
}

export interface AutomationOption {
    name: string;
    decision: AutomationDecision;
    rationale: string;
}

export interface AutomationDecisionResponse {
    decisionId: string;
    testScenario: TestScenario;
    decision: AutomationDecision;
    costAnalysis: CostAnalysis;
    roi: ROIAnalysis;
    implementationPlan?: AutomationPlan;
    monitoringPlan: MonitoringPlan;
    alternativeOptions: AutomationOption[];
    reviewTimeline: Timeline;
}

export interface OptimizationAction {
    type: string;
    affectedTests: string[];
    rationale: string;
    impact: number;
}

export interface OptimizationResult {
    actions: OptimizationAction[];
    optimizedSuite: TestSuite;
    metrics: { [key: string]: number };
}

export interface SavingsBreakdown {
    executionSavings: number;
    maintenanceSavings: number;
    reportingSavings: number;
}

export interface MaintenanceSavings {
    maintenanceReduction: number;
    costSavings: number;
    timeSavingsHours: number;
    breakdown: SavingsBreakdown;
}

export interface ImplementationGuidance {
    steps: string[];
    risks: string[];
    prerequisites: string[];
}

export interface ValidationResult {
    isValid: boolean;
    violations: string[];
    warnings: string[];
}

export interface ImprovementPlan {
    recommendations: string[];
    timeline: Timeline;
    successCriteria: string[];
}

export interface MaintenanceOptimizationResponse {
    optimizationId: string;
    originalTests: TestSuite;
    optimization: OptimizationResult;
    savings: MaintenanceSavings;
    implementationGuidance: ImplementationGuidance;
    preservationValidation: ValidationResult;
    riskAssessment: RiskAssessment;
    monitoringPlan: MonitoringPlan;
    continuousImprovement: ImprovementPlan;
}

export interface CostBenefitItem {
    category: string;
    amount: number;
    percentage: number;
}

export interface TangibleAnalysis {
    totalCost: number;
    totalBenefit: number;
    roi: number;
    costBreakdown: CostBenefitItem[];
    benefitBreakdown: CostBenefitItem[];
}

export interface IntangibleAnalysis {
    qualityScore: number;
    teamMorale: number;
    customerSatisfaction: number;
    qualitativeBenefits: string[];
}

export interface OverallROI {
    roiValue: number;
    paybackPeriod: number;
    netPresentValue: number;
    confidence: string;
}

export interface Insight {
    title: string;
    description: string;
    category: string;
    impact: number;
}

export interface Recommendation {
    title: string;
    description: string;
    expectedImpact: number;
    priority: string;
}

export interface BenchmarkComparison {
    industryAverage: number;
    bestInClass: number;
    percentile: number;
}

export interface ROIForecast {
    projectedROI: number;
    trends: string[];
    recommendations: string[];
}

export interface VisualizationData {
    timeSeries: { [key: string]: number[] };
    distribution: { [key: string]: number };
    chartConfig: string;
}

export interface ROIAnalysisResponse {
    analysisId: string;
    measurementPeriod: string;
    tangibleAnalysis: TangibleAnalysis;
    intangibleAnalysis?: IntangibleAnalysis;
    overallROI: OverallROI;
    insights: Insight[];
    recommendations: Recommendation[];
    benchmarkComparison: BenchmarkComparison;
    forecasting: ROIForecast;
    visualizationData: VisualizationData;
}

// models/error.model.ts
export interface LeanDiagnosticData {
    constraintAnalysis?: any;
    suggestedAdjustments?: any;
    coverageGapAnalysis?: any;
    constraintImpact?: any;
    missingCostData?: any;
    estimationChallenges?: any;
    violatedRules?: any;
    affectedTests?: any;
    dataGaps?: any;
    calculationLimitations?: any;
}

export interface LeanErrorResponse {
    context: string;
    errorType: string;
    leanPrincipleViolated: string;
    message: string;
    recoverySteps: string[];
    fallbackSuggestion: string;
    diagnosticData?: LeanDiagnosticData;
}