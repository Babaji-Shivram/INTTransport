using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Fleet;
using ERP.Transport.Application.DTOs.Transporter;
using ERP.Transport.Application.DTOs.Maintenance;
using ERP.Transport.Application.DTOs.Expense;
using ERP.Transport.Application.DTOs.Voucher;
using ERP.Transport.Application.DTOs.StampDuty;
using ERP.Transport.Application.DTOs.Report;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;
using ERP.Transport.Application.DTOs.Integration;
using ERP.Transport.Application.DTOs.Warehouse;

namespace ERP.Transport.Application.Interfaces.Services;

/// <summary>
/// ULIP business logic — caching, compliance checks, linking to transport jobs.
/// </summary>
public interface IUlipService
{
    // ── VAHAN ───────────────────────────────────────────────────
    Task<VehicleDetailDto> LookupVehicleAsync(
        VehicleLookupRequestDto request, CancellationToken ct = default);

    // ── SARATHI ─────────────────────────────────────────────────
    Task<DriverLicenseDetailDto> VerifyDriverLicenseAsync(
        DriverLicenseVerifyRequestDto request, CancellationToken ct = default);

    // ── FASTAG ──────────────────────────────────────────────────
    Task<FASTagLookupResponseDto> GetFASTagTransactionsAsync(
        string vehicleNumber, Guid? transportRequestId = null, CancellationToken ct = default);

    // ── TOLL ────────────────────────────────────────────────────
    Task<TollPlazaDto?> GetTollPlazaAsync(
        string plazaId, CancellationToken ct = default);

    // ── E-Way Bill ──────────────────────────────────────────────
    Task<EWayBillDto> GenerateEWayBillAsync(
        GenerateEWayBillRequestDto request, Guid userId, CancellationToken ct = default);
    Task<EWayBillDto> GenerateEWayBillFromJobAsync(
        Guid transportRequestId, Guid userId, CancellationToken ct = default);
    Task<EWayBillDto?> GetEWayBillByJobAsync(
        Guid transportRequestId, CancellationToken ct = default);

    // ── Health ──────────────────────────────────────────────────
    Task<UlipHealthDto> HealthCheckAsync(CancellationToken ct = default);
}
