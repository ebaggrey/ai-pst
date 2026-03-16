// models/configuration/ai-testing-config.model.ts
export interface AITestingConfig {
  apiBaseUrl: string;
  defaultProvider: string;
  defaultModel: string;
  maxDimensionsForThorough: number;
  maxPromptVariations: number;
  minSensitivityThreshold: number;
  maxSensitivityThreshold: number;
  minFactsForHallucinationTest: number;
  minBaselineResults: number;
  minDataPointsForDrift: number;
  defaultTestRuns: number;
  requestTimeout: number;
  enableCaching: boolean;
  cacheDuration: number;
  supportedProviders: string[];
  retryAttempts: number;
  retryDelay: number;
}

// Default configuration
export const DEFAULT_AITESTING_CONFIG: AITestingConfig = {
  apiBaseUrl: 'http://localhost:5000/api/ai-testing',
  defaultProvider: 'openai',
  defaultModel: 'gpt-4',
  maxDimensionsForThorough: 10,
  maxPromptVariations: 50,
  minSensitivityThreshold: 0.5,
  maxSensitivityThreshold: 0.95,
  minFactsForHallucinationTest: 5,
  minBaselineResults: 10,
  minDataPointsForDrift: 50,
  defaultTestRuns: 10,
  requestTimeout: 30000,
  enableCaching: true,
  cacheDuration: 300000, // 5 minutes
  supportedProviders: ['openai', 'anthropic', 'google', 'azure', 'aws'],
  retryAttempts: 3,
  retryDelay: 1000
};