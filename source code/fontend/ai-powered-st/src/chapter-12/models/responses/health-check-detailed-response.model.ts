// src/app/models/responses/health-check-detailed-response.model.ts
export interface HealthCheckDetailedResponse {
  status: string;
  checks: HealthCheckItem[];
  duration: string;
}

export interface HealthCheckItem {
  name: string;
  status: string;
  description?: string;
  duration: string;
  tags?: string[];
}

// src/app/models/responses/health-check-response.model.ts (if separate file)
export interface HealthCheckResponse {
  status: string;
  timestamp: Date;
  service: string;
}

// Example usage:
/*
const healthCheckDetail: HealthCheckDetailedResponse = {
  status: "Healthy",
  duration: "00:00:00.125",
  checks: [
    {
      name: "database",
      status: "Healthy",
      description: "Database connection successful",
      duration: "00:00:00.050",
      tags: ["database", "sqlserver"]
    },
    {
      name: "ai-service",
      status: "Degraded",
      description: "AI service response time high",
      duration: "00:00:00.500",
      tags: ["ai", "external"]
    }
  ]
};
*/