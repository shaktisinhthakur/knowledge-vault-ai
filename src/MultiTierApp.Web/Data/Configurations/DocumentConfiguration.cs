using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTierApp.Web.Models;

namespace MultiTierApp.Web.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Content)
            .IsRequired();

        builder.Property(d => d.Source)
            .HasMaxLength(50)
            .HasDefaultValue("manual");

        builder.Property(d => d.IsEmbedded)
            .HasDefaultValue(false);

        builder.Property(d => d.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(d => d.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(d => d.CategoryId);
        builder.HasIndex(d => d.Title);

        // One-to-many: Document -> DocumentChunks
        builder.HasMany(d => d.Chunks)
            .WithOne(c => c.Document)
            .HasForeignKey(c => c.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
