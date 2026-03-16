// models/requests/evolution-request.model.ts (updated)
import { BDDScenario } from '../bdd-scenario.model';

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