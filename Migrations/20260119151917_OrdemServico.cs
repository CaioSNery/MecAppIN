using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class OrdemServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Clientes_ClienteID",
                table: "OrdemServicos");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "OrdemServicos",
                newName: "Veiculo");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteID",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "ClienteNome",
                table: "OrdemServicos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrcamentoId",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Placa",
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

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Clientes_ClienteID",
                table: "OrdemServicos",
                column: "ClienteID",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensOrcamento_OrdemServicos_OrdemServicosId",
                table: "ItensOrcamento");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdemServicos_Clientes_ClienteID",
                table: "OrdemServicos");

            migrationBuilder.DropIndex(
                name: "IX_ItensOrcamento_OrdemServicosId",
                table: "ItensOrcamento");

            migrationBuilder.DropColumn(
                name: "ClienteNome",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "OrcamentoId",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "Placa",
                table: "OrdemServicos");

            migrationBuilder.DropColumn(
                name: "OrdemServicosId",
                table: "ItensOrcamento");

            migrationBuilder.RenameColumn(
                name: "Veiculo",
                table: "OrdemServicos",
                newName: "Descricao");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteID",
                table: "OrdemServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdemServicos_Clientes_ClienteID",
                table: "OrdemServicos",
                column: "ClienteID",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
