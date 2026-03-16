// models/responses/documentation-response.model.ts
export interface DocumentationResponse {
  documentationId: string;
  generatedDocumentation: GeneratedDocumentation;
  scenarioAnalysis: ScenarioAnalysis;
  audienceAppropriatenessScore: number;
  interactiveFeatures: InteractiveFeature[];
  updateMechanism: UpdateMechanism;
  accessPatterns: AccessPattern[];
  qualityMetrics: DocumentationQualityMetrics;
}

export interface GeneratedDocumentation {
  format: string;
  content: string;
  sections: string[];
  navigation: string[];
  interactiveElements: string[];
}

export interface ScenarioAnalysis {
  healthScore: number;
  coverageScore: number;
  healthyScenarios: string[];
  problematicScenarios: string[];
  recommendations: string[];
}

export interface InteractiveFeature {
  featureType: string;
  description: string;
  capabilities: string[];
  requirements: string[];
}

export interface UpdateMechanism {
  type: string;
  trigger: string;
  processes: string[];
  notifications: string[];
}

export interface AccessPattern {
  pattern: string;
  description: string;
  useCases: string[];
  bestPractices: string[];
}

export interface DocumentationQualityMetrics {
  clarityScore: number;
  completenessScore: number;
  accuracyScore: number;
  maintainabilityScore: number;
  issues: string[];
}