using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoworkingGes.Migrations
{
    /// <inheritdoc />
    public partial class lastt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_espace_AspNetUsers_UtilisateurId",
                table: "espace");

            migrationBuilder.AddForeignKey(
                name: "FK_espace_AspNetUsers_UtilisateurId",
                table: "espace",
                column: "UtilisateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_espace_AspNetUsers_UtilisateurId",
                table: "espace");

            migrationBuilder.AddForeignKey(
                name: "FK_espace_AspNetUsers_UtilisateurId",
                table: "espace",
                column: "UtilisateurId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
