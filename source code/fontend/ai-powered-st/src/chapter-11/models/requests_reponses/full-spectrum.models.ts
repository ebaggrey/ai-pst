// models/full-spectrum.models.ts
export namespace FullSpectrumModels {
  
  // ==================== ENUMS ====================
  
  export enum CollaborationMode {
    Synchronous = 'Synchronous',
    Asynchronous = 'Asynchronous',
    Hybrid = 'Hybrid'
  }

  export enum AnalysisDepth {
    Basic = 'Basic',
    Detailed = 'Detailed',
    Comprehensive = 'Comprehensive'
  }

  export enum DeploymentEnvironment {
    Development = 'Development',
    Staging = 'Staging',
    Production = 'Production',
    Canary = 'Canary'
  }

  export enum GateAction {
    Proceed = 'Proceed',
    Warn = 'Warn',
    Block = 'Block'
  }

  export enum SpectrumCoverage {
    LeftOnly = 'LeftOnly',
    RightOnly = 'RightOnly',
    FullSpectrum = 'FullSpectrum',
    Custom = 'Custom'
  }

  export enum OrchestrationStrategy {
    Sequential = 'Sequential',
    Parallel = 'Parallel',
    Adaptive = 'Adaptive',
    PriorityBased = 'PriorityBased'
  }

  export enum FailureResponseBehavior {
    Stop = 'Stop',
    Continue = 'Continue',
    ReportOnly = 'ReportOnly',
    Retry = 'Retry'
  }

  // ==================== REQUEST MODELS ====================

  // ShiftLeft Request Models
  export interface ShiftLeftRequest {
    requirements: RequirementCollection;
    designDocuments: DesignDocument[];
    shiftDepth: number;
    collaborationMode: CollaborationMode;
  }

  export interface RequirementCollection {
    items: Requirement[];
    stakeholders: Stakeholder[];
  }

  export interface Requirement {
    id: string;
    description: string;
    testability: number;
    acceptanceCriteria: string[];
  }

  export interface Stakeholder {
    id: string;
    name: string;
    role: string;
  }

  export interface DesignDocument {
    id: string;
    name: string;
    content: string;
    documentType: string;
  }

  // Testability Request Models
  export interface TestabilityRequest {
    codebase: Codebase;
    testabilityFramework: TestabilityFramework;
    analysisDepth: AnalysisDepth;
    improvementSuggestions: boolean;
    refactoringRecommendations: boolean;
  }

  export interface Codebase {
    repositoryUrl: string;
    branch: string;
    totalLines: number;
    files: CodeFile[];
  }

  export interface CodeFile {
    path: string;
    content: string;
    language: string;
  }

  export interface TestabilityFramework {
    name: string;
    version: string;
    supportedLanguages: string[];
  }

  // ShiftRight Request Models
  export interface ShiftRightRequest {
    productionSystem: ProductionSystem;
    userBehavior: UserBehavior;
    monitoringObjectives: MonitoringObjective[];
    feedbackLoops: FeedbackLoop[];
  }

  export interface ProductionSystem {
    id: string;
    name: string;
    components: ProductionComponent[];
    environment: DeploymentEnvironment;
  }

  export interface ProductionComponent {
    id: string;
    name: string;
    type: string;
    dependencies: string[];
  }

  export interface UserBehavior {
    patterns: UserPattern[];
    segments: UserSegment[];
  }

  export interface UserPattern {
    name: string;
    description: string;
    frequency: number;
  }

  export interface UserSegment {
    id: string;
    criteria: string;
  }

  export interface MonitoringObjective {
    id: string;
    name: string;
    metric: string;
    threshold: number;
  }

  export interface FeedbackLoop {
    id: string;
    name: string;
    sourceComponent: string;
    targetComponent: string;
  }

  // Pipeline Request Models
  export interface PipelineRequest {
    developmentStages: DevelopmentStage[];
    qualityGates: QualityGate[];
    spectrumCoverage: SpectrumCoverage;
    feedbackMechanisms: FeedbackMechanism[];
  }

  export interface DevelopmentStage {
    id: string;
    name: string;
    activities: string[];
    dependencies: string[];
  }

  export interface QualityGate {
    id: string;
    name: string;
    conditions: string[];
    action: GateAction;
  }

  export interface FeedbackMechanism {
    id: string;
    type: string;
    channel: string;
  }

  // Orchestration Request Models
  export interface OrchestrationRequest {
    testSuite: TestSuite;
    executionContext: ExecutionContext;
    orchestrationStrategy: OrchestrationStrategy;
    failureResponse: FailureResponseBehavior;
  }

