using AutoMapper;
using ERP.Transport.Application.DTOs.Expense;
using ERP.Transport.Application.DTOs.StampDuty;
using ERP.Transport.Application.DTOs.Voucher;
using ERP.Transport.Domain.Entities;

namespace ERP.Transport.Application.Mapping;

/// <summary>
/// AutoMapper profile — financial entity ↔ DTO mappings (stamp duty, vouchers, expense approval).
/// </summary>
public class FinancialMappingProfile : Profile
{
    public FinancialMappingProfile()
    {
        // ── StampDuty ────────────────────────────────────────
        CreateMap<StampDuty, StampDutyDto>();
        CreateMap<CreateStampDutyDto, StampDuty>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.ReferenceNumber, opt => opt.Ignore())
            .ForMember(d => d.IsPaid, opt => opt.Ignore())
            .ForMember(d => d.PaidDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── ExpenseApproval ──────────────────────────────────
        CreateMap<ExpenseApproval, ExpenseApprovalDto>();

        // ── PaymentVoucher ───────────────────────────────────
        CreateMap<PaymentVoucher, PaymentVoucherDto>();
        CreateMap<CreatePaymentVoucherDto, PaymentVoucher>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.VoucherNumber, opt => opt.Ignore())
            .ForMember(d => d.AmountInWords, opt => opt.Ignore())
            .ForMember(d => d.IsPrinted, opt => opt.Ignore())
            .ForMember(d => d.PdfUrl, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());
    }
}
