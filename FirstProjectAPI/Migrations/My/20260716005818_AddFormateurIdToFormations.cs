using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstProjectAPI.Migrations.My
{
    /// <inheritdoc />
    public partial class AddFormateurIdToFormations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormateurId",
                table: "Formations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Formations_FormateurId",
                table: "Formations",
                column: "FormateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Formations_Formateurs_FormateurId",
                table: "Formations",
                column: "FormateurId",
                principalTable: "Formateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Formations_Formateurs_FormateurId",
                table: "Formations");

            migrationBuilder.DropIndex(
                name: "IX_Formations_FormateurId",
                table: "Formations");

            migrationBuilder.DropColumn(
                name: "FormateurId",
                table: "Formations");
        }
    }
}