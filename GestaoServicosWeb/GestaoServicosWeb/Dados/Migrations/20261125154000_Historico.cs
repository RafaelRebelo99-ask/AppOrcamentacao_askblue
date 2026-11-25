using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoServicosWeb.Dados.Migrations
{
    /// <inheritdoc />
    public partial class Historico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Historico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUtilizador = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NomeUtilizador = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Acao = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historico", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Historico_DataHora",
                table: "Historico",
                column: "DataHora");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Historico");
        }
    }
}
