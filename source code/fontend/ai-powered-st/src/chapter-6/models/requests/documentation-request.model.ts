// models/requests/documentation-request.model.ts (updated)
import { BDDScenario } from '../bdd-scenario.model';

export interface DocumentationRequest {
  scenarios: BDDScenario[];
  testResults: TestResult[];
  audience: Audience;
  format: string;
  include: string[];
  updateStrategy: UpdateStrategy;
}

export interface TestResult {
  scenarioId: string;
  passed: boolean;
  errors: string[];
  executionTime: Date;
  duration: number;
}

export interface Audience {
  role: string;
  technicalLevel: string;
  interests: string[];
  constraints: string[];
}

export interface UpdateStrategy {
  trigger: string;
  autoUpdate: boolean;
  notifyRoles: string[];
  versioning: string;
}