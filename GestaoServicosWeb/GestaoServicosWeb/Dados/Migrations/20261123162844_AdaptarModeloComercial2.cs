using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoServicosWeb.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AdaptarModeloComercial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Utilizadores",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Utilizadores",
                columns: new[] { "Id", "Ativo", "Email", "EmpresaId", "Nome", "PasswordHash", "Role" },
                values: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), true, "admin@askblue.com", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Administrador", "YOUR_PRECOMPUTED_PASSWORD_HASH", "Administrador" });
        }
    }
}
