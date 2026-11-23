using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoServicosWeb.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AdaptarModeloComercial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orcamentos_Servicos_IdServico",
                table: "Orcamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Orcamentos_Utilizadores_IdPrestador",
                table: "Orcamentos");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_NIF",
                table: "Utilizadores");

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DropColumn(
                name: "NIF",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Utilizadores");

            migrationBuilder.RenameColumn(
                name: "IdServico",
                table: "Orcamentos",
                newName: "IdOportunidade");

            migrationBuilder.RenameColumn(
                name: "IdPrestador",
                table: "Orcamentos",
                newName: "IdCriador");

            migrationBuilder.RenameIndex(
                name: "IX_Orcamentos_IdServico",
                table: "Orcamentos",
                newName: "IX_Orcamentos_IdOportunidade");

            migrationBuilder.RenameIndex(
                name: "IX_Orcamentos_IdPrestador",
                table: "Orcamentos",
                newName: "IX_Orcamentos_IdCriador");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Utilizadores",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Utilizadores",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Utilizadores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "EmpresaId",
                table: "Utilizadores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Orcamentos",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Orcamentos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDecisao",
                table: "Orcamentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEnvio",
                table: "Orcamentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Oportunidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCliente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCategoria = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdResponsavel = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrazoProposta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataPretendida = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oportunidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Oportunidades_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Oportunidades_Utilizadores_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Oportunidades_Utilizadores_IdResponsavel",
                        column: x => x.IdResponsavel,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Nome",
                value: "Monitorização e Suporte Aplicacional");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Nome",
                value: "Quality Assurance");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "Nome",
                value: "Desenvolvimento Aplicacional");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "Nome",
                value: "Design de Serviços");

            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "Id", "Ativa", "DataCriacao", "Email", "NIF", "Nome", "Telefone" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "empresa@askblue.com", "123456789", "AskBlue", null });

            migrationBuilder.InsertData(
                table: "Utilizadores",
                columns: new[] { "Id", "Ativo", "Email", "EmpresaId", "Nome", "PasswordHash", "Role" },
                values: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), true, "admin@askblue.com", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Administrador", "YOUR_PRECOMPUTED_PASSWORD_HASH", "Administrador" });

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_EmpresaId",
                table: "Utilizadores",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_NIF",
                table: "Empresas",
                column: "NIF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Oportunidades_IdCategoria",
                table: "Oportunidades",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Oportunidades_IdCliente",
                table: "Oportunidades",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Oportunidades_IdResponsavel",
                table: "Oportunidades",
                column: "IdResponsavel");

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamentos_Oportunidades_IdOportunidade",
                table: "Orcamentos",
                column: "IdOportunidade",
                principalTable: "Oportunidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamentos_Utilizadores_IdCriador",
                table: "Orcamentos",
                column: "IdCriador",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizadores_Empresas_EmpresaId",
                table: "Utilizadores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orcamentos_Oportunidades_IdOportunidade",
                table: "Orcamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Orcamentos_Utilizadores_IdCriador",
                table: "Orcamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_Empresas_EmpresaId",
                table: "Utilizadores");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Oportunidades");

            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_EmpresaId",
                table: "Utilizadores");

            migrationBuilder.DeleteData(
                table: "Utilizadores",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "DataDecisao",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "DataEnvio",
                table: "Orcamentos");

            migrationBuilder.RenameColumn(
                name: "IdOportunidade",
                table: "Orcamentos",
                newName: "IdServico");

            migrationBuilder.RenameColumn(
                name: "IdCriador",
                table: "Orcamentos",
                newName: "IdPrestador");

            migrationBuilder.RenameIndex(
                name: "IX_Orcamentos_IdOportunidade",
                table: "Orcamentos",
                newName: "IX_Orcamentos_IdServico");

            migrationBuilder.RenameIndex(
                name: "IX_Orcamentos_IdCriador",
                table: "Orcamentos",
                newName: "IX_Orcamentos_IdPrestador");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Utilizadores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Utilizadores",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(254)",
                oldMaxLength: 254);

            migrationBuilder.AddColumn<string>(
                name: "NIF",
                table: "Utilizadores",
                type: "nchar(9)",
                fixedLength: true,
                maxLength: 9,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Utilizadores",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Orcamentos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCategoria = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCliente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataDesejada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PrazoPropostas = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicos_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Servicos_Utilizadores_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Nome",
                value: "Pintura e Remodelação");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Nome",
                value: "Eletricidade");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "Nome",
                value: "Canalização");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "Nome",
                value: "Carpintaria");

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nome" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), "Limpeza e Manutenção" });

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_NIF",
                table: "Utilizadores",
                column: "NIF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_IdCategoria",
                table: "Servicos",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_IdCliente",
                table: "Servicos",
                column: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamentos_Servicos_IdServico",
                table: "Orcamentos",
                column: "IdServico",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamentos_Utilizadores_IdPrestador",
                table: "Orcamentos",
                column: "IdPrestador",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
