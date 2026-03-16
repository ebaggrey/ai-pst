


// services/bdd-supercharged.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  BDCCoCreationRequest,
  AutomationRequest,
  EvolutionRequest,
  DriftDetectionRequest,
  DocumentationRequest
} from '../models/api-models';

@Injectable({
  providedIn: 'root'
})
export class BDDSuperchargedService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/bdd-supercharged`;

  constructor(private http: HttpClient) { }

  coCreateScenarios(request: BDCCoCreationRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/co-create-scenarios`, request);
  }

  translateScenarioToAutomation(request: AutomationRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/translate-to-automation`, request);
  }

  evolveScenarios(request: EvolutionRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/evolve-scenarios`, request);
  }

  detectScenarioDrift(request: DriftDetectionRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/detect-drift`, request);
  }

  generateLivingDocumentation(request: DocumentationRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/generate-documentation`, request);
  }
}