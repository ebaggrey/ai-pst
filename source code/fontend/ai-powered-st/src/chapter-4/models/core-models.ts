import { BDDScenario } from "src/chapter-6/models/bdd-scenario.model";
import { BaselineData } from "./supporting-models";
import { ImplementedBehavior } from "src/chapter-6/models/implemented-behavior.model";

// models/requests/ai-assessment-request.model.ts
export interface AIAssessmentRequest {
  provider: string;
  rigorLevel: 'standard' | 'thorough';
  dimensions: string[];
  modelName: string;
  maxTokens: number;
  includeBenchmarks: boolean;
}

// models/requests/robustness-test-request.model.ts
export interface RobustnessTestRequest {
  basePrompt: string;
  variations: string[];
  numberOfRuns: number;
  provider: string;
  modelName: string;
}

// models/requests/bias-detection-request.model.ts
export interface BiasDetectionRequest {
  contextAreas: string[];
  detectionMethods: string[];
  sensitivityThreshold: number;
  requireStatisticalSignificance: boolean;
  demographicData: Record<string, string>;
}

// models/requests/hallucination-test-request.model.ts
export interface HallucinationTestRequest {
  provider: string;
  knownFacts: string[];
  maxAllowedHallucinations: number;
  verificationSources: string[];
  testIterations: number;
}

// models/requests/drift-detection-request.model.ts
export interface DriftDetectionRequest {
  baseline: BaselineData;
  timeframe: string;
  metricsToMonitor: string[];
  driftThreshold: number;
  minimumDataPoints: number;
  documentedScenarios: BDDScenario[];
  implementedBehavior:ImplementedBehavior[];
  detectionMethods:string[];
  sensitivity:number;
  autoSuggestFixes:boolean,
}



