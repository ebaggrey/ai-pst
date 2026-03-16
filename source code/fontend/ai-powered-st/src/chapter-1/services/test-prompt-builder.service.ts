import { Injectable } from "@angular/core";
import { TestGenerationRequest, LLMProvider } from "../models/test-generation-request.model";
import { TestRunHistory,AnalysisRequest } from "../models/test-generation-request.model";

// services/test-prompt-builder.service.ts
@Injectable({
  providedIn: 'root'
})
export class TestPromptBuilderService {
  
  createExploratoryPrompt(feature: string, riskAreas: string[]): TestGenerationRequest {
    const prompt = `I need to test the "${feature}" feature. 
    I'm particularly worried about: ${riskAreas.join(', ')}.
    Give me 3-5 exploratory test scenarios that would uncover hidden 
    issues.
    Focus on edge cases and unhappy paths.`;
    
    return {
      prompt: prompt,
      promptType: 'exploratory',
      llmProvider: 'claude' as LLMProvider,
      context: `Application is a modern SPA with async API calls.
                Testing priority: finding hidden bugs over happy 
      path coverage.`,
      metadata: {
        urgency: 'thoughtful',
        expectedOutput: 'test scenarios',
        format: 'bullet points with risk ratings',
        tags: ['exploratory', 'manual-testing', 'risk-based']
      }
    };
  }

  createAutomationScriptPrompt(userStory: string, techStack: string): TestGenerationRequest {
    const prompt = `Write a complete automation test for: 
                    "${userStory}".
                     Use ${techStack} with explicit waits and robust     
                     selectors.
    Include setup, teardown, and at least three meaningful 
    assertions. `;
    
    return {
      prompt: prompt,
      promptType: 'automation',
      llmProvider: 'deepseek' as LLMProvider,
      context: `Generate production-ready test code with proper 
                 error handling.
                 Use Page Object Model pattern where appropriate.
                 Include comments for complex logic.`,
      metadata: {
        urgency: 'balanced',
        expectedOutput: 'executable code',
        format: 'complete test file with imports',
        tags: ['automation', 'code-generation', 'e2e']
      }
    };
  }

  createSecurityTestPrompt(feature: string, securityConcerns: string[]): TestGenerationRequest {
    const prompt = `Analyze "${feature}" for security vulnerabilities.
    Security concerns: ${securityConcerns.join(', ')}.
    Provide security test cases including OWASP Top 10 considerations.`;
    
    return {
      prompt: prompt,
      promptType: 'security',
      llmProvider: 'gemini' as LLMProvider,
      context: 'Focus on authentication, authorization, data validation, and injection vulnerabilities.',
      metadata: {
        urgency: 'standard',
        expectedOutput: 'security test scenarios',
        format: 'threat model + test cases',
        tags: ['security', 'penetration-testing', 'owasp']
      }
    };
  }

  createPerformanceTestPrompt(endpoint: string, loadProfile: string): TestGenerationRequest {
    const prompt = `Design performance tests for endpoint: ${endpoint}
    Load profile: ${loadProfile}
    Include stress, load, and spike testing scenarios.`;
    
    return {
      prompt: prompt,
      promptType: 'performance',
      llmProvider: 'deepseek' as LLMProvider,
      context: 'Include metrics collection, performance thresholds, and monitoring setup.',
      metadata: {
        urgency: 'fast',
        expectedOutput: 'performance test plan',
        format: 'test scenarios with KPI targets',
        tags: ['performance', 'load-testing', 'scalability']
      }
    };
  }

  createFlakyAnalysisPrompt(testHistory: TestRunHistory): AnalysisRequest {
    const failures = testHistory.runs.filter(r => !r.passed).length;
    
    return {
      prompt: `Analyze this test history for flaky patterns:
               Total runs: ${testHistory.runs.length}
               Failures: ${failures}
               Error samples: 
               ${testHistory.getRecentErrors().join('; ')}
               What makes these tests unstable? Suggest fixes.`,
      promptType: 'analysis',
      llmProvider: 'gemini' as LLMProvider,
      context: 'Focus on timing issues, selector problems, and test dependencies.',
      metadata: {
        urgency: 'standard',
        expectedOutput: 'root cause analysis',
        format: 'problem -> solution pairs'
      }
    };
  }

  // Helper method to create test run history from raw data
  createTestRunHistory(data: Partial<TestRunHistory>): TestRunHistory {
    return {
      runs: data.runs || [],
      testId: data.testId || 'unknown',
      testName: data.testName || 'Unnamed Test',
      getRecentErrors: function() {
        return this.runs
          .filter(r => r.error)
          .slice(-5) // Last 5 errors
          .map(r => r.error?.message || 'Unknown error');
      },
      getFlakinessScore: function() {
        if (this.runs.length === 0) return 0;
        const failures = this.runs.filter(r => !r.passed).length;
        return (failures / this.runs.length) * 10; // Score 0-10
      }
    };
  }

  // New method to generate API test prompts
  createApiTestPrompt(apiEndpoint: string, httpMethod: string, requestSchema: any): TestGenerationRequest {
    const prompt = `Generate comprehensive API tests for:
    Endpoint: ${apiEndpoint}
    Method: ${httpMethod}
    Request Schema: ${JSON.stringify(requestSchema, null, 2)}
    
    Include tests for:
    1. Valid requests
    2. Invalid inputs
    3. Edge cases
    4. Authentication/Authorization
    5. Rate limiting
    6. Error responses`;
    
    return {
      prompt: prompt,
      promptType: 'automation',
      llmProvider: 'deepseek' as LLMProvider,
      context: 'Use appropriate testing framework (e.g., Jest, Mocha) and include setup/teardown.',
      metadata: {
        urgency: 'balanced',
        expectedOutput: 'API test suite',
        format: 'code with test cases and assertions',
        tags: ['api-testing', 'integration', 'rest']
      }
    };
  }
}