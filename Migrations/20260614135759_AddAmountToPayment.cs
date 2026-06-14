using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchoolERP.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_StudentClasses_StudentClassClass_StudentClassSec",
                table: "Homeworks");

            migrationBuilder.DropIndex(
                name: "IX_Homeworks_StudentClassClass_StudentClassSec",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "StudentClassClass",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "StudentClassSec",
                table: "Homeworks");

            migrationBuilder.CreateTable(
                name: "Fees",
                columns: table => new
                {
                    FeeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AdmnNo = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fees", x => x.FeeId);
                    table.ForeignKey(
                        name: "FK_Fees_Students_AdmnNo",
                        column: x => x.AdmnNo,
                        principalTable: "Students",
                        principalColumn: "AdmnNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    InvoiceNo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FeeId = table.Column<int>(type: "integer", nullable: false),
                    AdmnNo = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    StripePaymentId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.InvoiceNo);
                    table.ForeignKey(
                        name: "FK_Payments_Fees_FeeId",
                        column: x => x.FeeId,
                        principalTable: "Fees",
                        principalColumn: "FeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Students_AdmnNo",
                        column: x => x.AdmnNo,
                        principalTable: "Students",
                        principalColumn: "AdmnNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Homeworks_Class_Sec",
                table: "Homeworks",
                columns: new[] { "Class", "Sec" });

            migrationBuilder.CreateIndex(
                name: "IX_Fees_AdmnNo",
                table: "Fees",
                column: "AdmnNo");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AdmnNo",
                table: "Payments",
                column: "AdmnNo");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_FeeId",
                table: "Payments",
                column: "FeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_StudentClasses_Class_Sec",
                table: "Homeworks",
                columns: new[] { "Class", "Sec" },
                principalTable: "StudentClasses",
                principalColumns: new[] { "Class", "Sec" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_StudentClasses_Class_Sec",
                table: "Homeworks");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Fees");

            migrationBuilder.DropIndex(
                name: "IX_Homeworks_Class_Sec",
                table: "Homeworks");

            migrationBuilder.AddColumn<string>(
                name: "StudentClassClass",
                table: "Homeworks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentClassSec",
                table: "Homeworks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Homeworks_StudentClassClass_StudentClassSec",
                table: "Homeworks",
                columns: new[] { "StudentClassClass", "StudentClassSec" });

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_StudentClasses_StudentClassClass_StudentClassSec",
                table: "Homeworks",
                columns: new[] { "StudentClassClass", "StudentClassSec" },
                principalTable: "StudentClasses",
                principalColumns: new[] { "Class", "Sec" });
        }
    }
}
