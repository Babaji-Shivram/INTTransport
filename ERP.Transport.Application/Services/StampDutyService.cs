using ERP.Transport.Application.DTOs.StampDuty;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Stamp duty management — regulatory compliance for transport documents.
/// </summary>
public class StampDutyService : IStampDutyService
{
    private readonly IRepository<StampDuty> _stampDutyRepo;
    private readonly IRepository<TransportRequest> _jobRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<StampDutyService> _logger;

    public StampDutyService(
        IRepository<StampDuty> stampDutyRepo,
        IRepository<TransportRequest> jobRepo,
        IUnitOfWork uow,
        ILogger<StampDutyService> logger)
    {
        _stampDutyRepo = stampDutyRepo;
        _jobRepo = jobRepo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<StampDutyDto> CreateAsync(CreateStampDutyDto dto, Guid userId)
    {
        // Generate reference number: SD-{Year}-{Seq}
        var year = DateTime.UtcNow.Year;
        var count = await _stampDutyRepo.CountAsync(s => s.CreatedDate.Year == year && !s.IsDeleted);
        var refNumber = $"SD-{year}-{(count + 1):D5}";

        var entity = new StampDuty
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = refNumber,
            TransportRequestId = dto.TransportRequestId,
            TransporterId = dto.TransporterId,
            DocumentType = dto.DocumentType,
            StampDutyAmount = dto.StampDutyAmount,
            DutyDate = dto.DutyDate,
            StateCode = dto.StateCode,
            Remarks = dto.Remarks,
            BranchId = dto.BranchId,
            CountryCode = dto.CountryCode,
            IsPaid = false,
            CreatedBy = userId,
            CreatedDate = DateTime.UtcNow
        };

        await _stampDutyRepo.AddAsync(entity);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Stamp duty {Ref} created for amount {Amount}", refNumber, dto.StampDutyAmount);
        return MapToDto(entity);
    }

    public async Task<StampDutyDto?> GetByIdAsync(Guid id)
    {
        var entity = await _stampDutyRepo.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<PagedResultDto<StampDutyDto>> GetAllAsync(StampDutyFilterDto filter)
    {
        var query = await _stampDutyRepo.FindAsync(s => !s.IsDeleted);
        var items = query.AsQueryable();

        if (filter.TransportRequestId.HasValue)
            items = items.Where(s => s.TransportRequestId == filter.TransportRequestId);
        if (filter.TransporterId.HasValue)
            items = items.Where(s => s.TransporterId == filter.TransporterId);
        if (filter.IsPaid.HasValue)
            items = items.Where(s => s.IsPaid == filter.IsPaid);
        if (!string.IsNullOrWhiteSpace(filter.StateCode))
            items = items.Where(s => s.StateCode == filter.StateCode);
        if (filter.FromDate.HasValue)
            items = items.Where(s => s.DutyDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue)
            items = items.Where(s => s.DutyDate <= filter.ToDate.Value);
        if (filter.BranchId.HasValue)
            items = items.Where(s => s.BranchId == filter.BranchId);

        var total = items.Count();
        var data = items
            .OrderByDescending(s => s.CreatedDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResultDto<StampDutyDto>
        {
            Items = data,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<StampDutyDto> UpdateAsync(Guid id, UpdateStampDutyDto dto, Guid userId)
    {
        var entity = await _stampDutyRepo.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
            ?? throw new TransportNotFoundException("StampDuty", id);

        if (entity.IsPaid)
            throw new TransportBusinessException("Cannot update a paid stamp duty record");

        if (dto.StampDutyAmount.HasValue)
            entity.StampDutyAmount = dto.StampDutyAmount.Value;
        if (dto.Remarks != null)
            entity.Remarks = dto.Remarks;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _stampDutyRepo.Update(entity);
        await _uow.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<StampDutyDto> RecordPaymentAsync(Guid id, RecordStampDutyPaymentDto dto, Guid userId)
    {
        var entity = await _stampDutyRepo.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
            ?? throw new TransportNotFoundException("StampDuty", id);

        if (entity.IsPaid)
            throw new TransportBusinessException("Stamp duty already paid");

        entity.PaidAmount = dto.PaidAmount;
        entity.ReceiptNumber = dto.ReceiptNumber;
        entity.ReceiptDocumentUrl = dto.ReceiptDocumentUrl;
        entity.PaidByName = dto.PaidByName;
        entity.PaidDate = DateTime.UtcNow;
        entity.IsPaid = true;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _stampDutyRepo.Update(entity);
        await _uow.SaveChangesAsync();

        _logger.LogInformation("Stamp duty {Ref} marked as paid: {Amount}", entity.ReferenceNumber, dto.PaidAmount);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var entity = await _stampDutyRepo.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
            ?? throw new TransportNotFoundException("StampDuty", id);

        entity.IsDeleted = true;
        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _stampDutyRepo.Update(entity);
        await _uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<StampDutyDto>> GetByJobAsync(Guid transportRequestId)
    {
        var items = await _stampDutyRepo.FindAsync(s => s.TransportRequestId == transportRequestId && !s.IsDeleted);
        return items.Select(MapToDto);
    }

    public async Task<decimal> GetTotalUnpaidByBranchAsync(Guid branchId)
    {
        var items = await _stampDutyRepo.FindAsync(s => s.BranchId == branchId && !s.IsPaid && !s.IsDeleted);
        return items.Sum(s => s.StampDutyAmount);
    }

    private static StampDutyDto MapToDto(StampDuty entity) => new()
    {
        Id = entity.Id,
        ReferenceNumber = entity.ReferenceNumber,
        TransportRequestId = entity.TransportRequestId,
        TransporterId = entity.TransporterId,
        DocumentType = entity.DocumentType,
        StampDutyAmount = entity.StampDutyAmount,
        PaidAmount = entity.PaidAmount,
        DutyDate = entity.DutyDate,
        StateCode = entity.StateCode,
        ReceiptNumber = entity.ReceiptNumber,
        ReceiptDocumentUrl = entity.ReceiptDocumentUrl,
        Remarks = entity.Remarks,
        IsPaid = entity.IsPaid,
        PaidDate = entity.PaidDate,
        PaidByName = entity.PaidByName,
        CurrencyCode = entity.CurrencyCode,
        CreatedDate = entity.CreatedDate
    };
}
