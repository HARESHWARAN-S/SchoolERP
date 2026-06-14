using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentAttendanceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassStrength",
                table: "StudentClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StudentAttendances",
                columns: table => new
                {
                    AdmnNo = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Class = table.Column<string>(type: "text", nullable: false),
                    Sec = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttendances", x => new { x.AdmnNo, x.Date });
                    table.ForeignKey(
                        name: "FK_StudentAttendances_Students_AdmnNo",
                        column: x => x.AdmnNo,
                        principalTable: "Students",
                        principalColumn: "AdmnNo",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "ClassStrength",
                table: "StudentClasses");
        }
    }
}
