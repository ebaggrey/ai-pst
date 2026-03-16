
/*
 ========================================================
 ONBOARDING ASSISTANT API - EXAMPLE REQUESTS & RESPONSES
 ========================================================
*/

/*

1. POST /api/onboarding/profiles - Create Onboarding Profile

// Example request body
{
  "userId": "user_123456",
  "userName": "John Doe",
  "userEmail": "john.doe@company.com",
  "role": "developer",
  "experienceLevel": "intermediate",
  "department": "Engineering",
  "manager": "Jane Smith",
  "startDate": "2024-03-15T09:00:00.000Z",
  "preferences": {
    "learningStyle": "mixed",
    "communicationFrequency": "weekly",
    "preferredChannels": ["email", "slack"],
    "timezone": "America/New_York",
    "workHours": {
      "start": "09:00",
      "end": "17:00",
      "timezone": "America/New_York"
    },
    "language": "en",
    "notificationsEnabled": true
  }
}

// Example response
{
  "success": true,
  "profileId": "prof_123456",
  "message": "Onboarding profile created successfully",
  "plan": {
    "id": "plan_123456",
    "profileId": "prof_123456",
    "name": "Developer Onboarding Plan",
    "description": "4-week onboarding for new developers",
    "modules": [
      {
        "id": "mod_001",
        "title": "Company Culture",
        "description": "Introduction to company values and culture",
        "type": "company-culture",
        "difficulty": "beginner",
        "prerequisites": [],
        "estimatedMinutes": 45,
        "status": "available"
      },
      {
        "id": "mod_002",
        "title": "Development Setup",
        "description": "Set up development environment",
        "type": "technical-training",
        "difficulty": "beginner",
        "prerequisites": [],
        "estimatedMinutes": 120,
        "status": "available"
      }
    ],
    "estimatedTotalMinutes": 165,
    "startDate": "2024-03-15T09:00:00.000Z",
    "targetCompletionDate": "2024-04-12T17:00:00.000Z",
    "milestones": [
      {
        "id": "ms_001",
        "title": "Week 1: Foundation",
        "description": "Complete company culture and setup",
        "dueDate": "2024-03-22T17:00:00.000Z",
        "completed": false,
        "moduleIds": ["mod_001", "mod_002"]
      }
    ],
    "status": "active",
    "progress": {
      "overall": 0,
      "modulesCompleted": 0,
      "totalModules": 2,
      "assessmentsPassed": 0,
      "totalAssessments": 0,
      "timeSpentMinutes": 0,
      "estimatedCompletionDate": "2024-04-12T17:00:00.000Z",
      "lastActivityDate": "2024-03-15T09:00:00.000Z"
    }
  },
  "nextSteps": [
    "Complete your profile",
    "Review onboarding materials",
    "Schedule welcome meeting with mentor"
  ]
}

*/

/*

2. GET /api/onboarding/profiles/{profileId} - Get Onboarding Profile

// Example URL
GET /api/onboarding/profiles/prof_123456

// Example response
{
  "id": "prof_123456",
  "userId": "user_123456",
  "userName": "John Doe",
  "userEmail": "john.doe@company.com",
  "role": "developer",
  "experienceLevel": "intermediate",
  "startDate": "2024-03-15T09:00:00.000Z",
  "department": "Engineering",
  "manager": "Jane Smith",
  "mentor": "Mike Johnson",
  "status": "active",
  "progress": {
    "overall": 25,
    "modulesCompleted": 1,
    "totalModules": 8,
    "assessmentsPassed": 2,
    "totalAssessments": 4,
    "timeSpentMinutes": 180,
    "estimatedCompletionDate": "2024-04-12T17:00:00.000Z",
    "lastActivityDate": "2024-03-18T14:30:00.000Z"
  },
  "preferences": {
    "learningStyle": "mixed",
    "communicationFrequency": "weekly",
    "preferredChannels": ["email", "slack"],
    "timezone": "America/New_York",
    "workHours": {
      "start": "09:00",
      "end": "17:00",
      "timezone": "America/New_York"
    },
    "language": "en",
    "notificationsEnabled": true
  },
  "createdAt": "2024-03-15T09:00:00.000Z",
  "updatedAt": "2024-03-18T14:30:00.000Z"
}

*/

