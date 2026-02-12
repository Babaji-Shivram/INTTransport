using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

/// <summary>
/// Abstract base entity configuration — mirrors CRM's BaseEntityConfiguration exactly.
/// PK: NEWSEQUENTIALID(), soft delete filter, concurrency token, audit fields.
/// </summary>
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    /// <summary>Entity name used for PK column e.g. "TransportRequest" → "TransportRequestId"</summary>
    protected abstract string EntityName { get; }

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName($"{EntityName}Id")
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        // Soft Delete
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
        builder.HasQueryFilter(e => !e.IsDeleted);

        // Default
        builder.Property(e => e.IsDefault)
            .HasDefaultValue(false);

        // Version
        builder.Property(e => e.Version)
            .HasDefaultValue(1);

        // Concurrency
        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        // Audit
        builder.Property(e => e.CreatedDate)
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(e => e.UpdatedDate)
            .HasColumnType("datetime2(7)");

        // Indexes
        builder.HasIndex(e => e.CreatedDate);
        builder.HasIndex(e => e.IsDeleted);
    }
}
