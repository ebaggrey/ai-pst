// models/api-models.ts
export interface BDCCoCreationRequest {
  requirement: string;
  stakeholderPerspectives: StakeholderPerspective[];
  conversationConstraints: ConversationConstraints;
  desiredOutcomes: string[];
}

export interface StakeholderPerspective {
  role: string;
  priorities: string[];
  concerns: string[];
}

export interface ConversationConstraints {
  maxRounds: number;
  consensusThreshold: number;
  forbiddenAssumptions: string[];
}

export interface AutomationRequest {
  scenario: BDDScenario;
  techContext: TechContext;
  translationStyle: string;
  qualityTargets: QualityTargets;
}

export interface BDDScenario {
  title: string;
  given: string[];
  when: string[];
  then: string[];
  tags: string[];
  description: string;
  examples: string[];
}

export interface TechContext {
  stack: string;
  testFramework: string;
  libraries: string[];
  constraints: string[];
}

export interface QualityTargets {
  minCoverage: number;
  maxComplexity: number;
  maxDependencies: number;
  validationRules: string[];
}

export interface EvolutionRequest {
  existingScenarios: BDDScenario[];
  newInformation: string;
  breakingChanges: BreakingChange[];
  validationRules: ValidationRule[];
  evolutionStrategy: string;
}

export interface BreakingChange {
  type: string;
  description: string;
  impactLevel: string;
  affectedAreas: string[];
}

export interface ValidationRule {
  name: string;
  condition: string;
  errorMessage: string;
}

// Fixed DriftDetectionRequest to match backend
export interface DriftDetectionRequest {
  documentedScenarios: BDDScenario[];
  implementedBehavior: ImplementedBehavior[];
  detectionMethods: string[];
  sensitivity: number;
  autoSuggestFixes: boolean;
  // Additional properties from the error message
  baseline?: string;
  timeframe?: string;
  metricsToMonitor?: string[];
  driftThreshold?: number;
  alertOnDrift?: boolean;
  includeRecommendations?: boolean;
  maxFindings?: number;
  compareMode?: string;
  ignorePatterns?: string[];
}

export interface ImplementedBehavior {
  scenarioId: string;
  steps: string[];
  outcomes: string[];
  edgeCases: string[];
  lastUpdated: string;
}

export interface DocumentationRequest {
  scenarios: BDDScenario[];
  testResults: TestResult[];
  audience: Audience;
  format: string;
  include: string[];
  updateStrategy: UpdateStrategy;
}

export interface TestResult {
  scenarioId: string;
  passed: boolean;
  errors: string[];
  executionTime: string;
  duration: number;
}

export interface Audience {
  role: string;
  technicalLevel: string;
  interests: string[];
  constraints: string[];
}

export interface UpdateStrategy {
  trigger: string;
  autoUpdate: boolean;
  notifyRoles: string[];
  versioning: string;
}