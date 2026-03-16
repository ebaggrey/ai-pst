import { BDDScenario } from "../bdd-scenario.model";

// models/responses/bdd-co-creation-response.model.ts
export interface BDDCoCreationResponse {
  sessionId: string;
  requirement: string;
  conversationRounds: ConversationRound[];
  generatedScenarios: BDDScenario[];
  assumptionsChallenged: string[];
  consensusPoints: string[];
  unresolvedQuestions: string[];
  nextSteps: string[];
  conversationQualityScore: number;
}

export interface ConversationRound {
  roundNumber: number;
  stakeholderInputs: string[];
  clarifications: string[];
  decisions: string[];
  consensusScore: number;
  updatedConversation: BDDConversation;
}

export interface BDDConversation {
  id: string;
  participants: string[];
  topics: string[];
  decisions: string[];
  openQuestions: string[];
  createdAt: Date;
  endedAt?: Date;
}