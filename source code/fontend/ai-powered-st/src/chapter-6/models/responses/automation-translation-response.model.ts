import { BDDScenario } from "../bdd-scenario.model";
import { ValidationRule } from "../requests/evolution-request.model";

// models/responses/automation-translation-response.model.ts
export interface AutomationTranslationResponse {
  translationId: string;
  originalScenario: BDDScenario;
  automationSteps: AutomationStep[];
  frameworkIntegration: FrameworkIntegration;
  livingTest: LivingTest;
  qualityReport: QualityReport;
  maintenanceGuidance: MaintenanceGuidance;
  evolutionHooks: EvolutionHook[];
  alternatives?: BDDScenario[];
  unsupportedPattern?: string;
  suggestedRefactoring?: string;
}

export interface AutomationStep {
  originalStep: string;
  automationCode: string;
  validationRules: ValidationRule[];
  dependencies: string[];
  qualityMetrics: QualityMetrics;
}

export interface FrameworkIntegration {
  frameworkSetup: string;
  testStructure: string;
  requiredPackages: string[];
  configuration: string[];
}

export interface LivingTest {
  id: string;
  steps: string[];
  adaptations: string[];
  monitoringPoints: string[];
  createdAt: Date;
}

export interface QualityMetrics {
  complexityScore: number;
  maintainabilityScore: number;
  readabilityScore: number;
  cyclomaticComplexity: number;
  linesOfCode: number;
}

export interface QualityReport {
  coverageScore: number;
  maintainabilityScore: number;
  readabilityScore: number;
  issues: string[];
  recommendations: string[];
}

export interface MaintenanceGuidance {
  commonIssues: string[];
  fixPatterns: string[];
  updateTriggers: string[];
  bestPractices: string[];
}

export interface EvolutionHook {
  hookType: string;
  trigger: string;
  action: string;
  dependencies: string[];
}