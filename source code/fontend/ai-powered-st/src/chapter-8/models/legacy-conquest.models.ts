// models/legacy-conquest.models.ts

// ============= REQUEST MODELS =============

export interface LegacyAnalysisRequest {
    codebase: CodebaseInfo;
    analysisDepth: string; // "quick", "standard", "comprehensive"
    businessContext: BusinessContext;
    safetyLevel?: string; // "conservative", "balanced", "aggressive"
    focusAreas?: string[];
}

export interface CodebaseInfo {
    name: string;
    technologyStack: string[];
    ageYears: number;
    totalLines: number;
    complexityScore: number;
    dependencies?: Dependency[];
}

export interface Dependency {
    name: string;
    version: string;
    isExternal: boolean;
}

export interface BusinessContext {
    criticalFlows: CriticalFlow[];
    stakeholderConcerns?: string[];
    businessRules?: Record<string, string>;
}

export interface CriticalFlow {
    id: string;
    description: string;
    businessValue: number;
    involvedSystems?: string[];
}

export interface WrapperRequest {
    legacyModule: LegacyModule;
    wrapperType: string; // "facade", "adapter", "proxy", "strangler"
    safetyLevel: string; // "conservative", "balanced", "aggressive"
    safetyMeasures?: SafetyMeasure[];
    validationRequirements?: ValidationRequirement[];
    modernizationStrategy?: string; // "strangler-fig", "branch-by-abstraction", "parallel-run"
}

export interface LegacyModule {
    name: string;
    version: string;
    exposedFunctions: string[];
    complexityScore: number;
    configuration?: Record<string, string>;
}

export interface SafetyMeasure {
    type: string; // "circuit-breaker", "retry", "timeout", "bulkhead"
    configuration: string;
}

export interface ValidationRequirement {
    name: string;
    validationType: string; // "input", "output", "state", "integration"
    isMandatory: boolean;
}

export interface CharacterizationRequest {
    observedOutputs: ObservedOutput[];
    legacyBehavior: LegacyBehavior;
    coverageGoal: number; // 0.0 to 1.0
    includeEdgeCases: boolean;
    testStrategy: string; // "record-replay", "property-based", "golden-master"
    generateDocumentation: boolean;
}

export interface ObservedOutput {
    input: string;
    output: string;
    timestamp: Date;
    context?: string;
}

export interface LegacyBehavior {
    id: string;
    description: string;
    category: string;
    knownVariations?: string[];
}

export interface RoadmapRequest {
    legacyAnalysis: LegacyAnalysis;
    businessPriorities: BusinessPriority[];
    modernizationApproach: string; // "big-bang", "incremental", "strangler"
    constraints: PipelineConstraints;
    successMetrics: SuccessMetric[];
}

export interface LegacyAnalysis {
    analysisId: string;
    analysisDate: Date;
    codebaseInfo: CodebaseInfo;
    metrics: AnalysisMetrics;
    findings: string[];
    recommendations: string[];
    rawData?: Record<string, any>;
}

export interface AnalysisMetrics {
    totalFiles: number;
    totalLinesOfCode: number;
    totalClasses: number;
    totalMethods: number;
    averageComplexity: number;
    maxComplexity: number;
    totalDependencies: number;
    circularDependencies: number;
    codeSmellsCount: number;
    technicalDebtRatio: number;
    testCoverage: number;
    securityVulnerabilities: number;
}

export interface BusinessPriority {
    id: string;
    name: string;
    priority: number; // 1-10
    dependentSystems?: string[];
}

export interface PipelineConstraints {
    maxDuration: string;
    maxCostPerRun: number;
    maxParallelWorkers: number;
    allowedDowntimeWindows: string[];
}

export interface SuccessMetric {
    name: string;
    targetValue: string;
    measurementMethod: string;
}

export interface HealthRequest {
    monitoredSystems: MonitoredSystem[];
    telemetryData: TelemetryDataPoint[];
    healthIndicators?: HealthIndicator[];
    alertThresholds?: AlertThreshold[];
}

export interface MonitoredSystem {
    id: string;
    name: string;
    type: string;
    dependencies?: string[];
}

export interface TelemetryDataPoint {
    systemId: string;
    timestamp: Date;
    metricName: string;
    value: number;
    tags?: Record<string, string>;
}

export interface HealthIndicator {
    name: string;
    metricSource: string;
    warningThreshold: number;
    criticalThreshold: number;
}

export interface AlertThreshold {
    indicator: string;
    level: string; // "warning", "critical"
    consecutiveOccurrences: number;
    timeWindow: string; // ISO duration format
}

// ============= RESPONSE MODELS =============

export interface LegacyAnalysisResponse {
    analysisId: string;
    codebaseSummary: CodebaseSummary;
    businessLogicMap: BusinessLogicMap[];
    riskHotspots: RiskHotspot[];
    hiddenAssumptions: HiddenAssumption[];
    modernizationReadiness: ModernizationReadiness;
    recommendedActions: RecommendedAction[];
    confidenceScores: ConfidenceScore[];
    nextSteps: NextStep[];
}

