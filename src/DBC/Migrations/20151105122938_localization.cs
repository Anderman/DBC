using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace DBC.Migrations
{
    public partial class localization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Localizations",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Culture = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizations", x => new { x.Key, x.Culture });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Localizations");
        }
    }
}
