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
/// Transporter vendor master CRUD + query.
/// </summary>
public interface ITransporterService
{
    Task<TransporterDto> CreateAsync(CreateTransporterDto dto, Guid userId);
    Task<TransporterDto?> GetByIdAsync(Guid id);
    Task<TransporterDto> UpdateAsync(Guid id, UpdateTransporterDto dto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<PagedResultDto<TransporterListDto>> GetAllAsync(TransporterFilterDto filter);

    // ── KYC ─────────────────────────────────────────────────────
    Task<TransporterKYCDto> AddKYCDocumentAsync(Guid transporterId, AddKYCDocumentDto dto, Guid userId);
    Task DeleteKYCDocumentAsync(Guid transporterId, Guid kycId, Guid userId);

    // ── Bank ────────────────────────────────────────────────────
    Task<TransporterBankDto> AddBankAccountAsync(Guid transporterId, AddBankAccountDto dto, Guid userId);
    Task<TransporterBankDto> UpdateBankAccountAsync(Guid transporterId, Guid bankId, UpdateBankAccountDto dto, Guid userId);
    Task DeleteBankAccountAsync(Guid transporterId, Guid bankId, Guid userId);

    // ── Notifications ──────────────────────────────────────────
    Task<IEnumerable<TransporterNotificationDto>> GetNotificationsAsync(Guid transporterId);
    Task<TransporterNotificationDto> AddNotificationAsync(Guid transporterId, CreateNotificationDto dto, Guid userId);
    Task<TransporterNotificationDto> UpdateNotificationAsync(Guid transporterId, Guid notificationId, UpdateNotificationDto dto, Guid userId);
    Task DeleteNotificationAsync(Guid transporterId, Guid notificationId, Guid userId);
}
