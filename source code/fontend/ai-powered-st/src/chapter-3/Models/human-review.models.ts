// models/domain/generated-test.model.ts
export interface GeneratedTest {
  content: string;
  confidenceScore: number;
  generatedAt: Date;
  testType: string;
  framework: string;
  language?: string;
  tags?: string[];
  dependencies?: string[];
  estimatedExecutionTimeMs?: number;
  complexity?: string;
  testCategories?: string[];
  modelVersion?: string;
  generationId?: string;
  confidenceBreakdown?: Record<string, number>;
  metadata?: GeneratedTestMetadata;
  assertions?: TestAssertion[];
  setup?: string;
  teardown?: string;
  testCases?: TestCase[];
}

export interface GeneratedTestMetadata {
  sourceFile?: string;
  targetMethod?: string;
  parameters?: Record<string, string>;
  coverageAreas?: string[];
  qualityScore?: number;
  isFlaky?: boolean;
  requiresSetup?: boolean;
  hasSideEffects?: boolean;
  generationContext?: Record<string, any>;
  customProperties?: Record<string, any>;
}

export interface TestAssertion {
  type: string;
  expected: string;
  actual: string;
  message?: string;
  lineNumber?: number;
  confidence?: number;
  metadata?: Record<string, any>;
}

export interface TestCase {
  name: string;
  description?: string;
  input?: Record<string, any>;
  expectedOutput?: any;
  setup?: string;
  teardown?: string;
  assertions?: TestAssertion[];
  isDataDriven?: boolean;
  data?: any[];
  metadata?: Record<string, any>;
}

// models/domain/generated-content.model.ts
export interface GeneratedContent {
  test: GeneratedTest;
  testType: string;
  sessionContext?: SessionContentContext;
  modifications?: ContentModification[];
  reviewStatus?: ContentReviewStatus;
  aiAnalysis?: ContentAiAnalysis;
  humanFeedback?: HumanFeedback[];
  version?: number;
  createdAt?: Date;
  lastModified?: Date;
  metadata?: GeneratedContentMetadata;
  
  // Convenience properties
  content?: string;
  confidenceScore?: number;
}

export interface SessionContentContext {
  purpose?: string;
  requirements?: string[];
  constraints?: Record<string, string>;
  dependencies?: string[];
  businessRules?: BusinessRule[];
  acceptanceCriteria?: string[];
}

export interface ContentModification {
  id: string;
  type: string;
  description?: string;
  appliedBy: string;
  appliedAt: Date;
  impact: ModificationImpact;
  before?: string;
  after?: string;
  reason?: string;
}

export interface ModificationImpact {
  level: string;
  affectedLines?: number[];
  qualityChange?: number;
  complexityChange?: number;
  coverageChange?: number;
}

export interface ContentReviewStatus {
  status: string;
  progress?: number;
  reviewedBy?: string[];
  reviewedAt?: Date[];
  issuesFound?: number;
  suggestionsApplied?: number;
}

export interface ContentAiAnalysis {
  confidence?: number;
  riskAreas?: string[];
  suggestions?: AiSuggestion[];
  qualityMetrics?: QualityMetrics;
  generatedAt?: Date;
  modelVersion?: string;
}

export interface AiSuggestion {
  id: string;
  suggestion: string;
  type: string;
  confidence: number;
  reasoning?: string;
  autoApply?: boolean;
  generatedAt?: Date;
  category?: string;
  priority?: string;
  affectedLines?: string[];
  prerequisites?: string[];
  estimatedEffort?: string;
  relatedSuggestions?: string[];
  metadata?: Record<string, any>;
}

export interface QualityMetrics {
  cyclomaticComplexity?: number;
  linesOfCode?: number;
  maintainabilityIndex?: number;
  testCoverage?: number;
  codeDuplication?: number;
  securityIssues?: number;
}

export interface HumanFeedback {
  id: string;
  type: string;
  content: string;
  providedBy: string;
  providedAt: Date;
  priority?: string;
  status?: string;
  lineNumbers?: number[];
  category?: string;
}

export interface GeneratedContentMetadata {
  source?: string;
  generationMethod?: string;
  tags?: string[];
  customProperties?: Record<string, any>;
  qualityScore?: number;
  complexityScore?: number;
  maintainabilityScore?: number;
}

