using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolumKurulu.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class KararYazisiEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KararYazilari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Yil = table.Column<int>(type: "int", nullable: false),
                    Sayi = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Ilgi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Gundem = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    KararMetni = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KararYazilari", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KararYazilari_Yil_Sayi",
                table: "KararYazilari",
                columns: new[] { "Yil", "Sayi" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KararYazilari");
        }
    }
}
