using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class Enums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicoId",
                table: "ItensOrdensServicos");

            migrationBuilder.DropIndex(
                name: "IX_ItensOrdensServicos_OrdemServicoId",
                table: "ItensOrdensServicos");

            migrationBuilder.RenameColumn(
                name: "OrdemServicoId",
                table: "ItensOrdensServicos",
                newName: "Bloco");

            migrationBuilder.AddColumn<int>(
                name: "OrdemServicosId",
                table: "ItensOrdensServicos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrdensServicos_OrdemServicosId",
                table: "ItensOrdensServicos",
                column: "OrdemServicosId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicosId",
                table: "ItensOrdensServicos",
                column: "OrdemServicosId",
                principalTable: "OrdemServicos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicosId",
                table: "ItensOrdensServicos");

            migrationBuilder.DropIndex(
                name: "IX_ItensOrdensServicos_OrdemServicosId",
                table: "ItensOrdensServicos");

            migrationBuilder.DropColumn(
                name: "OrdemServicosId",
                table: "ItensOrdensServicos");

            migrationBuilder.RenameColumn(
                name: "Bloco",
                table: "ItensOrdensServicos",
                newName: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrdensServicos_OrdemServicoId",
                table: "ItensOrdensServicos",
                column: "OrdemServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicoId",
                table: "ItensOrdensServicos",
                column: "OrdemServicoId",
                principalTable: "OrdemServicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
