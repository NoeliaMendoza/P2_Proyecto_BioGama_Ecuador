using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioGamaEcuador.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDonationsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "SpeciesSponsorships");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PhysicalProducts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PhysicalProducts",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Orders",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Enrollments",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Courses",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SpeciesSponsorships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DonorEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DonorName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DonorType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    WantsCertificate = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesSponsorships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeciesSponsorships_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    PaymentTransactionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorshipId = table.Column<int>(type: "integer", nullable: false),
                    AmountInCents = table.Column<int>(type: "integer", nullable: false),
                    ClientTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GatewayResponse = table.Column<string>(type: "text", nullable: true),
                    PayPalApprovalUrl = table.Column<string>(type: "text", nullable: true),
                    PayPalCaptureId = table.Column<string>(type: "text", nullable: true),
                    PayPalOrderId = table.Column<string>(type: "text", nullable: true),
                    PayphonePaymentUrl = table.Column<string>(type: "text", nullable: true),
                    PayphoneTransactionId = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.PaymentTransactionId);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_SpeciesSponsorships_SponsorshipId",
                        column: x => x.SponsorshipId,
                        principalTable: "SpeciesSponsorships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_SponsorshipId",
                table: "PaymentTransactions",
                column: "SponsorshipId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesSponsorships_SpeciesId",
                table: "SpeciesSponsorships",
                column: "SpeciesId");
        }
    }
}
