using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Citys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyNames_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Searchs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiteId = table.Column<int>(type: "integer", nullable: false),
                    Query = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Searchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Searchs_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobAds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RemoteType = table.Column<int>(type: "integer", nullable: false),
                    RemotePercent = table.Column<int>(type: "integer", nullable: true),
                    CompanyNameId = table.Column<int>(type: "integer", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobAds_CompanyNames_CompanyNameId",
                        column: x => x.CompanyNameId,
                        principalTable: "CompanyNames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CityJobAd",
                columns: table => new
                {
                    CitiesId = table.Column<int>(type: "integer", nullable: false),
                    JobAdsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityJobAd", x => new { x.CitiesId, x.JobAdsId });
                    table.ForeignKey(
                        name: "FK_CityJobAd_Citys_CitiesId",
                        column: x => x.CitiesId,
                        principalTable: "Citys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CityJobAd_JobAds_JobAdsId",
                        column: x => x.JobAdsId,
                        principalTable: "JobAds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Salarys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractType = table.Column<int>(type: "integer", nullable: false),
                    SalaryMin = table.Column<int>(type: "integer", nullable: false),
                    SalaryMax = table.Column<int>(type: "integer", nullable: false),
                    JobAdId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salarys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salarys_JobAds_JobAdId",
                        column: x => x.JobAdId,
                        principalTable: "JobAds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CityJobAd_JobAdsId",
                table: "CityJobAd",
                column: "JobAdsId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyNames_CompanyId",
                table: "CompanyNames",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobAds_CompanyNameId",
                table: "JobAds",
                column: "CompanyNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Salarys_JobAdId",
                table: "Salarys",
                column: "JobAdId");

            migrationBuilder.CreateIndex(
                name: "IX_Searchs_SiteId",
                table: "Searchs",
                column: "SiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CityJobAd");

            migrationBuilder.DropTable(
                name: "Salarys");

            migrationBuilder.DropTable(
                name: "Searchs");

            migrationBuilder.DropTable(
                name: "Citys");

            migrationBuilder.DropTable(
                name: "JobAds");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "CompanyNames");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
