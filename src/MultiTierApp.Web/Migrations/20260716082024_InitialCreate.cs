using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MultiTierApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "bi-folder"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "manual"),
                    IsEmbedded = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentChunks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChunkIndex = table.Column<int>(type: "int", nullable: false),
                    VectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentChunks_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IconClass", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Technical documentation for our products and APIs.", "bi-book", "Product Documentation", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Frequently asked questions and support articles.", "bi-question-circle", "FAQs & Support", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Internal processes, onboarding guides, and best practices.", "bi-gear", "Internal Guides", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Documents",
                columns: new[] { "Id", "CategoryId", "Content", "CreatedAt", "Source", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, "Welcome to our API documentation. Our RESTful API allows you to integrate our services into your applications seamlessly. To get started, you need to obtain an API key from the developer portal. Once you have your key, you can make requests to our endpoints using standard HTTP methods. The base URL for all API requests is https://api.example.com/v1. Authentication is handled via Bearer tokens — include your API key in the Authorization header of each request. Rate limits are set to 1000 requests per minute for standard tier and 10000 for enterprise tier. All responses are returned in JSON format with appropriate HTTP status codes.", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "manual", "Getting Started with Our API", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, "If you have forgotten your password, follow these steps to reset it: Step 1: Navigate to the login page and click 'Forgot Password'. Step 2: Enter the email address associated with your account. Step 3: Check your inbox for a password reset link (it may take up to 5 minutes). Step 4: Click the link and enter your new password. Passwords must be at least 12 characters long and include uppercase letters, lowercase letters, numbers, and special characters. Step 5: Confirm your new password and click 'Reset'. You will be redirected to the login page. If you do not receive the email, check your spam folder or contact support at support@example.com.", new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manual", "How to Reset Your Password", new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, "Welcome to the team! Here is your onboarding checklist to complete in your first week: Day 1: Complete HR paperwork, set up your workstation, and get your building access badge. Day 2: Attend the company orientation session and meet your team lead. Day 3: Set up your development environment following the DevOps setup guide. Install required tools: Visual Studio, Docker Desktop, Git, and SQL Server Management Studio. Day 4: Complete mandatory security training and code of conduct review. Day 5: Shadow a senior developer and review the codebase architecture documentation. By end of Week 2, you should have completed your first code review and submitted a small pull request.", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manual", "New Employee Onboarding Checklist", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentChunks_DocumentId",
                table: "DocumentChunks",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentChunks_VectorId",
                table: "DocumentChunks",
                column: "VectorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CategoryId",
                table: "Documents",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Title",
                table: "Documents",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentChunks");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
