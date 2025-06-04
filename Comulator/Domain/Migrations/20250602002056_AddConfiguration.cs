using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyNames_Companies_CompanyId",
                table: "CompanyNames");

            migrationBuilder.DropForeignKey(
                name: "FK_JobAds_CompanyNames_CompanyNameId",
                table: "JobAds");

            migrationBuilder.DropIndex(
                name: "IX_JobAds_CompanyNameId",
                table: "JobAds");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "JobAds");

            migrationBuilder.AddColumn<int>(
                name: "JobAdId",
                table: "CompanyNames",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Descriptions",
                columns: table => new
                {
                    JobAdId = table.Column<int>(type: "integer", nullable: false),
                    DescriptionText = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    Workstyle = table.Column<string>(type: "text", nullable: true),
                    AboutProject = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Descriptions", x => x.JobAdId);
                    table.ForeignKey(
                        name: "FK_Descriptions_JobAds_JobAdId",
                        column: x => x.JobAdId,
                        principalTable: "JobAds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyNames_JobAdId",
                table: "CompanyNames",
                column: "JobAdId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyNames_Companies_CompanyId",
                table: "CompanyNames",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyNames_JobAds_JobAdId",
                table: "CompanyNames",
                column: "JobAdId",
                principalTable: "JobAds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyNames_Companies_CompanyId",
                table: "CompanyNames");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyNames_JobAds_JobAdId",
                table: "CompanyNames");

            migrationBuilder.DropTable(
                name: "Descriptions");

            migrationBuilder.DropIndex(
                name: "IX_CompanyNames_JobAdId",
                table: "CompanyNames");

            migrationBuilder.DropColumn(
                name: "JobAdId",
                table: "CompanyNames");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "JobAds",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_JobAds_CompanyNameId",
                table: "JobAds",
                column: "CompanyNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyNames_Companies_CompanyId",
                table: "CompanyNames",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobAds_CompanyNames_CompanyNameId",
                table: "JobAds",
                column: "CompanyNameId",
                principalTable: "CompanyNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
