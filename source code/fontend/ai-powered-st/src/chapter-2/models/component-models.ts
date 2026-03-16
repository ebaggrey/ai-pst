// ==================== Request Models ====================
export interface PatternEstablishmentRequest {
  area: string;
  examples: TestExample[];
  desiredConsistency: 'low' | 'medium' | 'high';
  automationLevel: 'manual' | 'semi-automated' | 'fully-automated';
  validationCriteria: string[];
  metadata?: RequestMetadata;
}

export interface TestExample {
  testName: string;
  input: string;
  expectedOutput: string;
  actualOutput?: string;
  tags?: string[];
  complexity: 'low' | 'medium' | 'high';
}

export interface RequestMetadata {
  teamSize: number;
  experienceLevel: 'beginner' | 'intermediate' | 'expert';
  timeline: string;
}

export interface TrainingGenerationRequest {
  pattern: TestingPattern;
  audience: string;
  format: 'workshop-ready' | 'quick-start' | 'comprehensive';
  durationMinutes: number;
  includeHandsOn: boolean;
  prerequisites: string[];
  learningObjectives: string[];
}

export interface PipelineRequest {
  patternId: string;
  triggerEvents: string[];
  stages: PipelineStageRequest[];
  qualityGates: QualityGateRequest[];
}

export interface PipelineStageRequest {
  name: string;
  tasks: string[];
}

export interface QualityGateRequest {
  metric: string;
  threshold: number;
}

// ==================== Response Models ====================

export interface TestingPattern {
  id: string;
  name: string;
  area: string;
  problemStatement: string;
  solution: string;
  implementation: PatternImplementation;
  qualityIndicators: QualityIndicators;
  aiAssistance: AiAssistance;
  adoptionMetrics: AdoptionMetrics;
  createdAt: string; // ISO date string
  status: 'draft' | 'active' | 'deprecated' | 'archived';
}

export interface PatternImplementation {
  codeExamples: string[];
  configuration: { [key: string]: any };
  dosAndDonts: string[];
}

export interface QualityIndicators {
  repeatabilityScore: number; // 0-100
  learningCurve: 'easy' | 'medium' | 'steep';
  maintenanceCost: 'low' | 'medium' | 'high';
}

export interface AiAssistance {
  promptTemplates: string[];
  validationRules: string[];
  commonPitfalls: string[];
}

export interface AdoptionMetrics {
  estimatedTimeSave: string; // e.g., "40-60%"
  errorReduction: string; // e.g., "50-70%"
  teamSatisfaction: number; // 1-10
}

export interface TrainingMaterials {
  id: string;
  patternId: string;
  audience: string;
  title: string;
  format: 'workshop-ready' | 'quick-start' | 'comprehensive';
  durationMinutes: number;
  modules: TrainingModule[];
  prerequisites: string[];
  learningObjectives: string[];
  handsOn: HandsOnSection;
  assessment: Assessment;
  generatedAt: string; // ISO date string
}

export interface TrainingModule {
  title: string;
  content: string;
  durationMinutes: number;
  keyPoints: string[];
}

export interface HandsOnSection {
  included: boolean;
  setupInstructions?: string;
  exercises?: Exercise[];
  expectedOutcome?: string;
}

export interface Exercise {
  title: string;
  description: string;
  solutionHint: string;
  solution: string;
}

export interface Assessment {
  quizQuestions: QuizQuestion[];
  practicalTask?: PracticalTask;
  passingScore: number; // 0-100
}

export interface QuizQuestion {
  text: string;
  options: string[];
  correctAnswerIndex: number;
  explanation: string;
}

export interface PracticalTask {
  description: string;
  requirements: string[];
  successCriteria: string[];
}

export interface PipelineBlueprint {
  id: string;
  patternId: string;
  name: string;
  triggerEvents: string[];
  stages: PipelineStage[];
  qualityGates: QualityGate[];
  deploymentConfig: DeploymentConfiguration;
  monitoringConfig: MonitoringConfiguration;
  createdAt: string; // ISO date string
}

