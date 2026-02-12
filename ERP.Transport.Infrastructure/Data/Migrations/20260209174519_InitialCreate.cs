using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Transport.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DriverLicenseDetails",
                columns: table => new
                {
                    DriverLicenseDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    LicenseNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    HolderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FatherOrHusbandName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PinCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BloodGroup = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    IssuingAuthority = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VehicleClassesAuthorized = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HasHazardousGoodsEndorsement = table.Column<bool>(type: "bit", nullable: true),
                    HasHillDrivingEndorsement = table.Column<bool>(type: "bit", nullable: true),
                    LicenseStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: true),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: true),
                    LastFetchedFromUlip = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    RawApiResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_DriverLicenseDetails", x => x.DriverLicenseDetailId);
                });

            migrationBuilder.CreateTable(
                name: "GstDetails",
                columns: table => new
                {
                    GstDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Gstin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TradeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BusinessType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GstinStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CancellationDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StateCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    StateName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Pincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NatureOfBusiness = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ConstitutionOfBusiness = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TaxpayerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsEInvoiceApplicable = table.Column<bool>(type: "bit", nullable: true),
                    IsEWayBillApplicable = table.Column<bool>(type: "bit", nullable: true),
                    LastFiledReturn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastFiledReturnDate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LastFetchedFromApi = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    RawApiResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_GstDetails", x => x.GstDetailId);
                });

            migrationBuilder.CreateTable(
                name: "TollPlazas",
                columns: table => new
                {
                    TollPlazaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    PlazaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlazaName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Highway = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Direction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    SingleJourneyRate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ReturnJourneyRate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MonthlyPassRate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    VehicleClassApplicable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastFetchedFromUlip = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    RawApiResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_TollPlazas", x => x.TollPlazaId);
                });

            migrationBuilder.CreateTable(
                name: "Transporters",
                columns: table => new
                {
                    TransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransporterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PANNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    GSTNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Pincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SuspensionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuspensionDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(3,1)", nullable: false, defaultValue: 0m),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_Transporters", x => x.TransporterId);
                });

            migrationBuilder.CreateTable(
                name: "TransportRequests",
                columns: table => new
                {
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    RequestNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    SourceReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GSTNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OriginLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OriginLocationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PickupAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PickupCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PickupState = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PickupPincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DestinationLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DestinationLocationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DropAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DropCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DropState = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DropPincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CargoType = table.Column<int>(type: "int", nullable: false),
                    CargoDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GrossWeightKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfPackages = table.Column<int>(type: "int", nullable: false),
                    Container20Count = table.Column<int>(type: "int", nullable: false),
                    Container40Count = table.Column<int>(type: "int", nullable: false),
                    VehicleTypeRequired = table.Column<int>(type: "int", nullable: false),
                    DeliveryType = table.Column<int>(type: "int", nullable: false),
                    RequiredDeliveryDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Division = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Plant = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkflowStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WorkflowStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConsolidatedTripId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsConsolidated = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_TransportRequests", x => x.TransportRequestId);
                });

            migrationBuilder.CreateTable(
                name: "VehicleDetails",
                columns: table => new
                {
                    VehicleDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    VehicleNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PresentAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PermanentAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VehicleClass = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VehicleCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MakerModel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MakerDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BodyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FuelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ManufacturingYear = table.Column<int>(type: "int", nullable: true),
                    SeatingCapacity = table.Column<int>(type: "int", nullable: true),
                    GrossVehicleWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    UnladenWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    NumberOfCylinders = table.Column<int>(type: "int", nullable: true),
                    EngineNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChassisNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegisteringAuthority = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RegistrationState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    FitnessUpto = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    InsuranceUpto = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    InsuranceCompany = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InsurancePolicyNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PucValidUpto = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TaxValidUpto = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    PermitValidUpto = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    PermitType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NationalPermitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NationalPermitUpto = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    VehicleStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsBlacklisted = table.Column<bool>(type: "bit", nullable: true),
                    IsFinanced = table.Column<bool>(type: "bit", nullable: true),
                    FinancerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastFetchedFromUlip = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    RawApiResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_VehicleDetails", x => x.VehicleDetailId);
                });

            migrationBuilder.CreateTable(
                name: "TransporterBanks",
                columns: table => new
                {
                    TransporterBankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IFSCCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AccountHolderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_TransporterBanks", x => x.TransporterBankId);
                    table.ForeignKey(
                        name: "FK_TransporterBanks_Transporters_TransporterId",
                        column: x => x.TransporterId,
                        principalTable: "Transporters",
                        principalColumn: "TransporterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransporterKYCs",
                columns: table => new
                {
                    TransporterKYCId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedByName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
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
                    table.PrimaryKey("PK_TransporterKYCs", x => x.TransporterKYCId);
                    table.ForeignKey(
                        name: "FK_TransporterKYCs_Transporters_TransporterId",
                        column: x => x.TransporterId,
                        principalTable: "Transporters",
                        principalColumn: "TransporterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransporterNotifications",
                columns: table => new
                {
                    TransporterNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
                    table.PrimaryKey("PK_TransporterNotifications", x => x.TransporterNotificationId);
                    table.ForeignKey(
                        name: "FK_TransporterNotifications_Transporters_TransporterId",
                        column: x => x.TransporterId,
                        principalTable: "Transporters",
                        principalColumn: "TransporterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EInvoices",
                columns: table => new
                {
                    EInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Irn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AckNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AckDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    SignedInvoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignedQrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QrCodeImageBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EInvoiceStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    SellerGstin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SellerLegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SellerTradeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BuyerGstin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BuyerLegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BuyerTradeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TotalAssessableValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalCgstValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalSgstValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalIgstValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalCessValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalInvoiceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EwbNumber = table.Column<long>(type: "bigint", nullable: true),
                    EwbDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    EwbValidTill = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CancelRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancelledDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    RawRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_EInvoices", x => x.EInvoiceId);
                    table.ForeignKey(
                        name: "FK_EInvoices_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EWayBills",
                columns: table => new
                {
                    EWayBillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EWayBillNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    ValidUpto = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    EWayBillStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SupplierGstin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RecipientGstin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecipientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    HsnCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CgstAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SgstAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IgstAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalInvoiceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransporterGstin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TransporterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransportMode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VehicleNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VehicleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApproximateDistanceKm = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    FromState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FromPlace = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FromPincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ToState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ToPlace = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ToPincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    LastFetchedFromApi = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    RawApiResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_EWayBills", x => x.EWayBillId);
                    table.ForeignKey(
                        name: "FK_EWayBills_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FASTagTransactions",
                columns: table => new
                {
                    FASTagTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    VehicleNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TagId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TollPlazaName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TollPlazaId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LaneDirection = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TransactionDateTime = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TransactionAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    PlazaState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PlazaHighway = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    VehicleClassAtToll = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FetchedFromUlip = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    RawApiResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_FASTagTransactions", x => x.FASTagTransactionId);
                    table.ForeignKey(
                        name: "FK_FASTagTransactions_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TransportDeliveries",
                columns: table => new
                {
                    TransportDeliveryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PODNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PODDocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChallanNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChallanDocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LRCopyUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EWayBillNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DeliveryStatus = table.Column<int>(type: "int", nullable: false),
                    DamageNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ShortDeliveryNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_TransportDeliveries", x => x.TransportDeliveryId);
                    table.ForeignKey(
                        name: "FK_TransportDeliveries_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransportDocuments",
                columns: table => new
                {
                    TransportDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransportVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_TransportDocuments", x => x.TransportDocumentId);
                    table.ForeignKey(
                        name: "FK_TransportDocuments_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransportExpenses",
                columns: table => new
                {
                    TransportExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransportVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    CategoryDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "INR"),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceiptUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_TransportExpenses", x => x.TransportExpenseId);
                    table.ForeignKey(
                        name: "FK_TransportExpenses_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransportMovements",
                columns: table => new
                {
                    TransportMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Milestone = table.Column<int>(type: "int", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransportVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_TransportMovements", x => x.TransportMovementId);
                    table.ForeignKey(
                        name: "FK_TransportMovements_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransportRequestDetails",
                columns: table => new
                {
                    TransportRequestDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FieldLabel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FieldValue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    WorkflowStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_TransportRequestDetails", x => x.TransportRequestDetailId);
                    table.ForeignKey(
                        name: "FK_TransportRequestDetails_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransportVehicles",
                columns: table => new
                {
                    TransportVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransporterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VehicleNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DriverPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LRNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LRDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    MemoCopyUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
                    table.PrimaryKey("PK_TransportVehicles", x => x.TransportVehicleId);
                    table.ForeignKey(
                        name: "FK_TransportVehicles_TransportRequests_TransportRequestId",
                        column: x => x.TransportRequestId,
                        principalTable: "TransportRequests",
                        principalColumn: "TransportRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransportVehicles_Transporters_TransporterId",
                        column: x => x.TransporterId,
                        principalTable: "Transporters",
                        principalColumn: "TransporterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleFundRequests",
                columns: table => new
                {
                    VehicleFundRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "INR"),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IFSCCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProcessedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
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
                    table.PrimaryKey("PK_VehicleFundRequests", x => x.VehicleFundRequestId);
                    table.ForeignKey(
                        name: "FK_VehicleFundRequests_TransportVehicles_TransportVehicleId",
                        column: x => x.TransportVehicleId,
                        principalTable: "TransportVehicles",
                        principalColumn: "TransportVehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleRates",
                columns: table => new
                {
                    VehicleRateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TransportVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FreightRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DetentionCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VaraiCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmptyContainerReturn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TollCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "INR"),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
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
                    table.PrimaryKey("PK_VehicleRates", x => x.VehicleRateId);
                    table.ForeignKey(
                        name: "FK_VehicleRates_TransportVehicles_TransportVehicleId",
                        column: x => x.TransportVehicleId,
                        principalTable: "TransportVehicles",
                        principalColumn: "TransportVehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverLicenseDetails_CreatedDate",
                table: "DriverLicenseDetails",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DriverLicenseDetails_IsDeleted",
                table: "DriverLicenseDetails",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DriverLicenseDetails_LicenseNumber",
                table: "DriverLicenseDetails",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EInvoices_CreatedDate",
                table: "EInvoices",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoices_DocumentNumber",
                table: "EInvoices",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoices_EwbNumber",
                table: "EInvoices",
                column: "EwbNumber");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoices_Irn",
                table: "EInvoices",
                column: "Irn");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoices_IsDeleted",
                table: "EInvoices",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoices_TransportRequestId",
                table: "EInvoices",
                column: "TransportRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_EWayBills_CreatedDate",
                table: "EWayBills",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EWayBills_EWayBillNumber",
                table: "EWayBills",
                column: "EWayBillNumber");

            migrationBuilder.CreateIndex(
                name: "IX_EWayBills_IsDeleted",
                table: "EWayBills",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EWayBills_TransportRequestId",
                table: "EWayBills",
                column: "TransportRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FASTagTransactions_CreatedDate",
                table: "FASTagTransactions",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_FASTagTransactions_IsDeleted",
                table: "FASTagTransactions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FASTagTransactions_TransactionDateTime",
                table: "FASTagTransactions",
                column: "TransactionDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_FASTagTransactions_TransportRequestId",
                table: "FASTagTransactions",
                column: "TransportRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FASTagTransactions_VehicleNumber",
                table: "FASTagTransactions",
                column: "VehicleNumber");

            migrationBuilder.CreateIndex(
                name: "IX_GstDetails_CreatedDate",
                table: "GstDetails",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_GstDetails_Gstin",
                table: "GstDetails",
                column: "Gstin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GstDetails_IsDeleted",
                table: "GstDetails",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TollPlazas_CreatedDate",
                table: "TollPlazas",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TollPlazas_IsDeleted",
                table: "TollPlazas",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TollPlazas_PlazaCode",
                table: "TollPlazas",
                column: "PlazaCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportDeliveries_CreatedDate",
                table: "TransportDeliveries",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransportDeliveries_IsDeleted",
                table: "TransportDeliveries",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransportDeliveries_TransportRequestId",
                table: "TransportDeliveries",
                column: "TransportRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportDocuments_CreatedDate",
                table: "TransportDocuments",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransportDocuments_IsDeleted",
                table: "TransportDocuments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransportDocuments_TransportRequestId",
                table: "TransportDocuments",
                column: "TransportRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterBanks_CreatedDate",
                table: "TransporterBanks",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterBanks_IsDeleted",
                table: "TransporterBanks",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterBanks_TransporterId",
                table: "TransporterBanks",
                column: "TransporterId");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterKYCs_CreatedDate",
                table: "TransporterKYCs",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterKYCs_IsDeleted",
                table: "TransporterKYCs",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterKYCs_TransporterId",
                table: "TransporterKYCs",
                column: "TransporterId");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterNotifications_CreatedDate",
                table: "TransporterNotifications",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterNotifications_IsDeleted",
                table: "TransporterNotifications",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransporterNotifications_TransporterId",
                table: "TransporterNotifications",
                column: "TransporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Transporters_BranchId",
                table: "Transporters",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Transporters_CreatedDate",
                table: "Transporters",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transporters_IsDeleted",
                table: "Transporters",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Transporters_Status",
                table: "Transporters",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Transporters_TransporterName",
                table: "Transporters",
                column: "TransporterName");

            migrationBuilder.CreateIndex(
                name: "IX_TransportExpenses_CreatedDate",
                table: "TransportExpenses",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransportExpenses_IsDeleted",
                table: "TransportExpenses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransportExpenses_TransportRequestId",
                table: "TransportExpenses",
                column: "TransportRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportExpenses_TransportVehicleId",
                table: "TransportExpenses",
                column: "TransportVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportMovements_CreatedDate",
                table: "TransportMovements",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransportMovements_IsDeleted",
                table: "TransportMovements",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransportMovements_Timestamp",
                table: "TransportMovements",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_TransportMovements_TransportRequestId",
                table: "TransportMovements",
                column: "TransportRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequestDetails_CreatedDate",
                table: "TransportRequestDetails",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequestDetails_IsDeleted",
                table: "TransportRequestDetails",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequestDetails_TransportRequestId_FieldKey",
                table: "TransportRequestDetails",
                columns: new[] { "TransportRequestId", "FieldKey" });

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_BranchId",
                table: "TransportRequests",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_ConsolidatedTripId",
                table: "TransportRequests",
                column: "ConsolidatedTripId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_CountryCode",
                table: "TransportRequests",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_CreatedDate",
                table: "TransportRequests",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_CustomerId",
                table: "TransportRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_IsDeleted",
                table: "TransportRequests",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_RequestDate",
                table: "TransportRequests",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_RequestNumber",
                table: "TransportRequests",
                column: "RequestNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_SourceReferenceId",
                table: "TransportRequests",
                column: "SourceReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_Status",
                table: "TransportRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TransportVehicles_CreatedDate",
                table: "TransportVehicles",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransportVehicles_IsDeleted",
                table: "TransportVehicles",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransportVehicles_TransporterId",
                table: "TransportVehicles",
                column: "TransporterId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportVehicles_TransportRequestId",
                table: "TransportVehicles",
                column: "TransportRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDetails_CreatedDate",
                table: "VehicleDetails",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDetails_IsDeleted",
                table: "VehicleDetails",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDetails_VehicleNumber",
                table: "VehicleDetails",
                column: "VehicleNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFundRequests_CreatedDate",
                table: "VehicleFundRequests",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFundRequests_IsDeleted",
                table: "VehicleFundRequests",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFundRequests_TransportVehicleId",
                table: "VehicleFundRequests",
                column: "TransportVehicleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRates_CreatedDate",
                table: "VehicleRates",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRates_IsDeleted",
                table: "VehicleRates",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRates_TransportVehicleId",
                table: "VehicleRates",
                column: "TransportVehicleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverLicenseDetails");

            migrationBuilder.DropTable(
                name: "EInvoices");

            migrationBuilder.DropTable(
                name: "EWayBills");

            migrationBuilder.DropTable(
                name: "FASTagTransactions");

            migrationBuilder.DropTable(
                name: "GstDetails");

            migrationBuilder.DropTable(
                name: "TollPlazas");

            migrationBuilder.DropTable(
                name: "TransportDeliveries");

            migrationBuilder.DropTable(
                name: "TransportDocuments");

            migrationBuilder.DropTable(
                name: "TransporterBanks");

            migrationBuilder.DropTable(
                name: "TransporterKYCs");

            migrationBuilder.DropTable(
                name: "TransporterNotifications");

            migrationBuilder.DropTable(
                name: "TransportExpenses");

            migrationBuilder.DropTable(
                name: "TransportMovements");

            migrationBuilder.DropTable(
                name: "TransportRequestDetails");

            migrationBuilder.DropTable(
                name: "VehicleDetails");

            migrationBuilder.DropTable(
                name: "VehicleFundRequests");

            migrationBuilder.DropTable(
                name: "VehicleRates");

            migrationBuilder.DropTable(
                name: "TransportVehicles");

            migrationBuilder.DropTable(
                name: "TransportRequests");

            migrationBuilder.DropTable(
                name: "Transporters");
        }
    }
}
