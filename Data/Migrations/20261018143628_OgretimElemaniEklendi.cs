using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolumKurulu.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class OgretimElemaniEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OgretimElemanlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TcNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    SicilNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgretimElemanlari", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OgretimElemanlari_SicilNo",
                table: "OgretimElemanlari",
                column: "SicilNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OgretimElemanlari_TcNo",
                table: "OgretimElemanlari",
                column: "TcNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OgretimElemanlari");
        }
    }
}
