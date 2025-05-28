using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankSystem.Migrations
{
    /// <inheritdoc />
    public partial class MakeTransactionFksNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_ReceiverAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId",
                table: "Transactions");

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "SenderAccountId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SSN",
                table: "Customers",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_ReceiverAccountId",
                table: "Transactions",
                column: "ReceiverAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId",
                table: "Transactions",
                column: "SenderAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_ReceiverAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "SenderAccountId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SSN",
                table: "Customers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14);

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Department", "Email", "EmployeeID", "EmploymentStatus", "HireDate", "LastLogin", "Name", "Password", "PhoneNumber", "Position", "Role", "Username" },
                values: new object[] { 1, "IT", "admin@bankingsystem.com", "E0001", "Active", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "System Administrator", "Admin2347531$", "555-123-4567", "System Administrator", "Admin", "admin" });

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_ReceiverAccountId",
                table: "Transactions",
                column: "ReceiverAccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_SenderAccountId",
                table: "Transactions",
                column: "SenderAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
