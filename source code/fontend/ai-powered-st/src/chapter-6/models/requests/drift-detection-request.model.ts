// models/requests/drift-detection-request.model.ts (updated)
import { BDDScenario } from '../bdd-scenario.model';
import { ImplementedBehavior } from '../implemented-behavior.model';

export interface DriftDetectionRequest {
  documentedScenarios: BDDScenario[];
  implementedBehavior: ImplementedBehavior[];
  detectionMethods: string[];
  sensitivity: number;
  autoSuggestFixes: boolean;
}

