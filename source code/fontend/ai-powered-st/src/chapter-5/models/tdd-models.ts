// models/tdd-models.ts

// Enums
export enum TddStyle {
  Classic = 'classic',
  OutsideIn = 'outside-in',
  InsideOut = 'inside-out',
  London = 'london',
  Chicago = 'chicago'
}

export enum TimeHorizon {
  Immediate = 'immediate',
  Weekly = 'weekly',
  Monthly = 'monthly',
  Quarterly = 'quarterly',
  Yearly = 'yearly'
}

export enum ImplementationStrategy {
  SimplestFirst = 'simplest-first',
  MostReadable = 'most-readable',
  MostPerformant = 'most-performant',
  MostExtensible = 'most-extensible'
}

// Request Models
export interface TDDRequest {
  userStory: UserStory;
  tddStyle: TddStyle | string;
  constraints: Constraint[];
  generateMultipleApproaches?: boolean;
  maxComplexityLevel?: number;
}

export interface ImplementationRequest {
  failingTest: GeneratedTest;
  failureDetails: FailureDetails;
  implementationStrategy: ImplementationStrategy | string;
  constraints: Constraint[];
  allowMultipleImplementations?: boolean;
}

export interface RefactorRequest {
  workingCode: CodeSnippet;
  testSuite: TestSuite;
  refactoringGoals: RefactoringGoal[];
  safetyMeasures: SafetyMeasures;
  constraints: Constraint[];
}

export interface FuturePredictionRequest {
  currentCode: CodeSnippet;
  productRoadmap: ProductRoadmap;
  timeHorizon: TimeHorizon | string;
  confidenceThreshold: number;
  focusAreas: string[];
}

// Response Models
export interface TDDCycleResponse {
  cycleId: string;
  phase: string;
  generatedTest: GeneratedTest;
  implementationSuggestions: ImplementationSuggestion[];
  refactoringHints: RefactoringHint[];
  estimatedTimeline: string; // ISO string
  confidenceMetrics: ConfidenceMetrics;
  learningPoints: LearningPoint[];
  nextSteps: string[];
}

export interface ImplementationResponse {
  cycleId: string;
  implementations: TestedImplementation[];
  recommendedImplementation: TestedImplementation;
  alternativeApproaches: TestedImplementation[];
  codeSmellAnalysis: CodeSmellAnalysis;
  refactoringOpportunities: RefactoringOpportunity[];
  nextTDDCycle: TDDCycle;
  diagnostic?: ImplementationDiagnostic;
  suggestedNextStep?: string;
}

export interface RefactorResponse {
  cycleId: string;
  completedSteps: RefactoringStepResult[];
  originalCode: CodeSnippet;
  refactoredCode: CodeSnippet;
  improvementMetrics: ImprovementMetrics;
  testSafetyReport: TestSafetyReport;
  futureMaintenanceImpact: MaintenanceImpact;
  additionalRefactoringOpportunities: RefactoringOpportunity[];
  failedStep?: RefactoringStep;
  failureAnalysis?: FailureAnalysis;
  rollbackSuggestion?: RollbackSuggestion;
  safeToContinue?: boolean;
}

export interface FuturePredictionResponse {
  predictionId: string;
  timeHorizon: TimeHorizon | string;
  changePredictions: ChangePrediction[];
  futureTestRecommendations: FutureTestRecommendation[];
  prioritizedRecommendations: { [key: string]: FutureTestRecommendation[] };
  confidenceSummary: ConfidenceSummary;
  implementationTimeline: ImplementationTimeline;
  riskMitigationStrategies: RiskMitigationStrategy[];
}

export interface TDDErrorResponse {
  phase: string;
  errorType: string;
  message: string;
  recoveryStrategy: string[];
  suggestedFallback?: string;
  learningOpportunity?: string;
}

// Domain Models
export interface UserStory {
  id: string;
  title: string;
  description: string;
  acceptanceCriteria: AcceptanceCriteria[];
  businessRules: string[];
  examples: Example[];
}

export interface GeneratedTest {
  testCode: string;
  testFramework: string;
  testName: string;
  dependencies: string[];
  isFailingByDesign: boolean;
  expectedFailure: FailureDetails;
}

