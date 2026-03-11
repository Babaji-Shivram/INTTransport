using EPR.Shared.Contracts.Entities;

namespace ERP.Transport.Domain.Entities;

/// <summary>
/// Thin wrapper over SharedLibrary's AuditableEntity — keeps all existing entity code untouched.
/// </summary>
public abstract class BaseEntity : AuditableEntity { }
