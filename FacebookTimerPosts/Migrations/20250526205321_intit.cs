using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FacebookTimerPosts.Migrations
{
    /// <inheritdoc />
    public partial class intit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacebookPostResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookPostResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    MaxPosts = table.Column<int>(type: "int", nullable: false),
                    MaxTemplates = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacebookPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PageAccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookPages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultFontFamily = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiresSubscription = table.Column<bool>(type: "bit", nullable: false),
                    MinimumSubscriptionPlanId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HtmlTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Templates_SubscriptionPlans_MinimumSubscriptionPlanId",
                        column: x => x.MinimumSubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubscriptionPlanId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    PaymentReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FacebookPageId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EventDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomFontFamily = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomPrimaryColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ShowDays = table.Column<bool>(type: "bit", nullable: false),
                    ShowHours = table.Column<bool>(type: "bit", nullable: false),
                    ShowMinutes = table.Column<bool>(type: "bit", nullable: false),
                    ShowSeconds = table.Column<bool>(type: "bit", nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasOverlay = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FacebookPostId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledFor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshIntervalInMinutes = table.Column<int>(type: "int", nullable: true),
                    NextRefreshTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_FacebookPages_FacebookPageId",
                        column: x => x.FacebookPageId,
                        principalTable: "FacebookPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserSubscriptionId = table.Column<int>(type: "int", nullable: true),
                    PaymentProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentResults_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentResults_UserSubscriptions_UserSubscriptionId",
                        column: x => x.UserSubscriptionId,
                        principalTable: "UserSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CountdownTimers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountdownTimers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CountdownTimers_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "CreatedAt", "Description", "DurationInDays", "IsActive", "MaxPosts", "MaxTemplates", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3067), "Free plan with basic features", 0, true, 1, 7, "Free", 0m, null },
                    { 2, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3075), "Professional plan with advanced features", 30, true, 10, 20, "Pro", 20m, null },
                    { 3, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3080), "Premium plan with all features", 30, true, 20, 0, "Premium", 50m, null }
                });

            migrationBuilder.InsertData(
                table: "Templates",
                columns: new[] { "Id", "BackgroundImageUrl", "CreatedAt", "DefaultFontFamily", "Description", "HtmlTemplate", "IsActive", "MinimumSubscriptionPlanId", "Name", "PrimaryColor", "RequiresSubscription", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3399), "Arial, sans-serif", "A simple dark-themed countdown timer", "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Countdown Timer Post</title>\r\n  <style>\r\n    .countdown-container {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      justify-content: center;\r\n      height: 100%;\r\n      width: 100%;\r\n      padding: 20px;\r\n      box-sizing: border-box;\r\n      font-family: Arial, sans-serif;\r\n      background-color: #1a1a1a;\r\n      color: white;\r\n      text-align: center;\r\n    }\r\n    \r\n    .event-title {\r\n      font-size: 32px;\r\n      font-weight: bold;\r\n      margin-bottom: 30px;\r\n      text-transform: uppercase;\r\n    }\r\n    \r\n    .countdown-timer {\r\n      display: flex;\r\n      align-items: center;\r\n      justify-content: center;\r\n      margin-bottom: 30px;\r\n    }\r\n    \r\n    .time-block {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      margin: 0 5px;\r\n    }\r\n    \r\n    .time-value {\r\n      font-size: 48px;\r\n      font-weight: bold;\r\n      background-color: #333;\r\n      border-radius: 8px;\r\n      padding: 10px;\r\n      min-width: 80px;\r\n    }\r\n    \r\n    .time-label {\r\n      margin-top: 10px;\r\n      font-size: 14px;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n    }\r\n    \r\n    .time-separator {\r\n      font-size: 48px;\r\n      margin: 0 5px;\r\n      padding-bottom: 25px;\r\n    }\r\n    \r\n    .event-details {\r\n      max-width: 500px;\r\n    }\r\n    \r\n    .event-description {\r\n      margin-bottom: 20px;\r\n      font-size: 16px;\r\n      line-height: 1.5;\r\n    }\r\n    \r\n    .event-button {\r\n      display: inline-block;\r\n      padding: 12px 30px;\r\n      background-color: #ff4500;\r\n      color: white;\r\n      text-decoration: none;\r\n      border-radius: 30px;\r\n      font-weight: bold;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n      transition: all 0.3s ease;\r\n    }\r\n    \r\n    .event-button:hover {\r\n      background-color: #ff6a33;\r\n      transform: translateY(-2px);\r\n      box-shadow: 0 5px 15px rgba(255, 69, 0, 0.4);\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n<div class=\"countdown-container\">\r\n  <h1 class=\"event-title\">{{eventName}}</h1>\r\n  \r\n  <div class=\"countdown-timer\">\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{days}}</div>\r\n      <div class=\"time-label\">Days</div>\r\n    </div>\r\n    <div class=\"time-separator\">:</div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{hours}}</div>\r\n      <div class=\"time-label\">Hours</div>\r\n    </div>\r\n    <div class=\"time-separator\">:</div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{minutes}}</div>\r\n      <div class=\"time-label\">Minutes</div>\r\n    </div>\r\n    <div class=\"time-separator\">:</div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{seconds}}</div>\r\n      <div class=\"time-label\">Seconds</div>\r\n    </div>\r\n  </div>\r\n  \r\n  <div class=\"event-details\">\r\n    <p class=\"event-description\">{{eventDescription}}</p>\r\n    <a href=\"{{eventLink}}\" class=\"event-button\">{{buttonText}}</a>\r\n  </div>\r\n</div>\r\n</body>\r\n</html>", true, null, "Classic Dark", "#1a1a1a", false, null },
                    { 2, null, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3404), "Roboto, sans-serif", "A clean light-themed countdown timer", "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Simple Light Countdown</title>\r\n  <style>\r\n    .countdown-container {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      justify-content: center;\r\n      height: 100%;\r\n      width: 100%;\r\n      padding: 20px;\r\n      box-sizing: border-box;\r\n      font-family: Roboto, sans-serif;\r\n      background-color: #f5f5f5;\r\n      color: #333;\r\n      text-align: center;\r\n    }\r\n    \r\n    .event-title {\r\n      font-size: 28px;\r\n      font-weight: 500;\r\n      margin-bottom: 25px;\r\n    }\r\n    \r\n    .countdown-timer {\r\n      display: flex;\r\n      align-items: center;\r\n      justify-content: center;\r\n      margin-bottom: 25px;\r\n    }\r\n    \r\n    .time-block {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      margin: 0 10px;\r\n    }\r\n    \r\n    .time-value {\r\n      font-size: 40px;\r\n      font-weight: bold;\r\n      color: #333;\r\n      background-color: #fff;\r\n      border: 1px solid #ddd;\r\n      border-radius: 4px;\r\n      padding: 10px;\r\n      min-width: 60px;\r\n      box-shadow: 0 2px 5px rgba(0,0,0,0.1);\r\n    }\r\n    \r\n    .time-label {\r\n      margin-top: 8px;\r\n      font-size: 12px;\r\n      color: #666;\r\n      text-transform: uppercase;\r\n    }\r\n    \r\n    .event-details {\r\n      max-width: 500px;\r\n    }\r\n    \r\n    .event-description {\r\n      margin-bottom: 20px;\r\n      font-size: 16px;\r\n      line-height: 1.5;\r\n      color: #555;\r\n    }\r\n    \r\n    .event-button {\r\n      display: inline-block;\r\n      padding: 10px 25px;\r\n      background-color: #2196F3;\r\n      color: white;\r\n      text-decoration: none;\r\n      border-radius: 4px;\r\n      font-weight: 500;\r\n      transition: all 0.2s ease;\r\n    }\r\n    \r\n    .event-button:hover {\r\n      background-color: #1976D2;\r\n      box-shadow: 0 2px 8px rgba(33, 150, 243, 0.4);\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n<div class=\"countdown-container\">\r\n  <h1 class=\"event-title\">{{eventName}}</h1>\r\n  \r\n  <div class=\"countdown-timer\">\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{days}}</div>\r\n      <div class=\"time-label\">Days</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{hours}}</div>\r\n      <div class=\"time-label\">Hours</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{minutes}}</div>\r\n      <div class=\"time-label\">Minutes</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{seconds}}</div>\r\n      <div class=\"time-label\">Seconds</div>\r\n    </div>\r\n  </div>\r\n  \r\n  <div class=\"event-details\">\r\n    <p class=\"event-description\">{{eventDescription}}</p>\r\n    <a href=\"{{eventLink}}\" class=\"event-button\">{{buttonText}}</a>\r\n  </div>\r\n</div>\r\n</body>\r\n</html>", true, null, "Simple Light", "#f5f5f5", false, null },
                    { 3, null, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3409), "Montserrat, sans-serif", "A vibrant countdown with eye-catching colors", "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Get Ready Countdown</title>\r\n  <style>\r\n    .countdown-container {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      justify-content: center;\r\n      height: 100%;\r\n      width: 100%;\r\n      padding: 20px;\r\n      box-sizing: border-box;\r\n      font-family: Montserrat, sans-serif;\r\n      background: linear-gradient(135deg, #e91e63 0%, #5d02ff 100%);\r\n      color: white;\r\n      text-align: center;\r\n    }\r\n    \r\n    .event-header {\r\n      margin-bottom: 20px;\r\n      background-color: rgba(255, 255, 255, 0.15);\r\n      padding: 10px 20px;\r\n      border-radius: 30px;\r\n      font-weight: 700;\r\n      text-transform: uppercase;\r\n      letter-spacing: 2px;\r\n      font-size: 16px;\r\n    }\r\n    \r\n    .hours-block {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      margin: 0 auto 20px;\r\n      background-color: white;\r\n      color: #333;\r\n      height: 150px;\r\n      width: 150px;\r\n      border-radius: 50%;\r\n      display: flex;\r\n      justify-content: center;\r\n      align-items: center;\r\n      box-shadow: 0 10px 20px rgba(0,0,0,0.2);\r\n    }\r\n    \r\n    .hours-value {\r\n      font-size: 50px;\r\n      font-weight: 700;\r\n      line-height: 1;\r\n    }\r\n    \r\n    .hours-label {\r\n      font-size: 16px;\r\n      text-transform: uppercase;\r\n      font-weight: 500;\r\n      color: #e91e63;\r\n    }\r\n    \r\n    .event-title {\r\n      font-size: 28px;\r\n      font-weight: 700;\r\n      margin: 20px 0;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n    }\r\n    \r\n    .event-description {\r\n      margin-bottom: 30px;\r\n      font-size: 16px;\r\n      line-height: 1.6;\r\n      max-width: 600px;\r\n    }\r\n    \r\n    .event-button {\r\n      display: inline-block;\r\n      padding: 12px 30px;\r\n      background-color: white;\r\n      color: #e91e63;\r\n      text-decoration: none;\r\n      border-radius: 30px;\r\n      font-weight: 700;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n      transition: all 0.3s ease;\r\n      box-shadow: 0 5px 15px rgba(0,0,0,0.2);\r\n    }\r\n    \r\n    .event-button:hover {\r\n      transform: translateY(-3px);\r\n      box-shadow: 0 8px 20px rgba(0,0,0,0.3);\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n<div class=\"countdown-container\">\r\n  <div class=\"event-header\">GET READY!</div>\r\n  \r\n  <div class=\"hours-block\">\r\n    <div class=\"hours-value\">{{hours}}</div>\r\n    <div class=\"hours-label\">Hours</div>\r\n  </div>\r\n  \r\n  <h1 class=\"event-title\">{{eventName}}</h1>\r\n  \r\n  <p class=\"event-description\">{{eventDescription}}</p>\r\n  \r\n  <a href=\"{{eventLink}}\" class=\"event-button\">{{buttonText}}</a>\r\n</div>\r\n</body>\r\n</html>", true, null, "Get Ready", "#e91e63", false, null },
                    { 4, null, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3413), "Helvetica, sans-serif", "A minimalist countdown timer design", "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Minimal Countdown</title>\r\n  <style>\r\n    .countdown-container {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      justify-content: center;\r\n      height: 100%;\r\n      width: 100%;\r\n      padding: 20px;\r\n      box-sizing: border-box;\r\n      font-family: Helvetica, sans-serif;\r\n      background-color: #ffffff;\r\n      color: #333333;\r\n      text-align: center;\r\n    }\r\n    \r\n    .event-title {\r\n      font-size: 24px;\r\n      font-weight: 400;\r\n      margin-bottom: 40px;\r\n    }\r\n    \r\n    .countdown-timer {\r\n      display: flex;\r\n      align-items: baseline;\r\n      justify-content: center;\r\n      margin-bottom: 40px;\r\n      font-size: 60px;\r\n      font-weight: 300;\r\n      letter-spacing: -2px;\r\n    }\r\n    \r\n    .time-separator {\r\n      margin: 0 5px;\r\n      color: #cccccc;\r\n      font-weight: 300;\r\n    }\r\n    \r\n    .event-button {\r\n      display: inline-block;\r\n      padding: 12px 30px;\r\n      background-color: #333333;\r\n      color: white;\r\n      text-decoration: none;\r\n      border-radius: 2px;\r\n      font-size: 14px;\r\n      transition: all 0.2s ease;\r\n    }\r\n    \r\n    .event-button:hover {\r\n      background-color: #000000;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n<div class=\"countdown-container\">\r\n  <h1 class=\"event-title\">{{eventName}}</h1>\r\n  \r\n  <div class=\"countdown-timer\">\r\n    {{days}}\r\n    <span class=\"time-separator\">:</span>\r\n    {{hours}}\r\n    <span class=\"time-separator\">:</span>\r\n    {{minutes}}\r\n    <span class=\"time-separator\">:</span>\r\n    {{seconds}}\r\n  </div>\r\n  \r\n  <a href=\"{{eventLink}}\" class=\"event-button\">{{buttonText}}</a>\r\n</div>\r\n</body>\r\n</html>", true, null, "Minimal", "#333333", false, null },
                    { 5, null, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3417), "Poppins, sans-serif", "A professional design for promotional events", "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Special Promo Countdown</title>\r\n  <style>\r\n    .countdown-container {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      justify-content: center;\r\n      height: 100%;\r\n      width: 100%;\r\n      padding: 30px;\r\n      box-sizing: border-box;\r\n      font-family: Poppins, sans-serif;\r\n      background: linear-gradient(135deg, #8e44ad 0%, #3498db 100%);\r\n      color: white;\r\n      text-align: center;\r\n      border-radius: 10px;\r\n      overflow: hidden;\r\n      position: relative;\r\n    }\r\n    \r\n    .promo-label {\r\n      position: absolute;\r\n      top: 20px;\r\n      right: -35px;\r\n      background-color: #f39c12;\r\n      color: white;\r\n      padding: 5px 40px;\r\n      font-size: 12px;\r\n      transform: rotate(45deg);\r\n      font-weight: 600;\r\n      box-shadow: 0 2px 5px rgba(0,0,0,0.2);\r\n      z-index: 10;\r\n    }\r\n    \r\n    .event-title {\r\n      font-size: 26px;\r\n      font-weight: 600;\r\n      margin-bottom: 10px;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n    }\r\n    \r\n    .event-subtitle {\r\n      font-size: 14px;\r\n      opacity: 0.8;\r\n      margin-bottom: 30px;\r\n    }\r\n    \r\n    .countdown-timer {\r\n      display: flex;\r\n      align-items: center;\r\n      justify-content: center;\r\n      margin-bottom: 30px;\r\n      width: 100%;\r\n      max-width: 500px;\r\n    }\r\n    \r\n    .time-block {\r\n      flex: 1;\r\n      margin: 0 5px;\r\n      background-color: rgba(255, 255, 255, 0.1);\r\n      border-radius: 8px;\r\n      padding: 15px 5px;\r\n    }\r\n    \r\n    .time-value {\r\n      font-size: 36px;\r\n      font-weight: 700;\r\n      margin-bottom: 5px;\r\n    }\r\n    \r\n    .time-label {\r\n      font-size: 11px;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n      opacity: 0.8;\r\n    }\r\n    \r\n    .event-details {\r\n      background-color: rgba(255, 255, 255, 0.1);\r\n      padding: 20px;\r\n      border-radius: 8px;\r\n      max-width: 500px;\r\n      margin-bottom: 20px;\r\n    }\r\n    \r\n    .event-description {\r\n      margin-bottom: 20px;\r\n      font-size: 14px;\r\n      line-height: 1.6;\r\n    }\r\n    \r\n    .promo-code {\r\n      background-color: white;\r\n      color: #8e44ad;\r\n      padding: 10px 20px;\r\n      font-weight: 600;\r\n      border-radius: 5px;\r\n      margin-bottom: 20px;\r\n      letter-spacing: 2px;\r\n      display: inline-block;\r\n    }\r\n    \r\n    .event-button {\r\n      display: inline-block;\r\n      padding: 12px 30px;\r\n      background-color: #f39c12;\r\n      color: white;\r\n      text-decoration: none;\r\n      border-radius: 30px;\r\n      font-weight: 600;\r\n      transition: all 0.3s ease;\r\n      box-shadow: 0 4px 10px rgba(0,0,0,0.2);\r\n    }\r\n    \r\n    .event-button:hover {\r\n      background-color: #e67e22;\r\n      transform: translateY(-2px);\r\n      box-shadow: 0 6px 15px rgba(0,0,0,0.3);\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n<div class=\"countdown-container\">\r\n  <div class=\"promo-label\">SPECIAL PROMO</div>\r\n  \r\n  <h1 class=\"event-title\">{{eventName}}</h1>\r\n  <div class=\"event-subtitle\">Limited time offer - Don't miss out!</div>\r\n  \r\n  <div class=\"countdown-timer\">\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{days}}</div>\r\n      <div class=\"time-label\">Days</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{hours}}</div>\r\n      <div class=\"time-label\">Hours</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{minutes}}</div>\r\n      <div class=\"time-label\">Minutes</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{seconds}}</div>\r\n      <div class=\"time-label\">Seconds</div>\r\n    </div>\r\n  </div>\r\n  \r\n  <div class=\"event-details\">\r\n    <p class=\"event-description\">{{eventDescription}}</p>\r\n    <div class=\"promo-code\">PROMO25</div>\r\n  </div>\r\n  \r\n  <a href=\"{{eventLink}}\" class=\"event-button\">{{buttonText}}</a>\r\n</div>\r\n</body>\r\n</html>", true, 2, "Special Promo", "#8e44ad", true, null },
                    { 6, null, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3421), "Lato, sans-serif", "A professional teal-themed countdown for business events", "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Teal Business Countdown</title>\r\n  <style>\r\n    .countdown-container {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      justify-content: center;\r\n      height: 100%;\r\n      width: 100%;\r\n      padding: 30px;\r\n      box-sizing: border-box;\r\n      font-family: Lato, sans-serif;\r\n      background-color: #009688;\r\n      color: white;\r\n      text-align: center;\r\n    }\r\n    \r\n    .event-title {\r\n      font-size: 28px;\r\n      font-weight: 300;\r\n      margin-bottom: 5px;\r\n      text-transform: uppercase;\r\n      letter-spacing: 2px;\r\n    }\r\n    \r\n    .event-subtitle {\r\n      font-size: 16px;\r\n      font-weight: 300;\r\n      margin-bottom: 30px;\r\n      opacity: 0.9;\r\n    }\r\n    \r\n    .countdown-timer {\r\n      display: flex;\r\n      align-items: center;\r\n      justify-content: center;\r\n      margin-bottom: 30px;\r\n    }\r\n    \r\n    .time-block {\r\n      position: relative;\r\n      min-width: 80px;\r\n      margin: 0 10px;\r\n    }\r\n    \r\n    .time-value {\r\n      font-size: 54px;\r\n      font-weight: 300;\r\n      display: block;\r\n      line-height: 1;\r\n    }\r\n    \r\n    .time-label {\r\n      position: absolute;\r\n      bottom: -20px;\r\n      left: 0;\r\n      right: 0;\r\n      text-align: center;\r\n      font-size: 11px;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n      opacity: 0.8;\r\n    }\r\n    \r\n    .countdown-footer {\r\n      width: 100%;\r\n      max-width: 600px;\r\n      margin-top: 40px;\r\n      padding-top: 20px;\r\n      border-top: 1px solid rgba(255, 255, 255, 0.3);\r\n    }\r\n    \r\n    .event-description {\r\n      margin-bottom: 20px;\r\n      font-size: 16px;\r\n      line-height: 1.6;\r\n      font-weight: 300;\r\n    }\r\n    \r\n    .event-button {\r\n      display: inline-block;\r\n      padding: 12px 30px;\r\n      background-color: white;\r\n      color: #009688;\r\n      text-decoration: none;\r\n      border-radius: 3px;\r\n      font-weight: 700;\r\n      transition: all 0.3s ease;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n      font-size: 12px;\r\n    }\r\n    \r\n    .event-button:hover {\r\n      background-color: rgba(255, 255, 255, 0.9);\r\n      transform: translateY(-2px);\r\n      box-shadow: 0 4px 10px rgba(0,0,0,0.1);\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n<div class=\"countdown-container\">\r\n  <h1 class=\"event-title\">{{eventName}}</h1>\r\n  <div class=\"event-subtitle\">Join us for this exclusive event</div>\r\n  \r\n  <div class=\"countdown-timer\">\r\n    <div class=\"time-block\">\r\n      <span class=\"time-value\">{{days}}</span>\r\n      <span class=\"time-label\">Days</span>\r\n    </div>\r\n    \r\n    <div class=\"time-block\">\r\n      <span class=\"time-value\">{{hours}}</span>\r\n      <span class=\"time-label\">Hours</span>\r\n    </div>\r\n    \r\n    <div class=\"time-block\">\r\n      <span class=\"time-value\">{{minutes}}</span>\r\n      <span class=\"time-label\">Minutes</span>\r\n    </div>\r\n    \r\n    <div class=\"time-block\">\r\n      <span class=\"time-value\">{{seconds}}</span>\r\n      <span class=\"time-label\">Seconds</span>\r\n    </div>\r\n  </div>\r\n  \r\n  <div class=\"countdown-footer\">\r\n    <p class=\"event-description\">{{eventDescription}}</p>\r\n    <a href=\"{{eventLink}}\" class=\"event-button\">{{buttonText}}</a>\r\n  </div>\r\n</div>\r\n</body>\r\n</html>", true, 2, "Teal Business", "#009688", true, null },
                    { 7, null, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3425), "Oswald, sans-serif", "A high-impact sales countdown for Black Friday events", "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Black Friday Countdown</title>\r\n  <style>\r\n    @keyframes pulse {\r\n      0% { transform: scale(1); }\r\n      50% { transform: scale(1.05); }\r\n      100% { transform: scale(1); }\r\n    }\r\n    \r\n    .countdown-container {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      justify-content: center;\r\n      height: 100%;\r\n      width: 100%;\r\n      padding: 20px;\r\n      box-sizing: border-box;\r\n      font-family: Oswald, sans-serif;\r\n      background-color: #000000;\r\n      color: #ffffff;\r\n      text-align: center;\r\n      background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSI1IiBoZWlnaHQ9IjUiPgo8cmVjdCB3aWR0aD0iNSIgaGVpZ2h0PSI1IiBmaWxsPSIjMDAwIj48L3JlY3Q+CjxwYXRoIGQ9Ik0wIDVMNSAwWk02IDRMNCA2Wk0tMSAxTDEgLTFaIiBzdHJva2U9IiMyMjIiIHN0cm9rZS13aWR0aD0iMSI+PC9wYXRoPgo8L3N2Zz4=');\r\n    }\r\n    \r\n    .bf-header {\r\n      margin-bottom: 20px;\r\n    }\r\n    \r\n    .bf-title {\r\n      font-size: 40px;\r\n      font-weight: 700;\r\n      text-transform: uppercase;\r\n      line-height: 1;\r\n      margin: 0;\r\n      text-shadow: 0 0 10px rgba(255, 255, 255, 0.3);\r\n    }\r\n    \r\n    .bf-title span {\r\n      color: #aaff00;\r\n      display: block;\r\n      font-size: 60px;\r\n      animation: pulse 2s infinite;\r\n    }\r\n    \r\n    .countdown-timer {\r\n      display: flex;\r\n      align-items: center;\r\n      justify-content: center;\r\n      margin: 30px 0;\r\n    }\r\n    \r\n    .time-block {\r\n      background-color: #1a1a1a;\r\n      border: 2px solid #aaff00;\r\n      padding: 15px 5px;\r\n      margin: 0 5px;\r\n      min-width: 80px;\r\n      box-shadow: 0 0 15px rgba(170, 255, 0, 0.3);\r\n    }\r\n    \r\n    .time-value {\r\n      font-size: 50px;\r\n      font-weight: 700;\r\n      color: #aaff00;\r\n      line-height: 1;\r\n    }\r\n    \r\n    .time-label {\r\n      margin-top: 10px;\r\n      font-size: 14px;\r\n      text-transform: uppercase;\r\n      color: #ffffff;\r\n    }\r\n    \r\n    .days-remaining {\r\n      font-size: 24px;\r\n      text-transform: uppercase;\r\n      margin-bottom: 30px;\r\n    }\r\n    \r\n    .days-remaining strong {\r\n      color: #aaff00;\r\n    }\r\n    \r\n    .discount-badge {\r\n      background-color: #aaff00;\r\n      color: #000000;\r\n      font-size: 28px;\r\n      font-weight: 700;\r\n      padding: 10px 20px;\r\n      border-radius: 5px;\r\n      margin-bottom: 20px;\r\n      display: inline-block;\r\n      text-transform: uppercase;\r\n    }\r\n    \r\n    .event-description {\r\n      font-size: 16px;\r\n      line-height: 1.5;\r\n      margin-bottom: 30px;\r\n      max-width: 600px;\r\n    }\r\n    \r\n    .event-button {\r\n      display: inline-block;\r\n      padding: 15px 40px;\r\n      background-color: #aaff00;\r\n      color: #000000;\r\n      text-decoration: none;\r\n      border-radius: 5px;\r\n      font-weight: 700;\r\n      font-size: 18px;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n      transition: all 0.3s ease;\r\n      box-shadow: 0 0 20px rgba(170, 255, 0, 0.5);\r\n    }\r\n    \r\n    .event-button:hover {\r\n      transform: scale(1.05);\r\n      box-shadow: 0 0 30px rgba(170, 255, 0, 0.7);\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n<div class=\"countdown-container\">\r\n  <div class=\"bf-header\">\r\n    <h1 class=\"bf-title\">BLACK <span>FRIDAY</span></h1>\r\n  </div>\r\n  \r\n  <div class=\"days-remaining\">\r\n    <strong>{{days}}</strong> DAYS TO GO\r\n  </div>\r\n  \r\n  <div class=\"countdown-timer\">\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{days}}</div>\r\n      <div class=\"time-label\">Days</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{hours}}</div>\r\n      <div class=\"time-label\">Hours</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{minutes}}</div>\r\n      <div class=\"time-label\">Mins</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{seconds}}</div>\r\n      <div class=\"time-label\">Secs</div>\r\n    </div>\r\n  </div>\r\n  \r\n  <div class=\"discount-badge\">Up to 70% off</div>\r\n  \r\n  <p class=\"event-description\">{{eventDescription}}</p>\r\n  \r\n  <a href=\"{{eventLink}}\" class=\"event-button\">{{buttonText}}</a>\r\n</div>\r\n</body>\r\n</html>", true, 3, "Black Friday", "#000000", true, null },
                    { 8, null, new DateTime(2025, 5, 26, 20, 53, 20, 573, DateTimeKind.Utc).AddTicks(3431), "Raleway, sans-serif", "A sleek launch countdown for product releases", "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Launching Soon Countdown</title>\r\n  <style>\r\n    .countdown-container {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      justify-content: center;\r\n      height: 100%;\r\n      width: 100%;\r\n      padding: 30px;\r\n      box-sizing: border-box;\r\n      font-family: Raleway, sans-serif;\r\n      background-color: #2c3e50;\r\n      color: white;\r\n      text-align: center;\r\n      position: relative;\r\n      overflow: hidden;\r\n    }\r\n    \r\n    .bg-overlay {\r\n      position: absolute;\r\n      top: 0;\r\n      left: 0;\r\n      width: 100%;\r\n      height: 100%;\r\n      background: linear-gradient(45deg, rgba(52, 73, 94, 0.8) 0%, rgba(44, 62, 80, 0.8) 100%);\r\n      z-index: -1;\r\n    }\r\n    \r\n    .launch-badge {\r\n      background-color: #e74c3c;\r\n      color: white;\r\n      font-size: 14px;\r\n      font-weight: 600;\r\n      text-transform: uppercase;\r\n      letter-spacing: 2px;\r\n      padding: 8px 20px;\r\n      border-radius: 30px;\r\n      margin-bottom: 30px;\r\n    }\r\n    \r\n    .event-title {\r\n      font-size: 36px;\r\n      font-weight: 300;\r\n      margin: 0;\r\n      margin-bottom: 10px;\r\n      letter-spacing: 2px;\r\n    }\r\n    \r\n    .event-subtitle {\r\n      font-size: 16px;\r\n      font-weight: 300;\r\n      margin-bottom: 40px;\r\n      opacity: 0.8;\r\n      max-width: 600px;\r\n    }\r\n    \r\n    .countdown-timer {\r\n      display: flex;\r\n      align-items: flex-start;\r\n      justify-content: center;\r\n      margin-bottom: 40px;\r\n    }\r\n    \r\n    .time-block {\r\n      background-color: rgba(255, 255, 255, 0.1);\r\n      border-radius: 5px;\r\n      padding: 15px 10px;\r\n      margin: 0 5px;\r\n      min-width: 70px;\r\n      backdrop-filter: blur(5px);\r\n    }\r\n    \r\n    .time-value {\r\n      font-size: 36px;\r\n      font-weight: 200;\r\n      margin-bottom: 5px;\r\n    }\r\n    \r\n    .time-label {\r\n      font-size: 12px;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n      opacity: 0.7;\r\n    }\r\n    \r\n    .notification-form {\r\n      display: flex;\r\n      flex-direction: column;\r\n      align-items: center;\r\n      margin-bottom: 30px;\r\n      width: 100%;\r\n      max-width: 500px;\r\n    }\r\n    \r\n    .form-title {\r\n      font-size: 14px;\r\n      margin-bottom: 15px;\r\n      font-weight: 600;\r\n    }\r\n    \r\n    .email-input {\r\n      width: 100%;\r\n      padding: 12px 15px;\r\n      border-radius: 5px;\r\n      border: none;\r\n      margin-bottom: 10px;\r\n      font-family: Raleway, sans-serif;\r\n      font-size: 14px;\r\n    }\r\n    \r\n    .event-button {\r\n      display: inline-block;\r\n      padding: 12px 30px;\r\n      background-color: #3498db;\r\n      color: white;\r\n      text-decoration: none;\r\n      border-radius: 5px;\r\n      font-weight: 600;\r\n      transition: all 0.3s ease;\r\n      text-transform: uppercase;\r\n      letter-spacing: 1px;\r\n      font-size: 12px;\r\n      border: none;\r\n      cursor: pointer;\r\n      width: 100%;\r\n    }\r\n    \r\n    .event-button:hover {\r\n      background-color: #2980b9;\r\n    }\r\n    \r\n    .social-links {\r\n      display: flex;\r\n      justify-content: center;\r\n      margin-top: 20px;\r\n    }\r\n    \r\n    .social-link {\r\n      display: inline-flex;\r\n      align-items: center;\r\n      justify-content: center;\r\n      width: 40px;\r\n      height: 40px;\r\n      border-radius: 50%;\r\n      background-color: rgba(255, 255, 255, 0.1);\r\n      margin: 0 5px;\r\n      color: white;\r\n      text-decoration: none;\r\n      font-size: 16px;\r\n      transition: all 0.3s ease;\r\n    }\r\n    \r\n    .social-link:hover {\r\n      background-color: #3498db;\r\n      transform: translateY(-3px);\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n<div class=\"countdown-container\">\r\n  <div class=\"bg-overlay\"></div>\r\n  \r\n  <div class=\"launch-badge\">Launching Soon</div>\r\n  \r\n  <h1 class=\"event-title\">{{eventName}}</h1>\r\n  <p class=\"event-subtitle\">{{eventDescription}}</p>\r\n  \r\n  <div class=\"countdown-timer\">\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{days}}</div>\r\n      <div class=\"time-label\">Days</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{hours}}</div>\r\n      <div class=\"time-label\">Hours</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{minutes}}</div>\r\n      <div class=\"time-label\">Minutes</div>\r\n    </div>\r\n    <div class=\"time-block\">\r\n      <div class=\"time-value\">{{seconds}}</div>\r\n      <div class=\"time-label\">Seconds</div>\r\n    </div>\r\n  </div>\r\n  \r\n  <div class=\"notification-form\">\r\n    <div class=\"form-title\">GET NOTIFIED WHEN WE LAUNCH</div>\r\n    <input type=\"email\" class=\"email-input\" placeholder=\"Your email address\">\r\n    <button class=\"event-button\">NOTIFY ME</button>\r\n  </div>\r\n  \r\n  <a href=\"{{eventLink}}\" class=\"event-button\">{{buttonText}}</a>\r\n  \r\n  <div class=\"social-links\">\r\n    <a href=\"#\" class=\"social-link\">f</a>\r\n    <a href=\"#\" class=\"social-link\">t</a>\r\n    <a href=\"#\" class=\"social-link\">in</a>\r\n    <a href=\"#\" class=\"social-link\">ig</a>\r\n  </div>\r\n</div>\r\n</body>\r\n</html>", true, 3, "Launching Soon", "#2c3e50", true, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CountdownTimers_PostId",
                table: "CountdownTimers",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPages_UserId",
                table: "FacebookPages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentResults_UserId",
                table: "PaymentResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentResults_UserSubscriptionId",
                table: "PaymentResults",
                column: "UserSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_FacebookPageId",
                table: "Posts",
                column: "FacebookPageId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TemplateId",
                table: "Posts",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_MinimumSubscriptionPlanId",
                table: "Templates",
                column: "MinimumSubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_SubscriptionPlanId",
                table: "UserSubscriptions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CountdownTimers");

            migrationBuilder.DropTable(
                name: "FacebookPostResults");

            migrationBuilder.DropTable(
                name: "PaymentResults");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "FacebookPages");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");
        }
    }
}
