using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Cibrary.Migrations
{
    public partial class AddImageLinkToBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageLink",
                table: "Book",
                isNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImageLink", table: "Book");
        }
    }
}