export interface BusinessRule {
  id: string;
  description?: string;
  condition?: string;
  priority?: string;
}

// models/domain/review-session.model.ts
export interface ReviewSession {
  id: string;
  originalContent: GeneratedContent;
  currentContent: GeneratedContent;
  context: ReviewContext;
  analysis?: ReviewAnalysis;
  createdAt: Date;
  status: ReviewSessionStatus;
  aiConfidenceStatement?: string;
  areasNeedingHumanJudgment?: JudgmentArea[];
  suggestedReviewFocus?: string[];
  initialQuestions?: InitialQuestion[];
  editHistory?: EditRecord[];
  aiSuggestions?: AiSuggestion[];
  clarificationThreads?: ClarificationThread[];
  lastModified?: Date;
  closedAt?: Date;
  outcome?: ReviewOutcome;
  summary?: SessionSummary;
  activeCollaborators?: string[];
  version?: number;
  workspaceUrl?: string;
  metadata?: Record<string, any>;
  
  // Convenience property
  testType?: string;
}

export type ReviewSessionStatus = 
  | 'awaiting_review'
  | 'in_progress'
  | 'under_clarification'
  | 'pending_judgment'
  | 'closed';

export interface ReviewContext {
  testPurpose: string;
  riskLevel: string;
  businessDomains?: string[];
  technicalDomains?: string[];
  constraints?: Record<string, string>;
  priority?: string;
  stakeholders?: string[];
}

export interface ReviewAnalysis {
  needsHumanReview?: string[];
  likelyCorrect?: string[];
  missingEdgeCases?: string[];
  businessContextConcerns?: string[];
  flakinessRisks?: string[];
  overallConfidence?: number;
  areaConfidenceScores?: Record<string, number>;
}

export interface JudgmentArea {
  area: string;
  description?: string;
  whyHumanNeeded?: string;
  guidance?: string;
  examples?: string[];
  commonPitfalls?: string[];
  priority?: string;
  aiConfidenceInArea?: number;
  relatedAreas?: string[];
  validationCriteria?: string[];
  toolsNeeded?: string[];
  metadata?: Record<string, any>;
}

export interface InitialQuestion {
  question: string;
  type: string;
  whyImportant?: string;
  options?: string[];
  isRequired?: boolean;
  priority?: number;
  category?: string;
  tags?: string[];
  expectedAnswerType?: string;
  timeToAnswerSeconds?: number;
  hints?: string[];
  metadata?: Record<string, any>;
}

export interface EditRecord {
  edit: UserEdit;
  appliedAt: Date;
  appliedBy: string;
  impact: EditImpact;
  editId?: string;
  version?: number;
  dependencies?: string[];
  approvedByAi?: boolean;
  reviewComments?: string[];
  metadata?: Record<string, any>;
}

export interface UserEdit {
  content: string;
  intent: string;
  editorId: string;
  editType?: string;
  affectedLines?: string[];
  metadata?: Record<string, any>;
  createdAt?: Date;
  priority?: number;
}

export interface EditImpact {
  impactLevel: string;
  affectedAreas?: string[];
  risksIntroduced?: string[];
  benefits?: string[];
  summary?: string;
  linesChanged?: number;
  dependenciesAffected?: number;
  complexityChange?: number;
  testCoverageImpact?: number;
  maintainabilityImpact?: number;
  performanceImpact?: number;
  businessRiskLevel?: string;
  businessAreasAffected?: string[];
  metrics?: Record<string, any>;
}

export interface ClarificationThread {
  threadId: string;
  startedAt: Date;
  lastActivity?: Date;
  resolved?: boolean;
  resolvedAt?: Date;
  rounds: ClarificationRound[];
  topic?: string;
  tags?: string[];
  priority?: string;
  participants?: string[];
  metadata?: Record<string, any>;
}

export interface ClarificationRound {
  roundId: string;
  humanQuestion: string;
  aiResponse: AiClarification;
  askedAt: Date;
  questionType?: string;
  questionClarityScore?: number;
  answerRelevanceScore?: number;
  requiresFollowUp?: boolean;
  keywords?: string[];
  metadata?: Record<string, any>;
}

