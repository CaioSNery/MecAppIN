using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class ItemOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensOrcamento_OrdemServicos_OrdemServicosId",
                table: "ItensOrcamento");

            migrationBuilder.DropIndex(
                name: "IX_ItensOrcamento_OrdemServicosId",
                table: "ItensOrcamento");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "OrdemServicosId",
                table: "ItensOrcamento");

            migrationBuilder.AlterColumn<int>(
                name: "OrcamentoId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "OrdemServicos",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ItemOrdemServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrdemServicoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Servico = table.Column<string>(type: "TEXT", nullable: true),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOrdemServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOrdemServico_OrdemServicos_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdemServicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrdemServico_OrdemServicoId",
                table: "ItemOrdemServico",
                column: "OrdemServicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemOrdemServico");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "OrdemServicos");

            migrationBuilder.AlterColumn<int>(
                name: "OrcamentoId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Valor",
                table: "OrdemServicos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrdemServicosId",
                table: "ItensOrcamento",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamento_OrdemServicosId",
                table: "ItensOrcamento",
                column: "OrdemServicosId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensOrcamento_OrdemServicos_OrdemServicosId",
                table: "ItensOrcamento",
                column: "OrdemServicosId",
                principalTable: "OrdemServicos",
                principalColumn: "Id");
        }
    }
}
