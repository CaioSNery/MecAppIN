using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class LancamentoFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensOrcamento_Orcamentos_OrcamentoId",
                table: "ItensOrcamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItensOrcamento",
                table: "ItensOrcamento");

            migrationBuilder.RenameTable(
                name: "ItensOrcamento",
                newName: "ItemOrcamento");

            migrationBuilder.RenameIndex(
                name: "IX_ItensOrcamento_OrcamentoId",
                table: "ItemOrcamento",
                newName: "IX_ItemOrcamento_OrcamentoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemOrcamento",
                table: "ItemOrcamento",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Lancamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Forma = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lancamentos", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrcamento_Orcamentos_OrcamentoId",
                table: "ItemOrcamento",
                column: "OrcamentoId",
                principalTable: "Orcamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrcamento_Orcamentos_OrcamentoId",
                table: "ItemOrcamento");

            migrationBuilder.DropTable(
                name: "Lancamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemOrcamento",
                table: "ItemOrcamento");

            migrationBuilder.RenameTable(
                name: "ItemOrcamento",
                newName: "ItensOrcamento");

            migrationBuilder.RenameIndex(
                name: "IX_ItemOrcamento_OrcamentoId",
                table: "ItensOrcamento",
                newName: "IX_ItensOrcamento_OrcamentoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItensOrcamento",
                table: "ItensOrcamento",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensOrcamento_Orcamentos_OrcamentoId",
                table: "ItensOrcamento",
                column: "OrcamentoId",
                principalTable: "Orcamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
