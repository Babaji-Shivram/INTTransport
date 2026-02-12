using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Transport.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConsolidatedSubEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsolidatedExpenses",
                columns: table => new
                {
                    ConsolidatedExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ConsolidatedTripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: "INR"),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceiptUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsolidatedExpenses", x => x.ConsolidatedExpenseId);
                    table.ForeignKey(
                        name: "FK_ConsolidatedExpenses_ConsolidatedTrips_ConsolidatedTripId",
                        column: x => x.ConsolidatedTripId,
                        principalTable: "ConsolidatedTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsolidatedStopDeliveries",
                columns: table => new
                {
                    ConsolidatedStopDeliveryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ConsolidatedTripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StopSequence = table.Column<int>(type: "int", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Pincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EstimatedArrival = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualArrival = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryStatus = table.Column<int>(type: "int", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PODNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PODDocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsolidatedStopDeliveries", x => x.ConsolidatedStopDeliveryId);
                    table.ForeignKey(
                        name: "FK_ConsolidatedStopDeliveries_ConsolidatedTrips_ConsolidatedTripId",
                        column: x => x.ConsolidatedTripId,
                        principalTable: "ConsolidatedTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsolidatedStopDeliveries_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsolidatedVehicles",
                columns: table => new
                {
                    ConsolidatedVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ConsolidatedTripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransporterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VehicleNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DriverPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FreightRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TollCharges = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OtherCharges = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: "INR"),
                    LRNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemoCopyUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsolidatedVehicles", x => x.ConsolidatedVehicleId);
                    table.ForeignKey(
                        name: "FK_ConsolidatedVehicles_ConsolidatedTrips_ConsolidatedTripId",
                        column: x => x.ConsolidatedTripId,
                        principalTable: "ConsolidatedTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsolidatedVehicles_Transporters_TransporterId",
                        column: x => x.TransporterId,
                        principalTable: "Transporters",
                        principalColumn: "TransporterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedExpenses_ConsolidatedTripId",
                table: "ConsolidatedExpenses",
                column: "ConsolidatedTripId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedExpenses_CreatedDate",
                table: "ConsolidatedExpenses",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedExpenses_ExpenseDate",
                table: "ConsolidatedExpenses",
                column: "ExpenseDate");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedExpenses_IsDeleted",
                table: "ConsolidatedExpenses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedStopDeliveries_ConsolidatedTripId",
                table: "ConsolidatedStopDeliveries",
                column: "ConsolidatedTripId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedStopDeliveries_ConsolidatedTripId_StopSequence",
                table: "ConsolidatedStopDeliveries",
                columns: new[] { "ConsolidatedTripId", "StopSequence" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedStopDeliveries_CreatedDate",
                table: "ConsolidatedStopDeliveries",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedStopDeliveries_IsDeleted",
                table: "ConsolidatedStopDeliveries",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedStopDeliveries_TransportRequestId",
                table: "ConsolidatedStopDeliveries",
                column: "TransportRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedVehicles_ConsolidatedTripId",
                table: "ConsolidatedVehicles",
                column: "ConsolidatedTripId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedVehicles_CreatedDate",
                table: "ConsolidatedVehicles",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedVehicles_IsDeleted",
                table: "ConsolidatedVehicles",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedVehicles_TransporterId",
                table: "ConsolidatedVehicles",
                column: "TransporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedVehicles_VehicleNumber",
                table: "ConsolidatedVehicles",
                column: "VehicleNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsolidatedExpenses");

            migrationBuilder.DropTable(
                name: "ConsolidatedStopDeliveries");

            migrationBuilder.DropTable(
                name: "ConsolidatedVehicles");
        }
    }
}
