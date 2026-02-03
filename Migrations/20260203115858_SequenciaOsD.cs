using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MecAppIN.Migrations
{
    /// <inheritdoc />
    public partial class SequenciaOsD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SequenciasOs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TipoMotor = table.Column<string>(type: "TEXT", nullable: true),
                    UltimoNumero = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenciasOs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SequenciasOs");
        }
    }
}
