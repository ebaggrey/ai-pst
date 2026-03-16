import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TestAIService, TestCaseResponse, AIError, FlakyPrediction, TestRun } from '../../services/test-ai.service';
import { Subject } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged, finalize } from 'rxjs/operators';
import { HttpClientModule, HttpErrorResponse } from '@angular/common/http';
import { CommonModule, DecimalPipe} from "@angular/common";
@Component({
  standalone:true,
  imports: [DecimalPipe,CommonModule,
      FormsModule,
      ReactiveFormsModule,
      HttpClientModule],
  selector: 'app-test-generator',
  templateUrl: './test-generator.component.html',
  styleUrls: ['./test-generator.component.css']
})
export class TestGeneratorComponent implements OnInit, OnDestroy {
  // Form groups
  testGenerationForm: FormGroup;
  flakyAnalysisForm: FormGroup;
  
  // State management
  isLoading = false;
  isAnalyzing = false;
  generatedCode: string | null = null;
  testResponse: TestCaseResponse | null = null;
  flakyPredictions: FlakyPrediction[] | null = null;
  error: AIError | null = null;
  
  // Available options
  llmProviders = ['chatgpt', 'claude', 'deepseek', 'gemini', 'llama'];
  urgencyLevels = [
    { value: 1, label: 'Thoughtful (More thorough, slower)' },
    { value: 2, label: 'Balanced (Recommended)' },
    { value: 3, label: 'Fast (Quick results)' }
  ];
  
