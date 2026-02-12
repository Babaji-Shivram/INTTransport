# Legacy Transport Application — Comprehensive Feature Map

> **Generated**: Audit of ASP.NET WebForms app at `Transport\Transport\Transport\`  
> **Compared against**: New microservice at `d:\INTTransport`  
> **Total legacy files**: ~160 .aspx.cs code-behind files  
> **Database layer**: `DBOperations.cs` (21,979 lines) — all stored procedures via static methods  

---

## Executive Summary

The legacy transport application is a monolithic ASP.NET WebForms system covering **14 functional areas**. The new microservice (`INTTransport`) covers the **core job lifecycle** well (create → receive → assign vehicle → rate → movement → delivery → clearance) plus fleet management, maintenance, transporters, ULIP integration, and reports. However, **several major functional areas have NO equivalent in the new MS**:

| Gap Area | Business Impact | Priority |
|----------|----------------|----------|
| **Transport Billing (entire workflow)** | Revenue-critical — bill submission, approval, rejection, tracking, history | 🔴 Critical |
| **Vehicle Daily Expense (15-category)** | Cost tracking per vehicle per day — fuel, tolls, fines, etc. | 🔴 Critical |
| **Warehouse Delivery workflow** | Transit warehouse → complex delivery form with Indian compliance fields | 🟡 High |
| **PDF Voucher Generation** | Cash/Cheque/NEFT/RTGS voucher printing for expenses | 🟡 High |
| **Stamp Duty Management** | Regulatory compliance for stamp duty tracking | 🟡 High |
| **Crystal Reports / Trip Detail** | TripDetail.rpt — formatted printable trip reports | 🟢 Medium |
| **DSR with Module Filtering** | Daily Status Report — cross-module report with checkbox filtering | 🟢 Medium |
| **Account Expense / Fund Request Integration** | Cross-module integration with Accounts for payment requests | 🟡 High |

---

## 1. TRANSPORT BILLING WORKFLOW

### Status: 🔴 NOT IN NEW MS — Entire subsystem missing

The legacy app has **16+ pages** dedicated to transport billing. The new MS stops at "cleared — ready for billing" and has no billing entities, controllers, or services.

### Legacy Pages (16 files)

| Page | Purpose | Key SPs / Methods |
|------|---------|-------------------|
| **TransBill.aspx** | Bill listing — tabs for Normal / Consolidate jobs | SqlDataSource-driven; navigates to TestBillDetail or ConsolidateBill |
| **TransBillDetail.aspx** | Submit bill against truck request | `AddTransBillDetail`, `AddTransportRate` |
| **TestBillDetail.aspx** | Alternate bill detail entry | `GetTransportRequestDetail`, bill amount fields |
| **TransBillApproval.aspx** | Pending bill approval queue | Normal + Consolidate tabs, navigates to ApproveBill |
| **ApproveBill.aspx** | Approve/reject bill with amount modification | `AddTransApproveRejectBill`, `AddTransBillApprovalHistory`, `AddBillReceivedDetail`, `GetPackingListDocs` |
| **TransBillConsolidate.aspx** | Consolidated bill management | Consolidate-specific billing flow |
| **TransBillRejected.aspx** | Rejected bill listing | View/re-submit rejected bills |
| **TransBillTracking.aspx** (1623 lines) | Full bill tracking with rate edit, fund request, daily status email, selling details | `GetTransportRequestDetail`, `AddTransportRateDetail`, fund requests, document management |
| **BillHistory.aspx** | Historical bill listing — Normal + Consolidate | Navigate to TransBillTracking with TRId/TransporterId |
| **BillStatus.aspx** | Bill status overview | Status tracking grid |
| **BillTracking.aspx** | Bill lifecycle tracking | Approval/rejection history |
| **BillRejectedDetail.aspx** | Rejected bill detail view | Rejection remarks, resubmission |
| **FinalApprovedBills.aspx** | Final approved bills listing | End-state bill view |
| **ApprovedBillByDept.aspx** | Department-wise approved bills | Department filtering |
| **VehicleBillDetails.aspx** | Per-vehicle bill breakdowns | Vehicle-level billing |
| **VehicleBillDetailsFinal.aspx** | Final vehicle bill view | Read-only approved view |
| **VehicleBillDetails_Chr.aspx** | Chartered vehicle bill details | Separate billing for chartered |
| **ViewTransporterBill.aspx** | Transporter's view of their bills | External-facing bill details |
| **TransportBillDetail.aspx** | Transport bill detail variant | Bill detail form |
| **InvoicePending.aspx** | Invoice pending report | Simple grid + Excel export |
| **TestBilling.aspx** | Test/alternate billing flow | Development variant |

### Bill Data Model (from TransBillDetail.aspx.cs)
```
- BillNumber, BillAmount, DetentionAmount, VaraiAmount
- EmptyContRcptCharges, TotalAmount, BillPersonName, BillSubmitDate
- Per-vehicle rate: FreightAmount, DetentionCharges, VaraiExp, TollCharges, OtherCharges
```

### Approval Workflow (from ApproveBill.aspx.cs)
```
Submit Bill → Pending Approval → Approve (with modified amount) / Reject (with remarks)
                                  ↓                                    ↓
                          AddTransBillApprovalHistory           Rejection history
                          AddBillReceivedDetail                Re-submit flow
                                  ↓
                          Final Approved Bills
