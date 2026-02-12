using AutoMapper;
using ERP.Transport.Application.DTOs;
using ERP.Transport.Domain.Entities;

namespace ERP.Transport.Application.Mapping;

/// <summary>
/// AutoMapper profile — entity ↔ DTO mappings.
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

        // ── TransporterNotification ──────────────────────────
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
        // ── ULIP Entities ────────────────────────────────────────
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

        // ── CharteredInfo Entities ────────────────────────────────
        CreateMap<GstDetail, GstDetailDto>();
        CreateMap<EInvoice, EInvoiceDto>();

        // ── ConsolidatedTrip ─────────────────────────────────
        CreateMap<ConsolidatedTrip, ConsolidatedTripDto>();
        CreateMap<ConsolidatedTrip, ConsolidatedTripListDto>();
        CreateMap<ConsolidatedVehicle, ConsolidatedVehicleDto>();
        CreateMap<CreateConsolidatedVehicleDto, ConsolidatedVehicle>();
        CreateMap<ConsolidatedExpense, ConsolidatedExpenseDto>();
        CreateMap<CreateConsolidatedExpenseDto, ConsolidatedExpense>();
        CreateMap<ConsolidatedStopDelivery, ConsolidatedStopDeliveryDto>();

        // ── FleetVehicle ─────────────────────────────────────
        CreateMap<FleetVehicle, FleetVehicleDto>()
            .ForMember(d => d.IsInsuranceExpired, opt => opt.MapFrom(s =>
                s.InsuranceExpiry.HasValue && s.InsuranceExpiry.Value < DateTime.UtcNow))
            .ForMember(d => d.IsFitnessExpired, opt => opt.MapFrom(s =>
                s.FitnessExpiry.HasValue && s.FitnessExpiry.Value < DateTime.UtcNow))
            .ForMember(d => d.IsPermitExpired, opt => opt.MapFrom(s =>
                s.PermitExpiry.HasValue && s.PermitExpiry.Value < DateTime.UtcNow));
        CreateMap<FleetVehicle, FleetVehicleListDto>();
        CreateMap<CreateFleetVehicleDto, FleetVehicle>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.IsActive, opt => opt.Ignore())
            .ForMember(d => d.CurrentStatus, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── VehicleDriver ────────────────────────────────────
        CreateMap<VehicleDriver, VehicleDriverDto>();
        CreateMap<AssignDriverDto, VehicleDriver>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.FleetVehicleId, opt => opt.Ignore())
            .ForMember(d => d.IsActive, opt => opt.Ignore())
            .ForMember(d => d.AssignedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── VehicleDailyStatus ───────────────────────────────
        CreateMap<VehicleDailyStatus, VehicleDailyStatusDto>();

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

        // ── TransportLookup ──────────────────────────────────
        CreateMap<TransportLookup, TransportLookupDto>();
        CreateMap<CreateTransportLookupDto, TransportLookup>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── VehicleTravelLog ─────────────────────────────────
        CreateMap<VehicleTravelLog, VehicleTravelLogDto>();
        CreateMap<CreateVehicleTravelLogDto, VehicleTravelLog>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.DistanceKm, opt => opt.Ignore())
            .ForMember(d => d.TripDurationMinutes, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── VehicleDailyExpense (Legacy Gap) ─────────────────
        CreateMap<VehicleDailyExpense, VehicleDailyExpenseDto>();
        CreateMap<CreateVehicleDailyExpenseDto, VehicleDailyExpense>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.FleetVehicleId, opt => opt.Ignore())
            .ForMember(d => d.TotalAmount, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── StampDuty (Legacy Gap) ───────────────────────────
        CreateMap<StampDuty, StampDutyDto>();
        CreateMap<CreateStampDutyDto, StampDuty>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.ReferenceNumber, opt => opt.Ignore())
            .ForMember(d => d.IsPaid, opt => opt.Ignore())
            .ForMember(d => d.PaidDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── TransitWarehouse (Legacy Gap) ────────────────────
        CreateMap<TransitWarehouse, TransitWarehouseDto>();
        CreateMap<ArriveAtWarehouseDto, TransitWarehouse>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.TransportRequestId, opt => opt.Ignore())
            .ForMember(d => d.ArrivalDate, opt => opt.Ignore())
            .ForMember(d => d.IsDispatched, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── ExpenseApproval (Legacy Gap) ─────────────────────
        CreateMap<ExpenseApproval, ExpenseApprovalDto>();

        // ── PaymentVoucher (Legacy Gap) ──────────────────────
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
