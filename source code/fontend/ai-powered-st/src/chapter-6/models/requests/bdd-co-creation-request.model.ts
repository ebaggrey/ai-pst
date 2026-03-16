// models/requests/bdd-co-creation-request.model.ts
export interface BDCCoCreationRequest {
  requirement: string;
  stakeholderPerspectives: StakeholderPerspective[];
  conversationConstraints: ConversationConstraints;
  desiredOutcomes: string[];
}

export interface StakeholderPerspective {
  role: string;
  priorities: string[];
  concerns: string[];
}

export interface ConversationConstraints {
  maxRounds: number;
  consensusThreshold: number;
  forbiddenAssumptions: string[];
}