```

### What's Needed in New MS
- **BillingController** / **BillingService** — full CRUD for transport bills
- Bill entity: `TransportBill { BillNumber, BillAmount, DetentionAmount, VaraiAmount, Status, ... }`
- Bill approval entity: `TransportBillApproval { BillId, ApprovedAmount, Remarks, Status }`
- Workflow integration for multi-level bill approval / rejection
- Packing list document management (ZIP download support)

---

## 2. VEHICLE DAILY EXPENSE TRACKING

### Status: 🔴 NOT IN NEW MS

The new MS has `TransportExpense` (per-job expenses) and `VehicleFundRequest`, but NOT the **per-vehicle daily expense grid** with 15 fixed categories.

### Legacy: TransDailyExpense.aspx.cs

**Purpose**: Record daily operating expenses per vehicle across 15 categories.

| Expense Category | Field Name |
|-----------------|------------|
| Fuel (Primary) | Fuel |
| Fuel (Secondary) | Fuel2 |
| Fuel Liters (Primary) | FuelLiter |
| Fuel Liters (Secondary) | Fuel2Liter |
| Toll Charges | TollCharges |
| Fines (without Cleaner) | FineWithoutCleaner |
| Xerox | Xerox |
| Varai / Unloading | VaraiUnloading |
| Empty Container | EmptyContainer |
| Parking | Parking |
| Garage | Garage |
| Bhatta | Bhatta |
| ODC / Overweight | ODCOverweight |
| Other Charges | OtherCharges |
| Damage Container | DamageContainer |

**Key SP**: `AddVehicleDailyExpense`  
**Business Rule**: Date-limited to 30 days back — cannot enter expenses for older dates  
**Exports**: Daily and Monthly Excel exports

### What's Needed in New MS
- `VehicleDailyExpense` entity with all 15 categories
- API endpoint: `POST /fleet-vehicles/{id}/daily-expenses`
- Date validation (rolling 30-day window)
- Aggregation reports by vehicle, by category, by date range

---

## 3. WAREHOUSE / DELIVERY WORKFLOW

### Status: 🟡 PARTIALLY COVERED — New MS has `RecordDeliveryAsync` but lacks warehouse transit stages and Indian compliance fields

### Legacy Pages

| Page | Purpose | Key SPs |
|------|---------|---------|
| **InTransitWarehouse.aspx** | Jobs in general warehouse — "Move" to delivery | `AddTransitWarehouse` → `insJobTransitHistory` |
| **WarehouseDelivery.aspx** (775 lines) | Complex delivery form FROM warehouse | `GetJobDetailForDelivery`, `AddDeliveryWarehouse` |
| **UpdateDelivery.aspx** | Edit existing delivery details | Delivery update flow |
| **TransDeliveryDetail.aspx** | View delivery details | Read-only delivery view |
| **PendingTransDelivery.aspx** | Pending delivery listing (transport jobs) | Queue view |
| **VehicleDelivery.aspx** | Vehicle-specific delivery details | Vehicle-centric delivery |

### WarehouseDelivery.aspx — Fields NOT in New MS

These are India-specific logistics/compliance fields present in the legacy form:

```
- LRNo, LRDate                    (Lorry Receipt — Indian trucking standard)
- RoadPermitNo, RoadPermitDate    (RTO road permits)
- NFormNo, NFormDate              (N-Form for interstate movement)
- SFormNo, SFormDate              (S-Form for interstate stock transfer)
- OctroiReceiptNo, OctroiAmount   (Municipal Octroi tax — now mostly GST)
- BabajiChallanNo, BabajiChallanDate (Internal challan for own fleet "Babaji")
- EmptyContReturnDate             (Container return tracking)
- PODFile, ChallanFile, DamageFile (Document uploads)
- DeliveryPoint                   (Destination address)
- ContainerId selection            (For Sea mode — container-level tracking)
- rdlTransport (Babaji=Own / Customer) toggle
```

### Gap: Transit Warehouse Stage
Legacy has an explicit **"in warehouse"** stage between movement and delivery. The new MS goes directly from movement to delivery. Missing:
- `TransitWarehouse` entity or state
- Warehouse-to-delivery transition workflow
- Container-level tracking at warehouse stage

---

## 4. TRUCK REQUEST & VEHICLE PLACEMENT

### Status: 🟢 MOSTLY COVERED — New MS has `AssignVehicleAsync`, `EnterRateAsync`, consolidation. But legacy has more granular data.

### Legacy Pages

| Page | Purpose | Key SPs |
|------|---------|---------|
| **TruckRequest.aspx** (901 lines) | Create truck request from job | `AddJobTransportRequest`, `AddPackingListDocs`, `TR_updJobTransportBabaji` |
| **NewTruckRequest.aspx** (881 lines) | Create new truck request variant | Same SPs as TruckRequest |
| **RequestReceived.aspx** | View received truck requests | Queue listing |
| **RequestReceivedConsolidated.aspx** | Consolidated request queue | Consolidate view |
| **RequestRate.aspx** | Rate request before vehicle assign | Rate negotiation |
| **VehiclePlace.aspx** (1762 lines) | Detailed rate entry + vehicle assignment | `AddTransportRateDetail`, `AddTransporterPlaced`, `TR_AddBillingInstructions`, `GetTransporterBankDetails` |
| **VehiclePlaced.aspx** (771 lines) | Mark vehicles placed + create consolidate | `AddVehiclePlaced`, `GetConsolidateRefNo`, `AddConsolidateJob` |
| **ApprovedVehicle.aspx** | Approved vehicle assignments | Approval confirmation view |
| **TransApproval.aspx** | Transport request approval | Manager approval flow |
| **ViewRequest.aspx** | View truck request details | Read-only view |

### Fields in Legacy VehiclePlace.aspx NOT in New MS Rate Entry

| Field | Description |
|-------|-------------|
| `BillingInstruction` | Free-text billing instructions per rate | 
| `ContractPrice` | Agreed contract price |
| `SellingPrice` | Internal selling price |
| `MemoDocument` | Memo document upload |
| `Email/Contract file uploads` | Supporting document for rate approval |
| **Navbharat/NavJeevan special handling** | Hardcoded transporter IDs (524, 17304) show own fleet vehicle dropdown |
| `MarketRate` | Current market rate for comparison |
| Fund request with bank details | `GetTransporterBankDetails`, `AccountExpense.GetPaymentRequestById` |

### What's Partially Missing
- `BillingInstruction` field on rate entry
- `ContractPrice` / `SellingPrice` / `MarketRate` on rate details
- Hardcoded transporter-specific logic (should be configurable "own fleet transporter" flag)

---

## 5. CONSOLIDATION WORKFLOW

### Status: 🟢 MOSTLY COVERED — New MS has `ConsolidateJobsAsync`, `ConsolidatedTrip` entity. But legacy has more pages.

### Legacy Pages

| Page | Purpose | Key SPs |
|------|---------|---------|
| **VehiclePlaced.aspx** | Create consolidate from multiple jobs | `GetConsolidateRefNo`, `AddConsolidateJob` |
| **ConsolidateRequest.aspx** (1585 lines) | Consolidate request mgmt + rate detail | `GetConsolidateRequestById`, `UpdateTransportRateDetailForConsolidateJob` |
| **ConsolidateDispatch.aspx** | Consolidate dispatch tracking | Dispatch stage management |
| **ConsolidateVehicle.aspx** | Vehicle assignment for consolidate | Vehicle-level assignment |
| **ConsolidateTracking.aspx** | Track consolidated movements | Movement tracking |
| **ConVehicleTripDetail.aspx** | Vehicle trip detail in consolidate | Trip-level detail |
| **Clearance.aspx** (1095 lines) | Consolidate delivery detail (wizard) | `AddDeliveryConsolidateMS`, `AddDeliveryConsolidate`, delivery wizard |
| **TransClearance.aspx** (1031 lines) | Transport consolidate clearance | Similar wizard for transport-specific |
| **ConsolidateBill.aspx** | Billing for consolidated jobs | Consolidate billing flow |

### New MS Coverage
- `ConsolidateJobsAsync` — creates consolidated trip ✅
- `AssignConsolidatedVehicleAsync` ✅
- `AddConsolidatedVehicleAsync` ✅  
- `AddConsolidatedExpenseAsync` ✅
- `RecordStopDeliveryAsync` ✅
- **NOT covered**: Consolidate billing, consolidate dispatch tracking, consolidate clearance wizard

---

## 6. VESSEL / VEHICLE MAINTENANCE & EXPENSE

### Status: 🟡 PARTIALLY COVERED — New MS has `MaintenanceService` with work orders, but lacks vessel-specific features and expense voucher generation

### Legacy Pages

| Page | Purpose | Key SPs |
|------|---------|---------|
| **VesselMaintenance.aspx** | Create vessel work orders | `AddMaintenanceVessel`, `AddMaintenanceDocument`, `GetNewTransportRefNo`, `FillMaintenanceCategory` |
| **EditVesselMaintenance.aspx** | Edit vessel work orders | Work order update |
| **VesselExpense.aspx** (746 lines) | Vessel expense listing + PDF voucher | `GetWorkExpense`, `GetVehicleExpepsneByDate` |
| **VehicleMaintenance.aspx** | Vehicle maintenance entry | Vehicle-focused variant |
| **EditMaintenance.aspx** | Edit vehicle maintenance | Update flow |
| **VehicleExpense.aspx** | Vehicle expense management | Expense CRUD |
| **HOVehicleMaintenance.aspx** | HO-level maintenance view | Head Office maintenance oversight |
| **HOEditMaintenance.aspx** | HO-level maintenance edit | HO edit permissions |
| **HOVehicleExpense.aspx** | HO-level expense view | Head Office expense oversight |
| **HOVehicleLog.aspx** | HO vehicle log | Head Office vehicle log |
| **ApproveExpense.aspx** | Expense approval workflow | `insAdditionalExpenseApprovalHOD` |

### PDF Voucher Generation (NOT in New MS)
The legacy `VesselExpense.aspx.cs` generates formatted **payment vouchers** using iTextSharp:
- **Voucher types**: Cash, Cheque, NEFT, RTGS
- **Template**: `PrintVoucherVessel.htm` — HTML template with token replacement
- **Fields**: VoucherNo, Date, PaidTo, Amount (in words), Description, BillNo, PayType
- **Output**: Downloadable PDF voucher

### HO (Head Office) Oversight (NOT in New MS)
The legacy has dedicated HO-prefixed pages for head-office users to view and manage maintenance and expenses across branches. The new MS uses `DataScopeService` for branch-level access control but lacks dedicated HO views.

### Expense Approval Hierarchy (NOT in New MS)
- Legacy has explicit `ApproveExpense.aspx` with HOD approval workflow
- SP: `insAdditionalExpenseApprovalHOD`
- New MS has expense CRUD but no approval step

---

## 7. E-WAY BILL MANAGEMENT

### Status: 🟢 COVERED (different approach) — New MS has ULIP-based E-Way Bill via `UlipService`

### Legacy: EWayBill.aspx (1436 lines)
- Manual E-Way Bill generation form
- Fetches job details via `GetJobDetailByJobId`
- State selection for dispatch/delivery (GSTIN-linked)
- Invoice count tracking
- Sub-type selection
- JSON and Excel export options

### New MS: UlipController + UlipService
- `POST /ulip/eway-bill` — generate E-Way Bill
- `POST /ulip/eway-bill/from-job/{jobId}` — auto-fill from job
- `GET /ulip/eway-bill/by-job/{jobId}` — retrieve by job
- `EWayBill` entity with full data model
- Integrated with ULIP/NIC API (stubbed for now)

### Gap: Legacy has manual form entry with auto-complete from job. New MS has API-first approach which is better, but may need UI-facing DTO adjustments.

---

## 8. FLEET VEHICLE & DRIVER MANAGEMENT

### Status: 🟢 WELL COVERED — New MS has comprehensive `FleetVehicleService`

### Legacy Pages

| Page | Purpose |
|------|---------|
| **Equipment.aspx** (844 lines) | Full CRUD for TR_EquipmentMS (vehicles/equipment) |
| **VehicleDetail.aspx** | Vehicle detail view |
| **ViewVehicle.aspx** | Vehicle information display |
| **rvVehicleDetail.aspx** | Vehicle detail report variant |
| **VehicleDriver.aspx** | Driver assignment to vehicles |
| **VehicleDailyStatus.aspx** | Daily status recording per vehicle |
| **VehicleStatusReport.aspx** | Vehicle status reporting |
| **NewTransVehicleDetail.aspx** | New vehicle detail entry |

### New MS Coverage
| New MS Method | Covers Legacy Feature |
|---------------|----------------------|
| `CreateVehicleAsync` | Equipment.aspx Insert ✅ |
| `UpdateVehicleAsync` | Equipment.aspx Update ✅ |
| `DeleteVehicleAsync` | Equipment.aspx Delete ✅ |
| `AssignDriverAsync` | VehicleDriver.aspx ✅ |
| `RecordDailyStatusAsync` | VehicleDailyStatus.aspx ✅ |
| `GetDailyStatusHistoryAsync` | VehicleStatusReport.aspx ✅ |
| `GetExpiringComplianceAsync` | NEW — not in legacy ✅ |
| `CreateTravelLogAsync` | NEW — not in legacy ✅ |
| `GetVehicleUsageSummaryAsync` | NEW — not in legacy ✅ |

### Minor Gap: Equipment.aspx User Notification Configuration
Legacy Equipment.aspx has `AddUserNotification` — configure Email/SMS notifications per notification type per user. New MS has `TransportNotificationService` but tied to transport events, not user-configurable per equipment.

---

## 9. TRANSPORTER MANAGEMENT

### Status: 🟢 WELL COVERED

### Legacy Pages
| Page | Purpose |
|------|---------|
| **TransporterList.aspx** | Transporter listing |
| **TransporterTab.aspx** | Transporter detail tabs |

### New MS Coverage
- Full CRUD: Create, Get, Update, Delete ✅
- KYC Documents: Add, Delete ✅
- Bank Accounts: Add, Update, Delete ✅
- Notifications: CRUD ✅
- Paged listing with filters ✅

### Gap: None significant — new MS has better coverage.

---

## 10. MOVEMENT & TRACKING

### Status: 🟢 COVERED

### Legacy Pages
| Page | Purpose |
|------|---------|
| **TransMovement.aspx** | Movement entry |
| **TransMovementBT.aspx** | Movement (Break Transport variant) |
| **JobTracking.aspx** | Job location tracking |

### New MS Coverage
- `AddMovementAsync` ✅
- `GetJobTimelineAsync` — full timeline ✅
- `GetJobStatusAsync` — current status ✅

---

## 11. REPORTS & DASHBOARDS

### Status: 🟢 MOSTLY COVERED — New MS has extensive `ReportsService`. Legacy has more specialized reports.

### Legacy Report Pages (14 files)

| Page | Purpose | In New MS? |
|------|---------|------------|
| **TransDSR.aspx** | Daily Status Report with module filtering | ❌ No (cross-module report) |
| **TransDashboard.aspx** | Maintenance + Vehicle expense dashboard | ✅ `DashboardService` |
| **NBCPLDashboard.aspx** | NBCPL-specific dashboard | ❌ No (company-specific) |
| **TPWiseSummary.aspx** | Transporter-wise vehicle summary | ✅ `TransporterPerformanceAsync` |
| **ClientWsSummary.aspx** | Client-wise summary | ✅ `CustomerBillingAsync` |
| **ReportCustomerTransport.aspx** | Customer transport report | ✅ `CustomerBillingAsync` |
| **ReportExpense.aspx** | Expense report | ✅ `ExpenseAnalysisAsync` |
| **ReportMaintenance.aspx** | Maintenance report | ✅ `MaintenanceCostReportAsync` |
| **ReportLabour.aspx** | Labour report | ❌ No |
| **ReportTransporterMonth.aspx** | Transporter monthly report | ✅ `TransporterPerformanceAsync` |
| **ReportVehicleClosing.aspx** | Vehicle closing report | ❌ No |
| **ReportVehicleExpense.aspx** | Vehicle expense report | ✅ `ExpenseAnalysisAsync` |
| **ReportVehicleMonth.aspx** | Vehicle monthly report | ✅ `VehicleUtilizationAsync` |
| **ReportVehicleSummary.aspx** | Vehicle summary | ✅ `VehicleUtilizationAsync` |
| **ReportVehicleTrip.aspx** | Vehicle trip report | ✅ via TravelLogs |
| **ReportVehicleTripDaily.aspx** | Daily vehicle trip | ✅ via TravelLogs |
| **WeeklyTripReport.aspx** | Weekly trip aggregation | ❌ No (weekly aggregation) |
| **PendingUnloading.aspx** | Movement pending report | ✅ via queue |
| **InvoicePending.aspx** | Invoice pending status | ❌ No (billing dependent) |
| **VehicleTripDetail.aspx** | Trip detail | ✅ via TravelLogs |
| **TripDetail.aspx** + TripDetail.rpt | Crystal Reports trip detail | ❌ No (Crystal Reports) |

### New MS Additional Reports (not in legacy)
- `RouteAnalysisAsync` — route efficiency analysis
- `EmptyLegReportAsync` — empty leg tracking
- `FundRequestReportAsync` — fund request analysis
- `TollExpenseReportAsync` — toll-specific analysis
- `BranchComparisonAsync` — branch performance comparison
- Excel export endpoints ✅

### Missing Report Features
1. **DSR with Module Filtering** — cross-module checkbox-based report
2. **Crystal Reports** integration (TripDetail.rpt) — formatted print output
3. **Weekly Trip Report** — weekly aggregation
4. **Vehicle Closing Report** — end-of-period vehicle closing
5. **Labour Report** — labour cost reporting
6. **Invoice Pending Report** — dependent on billing subsystem

---

## 12. STAMP DUTY MANAGEMENT

### Status: 🔴 NOT IN NEW MS

### Legacy (from DBOperations.cs lines 1-100)
```csharp
BS_GetStampDutyDetailById(int ID)     // Retrieve stamp duty detail
insStampDutyDetail(...)                // Insert stamp duty
updStampDutyAmnt(int ID, decimal amt)  // Update stamp duty amount
```

No dedicated .aspx page found — likely embedded in billing or integrated within other pages.

### What's Needed
- StampDuty entity or lookup in new MS
- Integration with billing workflow for regulatory compliance

---

## 13. ADMIN & RECEIVED MANAGEMENT

### Status: 🟡 PARTIALLY COVERED

### Legacy Pages
| Page | Purpose |
|------|---------|
| **AdminReceived.aspx** | Admin received document management |
| **Transport.aspx** | Main transport landing/list |
| **NewTransport.aspx** | New transport job creation |
| **NavBharatNewTransport.aspx** | NavBharat-specific transport creation |
| **JobDetail.aspx** | Job detail view |
| **SuccessPage.aspx** | Post-action success page |

### New MS Coverage
- Job creation via `JobsController` — `POST /jobs`, `POST /jobs/from-enquiry`, `POST /jobs/from-freight` ✅
- No company-specific entry pages (correct — should be configurable) ✅

---

## 14. E-INVOICE / GST (CharteredInfo)

### Status: 🟢 WELL COVERED — New MS has full `CharteredInfoService`

### New MS Only Features
- GST lookup: `LookupGstAsync` ✅
- IRN generation: `GenerateIrnAsync` ✅
- IRN cancellation: `CancelIrnAsync` ✅
- E-Way Bill from IRN: `GenerateEwbFromIrnAsync` ✅
- E-Way Bill cancellation: `CancelEwbAsync` ✅
- Dynamic QR code: `GetDynamicQrAsync` ✅
- By transport job: `GetByTransportJobAsync` ✅

Legacy has `EWayBill.aspx` and `EWayBillNew.aspx` for manual E-Way Bill, but no e-Invoice/IRN support.

---

## Complete Stored Procedure Inventory (from legacy code reads)

### Billing SPs
| SP | Called From |
|----|------------|
| `AddTransBillDetail` | TransBillDetail.aspx |
| `AddTransportRate` | TransBillDetail.aspx |
| `AddTransApproveRejectBill` | ApproveBill.aspx |
| `AddTransBillApprovalHistory` | ApproveBill.aspx |
| `AddBillReceivedDetail` | ApproveBill.aspx |
| `GetPackingListDocs` | ApproveBill.aspx |

### Transport Request / Vehicle SPs
| SP | Called From |
|----|------------|
| `GetNewTransportNo` | TruckRequest.aspx |
| `AddJobTransportRequest` | TruckRequest.aspx |
| `AddPackingListDocs` | TruckRequest.aspx |
| `TR_updJobTransportBabaji` | TruckRequest.aspx |
| `AddTransAddDetails` | TruckRequest.aspx |
| `AddTransportRateDetail` | VehiclePlace.aspx |
| `UpdateTransportRateDetail` | VehiclePlace.aspx |
| `AddTransporterPlaced` | VehiclePlace.aspx |
| `TR_AddBillingInstructions` | VehiclePlace.aspx |
| `AddVehiclePlaced` | VehiclePlaced.aspx |
| `GetConsolidateRefNo` | VehiclePlaced.aspx |
| `AddConsolidateJob` | VehiclePlaced.aspx |
| `GetTransportRequestDetail` | Multiple pages |

### Delivery / Warehouse SPs
| SP | Called From |
|----|------------|
| `insJobTransitHistory` | InTransitWarehouse (via AddTransitWarehouse) |
| `insDeliveryDetail` | Via AddDeliveryDetail |
| `insDeliveryWarehouse` | WarehouseDelivery (via AddDeliveryWarehouse) |
| `insDeliveryConsolidateMS` | Clearance (via AddDeliveryConsolidateMS) |
| `insDeliveryConsolidate` | Clearance (via AddDeliveryConsolidateDetail) |
| `delDeliveryDetail` | Via DeleteDeliveryDetail |
| `GetJobDetailForDelivery` | WarehouseDelivery.aspx |
| `GetTruckRequestByJobId` | WarehouseDelivery.aspx |

### Maintenance / Expense SPs
| SP | Called From |
|----|------------|
| `AddMaintenanceVessel` | VesselMaintenance.aspx |
| `AddMaintenanceDocument` | VesselMaintenance.aspx |
| `GetNewTransportRefNo` | VesselMaintenance.aspx |
| `FillMaintenanceCategory` | VesselMaintenance.aspx |
| `GetWorkExpense` | VesselExpense.aspx |
| `GetVehicleExpepsneByDate` | VesselExpense.aspx |
| `AddVehicleDailyExpense` | TransDailyExpense.aspx |
| `AddVehicleDailyStatus` | VehicleDailyStatus.aspx |
| `AddVehicleDriver` | VehicleDriver.aspx |
| `insAdditionalExpenseApprovalHOD` | ApproveExpense.aspx |

### Master Data / Lookup SPs
| SP | Called From |
|----|------------|
| `FillVehicleType` | Multiple pages |
| `FillCompanyByCategory` | Multiple pages |
| `FillStateGSTID` | EWayBill.aspx |
| `FillTransSubType` | EWayBill.aspx |
| `FillVehicleForNavbharat` | VehiclePlace.aspx |
| `FillVehicleForNAVJEEVAN` | VehiclePlace.aspx |
| `FillVehicleNoForWarehouse` | WarehouseDelivery.aspx |
| `FillPendingContainerDetail` | WarehouseDelivery.aspx |
| `FillEWayBillGSTIN` | DBOperations helpers |
| `GetTransporterBankDetails` | VehiclePlace.aspx, ConsolidateRequest.aspx |

### Stamp Duty SPs
| SP | Called From |
|----|------------|
| `BS_GetStampDutyDetailById` | DBOperations |
| `insStampDutyDetail` | DBOperations |
| `updStampDutyAmnt` | DBOperations |

### Container SPs
| SP | Called From |
|----|------------|
| `insContainerDetail` | DBOperations |
| `updContainerDetail` | DBOperations |

### Consolidation SPs
| SP | Called From |
|----|------------|
| `GetConsolidateRequestById` | ConsolidateRequest.aspx |
| `NB_GetConsolidateRequestById` | ConsolidateRequest.aspx |
| `UpdateTransportRateDetailForConsolidateJob` | ConsolidateRequest.aspx |
| `GetConsolidateJobDetail` | ConsolidateRequest.aspx |

---

## Priority Migration Roadmap

### Phase 1 — Critical (Revenue / Compliance)
1. **Transport Billing Service** — entire bill lifecycle (submit → approve → reject → track → history)
2. **Vehicle Daily Expense** — 15-category daily expense tracking per vehicle
3. **Stamp Duty** — regulatory compliance

### Phase 2 — High (Operational Completeness)
4. **Warehouse Delivery** — transit warehouse stage + Indian compliance fields (LR, N-Form, S-Form, Octroi, Road Permit)
5. **PDF Voucher Generation** — payment voucher printing for expenses
6. **Expense Approval Workflow** — HOD approval for additional expenses
7. **Fund Request Integration** — transporter bank details + payment request linkage to Accounts module

### Phase 3 — Medium (Reporting & Dashboards)
8. **DSR** with cross-module filtering
9. **Labour Report**, **Vehicle Closing Report**, **Weekly Trip Report**
10. **Crystal Reports replacement** — formatted printable reports (PDF generation)
11. **Invoice Pending Report** (depends on Phase 1)

### Phase 4 — Low (Edge Cases)
12. **Company-specific logic** (NavBharat/NavJeevan hardcoded transporter handling → configurable "own fleet" flag)
13. **HO (Head Office) oversight views** — branch-level vs HO-level access patterns
14. **BillingInstruction, ContractPrice, SellingPrice, MarketRate** fields on rate details
15. **Legacy packing list document management** — ZIP packaging

---

## Appendix: Legacy File → New MS Mapping

| Legacy File | New MS Equivalent | Status |
|-------------|-------------------|--------|
| Transport.aspx | JobsController GET /jobs | ✅ Covered |
| NewTransport.aspx | JobsController POST /jobs | ✅ Covered |
| TruckRequest.aspx | JobsController POST /jobs/{id}/vehicles | ✅ Covered |
| VehiclePlace.aspx | JobsController POST /jobs/{id}/vehicles/{vid}/rate | ✅ Mostly |
| VehiclePlaced.aspx | JobsController POST /jobs/{id}/vehicles | ✅ Covered |
| TransMovement.aspx | JobsController POST /jobs/{id}/movements | ✅ Covered |
| InTransitWarehouse.aspx | — | ❌ Missing |
| WarehouseDelivery.aspx | JobsController POST /jobs/{id}/delivery | 🟡 Partial |
| TransBill*.aspx (16 pages) | — | ❌ Missing |
| ApproveBill.aspx | — | ❌ Missing |
| TransDailyExpense.aspx | — | ❌ Missing |
| VesselMaintenance.aspx | MaintenanceController | ✅ Covered |
| VesselExpense.aspx | — (voucher gen) | ❌ Missing |
| Equipment.aspx | FleetVehiclesController | ✅ Covered |
| VehicleDriver.aspx | FleetVehiclesController drivers | ✅ Covered |
| VehicleDailyStatus.aspx | FleetVehiclesController daily-status | ✅ Covered |
| Consolidate*.aspx | JobsController consolidated-trips | ✅ Mostly |
| EWayBill.aspx | UlipController eway-bill | ✅ Covered |
| TransDSR.aspx | — | ❌ Missing |
| Report*.aspx (14 pages) | ReportsController | 🟡 Mostly |
| TransporterList.aspx | TransportersController | ✅ Covered |
| ApproveExpense.aspx | — | ❌ Missing |
| TransBillTracking.aspx | — | ❌ Missing |
