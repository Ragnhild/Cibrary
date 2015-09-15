using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Cibrary.Migrations
{
    public partial class AddedBookInformationLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InformationLink",
                table: "Book",
                isNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "InformationLink", table: "Book");
        }
    }
}
