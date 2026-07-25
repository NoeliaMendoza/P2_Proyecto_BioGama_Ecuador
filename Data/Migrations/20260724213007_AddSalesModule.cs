using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioGamaEcuador.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Syllabus = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalSeats = table.Column<int>(type: "integer", nullable: false),
                    ReservedSeats = table.Column<int>(type: "integer", nullable: false),
                    ConfirmedSeats = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Modality = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Venue = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Instructor = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    InstructorBio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresPriorKnowledge = table.Column<bool>(type: "boolean", nullable: false),
                    TargetAudience = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.CheckConstraint("CK_Course_Dates_Valid", "\"EndDate\" >= \"StartDate\"");
                    table.CheckConstraint("CK_Course_Price_Positive", "\"Price\" > 0");
                    table.CheckConstraint("CK_Course_Seats_Consistent", "\"ReservedSeats\" >= 0 AND \"ConfirmedSeats\" >= 0 AND (\"ReservedSeats\" + \"ConfirmedSeats\") <= \"TotalSeats\"");
                    table.CheckConstraint("CK_Course_Times_Valid", "\"EndTime\" > \"StartTime\"");
                    table.CheckConstraint("CK_Course_TotalSeats_Positive", "\"TotalSeats\" > 0");
                    table.ForeignKey(
                        name: "FK_Courses_Species",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ShippingAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    UserNavigationId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserNavigationId",
                        column: x => x.UserNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhysicalProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    ReservedStock = table.Column<int>(type: "integer", nullable: false),
                    MinStock = table.Column<int>(type: "integer", nullable: false),
                    SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalProducts", x => x.Id);
                    table.CheckConstraint("CK_PhysicalProduct_Price_Positive", "\"Price\" > 0");
                    table.CheckConstraint("CK_PhysicalProduct_ReservedStock_NonNegative", "\"ReservedStock\" >= 0");
                    table.CheckConstraint("CK_PhysicalProduct_Stock_Consistent", "\"ReservedStock\" <= \"Stock\"");
                    table.CheckConstraint("CK_PhysicalProduct_Stock_NonNegative", "\"Stock\" >= 0");
                    table.ForeignKey(
                        name: "FK_PhysicalProducts_Species",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SpeciesSponsorships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DonorName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DonorEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DonorType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WantsCertificate = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConfirmationCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AttendedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GatewayResponse = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    VerificationAttempts = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: true),
                    PhysicalProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PreviousStock = table.Column<int>(type: "integer", nullable: false),
                    NewStock = table.Column<int>(type: "integer", nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryMovements", x => x.Id);
                    table.CheckConstraint("CK_InventoryMovement_Quantity_Positive", "\"Quantity\" > 0");
                    table.CheckConstraint("CK_InventoryMovement_Stock_NonNegative", "\"PreviousStock\" >= 0 AND \"NewStock\" >= 0");
                    table.ForeignKey(
                        name: "FK_InventoryMovements_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryMovements_PhysicalProducts_PhysicalProductId",
                        column: x => x.PhysicalProductId,
                        principalTable: "PhysicalProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: true),
                    PhysicalProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_PhysicalProducts_PhysicalProductId",
                        column: x => x.PhysicalProductId,
                        principalTable: "PhysicalProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    PaymentTransactionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorshipId = table.Column<int>(type: "integer", nullable: false),
                    Provider = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ClientTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PayphonePaymentUrl = table.Column<string>(type: "text", nullable: true),
                    PayphoneTransactionId = table.Column<string>(type: "text", nullable: true),
                    PayPalOrderId = table.Column<string>(type: "text", nullable: true),
                    PayPalCaptureId = table.Column<string>(type: "text", nullable: true),
                    PayPalApprovalUrl = table.Column<string>(type: "text", nullable: true),
                    GatewayResponse = table.Column<string>(type: "text", nullable: true),
                    AmountInCents = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "IX_Courses_Instructor",
                table: "Courses",
                column: "Instructor");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsActive",
                table: "Courses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsActive_StartDate",
                table: "Courses",
                columns: new[] { "IsActive", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Modality",
                table: "Courses",
                column: "Modality");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SpeciesId",
                table: "Courses",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_StartDate",
                table: "Courses",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ConfirmationCode",
                table: "Enrollments",
                column: "ConfirmationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId_UserId",
                table: "Enrollments",
                columns: new[] { "CourseId", "UserId" },
                unique: true,
                filter: "\"Status\" != 'Cancelled'");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_OrderId",
                table: "Enrollments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_Status",
                table: "Enrollments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_CourseId",
                table: "InventoryMovements",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_CourseId_Date",
                table: "InventoryMovements",
                columns: new[] { "CourseId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_Date",
                table: "InventoryMovements",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_PhysicalProductId",
                table: "InventoryMovements",
                column: "PhysicalProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_PhysicalProductId_Date",
                table: "InventoryMovements",
                columns: new[] { "PhysicalProductId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_Type",
                table: "InventoryMovements",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_UserId_Date",
                table: "InventoryMovements",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_CourseId",
                table: "OrderDetails",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_PhysicalProductId",
                table: "OrderDetails",
                column: "PhysicalProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserNavigationId",
                table: "Orders",
                column: "UserNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Provider_ExternalId",
                table: "Payments",
                columns: new[] { "Provider", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_SponsorshipId",
                table: "PaymentTransactions",
                column: "SponsorshipId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalProducts_IsActive",
                table: "PhysicalProducts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalProducts_SKU",
                table: "PhysicalProducts",
                column: "SKU",
                unique: true,
                filter: "\"SKU\" IS NOT NULL AND \"SKU\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalProducts_SpeciesId",
                table: "PhysicalProducts",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesSponsorships_SpeciesId",
                table: "SpeciesSponsorships",
                column: "SpeciesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "InventoryMovements");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "PhysicalProducts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "SpeciesSponsorships");
        }
    }
}
