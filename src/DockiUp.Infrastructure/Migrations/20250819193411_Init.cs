using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DockiUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectOrigin = table.Column<int>(type: "integer", nullable: false),
                    GitUrl = table.Column<string>(type: "text", nullable: true),
                    Compose = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: false),
                    ComposePath = table.Column<string>(type: "text", nullable: false),
                    ProjectUpdateMethod = table.Column<int>(type: "integer", nullable: false),
                    WebhookUrl = table.Column<string>(type: "text", nullable: true),
                    PeriodicIntervalInMinutes = table.Column<int>(type: "integer", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectInfo");
        }
    }
}
