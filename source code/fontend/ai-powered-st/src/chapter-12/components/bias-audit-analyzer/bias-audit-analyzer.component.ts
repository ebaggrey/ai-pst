// src/app/components/bias-audit-analyzer/bias-audit-analyzer.component.ts
import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { DataBiasAuditService } from '../../services/data-bias-audit.service';
import { TestDataBiasAuditRequest } from '../../models/requests/test-data-bias-audit-request.model';
import { BiasAuditResponse, BiasFinding, InclusiveSuggestion } from '../../models/responses/bias-audit-response.model';
import { BiasAuditErrorResponse } from '../../models/responses/bias-audit-error-response.model';
import { Subject } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';
import { HealthCheckDetailedResponse } from 'src/chapter-12/models/responses/health-check-detailed-response.model';

@Component({
  selector: 'app-bias-audit-analyzer',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [DataBiasAuditService],
  template: `
    <div class="bias-audit-analyzer">
      <!-- Header -->
      <header class="app-header">
        <h1>🤖 AI-Powered Bias Audit Analyzer</h1>
        <p class="subtitle">Analyze your datasets for bias and get inclusive suggestions</p>
      </header>

      <div class="container">
        <!-- Health Status -->
       <!-- Add to the health-status section in the component template -->
<div class="health-status" *ngIf="healthStatus">
  <div class="status-badge" [class.healthy]="healthStatus.status === 'Healthy'">
    {{ healthStatus.status }}
  </div>
  <span class="status-text">API Status: {{ healthStatus.status }}</span>
  <span class="timestamp">Last checked: {{ healthStatus.timestamp | date:'medium' }}</span>
  
  <!-- Detailed health toggle button -->
  <button class="btn-icon" (click)="checkDetailedHealth()" title="Check detailed health">
    🔍 Details
  </button>
</div>

<!-- Detailed health panel -->
<div class="health-details card" *ngIf="showHealthDetails && healthDetails">
  <div class="health-details-header">
    <h3>System Health Details</h3>
    <button class="close-btn" (click)="showHealthDetails = false">✕</button>
  </div>
  
  <div class="health-summary">
    <span class="overall-status" [style.color]="getHealthStatusColor(healthDetails.status)">
      Overall Status: {{ healthDetails.status }}
    </span>
    <span class="duration">Duration: {{ healthDetails.duration }}</span>
  </div>
  
  <div class="health-checks">
    <div class="health-check-item" *ngFor="let check of healthDetails.checks">
      <div class="check-header">
        <span class="check-name">{{ check.name }}</span>
        <span class="check-status" [style.background-color]="getHealthStatusColor(check.status)">
          {{ check.status }}
        </span>
      </div>
      
      <div class="check-details" *ngIf="check.description">
        <p class="description">{{ check.description }}</p>
        <div class="meta">
          <span class="duration">⏱️ {{ check.duration }}</span>
          <span class="tags" *ngIf="check.tags?.length">
            🏷️  {{ formatTags(check.tags) }}
          </span>
        </div>
      </div>
    </div>
  </div>
</div>

<!-- Add these styles -->
<style>
  .btn-icon {
    padding: 0.3rem 0.8rem;
    background: #667eea;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.9rem;
  }

  .btn-icon:hover {
    background: #764ba2;
  }

  .health-details {
    margin-top: 1rem;
    padding: 1.5rem;
    background: white;
    border-radius: 10px;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  }

  .health-details-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;
  }

  .health-details-header h3 {
    margin: 0;
    color: #333;
  }

  .close-btn {
    background: none;
    border: none;
    font-size: 1.2rem;
    cursor: pointer;
    color: #999;
  }

  .close-btn:hover {
    color: #666;
  }

  .health-summary {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
    background: #f8f9fa;
    border-radius: 8px;
    margin-bottom: 1rem;
  }

  .overall-status {
    font-weight: bold;
    font-size: 1.1rem;
  }

  .health-checks {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .health-check-item {
    padding: 1rem;
    background: #f8f9fa;
    border-radius: 8px;
    border-left: 4px solid;
    border-left-color: {{ check.status === 'Healthy' ? '#4caf50' : (check.status === 'Degraded' ? '#ff9800' : '#f44336') }};
  }

  .check-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 0.5rem;
  }

  .check-name {
    font-weight: bold;
    color: #333;
  }

  .check-status {
    padding: 0.2rem 0.8rem;
    border-radius: 20px;
    color: white;
    font-size: 0.85rem;
  }

  .check-details .description {
    color: #666;
    margin: 0.5rem 0;
  }

  .check-details .meta {
    display: flex;
    gap: 1rem;
    font-size: 0.85rem;
    color: #999;
  }

  .tags {
    background: #e0e0e0;
    padding: 0.2rem 0.5rem;
    border-radius: 4px;
    color: #666;
  }
</style>

        <!-- Main Content Grid -->
        <div class="content-grid">
          <!-- Left Column - Input Form -->
          <div class="input-section card">
            <h2>📊 Dataset Analysis</h2>
            
            <form [formGroup]="auditForm" (ngSubmit)="onSubmit()" class="audit-form">
              <!-- Dataset Name -->
              <div class="form-group">
                <label for="datasetName">
                  Dataset Name <span class="required">*</span>
                </label>
                <input 
                  type="text" 
                  id="datasetName" 
                  formControlName="datasetName"
                  placeholder="e.g., hiring-data-2024"
                  [class.invalid]="isFieldInvalid('datasetName')"
                >
                <div class="error-message" *ngIf="isFieldInvalid('datasetName')">
                  Dataset name is required
                </div>
              </div>

              <!-- Data Context -->
              <div class="form-group">
                <label for="dataContext">Data Context</label>
                <input 
                  type="text" 
                  id="dataContext" 
                  formControlName="dataContext"
                  placeholder="e.g., job applications, customer feedback"
                >
              </div>

              <!-- Suggestion Count -->
              <div class="form-group">
                <label for="suggestionCount">
                  Number of Suggestions <span class="required">*</span>
                </label>
                <input 
                  type="number" 
                  id="suggestionCount" 
                  formControlName="suggestionCount"
                  min="1" 
                  max="50"
                  [class.invalid]="isFieldInvalid('suggestionCount')"
                >
                <div class="error-message" *ngIf="isFieldInvalid('suggestionCount')">
                  Please enter between 1 and 50 suggestions
                </div>
              </div>

              <!-- Data Sample (JSON) -->
              <div class="form-group">
                <label for="dataSample">
                  Data Sample (JSON) <span class="required">*</span>
                </label>
                <textarea 
                  id="dataSample" 
                  formControlName="dataSample"
                      rows="8"
                  placeholder='[{"name": "John Smith", "age": 45, "gender": "Male"}]'
                  [class.invalid]="isFieldInvalid('dataSample')"
                ></textarea>
                <div class="error-message" *ngIf="isFieldInvalid('dataSample')">
                  Valid JSON data sample is required
                </div>
                <div class="help-text">
                  Enter JSON array of objects. Each object should represent a data row.
                </div>
              </div>

              <!-- AI Prompt (Optional) -->
              <div class="form-group">
                <label for="aiPrompt">Custom AI Prompt (Optional)</label>
                <textarea 
                  id="aiPrompt" 
                  formControlName="aiPrompt"
                  rows="3"
                  placeholder="Enter custom instructions for the AI..."
                ></textarea>
              </div>

              <!-- Action Buttons -->
              <div class="form-actions">
                <button 
                  type="submit" 
                  class="btn-primary"
                  [disabled]="auditForm.invalid || isAnalyzing"
                >
                  <span *ngIf="!isAnalyzing">🔍 Analyze for Bias</span>
                  <span *ngIf="isAnalyzing">⏳ Analyzing...</span>
                </button>
                
                <button 
                  type="button" 
                  class="btn-secondary"
                  (click)="loadExampleData()"
                  [disabled]="isAnalyzing"
                >
                  📋 Load Example
                </button>

                <button 
                  type="button" 
                  class="btn-secondary"
                  (click)="clearForm()"
                  [disabled]="isAnalyzing"
                >
                  🗑️ Clear
                </button>
              </div>
            </form>

            <!-- Error Display -->
            <div class="error-display" *ngIf="error">
              <h3>⚠️ Error</h3>
              <p>{{ error.message }}</p>
              <p class="error-detail">Error ID: {{ error.errorId }}</p>
              <p class="error-remediation">💡 {{ error.suggestedRemediation }}</p>
            </div>
          </div>

          <!-- Right Column - Results -->
          <div class="results-section card" *ngIf="auditResult">
            <h2>📈 Analysis Results</h2>
            
            <!-- Summary Cards -->
            <div class="summary-cards">
              <div class="summary-card">
                <div class="label">Audit ID</div>
                <div class="value">{{ auditResult.auditId }}</div>
              </div>
              
              <div class="summary-card">
                <div class="label">Date</div>
                <div class="value">{{ auditResult.auditDate | date:'medium' }}</div>
              </div>
              
              <div class="summary-card">
                <div class="label">Dataset</div>
                <div class="value">{{ auditResult.datasetName }}</div>
              </div>
            </div>

            <!-- Bias Score Gauge -->
            <div class="bias-score-section">
              <h3>Overall Bias Score</h3>
              <div class="score-gauge">
                <div class="gauge-container">
                  <div 
                    class="gauge-fill"
                    [style.width.%]="auditResult.overallBiasScore.overallScore * 100"
                    [style.background-color]="getScoreColor(auditResult.overallBiasScore.overallScore)"
                  ></div>
                </div>
                <div class="score-details">
                  <span class="score-value">
                    {{ auditResult.overallBiasScore.overallScore | percent }}
                  </span>
                  <span class="risk-level" [style.color]="getRiskLevelColor(auditResult.overallBiasScore.riskLevel)">
                    {{ auditResult.overallBiasScore.riskLevel }} Risk
                  </span>
                </div>
              </div>

              <!-- Detailed Scores -->
              <div class="detailed-scores">
                <div class="score-item">
                  <span class="label">Gender Bias:</span>
                  <div class="score-bar">
                    <div 
                      class="bar-fill"
                      [style.width.%]="auditResult.overallBiasScore.genderBiasScore * 100"
                      [style.background-color]="getScoreColor(auditResult.overallBiasScore.genderBiasScore)"
                    ></div>
                  </div>
                  <span class="value">{{ auditResult.overallBiasScore.genderBiasScore | percent }}</span>
                </div>

                <div class="score-item">
                  <span class="label">Racial Bias:</span>
                  <div class="score-bar">
                    <div 
                      class="bar-fill"
                      [style.width.%]="auditResult.overallBiasScore.racialBiasScore * 100"
                      [style.background-color]="getScoreColor(auditResult.overallBiasScore.racialBiasScore)"
                    ></div>
                  </div>
                  <span class="value">{{ auditResult.overallBiasScore.racialBiasScore | percent }}</span>
                </div>

                <div class="score-item">
                  <span class="label">Age Bias:</span>
                  <div class="score-bar">
                    <div 
                      class="bar-fill"
                      [style.width.%]="auditResult.overallBiasScore.ageBiasScore * 100"
                      [style.background-color]="getScoreColor(auditResult.overallBiasScore.ageBiasScore)"
                    ></div>
                  </div>
                  <span class="value">{{ auditResult.overallBiasScore.ageBiasScore | percent }}</span>
                </div>

                <div class="score-item">
                  <span class="label">Cultural Bias:</span>
                  <div class="score-bar">
                    <div 
                      class="bar-fill"
                      [style.width.%]="auditResult.overallBiasScore.culturalBiasScore * 100"
                      [style.background-color]="getScoreColor(auditResult.overallBiasScore.culturalBiasScore)"
                    ></div>
                  </div>
                  <span class="value">{{ auditResult.overallBiasScore.culturalBiasScore | percent }}</span>
                </div>
              </div>
            </div>

            <!-- Findings -->
            <div class="findings-section" *ngIf="auditResult.findings?.length">
              <h3>🔍 Bias Findings</h3>
              <div class="findings-list">
                <div class="finding-item" *ngFor="let finding of auditResult.findings">
                  <div class="finding-header">
                    <span class="field-name">{{ finding.fieldName }}</span>
                    <span class="bias-type">{{ finding.biasType }}</span>
                    <span 
                      class="severity"
                      [style.background-color]="getScoreColor(finding.severityScore)"
                    >
                      Severity: {{ finding.severityScore | percent }}
                    </span>
                  </div>
                  <p class="description">{{ finding.description }}</p>
                  <div class="examples" *ngIf="finding.examples?.length">
                    <strong>Examples:</strong>
                    <ul>
                      <li *ngFor="let example of finding.examples">{{ example }}</li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>

            <!-- Suggestions -->
            <div class="suggestions-section" *ngIf="auditResult.suggestions?.length">
              <h3>💡 Inclusive Suggestions</h3>
              <div class="suggestions-list">
                <div class="suggestion-item" *ngFor="let suggestion of auditResult.suggestions">
                  <div class="suggestion-header">
                    <span class="field-name">{{ suggestion.fieldName }}</span>
                    <span 
                      class="confidence"
                      [style.background-color]="getConfidenceColor(suggestion.confidenceScore)"
                    >
                      Confidence: {{ suggestion.confidenceScore }}%
                    </span>
                  </div>
                  <div class="suggestion-content">
                    <div class="original">
                      <strong>Original:</strong> {{ suggestion.originalValue }}
                    </div>
                    <div class="suggested">
                      <strong>Suggested:</strong> {{ suggestion.suggestedValue }}
                    </div>
                    <div class="rationale">
                      <strong>Why:</strong> {{ suggestion.rationale }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .bias-audit-analyzer {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    }

    .app-header {
      background: white;
      padding: 2rem;
      text-align: center;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
      margin-bottom: 2rem;
    }

    .app-header h1 {
      margin: 0;
      color: #333;
      font-size: 2.5rem;
    }

    .app-header .subtitle {
      color: #666;
      margin-top: 0.5rem;
      font-size: 1.1rem;
    }

    .container {
      max-width: 1400px;
      margin: 0 auto;
      padding: 0 2rem 2rem;
    }

    .health-status {
      background: white;
      border-radius: 10px;
      padding: 1rem 2rem;
      margin-bottom: 2rem;
      display: flex;
      align-items: center;
      gap: 1rem;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    }

    .status-badge {
      padding: 0.3rem 1rem;
      border-radius: 20px;
      font-weight: bold;
      background: #f0f0f0;
      color: #666;
    }

    .status-badge.healthy {
      background: #4caf50;
      color: white;
    }

    .status-text {
      flex: 1;
    }

    .timestamp {
      color: #999;
      font-size: 0.9rem;
    }

    .content-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 2rem;
    }

    .card {
      background: white;
      border-radius: 15px;
      padding: 2rem;
      box-shadow: 0 4px 20px rgba(0,0,0,0.15);
    }

    .card h2 {
      margin-top: 0;
      color: #333;
      border-bottom: 2px solid #f0f0f0;
      padding-bottom: 1rem;
      margin-bottom: 1.5rem;
    }

    .audit-form {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .form-group {
      display: flex;
      flex-direction: column;
    }

    .form-group label {
      font-weight: 600;
      color: #555;
      margin-bottom: 0.5rem;
    }

    .required {
      color: #f44336;
    }

    input, textarea {
      padding: 0.8rem;
      border: 2px solid #e0e0e0;
      border-radius: 8px;
      font-size: 1rem;
      transition: border-color 0.3s;
    }

    input:focus, textarea:focus {
      outline: none;
      border-color: #667eea;
    }

    input.invalid, textarea.invalid {
      border-color: #f44336;
    }

    .error-message {
      color: #f44336;
      font-size: 0.85rem;
      margin-top: 0.3rem;
    }

    .help-text {
      color: #999;
      font-size: 0.85rem;
      margin-top: 0.3rem;
    }

    .form-actions {
      display: flex;
      gap: 1rem;
      margin-top: 1rem;
    }

    button {
      padding: 0.8rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s;
      flex: 1;
    }

    button:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .btn-primary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
    }

    .btn-secondary {
      background: white;
      color: #667eea;
      border: 2px solid #667eea;
    }

    .btn-secondary:hover:not(:disabled) {
      background: #667eea;
      color: white;
    }

    .error-display {
      margin-top: 1.5rem;
      padding: 1rem;
      background: #fff3f3;
      border: 2px solid #f44336;
      border-radius: 8px;
    }

    .error-display h3 {
      color: #f44336;
      margin: 0 0 0.5rem 0;
    }

    .error-detail {
      font-size: 0.85rem;
      color: #999;
      margin: 0.5rem 0;
    }

    .error-remediation {
      background: #ffe6e6;
      padding: 0.8rem;
      border-radius: 4px;
      margin: 0.5rem 0 0;
    }

    .summary-cards {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1rem;
      margin-bottom: 2rem;
    }

    .summary-card {
      background: #f8f9fa;
      padding: 1rem;
      border-radius: 8px;
      text-align: center;
    }

    .summary-card .label {
      font-size: 0.85rem;
      color: #999;
      margin-bottom: 0.3rem;
    }

    .summary-card .value {
      font-weight: 600;
      color: #333;
      font-size: 1.1rem;
    }

    .bias-score-section {
      margin-bottom: 2rem;
    }

    .bias-score-section h3 {
      margin-bottom: 1rem;
    }

    .gauge-container {
      width: 100%;
      height: 30px;
      background: #f0f0f0;
      border-radius: 15px;
      overflow: hidden;
      margin-bottom: 0.5rem;
    }

    .gauge-fill {
      height: 100%;
      transition: width 0.5s;
    }

    .score-details {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }

    .score-value {
      font-size: 1.5rem;
      font-weight: bold;
    }

    .risk-level {
      font-weight: 600;
      padding: 0.3rem 1rem;
      border-radius: 20px;
      background: #f0f0f0;
    }

    .detailed-scores {
      background: #f8f9fa;
      padding: 1rem;
      border-radius: 8px;
    }

    .score-item {
      display: flex;
      align-items: center;
      gap: 1rem;
      margin-bottom: 0.8rem;
    }

    .score-item .label {
      width: 100px;
      font-size: 0.9rem;
    }

    .score-bar {
      flex: 1;
      height: 20px;
      background: #f0f0f0;
      border-radius: 10px;
      overflow: hidden;
    }

    .bar-fill {
      height: 100%;
      transition: width 0.5s;
    }

    .score-item .value {
      width: 60px;
      text-align: right;
      font-size: 0.9rem;
    }

    .findings-section, .suggestions-section {
      margin-top: 2rem;
    }

    .findings-section h3, .suggestions-section h3 {
      margin-bottom: 1rem;
    }

    .findings-list, .suggestions-list {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .finding-item, .suggestion-item {
      background: #f8f9fa;
      padding: 1rem;
      border-radius: 8px;
      border-left: 4px solid #667eea;
    }

    .finding-header, .suggestion-header {
      display: flex;
      align-items: center;
      gap: 1rem;
      margin-bottom: 0.5rem;
    }

    .field-name {
      font-weight: 600;
      color: #333;
    }

    .bias-type {
      background: #e0e0e0;
      padding: 0.2rem 0.5rem;
      border-radius: 4px;
      font-size: 0.85rem;
    }

    .severity, .confidence {
      padding: 0.2rem 0.5rem;
      border-radius: 4px;
      font-size: 0.85rem;
      color: white;
    }

    .description {
      color: #666;
      margin: 0.5rem 0;
    }

    .examples ul {
      margin: 0.3rem 0 0 1.5rem;
      color: #666;
    }

    .suggestion-content {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .original {
      color: #f44336;
    }

    .suggested {
      color: #4caf50;
    }

    .rationale {
      background: #e3f2fd;
      padding: 0.5rem;
      border-radius: 4px;
      font-style: italic;
    }

    @media (max-width: 1024px) {
      .content-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class BiasAuditAnalyzerComponent implements OnInit, OnDestroy {
  auditForm: FormGroup;
  auditResult: BiasAuditResponse | null = null;
  error: BiasAuditErrorResponse | null = null;
  healthStatus: { status: string; timestamp: Date; service: string } | null = null;
  isAnalyzing = false;
  
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private auditService: DataBiasAuditService
  ) {
    this.auditForm = this.createForm();
  }

  ngOnInit(): void {
    this.checkHealth();
    // Check health every 30 seconds
    setInterval(() => this.checkHealth(), 30000);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      datasetName: ['', [Validators.required, Validators.minLength(3)]],
      dataContext: ['job applications'],
      suggestionCount: [5, [Validators.required, Validators.min(1), Validators.max(50)]],
      dataSample: ['', [Validators.required, this.validateJson]],
      aiPrompt: ['']
    });
  }


// Safe tag display method (alternative approach)
formatTags(tags: string[] | undefined): string {
  if (!tags || tags.length === 0) return '';
  return tags.join(', ');
}

  private validateJson(control: any): { [key: string]: any } | null {
    if (!control.value) return null;
    
    try {
      const parsed = JSON.parse(control.value);
      if (!Array.isArray(parsed)) {
        return { invalidJson: 'Data must be a JSON array' };
      }
      if (parsed.length === 0) {
        return { invalidJson: 'Array cannot be empty' };
      }
      return null;
    } catch (e) {
      return { invalidJson: 'Invalid JSON format' };
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.auditForm.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }

  checkHealth(): void {
    this.auditService.checkHealth()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.healthStatus = response;
        },
        error: (error) => {
          console.error('Health check failed:', error);
          this.healthStatus = {
            status: 'Unhealthy',
            timestamp: new Date(),
            service: 'BiasAuditController'
          };
        }
      });
  }

  onSubmit(): void {
    if (this.auditForm.invalid) {
      Object.keys(this.auditForm.controls).forEach(key => {
        const control = this.auditForm.get(key);
        if (control?.invalid) {
          control.markAsTouched();
        }
      });
      return;
    }

    this.isAnalyzing = true;
    this.error = null;
    this.auditResult = null;

    const formValue = this.auditForm.value;
    const request: TestDataBiasAuditRequest = {
      datasetName: formValue.datasetName,
      dataContext: formValue.dataContext,
      suggestionCount: formValue.suggestionCount,
      dataSample: JSON.parse(formValue.dataSample),
      aiPrompt: formValue.aiPrompt || undefined
    };

    this.auditService.auditTestData(request)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isAnalyzing = false)
      )
      .subscribe({
        next: (response) => {
          this.auditResult = response;
        },
        error: (error) => {
          this.error = error;
        }
      });
  }

  loadExampleData(): void {
    const exampleData = [
      {
        name: 'John Smith',
        age: 45,
        gender: 'Male',
        ethnicity: 'Caucasian',
        experience: 20,
        position: 'Senior Manager',
        education: 'MBA'
      },
      {
        name: 'Maria Garcia',
        age: 32,
        gender: 'Female',
        ethnicity: 'Hispanic',
        experience: 8,
        position: 'Team Lead',
        education: 'Masters in CS'
      },
      {
        name: 'Wei Chen',
        age: 28,
        gender: 'Male',
        ethnicity: 'Asian',
        experience: 5,
        position: 'Software Engineer',
        education: 'Bachelors in CS'
      },
      {
        name: 'Sarah Johnson',
        age: 52,
        gender: 'Female',
        ethnicity: 'Caucasian',
        experience: 25,
        position: 'Director',
        education: 'PhD'
      },
      {
        name: 'James Williams',
        age: 35,
        gender: 'Male',
        ethnicity: 'African American',
        experience: 12,
        position: 'Senior Developer',
        education: 'Bachelors in CS'
      }
    ];

    this.auditForm.patchValue({
      datasetName: 'hiring-data-2024',
      dataContext: 'job applications',
      suggestionCount: 5,
      dataSample: JSON.stringify(exampleData, null, 2),
      aiPrompt: 'Analyze this hiring data for bias and suggest more inclusive alternatives.'
    });
  }

  clearForm(): void {
    this.auditForm.reset({
      datasetName: '',
      dataContext: 'job applications',
      suggestionCount: 5,
      dataSample: '',
      aiPrompt: ''
    });
    this.auditResult = null;
    this.error = null;
  }

  getScoreColor(score: number): string {
    if (score < 0.3) return '#4caf50'; // Green - Low bias
    if (score < 0.6) return '#ff9800'; // Orange - Medium bias
    return '#f44336'; // Red - High bias
  }

  getConfidenceColor(confidence: number): string {
    if (confidence >= 80) return '#4caf50';
    if (confidence >= 60) return '#ff9800';
    return '#f44336';
  }

  getRiskLevelColor(riskLevel: string): string {
    switch (riskLevel.toLowerCase()) {
      case 'low': return '#4caf50';
      case 'medium': return '#ff9800';
      case 'high': return '#f44336';
      case 'critical': return '#9c27b0';
      default: return '#666';
    }
  }


// Add to BiasAuditAnalyzerComponent
healthDetails: HealthCheckDetailedResponse | null = null;
showHealthDetails = false;

checkDetailedHealth(): void {
  this.auditService.checkReadiness()
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (response) => {
        this.healthDetails = response;
        this.showHealthDetails = true;
      },
      error: (error) => {
        console.error('Detailed health check failed:', error);
      }
    });
}

getHealthStatusColor(status: string): string {
  switch (status.toLowerCase()) {
    case 'healthy': return '#4caf50';
    case 'degraded': return '#ff9800';
    case 'unhealthy': return '#f44336';
    default: return '#999';
  }
}


}