// components/bdd-test/bdd-test.component.ts
import { Component, signal } from '@angular/core';
import { CommonModule, DecimalPipe, JsonPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BDDSuperchargedService } from '../../services/bdd-supercharged.service';
import {
  BDCCoCreationRequest,
  AutomationRequest,
  EvolutionRequest,
  DriftDetectionRequest,
  DocumentationRequest,
  BDDScenario,
  ImplementedBehavior
} from '../../models/api-models';

@Component({
  selector: 'app-bdd-test',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  templateUrl: './bdd-test.component.html',
  styleUrls: ['./bdd-test.component.css']
})
export class BDDTestComponent {
  environment = environment;
  
  // Signals for UI state
  selectedEndpoint = signal<string>('co-create');
  loading = signal<boolean>(false);
  responseData = signal<any>(null);
  errorMessage = signal<string>('');

  // Example request templates with all required properties
  coCreateRequest: BDCCoCreationRequest = {
    requirement: 'As a user, I want to login to access my account',
    stakeholderPerspectives: [
      {
        role: 'End User',
        priorities: ['Ease of use', 'Security'],
        concerns: ['Password recovery']
      }
    ],
    conversationConstraints: {
      maxRounds: 3,
      consensusThreshold: 0.7,
      forbiddenAssumptions: []
    },
    desiredOutcomes: ['Clear login scenarios']
  };

  automationRequest: AutomationRequest = {
    scenario: {
      title: 'User Login',
      given: ['the user is on login page', 'the user has valid credentials'],
      when: ['the user enters credentials', 'the user clicks login'],
      then: ['the user is redirected to dashboard', 'a session is created'],
      tags: ['authentication', 'login'],
      description: 'Successful user login',
      examples: []
    },
    techContext: {
      stack: 'dotnet',
      testFramework: 'xunit',
      libraries: [],
      constraints: []
    },
    translationStyle: 'declarative',
    qualityTargets: {
      minCoverage: 0.8,
      maxComplexity: 10,
      maxDependencies: 5,
      validationRules: []
    }
  };

  evolutionRequest: EvolutionRequest = {
    existingScenarios: [
      {
        title: 'User Registration',
        given: ['the user is on registration page'],
        when: ['the user enters valid details', 'the user submits form'],
        then: ['account is created', 'confirmation email is sent'],
        tags: ['registration'],
        description: 'User registration flow',
        examples: []
      }
    ],
    newInformation: 'Add GDPR consent checkbox',
    breakingChanges: [
      {
        type: 'addition',
        description: 'Add consent checkbox to form',
        impactLevel: 'medium',
        affectedAreas: ['registration-form']
      }
    ],
    validationRules: [],
    evolutionStrategy: 'preserve-intent'
  };

  // Fixed Drift Detection Request with all required properties
  driftRequest: DriftDetectionRequest = {
    documentedScenarios: [
      {
        title: 'User Login',
        given: ['the user is on login page'],
        when: ['the user enters credentials', 'the user clicks login'],
        then: ['the user is redirected to dashboard'],
        tags: ['login'],
        description: 'Login scenario',
        examples: []
      }
    ],
    implementedBehavior: [
      {
        scenarioId: 'user-login',
        steps: ['Navigate to login', 'Enter credentials', 'Click login'],
        outcomes: ['Check redirect to dashboard'],
        edgeCases: [],
        lastUpdated: new Date().toISOString()
      }
    ],
    detectionMethods: ['semantic', 'structural'],
    sensitivity: 0.7,
    autoSuggestFixes: true,
    // Additional properties
    baseline: 'production',
    timeframe: 'last-30-days',
    metricsToMonitor: ['step-coverage', 'outcome-accuracy', 'edge-case-coverage'],
    driftThreshold: 0.15,
    alertOnDrift: true,
    includeRecommendations: true,
    maxFindings: 50,
    compareMode: 'strict',
    ignorePatterns: ['timestamp', 'session-id']
  };

  documentationRequest: DocumentationRequest = {
    scenarios: [
      {
        title: 'Search Products',
        given: ['the user is on homepage'],
        when: ['the user enters search term', 'the user clicks search'],
        then: ['products are displayed', 'results are paginated'],
        tags: ['search', 'product'],
        description: 'Product search functionality',
        examples: []
      }
    ],
    testResults: [
      {
        scenarioId: 'search-products',
        passed: true,
        errors: [],
        executionTime: new Date().toISOString(),
        duration: 1.5
      }
    ],
    audience: {
      role: 'developer',
      technicalLevel: 'intermediate',
      interests: [],
      constraints: []
    },
    format: 'html',
    include: ['test-results'],
    updateStrategy: {
      trigger: 'scenario-change',
      autoUpdate: true,
      notifyRoles: [],
      versioning: 'semantic'
    }
  };

  // Available endpoints
  endpoints = [
    { id: 'co-create', name: 'Co-create Scenarios' },
    { id: 'translate', name: 'Translate to Automation' },
    { id: 'evolve', name: 'Evolve Scenarios' },
    { id: 'drift', name: 'Detect Drift' },
    { id: 'documentation', name: 'Generate Documentation' }
  ];

  constructor(private bddService: BDDSuperchargedService) {}

  setEndpoint(endpointId: string): void {
    this.selectedEndpoint.set(endpointId);
    this.responseData.set(null);
    this.errorMessage.set('');
  }

  executeRequest(): void {
    this.loading.set(true);
    this.responseData.set(null);
    this.errorMessage.set('');

    const endpoint = this.selectedEndpoint();
    let request$;

    switch (endpoint) {
      case 'co-create':
        request$ = this.bddService.coCreateScenarios(this.coCreateRequest);
        break;
      case 'translate':
        request$ = this.bddService.translateScenarioToAutomation(this.automationRequest);
        break;
      case 'evolve':
        request$ = this.bddService.evolveScenarios(this.evolutionRequest);
        break;
      case 'drift':
        request$ = this.bddService.detectScenarioDrift(this.driftRequest);
        break;
      case 'documentation':
        request$ = this.bddService.generateLivingDocumentation(this.documentationRequest);
        break;
      default:
        return;
    }

    request$.subscribe({
      next: (response) => {
        this.responseData.set(response);
        this.loading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(error.message || 'An error occurred');
        this.loading.set(false);
        console.error('API Error:', error);
      }
    });
  }

  getCurrentRequest(): any {
    switch (this.selectedEndpoint()) {
      case 'co-create':
        return this.coCreateRequest;
      case 'translate':
        return this.automationRequest;
      case 'evolve':
        return this.evolutionRequest;
      case 'drift':
        return this.driftRequest;
      case 'documentation':
        return this.documentationRequest;
      default:
        return null;
    }
  }

  getCurrentTimestamp(): string {
    return new Date().toLocaleString();
  }
}