export interface CodeSnippet {
  id: string;
  language: string;
  code: string;
  dependencies: Dependency[];
  complexityMetrics: ComplexityMetrics;
}

export interface TDDCycle {
  id: string;
  phase: string;
  description: string;
  estimatedDuration: string; // ISO string
  focusAreas: string[];
  test?: GeneratedTest;
  implementation?: CodeSnippet;
  refactoringSteps?: RefactoringStep[];
  createdAt: string; // ISO string
  completedAt?: string; // ISO string
  status: string;
}

export interface TestedImplementation {
  implementation: CodeSnippet;
  testResults: TestResult[];
  analysis: ImplementationAnalysis;
  passesAllTests: boolean;
  qualityScore: number;
}

export interface RefactoringStepResult {
  step: RefactoringStep;
  successful: boolean;
  resultingCode: string;
  result: string;
  codeAfterStep: CodeSnippet;
  duration: string; // ISO string
  testResults: TestResult[];
  failureReason?: string;
  failureAnalysis?: FailureAnalysis;
  safeToContinue: boolean;
}

// Value Objects
export interface Constraint {
  type: string;
  value: string;
  description: string;
}

export interface AcceptanceCriteria {
  id: string;
  description: string;
  testConditions: string[];
}

export interface RefactoringGoal {
  type: string;
  description: string;
  priority: number;
}

export interface SafetyMeasures {
  preserveBehavior: boolean;
  createCheckpoints: boolean;
  suggestRollbackPoints: boolean;
  maxStepsWithoutCommit: number;
}

export interface FailureDetails {
  expected: string;
  actual: string;
  message: string;
}

export interface TestValidation {
  test: GeneratedTest;
  isFailingByDesign: boolean;
  failureDetails: FailureDetails;
  issues: string[];
}

export interface ImplementationSuggestion {
  id: string;
  approach: string;
  codeSnippet: CodeSnippet;
  explanation: string;
  tradeoffs: string[];
}

// Additional Domain Models
export interface TestResult {
  testName: string;
  passed: boolean;
  errorMessage?: string;
  duration: string; // ISO string
  assertionFailures: AssertionFailure[];
}

export interface TestSuite {
  name: string;
  tests: GeneratedTest[];
  framework: string;
  lastRun: string; // ISO string
  passRate: number;
  coverage: TestCoverage;
}

export interface AssertionFailure {
  expected: string;
  actual: string;
  message: string;
  stackTrace: string;
}

export interface ComplexityMetrics {
  cyclomaticComplexity: number;
  linesOfCode: number;
  methodCount: number;
  depthOfInheritance: number;
  classCoupling: number;
}

export interface Dependency {
  name: string;
  version: string;
  type: string;
}

export interface Example {
  scenario: string;
  given: string;
  when: string;
  then: string;
  expectedResult: string;
}

// Analysis Models
export interface ImplementationAnalysis {
  implementationId: string;
  passesTests: boolean;
  codeQuality: number;
  maintainabilityScore: number;
  issues: string[];
}

export interface RefactoringPlan {
  id: string;
  steps: RefactoringStep[];
  estimatedDuration: string; // ISO string
  riskLevel: string;
}

export interface RefactoringStep {
  id: string;
  type: string;
  description: string;
  before: string;
  after: string;
  safetyChecks: string[];
  requiresManualReview?: boolean;
  order?: number;
}

export interface ChangePrediction {
  id: string;
  area: string;
  description: string;
  probability: number;
  confidence: number;
  timeline: string; // ISO string
}

export interface FutureTestRecommendation {
  id: string;
  predictionId: string;
  testType: string;
  description: string;
  priority: string;
  implementationEffort: string;
}

export interface ConfidenceSummary {
  averageConfidence: number;
  highConfidencePredictions: number;
  mediumConfidencePredictions: number;
  lowConfidencePredictions: number;
  overallReliability: string;
}

export interface ImplementationTimeline {
  phases: Phase[];
  startDate: string; // ISO string
  endDate: string; // ISO string
  totalDuration: string; // ISO string
  dependencies: TimelineDependency[];
  risks: Risk[];
}

