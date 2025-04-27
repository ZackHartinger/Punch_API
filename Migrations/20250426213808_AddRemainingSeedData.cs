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
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "CompanyId", "CompanyName" },
                values: new object[] { 1, "Outdoors By Design" });

            migrationBuilder.InsertData(
                table: "AppUserCompany",
                columns: new[] { "AppUsersId", "CompaniesCompanyId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "EmployeeWorkDays",
                columns: new[] { "EmployeeWorkDayId", "CompanyId", "CreatedAt", "CustomerName", "Date", "EndTime", "IsNew", "LunchDuration", "LunchTime", "Mileage", "StartTime", "TruckName", "UserId" },
                values: new object[] { 1, 1, new DateTime(2025, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bourne", new DateOnly(2025, 4, 18), new TimeOnly(16, 30, 0), false, 30, new TimeOnly(12, 0, 0), null, new TimeOnly(8, 0, 0), null, 1 });

            migrationBuilder.InsertData(
                table: "WorkTasks",
                columns: new[] { "WorkTaskId", "Category", "CompanyId", "Description", "IsDeprecated" },
                values: new object[,]
                {
                    { 1, "General Labor", 1, "Demoed landscape", false },
                    { 2, "General Labor", 1, "Installed bark", false },
                    { 3, "General Labor", 1, "Installed top soil", false },
                    { 4, "General Labor", 1, "Cleaned driveway/sidewalks", false },
                    { 5, "Hardscape", 1, "Prepped subbase", false },
                    { 6, "Hardscape", 1, "Set block/pavers/stone", false },
                    { 7, "Hardscape", 1, "Cut block/pavers/stone", false },
                    { 8, "Irrigation", 1, "Dug ditches", false },
                    { 9, "Irrigation", 1, "Plumbed lines", false },
                    { 10, "Irrigation", 1, "Adjusted heads", false }
                });

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
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "EmployeeWorkDays",
                keyColumn: "EmployeeWorkDayId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorkTasks",
                keyColumn: "WorkTaskId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "CompanyId",
                keyValue: 1);
        }
    }
}
