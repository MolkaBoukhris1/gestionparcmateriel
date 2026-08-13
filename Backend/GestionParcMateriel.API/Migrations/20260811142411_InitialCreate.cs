using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestionParcMateriel.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Matricule = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Service = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Poste = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EtatsMateriels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtatsMateriels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localisations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localisations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypesMateriels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesMateriels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materiels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeBarre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TypeMaterielId = table.Column<int>(type: "int", nullable: false),
                    Marque = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Modele = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NumeroSerie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateAcquisition = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EtatId = table.Column<int>(type: "int", nullable: false),
                    LocalisationId = table.Column<int>(type: "int", nullable: false),
                    EmployeId = table.Column<int>(type: "int", nullable: true),
                    Actif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Commentaires = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materiels_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Materiels_EtatsMateriels_EtatId",
                        column: x => x.EtatId,
                        principalTable: "EtatsMateriels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Materiels_Localisations_LocalisationId",
                        column: x => x.LocalisationId,
                        principalTable: "Localisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Materiels_TypesMateriels_TypeMaterielId",
                        column: x => x.TypeMaterielId,
                        principalTable: "TypesMateriels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MouvementsMateriels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterielId = table.Column<int>(type: "int", nullable: false),
                    DateMouvement = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    TypeMouvement = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeId = table.Column<int>(type: "int", nullable: true),
                    LocalisationId = table.Column<int>(type: "int", nullable: false),
                    AncienEtatId = table.Column<int>(type: "int", nullable: false),
                    NouvelEtatId = table.Column<int>(type: "int", nullable: false),
                    Commentaires = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MouvementsMateriels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MouvementsMateriels_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MouvementsMateriels_EtatsMateriels_AncienEtatId",
                        column: x => x.AncienEtatId,
                        principalTable: "EtatsMateriels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MouvementsMateriels_EtatsMateriels_NouvelEtatId",
                        column: x => x.NouvelEtatId,
                        principalTable: "EtatsMateriels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MouvementsMateriels_Localisations_LocalisationId",
                        column: x => x.LocalisationId,
                        principalTable: "Localisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MouvementsMateriels_Materiels_MaterielId",
                        column: x => x.MaterielId,
                        principalTable: "Materiels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EtatsMateriels",
                columns: new[] { "Id", "Libelle" },
                values: new object[,]
                {
                    { 1, "En stock" },
                    { 2, "Affecté" },
                    { 3, "En maintenance" },
                    { 4, "Panne" },
                    { 5, "HS" },
                    { 6, "Rebut" }
                });

            migrationBuilder.InsertData(
                table: "Localisations",
                columns: new[] { "Id", "Nom" },
                values: new object[,]
                {
                    { 1, "Salle Serveur" },
                    { 2, "Administration" },
                    { 3, "Atelier" },
                    { 4, "Magasin" },
                    { 5, "Direction" }
                });

            migrationBuilder.InsertData(
                table: "TypesMateriels",
                columns: new[] { "Id", "Libelle" },
                values: new object[,]
                {
                    { 1, "PC Portable" },
                    { 2, "PC Fixe" },
                    { 3, "Écran" },
                    { 4, "Imprimante" },
                    { 5, "Scanner" },
                    { 6, "Switch" },
                    { 7, "Routeur" },
                    { 8, "Téléphone IP" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employe_Matricule",
                table: "Employes",
                column: "Matricule",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EtatMateriel_Libelle",
                table: "EtatsMateriels",
                column: "Libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Localisation_Nom",
                table: "Localisations",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materiel_CodeBarre",
                table: "Materiels",
                column: "CodeBarre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materiel_NumeroSerie",
                table: "Materiels",
                column: "NumeroSerie",
                unique: true,
                filter: "[NumeroSerie] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_EmployeId",
                table: "Materiels",
                column: "EmployeId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_EtatId",
                table: "Materiels",
                column: "EtatId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_LocalisationId",
                table: "Materiels",
                column: "LocalisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_TypeMaterielId",
                table: "Materiels",
                column: "TypeMaterielId");

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsMateriels_AncienEtatId",
                table: "MouvementsMateriels",
                column: "AncienEtatId");

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsMateriels_EmployeId",
                table: "MouvementsMateriels",
                column: "EmployeId");

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsMateriels_LocalisationId",
                table: "MouvementsMateriels",
                column: "LocalisationId");

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsMateriels_MaterielId",
                table: "MouvementsMateriels",
                column: "MaterielId");

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsMateriels_NouvelEtatId",
                table: "MouvementsMateriels",
                column: "NouvelEtatId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeMateriel_Libelle",
                table: "TypesMateriels",
                column: "Libelle",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MouvementsMateriels");

            migrationBuilder.DropTable(
                name: "Materiels");

            migrationBuilder.DropTable(
                name: "Employes");

            migrationBuilder.DropTable(
                name: "EtatsMateriels");

            migrationBuilder.DropTable(
                name: "Localisations");

            migrationBuilder.DropTable(
                name: "TypesMateriels");
        }
    }
}
