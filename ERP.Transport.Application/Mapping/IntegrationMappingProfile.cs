using AutoMapper;
using ERP.Transport.Application.DTOs.Integration;
using ERP.Transport.Domain.Entities;

namespace ERP.Transport.Application.Mapping;

/// <summary>
/// AutoMapper profile — ULIP + CharteredInfo entity ↔ DTO mappings.
/// </summary>
public class IntegrationMappingProfile : Profile
{
    public IntegrationMappingProfile()
    {
        // ── ULIP Entities ────────────────────────────────────
        CreateMap<VehicleDetail, VehicleDetailDto>()
            .ForMember(d => d.IsFitnessExpired, opt => opt.MapFrom(s =>
                s.FitnessUpto.HasValue && s.FitnessUpto.Value < DateTime.UtcNow))
            .ForMember(d => d.IsInsuranceExpired, opt => opt.MapFrom(s =>
                s.InsuranceUpto.HasValue && s.InsuranceUpto.Value < DateTime.UtcNow))
            .ForMember(d => d.IsPucExpired, opt => opt.MapFrom(s =>
                s.PucValidUpto.HasValue && s.PucValidUpto.Value < DateTime.UtcNow))
            .ForMember(d => d.IsPermitExpired, opt => opt.MapFrom(s =>
                s.PermitValidUpto.HasValue && s.PermitValidUpto.Value < DateTime.UtcNow));

        CreateMap<DriverLicenseDetail, DriverLicenseDetailDto>()
            .ForMember(d => d.IsExpired, opt => opt.MapFrom(s =>
                s.ValidTo.HasValue && s.ValidTo.Value < DateTime.UtcNow))
            .ForMember(d => d.IsValidForHMV, opt => opt.MapFrom(s =>
                !string.IsNullOrEmpty(s.VehicleClassesAuthorized) &&
                (s.VehicleClassesAuthorized.Contains("HMV") ||
                 s.VehicleClassesAuthorized.Contains("HPMV") ||
                 s.VehicleClassesAuthorized.Contains("TRANS"))));

        CreateMap<FASTagTransaction, FASTagTransactionDto>();
        CreateMap<TollPlaza, TollPlazaDto>();
        CreateMap<EWayBill, EWayBillDto>()
            .ForMember(d => d.IsExpired, opt => opt.MapFrom(s =>
                s.ValidUpto.HasValue && s.ValidUpto.Value < DateTime.UtcNow));

        // ── CharteredInfo Entities ───────────────────────────
        CreateMap<GstDetail, GstDetailDto>();
        CreateMap<EInvoice, EInvoiceDto>();
    }
}
