// services/tdd-data.service.ts
import { Injectable } from '@angular/core';
import {
  TDDRequest, UserStory, Constraint, 
  ImplementationRequest, GeneratedTest, FailureDetails,
  RefactorRequest, TestSuite, RefactoringGoal, SafetyMeasures,
  FuturePredictionRequest, ProductRoadmap, 
  CodeSnippet} from '../models/tdd-models';

@Injectable({
  providedIn: 'root'
})
export class TDDDataService {
  
  // Create sample TDD request
  createSampleTDDRequest(): TDDRequest {
    return {
      userStory: this.createSampleUserStory(),
      tddStyle: 'classic',
      constraints: this.createSampleConstraints(),
      generateMultipleApproaches: true,
      maxComplexityLevel: 5
    };
  }

  // Create sample implementation request
  createSampleImplementationRequest(): ImplementationRequest {
    return {
      failingTest: this.createSampleGeneratedTest(),
      failureDetails: this.createSampleFailureDetails(),
      implementationStrategy: 'simplest-first',
      constraints: this.createSampleConstraints(),
      allowMultipleImplementations: true
    };
  }

  // Create sample refactor request
  createSampleRefactorRequest(): RefactorRequest {
    return {
      workingCode: this.createSampleCodeSnippet(),
      testSuite: this.createSampleTestSuite(),
      refactoringGoals: this.createSampleRefactoringGoals(),
      safetyMeasures: this.createSampleSafetyMeasures(),
      constraints: this.createSampleConstraints()
    };
  }

  // Create sample future prediction request
  createSampleFuturePredictionRequest(): FuturePredictionRequest {
    return {
      currentCode: this.createSampleCodeSnippet(),
      productRoadmap: this.createSampleProductRoadmap(),
      timeHorizon: 'quarterly',
      confidenceThreshold: 0.7,
      focusAreas: ['scalability', 'security']
    };
  }

  // Helper methods to create sample data
  private createSampleUserStory(): UserStory {
    return {
      id: 'US-123',
      title: 'User Registration',
      description: 'As a new user, I want to register for an account so that I can access the system.',
      acceptanceCriteria: [
        {
          id: 'AC-1',
          description: 'User can provide email and password',
          testConditions: ['Email is valid', 'Password meets requirements']
        },
        {
          id: 'AC-2',
          description: 'System validates unique email',
          testConditions: ['Email not already registered', 'Validation error shown if duplicate']
        }
      ],
      businessRules: [
        'Password must be at least 8 characters',
        'Email must be in valid format'
      ],
      examples: [
        {
          scenario: 'Successful registration',
          given: 'New user with valid email and password',
          when: 'User submits registration form',
          then: 'Account is created and confirmation email sent',
          expectedResult: 'User can log in with new credentials'
        }
      ]
    };
  }

  private createSampleGeneratedTest(): GeneratedTest {
    return {
      testCode: `[Fact]
public void User_Registration_Should_Create_Account()
{
    // Arrange
    var registrationService = new RegistrationService();
    var userDto = new UserDto { Email = "test@example.com", Password = "Password123!" };
    
    // Act
    var result = registrationService.Register(userDto);
    
    // Assert
    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
    result.UserId.Should().NotBeEmpty();
}`,
      testFramework: 'xUnit',
      testName: 'User_Registration_Should_Create_Account',
      dependencies: ['xunit', 'Moq', 'FluentAssertions'],
      isFailingByDesign: true,
      expectedFailure: {
        expected: 'Registration succeeds with valid input',
        actual: 'System.NotImplementedException',
        message: 'Test should fail because RegistrationService is not implemented'
      }
    };
  }

  private createSampleCodeSnippet(): CodeSnippet {
    return {
      id: 'code-001',
      language: 'csharp',
      code: `public class RegistrationService
{
    public RegistrationResult Register(UserDto userDto)
    {
        // TODO: Implement registration logic
        throw new NotImplementedException();
    }
}`,
      dependencies: [],
      complexityMetrics: {
        cyclomaticComplexity: 1,
        linesOfCode: 7,
        methodCount: 1,
        depthOfInheritance: 1,
        classCoupling: 2
      }
    };
  }

  private createSampleConstraints(): Constraint[] {
    return [
      {
        type: 'time',
        value: '2h',
        description: 'Complete within 2 hours'
      },
      {
        type: 'complexity',
        value: '5',
        description: 'Maximum cyclomatic complexity of 5'
      }
    ];
  }

  private createSampleFailureDetails(): FailureDetails {
    return {
      expected: 'RegistrationResult with Success = true',
      actual: 'NotImplementedException thrown',
      message: 'Service implementation is missing'
    };
  }

  private createSampleTestSuite(): TestSuite {
    return {
      name: 'RegistrationServiceTests',
      tests: [this.createSampleGeneratedTest()],
      framework: 'xUnit',
      lastRun: new Date().toISOString(),
      passRate: 0.0,
      coverage: {
        lineCoverage: 0.0,
        branchCoverage: 0.0,
        methodCoverage: 0.0,
        uncoveredLines: ['All lines']
      }
    };
  }

  private createSampleRefactoringGoals(): RefactoringGoal[] {
    return [
      {
        type: 'extract-method',
        description: 'Extract validation logic into separate method',
        priority: 1
      },
      {
        type: 'rename',
        description: 'Rame variables for better clarity',
        priority: 2
      }
    ];
  }

  private createSampleSafetyMeasures(): SafetyMeasures {
    return {
      preserveBehavior: true,
      createCheckpoints: true,
      suggestRollbackPoints: true,
      maxStepsWithoutCommit: 5
    };
  }

  private createSampleProductRoadmap(): ProductRoadmap {
    return {
      id: 'roadmap-2024',
      name: '2024 Product Roadmap',
      features: [
        {
          id: 'feat-001',
          title: 'Social Authentication',
          description: 'Add Google and Facebook login options',
          priority: 'high',
          targetDate: '2024-06-30T00:00:00Z',
          status: 'planned',
          dependencies: []
        },
        {
          id: 'feat-002',
          title: 'Two-Factor Authentication',
          description: 'Add 2FA for enhanced security',
          priority: 'medium',
          targetDate: '2024-09-30T00:00:00Z',
          status: 'planned',
          dependencies: ['feat-001']
        }
      ],
      milestones: [
        {
          id: 'milestone-001',
          name: 'Q2 Release',
          date: '2024-06-30T00:00:00Z',
          deliverables: ['Social Authentication', 'Performance Improvements'],
          status: 'upcoming'
        }
      ],
      startDate: '2024-01-01T00:00:00Z',
      endDate: '2024-12-31T00:00:00Z'
    };
  }
}