/*

3. GET /api/onboarding/plans/{profileId} - Get Onboarding Plan

// Example URL
GET /api/onboarding/plans/prof_123456

// Example response
{
  "id": "plan_123456",
  "profileId": "prof_123456",
  "name": "Developer Onboarding Plan",
  "description": "4-week onboarding for new developers",
  "modules": [
    {
      "id": "mod_001",
      "title": "Company Culture",
      "description": "Introduction to company values and culture",
      "type": "company-culture",
      "difficulty": "beginner",
      "prerequisites": [],
      "estimatedMinutes": 45,
      "content": {
        "introduction": "Welcome to the company!",
        "sections": [
          {
            "title": "Our Values",
            "content": "We value innovation, collaboration...",
            "durationMinutes": 15
          }
        ],
        "summary": "Understanding our culture",
        "keyTakeaways": ["Value 1", "Value 2", "Value 3"]
      },
      "resources": [
        {
          "id": "res_001",
          "title": "Culture Deck",
          "type": "documentation",
          "url": "/docs/culture.pdf",
          "description": "Company culture presentation",
          "tags": ["culture", "onboarding"],
          "estimatedMinutes": 20,
          "required": true
        }
      ],
      "status": "completed",
      "completedAt": "2024-03-16T10:30:00.000Z",
      "score": 95
    },
    {
      "id": "mod_002",
      "title": "Development Setup",
      "description": "Set up development environment",
      "type": "technical-training",
      "difficulty": "beginner",
      "prerequisites": [],
      "estimatedMinutes": 120,
      "content": {
        "introduction": "Setting up your dev environment",
        "sections": [
          {
            "title": "Installation",
            "content": "Install required tools...",
            "durationMinutes": 60,
            "codeBlocks": [
              {
                "language": "bash",
                "code": "npm install -g @company/cli",
                "explanation": "Install CLI tool"
              }
            ]
          }
        ],
        "summary": "Environment ready",
        "keyTakeaways": ["Tool 1", "Tool 2", "Tool 3"]
      },
      "resources": [
        {
          "id": "res_002",
          "title": "Setup Guide",
          "type": "documentation",
          "url": "/docs/setup.md",
          "description": "Step-by-step setup guide",
          "tags": ["setup", "development"],
          "estimatedMinutes": 30,
          "required": true
        }
      ],
      "assessment": {
        "questions": [
          {
            "id": "q1",
            "type": "multiple-choice",
            "text": "Which command installs dependencies?",
            "options": ["npm install", "npm start", "npm test", "npm build"],
            "correctAnswer": "0",
            "points": 10
          }
        ],
        "passingScore": 70,
        "maxAttempts": 3,
        "allowReview": true
      },
      "status": "in-progress"
    }
  ],
  "estimatedTotalMinutes": 480,
  "startDate": "2024-03-15T09:00:00.000Z",
  "targetCompletionDate": "2024-04-12T17:00:00.000Z",
  "milestones": [
    {
      "id": "ms_001",
      "title": "Week 1: Foundation",
      "description": "Complete company culture and setup",
      "dueDate": "2024-03-22T17:00:00.000Z",
      "completed": true,
      "completedAt": "2024-03-20T15:00:00.000Z",
      "moduleIds": ["mod_001", "mod_002"]
    },
    {
      "id": "ms_002",
      "title": "Week 2: Core Skills",
      "description": "Complete technical training modules",
      "dueDate": "2024-03-29T17:00:00.000Z",
      "completed": false,
      "moduleIds": ["mod_003", "mod_004"]
    }
  ],
  "mentorAssigned": {
    "id": "mentor_001",
    "name": "Mike Johnson",
    "email": "mike.j@company.com",
    "role": "Senior Developer",
    "availability": ["Monday", "Wednesday", "Friday"],
    "expertise": ["JavaScript", "React", "Node.js"]
  },
  "status": "active",
  "progress": {
    "overall": 25,
    "modulesCompleted": 1,
    "totalModules": 8,
    "assessmentsPassed": 2,
    "totalAssessments": 4,
    "timeSpentMinutes": 180,
    "estimatedCompletionDate": "2024-04-12T17:00:00.000Z",
    "lastActivityDate": "2024-03-18T14:30:00.000Z"
  }
}

*/

