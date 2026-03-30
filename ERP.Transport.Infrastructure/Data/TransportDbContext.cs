using ERP.Transport.Domain.Entities;
using EPR.Shared.Contracts.Entities;
using EPR.Shared.Contracts.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Transport.Infrastructure.Data;

/// <summary>
/// Transport DbContext — mirrors CRM's CRMDbContext pattern.
/// </summary>
public class TransportDbContext : DbContext
{
    public TransportDbContext(DbContextOptions<TransportDbContext> options) : base(options) { }

    // ── Core ─────────────────────────────────────────────────────
    public DbSet<TransportRequest> TransportRequests { get; set; } = null!;
    public DbSet<TransportRequestDetail> TransportRequestDetails { get; set; } = null!;
    public DbSet<TransportVehicle> TransportVehicles { get; set; } = null!;
    public DbSet<VehicleRate> VehicleRates { get; set; } = null!;
    public DbSet<VehicleFundRequest> VehicleFundRequests { get; set; } = null!;
    public DbSet<TransportMovement> TransportMovements { get; set; } = null!;
    public DbSet<TransportDelivery> TransportDeliveries { get; set; } = null!;
    public DbSet<TransportDocument> TransportDocuments { get; set; } = null!;
    public DbSet<TransportExpense> TransportExpenses { get; set; } = null!;

    // ── Transporter Master ──────────────────────────────────────
    public DbSet<Transporter> Transporters { get; set; } = null!;
    public DbSet<TransporterKYC> TransporterKYCs { get; set; } = null!;
    public DbSet<TransporterBank> TransporterBanks { get; set; } = null!;
    public DbSet<TransporterNotification> TransporterNotifications { get; set; } = null!;

    // ── Consolidation ───────────────────────────────────────────
    public DbSet<ConsolidatedTrip> ConsolidatedTrips { get; set; } = null!;
    public DbSet<ConsolidatedVehicle> ConsolidatedVehicles { get; set; } = null!;
    public DbSet<ConsolidatedExpense> ConsolidatedExpenses { get; set; } = null!;
    public DbSet<ConsolidatedStopDelivery> ConsolidatedStopDeliveries { get; set; } = null!;

    // ── Fleet Vehicle Master ────────────────────────────────────
    public DbSet<FleetVehicle> FleetVehicles { get; set; } = null!;
    public DbSet<VehicleDriver> VehicleDrivers { get; set; } = null!;
    public DbSet<VehicleDailyStatus> VehicleDailyStatuses { get; set; } = null!;
    public DbSet<VehicleTravelLog> VehicleTravelLogs { get; set; } = null!;

    // ── Maintenance ─────────────────────────────────────────────
    public DbSet<MaintenanceWorkOrder> MaintenanceWorkOrders { get; set; } = null!;
    public DbSet<MaintenancePart> MaintenanceParts { get; set; } = null!;
    public DbSet<MaintenanceDocument> MaintenanceDocuments { get; set; } = null!;

    // ── Legacy Gap Entities ─────────────────────────────────────
    public DbSet<VehicleDailyExpense> VehicleDailyExpenses { get; set; } = null!;
    public DbSet<StampDuty> StampDuties { get; set; } = null!;
    public DbSet<TransitWarehouse> TransitWarehouses { get; set; } = null!;
    public DbSet<ExpenseApproval> ExpenseApprovals { get; set; } = null!;
    public DbSet<PaymentVoucher> PaymentVouchers { get; set; } = null!;

    // ── Lookup / Master Tables ──────────────────────────────────
    public DbSet<TransportLookup> TransportLookups { get; set; } = null!;

    // ── ULIP Integration ────────────────────────────────────────
    public DbSet<VehicleDetail> VehicleDetails { get; set; } = null!;
    public DbSet<DriverLicenseDetail> DriverLicenseDetails { get; set; } = null!;
    public DbSet<FASTagTransaction> FASTagTransactions { get; set; } = null!;
    public DbSet<TollPlaza> TollPlazas { get; set; } = null!;
    public DbSet<EWayBill> EWayBills { get; set; } = null!;

    // ── CharteredInfo (e-Invoice / GST) ─────────────────────────
    public DbSet<EInvoice> EInvoices { get; set; } = null!;
    public DbSet<GstDetail> GstDetails { get; set; } = null!;

    // ── Dynamic Fields (Workflow-driven EAV) ────────────────────
    public DbSet<DynamicFieldDetail> DynamicFieldDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransportDbContext).Assembly);

        // SharedLibrary: global soft-delete filter + audit log table
        modelBuilder.ApplySoftDeleteFilter();
        modelBuilder.ConfigureAuditLog();
    }
}