export interface Phase {
  name: string;
  description: string;
  startDate: string; // ISO string
  endDate: string; // ISO string
  deliverables: string[];
  status: string;
}

export interface TimelineDependency {
  fromPhase: string;
  toPhase: string;
  type: string;
  lagDays: number;
}

export interface Risk {
  description: string;
  impact: string;
  probability: string;
  mitigationStrategy: string;
  owner: string;
}

export interface RiskMitigationStrategy {
  predictionId: string;
  strategy: string;
  rationale: string;
  implementationSteps: string[];
  expectedOutcome: string;
  owner: string;
}

export interface RefactoringHint {
  implementationId: string;
  suggestion: string;
  reason: string;
  priority: string;
  estimatedEffort: string;
}

export interface RefactoringOpportunity {
  implementationId: string;
  area: string;
  suggestion: string;
  expectedImprovement: number;
  effort: string;
  priority: string;
}

export interface CodeSmellAnalysis {
  implementationId: string;
  codeSmells: CodeSmell[];
  overallSmellScore: number;
  recommendations: string[];
  severity: string;
}

export interface CodeSmell {
  type: string;
  location: string;
  description: string;
  severity: string;
  fixSuggestion: string;
}

export interface ConfidenceMetrics {
  testQuality: number;
  implementationQuality: number;
  refactoringSafety: number;
  overallConfidence: number;
  confidenceFactors: string[];
  riskFactors: string[];
}

export interface LearningPoint {
  category: string;
  title: string;
  description: string;
  example: string;
  impact: string;
}

// Diagnostic Models
export interface ImplementationDiagnostic {
  issue: string;
  rootCause: string;
  failedTests: string[];
  suggestedFixes: string[];
  severity: string;
}

export interface FailureAnalysis {
  rootCause: string;
  contributingFactors: string[];
  suggestedFixes: string[];
  impactLevel: string;
  canRetry: boolean;
  prerequisitesForRetry: string[];
}

export interface RollbackSuggestion {
  stepNumber: number;
  reason: string;
  codeState: string;
  testsToVerify: string[];
  recommended: boolean;
}

export interface ImprovementMetrics {
  overallImprovement: number;
  maintainabilityGain: number;
  readabilityGain: number;
  performanceChange: number;
  complexityReduction: number;
  linesOfCodeChange: number;
}

export interface TestSafetyReport {
  totalTests: number;
  passingTests: number;
  failingTests: number;
  coverage: TestCoverage;
  safetyIssues: string[];
  allTestsPass: boolean;
  totalTestDuration: string; // ISO string
}

export interface TestCoverage {
  lineCoverage: number;
  branchCoverage: number;
  methodCoverage: number;
  uncoveredLines: string[];
}

export interface MaintenanceImpact {
  estimatedMaintenanceCost: number;
  riskFactors: string[];
  improvementAreas: string[];
  technicalDebtReduction: number;
  longTermSustainability: string;
}

export interface CodeAnalysis {
  codeId: string;
  complexity: number;
  maintainabilityIndex: number;
  cyclomaticComplexity: number;
  linesOfCode: number;
  depthOfInheritance: number;
  classCoupling: number;
  codeSmells: string[];
  improvementOpportunities: string[];
}

export interface RoadmapAnalysis {
  roadmap: ProductRoadmap;
  highPriorityFeatures: string[];
  technicalRequirements: string[];
  dependencies: string[];
  estimatedCompletion: string; // ISO string
  riskAreas: string[];
  features: RoadmapFeature[];
}

export interface ProductRoadmap {
  id: string;
  name: string;
  features: RoadmapFeature[];
  milestones: Milestone[];
  startDate: string; // ISO string
  endDate: string; // ISO string
}

export interface RoadmapFeature {
  id: string;
  title: string;
  description: string;
  priority: string;
  targetDate: string; // ISO string
  status: string;
  dependencies: string[];
}

export interface Milestone {
  id: string;
  name: string;
  date: string; // ISO string
  deliverables: string[];
  status: string;
}