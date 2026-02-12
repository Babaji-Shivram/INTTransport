using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs;

// ═══════════════════════════════════════════════════════════════
//  Transport Lookup / Master Table DTOs
// ═══════════════════════════════════════════════════════════════

public class TransportLookupDto
{
    public Guid Id { get; set; }
    public LookupCategory Category { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string? CountryCode { get; set; }
}

public class CreateTransportLookupDto
{
    public LookupCategory Category { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string? CountryCode { get; set; }
}

public class UpdateTransportLookupDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? IsActive { get; set; }
}

public class LookupFilterDto
{
    public LookupCategory? Category { get; set; }
    public string? CountryCode { get; set; }
    public bool? IsActive { get; set; } = true;
    public string? SearchTerm { get; set; }
}
