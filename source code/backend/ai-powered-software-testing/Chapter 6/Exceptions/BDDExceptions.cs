namespace Chapter_6.Exceptions
{
    // Exceptions/BDDExceptions.cs
    
        public class ConversationStallException : Exception
        {
            public string ConversationId { get; }
            public int RoundNumber { get; }
            public double ConsensusScore { get; }

            public ConversationStallException(string message, string conversationId, int roundNumber, double consensusScore)
                : base(message)
            {
                ConversationId = conversationId;
                RoundNumber = roundNumber;
                ConsensusScore = consensusScore;
            }

            public ConversationStallException(string message, string conversationId, int roundNumber, double consensusScore, Exception innerException)
                : base(message, innerException)
            {
                ConversationId = conversationId;
                RoundNumber = roundNumber;
                ConsensusScore = consensusScore;
            }
        }

        public class StakeholderConflictException : Exception
        {
            public string[] ConflictingStakeholders { get; }
            public string[] ConflictAreas { get; }

            public StakeholderConflictException(string message, string[] conflictingStakeholders, string[] conflictAreas)
                : base(message)
            {
                ConflictingStakeholders = conflictingStakeholders;
                ConflictAreas = conflictAreas;
            }

            public StakeholderConflictException(string message, string[] conflictingStakeholders, string[] conflictAreas, Exception innerException)
                : base(message, innerException)
            {
                ConflictingStakeholders = conflictingStakeholders;
                ConflictAreas = conflictAreas;
            }
        }

        public class AmbiguousStepException : Exception
        {
            public string AmbiguousStep { get; }
            public string[] PossibleInterpretations { get; }

            public AmbiguousStepException(string message, string ambiguousStep, string[] possibleInterpretations)
                : base(message)
            {
                AmbiguousStep = ambiguousStep;
                PossibleInterpretations = possibleInterpretations;
            }

            public AmbiguousStepException(string message, string ambiguousStep, string[] possibleInterpretations, Exception innerException)
                : base(message, innerException)
            {
                AmbiguousStep = ambiguousStep;
                PossibleInterpretations = possibleInterpretations;
            }
        }

        public class UnsupportedPatternException : Exception
        {
            public string Pattern { get; }
            public string SuggestedRefactoring { get; }

            public UnsupportedPatternException(string message, string pattern, string suggestedRefactoring)
                : base(message)
            {
                Pattern = pattern;
                SuggestedRefactoring = suggestedRefactoring;
            }

            public UnsupportedPatternException(string message, string pattern, string suggestedRefactoring, Exception innerException)
                : base(message, innerException)
            {
                Pattern = pattern;
                SuggestedRefactoring = suggestedRefactoring;
            }
        }

   


    public class IntentPreservationException : Exception
    {
        public string ProblematicScenario { get; }
        public string[] Ambiguities { get; }

        public IntentPreservationException(string message, string problematicScenario, string[] ambiguities)
            : base(message)
        {
            ProblematicScenario = problematicScenario;
            Ambiguities = ambiguities ?? Array.Empty<string>();
        }

        public IntentPreservationException(string message, string problematicScenario, Exception innerException)
            : base(message, innerException)
        {
            ProblematicScenario = problematicScenario;
            Ambiguities = Array.Empty<string>();
        }
    }

    public class DriftAnalysisException : Exception
        {
            public string AnalysisMethod { get; }
            public string[] FailedScenarios { get; }

            public DriftAnalysisException(string message, string analysisMethod, string[] failedScenarios)
                : base(message)
            {
                AnalysisMethod = analysisMethod;
                FailedScenarios = failedScenarios;
            }

            public DriftAnalysisException(string message, string analysisMethod, string[] failedScenarios, Exception innerException)
                : base(message, innerException)
            {
                AnalysisMethod = analysisMethod;
                FailedScenarios = failedScenarios;
            }
        }

        public class AudienceMismatchException : Exception
        {
        private string role;
        private string[] strings;

        public string TargetAudience { get; }
            public string[] UnsupportedFormats { get; }

            public AudienceMismatchException(string message, string targetAudience, string[] unsupportedFormats, Exception ex)
                : base(message)
            {
                TargetAudience = targetAudience;
                UnsupportedFormats = unsupportedFormats;
            }

            public AudienceMismatchException(string message, string targetAudience, Exception innerException)
                : base(message, innerException)
            {
                TargetAudience = targetAudience;
                UnsupportedFormats = Array.Empty<string>();
            }

        public AudienceMismatchException(string? message, string role, string[] strings) : base(message)
        {
            this.role = role;
            this.strings = strings;
        }
    }
   
}