export interface AiClarification {
  directAnswer: string;
  alternatives?: string[];
  confidence: number;
  confidenceStatement?: string;
  assumptions?: string[];
  recommendedAction?: string;
  whenToAskHuman?: string;
  references?: string[];
  relatedQuestions?: string[];
  answerType?: string;
  keyPoints?: string[];
  limitations?: string[];
  generatedAt?: Date;
  modelVersion?: string;
  metadata?: Record<string, any>;
}

export interface ReviewOutcome {
  decision: string;
  decisionSummary: string;
  appliedEdits?: string[];
  acceptedSuggestions?: string[];
  decidedAt: Date;
  decidedBy: string;
  metadata?: Record<string, any>;
}

export interface SessionSummary {
  sessionId: string;
  createdAt: Date;
  closedAt?: Date;
  duration?: number;
  editCount?: number;
  clarificationCount?: number;
  finalDecision?: string;
  keyInsights?: string[];
  learningPoints?: LearningPoint[];
  metrics?: Record<string, any>;
}

export interface LearningPoint {
  category: string;
  description: string;
  impact?: string;
  examples?: string[];
  appliedToModel?: boolean;
  appliedAt?: Date;
}


// models/requests/review-request.model.ts
export interface ReviewRequest {
  generatedContent: GeneratedTest;
  context: ReviewContext;
  reviewerGuidance: string;
  submissionMetadata: SubmissionMetadata;
  preferredReviewers?: string[];
  priority?: string;
  enableRealTimeCollaboration?: boolean;
  autoSuggestImprovements?: boolean;
  reviewCategories?: string[];
  deadline?: Date;
  estimatedEffortMinutes?: number;
  additionalSettings?: Record<string, any>;
 
}

export interface SubmissionMetadata {
  submittedBy: string;
  submittedFrom?: string;
  submittedAt: Date;
  requestId: string;
  additionalInfo?: Record<string, string>;
}

// models/requests/collaborative-edit-request.model.ts
export interface CollaborativeEditRequest {
  userEdit: UserEdit;
  editContext?: string;
  relatedIssues?: string[];
  requestAiAnalysis?: boolean;
}

// models/requests/clarification-request.model.ts
export interface ClarificationRequest {
  humanQuestion: string;
  questionType?: string;
  contextTags?: string[];
  urgency?: string;
  additionalContext?: Record<string, string>;
}

// models/requests/judgment-request.model.ts
export interface JudgmentRequest {
  judgment: HumanJudgment;
  areasReviewed?: string[];
  feedbackForAi?: string;
  storeForTraining?: boolean;
  metadata?: Record<string, any>;
}

export interface HumanJudgment {
  decision: string;
  reasoning: string;
  suggestedImprovements?: string[];
  areasOfConcern?: string[];
  confidenceInJudgment?: number;
  supportingEvidence?: string[];
  specificFeedback?: Record<string, string>;
}

// models/responses/review-session-response.model.ts
export interface ReviewSessionResponse {
  sessionId: string;
  status?: string;
  reviewersAssigned: string[];
  estimatedReviewTime: number; // in milliseconds
  reviewChecklist: ReviewChecklistItem[];
  workspaceUrl: string;
  initialQuestions: InitialQuestion[];
  aiConfidenceStatement: string;
  areasNeedingHumanJudgment: JudgmentArea[];
  availableTools: CollaborationTools;
  quickActions: string[];
  createdAt?: Date;
  expiresAt?: Date;
  editCount?: number;
  clarificationCount?: number;
  progressPercentage?: number;
  metadata?: Record<string, any>;
}

export interface ReviewChecklistItem {
  item: string;
  category: string;
  description?: string;
  isRequired?: boolean;
  guidance?: string;
  examples?: string[];
  priority?: string;
  relatedItems?: string[];
  isCompleted?: boolean;
  completedAt?: Date;
  completedBy?: string;
  metadata?: Record<string, any>;
}

export interface CollaborationTools {
  realTimeEditing?: boolean;
  comments?: boolean;
  suggestions?: boolean;
  chat?: boolean;
  versionHistory?: boolean;
  sideBySideDiff?: boolean;
  autoSave?: boolean;
  conflictDetection?: boolean;
  presenceIndicators?: boolean;
  changeTracking?: boolean;
  reviewMode?: boolean;
  settings?: Record<string, any>;
}

