using AutoMapper;
using ERP.Transport.Application.DTOs.Maintenance;
using ERP.Transport.Domain.Entities;

namespace ERP.Transport.Application.Mapping;

/// <summary>
/// AutoMapper profile — maintenance ↔ DTO mappings.
/// </summary>
public class MaintenanceMappingProfile : Profile
{
    public MaintenanceMappingProfile()
    {
        // ── MaintenanceWorkOrder ─────────────────────────────
        CreateMap<MaintenanceWorkOrder, MaintenanceWorkOrderDto>();
        CreateMap<MaintenanceWorkOrder, MaintenanceWorkOrderListDto>();
        CreateMap<CreateMaintenanceWorkOrderDto, MaintenanceWorkOrder>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.WorkOrderNumber, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.StartedDate, opt => opt.Ignore())
            .ForMember(d => d.CompletedDate, opt => opt.Ignore())
            .ForMember(d => d.CompletedBy, opt => opt.Ignore())
            .ForMember(d => d.ActualCost, opt => opt.Ignore())
            .ForMember(d => d.ActualHours, opt => opt.Ignore())
            .ForMember(d => d.CompletionNotes, opt => opt.Ignore())
            .ForMember(d => d.Parts, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── MaintenancePart ──────────────────────────────────
        CreateMap<MaintenancePart, MaintenancePartDto>();
        CreateMap<AddMaintenancePartDto, MaintenancePart>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.MaintenanceWorkOrderId, opt => opt.Ignore())
            .ForMember(d => d.TotalCost, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── MaintenanceDocument ──────────────────────────────
        CreateMap<MaintenanceDocument, MaintenanceDocumentDto>();
        CreateMap<CreateMaintenanceDocumentDto, MaintenanceDocument>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.MaintenanceWorkOrderId, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());
    }
}
