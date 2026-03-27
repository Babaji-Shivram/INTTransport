using AutoMapper;
using ERP.Transport.Application.DTOs.Rate;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Vehicle rate management — independent CRUD + search for rates.
/// </summary>
public class VehicleRateService : IVehicleRateService
{
    private readonly IRepository<VehicleRate> _rateRepo;
    private readonly IRepository<TransportVehicle> _vehicleRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<VehicleRateService> _logger;

    public VehicleRateService(
        IRepository<VehicleRate> rateRepo,
        IRepository<TransportVehicle> vehicleRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<VehicleRateService> logger)
    {
        _rateRepo = rateRepo;
        _vehicleRepo = vehicleRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<VehicleRateListDto>> SearchAsync(
        RateSearchRequest request, CancellationToken ct = default)
    {
        var (items, totalCount) = await _rateRepo.GetPagedAsync(
            predicate: r =>
                (!request.TransportVehicleId.HasValue || r.TransportVehicleId == request.TransportVehicleId.Value) &&
                (!request.IsApproved.HasValue || r.IsApproved == request.IsApproved.Value) &&
                (request.CurrencyCode == null || r.CurrencyCode == request.CurrencyCode) &&
                (!request.MinRate.HasValue || r.TotalRate >= request.MinRate.Value) &&
                (!request.MaxRate.HasValue || r.TotalRate <= request.MaxRate.Value),
            orderBy: q => q.OrderByDescending(r => r.CreatedDate),
            page: request.Page,
            pageSize: request.PageSize,
            includes: r => r.TransportVehicle);

        var dtos = items.Select(r =>
        {
            var dto = _mapper.Map<VehicleRateListDto>(r);
            dto.TransporterName = r.TransportVehicle?.TransporterName;
            dto.VehicleNumber = r.TransportVehicle?.VehicleNumber;
            return dto;
        }).ToList();

        return new PagedResultDto<VehicleRateListDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<VehicleRateMasterDto?> GetByIdAsync(Guid rateId, CancellationToken ct = default)
    {
        var entity = await _rateRepo.GetByIdAsync(rateId);
        if (entity == null) return null;

        var dto = _mapper.Map<VehicleRateMasterDto>(entity);

        // Enrich with vehicle/transporter info
        var vehicle = await _vehicleRepo.GetByIdAsync(entity.TransportVehicleId);
        if (vehicle != null)
        {
            dto.TransporterId = vehicle.TransporterId;
            dto.TransporterName = vehicle.TransporterName;
            dto.VehicleNumber = vehicle.VehicleNumber;
        }

        return dto;
    }

    public async Task<VehicleRateMasterDto?> GetBestRateAsync(
        Guid? transporterId, Guid? transportVehicleId, CancellationToken ct = default)
    {
        var rates = await _rateRepo.FindAsync(r =>
            r.IsApproved &&
            (!transportVehicleId.HasValue || r.TransportVehicleId == transportVehicleId.Value));

        // If filtering by transporter, we need to join via TransportVehicle
        if (transporterId.HasValue)
        {
            var vehicleIds = (await _vehicleRepo.FindAsync(v => v.TransporterId == transporterId.Value))
                .Select(v => v.Id)
                .ToHashSet();

            rates = rates.Where(r => vehicleIds.Contains(r.TransportVehicleId));
        }

        var bestRate = rates.OrderBy(r => r.TotalRate).FirstOrDefault();
        if (bestRate == null) return null;

        return await GetByIdAsync(bestRate.Id, ct);
    }

    public async Task<VehicleRateMasterDto> CreateAsync(
        CreateVehicleRateRequest request, Guid userId, CancellationToken ct = default)
    {
        // Validate vehicle exists
        var vehicle = await _vehicleRepo.GetByIdAsync(request.TransportVehicleId)
            ?? throw new KeyNotFoundException($"Transport vehicle {request.TransportVehicleId} not found");

        var entity = _mapper.Map<VehicleRate>(request);
        entity.TotalRate = request.FreightRate + request.DetentionCharges + request.VaraiCharges +
                           request.EmptyContainerReturn + request.TollCharges + request.OtherCharges;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _rateRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Rate {RateId} created for vehicle {VehicleId}, total={Total}",
            entity.Id, entity.TransportVehicleId, entity.TotalRate);

        return await GetByIdAsync(entity.Id, ct) ?? _mapper.Map<VehicleRateMasterDto>(entity);
    }

    public async Task<VehicleRateMasterDto> UpdateAsync(
        Guid rateId, UpdateVehicleRateRequest request, Guid userId, CancellationToken ct = default)
    {
        var entity = await _rateRepo.GetByIdAsync(rateId)
            ?? throw new KeyNotFoundException($"Vehicle rate {rateId} not found");

        if (entity.IsApproved)
            throw new InvalidOperationException("Cannot update an already-approved rate");

        if (request.FreightRate.HasValue) entity.FreightRate = request.FreightRate.Value;
        if (request.DetentionCharges.HasValue) entity.DetentionCharges = request.DetentionCharges.Value;
        if (request.VaraiCharges.HasValue) entity.VaraiCharges = request.VaraiCharges.Value;
        if (request.EmptyContainerReturn.HasValue) entity.EmptyContainerReturn = request.EmptyContainerReturn.Value;
        if (request.TollCharges.HasValue) entity.TollCharges = request.TollCharges.Value;
        if (request.OtherCharges.HasValue) entity.OtherCharges = request.OtherCharges.Value;
        if (request.CurrencyCode != null) entity.CurrencyCode = request.CurrencyCode;
        if (request.BillingInstruction != null) entity.BillingInstruction = request.BillingInstruction;
        if (request.ContractPrice.HasValue) entity.ContractPrice = request.ContractPrice.Value;
        if (request.SellingPrice.HasValue) entity.SellingPrice = request.SellingPrice.Value;
        if (request.MarketRate.HasValue) entity.MarketRate = request.MarketRate.Value;
        if (request.MemoDocumentUrl != null) entity.MemoDocumentUrl = request.MemoDocumentUrl;

        // Recalculate total
        entity.TotalRate = entity.FreightRate + entity.DetentionCharges + entity.VaraiCharges +
                           entity.EmptyContainerReturn + entity.TollCharges + entity.OtherCharges;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _rateRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Rate {RateId} updated, new total={Total}", rateId, entity.TotalRate);

        return await GetByIdAsync(entity.Id, ct) ?? _mapper.Map<VehicleRateMasterDto>(entity);
    }

    public async Task DeleteAsync(Guid rateId, Guid userId, CancellationToken ct = default)
    {
        var entity = await _rateRepo.GetByIdAsync(rateId)
            ?? throw new KeyNotFoundException($"Vehicle rate {rateId} not found");

        if (entity.IsApproved)
            throw new InvalidOperationException("Cannot delete an approved rate");

        entity.IsDeleted = true;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _rateRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Rate {RateId} soft-deleted", rateId);
    }
}
