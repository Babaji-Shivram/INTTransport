using AutoMapper;
using ERP.Transport.Application.DTOs.Transporter;
using ERP.Transport.Domain.Entities;

namespace ERP.Transport.Application.Mapping;

/// <summary>
/// AutoMapper profile — transporter ↔ DTO mappings.
/// </summary>
public class TransporterMappingProfile : Profile
{
    public TransporterMappingProfile()
    {
        // ── Transporter ──────────────────────────────────────
        CreateMap<TransporterNotification, TransporterNotificationDto>();
        CreateMap<CreateNotificationDto, TransporterNotification>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransporterId, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());
        CreateMap<Transporter, TransporterDto>();
        CreateMap<Transporter, TransporterListDto>();
        CreateMap<CreateTransporterDto, Transporter>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.MapFrom(_ => Domain.Enums.TransporterStatus.Active))
            .ForMember(d => d.Rating, opt => opt.MapFrom(_ => 0m))
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransporterKYC ───────────────────────────────────
        CreateMap<TransporterKYC, TransporterKYCDto>();
        CreateMap<AddKYCDocumentDto, TransporterKYC>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransporterId, opt => opt.Ignore())
            .ForMember(d => d.IsVerified, opt => opt.MapFrom(_ => false))
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransporterBank ──────────────────────────────────
        CreateMap<TransporterBank, TransporterBankDto>();
        CreateMap<AddBankAccountDto, TransporterBank>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransporterId, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());
    }
}
