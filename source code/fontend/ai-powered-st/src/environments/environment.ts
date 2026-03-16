import { AITestingConfig } from "src/chapter-4/configuration";

export const environment = {
  production: false,
  apiBaseUrl: 'http://localhost:5000', // Your backend URL
  testingApiUrl: 'https://your-api-base-url.com',  // Update with your actual API URL
  patternEstablishmentAPiUrl:'http://localhost:5000/api', // Update with your backend URL
  pipelineCookbookApiUrl:'http://localhost:5000/api',
  legacyConquestAPiUrl:'http://localhost:7025/api',
  leanTestingAPiUrl:'http://localhost:5001/api',
  metricsThatMatterAPIUrl:'http://localhost:7066/api',
  fullSpectrumAPIUrl:'http://localhost:7066/api',
  dataBiasAPIUrl:'http://localhost:5001/api', 
  tddReimaginedAPIUrl:'http://localhost:5001/api', 
  bddsuperchargedAPiUrl:'http://localhost:5001/api',
 
  // Optional: Add other environment-specific configurations
  onboarding: {
    defaultTimeout: 300000, // 5 minutes
    maxQuestions: 20,
    supportedFixStrategies: ['conservative', 'aggressive', 'refactor']
  },
    aiTestingConfig: {
    apiBaseUrl: 'http://localhost:5000/api/ai-testing',
    enableCaching: false,
    enableDetailedLogging: true,
    requestTimeout: 60000, // Longer timeout for development
    retryAttempts: 2
  } as Partial<AITestingConfig>

};
