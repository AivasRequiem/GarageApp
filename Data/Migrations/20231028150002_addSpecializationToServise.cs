using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addSpecializationToServise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SpecializationId",
                table: "GarageServise",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_GarageServise_SpecializationId",
                table: "GarageServise",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageServise_Specialization_SpecializationId",
                table: "GarageServise",
                column: "SpecializationId",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageServise_Specialization_SpecializationId",
                table: "GarageServise");

            migrationBuilder.DropIndex(
                name: "IX_GarageServise_SpecializationId",
                table: "GarageServise");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "GarageServise");
        }
    }
}