using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClienteEndereco",
                table: "Orcamentos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClienteTelefone",
                table: "Orcamentos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoMotor",
                table: "Orcamentos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClienteEndereco",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "ClienteTelefone",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "TipoMotor",
                table: "Orcamentos");
        }
    }
}
