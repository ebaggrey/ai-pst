using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chapter_7.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pipelines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PipelineId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PipelineDefinition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TestCoverage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLines = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pipelines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PredictionModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accuracy = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeactivatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdaptationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PipelineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdaptationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdaptationPlan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImpactScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValidationResults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImplementedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdaptationEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdaptationEvents_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OptimizationOpportunities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PipelineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpportunityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Impact = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Effort = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImplementedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptimizationOpportunities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptimizationOpportunities_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PipelineRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PipelineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TriggeredBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TriggeredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineRuns_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PipelineStages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PipelineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StageOrder = table.Column<int>(type: "int", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AverageDuration = table.Column<int>(type: "int", nullable: true),
                    SuccessRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineStages_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PredictionResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PipelineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PredictionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PredictionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PredictionData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualOutcome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionResults_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PredictionResults_PredictionModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "PredictionModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PipelineFailures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FailureType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FailureMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FailureStage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RawLogs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineFailures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineFailures_PipelineRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "PipelineRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdaptationEvents_AdaptationId",
                table: "AdaptationEvents",
                column: "AdaptationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdaptationEvents_PipelineId",
                table: "AdaptationEvents",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_OptimizationOpportunities_OpportunityId",
                table: "OptimizationOpportunities",
                column: "OpportunityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OptimizationOpportunities_PipelineId",
                table: "OptimizationOpportunities",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineFailures_FailureType",
                table: "PipelineFailures",
                column: "FailureType");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineFailures_RunId",
                table: "PipelineFailures",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRuns_PipelineId",
                table: "PipelineRuns",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRuns_RunId",
                table: "PipelineRuns",
                column: "RunId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRuns_Status",
                table: "PipelineRuns",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRuns_TriggeredAt",
                table: "PipelineRuns",
                column: "TriggeredAt");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_Language",
                table: "Pipelines",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_PipelineId",
                table: "Pipelines",
                column: "PipelineId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_Status",
                table: "Pipelines",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStages_PipelineId_StageId",
                table: "PipelineStages",
                columns: new[] { "PipelineId", "StageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PredictionModels_ModelId_Version",
                table: "PredictionModels",
                columns: new[] { "ModelId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PredictionResults_ModelId",
                table: "PredictionResults",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionResults_PipelineId",
                table: "PredictionResults",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionResults_PredictionId",
                table: "PredictionResults",
                column: "PredictionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdaptationEvents");

            migrationBuilder.DropTable(
                name: "OptimizationOpportunities");

            migrationBuilder.DropTable(
                name: "PipelineFailures");

            migrationBuilder.DropTable(
                name: "PipelineStages");

            migrationBuilder.DropTable(
                name: "PredictionResults");

            migrationBuilder.DropTable(
                name: "PipelineRuns");

            migrationBuilder.DropTable(
                name: "PredictionModels");

            migrationBuilder.DropTable(
                name: "Pipelines");
        }
    }
}
