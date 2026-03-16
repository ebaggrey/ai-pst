// ai-testing.component.ts
import { Component, signal, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe, PercentPipe, DatePipe, CommonModule } from '@angular/common';
import { AITestingService } from '../../services/ai-testing.service';
import { AICapabilityReport, BiasDetectionReport,
         DriftDetectionReport, HallucinationDetectionReport,
         RobustnessTestReport } from 'src/chapter-4/models/response-models';
import { AITestingError } from 'src/chapter-4/models/supporting-models';
import { AIAssessmentRequest, BiasDetectionRequest,
         DriftDetectionRequest, HallucinationTestRequest,
         RobustnessTestRequest } from 'src/chapter-4/models/core-models';


@Component({
  selector: 'app-ai-testing',
  standalone: true,
  imports: [FormsModule, PercentPipe,CommonModule],
  templateUrl: './ai-testing.component.html',
  styleUrls: ['./ai-testing.component.css']
})
export class AITestingComponent {
  // Service
  private aiTestingService = inject(AITestingService);

  // Get base URL from service config
  baseUrl = this.aiTestingService.getConfig().apiBaseUrl;

  // API endpoints
  apiEndpoints = [
    { path: '/assess-capabilities', name: 'Capability Assessment' },
    { path: '/test-robustness', name: 'Robustness Test' },
    { path: '/detect-bias', name: 'Bias Detection' },
    { path: '/test-hallucinations', name: 'Hallucination Test' },
    { path: '/monitor-drift', name: 'Drift Monitoring' }
  ];

  // State signals
  loadingCapability = signal(false);
  loadingRobustness = signal(false);
  loadingBias = signal(false);
  loadingHallucination = signal(false);
  loadingDrift = signal(false);

  capabilityReport = signal<AICapabilityReport | null>(null);
  robustnessReport = signal<RobustnessTestReport | null>(null);
  biasReport = signal<BiasDetectionReport | null>(null);
  hallucinationReport = signal<HallucinationDetectionReport | null>(null);
  driftReport = signal<DriftDetectionReport | null>(null);

  error = signal<AITestingError | null>(null);
  activeTab = signal<'capability' | 'robustness' | 'bias' | 'hallucination' | 'drift'>('capability');

  isLoading = computed(() => 
    this.loadingCapability() || 
    this.loadingRobustness() || 
    this.loadingBias() || 
    this.loadingHallucination() || 
    this.loadingDrift()
  );

  hasError = computed(() => this.error() !== null);

  // ========== Capability Assessment Models ==========
  capabilityRequest: AIAssessmentRequest = {
    provider: 'openai',
    rigorLevel: 'standard',  // Now properly typed as "standard" | "thorough"
    dimensions: ['accuracy', 'speed'],
    modelName: 'gpt-4',
    maxTokens: 1000,
    includeBenchmarks: true
  };
  capabilityDimensions = 'accuracy,speed';

  // ========== Robustness Testing Models ==========
  robustnessRequest: RobustnessTestRequest = {
    basePrompt: 'Explain quantum computing in simple terms',
    variations: ['Explain to a child', 'Explain to an expert'],
    numberOfRuns: 10,
    provider: 'openai',
    modelName: 'gpt-4'
  };
  robustnessVariations = 'Explain to a child\nExplain to an expert\nExplain with an analogy';

  // ========== Bias Detection Models ==========
  biasRequest: BiasDetectionRequest = {
    contextAreas: ['hiring', 'lending'],
    detectionMethods: ['statistical', 'content-analysis'],
    sensitivityThreshold: 0.75,
    requireStatisticalSignificance: false,
    demographicData: {
      ageGroups: '18-30,31-50,51+',
      genders: 'male,female,non-binary',
      ethnicities: 'white,black,asian,hispanic'
    }
  };
  biasContextAreas = 'hiring,lending,healthcare';
  biasDetectionMethods = 'statistical,content-analysis';

  // ========== Hallucination Test Models ==========
  hallucinationRequest: HallucinationTestRequest = {
    provider: 'openai',
    knownFacts: [
      'The Earth orbits the Sun',
      'Water boils at 100°C at sea level',
      'Shakespeare wrote Hamlet',
      'The capital of France is Paris',
      'Python is a programming language'
    ],
    maxAllowedHallucinations: 3,
    verificationSources: ['wikipedia', 'britannica'],
    testIterations: 20
  };
  hallucinationFacts = 'The Earth orbits the Sun\nWater boils at 100°C at sea level\nShakespeare wrote Hamlet\nThe capital of France is Paris\nPython is a programming language';

