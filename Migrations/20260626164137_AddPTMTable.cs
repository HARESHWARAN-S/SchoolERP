using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Migrations
{
    /// <inheritdoc />
    public partial class AddPTMTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PTMs",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AdmnNo = table.Column<string>(type: "text", nullable: false),
                    TeacherId = table.Column<string>(type: "text", nullable: false),
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTMs", x => new { x.Date, x.AdmnNo, x.TeacherId });
                    table.ForeignKey(
                        name: "FK_PTMs_StudentClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "StudentClasses",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PTMs_Students_AdmnNo",
                        column: x => x.AdmnNo,
                        principalTable: "Students",
                        principalColumn: "AdmnNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PTMs_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PTMs_AdmnNo",
                table: "PTMs",
                column: "AdmnNo");

            migrationBuilder.CreateIndex(
                name: "IX_PTMs_ClassId",
                table: "PTMs",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PTMs_TeacherId",
                table: "PTMs",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PTMs");
        }
    }
}