  export interface TestSuite {
    id: string;
    name: string;
    tests: Test[];
  }

  export interface Test {
    id: string;
    name: string;
    type: string;
    tags: string[];
  }

  export interface ExecutionContext {
    environment: string;
    capabilities: ExecutionCapability;
    timeout: string; // ISO 8601 duration format
  }

  export interface ExecutionCapability {
    maxParallelTests: number;
    supportedEnvironments: string[];
  }



// ==================== RESPONSE MODELS ====================

  // ShiftLeft Response Models
  export interface ShiftLeftResponse {
    artifactsId: string;
    requirements: RequirementCollection;
    acceptanceCriteria: AcceptanceCriteria[];
    testScenarios: TestScenario[];
    testDataRequirements: TestDataRequirement[];
    riskAssessment: RiskAssessment;
    collaborationHistory: CollaborationHistory;
    coverageMetrics: CoverageMetrics;
    implementationGuidance: ImplementationGuidance;
    validationChecklist: ValidationChecklist;
  }

  export interface AcceptanceCriteria {
    id: string;
    requirementId: string;
    criterion: string;
    isAutomated: boolean;
  }

  export interface TestScenario {
    id: string;
    name: string;
    steps: string[];
    expectedOutcome: string;
    tags: string[];
  }

  export interface TestDataRequirement {
    id: string;
    testScenarioId: string;
    dataType: string;
    sampleData: any;
  }

  export interface RiskAssessment {
    id: string;
    highRisks: RiskItem[];
    mediumRisks: RiskItem[];
    lowRisks: RiskItem[];
  }

  export interface RiskItem {
    description: string;
    probability: number;
    impact: number;
    mitigation: string;
  }

  export interface CollaborationHistory {
    mode: string;
    entries: CollaborationEntry[];
  }

  export interface CollaborationEntry {
    timestamp: Date;
    participant: string;
    action: string;
  }

  export interface CoverageMetrics {
    requirementCoverage: number;
    scenarioCoverage: number;
    riskCoverage: number;
  }

  export interface ImplementationGuidance {
    steps: string[];
    bestPractices: string[];
    warnings: string[];
  }

  export interface ValidationChecklist {
    items: ChecklistItem[];
  }

  export interface ChecklistItem {
    description: string;
    isRequired: boolean;
  }

  // Testability Response Models
  export interface TestabilityResponse {
    analysisId: string;
    codebase: Codebase;
    analysis: TestabilityAnalysis;
    testabilityScore: TestabilityScore;
    improvements: Improvement[];
    refactoringRecommendations: RefactoringRecommendation[];
    impactAssessment: ImpactAssessment;
    implementationRoadmap: ImplementationRoadmap;
    monitoringPlan: MonitoringPlan;
  }

  export interface TestabilityAnalysis {
    id: string;
    codeSmells: CodeSmell[];
    dependencyIssues: DependencyIssue[];
    complexityMetrics: ComplexityMetric[];
  }

  export interface CodeSmell {
    type: string;
    location: string;
    description: string;
    severity: string;
  }

  export interface DependencyIssue {
    dependency: string;
    issue: string;
    recommendation: string;
  }

  export interface ComplexityMetric {
    name: string;
    value: number;
    threshold: number;
    exceedsThreshold: boolean;
  }

  export interface TestabilityScore {
    score: number;
    componentScores: ComponentScore[];
  }

  export interface ComponentScore {
    componentName: string;
    score: number;
  }

  export interface Improvement {
    id: string;
    description: string;
    category: string;
    estimatedEffort: number;
    impact: number;
  }

  export interface RefactoringRecommendation {
    id: string;
    location: string;
    currentPattern: string;
    recommendedPattern: string;
    steps: string[];
  }

  export interface ImpactAssessment {
    assessment: string;
    areas: ImpactArea[];
  }

  export interface ImpactArea {
    name: string;
    impact: string;
    confidence: number;
  }

  export interface ImplementationRoadmap {
    phases: RoadmapPhase[];
  }

  export interface RoadmapPhase {
    name: string;
    tasks: string[];
    duration: string;
  }

  export interface MonitoringPlan {
    metrics: MonitoringMetric[];
  }

  export interface MonitoringMetric {
    name: string;
    collectionMethod: string;
    target: number;
  }

  // ShiftRight Response Models
  export interface ShiftRightResponse {
    monitorsId: string;
    productionSystem: ProductionSystem;
    monitors: Monitor[];
    feedbackLoops: FeedbackLoop[];
    incidentResponse: IncidentResponse;
    coverageAssessment: MonitoringCoverage;
    alertConfiguration: AlertConfiguration;
    integrationPlan: IntegrationPlan;
    costBenefitAnalysis: CostBenefitAnalysis;
  }

