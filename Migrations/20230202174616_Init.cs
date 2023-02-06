using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbLogger.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContextId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Context = table.Column<string>(type: "TEXT", nullable: true),
                    Property = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PreviousValue = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CurrentValue = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChangedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    LogType = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    LogTypeBy = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Revision = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
