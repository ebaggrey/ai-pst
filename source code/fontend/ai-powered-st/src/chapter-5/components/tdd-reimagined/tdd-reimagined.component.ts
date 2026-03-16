import { Component, signal, inject } from '@angular/core';
import { CommonModule, JsonPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { TDDReimaginedService } from '../../services/tdd-reimagined.service';
import { TDDDataService } from '../../services/tdd-data.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-tdd-simple',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: '../tdd-reimagined/tdd-reimagined.component.html',
  styleUrls: ['../tdd-reimagined/tdd-reimagined.component.css']
})
export class TDDSimpleComponent {
  private tddService = inject(TDDReimaginedService);
  private dataService = inject(TDDDataService);

  // Base URL from environment
  public baseUrl = environment.tddReimaginedAPIUrl;
  public apiBaseUrl = environment.apiBaseUrl;

  // Signals
  public loading = signal(false);
  public selectedEndpoint = signal('generate');
  public response = signal<any>(null);
  public error = signal<any>(null);
  public currentDate = signal(new Date());

  // Request objects
  public tddRequest = this.dataService.createSampleTDDRequest();
  public implRequest = this.dataService.createSampleImplementationRequest();
  public refactorRequest = this.dataService.createSampleRefactorRequest();
  public futureRequest = this.dataService.createSampleFuturePredictionRequest();

  public endpoints = [
    { value: 'generate', label: '/generate-test-first', method: 'POST', endpoint: '/generate-test-first' },
    { value: 'implement', label: '/implement-from-failing-test', method: 'POST', endpoint: '/implement-from-failing-test' },
    { value: 'refactor', label: '/refactor-with-confidence', method: 'POST', endpoint: '/refactor-with-confidence' },
    { value: 'predict', label: '/predict-future-tests', method: 'POST', endpoint: '/predict-future-tests' },
    { value: 'health', label: '/health', method: 'GET', endpoint: '/health' }
  ];

  // TrackBy functions for performance
  trackByValue(index: number, item: any): string {
    return item.value;
  }

  trackByStrategy(index: number, strategy: string): string {
    return strategy;
  }

  setEndpoint(value: string) {
    this.selectedEndpoint.set(value);
    this.response.set(null);
    this.error.set(null);
  }

  sendRequest() {
    this.loading.set(true);
    this.response.set(null);
    this.error.set(null);
    this.currentDate.set(new Date());

    switch(this.selectedEndpoint()) {
      case 'generate':
        this.tddService.generateTestFirst(this.tddRequest).subscribe({
          next: (res) => { this.response.set(res); this.loading.set(false); },
          error: (err) => { this.error.set(err); this.loading.set(false); }
        });
        break;
      case 'implement':
        this.tddService.implementFromFailingTest(this.implRequest).subscribe({
          next: (res) => { this.response.set(res); this.loading.set(false); },
          error: (err) => { this.error.set(err); this.loading.set(false); }
        });
        break;
      case 'refactor':
        this.tddService.refactorWithConfidence(this.refactorRequest).subscribe({
          next: (res) => { this.response.set(res); this.loading.set(false); },
          error: (err) => { this.error.set(err); this.loading.set(false); }
        });
        break;
      case 'predict':
        this.tddService.predictFutureTests(this.futureRequest).subscribe({
          next: (res) => { this.response.set(res); this.loading.set(false); },
          error: (err) => { this.error.set(err); this.loading.set(false); }
        });
        break;
      case 'health':
        this.tddService.checkHealth().subscribe({
          next: (res) => { this.response.set(res); this.loading.set(false); },
          error: (err) => { this.error.set(err); this.loading.set(false); }
        });
        break;
    }
  }

  getEndpointUrl(endpointValue: string): string {
    const endpoint = this.endpoints.find(e => e.value === endpointValue);
    if (!endpoint) return '#';
    
    if (endpointValue === 'health') {
      return `${this.baseUrl}${endpoint.endpoint}`;
    }
    return `${this.apiBaseUrl}${endpoint.endpoint}`;
  }

  getEndpointMethod(endpointValue: string): string {
    const endpoint = this.endpoints.find(e => e.value === endpointValue);
    return endpoint?.method || 'POST';
  }

  getCurrentDate(): Date {
    return this.currentDate();
  }

  formatJson(data: any): string {
    return JSON.stringify(data, null, 2);
  }

  copyJson() {
    if (this.response()) {
      navigator.clipboard.writeText(this.formatJson(this.response()));
      alert('Response copied to clipboard!');
    }
  }

  clearResponse() {
    this.response.set(null);
    this.error.set(null);
  }
}