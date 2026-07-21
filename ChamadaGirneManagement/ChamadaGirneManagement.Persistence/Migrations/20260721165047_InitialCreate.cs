using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChamadaGirneManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Uygulamalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UygulamaAdi = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UygulamaKodu = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Port = table.Column<int>(type: "int", nullable: false),
                    GecisYolu = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    IstemciAnahtarHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uygulamalar", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "YonetimKullanicilari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KullaniciAdi = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SifreHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdSoyad = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YonetimKullanicilari", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UygulamaGecisKodlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KodHash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    YonetimKullaniciId = table.Column<int>(type: "int", nullable: false),
                    UygulamaId = table.Column<int>(type: "int", nullable: false),
                    SonKullanmaTarihi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    KullanildiMi = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    YonetimDonusAdresi = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UygulamaGecisKodlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UygulamaGecisKodlari_Uygulamalar_UygulamaId",
                        column: x => x.UygulamaId,
                        principalTable: "Uygulamalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UygulamaGecisKodlari_YonetimKullanicilari_YonetimKullaniciId",
                        column: x => x.YonetimKullaniciId,
                        principalTable: "YonetimKullanicilari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "YonetimKullanicilari",
                columns: new[] { "Id", "AdSoyad", "KullaniciAdi", "SifreHash" },
                values: new object[] { 1, "Sistem Yöneticisi", "IT", "$2a$11$nB8kNi06IPQiG//LeYdNqe10O54oTT9NvQ5QASZ641yBgrbT9mZlq" });

            migrationBuilder.CreateIndex(
                name: "IX_UygulamaGecisKodlari_KodHash",
                table: "UygulamaGecisKodlari",
                column: "KodHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UygulamaGecisKodlari_UygulamaId",
                table: "UygulamaGecisKodlari",
                column: "UygulamaId");

            migrationBuilder.CreateIndex(
                name: "IX_UygulamaGecisKodlari_YonetimKullaniciId",
                table: "UygulamaGecisKodlari",
                column: "YonetimKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Uygulamalar_UygulamaKodu",
                table: "Uygulamalar",
                column: "UygulamaKodu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YonetimKullanicilari_KullaniciAdi",
                table: "YonetimKullanicilari",
                column: "KullaniciAdi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UygulamaGecisKodlari");

            migrationBuilder.DropTable(
                name: "Uygulamalar");

            migrationBuilder.DropTable(
                name: "YonetimKullanicilari");
        }
    }
}
