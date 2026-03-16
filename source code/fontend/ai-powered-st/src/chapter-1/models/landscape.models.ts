
export interface AppProfile {
  name: string;
  architectureType: string; // 'microservices' | 'monolith' | 'serverless' | 'hybrid'
  frontendFrameworks: string[];
  backendServicesCount: number;  // This should be a number, not duplicated
  backendServices: BackendService[];
  dataSources: string[];
  expectedUsers: UserScale;
  criticalUserJourneys: string[];
  techDebtAreas?: Record<string, string>;
  deploymentEnvironment?: string;
  complianceRequirements?: string[];
  availabilityRequirement?: number;
  performanceSLAs?: Record<string, string>;
  securityRequirements?: string[];
}



// models/landscape.models.ts
export interface LandscapeTestRequest {
  applicationProfile: ApplicationProfile;
  testingFocus: string[];
  riskAssessment?: RiskAssessment;
  promptVersion: string;
  requestedArtifacts: string[];
  customParameters?: Record<string, string>;
  includeDetailedAnalysis?: boolean;
  analysisDepth?: 'quick' | 'standard' | 'comprehensive';
  maxRecommendationsPerArea?: number;
}

export interface ApplicationProfile {
  name: string;
  architectureType: string; // 'microservices' | 'monolith' | 'serverless' | 'hybrid'
  frontendFrameworks: string[];
  backendServicesCount: number;
  backendServices: BackendService[];
  dataSources: string[];
  expectedUsers: UserScale;
  criticalUserJourneys: string[];
  techDebtAreas?: Record<string, string>;
  deploymentEnvironment?: string;
  complianceRequirements?: string[];
  availabilityRequirement?: number;
  performanceSLAs?: Record<string, string>;
  securityRequirements?: string[];
}

export interface BackendService {
  name: string;
  technology: string;
  endpoints: string[];
  hasDatabase: boolean;
  requestRatePerSecond?: number;
  dependencies?: string[];
  healthCheckEndpoint?: string;
}

export enum UserScale {
  Small = 'small',       // < 1,000 users
  Medium = 'medium',     // 1,000 - 10,000 users
  Large = 'large',       // 10,000 - 100,000 users
  Enterprise = 'enterprise' // > 100,000 users
}

export interface RiskAssessment {
  criticality: number; // 1-10
  complianceRequirements: string[];
  dataSensitivity: string[];
  riskFactors: RiskFactor[];
}

export interface RiskFactor {
  area: string;
  likelihood: number; // 1-10
  impact: number; // 1-10
  description: string;
}

export interface TestLandscapeResponse {
  analysisId: string;
  highPriorityScenarios: TestScenario[];
  recommendedAutomation: AutomationBlueprint[];
  identifiedRisks: RiskHotspot[];
  flakyPredictions: FlakyTestPrediction[];
  monitoringSuggestions: MonitoringStrategy;
  summary: string;
  generatedAt: Date;
  estimatedEffort: string;
  confidenceScores?: Record<string, number>;
  usedLLMProviders?: string[];
  analysisMetadata?: Record<string, any>;
  nextSteps?: string;
  resourceRecommendations?: ResourceRecommendation[];
  costEstimate?: CostEstimate;
}

export interface TestScenario {
  id: string;
  title: string;
  description: string;
  priority: 'high' | 'medium' | 'low';
  riskAreas: string[];
  steps: string[];
  expectedOutcome: string;
  estimatedDuration?: string;
}

export interface AutomationBlueprint {
  id: string;
  name: string;
  technologyStack: string[];
  codeSnippet: string;
  coverage: string[];
  prerequisites: string[];
  implementationComplexity?: string;
}

export interface RiskHotspot {
  area: string;
  riskLevel: 'critical' | 'high' | 'medium' | 'low';
  description: string;
  mitigation: string[];
  testApproach: string;
  probability?: number;
  impact?: number;
}

export interface FlakyTestPrediction {
  testType: string;
  area: string;
  flakinessScore: number; // 0-10
  reasons: string[];
  stabilizationTips: string[];
  warningSigns?: string[];
}

export interface MonitoringStrategy {
  metrics: MonitoringMetric[];
  alerts: AlertRule[];
  dashboards: DashboardSuggestion[];
  implementationGuide?: string;
}

export interface MonitoringMetric {
  name: string;
  description: string;
  threshold: number;
  collectionFrequency: string;
  dataSource?: string;
}

export interface AlertRule {
  condition: string;
  severity: 'critical' | 'warning' | 'info';
  action: string;
  isEnabled?: boolean;
}

export interface DashboardSuggestion {
  name: string;
  metrics: string[];
  refreshInterval: string;
  purpose?: string;
}

export interface ResourceRecommendation {
  type: string; // tool, framework, library, service
  name: string;
  description: string;
  justification: string;
  useCases: string[];
  cost: CostRange;
  alternatives?: string[];
}

export interface CostRange {
  min: number;
  max: number;
  currency: string;
  period: string; // month, year, one-time
}

export interface CostEstimate {
  automationDevelopment: number;
  manualTesting: number;
  toolLicensing: number;
  infrastructure: number;
  total: number;
  currency: string;
  timeframe: string;
}

export interface LandscapeError {
  errorCode: string;
  message: string;
  recoverySteps: string[];
  fallbackSuggestion: string;
  timestamp: Date;
  context?: Record<string, any>;
  correlationId?: string;
  severity?: 'error' | 'warning' | 'info';
}