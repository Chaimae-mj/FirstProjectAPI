using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;
namespace FirstProjectAPI.Migrations.My
{
    /// <inheritdoc />
    public partial class CreateFormateursTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Formateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                       .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),

                    Nom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),

                    Prenom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),

                    Email = table.Column<string>(type: "longtext", nullable: false),

                    Telephone = table.Column<string>(type: "longtext", nullable: true),

                    Specialite = table.Column<string>(type: "longtext", nullable: true),

                    Photo = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formateurs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
