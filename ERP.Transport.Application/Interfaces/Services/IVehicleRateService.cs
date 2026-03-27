using ERP.Transport.Application.DTOs.Rate;
using ERP.Transport.Application.DTOs.Common;

namespace ERP.Transport.Application.Interfaces.Services;

/// <summary>
/// Vehicle rate management — independent CRUD + search for rates.
/// </summary>
public interface IVehicleRateService
{
    Task<PagedResultDto<VehicleRateListDto>> SearchAsync(RateSearchRequest request, CancellationToken ct = default);
    Task<VehicleRateMasterDto?> GetByIdAsync(Guid rateId, CancellationToken ct = default);
    Task<VehicleRateMasterDto?> GetBestRateAsync(Guid? transporterId, Guid? transportVehicleId, CancellationToken ct = default);
    Task<VehicleRateMasterDto> CreateAsync(CreateVehicleRateRequest request, Guid userId, CancellationToken ct = default);
    Task<VehicleRateMasterDto> UpdateAsync(Guid rateId, UpdateVehicleRateRequest request, Guid userId, CancellationToken ct = default);
    Task DeleteAsync(Guid rateId, Guid userId, CancellationToken ct = default);
}