export interface CodebaseSummary {
    name: string;
    totalLines: number;
    complexityScore: number;
    primaryTechnologies: string[];
    dependencyCount: number;
    technicalDebtEstimate: number;
}

export interface BusinessLogicMap {
    businessFlowId: string;
    businessFlowDescription: string;
    codeLocations: string[];
    mappingConfidence: number;
    gaps?: string[];
}

export interface RiskHotspot {
    id: string;
    location: string;
    riskType: string;
    severity: number; // 1-10
    description: string;
    mitigationStrategies?: string[];
}

export interface HiddenAssumption {
    id: string;
    description: string;
    location: string;
    impact: string;
    isValidated: boolean;
}

export interface ModernizationReadiness {
    readinessScore: number; // 0-1
    strengths: string[];
    weaknesses: string[];
    opportunities: string[];
    threats: string[];
}

export interface RecommendedAction {
    id: string;
    title: string;
    description: string;
    priority: number;
    estimatedEffort: string;
    dependencies?: string[];
}

export interface ConfidenceScore {
    metric: string;
    score: number; // 0-1
    explanation: string;
}

export interface NextStep {
    step: string;
    owner: string;
    timeline: string;
}

export interface WrapperGenerationResponse {
    wrapperId: string;
    originalModule: LegacyModule;
    generatedWrapper: GeneratedWrapper;
    validationTests: ValidationTest[];
    migrationPlan: MigrationPlan;
    safetyAssessment: SafetyAssessment;
    performanceImpact: PerformanceImpact;
    rollbackStrategy: RollbackStrategy;
}

export interface GeneratedWrapper {
    name: string;
    code: string;
    language: string;
    safetyFeatures: SafetyFeature[];
    exposedInterfaces: string[];
}

export interface SafetyFeature {
    name: string;
    type: string;
    configuration: string;
}

export interface ValidationTest {
    id: string;
    name: string;
    testCode: string;
    expectedResult: string;
}

export interface MigrationPlan {
    strategy: string;
    phases: Phase[];
    estimatedDuration: string;
}

export interface Phase {
    name: string;
    steps: string[];
    successCriteria: string;
}

export interface SafetyAssessment {
    safetyScore: number;
    coveredRisks: string[];
    uncoveredRisks: string[];
    recommendations: string[];
}

export interface PerformanceImpact {
    latencyIncrease: number;
    memoryOverhead: number;
    throughputImpact: number;
    bottlenecks: string[];
}

export interface RollbackStrategy {
    trigger: string;
    steps: string[];
    estimatedRevertTime: string;
    dataPreservationSteps: string[];
}

export interface CharacterizationResponse {
    testSuiteId: string;
    legacyBehavior: LegacyBehavior;
    characterizationTests: CharacterizationTest[];
    documentation?: TestDocumentation;
    testHarness: TestHarness;
    validationSuite: ValidationSuite;
    coverageReport: CoverageReport;
    confidenceMetrics: ConfidenceMetric[];
    maintenanceGuide: MaintenanceGuide;
}

export interface CharacterizationTest {
    id: string;
    name: string;
    testCode: string;
    inputs: string[];
    expectedOutput: string;
    category: string;
}

export interface TestDocumentation {
    overview: string;
    scenarios: TestScenario[];
    assumptions: string[];
    limitations: string[];
}

export interface TestScenario {
    name: string;
    description: string;
    coveredBehaviors: string[];
}

export interface TestHarness {
    setupCode: string;
    teardownCode: string;
    requiredMocks: string[];
    configuration: Record<string, string>;
}

export interface ValidationSuite {
    validationRules: string[];
    validationTests: string[];
    validationCoverage: number;
}

export interface CoverageReport {
    coveragePercentage: number; // 0-1
    coveredBehaviors: string[];
    uncoveredBehaviors: string[];
    recommendations: string[];
}

export interface ConfidenceMetric {
    metric: string;
    value: number; // 0-1
    justification: string;
}

export interface MaintenanceGuide {
    commonIssues: string[];
    troubleshootingSteps: string[];
    updateProcedures: string[];
}

export interface ModernizationResponse {
    roadmapId: string;
    modernizationApproach: string;
    roadmap: ModernizationRoadmap;
    riskAssessment: RiskAssessment;
    implementationPlan: ImplementationPlan;
    successMetrics: SuccessMetrics;
    stakeholderCommunication: StakeholderCommunication;
    monitoringPlan: MonitoringPlan;
    contingencyPlans: ContingencyPlan[];
}

export interface ModernizationRoadmap {
    phases: RoadmapPhase[];
    totalDuration: string;
    milestones: string[];
    dependencies: Record<string, string>;
}

export interface RoadmapPhase {
    name: string;
    description: string;
    duration: string;
    steps: string[];
    deliverables: string[];
}

export interface RiskAssessment {
    identifiedRisks: Risk[];
    overallRiskLevel: string;
    mitigationPlans: MitigationPlan[];
}

export interface Risk {
    name: string;
    description: string;
    likelihood: string;
    impact: string;
}

