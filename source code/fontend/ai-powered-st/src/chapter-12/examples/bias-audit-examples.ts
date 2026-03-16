//Data Bias Audit Service - HTTP Request Examples
/*
1. POST /api/bias/audit-data - Perform Bias Audit on Test Data

// Example request body
{
  "datasetName": "customer-feedback-q2",
  "dataContext": "customer satisfaction survey",
  "dataSample": [
    {"age": 25, "gender": "male", "feedback": "great product"},
    {"age": 34, "gender": "female", "feedback": "needs improvement"},
    {"age": 45, "gender": "male", "feedback": "excellent service"}
  ],
  "suggestionCount": 5,
  "aiPrompt": "Audit the provided customer satisfaction survey dataset named 'customer-feedback-q2' for homogeneity and bias. Focus on missing perspectives. Suggest 5 inclusive alternatives."
}

// Example response
{
  "auditId": "audit-123e4567-e89b-12d3-a456-426614174000",
  "datasetName": "customer-feedback-q2",
  "biasFindings": [
    {
      "category": "Gender Bias",
      "issue": "Underrepresentation of non-binary genders",
      "severity": "medium"
    },
    {
      "category": "Age Bias",
      "issue": "Missing voices from 18-24 and 55+ age groups",
      "severity": "high"
    }
  ],
  "suggestions": [
    "Include non-binary gender option in demographic collection",
    "Add targeted outreach to younger users (18-24)",
    "Create senior-friendly feedback channels for users 55+",
    "Consider cultural background as additional demographic",
    "Add geographic diversity to capture regional differences"
  ],
  "completionTime": "2024-01-15T10:30:00Z"
}
*/

/*
2. GET /api/bias/health - Health Check Endpoint

// Example request
No request body required

// Example response
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "service": "BiasAuditController"
}
*/

/*
3. Error Responses

// 400 Bad Request - Missing Dataset Name
{
  "message": "We need a name for this dataset to track the audit.",
  "datasetName": null,
  "errorType": "MissingDatasetName",
  "suggestedRemediation": "Please provide a DatasetName in your request.",
  "errorId": "err-123e4567-e89b-12d3-a456-426614174000",
  "timestamp": "2024-01-15T10:30:00Z"
}

// 400 Bad Request - Empty Data Sample
{
  "message": "The data sample is empty. Send us a few rows to look at.",
  "datasetName": "empty-dataset",
  "errorType": "EmptyDataSample",
  "suggestedRemediation": "Please provide at least one row of data in the DataSample array.",
  "errorId": "err-223e4567-e89b-12d3-a456-426614174000",
  "timestamp": "2024-01-15T10:30:00Z"
}

// 400 Bad Request - Invalid Suggestion Count
{
  "message": "Please ask for between 1 and 50 suggestions.",
  "datasetName": "test-data",
  "errorType": "InvalidSuggestionCount",
  "suggestedRemediation": "Set SuggestionCount to a value between 1 and 50.",
  "errorId": "err-323e4567-e89b-12d3-a456-426614174000",
  "timestamp": "2024-01-15T10:30:00Z"
}

// 503 Service Unavailable - AI Service Error
{
  "message": "The AI insight service is having trouble right now.",
  "datasetName": "customer-feedback-q2",
  "errorType": "AIServiceUnavailable",
  "suggestedRemediation": "Please try again in a few moments. If it persists, contact the AI ops team.",
  "errorId": "err-423e4567-e89b-12d3-a456-426614174000",
  "timestamp": "2024-01-15T10:30:00Z"
}

// 400 Bad Request - Data Format Error
{
  "message": "We couldn't make sense of the data format.",
  "datasetName": "malformed-data",
  "errorType": "DataFormatError",
  "suggestedRemediation": "Check that your data sample is a valid JSON array of objects.",
  "errorId": "err-523e4567-e89b-12d3-a456-426614174000",
  "timestamp": "2024-01-15T10:30:00Z"
}

// 500 Internal Server Error
{
  "message": "An unexpected error occurred while processing your request.",
  "datasetName": "problem-dataset",
  "errorType": "InternalServerError",
  "suggestedRemediation": "Please try again later or contact support.",
  "errorId": "err-623e4567-e89b-12d3-a456-426614174000",
  "timestamp": "2024-01-15T10:30:00Z"
}
*/