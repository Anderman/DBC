using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace DBC.Migrations
{
    public partial class a : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.DropIndex(name: "EmailIndex", table: "AspNetUsers");
            migration.DropIndex(name: "UserNameIndex", table: "AspNetUsers");
            migration.DropIndex(name: "RoleNameIndex", table: "AspNetRoles");
            migration.CreateTable(
                name: "Beroep",
                columns: table => new
                {
                    BeroepID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    beschrijving = table.Column(type: "nvarchar(max)", nullable: true),
                    branche_indicatie = table.Column(type: "int", nullable: false),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    element = table.Column(type: "nvarchar(max)", nullable: true),
                    groepcode = table.Column(type: "nvarchar(max)", nullable: true),
                    hierarchieniveau = table.Column(type: "int", nullable: false),
                    mutatie = table.Column(type: "int", nullable: true),
                    selecteerbaar = table.Column(type: "int", nullable: false),
                    sorteervolgorde = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beroep", x => x.BeroepID);
                });
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("Beroep");
            migration.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");
            migration.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName");
            migration.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
