using AutoMapper;
using ERP.Transport.Application.DTOs.Job;
using ERP.Transport.Application.DTOs.Expense;
using ERP.Transport.Application.DTOs.Common;
using ERP.Transport.Application.DTOs.ConsolidatedTrip;
using ERP.Transport.Application.DTOs.Warehouse;
using ERP.Transport.Domain.Entities;

namespace ERP.Transport.Application.Mapping;

/// <summary>
/// AutoMapper profile — core job entity ↔ DTO mappings.
/// </summary>
public class TransportMappingProfile : Profile
{
    public TransportMappingProfile()
    {
        // ── TransportRequest ─────────────────────────────────
        CreateMap<TransportRequest, TransportJobDto>();
        CreateMap<TransportRequest, TransportJobListDto>();
        CreateMap<TransportRequest, TransportJobStatusDto>();
        CreateMap<CreateTransportJobDto, TransportRequest>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.RequestNumber, opt => opt.Ignore())
            .ForMember(d => d.RequestDate, opt => opt.Ignore())
            .ForMember(d => d.Source, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.WorkflowInstanceId, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransportVehicle ─────────────────────────────────
        CreateMap<TransportVehicle, TransportVehicleDto>();
        CreateMap<AssignVehicleDto, TransportVehicle>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransportRequestId, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── VehicleRate ──────────────────────────────────────
        CreateMap<VehicleRate, VehicleRateDto>();
        CreateMap<EnterRateDto, VehicleRate>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransportVehicleId, opt => opt.Ignore())
            .ForMember(d => d.TotalRate, opt => opt.MapFrom(s =>
                s.FreightRate + s.DetentionCharges + s.VaraiCharges +
                s.EmptyContainerReturn + s.TollCharges + s.OtherCharges))
            .ForMember(d => d.IsApproved, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransportMovement ────────────────────────────────
        CreateMap<TransportMovement, TransportMovementDto>();
        CreateMap<AddMovementDto, TransportMovement>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransportRequestId, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransportDelivery ────────────────────────────────
        CreateMap<TransportDelivery, TransportDeliveryDto>();
        CreateMap<RecordDeliveryDto, TransportDelivery>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransportRequestId, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransportDocument ────────────────────────────────
        CreateMap<TransportDocument, TransportDocumentDto>();
        CreateMap<UploadDocumentDto, TransportDocument>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransportRequestId, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransportExpense ─────────────────────────────────
        CreateMap<TransportExpense, TransportExpenseDto>();
        CreateMap<CreateExpenseDto, TransportExpense>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransportRequestId, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── VehicleFundRequest ───────────────────────────────
        CreateMap<VehicleFundRequest, VehicleFundRequestDto>();
        CreateMap<CreateFundRequestDto, VehicleFundRequest>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransportVehicleId, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.ProcessedBy, opt => opt.Ignore())
            .ForMember(d => d.ProcessedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransportLookup ──────────────────────────────────
        CreateMap<TransportLookup, TransportLookupDto>();
        CreateMap<CreateTransportLookupDto, TransportLookup>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransitWarehouse ─────────────────────────────────
        CreateMap<TransitWarehouse, TransitWarehouseDto>();
        CreateMap<ArriveAtWarehouseDto, TransitWarehouse>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransportRequestId, opt => opt.Ignore())
            .ForMember(d => d.ArrivalDate, opt => opt.Ignore())
            .ForMember(d => d.IsDispatched, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── ConsolidatedTrip ─────────────────────────────────
        CreateMap<ConsolidatedTrip, ConsolidatedTripDto>();
        CreateMap<ConsolidatedTrip, ConsolidatedTripListDto>();
        CreateMap<ConsolidatedVehicle, ConsolidatedVehicleDto>();
        CreateMap<CreateConsolidatedVehicleDto, ConsolidatedVehicle>();
        CreateMap<ConsolidatedExpense, ConsolidatedExpenseDto>();
        CreateMap<CreateConsolidatedExpenseDto, ConsolidatedExpense>();
        CreateMap<ConsolidatedStopDelivery, ConsolidatedStopDeliveryDto>();
    }
}
