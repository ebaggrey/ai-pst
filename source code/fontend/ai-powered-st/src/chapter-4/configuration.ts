// models/configuration/ai-testing-config.model.ts
export interface AITestingConfig {
  // API Configuration
  apiBaseUrl: string;
  apiVersion: string;
  
  // Default Values
  defaultProvider: string;
  defaultModel: string;
  defaultRigorLevel: 'standard' | 'thorough';
  defaultTestRuns: number;
  defaultTimeout: number;
  defaultDriftThreshold: number;
  
  // Validation Limits
  maxDimensionsForThorough: number;
  maxPromptVariations: number;
  minSensitivityThreshold: number;
  maxSensitivityThreshold: number;
  minFactsForHallucinationTest: number;
  minBaselineResults: number;
  minDataPointsForDrift: number;
  maxBatchTests: number;
  
  // Performance Settings
  requestTimeout: number;
  retryAttempts: number;
  retryDelay: number;
  retryBackoffMultiplier: number;
  
  // Cache Settings
  enableCaching: boolean;
  cacheDuration: number; // milliseconds
  maxCacheSize: number;
  
  // Supported Providers & Models
  supportedProviders: string[];
  providerModels: Record<string, string[]>;
  
  // Feature Flags
  enableBatchTesting: boolean;
  enableRealTimeMonitoring: boolean;
  enableExportFeatures: boolean;
  enableDetailedLogging: boolean;
  
  // UI Settings
  autoRefreshInterval: number; // milliseconds
  maxResultsToDisplay: number;
  defaultResultsPageSize: number;
  
  // Security
  requireAuthentication: boolean;
  tokenRefreshInterval: number;
  maxConcurrentRequests: number;
  maxRetryAttempts:number
}

export const DEFAULT_AITESTING_CONFIG: AITestingConfig = {
  // API Configuration
  apiBaseUrl: 'http://localhost:5000/api/ai-testing',
  apiVersion: 'v1',
  
  // Default Values
  defaultProvider: 'openai',
  defaultModel: 'gpt-4',
  defaultRigorLevel: 'standard',
  defaultTestRuns: 10,
  defaultTimeout: 30000, // 30 seconds
  defaultDriftThreshold: 0.15,
  
  // Validation Limits
  maxDimensionsForThorough: 10,
  maxPromptVariations: 50,
  minSensitivityThreshold: 0.5,
  maxSensitivityThreshold: 0.95,
  minFactsForHallucinationTest: 5,
  minBaselineResults: 10,
  minDataPointsForDrift: 50,
  maxBatchTests: 20,
  
  // Performance Settings
  requestTimeout: 30000, // 30 seconds
  retryAttempts: 3,
  retryDelay: 1000, // 1 second
  retryBackoffMultiplier: 2,
  
  // Cache Settings
  enableCaching: true,
  cacheDuration: 300000, // 5 minutes
  maxCacheSize: 100,
  
  // Supported Providers & Models
  supportedProviders: ['openai', 'anthropic', 'google', 'azure', 'aws', 'huggingface'],
  providerModels: {
    openai: ['gpt-4', 'gpt-4-turbo', 'gpt-3.5-turbo', 'text-davinci-003'],
    anthropic: ['claude-3-opus', 'claude-3-sonnet', 'claude-3-haiku', 'claude-2.1'],
    google: ['gemini-pro', 'gemini-ultra', 'palm-2'],
    azure: ['gpt-4', 'gpt-35-turbo'],
    aws: ['claude-v2', 'titan-text'],
    huggingface: ['llama-2', 'mistral', 'zephyr']
  },
  
  // Feature Flags
  enableBatchTesting: true,
  enableRealTimeMonitoring: true,
  enableExportFeatures: true,
  enableDetailedLogging: false,
  
  // UI Settings
  autoRefreshInterval: 30000, // 30 seconds
  maxResultsToDisplay: 100,
  defaultResultsPageSize: 10,
  
  // Security
  requireAuthentication: false,
  tokenRefreshInterval: 3600000, // 1 hour
  maxConcurrentRequests: 5,
  maxRetryAttempts:3
};