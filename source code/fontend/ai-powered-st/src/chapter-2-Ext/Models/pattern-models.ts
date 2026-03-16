// ==================== Request Models ====================

export interface PatternEstablishmentRequest {
  area: string;
  examples: TestExample[];
  desiredConsistency: 'low' | 'medium' | 'high';
  automationLevel: 'manual' | 'semi-automated' | 'fully-automated';
  validationCriteria: string[];
  metadata?: PatternMetadata;
}

export interface TestExample {
  id?: string;
  name: string;
  code: string;
  description: string;
  tags: string[];
  context: string;
  metrics?: ExampleMetrics;
}

export interface ExampleMetrics {
  linesOfCode: number;
  complexityScore: number;
  readabilityScore: number;
  dependencies: number;
}

export interface PatternMetadata {
  teamSize: number;
  experienceLevel: string;
  timeline: string;
  tools: string[];
  constraints?: { [key: string]: string };
}

export interface TrainingGenerationRequest {
  pattern: TestingPattern;
  audience: string;
  format: string;
  durationMinutes: number;
  includeHandsOn: boolean;
  prerequisites: string[];
  learningObjectives: string[];
  customizations?: { [key: string]: any };
}

export interface PipelineRequest {
  patternId: string;
  triggerEvents: string[];
  stages: PipelineStage[];
  qualityGates: QualityGate[];
  configuration?: { [key: string]: any };
}

export interface PipelineStage {
  name: string;
  tasks: string[];
  dependencies?: string[];
  timeoutMinutes?: number;
  requiredTools?: string[];
}

export interface QualityGate {
  metric: string;
  threshold: number;
  operator?: string;
  action?: string;
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
  createdAt: string;
  updatedAt: string;
  status: 'draft' | 'validated' | 'adopted' | 'deprecated';
}

export interface PatternImplementation {
  codeExamples: string[];
  configuration: { [key: string]: any };
  dosAndDonts: string[];
  implementationSteps: Step[];
}

export interface Step {
  order: number;
  title: string;
  description: string;
  codeSnippet: string;
  expectedOutcome: string;
}

export interface QualityIndicators {
  repeatabilityScore: number;
  learningCurve: 'easy' | 'medium' | 'steep';
  maintenanceCost: 'low' | 'medium' | 'high';
  codeCoverage: number;
  performanceImpact: number;
}

export interface AiAssistance {
  promptTemplates: string[];
  validationRules: string[];
  commonPitfalls: string[];
  optimizationTips: string[];
}

export interface AdoptionMetrics {
  estimatedTimeSave: string;
  errorReduction: string;
  teamSatisfaction: number;
  adoptionRate: number;
  targetAdoptionDate?: string;
}

export interface TrainingMaterials {
  id: string;
  patternId: string;
  patternName: string;
  audience: string;
  content: TrainingContent;
  schedule: TrainingSchedule;
  assessments: Assessment[];
  resources: Resource[];
  generatedAt: string;
}

export interface TrainingContent {
  presentation: string;
  slides: string[];
  exercises: Exercise[];
  discussionPoints: string[];
  keyTakeaways: string[];
}

export interface Exercise {
  title: string;
  description: string;
  instructions: string;
  expectedSolution: string;
  estimatedMinutes: number;
  difficulty: string;
}

export interface TrainingSchedule {
  startDate: string;
  sessions: Session[];
  breaks: Break[];
}

export interface Session {
  title: string;
  durationMinutes: number;
  facilitator: string;
  materials: string[];
}

export interface Break {
  name: string;
  durationMinutes: number;
  description: string;
}

export interface Assessment {
  type: string;
  questions: Question[];
  passingScore: number;
  timeLimit: string;
}

export interface Question {
  text: string;
  category: string;
  difficulty: string;
  options?: string[];
  correctAnswer?: string;
}

export interface Resource {
  type: string;
  title: string;
  url: string;
  description: string;
  estimatedMinutes: number;
}

export interface PipelineBlueprint {
  id: string;
  patternId: string;
  pipelineName: string;
  configuration: PipelineConfiguration;
  implementationScripts: string[];
  integrations: Integration[];
  monitoring: MonitoringSettings;
  createdAt: string;
}

export interface PipelineConfiguration {
  version: string;
  environmentVariables: { [key: string]: string };
  toolConfigurations: { [key: string]: any };
  notifications: string[];
  artifacts: string[];
}

export interface Integration {
  type: string;
  name: string;
  configuration: { [key: string]: any };
  enabled: boolean;
}

export interface MonitoringSettings {
  metrics: string[];
  alerts: Alert[];
  dashboard: Dashboard;
}

export interface Alert {
  name: string;
  condition: string;
  channels: string[];
  severity: string;
}

export interface Dashboard {
  name: string;
  widgets: Widget[];
  viewers: string[];
}

export interface Widget {
  type: string;
  title: string;
  configuration: { [key: string]: any };
}

// ==================== Validation Models ====================

export interface PatternValidationResult {
  isValid: boolean;
  errors: string[];
  warnings: string[];
  metrics: ValidationMetric[];
}

export interface ValidationMetric {
  name: string;
  value: number;
  threshold: number;
  passed: boolean;
}

// ==================== Error Models ====================

export interface PatternError {
  errorId: string;
  patternArea: string;
  failureType: 'generation' | 'validation' | 'adoption';
  symptoms: string[];
  rootCause: string;
  mitigationSteps: string[];
  temporaryWorkaround: string;
  occurredAt: string;
}