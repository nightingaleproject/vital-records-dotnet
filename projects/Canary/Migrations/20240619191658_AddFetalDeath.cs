using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace canary.Migrations
{
    /// <inheritdoc />
    public partial class AddFetalDeath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FetalDeathRecords",
                columns: table => new
                {
                    RecordId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Xml = table.Column<string>(nullable: true),
                    Json = table.Column<string>(nullable: true),
                    Ije = table.Column<string>(nullable: true),
                    IjeInfo = table.Column<string>(nullable: true),
                    FhirInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FetalDeathRecords", x => x.RecordId);
                });

            migrationBuilder.CreateTable(
                name: "FetalDeathTests",
                columns: table => new
                {
                    TestId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created = table.Column<DateTime>(nullable: false),
                    CompletedDateTime = table.Column<DateTime>(nullable: false),
                    CompletedBool = table.Column<bool>(nullable: false),
                    Total = table.Column<int>(nullable: false),
                    Correct = table.Column<int>(nullable: false),
                    Incorrect = table.Column<int>(nullable: false),
                    ReferenceRecord = table.Column<string>(nullable: true),
                    TestRecord = table.Column<string>(nullable: true),
                    Results = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    TestMessageMessageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FetalDeathTests", x => x.TestId);
                });
        
            migrationBuilder.CreateIndex(
                name: "IX_FetalDeathTests_TestMessageMessageId",
                table: "FetalDeathTests",
                column: "TestMessageMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FetalDeathTest");
        }
    }
}
