// enums/rigor-level.enum.ts
export enum RigorLevel {
  Standard = 'standard',
  Thorough = 'thorough'
}

// enums/severity.enum.ts
export enum Severity {
  Low = 'low',
  Medium = 'medium',
  High = 'high',
  Critical = 'critical'
}

// enums/priority.enum.ts
export enum Priority {
  Low = 'low',
  Medium = 'medium',
  High = 'high',
  Critical = 'critical'
}

// enums/bias-type.enum.ts
export enum BiasType {
  Demographic = 'demographic',
  Cultural = 'cultural',
  Gender = 'gender',
  Racial = 'racial',
  Age = 'age',
  Socioeconomic = 'socioeconomic',
  Political = 'political',
  Religious = 'religious'
}

// enums/hallucination-category.enum.ts
export enum HallucinationCategory {
  Factual = 'factual',
  Numerical = 'numerical',
  Temporal = 'temporal',
  Geographical = 'geographical',
  Personal = 'personal',
  Logical = 'logical',
  Citation = 'citation'
}

// enums/test-status.enum.ts
export enum TestStatus {
  Pending = 'pending',
  Running = 'running',
  Completed = 'completed',
  Failed = 'failed',
  Cancelled = 'cancelled'
}

// enums/provider.enum.ts
export enum AIProvider {
  OpenAI = 'openai',
  Anthropic = 'anthropic',
  Google = 'google',
  Azure = 'azure',
  AWS = 'aws',
  HuggingFace = 'huggingface'
}