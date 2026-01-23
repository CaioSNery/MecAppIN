using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class BuscarOS2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orcamentos_Clientes_ClienteId",
                table: "Orcamentos");

            migrationBuilder.DropIndex(
                name: "IX_Orcamentos_ClienteId",
                table: "Orcamentos");

            migrationBuilder.AddColumn<int>(
                name: "NumeroOs",
                table: "Orcamentos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Bloco",
                table: "ItensOrcamento",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPeca",
                table: "ItensOrcamento",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroOs",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "Bloco",
                table: "ItensOrcamento");

            migrationBuilder.DropColumn(
                name: "IsPeca",
                table: "ItensOrcamento");

            migrationBuilder.CreateIndex(
                name: "IX_Orcamentos_ClienteId",
                table: "Orcamentos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamentos_Clientes_ClienteId",
                table: "Orcamentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
