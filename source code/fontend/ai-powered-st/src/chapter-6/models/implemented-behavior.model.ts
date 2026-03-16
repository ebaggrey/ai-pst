// models/implemented-behavior.model.ts
export interface ImplementedBehavior {
  scenarioId: string;
  steps: string[];
  outcomes: string[];
  edgeCases: string[];
  lastUpdated: Date;
}

// Optional: Create a class with helper methods
export class ImplementedBehaviorModel implements ImplementedBehavior {
  scenarioId: string = '';
  steps: string[] = [];
  outcomes: string[] = [];
  edgeCases: string[] = [];
  lastUpdated: Date = new Date();

  constructor(data?: Partial<ImplementedBehavior>) {
    if (data) {
      Object.assign(this, data);
      
      // Ensure lastUpdated is a Date object
      if (data.lastUpdated && !(data.lastUpdated instanceof Date)) {
        this.lastUpdated = new Date(data.lastUpdated);
      }
    }
  }

  // Helper method to get total actions (steps + outcomes)
  get totalActions(): number {
    return this.steps.length + this.outcomes.length;
  }

  // Helper method to check if behavior is valid
  isValid(): boolean {
    return !!this.scenarioId && 
           (this.steps.length > 0 || this.outcomes.length > 0);
  }

  // Helper method to format behavior as text
  toText(): string {
    const lines: string[] = [];
    
    lines.push(`Scenario ID: ${this.scenarioId}`);
    lines.push(`Last Updated: ${this.lastUpdated.toLocaleString()}`);
    lines.push('');
    
    if (this.steps.length > 0) {
      lines.push('Steps:');
      this.steps.forEach(step => lines.push(`  - ${step}`));
      lines.push('');
    }
    
    if (this.outcomes.length > 0) {
      lines.push('Outcomes:');
      this.outcomes.forEach(outcome => lines.push(`  - ${outcome}`));
      lines.push('');
    }
    
    if (this.edgeCases.length > 0) {
      lines.push('Edge Cases:');
      this.edgeCases.forEach(edgeCase => lines.push(`  - ${edgeCase}`));
    }
    
    return lines.join('\n');
  }

  // Helper method to create a deep copy
  clone(): ImplementedBehaviorModel {
    return new ImplementedBehaviorModel({
      scenarioId: this.scenarioId,
      steps: [...this.steps],
      outcomes: [...this.outcomes],
      edgeCases: [...this.edgeCases],
      lastUpdated: new Date(this.lastUpdated)
    });
  }

  // Helper method to update from another behavior
  updateFrom(other: Partial<ImplementedBehavior>): void {
    Object.assign(this, other);
    
    // Handle Date conversion for lastUpdated
    if (other.lastUpdated && !(other.lastUpdated instanceof Date)) {
      this.lastUpdated = new Date(other.lastUpdated);
    }
  }

  // Helper method to add a step
  addStep(step: string): void {
    this.steps.push(step);
    this.updateTimestamp();
  }

  // Helper method to remove a step
  removeStep(index: number): void {
    if (index >= 0 && index < this.steps.length) {
      this.steps.splice(index, 1);
      this.updateTimestamp();
    }
  }

  // Helper method to add an outcome
  addOutcome(outcome: string): void {
    this.outcomes.push(outcome);
    this.updateTimestamp();
  }

  // Helper method to remove an outcome
  removeOutcome(index: number): void {
    if (index >= 0 && index < this.outcomes.length) {
      this.outcomes.splice(index, 1);
      this.updateTimestamp();
    }
  }

  // Helper method to add an edge case
  addEdgeCase(edgeCase: string): void {
    this.edgeCases.push(edgeCase);
    this.updateTimestamp();
  }

  // Helper method to remove an edge case
  removeEdgeCase(index: number): void {
    if (index >= 0 && index < this.edgeCases.length) {
      this.edgeCases.splice(index, 1);
      this.updateTimestamp();
    }
  }

  // Helper method to update timestamp
  private updateTimestamp(): void {
    this.lastUpdated = new Date();
  }

  // Helper method to check if behavior matches a scenario
  matchesScenario(scenarioId: string): boolean {
    return this.scenarioId === scenarioId;
  }

  // Helper method to get behavior age in days
  get ageInDays(): number {
    const now = new Date();
    const diffTime = Math.abs(now.getTime() - this.lastUpdated.getTime());
    return Math.floor(diffTime / (1000 * 60 * 60 * 24));
  }

  // Static method to create from scenario
  static fromScenario(scenarioId: string, steps: string[] = [], outcomes: string[] = []): ImplementedBehaviorModel {
    return new ImplementedBehaviorModel({
      scenarioId,
      steps,
      outcomes,
      edgeCases: [],
      lastUpdated: new Date()
    });
  }

  // Static method to create default behavior
  static default(): ImplementedBehaviorModel {
    return new ImplementedBehaviorModel({
      scenarioId: 'unknown-scenario',
      steps: ['Initialize system', 'Execute action'],
      outcomes: ['Verify result', 'Clean up resources'],
      edgeCases: ['Network timeout', 'Invalid input'],
      lastUpdated: new Date()
    });
  }

  // Static method to parse from JSON string
  static fromJson(json: string): ImplementedBehaviorModel {
    try {
      const data = JSON.parse(json);
      return new ImplementedBehaviorModel(data);
    } catch (error) {
      console.error('Failed to parse ImplementedBehavior from JSON:', error);
      return ImplementedBehaviorModel.default();
    }
  }

  // Convert to JSON string
  toJson(): string {
    return JSON.stringify(this, null, 2);
  }
}