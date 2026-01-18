using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class OrcamentoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Orçamentos",
                table: "Orçamentos");

            migrationBuilder.RenameTable(
                name: "Orçamentos",
                newName: "Orcamentos");

            migrationBuilder.RenameColumn(
                name: "ValorEstimado",
                table: "Orcamentos",
                newName: "Veiculo");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Orcamentos",
                newName: "Total");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Orcamentos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Placa",
                table: "Orcamentos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orcamentos",
                table: "Orcamentos",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ItensOrcamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrcamentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Servico = table.Column<string>(type: "TEXT", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensOrcamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensOrcamento_Orcamentos_OrcamentoId",
                        column: x => x.OrcamentoId,
                        principalTable: "Orcamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orcamentos_ClienteId",
                table: "Orcamentos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamento_OrcamentoId",
                table: "ItensOrcamento",
                column: "OrcamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orcamentos_Clientes_ClienteId",
                table: "Orcamentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orcamentos_Clientes_ClienteId",
                table: "Orcamentos");

            migrationBuilder.DropTable(
                name: "ItensOrcamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orcamentos",
                table: "Orcamentos");

            migrationBuilder.DropIndex(
                name: "IX_Orcamentos_ClienteId",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "Placa",
                table: "Orcamentos");

            migrationBuilder.RenameTable(
                name: "Orcamentos",
                newName: "Orçamentos");

            migrationBuilder.RenameColumn(
                name: "Veiculo",
                table: "Orçamentos",
                newName: "ValorEstimado");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Orçamentos",
                newName: "Descricao");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orçamentos",
                table: "Orçamentos",
                column: "Id");
        }
    }
}
