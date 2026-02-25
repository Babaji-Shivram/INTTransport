using AutoMapper;
using ERP.Transport.Application.DTOs.Transporter;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.Interfaces.Services;
using ERP.Transport.Application.Interfaces.Repositories;
using ERP.Transport.Domain.Entities;
using ERP.Transport.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ERP.Transport.Application.Services;

/// <summary>
/// Transporter vendor master CRUD.
/// </summary>
public class TransporterService : ITransporterService
{
    private readonly IRepository<Transporter> _transporterRepo;
    private readonly IRepository<TransporterKYC> _kycRepo;
    private readonly IRepository<TransporterBank> _bankRepo;
    private readonly IRepository<TransporterNotification> _notificationRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TransporterService> _logger;

    public TransporterService(
        IRepository<Transporter> transporterRepo,
        IRepository<TransporterKYC> kycRepo,
        IRepository<TransporterBank> bankRepo,
        IRepository<TransporterNotification> notificationRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<TransporterService> logger)
    {
        _transporterRepo = transporterRepo;
        _kycRepo = kycRepo;
        _bankRepo = bankRepo;
        _notificationRepo = notificationRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TransporterDto> CreateAsync(CreateTransporterDto dto, Guid userId)
    {
        var entity = _mapper.Map<Transporter>(dto);
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _transporterRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransporterDto>(entity);
    }

    public async Task<TransporterDto?> GetByIdAsync(Guid id)
    {
        var entity = await _transporterRepo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<TransporterDto>(entity);
    }

    public async Task<TransporterDto> UpdateAsync(Guid id, UpdateTransporterDto dto, Guid userId)
    {
        var entity = await _transporterRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Transporter {id} not found");

        if (dto.TransporterName != null) entity.TransporterName = dto.TransporterName;
        if (dto.ContactPerson != null) entity.ContactPerson = dto.ContactPerson;
        if (dto.Phone != null) entity.Phone = dto.Phone;
        if (dto.Email != null) entity.Email = dto.Email;
        if (dto.PANNumber != null) entity.PANNumber = dto.PANNumber;
        if (dto.GSTNumber != null) entity.GSTNumber = dto.GSTNumber;
        if (dto.Address != null) entity.Address = dto.Address;
        if (dto.City != null) entity.City = dto.City;
        if (dto.State != null) entity.State = dto.State;
        if (dto.Pincode != null) entity.Pincode = dto.Pincode;

        if (dto.Status.HasValue)
        {
            entity.Status = dto.Status.Value;
            if (dto.Status.Value == TransporterStatus.Suspended)
            {
                entity.SuspensionReason = dto.SuspensionReason;
                entity.SuspensionDate = DateTime.UtcNow;
            }
        }

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _transporterRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransporterDto>(entity);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var entity = await _transporterRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Transporter {id} not found");

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;
        _transporterRepo.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PagedResultDto<TransporterListDto>> GetAllAsync(TransporterFilterDto filter)
    {
        var (items, totalCount) = await _transporterRepo.GetPagedAsync(
            predicate: t =>
                (filter.Status == null || t.Status == filter.Status) &&
                (filter.CountryCode == null || t.CountryCode == filter.CountryCode) &&
                (filter.BranchId == null || t.BranchId == filter.BranchId) &&
                (filter.Search == null || t.TransporterName.Contains(filter.Search)),
            page: filter.Page,
            pageSize: filter.PageSize);

        return new PagedResultDto<TransporterListDto>
        {
            Items = _mapper.Map<IEnumerable<TransporterListDto>>(items),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    // ── KYC ─────────────────────────────────────────────────────

    public async Task<TransporterKYCDto> AddKYCDocumentAsync(
        Guid transporterId, AddKYCDocumentDto dto, Guid userId)
    {
        var entity = _mapper.Map<TransporterKYC>(dto);
        entity.TransporterId = transporterId;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _kycRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransporterKYCDto>(entity);
    }

    public async Task DeleteKYCDocumentAsync(Guid transporterId, Guid kycId, Guid userId)
    {
        var entity = await _kycRepo.GetByIdAsync(kycId)
            ?? throw new KeyNotFoundException($"KYC document {kycId} not found");

        if (entity.TransporterId != transporterId)
            throw new InvalidOperationException("KYC document does not belong to this transporter");

        entity.UpdatedBy = userId;
        _kycRepo.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    // ── Bank ────────────────────────────────────────────────────

    public async Task<TransporterBankDto> AddBankAccountAsync(
        Guid transporterId, AddBankAccountDto dto, Guid userId)
    {
        var entity = _mapper.Map<TransporterBank>(dto);
        entity.TransporterId = transporterId;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        // If marking as primary, unset others
        if (dto.IsPrimary)
        {
            var existing = await _bankRepo.FindAsync(b => b.TransporterId == transporterId && b.IsPrimary);
            foreach (var bank in existing)
            {
                bank.IsPrimary = false;
                _bankRepo.Update(bank);
            }
        }

        await _bankRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransporterBankDto>(entity);
    }

    public async Task<TransporterBankDto> UpdateBankAccountAsync(
        Guid transporterId, Guid bankId, UpdateBankAccountDto dto, Guid userId)
    {
        var entity = await _bankRepo.GetByIdAsync(bankId)
            ?? throw new KeyNotFoundException($"Bank account {bankId} not found");

        if (entity.TransporterId != transporterId)
            throw new InvalidOperationException("Bank account does not belong to this transporter");

        if (dto.BankName != null) entity.BankName = dto.BankName;
        if (dto.AccountNumber != null) entity.AccountNumber = dto.AccountNumber;
        if (dto.IFSCCode != null) entity.IFSCCode = dto.IFSCCode;
        if (dto.BranchName != null) entity.BranchName = dto.BranchName;
        if (dto.AccountHolderName != null) entity.AccountHolderName = dto.AccountHolderName;
        if (dto.IsPrimary.HasValue) entity.IsPrimary = dto.IsPrimary.Value;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _bankRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransporterBankDto>(entity);
    }

    public async Task DeleteBankAccountAsync(Guid transporterId, Guid bankId, Guid userId)
    {
        var entity = await _bankRepo.GetByIdAsync(bankId)
            ?? throw new KeyNotFoundException($"Bank account {bankId} not found");

        if (entity.TransporterId != transporterId)
            throw new InvalidOperationException("Bank account does not belong to this transporter");

        entity.UpdatedBy = userId;
        _bankRepo.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    // ── Notifications ───────────────────────────────────────────

    public async Task<IEnumerable<TransporterNotificationDto>> GetNotificationsAsync(Guid transporterId)
    {
        var items = await _notificationRepo.FindAsync(n => n.TransporterId == transporterId);
        return _mapper.Map<IEnumerable<TransporterNotificationDto>>(items);
    }

    public async Task<TransporterNotificationDto> AddNotificationAsync(
        Guid transporterId, CreateNotificationDto dto, Guid userId)
    {
        // Verify transporter exists
        _ = await _transporterRepo.GetByIdAsync(transporterId)
            ?? throw new KeyNotFoundException($"Transporter {transporterId} not found");

        // Prevent duplicate type+destination
        var existing = await _notificationRepo.FirstOrDefaultAsync(
            n => n.TransporterId == transporterId && n.Type == dto.Type && n.Destination == dto.Destination);
        if (existing != null)
            throw new InvalidOperationException(
                $"Notification preference for {dto.Type} to '{dto.Destination}' already exists");

        var entity = _mapper.Map<TransporterNotification>(dto);
        entity.TransporterId = transporterId;
        entity.CreatedBy = userId;
        entity.CreatedDate = DateTime.UtcNow;

        await _notificationRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransporterNotificationDto>(entity);
    }

    public async Task<TransporterNotificationDto> UpdateNotificationAsync(
        Guid transporterId, Guid notificationId, UpdateNotificationDto dto, Guid userId)
    {
        var entity = await _notificationRepo.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException($"Notification {notificationId} not found");

        if (entity.TransporterId != transporterId)
            throw new InvalidOperationException("Notification does not belong to this transporter");

        if (dto.Destination != null) entity.Destination = dto.Destination;
        if (dto.IsEnabled.HasValue) entity.IsEnabled = dto.IsEnabled.Value;

        entity.UpdatedBy = userId;
        entity.UpdatedDate = DateTime.UtcNow;

        _notificationRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransporterNotificationDto>(entity);
    }

    public async Task DeleteNotificationAsync(
        Guid transporterId, Guid notificationId, Guid userId)
    {
        var entity = await _notificationRepo.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException($"Notification {notificationId} not found");

        if (entity.TransporterId != transporterId)
            throw new InvalidOperationException("Notification does not belong to this transporter");

        entity.UpdatedBy = userId;
        _notificationRepo.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
