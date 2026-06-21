using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioGamaEcuador.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedAtToSpecies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Species",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Species");
        }
    }
}
