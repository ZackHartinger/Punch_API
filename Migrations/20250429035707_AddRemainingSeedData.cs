using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Punch_API.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppUserCompany",
                columns: new[] { "AppUsersId", "CompaniesCompanyId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "EmployeeWorkDays",
                columns: new[] { "EmployeeWorkDayId", "CompanyId", "CreatedAt", "CustomerName", "Date", "EndTime", "IsNew", "LunchDuration", "LunchTime", "Mileage", "StartTime", "TruckName", "UserId" },
                values: new object[] { 1, 1, new DateTime(2025, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bourne", new DateOnly(2025, 4, 18), new TimeOnly(16, 30, 0), true, 30, new TimeOnly(12, 0, 0), null, new TimeOnly(8, 0, 0), null, 1 });

            migrationBuilder.InsertData(
                table: "WorkDayTasks",
                columns: new[] { "WorkDayTaskId", "EmployeeWorkDayId", "WorkTaskId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 1, 2 },
                    { 3, 1, 3 },
                    { 4, 1, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppUserCompany",
                keyColumns: new[] { "AppUsersId", "CompaniesCompanyId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "WorkDayTasks",
                keyColumn: "WorkDayTaskId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkDayTasks",
                keyColumn: "WorkDayTaskId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorkDayTasks",
                keyColumn: "WorkDayTaskId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorkDayTasks",
                keyColumn: "WorkDayTaskId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EmployeeWorkDays",
                keyColumn: "EmployeeWorkDayId",
                keyValue: 1);
        }
    }
}
