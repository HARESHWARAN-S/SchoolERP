using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchoolERP.Migrations
{
    /// <inheritdoc />
    public partial class AddClassIdAndAcademicYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_StudentClasses_Class_Sec",
                table: "Homeworks");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_StudentClasses_Class_Sec",
                table: "Subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subjects",
                table: "Subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentClasses",
                table: "StudentClasses");

            migrationBuilder.DropIndex(
                name: "IX_StudentClasses_ClassTeacherId",
                table: "StudentClasses");

            migrationBuilder.DropIndex(
                name: "IX_Homeworks_Class_Sec",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Sec",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "Sec",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "Sec",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "Sec",
                table: "Homeworks");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "StudentClasses",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "AcademicYear",
                table: "StudentClasses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "StudentAttendances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "Notifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Marks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Homeworks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subjects",
                table: "Subjects",
                columns: new[] { "ClassId", "SubjectName" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentClasses",
                table: "StudentClasses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_Class_Sec_AcademicYear",
                table: "StudentClasses",
                columns: new[] { "Class", "Sec", "AcademicYear" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_ClassTeacherId_AcademicYear",
                table: "StudentClasses",
                columns: new[] { "ClassTeacherId", "AcademicYear" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_ClassId",
                table: "StudentAttendances",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_ClassId",
                table: "Marks",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Homeworks_ClassId",
                table: "Homeworks",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_StudentClasses_ClassId",
                table: "Homeworks",
                column: "ClassId",
                principalTable: "StudentClasses",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_StudentClasses_ClassId",
                table: "Marks",
                column: "ClassId",
                principalTable: "StudentClasses",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendances_StudentClasses_ClassId",
                table: "StudentAttendances",
                column: "ClassId",
                principalTable: "StudentClasses",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_StudentClasses_ClassId",
                table: "Subjects",
                column: "ClassId",
                principalTable: "StudentClasses",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_StudentClasses_ClassId",
                table: "Homeworks");

            migrationBuilder.DropForeignKey(
                name: "FK_Marks_StudentClasses_ClassId",
                table: "Marks");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendances_StudentClasses_ClassId",
                table: "StudentAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_StudentClasses_ClassId",
                table: "Subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subjects",
                table: "Subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentClasses",
                table: "StudentClasses");

            migrationBuilder.DropIndex(
                name: "IX_StudentClasses_Class_Sec_AcademicYear",
                table: "StudentClasses");

            migrationBuilder.DropIndex(
                name: "IX_StudentClasses_ClassTeacherId_AcademicYear",
                table: "StudentClasses");

            migrationBuilder.DropIndex(
                name: "IX_StudentAttendances_ClassId",
                table: "StudentAttendances");

            migrationBuilder.DropIndex(
                name: "IX_Marks_ClassId",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Homeworks_ClassId",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "AcademicYear",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Homeworks");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "Subjects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sec",
                table: "Subjects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "StudentAttendances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sec",
                table: "StudentAttendances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Target",
                table: "Notifications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "Marks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sec",
                table: "Marks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "Homeworks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sec",
                table: "Homeworks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subjects",
                table: "Subjects",
                columns: new[] { "Class", "Sec", "SubjectName" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentClasses",
                table: "StudentClasses",
                columns: new[] { "Class", "Sec" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_ClassTeacherId",
                table: "StudentClasses",
                column: "ClassTeacherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Homeworks_Class_Sec",
                table: "Homeworks",
                columns: new[] { "Class", "Sec" });

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_StudentClasses_Class_Sec",
                table: "Homeworks",
                columns: new[] { "Class", "Sec" },
                principalTable: "StudentClasses",
                principalColumns: new[] { "Class", "Sec" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_StudentClasses_Class_Sec",
                table: "Subjects",
                columns: new[] { "Class", "Sec" },
                principalTable: "StudentClasses",
                principalColumns: new[] { "Class", "Sec" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
