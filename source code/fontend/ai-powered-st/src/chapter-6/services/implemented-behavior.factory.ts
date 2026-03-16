// services/implemented-behavior.factory.ts
import { Injectable } from '@angular/core';
import { ImplementedBehavior, ImplementedBehaviorModel } from '../models/implemented-behavior.model';
import { BDDScenario } from '../models/bdd-scenario.model';

@Injectable({
  providedIn: 'root'
})
export class ImplementedBehaviorFactory {
  
  createEmptyBehavior(): ImplementedBehavior {
    return {
      scenarioId: '',
      steps: [],
      outcomes: [],
      edgeCases: [],
      lastUpdated: new Date()
    };
  }

  createFromScenario(scenario: BDDScenario): ImplementedBehavior {
    // Generate a scenario ID from title hash
    const scenarioId = this.generateScenarioId(scenario.title);
    
    // Convert Given/When/Then steps to implementation steps
    const steps = [
      ...scenario.given.map(g => `Setup: ${g}`),
      ...scenario.when.map(w => `Action: ${w}`)
    ];
    
    // Convert Then steps to outcomes
    const outcomes = scenario.then.map(t => `Verify: ${t}`);
    
    // Extract potential edge cases from examples
    const edgeCases = scenario.examples
      .filter(example => example.toLowerCase().includes('edge') || 
                        example.toLowerCase().includes('corner') ||
                        example.toLowerCase().includes('exception'))
      .map(example => `Edge case: ${example}`);
    
    return {
      scenarioId,
      steps,
      outcomes,
      edgeCases,
      lastUpdated: new Date()
    };
  }

  createDefaultBehavior(): ImplementedBehavior {
    return {
      scenarioId: 'default-scenario',
      steps: [
        'Initialize test environment',
        'Load test data',
        'Execute main flow'
      ],
      outcomes: [
        'Verify expected results',
        'Check response time',
        'Validate data integrity'
      ],
      edgeCases: [
        'Handle empty input',
        'Test maximum limits',
        'Simulate network failure'
      ],
      lastUpdated: new Date()
    };
  }

  createLoginBehavior(): ImplementedBehavior {
    return {
      scenarioId: 'login-scenario',
      steps: [
        'Navigate to login page',
        'Enter username',
        'Enter password',
        'Click login button'
      ],
      outcomes: [
        'User is redirected to dashboard',
        'Welcome message is displayed',
        'Session token is created'
      ],
      edgeCases: [
        'Invalid credentials',
        'Empty password field',
        'Account locked'
      ],
      lastUpdated: new Date()
    };
  }

  createRegistrationBehavior(): ImplementedBehavior {
    return {
      scenarioId: 'registration-scenario',
      steps: [
        'Navigate to registration page',
        'Fill registration form',
        'Submit form'
      ],
      outcomes: [
        'Success message displayed',
        'Confirmation email sent',
        'User redirected to login'
      ],
      edgeCases: [
        'Duplicate email address',
        'Weak password',
        'Invalid email format'
      ],
      lastUpdated: new Date()
    };
  }

  // Validation helper
  validateBehavior(behavior: ImplementedBehavior): { isValid: boolean; errors: string[] } {
    const errors: string[] = [];

    if (!behavior.scenarioId?.trim()) {
      errors.push('Scenario ID is required');
    }

    if (behavior.steps?.length === 0 && behavior.outcomes?.length === 0) {
      errors.push('At least one step or outcome is required');
    }

    if (!behavior.lastUpdated || isNaN(new Date(behavior.lastUpdated).getTime())) {
      errors.push('Valid lastUpdated date is required');
    }

    return {
      isValid: errors.length === 0,
      errors
    };
  }

  // Compare behaviors for drift detection
  compareBehaviors(
    documented: ImplementedBehavior, 
    implemented: ImplementedBehavior
  ): { similarity: number; differences: string[] } {
    const differences: string[] = [];
    let similarityScore = 0;
    const maxScore = 4; // Steps, outcomes, edge cases, recency

    // Compare steps
    const stepSimilarity = this.calculateArraySimilarity(documented.steps, implemented.steps);
    similarityScore += stepSimilarity;
    if (stepSimilarity < 0.8) {
      differences.push(`Step similarity: ${(stepSimilarity * 100).toFixed(0)}%`);
    }

    // Compare outcomes
    const outcomeSimilarity = this.calculateArraySimilarity(documented.outcomes, implemented.outcomes);
    similarityScore += outcomeSimilarity;
    if (outcomeSimilarity < 0.8) {
      differences.push(`Outcome similarity: ${(outcomeSimilarity * 100).toFixed(0)}%`);
    }

    // Compare edge cases
    const edgeCaseSimilarity = this.calculateArraySimilarity(documented.edgeCases, implemented.edgeCases);
    similarityScore += edgeCaseSimilarity;
    if (edgeCaseSimilarity < 0.6) {
      differences.push(`Edge case coverage: ${(edgeCaseSimilarity * 100).toFixed(0)}%`);
    }

    // Check recency (within 30 days is good)
    const implementedDate = new Date(implemented.lastUpdated);
    const daysSinceUpdate = Math.floor((Date.now() - implementedDate.getTime()) / (1000 * 60 * 60 * 24));
    const recencyScore = daysSinceUpdate <= 30 ? 1 : Math.max(0, 1 - (daysSinceUpdate - 30) / 30);
    similarityScore += recencyScore;
    if (recencyScore < 0.7) {
      differences.push(`Behavior is ${daysSinceUpdate} days old`);
    }

    return {
      similarity: similarityScore / maxScore,
      differences
    };
  }

