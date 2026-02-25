using AutoMapper;
using ERP.Transport.Application.DTOs.Fleet;
using ERP.Transport.Domain.Entities;

namespace ERP.Transport.Application.Mapping;

/// <summary>
/// AutoMapper profile — fleet vehicle ↔ DTO mappings.
/// </summary>
public class FleetMappingProfile : Profile
{
    public FleetMappingProfile()
    {
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

        // ── VehicleTravelLog ─────────────────────────────────
        CreateMap<VehicleTravelLog, VehicleTravelLogDto>();
        CreateMap<CreateVehicleTravelLogDto, VehicleTravelLog>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.DistanceKm, opt => opt.Ignore())
            .ForMember(d => d.TripDurationMinutes, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());

        // ── VehicleDailyExpense ──────────────────────────────
        CreateMap<VehicleDailyExpense, VehicleDailyExpenseDto>();
        CreateMap<CreateVehicleDailyExpenseDto, VehicleDailyExpense>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.FleetVehicleId, opt => opt.Ignore())
            .ForMember(d => d.TotalAmount, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore());
    }
}
