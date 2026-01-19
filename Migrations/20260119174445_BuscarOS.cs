using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class BuscarOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrdemServico_OrdemServicos_OrdemServicoId",
                table: "ItemOrdemServico");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Clientes_ClienteID",
                table: "OrdemServicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemOrdemServico",
                table: "ItemOrdemServico");

            migrationBuilder.RenameTable(
                name: "ItemOrdemServico",
                newName: "ItensOrdensServicos");

            migrationBuilder.RenameColumn(
                name: "ClienteID",
                table: "OrdemServicos",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdemServicos_ClienteID",
                table: "OrdemServicos",
                newName: "IX_OrdemServicos_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemOrdemServico_OrdemServicoId",
                table: "ItensOrdensServicos",
                newName: "IX_ItensOrdensServicos_OrdemServicoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItensOrdensServicos",
                table: "ItensOrdensServicos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicoId",
                table: "ItensOrdensServicos",
                column: "OrdemServicoId",
                principalTable: "OrdemServicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Clientes_ClienteId",
                table: "OrdemServicos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicoId",
                table: "ItensOrdensServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Clientes_ClienteId",
                table: "OrdemServicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItensOrdensServicos",
                table: "ItensOrdensServicos");

            migrationBuilder.RenameTable(
                name: "ItensOrdensServicos",
                newName: "ItemOrdemServico");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "OrdemServicos",
                newName: "ClienteID");

            migrationBuilder.RenameIndex(
                name: "IX_OrdemServicos_ClienteId",
                table: "OrdemServicos",
                newName: "IX_OrdemServicos_ClienteID");

            migrationBuilder.RenameIndex(
                name: "IX_ItensOrdensServicos_OrdemServicoId",
                table: "ItemOrdemServico",
                newName: "IX_ItemOrdemServico_OrdemServicoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemOrdemServico",
                table: "ItemOrdemServico",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrdemServico_OrdemServicos_OrdemServicoId",
                table: "ItemOrdemServico",
                column: "OrdemServicoId",
                principalTable: "OrdemServicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Clientes_ClienteID",
                table: "OrdemServicos",
                column: "ClienteID",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
