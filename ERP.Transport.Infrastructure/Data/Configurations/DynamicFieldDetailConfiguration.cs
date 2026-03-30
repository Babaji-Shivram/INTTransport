using EPR.Shared.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

public class DynamicFieldDetailConfiguration : IEntityTypeConfiguration<DynamicFieldDetail>
{
    public void Configure(EntityTypeBuilder<DynamicFieldDetail> builder)
    {
        builder.ToTable("DynamicFieldDetails");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ParentEntityType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.FieldName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.FieldValue).HasColumnType("nvarchar(max)");
        builder.Property(e => e.FieldType).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Section).HasMaxLength(100);
        builder.Property(e => e.StepCode).HasMaxLength(50);

        builder.HasIndex(e => new { e.ParentEntityId, e.ParentEntityType })
            .HasDatabaseName("IX_DynamicFieldDetails_Parent");
    }
}