  // ========== Drift Monitoring Models ==========
  driftRequest: DriftDetectionRequest = {
    baseline: {
      testResults: [],
      collectedOn: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000),
      environment: 'production'
    },
    timeframe: '30d',
    metricsToMonitor: ['accuracy', 'latency', 'consistency'],
    driftThreshold: 0.15,
    minimumDataPoints: 100,
    documentedScenarios: [],
    implementedBehavior: [],
    detectionMethods: [],
    sensitivity: 0,
    autoSuggestFixes: false
  };
  driftMetrics = 'accuracy,latency,consistency';

  // Helper method to get full URL
  getFullUrl(endpoint: string): string {
    return `${this.baseUrl}${endpoint}`;
  }

  // TrackBy function for ngFor
  trackByPath(index: number, item: any): string {
    return item.path;
  }

  trackByArea(index: number, item: any): string {
    return item?.area || index.toString();
  }

  // Tab management
  setActiveTab(tab: 'capability' | 'robustness' | 'bias' | 'hallucination' | 'drift'): void {
    this.activeTab.set(tab);
    this.error.set(null);
  }

  // Clear all results
  clearResults(): void {
    this.capabilityReport.set(null);
    this.robustnessReport.set(null);
    this.biasReport.set(null);
    this.hallucinationReport.set(null);
    this.driftReport.set(null);
    this.error.set(null);
  }

  // Open API documentation
  openApiDocs(): void {
    window.open('https://your-api-docs.com', '_blank');
  }

  // ========== API Methods ==========
  assessCapabilities(): void {
    // Parse dimensions from comma-separated string
    this.capabilityRequest.dimensions = this.capabilityDimensions
      .split(',')
      .map(d => d.trim())
      .filter(d => d.length > 0);
    
    // Ensure rigorLevel is the correct type
    const rigorLevel = this.capabilityRequest.rigorLevel;
    if (rigorLevel !== 'standard' && rigorLevel !== 'thorough') {
      this.capabilityRequest.rigorLevel = 'standard'; // Default to standard if invalid
    }
    
    this.loadingCapability.set(true);
    this.error.set(null);
    
    this.aiTestingService.assessAICapabilities(this.capabilityRequest).subscribe({
      next: (response) => {
        this.capabilityReport.set(response);
        this.loadingCapability.set(false);
      },
      error: (err) => {
        this.error.set(err);
        this.loadingCapability.set(false);
      }
    });
  }

  testRobustness(): void {
    // Parse variations from newline-separated string
    this.robustnessRequest.variations = this.robustnessVariations
      .split('\n')
      .map(v => v.trim())
      .filter(v => v.length > 0);
    
    this.loadingRobustness.set(true);
    this.error.set(null);
    
    this.aiTestingService.testPromptRobustness(this.robustnessRequest).subscribe({
      next: (response) => {
        this.robustnessReport.set(response);
        this.loadingRobustness.set(false);
      },
      error: (err) => {
        this.error.set(err);
        this.loadingRobustness.set(false);
      }
    });
  }

  detectBias(): void {
    // Parse context areas from comma-separated string
    this.biasRequest.contextAreas = this.biasContextAreas
      .split(',')
      .map(a => a.trim())
      .filter(a => a.length > 0);
    
    this.loadingBias.set(true);
    this.error.set(null);
    
    this.aiTestingService.detectAIBias(this.biasRequest).subscribe({
      next: (response) => {
        this.biasReport.set(response);
        this.loadingBias.set(false);
      },
      error: (err) => {
        this.error.set(err);
        this.loadingBias.set(false);
      }
    });
  }

  testHallucinations(): void {
    // Parse facts from newline-separated string
    this.hallucinationRequest.knownFacts = this.hallucinationFacts
      .split('\n')
      .map(f => f.trim())
      .filter(f => f.length > 0);
    
    this.loadingHallucination.set(true);
    this.error.set(null);
    
    this.aiTestingService.testForHallucinations(this.hallucinationRequest).subscribe({
      next: (response) => {
        this.hallucinationReport.set(response);
        this.loadingHallucination.set(false);
      },
      error: (err) => {
        this.error.set(err);
        this.loadingHallucination.set(false);
      }
    });
  }

  monitorDrift(): void {
    // Parse metrics from comma-separated string
    this.driftRequest.metricsToMonitor = this.driftMetrics
      .split(',')
      .map(m => m.trim())
      .filter(m => m.length > 0);
    
    this.loadingDrift.set(true);
    this.error.set(null);
    
    this.aiTestingService.monitorAIDrift(this.driftRequest).subscribe({
      next: (response) => {
        this.driftReport.set(response);
        this.loadingDrift.set(false);
      },
      error: (err) => {
        this.error.set(err);
        this.loadingDrift.set(false);
      }
    });
  }

  // Helper method to update rigor level from select input
  updateRigorLevel(value: string): void {
    if (value === 'standard' || value === 'thorough') {
      this.capabilityRequest.rigorLevel = value;
    }
  }
}