// src/app/models/pipeline-cookbook/errors.ts

export interface PipelineErrorResponse {
    errorType: string;
    message: string;
    recoverySteps: string[];
    fallbackSuggestion: string;
}