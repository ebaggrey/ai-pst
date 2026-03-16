import { BDDScenario } from "../bdd-scenario.model";
import { ImplementedBehavior } from "../implemented-behavior.model";

// models/responses/drift-detection-response.model.ts
export interface DriftDetectionResponse {
  detectionId: string;
  documentedScenarios: BDDScenario[];
  implementedBehavior: ImplementedBehavior[];
  driftFindings: DriftFinding[];
  driftSeverity: string;
  coverageGaps: CoverageGap[];
  suggestedFixes: DriftFix[];
  prioritizedActions: PrioritizedAction[];
  monitoringRecommendations: MonitoringRecommendation[];
}

export interface DriftFinding {
  type: string;
  scenarioId: string;
  description: string;
  severity: string;
  evidence: string[];
  impact: string[];
}

export interface CoverageGap {
  area: string;
  description: string;
  riskLevel: string;
  affectedScenarios: string[];
}

export interface DriftFix {
  driftFindingId: string;
  fixType: string;
  description: string;
  steps: string[];
  verification: string[];
}

export interface PrioritizedAction {
  action: string;
  priority: string;
  dependencies: string[];
  resources: string[];
}

export interface MonitoringRecommendation {
  area: string;
  metric: string;
  threshold: string;
  triggers: string[];
}