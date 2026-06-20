using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BioGamaEcuador.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConservationStatusAndPublication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConservationStatusId",
                table: "Species",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConservationStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConservationStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ConservationStatuses",
                columns: new[] { "Id", "Code", "CreatedAt", "DeletedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "EX", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "No quedan individuos vivos conocidos.", true, "Extinta", null },
                    { 2, "CR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Riesgo extremadamente alto de extinción en estado silvestre.", true, "En peligro crítico", null },
                    { 3, "EN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Riesgo muy alto de extinción en estado silvestre.", true, "En peligro", null },
                    { 4, "VU", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Riesgo alto de extinción en estado silvestre.", true, "Vulnerable", null },
                    { 5, "NT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Podría calificar como amenazada en un futuro cercano.", true, "Casi amenazada", null },
                    { 6, "LC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "No calificada como amenazada actualmente.", true, "Preocupación menor", null },
                    { 7, "DD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "No hay información suficiente para evaluar el riesgo.", true, "Datos insuficientes", null }
                });

            migrationBuilder.Sql(@"
    UPDATE ""Species""
    SET ""ConservationStatusId"" = COALESCE(
        (SELECT cs.""Id"" FROM ""ConservationStatuses"" cs
         WHERE LOWER(TRIM(cs.""Code"")) = LOWER(TRIM(""Species"".""ConservationStatus""))
         LIMIT 1),
        7
    );
");

            migrationBuilder.DropColumn(
                name: "ConservationStatus",
                table: "Species");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Species",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Researchers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Researchers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Records",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Records",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "NaturalReserves",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "NaturalReserves",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Locations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Locations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Families",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Families",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Publications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Journal = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PublicationYear = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ResearcherId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publications_Researchers_ResearcherId",
                        column: x => x.ResearcherId,
                        principalTable: "Researchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicationSpecies",
                columns: table => new
                {
                    PublicationsId = table.Column<int>(type: "integer", nullable: false),
                    RelatedSpeciesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicationSpecies", x => new { x.PublicationsId, x.RelatedSpeciesId });
                    table.ForeignKey(
                        name: "FK_PublicationSpecies_Publications_PublicationsId",
                        column: x => x.PublicationsId,
                        principalTable: "Publications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicationSpecies_Species_RelatedSpeciesId",
                        column: x => x.RelatedSpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Species_ConservationStatusId",
                table: "Species",
                column: "ConservationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Publications_ResearcherId",
                table: "Publications",
                column: "ResearcherId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicationSpecies_RelatedSpeciesId",
                table: "PublicationSpecies",
                column: "RelatedSpeciesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Species_ConservationStatuses_ConservationStatusId",
                table: "Species",
                column: "ConservationStatusId",
                principalTable: "ConservationStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Species_ConservationStatuses_ConservationStatusId",
                table: "Species");

            migrationBuilder.DropTable(
                name: "ConservationStatuses");

            migrationBuilder.DropTable(
                name: "PublicationSpecies");

            migrationBuilder.DropTable(
                name: "Publications");

            migrationBuilder.DropIndex(
                name: "IX_Species_ConservationStatusId",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "ConservationStatusId",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Researchers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Researchers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "NaturalReserves");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "NaturalReserves");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Families");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Families");

            migrationBuilder.AddColumn<string>(
                name: "ConservationStatus",
                table: "Species",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
