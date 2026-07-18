using Microsoft.EntityFrameworkCore;
using MultiTierApp.Web.Data.Configurations;
using MultiTierApp.Web.Models;

namespace MultiTierApp.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Fluent API configurations
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentChunkConfiguration());

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        var cat1 = new Category
        {
            Id = 1,
            Name = "Product Documentation",
            Description = "Technical documentation for our products and APIs.",
            IconClass = "bi-book",
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var cat2 = new Category
        {
            Id = 2,
            Name = "FAQs & Support",
            Description = "Frequently asked questions and support articles.",
            IconClass = "bi-question-circle",
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var cat3 = new Category
        {
            Id = 3,
            Name = "Internal Guides",
            Description = "Internal processes, onboarding guides, and best practices.",
            IconClass = "bi-gear",
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        modelBuilder.Entity<Category>().HasData(cat1, cat2, cat3);

        // Seed Documents
        modelBuilder.Entity<Document>().HasData(
            new Document
            {
                Id = 1,
                Title = "Getting Started with Our API",
                Content = "Welcome to our API documentation. Our RESTful API allows you to integrate our services into your applications seamlessly. " +
                          "To get started, you need to obtain an API key from the developer portal. Once you have your key, you can make requests " +
                          "to our endpoints using standard HTTP methods. The base URL for all API requests is https://api.example.com/v1. " +
                          "Authentication is handled via Bearer tokens — include your API key in the Authorization header of each request. " +
                          "Rate limits are set to 1000 requests per minute for standard tier and 10000 for enterprise tier. " +
                          "All responses are returned in JSON format with appropriate HTTP status codes.",
                Source = "manual",
                CategoryId = 1,
                IsEmbedded = false,
                CreatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new Document
            {
                Id = 2,
                Title = "How to Reset Your Password",
                Content = "If you have forgotten your password, follow these steps to reset it: " +
                          "Step 1: Navigate to the login page and click 'Forgot Password'. " +
                          "Step 2: Enter the email address associated with your account. " +
                          "Step 3: Check your inbox for a password reset link (it may take up to 5 minutes). " +
                          "Step 4: Click the link and enter your new password. Passwords must be at least 12 characters long " +
                          "and include uppercase letters, lowercase letters, numbers, and special characters. " +
                          "Step 5: Confirm your new password and click 'Reset'. You will be redirected to the login page. " +
                          "If you do not receive the email, check your spam folder or contact support at support@example.com.",
                Source = "manual",
                CategoryId = 2,
                IsEmbedded = false,
                CreatedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Document
            {
                Id = 3,
                Title = "New Employee Onboarding Checklist",
                Content = "Welcome to the team! Here is your onboarding checklist to complete in your first week: " +
                          "Day 1: Complete HR paperwork, set up your workstation, and get your building access badge. " +
                          "Day 2: Attend the company orientation session and meet your team lead. " +
                          "Day 3: Set up your development environment following the DevOps setup guide. Install required tools: " +
                          "Visual Studio, Docker Desktop, Git, and SQL Server Management Studio. " +
                          "Day 4: Complete mandatory security training and code of conduct review. " +
                          "Day 5: Shadow a senior developer and review the codebase architecture documentation. " +
                          "By end of Week 2, you should have completed your first code review and submitted a small pull request.",
                Source = "manual",
                CategoryId = 3,
                IsEmbedded = false,
                CreatedAt = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
