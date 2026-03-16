// services/example-usage.service.ts
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BDDSuperchargedService } from './bdd-supercharged.service';

@Injectable({
  providedIn: 'root'
})
export class ExampleUsageService {
  constructor(private bddService: BDDSuperchargedService) {}

  // 1. Example: Co-create scenarios
  exampleCoCreateScenarios(): Observable<any> {
    const request = {
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

    return this.bddService.coCreateScenarios(request);
  }

  // 2. Example: Translate scenario to automation
  exampleTranslateToAutomation(): Observable<any> {
    const request = {
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

    return this.bddService.translateScenarioToAutomation(request);
  }

  // 3. Example: Evolve scenarios
  exampleEvolveScenarios(): Observable<any> {
    const request = {
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

    return this.bddService.evolveScenarios(request);
  }

  // 4. Example: Detect drift
  exampleDetectDrift(): Observable<any> {
    // Create documented scenarios
    const documentedScenarios = [
      {
        title: 'User Login',
        given: ['the user is on login page'],
        when: ['the user enters credentials', 'the user clicks login'],
        then: ['the user is redirected to dashboard'],
        tags: ['login'],
        description: 'Login scenario',
        examples: []
      }
    ];

    // Create implemented behavior
    const implementedBehavior = [
      {
        scenarioId: 'user-login',
        steps: ['Navigate to login', 'Enter credentials', 'Click login'],
        outcomes: ['Check redirect to dashboard'],
        edgeCases: [],
        lastUpdated: new Date().toISOString()
      }
    ];

    const request = {
      DocumentedScenarios: documentedScenarios,
      ImplementedBehavior: implementedBehavior,
      DetectionMethods: ['semantic', 'structural'],
      Sensitivity: 0.7,
      AutoSuggestFixes: true
    };

    return this.bddService.detectScenarioDrift(request);
  }

  // 5. Example: Generate documentation
  exampleGenerateDocumentation(): Observable<any> {
    const request = {
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

    return this.bddService.generateLivingDocumentation(request);
  }

  // Helper: Get all examples as an array
  getAllExamples(): Array<{ name: string; method: () => Observable<any> }> {
    return [
      { name: 'Co-create Scenarios', method: () => this.exampleCoCreateScenarios() },
      { name: 'Translate to Automation', method: () => this.exampleTranslateToAutomation() },
      { name: 'Evolve Scenarios', method: () => this.exampleEvolveScenarios() },
      { name: 'Detect Drift', method: () => this.exampleDetectDrift() },
      { name: 'Generate Documentation', method: () => this.exampleGenerateDocumentation() }
    ];
  }

  // Helper: Run example by name
  runExampleByName(name: string): Observable<any> {
    const example = this.getAllExamples().find(ex => ex.name === name);
    if (example) {
      return example.method();
    }
    throw new Error(`Example "${name}" not found`);
  }

  // Simple test method for debugging
  testConnection(): Observable<any> {
    // Simple request to test if service is working
    const testRequest = {
      requirement: 'Test connection',
      stakeholderPerspectives: [],
      conversationConstraints: {
        maxRounds: 1,
        consensusThreshold: 0.5,
        forbiddenAssumptions: []
      },
      desiredOutcomes: []
    };

    return this.bddService.coCreateScenarios(testRequest);
  }
}