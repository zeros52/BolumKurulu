using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolumKurulu.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class KurulVeUyeler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kurullar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KurulAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kurullar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KurulUyeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KurulId = table.Column<int>(type: "int", nullable: false),
                    OgretimElemaniId = table.Column<int>(type: "int", nullable: false),
                    Gorev = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KurulUyeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KurulUyeleri_Kurullar_KurulId",
                        column: x => x.KurulId,
                        principalTable: "Kurullar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KurulUyeleri_OgretimElemanlari_OgretimElemaniId",
                        column: x => x.OgretimElemaniId,
                        principalTable: "OgretimElemanlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KurulUyeleri_KurulId_OgretimElemaniId",
                table: "KurulUyeleri",
                columns: new[] { "KurulId", "OgretimElemaniId" });

            migrationBuilder.CreateIndex(
                name: "IX_KurulUyeleri_OgretimElemaniId",
                table: "KurulUyeleri",
                column: "OgretimElemaniId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KurulUyeleri");

            migrationBuilder.DropTable(
                name: "Kurullar");
        }
    }
}
