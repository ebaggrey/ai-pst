// ==================== Onboarding Models ====================

export interface OnboardingProfile {
  id: string;
  userId: string;
  userName: string;
  userEmail: string;
  role: OnboardingRole;
  experienceLevel: ExperienceLevel;
  startDate: string;
  department: string;
  manager: string;
  mentor?: string;
  status: OnboardingStatus;
  progress: OnboardingProgress;
  preferences: OnboardingPreferences;
  createdAt: string;
  updatedAt: string;
}

export type OnboardingRole = 
  | 'developer' 
  | 'qa-engineer' 
  | 'devops-engineer' 
  | 'product-manager' 
  | 'technical-lead' 
  | 'architect';

export type ExperienceLevel = 'beginner' | 'intermediate' | 'expert';

export type OnboardingStatus = 'pending' | 'active' | 'paused' | 'completed' | 'cancelled';

export interface OnboardingProgress {
  overall: number; // 0-100
  modulesCompleted: number;
  totalModules: number;
  assessmentsPassed: number;
  totalAssessments: number;
  timeSpentMinutes: number;
  estimatedCompletionDate: string;
  lastActivityDate: string;
}

export interface OnboardingPreferences {
  learningStyle: LearningStyle;
  communicationFrequency: CommunicationFrequency;
  preferredChannels: CommunicationChannel[];
  timezone: string;
  workHours: WorkHours;
  language: string;
  notificationsEnabled: boolean;
}

export type LearningStyle = 'visual' | 'auditory' | 'reading' | 'kinesthetic' | 'mixed';
export type CommunicationFrequency = 'daily' | 'weekly' | 'biweekly' | 'monthly' | 'as-needed';
export type CommunicationChannel = 'email' | 'slack' | 'teams' | 'zoom' | 'in-person';

export interface WorkHours {
  start: string; // "09:00"
  end: string; // "17:00"
  timezone: string;
}

export interface OnboardingModule {
  id: string;
  title: string;
  description: string;
  type: ModuleType;
  difficulty: ExperienceLevel;
  prerequisites: string[]; // Module IDs
  estimatedMinutes: number;
  content: ModuleContent;
  assessment?: ModuleAssessment;
  resources: LearningResource[];
  status: ModuleStatus;
  completedAt?: string;
  score?: number;
}

export type ModuleType = 
  | 'company-culture' 
  | 'technical-training' 
  | 'process-training' 
  | 'tool-training' 
  | 'pattern-training' 
  | 'best-practices'
  | 'compliance'
  | 'security';

export type ModuleStatus = 'locked' | 'available' | 'in-progress' | 'completed' | 'failed';

export interface ModuleContent {
  introduction: string;
  sections: ContentSection[];
  summary: string;
  keyTakeaways: string[];
  videoUrl?: string;
  slidesUrl?: string;
  codeExamples?: CodeExample[];
}

export interface ContentSection {
  title: string;
  content: string;
  durationMinutes: number;
  images?: string[];
  codeBlocks?: CodeBlock[];
}

export interface CodeBlock {
  language: string;
  code: string;
  explanation: string;
}

export interface CodeExample {
  title: string;
  description: string;
  language: string;
  code: string;
  expectedOutput?: string;
}

export interface ModuleAssessment {
  questions: AssessmentQuestion[];
  passingScore: number;
  maxAttempts: number;
  timeLimitMinutes?: number;
  allowReview: boolean;
}

export interface AssessmentQuestion {
  id: string;
  type: QuestionType;
  text: string;
  options?: string[];
  correctAnswer: string | string[];
  points: number;
  explanation?: string;
  codeBlock?: CodeBlock;
}

export type QuestionType = 'multiple-choice' | 'single-choice' | 'true-false' | 'code-completion' | 'essay';

export interface LearningResource {
  id: string;
  title: string;
  type: ResourceType;
  url: string;
  description: string;
  tags: string[];
  estimatedMinutes: number;
  required: boolean;
}

export type ResourceType = 'video' | 'article' | 'documentation' | 'code-repo' | 'interactive' | 'quiz';

export interface OnboardingPlan {
  id: string;
  profileId: string;
  name: string;
  description: string;
  modules: OnboardingModule[];
  estimatedTotalMinutes: number;
  startDate: string;
  targetCompletionDate: string;
  milestones: Milestone[];
  mentorAssigned?: MentorInfo;
  status: OnboardingStatus;
  progress: OnboardingProgress;
}

export interface Milestone {
  id: string;
  title: string;
  description: string;
  dueDate: string;
  completed: boolean;
  completedAt?: string;
  moduleIds: string[];
}

export interface MentorInfo {
  id: string;
  name: string;
  email: string;
  role: string;
  availability: string[];
  expertise: string[];
}

export interface OnboardingTask {
  id: string;
  title: string;
  description: string;
  type: TaskType;
  priority: TaskPriority;
  dueDate: string;
  assignedTo: string;
  moduleId?: string;
  status: TaskStatus;
  dependencies: string[]; // Task IDs
  notes?: string;
  attachments?: Attachment[];
  createdAt: string;
  completedAt?: string;
}

export type TaskType = 'orientation' | 'training' | 'assessment' | 'meeting' | 'setup' | 'review' | 'documentation';
export type TaskPriority = 'high' | 'medium' | 'low';
export type TaskStatus = 'pending' | 'in-progress' | 'completed' | 'blocked' | 'cancelled';

export interface Attachment {
  name: string;
  url: string;
  type: string;
  size: number;
}

export interface OnboardingProgressUpdate {
  profileId: string;
  moduleId?: string;
  taskId?: string;
  status: ModuleStatus | TaskStatus;
  score?: number;
  timeSpentMinutes?: number;
  notes?: string;
}

export interface OnboardingRecommendation {
  id: string;
  profileId: string;
  type: RecommendationType;
  title: string;
  description: string;
  priority: TaskPriority;
  suggestedModules: string[];
  suggestedResources: string[];
  suggestedMentor?: string;
  reason: string;
  createdAt: string;
  expiresAt?: string;
}

export type RecommendationType = 
  | 'skill-gap' 
  | 'learning-path' 
  | 'mentor-match' 
  | 'resource-suggestion' 
  | 'milestone-reminder';

export interface OnboardingFeedback {
  id: string;
  profileId: string;
  moduleId?: string;
  taskId?: string;
  rating: number; // 1-5
  comments: string;
  suggestions: string[];
  createdAt: string;
}

export interface OnboardingStats {
  totalOnboardings: number;
  activeOnboardings: number;
  completedOnboardings: number;
  averageCompletionDays: number;
  averageSatisfactionScore: number;
  moduleCompletionRates: Record<string, number>;
  commonChallenges: string[];
  mentorEffectiveness: number;
}

export interface OnboardingRequest {
  userId: string;
  userName: string;
  userEmail: string;
  role: OnboardingRole;
  experienceLevel: ExperienceLevel;
  department: string;
  manager: string;
  startDate: string;
  preferences: OnboardingPreferences;
}

export interface OnboardingResponse {
  success: boolean;
  profileId: string;
  message: string;
  plan: OnboardingPlan;
  nextSteps: string[];
}

export interface OnboardingError {
  code: string;
  message: string;
  details?: string;
  suggestion?: string;
  timestamp: string;
}