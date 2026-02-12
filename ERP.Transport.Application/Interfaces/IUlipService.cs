using ERP.Transport.Application.DTOs;

namespace ERP.Transport.Application.Interfaces;

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
