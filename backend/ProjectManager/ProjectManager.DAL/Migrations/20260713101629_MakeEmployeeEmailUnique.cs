using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmployeeEmailUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDocument_Projects_ProjectId",
                table: "ProjectDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDocument",
                table: "ProjectDocument");

            migrationBuilder.RenameTable(
                name: "ProjectDocument",
                newName: "ProjectDocuments");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectDocument_ProjectId",
                table: "ProjectDocuments",
                newName: "IX_ProjectDocuments_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDocuments",
                table: "ProjectDocuments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDocuments_Projects_ProjectId",
                table: "ProjectDocuments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDocuments_Projects_ProjectId",
                table: "ProjectDocuments");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectDocuments",
                table: "ProjectDocuments");

            migrationBuilder.RenameTable(
                name: "ProjectDocuments",
                newName: "ProjectDocument");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectDocuments_ProjectId",
                table: "ProjectDocument",
                newName: "IX_ProjectDocument_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectDocument",
                table: "ProjectDocument",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDocument_Projects_ProjectId",
                table: "ProjectDocument",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