  // Helper method to calculate array similarity
  private calculateArraySimilarity(arr1: string[], arr2: string[]): number {
    if (arr1.length === 0 && arr2.length === 0) return 1;
    if (arr1.length === 0 || arr2.length === 0) return 0;

    const set1 = new Set(arr1.map(item => item.toLowerCase()));
    const set2 = new Set(arr2.map(item => item.toLowerCase()));
    
    const intersection = new Set([...set1].filter(x => set2.has(x)));
    const union = new Set([...set1, ...set2]);
    
    return intersection.size / union.size;
  }

  // Generate scenario ID from title
  private generateScenarioId(title: string): string {
    // Simple hash function
    let hash = 0;
    for (let i = 0; i < title.length; i++) {
      const char = title.charCodeAt(i);
      hash = ((hash << 5) - hash) + char;
      hash = hash & hash; // Convert to 32bit integer
    }
    return `scenario-${Math.abs(hash)}`;
  }

  // Convert to model class
  toModel(behavior: ImplementedBehavior): ImplementedBehaviorModel {
    return new ImplementedBehaviorModel(behavior);
  }

  // Deep clone behavior
  cloneBehavior(behavior: ImplementedBehavior): ImplementedBehavior {
    return {
      scenarioId: behavior.scenarioId,
      steps: [...(behavior.steps || [])],
      outcomes: [...(behavior.outcomes || [])],
      edgeCases: [...(behavior.edgeCases || [])],
      lastUpdated: new Date(behavior.lastUpdated)
    };
  }

  // Merge behaviors
  mergeBehaviors(base: ImplementedBehavior, updates: Partial<ImplementedBehavior>): ImplementedBehavior {
    return {
      scenarioId: updates.scenarioId ?? base.scenarioId,
      steps: updates.steps ?? [...base.steps],
      outcomes: updates.outcomes ?? [...base.outcomes],
      edgeCases: updates.edgeCases ?? [...base.edgeCases],
      lastUpdated: updates.lastUpdated ? new Date(updates.lastUpdated) : new Date(base.lastUpdated)
    };
  }

  // Create behavior from template
  createFromTemplate(template: 'login' | 'registration' | 'search' | 'checkout' | 'api'): ImplementedBehavior {
    const templates = {
      login: this.createLoginBehavior(),
      registration: this.createRegistrationBehavior(),
      search: this.createSearchBehavior(),
      checkout: this.createCheckoutBehavior(),
      api: this.createApiBehavior()
    };

    return templates[template] || this.createDefaultBehavior();
  }

  private createSearchBehavior(): ImplementedBehavior {
    return {
      scenarioId: 'search-scenario',
      steps: [
        'Navigate to search page',
        'Enter search query',
        'Apply filters',
        'Execute search'
      ],
      outcomes: [
        'Results are displayed',
        'Search count is shown',
        'Results are relevant'
      ],
      edgeCases: [
        'Empty search results',
        'Special characters in query',
        'Very long search query'
      ],
      lastUpdated: new Date()
    };
  }

  private createCheckoutBehavior(): ImplementedBehavior {
    return {
      scenarioId: 'checkout-scenario',
      steps: [
        'Add items to cart',
        'Proceed to checkout',
        'Enter shipping details',
        'Select payment method'
      ],
      outcomes: [
        'Order is confirmed',
        'Receipt is generated',
        'Inventory is updated'
      ],
      edgeCases: [
        'Out of stock items',
        'Invalid shipping address',
        'Payment gateway timeout'
      ],
      lastUpdated: new Date()
    };
  }

  private createApiBehavior(): ImplementedBehavior {
    return {
      scenarioId: 'api-scenario',
      steps: [
        'Send HTTP request',
        'Include authentication headers',
        'Add request parameters'
      ],
      outcomes: [
        'Receive HTTP response',
        'Validate response format',
        'Check status code'
      ],
      edgeCases: [
        'Network timeout',
        'Invalid JSON response',
        'Rate limiting'
      ],
      lastUpdated: new Date()
    };
  }
}