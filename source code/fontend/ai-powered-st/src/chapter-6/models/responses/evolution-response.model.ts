import { BDDScenario } from "../bdd-scenario.model";

// models/responses/evolution-response.model.ts
export interface EvolutionResponse {
  evolutionId: string;
  originalScenarios: BDDScenario[];
  evolvedScenarios: BDDScenario[];
  evolutionRecords: EvolutionRecord[];
  impactAnalysis: ImpactAnalysis;
  preservationMetrics: PreservationMetrics;
  breakingChangeHandling: BreakingChangeHandling;
  futureCompatibility: FutureCompatibility;
}

export interface EvolutionRecord {
  originalScenarioId: string;
  evolvedScenarioId: string;
  changes: string[];
  rationale: string[];
  preservationScore: number;
  evolvedAt: Date;
}

export interface ImpactAnalysis {
  highImpactChanges: string[];
  mediumImpactChanges: string[];
  lowImpactChanges: string[];
  affectedAreas: string[];
  risks: string[];
}

export interface PreservationMetrics {
  averagePreservation: number;
  minPreservation: number;
  maxPreservation: number;
  wellPreservedAreas: string[];
  poorlyPreservedAreas: string[];
}

export interface BreakingChangeHandling {
  successfullyHandled: string[];
  partiallyHandled: string[];
  notHandled: string[];
  workarounds: string[];
}

export interface FutureCompatibility {
  compatibilityScore: number;
  futureProofAreas: string[];
  vulnerableAreas: string[];
  recommendations: string[];
}