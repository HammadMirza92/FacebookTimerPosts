using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacebookTimerPosts.Migrations
{
    /// <inheritdoc />
    public partial class intit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FacebookPostId",
                table: "Posts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4068));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4075));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4079));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4331));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4335));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4339));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4344));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4348));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4351));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4354));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 57, 37, 928, DateTimeKind.Utc).AddTicks(4359));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FacebookPostId",
                table: "Posts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3067));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3075));

            migrationBuilder.UpdateData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3080));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3399));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3404));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3409));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3413));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3417));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3421));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3425));

            migrationBuilder.UpdateData(
                table: "Templates",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3431));
        }
    }
}
