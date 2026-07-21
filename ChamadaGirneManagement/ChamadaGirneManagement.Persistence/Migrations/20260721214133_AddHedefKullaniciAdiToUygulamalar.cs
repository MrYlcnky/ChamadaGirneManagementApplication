using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChamadaGirneManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHedefKullaniciAdiToUygulamalar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HedefKullaniciAdi",
                table: "Uygulamalar",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HedefKullaniciAdi",
                table: "Uygulamalar");
        }
    }
}
