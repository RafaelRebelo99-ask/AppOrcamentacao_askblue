using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoServicosWeb.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AdaptarModeloComercial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orcamentos_IdOportunidade",
                table: "Orcamentos");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamentos_IdOportunidade",
                table: "Orcamentos",
                column: "IdOportunidade",
                unique: true,
                filter: "[Estado] = 'Aceite'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orcamentos_IdOportunidade",
                table: "Orcamentos");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamentos_IdOportunidade",
                table: "Orcamentos",
                column: "IdOportunidade");
        }
    }
}