// models/responses/ai-edit-analysis.model.ts
export interface AiEditAnalysis {
  editId?: string;
  alignsWithIntent: boolean;
  improvements?: string[];
  potentialIssues?: string[];
  alternativeSuggestions?: string[];
  editImpact: EditImpact;
  shouldLearnFromEdit?: boolean;
  analyzedAt?: Date;
  confidenceInAnalysis?: number;
}

// models/responses/collaboration-response.model.ts
export interface CollaborationResponse {
  session: ReviewSession;
  aiPerspective: AiEditAnalysis;
  impactAnalysis: EditImpact;
  suggestedNextEdits?: AiSuggestion[];
  learningOpportunities?: LearningOpportunity[];
}

export interface LearningOpportunity {
  opportunity: string;
  category: string;
  whyImportant?: string;
  examples?: string[];
  shouldPrioritize?: boolean;
}

// models/responses/collaboration-error.model.ts
export interface CollaborationError {
  sessionId: string;
  conflictType: string;
  conflictingEdits: UserEdit[];
  resolutionOptions: string[];
  aiMergeSuggestion: string;
  occurredAt: Date;
}

// models/responses/question-analysis.model.ts
export interface QuestionAnalysis {
  isAmbiguous: boolean;
  interpretations?: string[];
  questionType?: string;
  keyTopics?: string[];
  underlyingNeed?: string;
  clarityScore?: number;
  missingContext?: string[];
}

// models/responses/clarification-response.model.ts
export interface ClarificationResponse {
  roundId: string;
  questionAnalysis: QuestionAnalysis;
  aiAnswer: AiClarification;
  relevanceScore: number;
  suggestedFollowUps?: string[];
  confidenceStatement?: string;
  whenToAskHuman?: string;
  rephrasingSuggestions?: string[];
}

// models/responses/judgment-response.model.ts
export interface JudgmentResponse {
  outcome: ReviewOutcome;
  insights: ReviewInsight[];
  nextSteps: string[];
  feedbackForHuman: string;
  modelUpdatesApplied: ModelUpdateSummary[];
}

export interface ReviewInsight {
  insight: string;
  category: string;
  impact?: string;
  evidence?: string[];
  actionable?: boolean;
}

export interface ModelUpdateSummary {
  modelName: string;
  updateType: string;
  areasUpdated: string[];
  confidenceImpact: number;
  updatedAt: Date;
}

// models/responses/human-review-error.model.ts
export interface HumanReviewError {
  errorCode: string;
  message: string;
  requiredElements?: string[];
  missingElements?: string[];
  suggestion?: string;
  contextEnhancementPrompts?: string[];
  recoveryActions?: string[];
  reportedAt: Date;
  errorType?: string;
  referenceId?: string;
  details?: Record<string, any>;
}

// models/domain/enums.ts
export enum RiskLevel {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high',
  CRITICAL = 'critical'
}

export enum TestType {
  UNIT = 'unit',
  INTEGRATION = 'integration',
  FUNCTIONAL = 'functional',
  E2E = 'e2e',
  PERFORMANCE = 'performance',
  SECURITY = 'security',
  REGRESSION = 'regression'
}

export enum Priority {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high',
  URGENT = 'urgent'
}

export enum ReviewDecision {
  APPROVE = 'approve',
  REQUEST_REVISION = 'request-revision',
  REJECT = 'reject'
}



// human-review.models.ts or at the top of component
export interface GeneratedTestMetadata {
  source?: string;
  generationMethod?: string;
  tags?: string[];
  qualityScore?: number;
  customProperties?: Record<string, any>;
  [key: string]: any; // This allows any additional properties
}

export interface GeneratedTest {
  content: string;
  confidenceScore: number;
  generatedAt: Date ;
  testType: string;
  framework: string;
  language?: string;
  tags?: string[];
  dependencies?: string[];
  metadata?: GeneratedTestMetadata;
  assertions?: TestAssertion[];
  testCases?: TestCase[];
  [key: string]: any; // Allow additional properties
}