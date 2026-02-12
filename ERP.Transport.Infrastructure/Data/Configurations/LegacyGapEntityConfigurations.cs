using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

// ═══════════════════════════════════════════════════════════════
//  Legacy Gap Entity Configurations
//  Vehicle Daily Expense, Stamp Duty, Transit Warehouse,
//  Expense Approval, Payment Voucher
// ═══════════════════════════════════════════════════════════════

public class VehicleDailyExpenseConfiguration : IEntityTypeConfiguration<VehicleDailyExpense>
{
    public void Configure(EntityTypeBuilder<VehicleDailyExpense> builder)
    {
        builder.ToTable("VehicleDailyExpenses");

        builder.HasIndex(e => new { e.FleetVehicleId, e.ExpenseDate })
            .HasDatabaseName("IX_VehicleDailyExpenses_Vehicle_Date");

        builder.HasIndex(e => e.ExpenseDate)
            .HasDatabaseName("IX_VehicleDailyExpenses_Date");

        builder.Property(e => e.Fuel).HasColumnType("decimal(18,2)");
        builder.Property(e => e.FuelLitres).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Fuel2).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Fuel2Litres).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TollCharges).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Fines).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Xerox).HasColumnType("decimal(18,2)");
        builder.Property(e => e.VaraiUnloading).HasColumnType("decimal(18,2)");
        builder.Property(e => e.EmptyContainer).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Parking).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Garage).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Bhatta).HasColumnType("decimal(18,2)");
        builder.Property(e => e.ODCOverweight).HasColumnType("decimal(18,2)");
        builder.Property(e => e.OtherCharges).HasColumnType("decimal(18,2)");
        builder.Property(e => e.DamageContainer).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CurrencyCode).HasMaxLength(5);
        builder.Property(e => e.Remarks).HasMaxLength(500);

        builder.HasOne(e => e.FleetVehicle)
            .WithMany(v => v.DailyExpenses)
            .HasForeignKey(e => e.FleetVehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.TransportRequest)
            .WithMany()
            .HasForeignKey(e => e.TransportRequestId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class StampDutyConfiguration : IEntityTypeConfiguration<StampDuty>
{
    public void Configure(EntityTypeBuilder<StampDuty> builder)
    {
        builder.ToTable("StampDuties");

        builder.HasIndex(e => e.ReferenceNumber).IsUnique()
            .HasDatabaseName("IX_StampDuties_RefNumber");

        builder.HasIndex(e => e.TransportRequestId)
            .HasDatabaseName("IX_StampDuties_JobId");

        builder.Property(e => e.ReferenceNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.DocumentType).HasMaxLength(100);
        builder.Property(e => e.StampDutyAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.StateCode).HasMaxLength(10);
        builder.Property(e => e.ReceiptNumber).HasMaxLength(100);
        builder.Property(e => e.ReceiptDocumentUrl).HasMaxLength(500);
        builder.Property(e => e.Remarks).HasMaxLength(500);
        builder.Property(e => e.PaidByName).HasMaxLength(200);
        builder.Property(e => e.CurrencyCode).HasMaxLength(5);
        builder.Property(e => e.CountryCode).HasMaxLength(5);

        builder.HasOne(e => e.TransportRequest)
            .WithMany(r => r.StampDuties)
            .HasForeignKey(e => e.TransportRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Transporter)
            .WithMany()
            .HasForeignKey(e => e.TransporterId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class TransitWarehouseConfiguration : IEntityTypeConfiguration<TransitWarehouse>
{
    public void Configure(EntityTypeBuilder<TransitWarehouse> builder)
    {
        builder.ToTable("TransitWarehouses");

        builder.HasIndex(e => e.TransportRequestId)
            .HasDatabaseName("IX_TransitWarehouses_JobId");

        builder.HasIndex(e => new { e.TransportRequestId, e.IsDispatched })
            .HasDatabaseName("IX_TransitWarehouses_Job_Status");

        builder.Property(e => e.WarehouseName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.WarehouseAddress).HasMaxLength(500);
        builder.Property(e => e.WarehouseCity).HasMaxLength(100);
        builder.Property(e => e.WarehouseState).HasMaxLength(100);
        builder.Property(e => e.WarehousePincode).HasMaxLength(10);
        builder.Property(e => e.ArrivalRemarks).HasMaxLength(500);
        builder.Property(e => e.ReceivedBy).HasMaxLength(200);
        builder.Property(e => e.DepartureRemarks).HasMaxLength(500);
        builder.Property(e => e.DispatchedBy).HasMaxLength(200);
        builder.Property(e => e.ContainerId).HasMaxLength(50);
        builder.Property(e => e.ContainerSealNumber).HasMaxLength(50);

        builder.HasOne(e => e.TransportRequest)
            .WithMany(r => r.TransitWarehouses)
            .HasForeignKey(e => e.TransportRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.TransportVehicle)
            .WithMany()
            .HasForeignKey(e => e.TransportVehicleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ExpenseApprovalConfiguration : IEntityTypeConfiguration<ExpenseApproval>
{
    public void Configure(EntityTypeBuilder<ExpenseApproval> builder)
    {
        builder.ToTable("ExpenseApprovals");

        builder.HasIndex(e => e.TransportExpenseId)
            .HasDatabaseName("IX_ExpenseApprovals_ExpenseId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_ExpenseApprovals_Status");

        builder.Property(e => e.RequestedAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.ApprovedAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Remarks).HasMaxLength(500);
        builder.Property(e => e.RejectionReason).HasMaxLength(500);
        builder.Property(e => e.ApproverName).HasMaxLength(200);
        builder.Property(e => e.ApprovalRole).HasMaxLength(100);

        builder.HasOne(e => e.TransportExpense)
            .WithMany(exp => exp.Approvals)
            .HasForeignKey(e => e.TransportExpenseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PaymentVoucherConfiguration : IEntityTypeConfiguration<PaymentVoucher>
{
    public void Configure(EntityTypeBuilder<PaymentVoucher> builder)
    {
        builder.ToTable("PaymentVouchers");

        builder.HasIndex(e => e.VoucherNumber).IsUnique()
            .HasDatabaseName("IX_PaymentVouchers_Number");

        builder.HasIndex(e => e.VoucherDate)
            .HasDatabaseName("IX_PaymentVouchers_Date");

        builder.Property(e => e.VoucherNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PaidTo).HasMaxLength(300).IsRequired();
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.AmountInWords).HasMaxLength(500);
        builder.Property(e => e.CurrencyCode).HasMaxLength(5);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.BankName).HasMaxLength(200);
        builder.Property(e => e.ChequeNumber).HasMaxLength(50);
        builder.Property(e => e.TransactionReference).HasMaxLength(100);
        builder.Property(e => e.AccountNumber).HasMaxLength(50);
        builder.Property(e => e.IFSCCode).HasMaxLength(20);
        builder.Property(e => e.BillNumber).HasMaxLength(50);
        builder.Property(e => e.PdfUrl).HasMaxLength(500);
        builder.Property(e => e.CountryCode).HasMaxLength(5);

        builder.HasOne(e => e.TransportExpense)
            .WithMany()
            .HasForeignKey(e => e.TransportExpenseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.MaintenanceWorkOrder)
            .WithMany()
            .HasForeignKey(e => e.MaintenanceWorkOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.VehicleDailyExpense)
            .WithMany()
            .HasForeignKey(e => e.VehicleDailyExpenseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
