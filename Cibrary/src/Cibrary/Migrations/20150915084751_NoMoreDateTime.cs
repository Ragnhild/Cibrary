using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Cibrary.Migrations
{
    public partial class NoMoreDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("Year", "Book");
            migrationBuilder.AddColumn<int>("Year", "Book", defaultValue: 2015);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("Year", "Book");
            migrationBuilder.AddColumn<DateTime>("Year", "Book");

        }
    }
}