/*

4. GET /api/onboarding/tasks/{profileId} - Get Onboarding Tasks

// Example URL
GET /api/onboarding/tasks/prof_123456?status=pending

// Example response
[
  {
    "id": "task_001",
    "title": "Complete Company Culture Module",
    "description": "Read through the company culture presentation",
    "type": "training",
    "priority": "high",
    "dueDate": "2024-03-22T17:00:00.000Z",
    "assignedTo": "prof_123456",
    "moduleId": "mod_001",
    "status": "completed",
    "dependencies": [],
    "createdAt": "2024-03-15T09:00:00.000Z",
    "completedAt": "2024-03-16T10:30:00.000Z"
  },
  {
    "id": "task_002",
    "title": "Set Up Development Environment",
    "description": "Install all required tools and verify setup",
    "type": "setup",
    "priority": "high",
    "dueDate": "2024-03-22T17:00:00.000Z",
    "assignedTo": "prof_123456",
    "moduleId": "mod_002",
    "status": "in-progress",
    "dependencies": ["task_001"],
    "notes": "Need help with Docker installation",
    "createdAt": "2024-03-15T09:00:00.000Z"
  },
  {
    "id": "task_003",
    "title": "Schedule 1:1 with Mentor",
    "description": "Set up recurring weekly meeting with mentor",
    "type": "meeting",
    "priority": "medium",
    "dueDate": "2024-03-29T17:00:00.000Z",
    "assignedTo": "prof_123456",
    "status": "pending",
    "dependencies": [],
    "createdAt": "2024-03-15T09:00:00.000Z"
  }
]

*/

/*

5. POST /api/onboarding/progress - Update Onboarding Progress

// Example request body
{
  "profileId": "prof_123456",
  "moduleId": "mod_002",
  "taskId": "task_002",
  "status": "completed",
  "score": 85,
  "timeSpentMinutes": 45,
  "notes": "Completed setup with mentor assistance"
}

// Example response
{
  "overall": 37,
  "modulesCompleted": 2,
  "totalModules": 8,
  "assessmentsPassed": 3,
  "totalAssessments": 4,
  "timeSpentMinutes": 225,
  "estimatedCompletionDate": "2024-04-10T17:00:00.000Z",
  "lastActivityDate": "2024-03-18T15:15:00.000Z"
}

*/

/*

6. PATCH /api/onboarding/tasks/{taskId} - Update Task Status

// Example URL
PATCH /api/onboarding/tasks/task_002

// Example request body
{
  "status": "completed",
  "notes": "Environment setup complete",
  "completedAt": "2024-03-18T15:15:00.000Z"
}

// Example response
{
  "id": "task_002",
  "title": "Set Up Development Environment",
  "description": "Install all required tools and verify setup",
  "type": "setup",
  "priority": "high",
  "dueDate": "2024-03-22T17:00:00.000Z",
  "assignedTo": "prof_123456",
  "moduleId": "mod_002",
  "status": "completed",
  "dependencies": ["task_001"],
  "notes": "Environment setup complete",
  "createdAt": "2024-03-15T09:00:00.000Z",
  "completedAt": "2024-03-18T15:15:00.000Z"
}

*/