export interface PipelineStage {
  name: string;
  tasks: PipelineTask[];
  parameters?: { [key: string]: string };
  executor: string;
}

export interface PipelineTask {
  name: string;
  type: string;
  configuration?: { [key: string]: any };
  timeoutMinutes: number;
}

export interface QualityGate {
  metric: string;
  threshold: number;
  operator: '>=' | '<=' | '>' | '<' | '==';
  actionOnFail: 'block' | 'warn' | 'continue';
}

export interface DeploymentConfiguration {
  environment: 'development' | 'staging' | 'production';
  autoDeploy: boolean;
  approvers?: string[];
  rollbackStrategy: RollbackStrategy;
}

export interface RollbackStrategy {
  enabled: boolean;
  triggerCondition: string;
  method: 'automatic' | 'manual';
}

export interface MonitoringConfiguration {
  metrics: string[];
  alertRules: AlertRule[];
  dashboardUrl: string;
}

export interface AlertRule {
  name: string;
  condition: string;
  notifyChannels: ('email' | 'slack' | 'teams' | 'pagerduty')[];
}

export interface HealthStatus {
  status: 'healthy' | 'degraded' | 'unhealthy';
  timestamp: string; // ISO date string
  service: string;
  version?: string;
  dependencies?: HealthDependency[];
}

export interface HealthDependency {
  name: string;
  status: 'healthy' | 'degraded' | 'unhealthy';
  message?: string;
}

// ==================== Error Models ====================

export interface PatternError {
  patternArea: string;
  failureType: 'generation' | 'validation' | 'adoption';
  symptoms: string[];
  rootCause: string;
  mitigationSteps: string[];
  temporaryWorkaround?: string;
  errorTime: string; // ISO date string
  correlationId: string;
}

export interface ApiErrorResponse {
  errorCode: string;
  message: string;
  details?: string;
  correlationId: string;
  timestamp: string; // ISO date string
  patternError?: PatternError;
}

// ==================== Enum Types for Type Safety ====================

export type ConsistencyLevel = 'low' | 'medium' | 'high';
export type AutomationLevel = 'manual' | 'semi-automated' | 'fully-automated';
export type LearningCurve = 'easy' | 'medium' | 'steep';
export type MaintenanceCost = 'low' | 'medium' | 'high';
export type PatternStatus = 'draft' | 'active' | 'deprecated' | 'archived';
export type TrainingFormat = 'workshop-ready' | 'quick-start' | 'comprehensive';
export type Environment = 'development' | 'staging' | 'production';
export type RollbackMethod = 'automatic' | 'manual';
export type HealthStatusType = 'healthy' | 'degraded' | 'unhealthy';
export type FailureType = 'generation' | 'validation' | 'adoption';
export type NotifyChannel = 'email' | 'slack' | 'teams' | 'pagerduty';

// ==================== Factory Functions for Creating Default Objects ====================

export function createDefaultTestingPattern(): TestingPattern {
  return {
    id: '',
    name: '',
    area: '',
    problemStatement: '',
    solution: '',
    implementation: {
      codeExamples: [],
      configuration: {},
      dosAndDonts: []
    },
    qualityIndicators: {
      repeatabilityScore: 0,
      learningCurve: 'medium',
      maintenanceCost: 'medium'
    },
    aiAssistance: {
      promptTemplates: [],
      validationRules: [],
      commonPitfalls: []
    },
    adoptionMetrics: {
      estimatedTimeSave: '',
      errorReduction: '',
      teamSatisfaction: 0
    },
    createdAt: new Date().toISOString(),
    status: 'draft'
  };
}

export function createDefaultTestExample(): TestExample {
  return {
    testName: '',
    input: '',
    expectedOutput: '',
    complexity: 'medium',
    tags: []
  };
}

export function createDefaultHealthStatus(): HealthStatus {
  return {
    status: 'healthy',
    timestamp: new Date().toISOString(),
    service: 'PatternEstablishmentAPI',
    version: '1.0.0'
  };
}

