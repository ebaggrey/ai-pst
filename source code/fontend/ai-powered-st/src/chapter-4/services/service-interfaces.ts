// services/interfaces/ai-testing-service.interface.ts
import { Observable } from 'rxjs';
import { AIAssessmentRequest, BiasDetectionRequest, DriftDetectionRequest,
     HallucinationTestRequest, RobustnessTestRequest } from '../models/core-models';
import { AICapabilityReport, BiasDetectionReport, 
         DriftDetectionReport, HallucinationDetectionReport,
         RobustnessTestReport } from '../models/response-models';

export interface IAITestingService {
  // Capability Assessment
  assessAICapabilities(request: AIAssessmentRequest): Observable<AICapabilityReport>;
  
  // Robustness Testing
  testPromptRobustness(request: RobustnessTestRequest): Observable<RobustnessTestReport>;
  
  // Bias Detection
  detectAIBias(request: BiasDetectionRequest): Observable<BiasDetectionReport>;
  
  // Hallucination Testing
  testForHallucinations(request: HallucinationTestRequest): Observable<HallucinationDetectionReport>;
  
  // Drift Monitoring
  monitorAIDrift(request: DriftDetectionRequest): Observable<DriftDetectionReport>;
  
  // Batch Testing
  runBatchTests(tests: any[]): Observable<any>;
  
  // Test History
  getTestHistory(provider?: string, startDate?: Date, endDate?: Date): Observable<any[]>;
  
  // Test Status
  getTestStatus(testId: string): Observable<any>;
  
  // Cancel Test
  cancelTest(testId: string): Observable<boolean>;
}