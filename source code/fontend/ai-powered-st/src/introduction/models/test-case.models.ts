export interface TestCaseRequest {
  prompt: string;
  llmProvider: string; // Changed from LLMProvider type to string to match backend regex
  context?: string;
  urgency: number; // Changed from string to number (1-3)
}

export interface TestCaseResponse {
  generatedCode: string;
  testFramework: string;
  explanation: string[];
  estimatedComplexity: number;
  potentialFlakyPoints: string[];
  chosenProvider: string;
}

export interface AIError {
  errorId?: string;
  message?: string;
  suggestion?: string;
  recoverable?: boolean;
  provider?: string;
}

export interface AnalysisRequest {
  testRuns: TestRun[];
  analysisType: string;
  confidenceThreshold: number;
}

export interface FlakyPrediction {
  // Add properties based on what your backend returns
  testName: string;
  flakyScore: number;
  confidence: number;
  patterns: string[];
}

export interface TestRun {
  // Define based on your actual TestRun structure
  id: string;
  testName: string;
  passed: boolean;
  timestamp: Date;
  duration: number;
}