  // Example test runs for demo
  mockTestRuns: TestRun[] = [
    {
      id: '1',
      testName: 'Login Test',
      passed: true,
      timestamp: new Date('2024-01-15T10:30:00'),
      duration: 2500
    },
    {
      id: '2',
      testName: 'Login Test',
      passed: false,
      timestamp: new Date('2024-01-15T10:35:00'),
      duration: 3100
    },
    {
      id: '3',
      testName: 'Login Test',
      passed: true,
      timestamp: new Date('2024-01-15T10:40:00'),
      duration: 2800
    },
    {
      id: '4',
      testName: 'Checkout Flow',
      passed: true,
      timestamp: new Date('2024-01-15T11:00:00'),
      duration: 4500
    },
    {
      id: '5',
      testName: 'Checkout Flow',
      passed: false,
      timestamp: new Date('2024-01-15T11:05:00'),
      duration: 5200
    }
  ];

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private testAIService: TestAIService
  ) {
    this.testGenerationForm = this.fb.group({
      prompt: ['', [Validators.required, Validators.minLength(10)]],
      llmProvider: ['chatgpt', Validators.required],
      context: ['Generate a complete, executable test case with assertions.'],
      urgency: [2, [Validators.required, Validators.min(1), Validators.max(3)]]
    });

    this.flakyAnalysisForm = this.fb.group({
      confidenceThreshold: [0.75, [Validators.required, Validators.min(0), Validators.max(1)]]
    });
  }

  ngOnInit(): void {
    // Example 1: HTTP POST - Generate Test Case
    this.setupRealtimeValidation();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ==================== EXAMPLE 1: POST /api/testgen ====================
  // Generate a test case with the selected provider and urgency
  generateTestCase(): void {
    if (this.testGenerationForm.invalid) {
      this.markFormGroupTouched(this.testGenerationForm);
      return;
    }

    this.isLoading = true;
    this.error = null;
    this.testResponse = null;
    this.generatedCode = null;

    const { prompt, llmProvider, context, urgency } = this.testGenerationForm.value;

    // HTTP Request Example:
    // POST {{apiBaseUrl}}/api/testgen
    // Content-Type: application/json
    // {
    //   "prompt": "Test login functionality",
    //   "llmProvider": "chatgpt",
    //   "context": "Generate a complete, executable test case with assertions.",
    //   "urgency": 2
    // }
    this.testAIService.generateTestCase(prompt, llmProvider, urgency)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isLoading = false)
      )
      .subscribe({
        next: (response: TestCaseResponse) => {
          this.testResponse = response;
          this.generatedCode = response.generatedCode;
          
          // Log the complete HTTP response for debugging
          console.log('HTTP Response:', {
            status: 200,
            body: response,
            headers: {
              'content-type': 'application/json'
            }
          });
        },
        error: (error: AIError) => {
          this.error = error;
          console.error('HTTP Error Response:', error);
        }
      });
  }

  // ==================== EXAMPLE 2: POST /api/testgen/analyze/flaky ====================
  // Analyze test runs for flaky patterns
  analyzeFlakyTests(): void {
    this.isAnalyzing = true;
    this.error = null;
    this.flakyPredictions = null;

    const threshold = this.flakyAnalysisForm.get('confidenceThreshold')?.value || 0.75;

    // HTTP Request Example:
    // POST {{apiBaseUrl}}/api/testgen/analyze/flaky
    // Content-Type: application/json
    // {
    //   "testRuns": [
    //     {
    //       "id": "1",
    //       "testName": "Login Test",
    //       "passed": true,
    //       "timestamp": "2024-01-15T10:30:00",
    //       "duration": 2500
    //     }
    //   ],
    //   "analysisType": "flaky-pattern",
    //   "confidenceThreshold": 0.75
    // }
    this.testAIService.checkForFlakyTests(this.mockTestRuns)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isAnalyzing = false)
      )
      .subscribe({
        next: (predictions: FlakyPrediction[]) => {
          this.flakyPredictions = predictions;
          
          // Log the complete HTTP response for debugging
          console.log('HTTP Response:', {
            status: 200,
            body: predictions,
            headers: {
              'content-type': 'application/json'
            }
          });
        },
        error: (error: AIError) => {
          this.error = error;
          console.error('HTTP Error Response:', error);
        }
      });
  }

  // ==================== EXAMPLE HTTP REQUESTS ====================
  // These methods demonstrate raw HTTP requests using HttpClient
  // They show the exact structure of requests to your backend endpoints

  /**
   * Example 1: Raw HTTP POST for test generation
   * Demonstrates the complete HTTP request structure
   */
  rawGenerateTestExample(): void {
    const httpExample = `
    POST /api/testgen HTTP/1.1
    Host: ${this.testAIService['apiUrl']}
    Content-Type: application/json
    Authorization: Bearer <your-token> (if required)
    
    {
      "prompt": "Create a login test that validates user authentication with valid and invalid credentials",
      "llmProvider": "chatgpt",
      "context": "Generate a complete, executable test case with assertions using Playwright",
      "urgency": 2
    }
    
    Expected Response (200 OK):
    {
      "generatedCode": "test('login with valid credentials', async ({ page }) => { ... })",
      "testFramework": "playwright",
      "explanation": [
        "Generated by AI, reviewed by you.",
        "Check selectors match your actual page."
      ],
      "estimatedComplexity": 3,
      "potentialFlakyPoints": [
        "Fixed delays can cause flakiness—consider explicit waits."
      ],
      "chosenProvider": "chatgpt"
    }
    `;
    
    console.log(httpExample);
    alert('Check console for HTTP request examples');
  }

  /**
   * Example 2: Raw HTTP POST for flaky test analysis
   */
  rawFlakyAnalysisExample(): void {
    const httpExample = `
    POST /api/testgen/analyze/flaky HTTP/1.1
    Host: ${this.testAIService['apiUrl']}
    Content-Type: application/json
    
    {
      "testRuns": [
        {
          "id": "test-123",
          "testName": "UserLoginTest",
          "passed": false,
          "timestamp": "2024-01-15T10:35:00Z",
          "duration": 3100
        }
      ],
      "analysisType": "flaky-pattern",
      "confidenceThreshold": 0.75
    }
    
    Expected Response (200 OK):
    [
      {
        "testName": "UserLoginTest",
        "flakyScore": 0.85,
        "confidence": 0.92,
        "patterns": ["Timing issues", "Network instability"]
      }
    ]
    
    Error Response (503 Service Unavailable):
    {
      "errorId": "550e8400-e29b-41d4-a716-446655440000",
      "message": "The chatgpt service is having a moment.",
      "suggestion": "Try again in a bit, or switch to a different provider.",
      "recoverable": true,
      "provider": "chatgpt"
    }
    `;
    
    console.log(httpExample);
    alert('Check console for HTTP request examples');
  }

  // ==================== UTILITY METHODS ====================

  private setupRealtimeValidation(): void {
    this.testGenerationForm.get('prompt')?.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe(value => {
        if (value && value.length < 10) {
          this.testGenerationForm.get('prompt')?.setErrors({ 
            minlength: { requiredLength: 10, actualLength: value.length } 
          });
        }
      });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if ((control as any).controls) {
        this.markFormGroupTouched(control as FormGroup);
      }
    });
  }

  // Validation helpers
  getPromptErrorMessage(): string {
    const control = this.testGenerationForm.get('prompt');
    if (control?.hasError('required')) {
      return 'Tell us what to test! A prompt is required.';
    }
    if (control?.hasError('minlength')) {
      return 'Give the AI a bit more to work with—at least 10 characters.';
    }
    return '';
  }

  getUrgencyErrorMessage(): string {
    const control = this.testGenerationForm.get('urgency');
    if (control?.hasError('required')) {
      return 'Urgency level is required.';
    }
    if (control?.hasError('min') || control?.hasError('max')) {
      return 'Urgency must be between 1 (thoughtful) and 3 (fast).';
    }
    return '';
  }

  // Reset forms
  resetForms(): void {
    this.testGenerationForm.reset({
      prompt: '',
      llmProvider: 'chatgpt',
      context: 'Generate a complete, executable test case with assertions.',
      urgency: 2
    });
    this.flakyAnalysisForm.reset({
      confidenceThreshold: 0.75
    });
    this.generatedCode = null;
    this.testResponse = null;
    this.flakyPredictions = null;
    this.error = null;
  }

  // Copy generated code to clipboard
  copyToClipboard(): void {
    if (this.generatedCode) {
      navigator.clipboard.writeText(this.generatedCode).then(() => {
        alert('Code copied to clipboard!');
      });
    }
  }
}