export interface MitigationPlan {
    riskName: string;
    strategy: string;
    actions: string[];
}

export interface ImplementationPlan {
    tasks: Task[];
    resourceRequirements: string[];
    dependencies: Record<string, string>;
}

export interface Task {
    id: string;
    description: string;
    assignedTo?: string;
    estimatedHours: string;
    prerequisites?: string[];
}

export interface SuccessMetrics {
    id: string;
    name: string;
    description: string;
    createdAt: Date;
    lastUpdatedAt?: Date;
    metrics: MetricDefinition[];
    overallAssessment: OverallAssessment;
    recommendations: string[];
}

export interface MetricDefinition {
    id: string;
    name: string;
    category: string; // "technical", "business", "process", "quality"
    description: string;
    measurementMethod: string;
    targetValue: string;
    currentValue: string;
    unit: string;
    baselineValue?: number;
    thresholdWarning?: number;
    thresholdCritical?: number;
    status: string; // "on-track", "at-risk", "critical", "completed"
    trend: Trend;
    historicalData?: HistoricalDataPoint[];
    metadata?: Record<string, string>;
}

export interface Trend {
    direction: string; // "improving", "stable", "degrading"
    changeRate: number;
    interpretation: string;
    periodStart: Date;
    periodEnd: Date;
}

export interface HistoricalDataPoint {
    timestamp: Date;
    value: number;
    annotation?: string;
}

export interface OverallAssessment {
    overallScore: number;
    grade: string;
    summary: string;
    keyAchievements: string[];
    areasForImprovement: string[];
    riskAssessment: RiskAssessment;
    categoryScores: Record<string, number>;
}

export interface StakeholderCommunication {
    stakeholders: string[];
    communicationPlans: CommunicationPlan[];
    keyMessages: string[];
}

export interface CommunicationPlan {
    audience: string;
    frequency: string;
    format: string;
    content: string;
}

export interface MonitoringPlan {
    metrics: Metric[];
    alertRules: string[];
    reviewFrequency: string;
}

export interface Metric {
    name: string;
    target: string;
    measurementMethod: string;
}

export interface ContingencyPlan {
    trigger: string;
    actions: string[];
    owner: string;
}

export interface HealthResponse {
    healthReportId: string;
    monitoredSystems: MonitoredSystem[];
    healthScores: HealthScore[];
    detectedAnomalies: Anomaly[];
    predictions: Prediction[];
    recommendations: Recommendation[];
    alertStatus: AlertStatus;
    trendAnalysis: TrendAnalysis;
    actionableInsights: ActionableInsight[];
}

export interface HealthScore {
    systemId: string;
    systemName: string;
    score: number; // 0-100
    status: string; // "healthy", "warning", "critical"
    componentScores: Record<string, number>;
}

export interface Anomaly {
    id: string;
    systemId: string;
    detectedAt: Date;
    metric: string;
    expectedValue: number;
    actualValue: number;
    severity: string;
    description: string;
}

export interface Prediction {
    id: string;
    systemId: string;
    predictedIssue: string;
    predictedTimeframe: Date;
    confidence: number; // 0-1
    suggestedActions: string[];
}

export interface Recommendation {
    id: string;
    title: string;
    description: string;
    impact: string;
    effort: string;
}

export interface AlertStatus {
    level: string; // "none", "warning", "critical"
    activeAlerts: Alert[];
    recommendedResponses: string[];
}

export interface Alert {
    id: string;
    systemId: string;
    type: string;
    message: string;
    triggeredAt: Date;
}

export interface TrendAnalysis {
    increasingTrends: Trend[];
    decreasingTrends: Trend[];
    stableTrends: Trend[];
    seasonalPatterns?: SeasonalPattern[];
    anomalyTrends?: AnomalyTrend[];
    correlations?: Correlation[];
}

export interface SeasonalPattern {
    metric: string;
    pattern: string; // "daily", "weekly", "monthly", "quarterly"
    confidence: number;
    typicalRanges: Record<string, number>;
}

export interface AnomalyTrend {
    metric: string;
    anomalyCount: number;
    commonTypes: string[];
    affectedSystems: string[];
    frequencyTrend: number;
}

export interface Correlation {
    metric1: string;
    metric2: string;
    correlationCoefficient: number; // -1 to 1
    relationship: string; // "positive", "negative", "none"
    description: string;
}

export interface ActionableInsight {
    category: string;
    insight: string;
    recommendedAction: string;
    priority: string;
}

// ============= ERROR MODELS =============

export interface LegacyErrorResponse {
    errorType: string;
    message: string;
    recoverySteps?: string[];
    fallbackSuggestion?: string;
    diagnosticData?: LegacyDiagnosticData;
}

export interface LegacyDiagnosticData {
    problematicComplexity?: number;
    suggestedSimplification?: string;
    problematicModule?: string;
    complexityIssues?: string[];
    ambiguityAreas?: string[];
    suggestedClarifications?: string[];
    conflictingConstraints?: string[];
    suggestedAdjustments?: string[];
    inconsistencyDetails?: string;
    dataQualityIssues?: string[];
}