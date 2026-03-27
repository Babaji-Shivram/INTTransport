using ERP.Transport.Domain.Enums;

namespace ERP.Transport.Application.DTOs.Dispatch;

// ═══════════════════════════════════════════════════════════════
//  Dispatch Management DTOs
// ═══════════════════════════════════════════════════════════════

/// <summary>Dispatch info for a transport job.</summary>
public class DispatchDto
{
    public Guid JobId { get; set; }
    public string RequestNumber { get; set; } = null!;
    public TransportStatus Status { get; set; }

    // ── Customer / Route ─────────────────────────────────────
    public string CustomerName { get; set; } = null!;
    public string? PickupCity { get; set; }
    public string? DropCity { get; set; }

    // ── Vehicle ──────────────────────────────────────────────
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public string? TransporterName { get; set; }

    // ── Dispatch Info ────────────────────────────────────────
    public DateTime? DispatchDate { get; set; }
    public Guid? DispatchedBy { get; set; }
    public string? Remarks { get; set; }

    // ── Cargo ────────────────────────────────────────────────
    public decimal GrossWeightKg { get; set; }
    public int NumberOfPackages { get; set; }
    public DateTime? RequiredDeliveryDate { get; set; }
}

/// <summary>Dispatch a job — set it in transit.</summary>
public class CreateDispatchRequest
{
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public DateTime? DispatchDate { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>Dashboard summary for dispatch operations.</summary>
public class DispatchSummaryDto
{
    public DateTime Date { get; set; }
    public int TotalDispatched { get; set; }
    public int PendingDispatch { get; set; }
    public int InTransit { get; set; }
    public int DeliveredToday { get; set; }
}
