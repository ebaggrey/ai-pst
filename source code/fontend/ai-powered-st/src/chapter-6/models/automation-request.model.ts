// models/requests/automation-request.model.ts (updated)

import { BDDScenario } from "./bdd-scenario.model";


export interface AutomationRequest {
  scenario: BDDScenario;
  techContext: TechContext;
  translationStyle: string;
  qualityTargets: QualityTargets;
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