// services/bdd-scenario.factory.ts
import { Injectable } from '@angular/core';
import { BDDScenario, BDDScenarioModel } from '../models/bdd-scenario.model';

@Injectable({
  providedIn: 'root'
})
export class BDDScenarioFactory {
  
  createEmptyScenario(): BDDScenario {
    return {
      title: '',
      given: [],
      when: [],
      then: [],
      tags: [],
      description: '',
      examples: []
    };
  }

  createDefaultScenario(): BDDScenario {
    return {
      title: 'New Scenario',
      given: ['the system is initialized'],
      when: ['an action is performed'],
      then: ['an expected outcome occurs'],
      tags: ['new', 'untested'],
      description: 'Describe your scenario here...',
      examples: []
    };
  }

  createLoginScenario(): BDDScenario {
    return {
      title: 'User Login Success',
      given: ['the user is registered', 'the user is on the login page'],
      when: ['the user enters valid credentials', 'the user clicks login'],
      then: ['the user is redirected to dashboard', 'a session is created'],
      tags: ['authentication', 'login', 'smoke'],
      description: 'Successful user authentication',
      examples: ['Username: user@example.com', 'Password: Password123!']
    };
  }

  createRegistrationScenario(): BDDScenario {
    return {
      title: 'User Registration',
      given: ['the user is on registration page'],
      when: ['the user enters valid registration details', 'the user submits the form'],
      then: ['a new account is created', 'a confirmation email is sent'],
      tags: ['registration', 'user-management'],
      description: 'New user account creation',
      examples: []
    };
  }

  createSearchScenario(): BDDScenario {
    return {
      title: 'Product Search',
      given: ['the user is on the homepage'],
      when: ['the user enters a search term', 'the user clicks search'],
      then: ['relevant products are displayed', 'search results are paginated'],
      tags: ['search', 'product', 'navigation'],
      description: 'Search functionality for products',
      examples: ['Search term: "laptop"', 'Category: "electronics"']
    };
  }

  // Validation helper
  validateScenario(scenario: BDDScenario): { isValid: boolean; errors: string[] } {
    const errors: string[] = [];

    if (!scenario.title?.trim()) {
      errors.push('Scenario title is required');
    }

    if (scenario.given?.length === 0) {
      errors.push('At least one Given step is required');
    }

    if (scenario.when?.length === 0) {
      errors.push('At least one When step is required');
    }

    if (scenario.then?.length === 0) {
      errors.push('At least one Then step is required');
    }

    return {
      isValid: errors.length === 0,
      errors
    };
  }

  // Create scenario from template
  createFromTemplate(template: 'login' | 'registration' | 'search' | 'checkout' | 'payment'): BDDScenario {
    const templates = {
      login: this.createLoginScenario(),
      registration: this.createRegistrationScenario(),
      search: this.createSearchScenario(),
      checkout: this.createCheckoutScenario(),
      payment: this.createPaymentScenario()
    };

    return templates[template] || this.createDefaultScenario();
  }

  private createCheckoutScenario(): BDDScenario {
    return {
      title: 'Checkout Process',
      given: ['the user has items in cart', 'the user is logged in'],
      when: ['the user proceeds to checkout', 'the user enters shipping information'],
      then: ['the order is created', 'a confirmation is displayed'],
      tags: ['checkout', 'ecommerce', 'order'],
      description: 'Complete checkout process',
      examples: []
    };
  }

  private createPaymentScenario(): BDDScenario {
    return {
      title: 'Payment Processing',
      given: ['the user is at payment step', 'the order total is calculated'],
      when: ['the user enters payment details', 'the user confirms payment'],
      then: ['payment is processed', 'receipt is generated'],
      tags: ['payment', 'ecommerce', 'financial'],
      description: 'Payment gateway integration',
      examples: ['Payment method: Credit Card', 'Currency: USD']
    };
  }

  // Convert to model class
  toModel(scenario: BDDScenario): BDDScenarioModel {
    return new BDDScenarioModel(scenario);
  }

  // Deep clone scenario
  cloneScenario(scenario: BDDScenario): BDDScenario {
    return {
      title: scenario.title,
      given: [...(scenario.given || [])],
      when: [...(scenario.when || [])],
      then: [...(scenario.then || [])],
      tags: [...(scenario.tags || [])],
      description: scenario.description,
      examples: [...(scenario.examples || [])]
    };
  }

  // Merge scenarios
  mergeScenarios(base: BDDScenario, updates: Partial<BDDScenario>): BDDScenario {
    return {
      title: updates.title ?? base.title,
      given: updates.given ?? [...base.given],
      when: updates.when ?? [...base.when],
      then: updates.then ?? [...base.then],
      tags: updates.tags ?? [...base.tags],
      description: updates.description ?? base.description,
      examples: updates.examples ?? [...base.examples]
    };
  }
}