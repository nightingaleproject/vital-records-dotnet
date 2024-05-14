using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace canary.Migrations
{
    /// <inheritdoc />
    public partial class AddBirth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "Records", newName: "DeathRecords");
            migrationBuilder.RenameTable(name: "Tests", newName: "DeathTests");

            migrationBuilder.CreateTable(
                name: "BirthRecords",
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
                    table.PrimaryKey("PK_BirthRecords", x => x.RecordId);
                });

            migrationBuilder.CreateTable(
                name: "BirthTests",
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
                    table.PrimaryKey("PK_BirthTests", x => x.TestId);
                });
        
            migrationBuilder.CreateIndex(
                name: "IX_BirthTests_TestMessageMessageId",
                table: "BirthTests",
                column: "TestMessageMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "DeathRecords", newName: "Records");
            migrationBuilder.RenameTable(name: "DeathTests", newName: "Tests");
            migrationBuilder.DropTable("BirthRecords");
            migrationBuilder.DropIndex("IX_BirthTests_TestMessageMessageId");
            migrationBuilder.DropTable("BirthTests");
        }
    }
}