/*

7. GET /api/onboarding/modules/{moduleId} - Get Module Details

// Example URL
GET /api/onboarding/modules/mod_002

// Example response
{
  "id": "mod_002",
  "title": "Development Setup",
  "description": "Set up development environment",
  "type": "technical-training",
  "difficulty": "beginner",
  "prerequisites": ["mod_001"],
  "estimatedMinutes": 120,
  "content": {
    "introduction": "Setting up your dev environment is crucial for productivity.",
    "sections": [
      {
        "title": "Required Tools",
        "content": "Install the following tools:",
        "durationMinutes": 30,
        "codeBlocks": [
          {
            "language": "bash",
            "code": "brew install git node docker",
            "explanation": "Install on macOS"
          },
          {
            "language": "bash",
            "code": "choco install git nodejs docker-desktop",
            "explanation": "Install on Windows"
          }
        ]
      },
      {
        "title": "Configuration",
        "content": "Configure your development environment:",
        "durationMinutes": 45,
        "codeBlocks": [
          {
            "language": "bash",
            "code": "git config --global user.name 'John Doe'\ngit config --global user.email 'john.doe@company.com'",
            "explanation": "Configure Git"
          }
        ]
      },
      {
        "title": "Verification",
        "content": "Verify everything is working:",
        "durationMinutes": 45,
        "codeBlocks": [
          {
            "language": "bash",
            "code": "node --version\nnpm --version\ngit --version\ndocker --version",
            "explanation": "Check versions"
          }
        ]
      }
    ],
    "summary": "Your development environment is now ready!",
    "keyTakeaways": [
      "All required tools are installed",
      "Git is configured with your identity",
      "You can run the development server"
    ],
    "codeExamples": [
      {
        "title": "Hello World",
        "description": "Simple test to verify setup",
        "language": "javascript",
        "code": "console.log('Hello, World!');",
        "expectedOutput": "Hello, World!"
      }
    ]
  },
  "assessment": {
    "questions": [
      {
        "id": "q1",
        "type": "multiple-choice",
        "text": "Which command verifies Node.js is installed?",
        "options": ["node --version", "npm install", "node start", "node verify"],
        "correctAnswer": "0",
        "points": 10,
        "explanation": "node --version displays the installed Node.js version"
      },
      {
        "id": "q2",
        "type": "code-completion",
        "text": "Complete the Git configuration command:",
        "correctAnswer": "git config --global user.name",
        "points": 20,
        "codeBlock": {
          "language": "bash",
          "code": "___ --global user.name 'John Doe'",
          "explanation": "The git config command sets configuration values"
        }
      }
    ],
    "passingScore": 70,
    "maxAttempts": 3,
    "timeLimitMinutes": 30,
    "allowReview": true
  },
  "resources": [
    {
      "id": "res_002",
      "title": "Setup Guide",
      "type": "documentation",
      "url": "/docs/setup.md",
      "description": "Step-by-step setup guide",
      "tags": ["setup", "development"],
      "estimatedMinutes": 30,
      "required": true
    },
    {
      "id": "res_003",
      "title": "Troubleshooting Video",
      "type": "video",
      "url": "https://company.com/videos/troubleshooting",
      "description": "Common issues and solutions",
      "tags": ["troubleshooting", "video"],
      "estimatedMinutes": 15,
      "required": false
    }
  ],
  "status": "available"
}

*/

/*

8. POST /api/onboarding/mentor/assign - Assign Mentor

// Example request body
{
  "profileId": "prof_123456",
  "mentorId": "mentor_001"
}

// Example response
{
  "id": "mentor_001",
  "name": "Mike Johnson",
  "email": "mike.j@company.com",
  "role": "Senior Developer",
  "availability": ["Monday 10am-12pm", "Wednesday 2pm-4pm", "Friday 9am-11am"],
  "expertise": ["JavaScript", "React", "Node.js", "Testing"],
  "message": "Mentor assigned successfully. They will contact you within 24 hours."
}

*/

