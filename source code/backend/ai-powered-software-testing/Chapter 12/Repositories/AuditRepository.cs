

// Repositories/AuditRepository.cs

using Chapter_12.Data;
using Chapter_12.Interfaces;
using Chapter_12.Models.Database;
using Chapter_12.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Chapter_12.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly AuditDbContext _context;
        private readonly ILogger<AuditRepository> _logger;

        public AuditRepository(AuditDbContext context, ILogger<AuditRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AuditRecord> SaveAuditRecordAsync(BiasAuditResponse auditResponse, string status)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create the main audit record
                var auditRecord = new AuditRecord
                {
                    AuditId = $"AUD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 30),
                    DatasetName = auditResponse.DatasetName,
                    AuditDate = auditResponse.AuditDate,
                    Status = status,
                    OverallBiasScore = auditResponse.OverallBiasScore?.OverallScore ?? 0,
                    RiskLevel = auditResponse.OverallBiasScore?.RiskLevel ?? "Unknown",
                    FullResponse = JsonSerializer.Serialize(auditResponse),
                    SuggestionCount = auditResponse.Suggestions?.Count ?? 0,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    BiasFindings = new List<BiasFindingEntity>(),
                    Suggestions = new List<SuggestionEntity>()
                };

                // Add bias findings
                if (auditResponse.Findings != null && auditResponse.Findings.Any())
                {
                    foreach (var finding in auditResponse.Findings)
                    {
                        auditRecord.BiasFindings.Add(new BiasFindingEntity
                        {
                            FieldName = finding.FieldName,
                            BiasType = finding.BiasType,
                            Description = finding.Description,
                            SeverityScore = finding.SeverityScore,
                            Examples = JsonSerializer.Serialize(finding.Examples ?? new List<string>())
                        });
                    }
                }

                // Add suggestions
                if (auditResponse.Suggestions != null && auditResponse.Suggestions.Any())
                {
                    foreach (var suggestion in auditResponse.Suggestions)
                    {
                        auditRecord.Suggestions.Add(new SuggestionEntity
                        {
                            OriginalValue = suggestion.OriginalValue,
                            SuggestedValue = suggestion.SuggestedValue,
                            FieldName = suggestion.FieldName,
                            Rationale = suggestion.Rationale,
                            ConfidenceScore = suggestion.ConfidenceScore
                        });
                    }
                }

                // Save to database
                _context.AuditRecords.Add(auditRecord);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return auditRecord;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error saving audit record for dataset {DatasetName}",
                    auditResponse.DatasetName);
                throw;
            }
        }

        public async Task<List<AuditRecord>> GetAuditRecordsByDatasetAsync(string datasetName, DateTime? fromDate = null)
        {
            var query = _context.AuditRecords
                .Include(a => a.BiasFindings)
                .Include(a => a.Suggestions)
                .Where(a => a.DatasetName == datasetName);

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.AuditDate >= fromDate.Value);
            }

            return await query
                .OrderByDescending(a => a.AuditDate)
                .ToListAsync();
        }

        public async Task<AuditRecord> GetAuditRecordByIdAsync(string auditId)
        {
            return await _context.AuditRecords
                .Include(a => a.BiasFindings)
                .Include(a => a.Suggestions)
                .FirstOrDefaultAsync(a => a.AuditId == auditId);
        }
    }
}