// interfaces/simple-models.ts

// Simple interfaces for the example service
export interface SimpleBDDScenario {
  title: string;
  given: string[];
  when: string[];
  then: string[];
  tags: string[];
  description: string;
  examples: string[];
}

export interface SimpleImplementedBehavior {
  scenarioId: string;
  steps: string[];
  outcomes: string[];
  edgeCases: string[];
  lastUpdated: string; // ISO string for simplicity
}

export interface SimpleStakeholderPerspective {
  role: string;
  priorities: string[];
  concerns: string[];
}

export interface SimpleConversationConstraints {
  maxRounds: number;
  consensusThreshold: number;
  forbiddenAssumptions: string[];
}

export interface SimpleTechContext {
  stack: string;
  testFramework: string;
  libraries: string[];
  constraints: string[];
}

export interface SimpleQualityTargets {
  minCoverage: number;
  maxComplexity: number;
  maxDependencies: number;
  validationRules: string[];
}

export interface SimpleBreakingChange {
  type: string;
  description: string;
  impactLevel: string;
  affectedAreas: string[];
}

export interface SimpleTestResult {
  scenarioId: string;
  passed: boolean;
  errors: string[];
  executionTime: string;
  duration: number;
}

export interface SimpleAudience {
  role: string;
  technicalLevel: string;
  interests: string[];
  constraints: string[];
}

export interface SimpleUpdateStrategy {
  trigger: string;
  autoUpdate: boolean;
  notifyRoles: string[];
  versioning: string;
}