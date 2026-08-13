using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionParcMateriel.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Utilisateurs",
                columns: new[] { "Id", "Email", "MotDePasseHash", "NomComplet", "Role" },
                values: new object[] { 2, "user@sopal.tn", "vFhI8ifMFh619o3+mMsTEQqchDzmnpU6iBB9hlWD05c=", "Technicien IT", "Utilisateur" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Utilisateurs",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
