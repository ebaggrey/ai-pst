// models/errors/bdd-error-response.model.ts
export interface BDDErrorResponse {
  errorType: string;
  phase: string;
  message: string;
  recoveryPath: string[];
  fallbackSuggestion: string;
  conflictDetails?: BDDConflictDetails;
}

export interface BDDConflictDetails {
  conflictingScenarios: string[];
  ambiguityAreas: string[];
  stakeholderConflicts: string[];
}