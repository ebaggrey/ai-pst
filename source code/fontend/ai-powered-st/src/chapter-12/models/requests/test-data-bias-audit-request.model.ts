// src/app/models/requests/test-data-bias-audit-request.model.ts
export interface TestDataBiasAuditRequest {
  datasetName: string;
  dataSample: Record<string, any>[];  // Array of objects
  dataContext?: string;
  suggestionCount: number;
  aiPrompt?: string;
}

// Example usage:
/*
const exampleRequest: TestDataBiasAuditRequest = {
  datasetName: 'hiring-data-2024',
  dataContext: 'job applications',
  suggestionCount: 5,
  dataSample: [
    {
      name: 'John Smith',
      age: 45,
      gender: 'Male',
      ethnicity: 'Caucasian',
      experience: 20,
      position: 'Senior Manager'
    },
    {
      name: 'Maria Garcia',
      age: 32,
      gender: 'Female',
      ethnicity: 'Hispanic',
      experience: 8,
      position: 'Team Lead'
    }
  ]
};
*/