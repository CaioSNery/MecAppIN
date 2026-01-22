using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class PecasBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicosId",
                table: "ItensOrdensServicos");

            migrationBuilder.AlterColumn<int>(
                name: "OrdemServicosId",
                table: "ItensOrdensServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPeca",
                table: "ItensOrdensServicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicosId",
                table: "ItensOrdensServicos",
                column: "OrdemServicosId",
                principalTable: "OrdemServicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicosId",
                table: "ItensOrdensServicos");

            migrationBuilder.DropColumn(
                name: "IsPeca",
                table: "ItensOrdensServicos");

            migrationBuilder.AlterColumn<int>(
                name: "OrdemServicosId",
                table: "ItensOrdensServicos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensOrdensServicos_OrdemServicos_OrdemServicosId",
                table: "ItensOrdensServicos",
                column: "OrdemServicosId",
                principalTable: "OrdemServicos",
                principalColumn: "Id");
        }
    }
}
