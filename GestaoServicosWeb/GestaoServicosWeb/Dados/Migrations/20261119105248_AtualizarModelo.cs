using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoServicosWeb.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Orcamentos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Orcamentos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Orcamentos");
        }
    }
}
