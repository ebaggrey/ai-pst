// models/bdd-scenario.model.ts
export interface BDDScenario {
  title: string;
  given: string[];
  when: string[];
  then: string[];
  tags: string[];
  description: string;
  examples: string[];
}

// Optional: Create a class with helper methods
export class BDDScenarioModel implements BDDScenario {
  title: string = '';
  given: string[] = [];
  when: string[] = [];
  then: string[] = [];
  tags: string[] = [];
  description: string = '';
  examples: string[] = [];

  constructor(data?: Partial<BDDScenario>) {
    if (data) {
      Object.assign(this, data);
    }
  }

  // Helper method to get total steps
  get totalSteps(): number {
    return this.given.length + this.when.length + this.then.length;
  }

  // Helper method to check if scenario is valid
  isValid(): boolean {
    return !!this.title && 
           this.given.length > 0 && 
           this.when.length > 0 && 
           this.then.length > 0;
  }

  // Helper method to format scenario as text
  toText(): string {
    const lines: string[] = [];
    
    if (this.title) {
      lines.push(`Scenario: ${this.title}`);
      lines.push('');
    }
    
    if (this.description) {
      lines.push(this.description);
      lines.push('');
    }
    
    if (this.tags.length > 0) {
      lines.push(`Tags: ${this.tags.join(', ')}`);
      lines.push('');
    }
    
    if (this.given.length > 0) {
      this.given.forEach(step => lines.push(`Given ${step}`));
    }
    
    if (this.when.length > 0) {
      this.when.forEach(step => lines.push(`When ${step}`));
    }
    
    if (this.then.length > 0) {
      this.then.forEach(step => lines.push(`Then ${step}`));
    }
    
    if (this.examples.length > 0) {
      lines.push('');
      lines.push('Examples:');
      this.examples.forEach(example => lines.push(`  ${example}`));
    }
    
    return lines.join('\n');
  }

  // Helper method to create a deep copy
  clone(): BDDScenarioModel {
    return new BDDScenarioModel({
      title: this.title,
      given: [...this.given],
      when: [...this.when],
      then: [...this.then],
      tags: [...this.tags],
      description: this.description,
      examples: [...this.examples]
    });
  }

  // Helper method to update from another scenario
  updateFrom(other: Partial<BDDScenario>): void {
    Object.assign(this, other);
  }

  // Helper method to add a step
  addStep(type: 'given' | 'when' | 'then', step: string): void {
    this[type].push(step);
  }

  // Helper method to remove a step
  removeStep(type: 'given' | 'when' | 'then', index: number): void {
    if (index >= 0 && index < this[type].length) {
      this[type].splice(index, 1);
    }
  }

  // Helper method to add a tag
  addTag(tag: string): void {
    if (!this.tags.includes(tag)) {
      this.tags.push(tag);
    }
  }

  // Helper method to remove a tag
  removeTag(tag: string): void {
    const index = this.tags.indexOf(tag);
    if (index >= 0) {
      this.tags.splice(index, 1);
    }
  }

  // Static method to create from text
  static fromText(text: string): BDDScenarioModel {
    const lines = text.split('\n').filter(line => line.trim());
    const scenario = new BDDScenarioModel();
    
    lines.forEach(line => {
      const trimmed = line.trim();
      
      if (trimmed.startsWith('Scenario:')) {
        scenario.title = trimmed.replace('Scenario:', '').trim();
      } else if (trimmed.startsWith('Given ')) {
        scenario.given.push(trimmed.replace('Given ', '').trim());
      } else if (trimmed.startsWith('When ')) {
        scenario.when.push(trimmed.replace('When ', '').trim());
      } else if (trimmed.startsWith('Then ')) {
        scenario.then.push(trimmed.replace('Then ', '').trim());
      } else if (trimmed.startsWith('Tags:')) {
        scenario.tags = trimmed.replace('Tags:', '').trim().split(',').map(t => t.trim());
      } else if (trimmed.startsWith('Examples:')) {
        // Examples handled separately
      } else if (!scenario.title && trimmed) {
        // First non-empty line without prefix is description
        scenario.description = trimmed;
      }
    });
    
    return scenario;
  }

  // Static method to create default scenario
  static default(): BDDScenarioModel {
    return new BDDScenarioModel({
      title: 'New Scenario',
      given: ['the user is on the application'],
      when: ['the user performs an action'],
      then: ['the system should respond appropriately'],
      tags: ['new', 'untested'],
      description: 'Describe the scenario here...',
      examples: []
    });
  }
}