export function createDefaultTrainingMaterials(): TrainingMaterials {
  return {
    id: '',
    patternId: '',
    audience: '',
    title: '',
    format: 'workshop-ready',
    durationMinutes: 60,
    modules: [],
    prerequisites: [],
    learningObjectives: [],
    handsOn: {
      included: false
    },
    assessment: {
      quizQuestions: [],
      passingScore: 80
    },
    generatedAt: new Date().toISOString()
  };
}

export function createDefaultPipelineBlueprint(): PipelineBlueprint {
  return {
    id: '',
    patternId: '',
    name: '',
    triggerEvents: [],
    stages: [],
    qualityGates: [],
    deploymentConfig: {
      environment: 'development',
      autoDeploy: false,
      rollbackStrategy: {
        enabled: true,
        triggerCondition: 'failureRate > 10%',
        method: 'automatic'
      }
    },
    monitoringConfig: {
      metrics: [],
      alertRules: [],
      dashboardUrl: ''
    },
    createdAt: new Date().toISOString()
  };
}

export function createDefaultPatternError(): PatternError {
  return {
    patternArea: '',
    failureType: 'generation',
    symptoms: [],
    rootCause: '',
    mitigationSteps: [],
    errorTime: new Date().toISOString(),
    correlationId: ''
  };
}

// ==================== Type Guards ====================

export function isTestingPattern(obj: any): obj is TestingPattern {
  return obj && typeof obj === 'object' && 'id' in obj && 'name' in obj && 'area' in obj;
}

export function isTrainingMaterials(obj: any): obj is TrainingMaterials {
  return obj && typeof obj === 'object' && 'id' in obj && 'patternId' in obj && 'modules' in obj;
}

export function isPipelineBlueprint(obj: any): obj is PipelineBlueprint {
  return obj && typeof obj === 'object' && 'id' in obj && 'patternId' in obj && 'stages' in obj;
}

export function isHealthStatus(obj: any): obj is HealthStatus {
  return obj && typeof obj === 'object' && 'status' in obj && 'timestamp' in obj && 'service' in obj;
}

export function isApiErrorResponse(obj: any): obj is ApiErrorResponse {
  return obj && typeof obj === 'object' && 'errorCode' in obj && 'message' in obj && 'correlationId' in obj;
}

// ==================== Validation Helper ====================

export function validateTestingPattern(pattern: TestingPattern): string[] {
  const errors: string[] = [];
  
  if (!pattern.id) errors.push('Pattern ID is required');
  if (!pattern.name) errors.push('Pattern name is required');
  if (!pattern.area) errors.push('Pattern area is required');
  if (!pattern.problemStatement) errors.push('Problem statement is required');
  if (!pattern.solution) errors.push('Solution is required');
  
  if (pattern.qualityIndicators.repeatabilityScore < 0 || pattern.qualityIndicators.repeatabilityScore > 100) {
    errors.push('Repeatability score must be between 0 and 100');
  }
  
  if (pattern.adoptionMetrics.teamSatisfaction < 1 || pattern.adoptionMetrics.teamSatisfaction > 10) {
    errors.push('Team satisfaction must be between 1 and 10');
  }
  
  return errors;
}

export function validateTrainingMaterials(materials: TrainingMaterials): string[] {
  const errors: string[] = [];
  
  if (!materials.id) errors.push('Training materials ID is required');
  if (!materials.patternId) errors.push('Pattern ID is required');
  if (!materials.title) errors.push('Title is required');
  if (materials.durationMinutes <= 0) errors.push('Duration must be positive');
  
  return errors;
}

export function validatePipelineBlueprint(pipeline: PipelineBlueprint): string[] {
  const errors: string[] = [];
  
  if (!pipeline.id) errors.push('Pipeline ID is required');
  if (!pipeline.patternId) errors.push('Pattern ID is required');
  if (!pipeline.name) errors.push('Pipeline name is required');
  if (pipeline.stages.length === 0) errors.push('At least one stage is required');
  
  return errors;
}