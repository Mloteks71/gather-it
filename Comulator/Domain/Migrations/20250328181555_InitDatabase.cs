using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CityJobAd_Citys_CitiesId",
                table: "CityJobAd");

            migrationBuilder.DropForeignKey(
                name: "FK_Salarys_JobAds_JobAdId",
                table: "Salarys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Salarys",
                table: "Salarys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Citys",
                table: "Citys");

            migrationBuilder.RenameTable(
                name: "Salarys",
                newName: "Salaries");

            migrationBuilder.RenameTable(
                name: "Citys",
                newName: "Cities");

            migrationBuilder.RenameIndex(
                name: "IX_Salarys_JobAdId",
                table: "Salaries",
                newName: "IX_Salaries_JobAdId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Salaries",
                table: "Salaries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cities",
                table: "Cities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CityJobAd_Cities_CitiesId",
                table: "CityJobAd",
                column: "CitiesId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Salaries_JobAds_JobAdId",
                table: "Salaries",
                column: "JobAdId",
                principalTable: "JobAds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CityJobAd_Cities_CitiesId",
                table: "CityJobAd");

            migrationBuilder.DropForeignKey(
                name: "FK_Salaries_JobAds_JobAdId",
                table: "Salaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Salaries",
                table: "Salaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cities",
                table: "Cities");

            migrationBuilder.RenameTable(
                name: "Salaries",
                newName: "Salarys");

            migrationBuilder.RenameTable(
                name: "Cities",
                newName: "Citys");

            migrationBuilder.RenameIndex(
                name: "IX_Salaries_JobAdId",
                table: "Salarys",
                newName: "IX_Salarys_JobAdId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Salarys",
                table: "Salarys",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Citys",
                table: "Citys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CityJobAd_Citys_CitiesId",
                table: "CityJobAd",
                column: "CitiesId",
                principalTable: "Citys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Salarys_JobAds_JobAdId",
                table: "Salarys",
                column: "JobAdId",
                principalTable: "JobAds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