/*

9. POST /api/onboarding/feedback - Submit Feedback

// Example request body
{
  "id": "fb_123456",
  "profileId": "prof_123456",
  "moduleId": "mod_002",
  "taskId": "task_002",
  "rating": 5,
  "comments": "Great module! The setup guide was very clear.",
  "suggestions": ["Add more code examples", "Include video tutorial"],
  "createdAt": "2024-03-18T16:30:00.000Z"
}

// Example response
{
  "success": true,
  "message": "Feedback submitted successfully. Thank you for helping us improve!"
}

*/

/*

10. GET /api/onboarding/statistics - Get Onboarding Statistics

// Example URL
GET /api/onboarding/statistics?department=Engineering

// Example response
{
  "totalOnboardings": 45,
  "activeOnboardings": 12,
  "completedOnboardings": 33,
  "averageCompletionDays": 18.5,
  "averageSatisfactionScore": 4.7,
  "moduleCompletionRates": {
    "mod_001": 98,
    "mod_002": 95,
    "mod_003": 87,
    "mod_004": 82,
    "mod_005": 76
  },
  "commonChallenges": [
    "Environment setup (23% of users)",
    "Understanding codebase (18% of users)",
    "Tool configuration (15% of users)"
  ],
  "mentorEffectiveness": 94,
  "departmentStats": {
    "Engineering": {
      "total": 28,
      "avgCompletion": 16.2,
      "satisfaction": 4.8
    },
    "Product": {
      "total": 10,
      "avgCompletion": 19.5,
      "satisfaction": 4.5
    },
    "QA": {
      "total": 7,
      "avgCompletion": 21.0,
      "satisfaction": 4.6
    }
  },
  "timeRange": {
    "start": "2024-01-01T00:00:00.000Z",
    "end": "2024-03-31T23:59:59.000Z"
  }
}

*/

/*

11. POST /api/onboarding/complete/{profileId} - Complete Onboarding

// Example URL
POST /api/onboarding/complete/prof_123456

// Example request body
{
  "feedback": "Great onboarding experience! Ready to start contributing."
}

// Example response
{
  "success": true,
  "certificate": "https://company.com/certificates/onboarding/prof_123456.pdf",
  "message": "Onboarding completed successfully! Certificate generated.",
  "nextSteps": [
    "Join the team channels",
    "Attend team meeting",
    "Start first sprint"
  ]
}

*/

/*

========================================================
ERROR RESPONSES
========================================================

// 400 Bad Request - Validation Error
{
  "code": "VALIDATION_ERROR",
  "message": "Invalid request data",
  "details": "userEmail must be a valid email address",
  "suggestion": "Please check your input and try again",
  "timestamp": "2024-03-18T16:30:00.000Z"
}

// 404 Not Found - Resource Not Found
{
  "code": "PROFILE_NOT_FOUND",
  "message": "Profile with ID prof_999999 not found",
  "suggestion": "Verify the profile ID and try again",
  "timestamp": "2024-03-18T16:30:00.000Z"
}

// 409 Conflict - Duplicate Resource
{
  "code": "PROFILE_EXISTS",
  "message": "Profile already exists for user user_123456",
  "suggestion": "Use existing profile or update current one",
  "timestamp": "2024-03-18T16:30:00.000Z"
}

// 500 Internal Server Error
{
  "code": "INTERNAL_ERROR",
  "message": "An unexpected error occurred",
  "details": "Database connection failed",
  "suggestion": "Please try again later",
  "timestamp": "2024-03-18T16:30:00.000Z"
}

// 503 Service Unavailable
{
  "code": "SERVICE_UNAVAILABLE",
  "message": "Mentor assignment service is temporarily unavailable",
  "suggestion": "Please try again in a few minutes",
  "timestamp": "2024-03-18T16:30:00.000Z"
}

*/


