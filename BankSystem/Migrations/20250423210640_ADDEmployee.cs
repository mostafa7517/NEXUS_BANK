using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankSystem.Migrations
{
    /// <inheritdoc />
    public partial class ADDEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Department", "Email", "EmployeeID", "EmploymentStatus", "HireDate", "LastLogin", "Name", "Password", "PhoneNumber", "Position", "Role", "Username" },
                values: new object[] { 1, "IT", "admin@bankingsystem.com", "E0001", "Active", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "System Administrator", "Admin2347531$", "555-123-4567", "System Administrator", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