  export interface Monitor {
    id: string;
    name: string;
    type: string;
    configuration: string;
    targets: string[];
  }

  export interface IncidentResponse {
    rules: IncidentRule[];
    actionPlans: ActionPlan[];
  }

  export interface IncidentRule {
    condition: string;
    severity: string;
    action: string;
  }

  export interface ActionPlan {
    name: string;
    steps: string[];
  }

  export interface MonitoringCoverage {
    overallCoverage: number;
    componentCoverage: ComponentCoverage[];
  }

  export interface ComponentCoverage {
    componentName: string;
    coverage: number;
  }

  export interface AlertConfiguration {
    channels: AlertChannel[];
    rules: AlertRule[];
  }

  export interface AlertChannel {
    type: string;
    destination: string;
  }

  export interface AlertRule {
    name: string;
    condition: string;
    severity: string;
  }

  export interface IntegrationPlan {
    steps: IntegrationStep[];
  }

  export interface IntegrationStep {
    order: number;
    description: string;
  }

  export interface CostBenefitAnalysis {
    estimatedCost: number;
    estimatedBenefit: number;
    roi: number;
  }

  // Pipeline Response Models
  export interface PipelineResponse {
    pipelineId: string;
    pipeline: Pipeline;
    feedbackConfiguration: FeedbackConfiguration;
    implementationPlan: ImplementationPlan;
    spectrumCoverage: SpectrumCoverage;
    performanceProjections: PerformanceProjection;
    riskAssessment: PipelineRiskAssessment;
    optimizationRecommendations: OptimizationRecommendation[];
  }

  export interface Pipeline {
    stages: PipelineStage[];
    qualityGates: QualityGate[];
  }

  export interface PipelineStage {
    id: string;
    name: string;
    activities: string[];
    metrics: string[];
  }

  export interface FeedbackConfiguration {
    channels: FeedbackChannel[];
  }

  export interface FeedbackChannel {
    type: string;
    configuration: string;
  }

  export interface ImplementationPlan {
    tasks: ImplementationTask[];
  }

  export interface ImplementationTask {
    id: string;
    description: string;
    order: number;
  }

  export interface PerformanceProjection {
    expectedThroughput: number;
    expectedDuration: string;
    successRate: number;
  }

  export interface PipelineRiskAssessment {
    risks: RiskItem[];
  }

  export interface OptimizationRecommendation {
    area: string;
    recommendation: string;
    impact: number;
  }

  // Orchestration Response Models
  export interface OrchestrationResponse {
    orchestrationId: string;
    testSuite: TestSuite;
    orchestration: Orchestration;
    executionResults: ExecutionResult[];
    processedResults: ProcessedResult[];
    feedback: OrchestrationFeedback;
    performanceMetrics: PerformanceMetrics;
    improvementRecommendations: ImprovementRecommendation[];
    documentationUpdates: DocumentationUpdate[];
  }

  export interface Orchestration {
    id: string;
    strategy: OrchestrationStrategy;
    executionPlans: TestExecutionPlan[];
  }

  export interface TestExecutionPlan {
    testId: string;
    order: number;
    dependencies: string[];
  }

  export interface ExecutionResult {
    testId: string;
    status: string;
    duration: string;
    errorMessage?: string;
  }

  export interface ProcessedResult {
    testId: string;
    output: any;
    issues: string[];
  }

  export interface OrchestrationFeedback {
    summary: string;
    items: FeedbackItem[];
  }

  export interface FeedbackItem {
    type: string;
    message: string;
  }

  export interface PerformanceMetrics {
    totalDuration: string;
    totalTests: number;
    passedTests: number;
    failedTests: number;
  }

  export interface ImprovementRecommendation {
    category: string;
    recommendation: string;
    impact: number;
  }

  export interface DocumentationUpdate {
    testId: string;
    field: string;
    update: string;
  }

  // Error Response Model
  export interface SpectrumErrorResponse {
    context: string;
    errorType: string;
    spectrumLocation: string;
    message: string;
    recoverySteps: string[];
    fallbackSuggestion: string;
    diagnosticData?: SpectrumDiagnosticData;
  }

  export interface SpectrumDiagnosticData {
    ambiguousRequirements?: string[];
    clarificationQuestions?: string[];
    frameworkIssues?: string[];
    technologyMismatches?: string[];
    complexityIssues?: string[];
    recommendedSimplifications?: string[];
    conflictingStages?: string[];
    dependencyIssues?: string[];
    complexityFactors?: string[];
    simplificationSuggestions?: string[];
  }



  
}