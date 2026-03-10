namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Base entity with common audit fields — identical to CRM/Masters pattern
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool IsDefault { get; set; }

    public int Version { get; set; } = 1;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
