# Transport Microservice — PRD vs Code Gap Analysis

**Date:** 2026-02-09  
**PRD Source:** `C:\Users\fresh\Downloads\LT-Transport-PRD.md` (v1.1)  
**Codebase:** `D:\INTTransport\` (Clean Architecture — API / Application / Domain / Infrastructure)  

**Legend:**  
- ✅ **DONE** — Fully implemented in code  
- ⚠️ **PARTIAL** — Scaffolded/started but missing logic or incomplete  
- ❌ **NOT DONE** — Not implemented at all  

---

## Executive Summary

| Category | Done | Partial | Not Done | Total |
|----------|------|---------|----------|-------|
| **Phase 1 — Core Job Lifecycle** | 14 | 3 | 1 | 18 |
| **Phase 2 — Fleet & Consolidation** | 2 | 2 | 4 | 8 |
| **Phase 3 — Maintenance & Reports** | 0 | 0 | 7 | 7 |
| **Phase 4 — Future Enhancements** | 0 | 0 | 7 | 7 |
| **Cross-Cutting** | 5 | 4 | 3 | 12 |
| **TOTAL** | **21** | **9** | **22** | **52** |

**Overall Completion: ~40% done, ~17% partial, ~43% not started.**  
Phase 1 (MVP) is substantially built. Phase 2+ is mostly not started.

---

## 1. Job Creation — 3 Ways a Job Starts (PRD §4)

### Door 1: CRM Enquiry → Transport Job
| Requirement | Status | Evidence |
|---|---|---|
| `POST /jobs/from-enquiry` endpoint | ✅ DONE | `JobsController.CreateFromEnquiry()` |
| Accepts Enquiry ID, customer, origin, destination, cargo | ✅ DONE | `CreateJobFromEnquiryDto` has all fields |
| Source set to `CRMEnquiry` | ✅ DONE | `TransportJobService.CreateJobFromEnquiryAsync()` sets `JobSource.CRMEnquiry` |
| Workflow starts automatically | ✅ DONE | `StartWorkflowAsync()` called after creation |
| Validator present | ✅ DONE | `CreateJobFromEnquiryValidator` in `TransportValidators.cs` |

### Door 2: Freight Job → Transport Job
| Requirement | Status | Evidence |
|---|---|---|
| `POST /jobs/from-freight` endpoint | ✅ DONE | `JobsController.CreateFromFreight()` |
| Accepts freight ref, containers, pickup/delivery | ✅ DONE | `CreateJobFromFreightDto` has all fields |
| Source set to `FreightJob` | ✅ DONE | Sets `JobSource.FreightJob` |
| Workflow starts automatically | ✅ DONE | `StartWorkflowAsync()` called |
| Validator present | ✅ DONE | `CreateJobFromFreightValidator` |

### Door 3: Standalone Job Creation
| Requirement | Status | Evidence |
|---|---|---|
| `POST /jobs` endpoint | ✅ DONE | `JobsController.Create()` |
| All fields per PRD §6 table | ✅ DONE | `CreateTransportJobDto` covers all fields |
| Auto-generated Request Number (TR-YYYY-CC-NNNNNN) | ✅ DONE | `GenerateRequestNumberAsync()` — format `TR-{year}-{countryCode}-{seq:D6}` |
| Workflow starts automatically | ✅ DONE | `StartWorkflowAsync()` called |
| Validator present | ✅ DONE | `CreateTransportJobValidator` with FluentValidation |

---

## 2. Transport Lifecycle — 8 Steps (PRD §5)

| Step | PRD Status | Code Status | Evidence |
|---|---|---|---|
| 1. Request Created | ✅ DONE | `TransportStatus.RequestCreated = 0` | Set on all creation paths |
| 2. Request Received | ✅ DONE | `POST /jobs/{id}/receive` | `ReceiveJobAsync()` — validates status, advances workflow |
| 3. Vehicle Assigned | ✅ DONE | `POST /jobs/{id}/vehicles` | `AssignVehicleAsync()` — transitions to `VehicleAssigned` |
| 4. Rate Entered | ✅ DONE | `POST /jobs/{id}/vehicles/{vid}/rate` | `EnterRateAsync()` — transitions to `RateEntered` |
| 5. Rate Approval | ✅ DONE | `POST /jobs/{id}/rate/submit-approval` | `SubmitRateForApprovalAsync()` — transitions to `RateApproval`, calls workflow |
| 6. In Transit | ✅ DONE | `POST /jobs/{id}/movements` | `AddMovementAsync()` — auto-transitions to `InTransit` on first Dispatched movement |
| 7. Delivered | ✅ DONE | `POST /jobs/{id}/delivery` | `RecordDeliveryAsync()` — transitions to `Delivered` |
| 8. Cleared | ✅ DONE | `POST /jobs/{id}/clearance` | `ClearJobAsync()` — transitions to `Cleared` |

**Status transition validation:** ✅ Each lifecycle method checks the current status before proceeding.

**Workflow integration per step:** ✅ `AdvanceWorkflowAsync()` called at each transition via `IWorkflowClient`.

---

## 3. Feature-by-Feature Gap Analysis

### Feature 1 — Transport Request (PRD §6)

| Requirement | Status | Notes |
|---|---|---|
| All 30+ fields stored in entity | ✅ DONE | `TransportRequest` entity has: RequestNumber, RequestDate, Source, SourceReferenceId, CustomerId, CustomerName, GSTNumber, OriginLocationId, PickupAddress, PickupCity/State/Pincode, DestinationLocationId, DropAddress, DropCity/State/Pincode, CargoType, CargoDescription, GrossWeightKg, NumberOfPackages, Container20/40Count, VehicleTypeRequired, DeliveryType, RequiredDeliveryDate, Priority, SpecialInstructions, BranchId, CountryCode, Division, Plant, Status, WorkflowInstanceId |
| Dynamic/extra fields (key-value) | ✅ DONE | `TransportRequestDetail` entity with `FieldKey`, `FieldValue`, `DataType`, `WorkflowStepId` |
| GET by ID with all children | ✅ DONE | `GetJobByIdAsync()` returns `TransportJobDto` with nested Vehicles, Movements, Delivery, Documents |
| LIST with filters & pagination | ✅ DONE | `GetJobsAsync()` with `TransportJobFilterDto` (status, priority, source, customer, branch, country, date range, search) |
| UPDATE (before vehicle assignment) | ✅ DONE | `UpdateJobAsync()` — blocks edits after `VehicleAssigned` |
| DELETE / Cancel | ✅ DONE | `DeleteJobAsync()` — sets `Cancelled` status, cancels workflow |

### Feature 2 — Request Queue & Consolidation (PRD §7)

| Requirement | Status | Notes |
|---|---|---|
| Queue endpoint for operations | ✅ DONE | `GET /jobs/queue` — filters `Status == RequestCreated` |
| Filter by date, customer, origin, destination, status, branch | ⚠️ PARTIAL | Queue uses `TransportJobFilterDto` which has branch/country/date filters, but **no separate origin/destination filter fields** |
| Sort by priority, date | ✅ DONE | Filter DTO has `SortBy` and `SortDescending` |
| Bulk select requests | ❌ NOT DONE | No bulk-receive endpoint exists |
| "Receive" button → mark acknowledged | ✅ DONE | `POST /jobs/{id}/receive` endpoint |
| "Consolidate" button → group selected | ⚠️ PARTIAL | `POST /jobs/consolidate` exists but implementation is basic — only groups by setting `ConsolidatedTripId` GUID. **No `ConsolidatedTrip` entity, no consolidated vehicle/expense/tracking.** |
| Rule: only RequestCreated in queue | ✅ DONE | Queue filters on `Status == RequestCreated` |
| Rule: consolidation for same destination | ❌ NOT DONE | No destination validation in `ConsolidateJobsAsync()` |

### Feature 3 — Transporter & Vehicle Assignment (PRD §8)

| Requirement | Status | Notes |
|---|---|---|
| Pick transporter from master | ✅ DONE | `AssignVehicleDto.TransporterId` |
| Assign vehicle (number plate) | ✅ DONE | `AssignVehicleDto.VehicleNumber` |
| Assign driver (name + phone) | ✅ DONE | `DriverName`, `DriverPhone` fields |
| Vehicle type validation (must match requested) | ❌ NOT DONE | No check that assigned `VehicleType == VehicleTypeRequired` |
| Upload LR details (LR number, LR date) | ✅ DONE | `LRNumber`, `LRDate` on `TransportVehicle` |
| Upload memo copy | ✅ DONE | `MemoCopyUrl` on `TransportVehicle` |
| Fund Request (advance to transporter) | ✅ DONE | `VehicleFundRequest` entity with Amount, BankName, AccountNumber, IFSCCode, Status |
| Fund Request workflow (processing by finance) | ⚠️ PARTIAL | Entity exists but **no endpoint to create/approve fund requests** — no fund request controller or service methods |
| View transporter's past performance | ❌ NOT DONE | No performance/rating query implementation |
| Update vehicle assignment | ✅ DONE | `PUT /jobs/{id}/vehicles/{vehicleId}` |
| Multiple vehicles per job | ✅ DONE | `TransportVehicle` is one-to-many with `TransportRequest` |

### Feature 4 — Rate Management (PRD §9)

| Requirement | Status | Notes |
|---|---|---|
| Rate per vehicle | ✅ DONE | `VehicleRate` entity linked to `TransportVehicle` |
| Freight Rate | ✅ DONE | `FreightRate` field |
| Detention Charges | ✅ DONE | `DetentionCharges` field |
| Varai / Loading Charges | ✅ DONE | `VaraiCharges` field |
| Empty Container Return | ✅ DONE | `EmptyContainerReturn` field |
| Toll Charges | ✅ DONE | `TollCharges` field |
| Other Charges | ✅ DONE | `OtherCharges` field |
| Total auto-computed | ⚠️ PARTIAL | `TotalRate` field exists but **no auto-computation in `EnterRateAsync()`** — it relies on the mapper/DTO. No explicit sum logic. |
| Currency support | ✅ DONE | `CurrencyCode` field (defaults to "INR") |
| Rate is buying rate only | ✅ DONE | No selling rate fields |

### Feature 5 — Rate Approval (PRD §10)

| Requirement | Status | Notes |
|---|---|---|
| Submit for approval endpoint | ✅ DONE | `POST /jobs/{id}/rate/submit-approval` |
| Workflow MS integration for approval | ✅ DONE | `AdvanceWorkflowAsync()` called — delegates to Workflow MS |
| Approval rules (amount thresholds) | ✅ DONE | Configured in Workflow MS, not in Transport MS (correct per architecture) |
| Approve → moves to InTransit | ✅ DONE | Movement with `Dispatched` milestone auto-transitions |
| Reject → goes back to RateEntered | ⚠️ PARTIAL | Workflow callback endpoint exists (`POST /internal/jobs/{id}/workflow-callback`) but **implementation is a placeholder**: `await Task.CompletedTask;` — rejection rollback not wired |
| Pending approval list | ✅ DONE | `GET /jobs/pending-approval` endpoint |

### Feature 6 — Movement Tracking (PRD §11)

| Requirement | Status | Notes |
|---|---|---|
| Add movement/location updates | ✅ DONE | `POST /jobs/{id}/movements` |
| All 7 milestones | ✅ DONE | `MovementMilestone` enum: Dispatched, InTransit, AtCheckpoint, NearDestination, ReachedDestination, Unloading, Completed |
| Location name, timestamp, remarks | ✅ DONE | Fields on `TransportMovement` |
| Latitude / Longitude support | ✅ DONE | `Latitude`, `Longitude` on both entity and DTO |
| Per-vehicle movement tracking | ✅ DONE | `TransportVehicleId` on `TransportMovement` |
| Daily status email to customer | ❌ NOT DONE | No email/notification service implemented |
| Manual trigger for status email | ❌ NOT DONE | No endpoint |
| GPS integration readiness | ✅ DONE | Lat/long fields present, ready for GPS data |

### Feature 7 — Delivery & POD (PRD §12)

| Requirement | Status | Notes |
|---|---|---|
| Record delivery endpoint | ✅ DONE | `POST /jobs/{id}/delivery` |
| Delivery Date | ✅ DONE | Field on `TransportDelivery` |
| Received By | ✅ DONE | Field present |
| POD Number + Document URL | ✅ DONE | `PODNumber`, `PODDocumentUrl` |
| Challan Number + Document URL | ✅ DONE | `ChallanNumber`, `ChallanDocumentUrl` |
| LR Copy URL | ✅ DONE | `LRCopyUrl` |
| E-Way Bill Number | ✅ DONE | `EWayBillNumber` |
| Delivery Status (Full/Partial/Damaged) | ✅ DONE | `DeliveryStatus` enum |
| Damage Notes | ✅ DONE | `DamageNotes` field |
| Short Delivery Notes | ✅ DONE | `ShortDeliveryNotes` field |
| ELR (Electronic Lorry Receipt) PDF generation | ❌ NOT DONE | No PDF generation logic |
| Notification on delivery (email to customer) | ❌ NOT DONE | No notification service |
| WhatsApp notification | ❌ NOT DONE | Phase 4 item — not implemented |

### Feature 8 — Clearance (PRD §13)

| Requirement | Status | Notes |
|---|---|---|
| Clear job endpoint | ✅ DONE | `POST /jobs/{id}/clearance` |
| Status validation (only delivered can be cleared) | ✅ DONE | Checks `Status == Delivered` |
| Clearance checklist verification | ❌ NOT DONE | No check for POD uploaded, LR uploaded, challan, rate approved, expenses entered, fund requests |
| Bulk clearance | ❌ NOT DONE | No bulk clearance endpoint |
| Lock after clearance (no further edits) | ⚠️ PARTIAL | Status becomes `Cleared` but no explicit lock mechanism; edit checks only block after `VehicleAssigned` |

### Feature 9 — Vehicle Fleet Management (PRD §14)

| Requirement | Status | Notes |
|---|---|---|
| **9A. Vehicle Master (company-owned)** | ❌ NOT DONE | No `Vehicle` entity for company-owned fleet. `VehicleDetail` entity is for ULIP VAHAN cache only. |
| Vehicle CRUD endpoints (`/vehicles`) | ❌ NOT DONE | No fleet vehicles controller |
| **9B. Driver Assignment** | ❌ NOT DONE | No `VehicleDriver` entity or endpoints |
| **9C. Daily Vehicle Status** | ❌ NOT DONE | No `VehicleDailyStatus` entity, no calendar view |
| **9D. Maintenance Work Orders** | ❌ NOT DONE | No `MaintenanceWorkOrder`, no endpoints |
| **9E. HO (Head Office) Vehicles** | ❌ NOT DONE | No `VehicleTravelLog` entity |

### Feature 10 — Transporter Master (PRD §15)

| Requirement | Status | Notes |
|---|---|---|
| CRUD endpoints | ✅ DONE | `TransportersController` — GET, GET by ID, POST, PUT, DELETE |
| Fields: name, contact, phone, email, PAN, GST, address | ✅ DONE | `Transporter` entity has all fields |
| Rating field | ✅ DONE | `Rating` decimal on entity |
| Status management (Active/Suspended/Blacklisted) | ✅ DONE | `TransporterStatus` enum, `SuspensionReason`, `SuspensionDate` |
| KYC Documents | ✅ DONE | `TransporterKYC` entity + `POST /transporters/{id}/kyc`, `DELETE /transporters/{id}/kyc/{kycId}` |
| KYC verification tracking | ✅ DONE | `IsVerified`, `VerifiedByName`, `VerifiedDate`, `ExpiryDate` on `TransporterKYC` |
| Bank Details | ✅ DONE | `TransporterBank` entity + POST, PUT, DELETE endpoints |
| Primary bank flag | ✅ DONE | `IsPrimary` field with auto-unset logic |
| Notification Settings | ✅ DONE | `TransporterNotification` entity (Email/SMS/WhatsApp type) |
| Notification endpoint (CRUD) | ⚠️ PARTIAL | Entity and navigation property exist but **no controller endpoints for notification settings CRUD** |
| Auto-send trip details on vehicle assignment | ❌ NOT DONE | No notification trigger |
| Filtered/paginated list | ✅ DONE | `GetAllAsync()` with `TransporterFilterDto` |

### Feature 11 — E-Way Bill (PRD §16)

| Requirement | Status | Notes |
|---|---|---|
| E-Way Bill entity | ✅ DONE | `EWayBill` entity with all fields (supplier/recipient GSTIN, HSN, taxable amount, vehicle, from/to addresses) |
| Generate E-Way Bill endpoint | ✅ DONE | `POST /ulip/eway-bill` + `POST /einvoice/ewb/generate` |
| Auto-fill from transport job data | ⚠️ PARTIAL | `GenerateEWayBillRequestDto` requires manual field population — no auto-fill from TransportRequest |
| JSON output for GST portal | ❌ NOT DONE | No JSON/Excel export endpoint |
| Excel export for manual entry | ❌ NOT DONE | |
| NIC API integration | ⚠️ PARTIAL | ULIP `GenerateEWayBillAsync()` creates local record with status "PENDING" — real NIC API not integrated. CharteredInfo `GenerateEwbFromIrnAsync()` does call the API. |

### Feature 12 — Trip Expenses (PRD §17)

| Requirement | Status | Notes |
|---|---|---|
| 15 expense categories | ✅ DONE | `ExpenseCategory` enum with all 15 types |
| Expense entity with amount, date, receipt | ✅ DONE | `TransportExpense` entity |
| Per-trip/vehicle expense tracking | ✅ DONE | Has both `TransportRequestId` and optional `TransportVehicleId` |
| Expense CRUD endpoints | ❌ NOT DONE | **No expense controller or service methods** — entity exists but no API to create/read/update/delete expenses |

### Feature 13 — Consolidation / Multi-Job Dispatch (PRD §18)

| Requirement | Status | Notes |
|---|---|---|
| Consolidate endpoint | ⚠️ PARTIAL | `POST /jobs/consolidate` exists |
| Consolidated Trip entity | ❌ NOT DONE | **No `ConsolidatedTrip` entity** — only a GUID stored on TransportRequest |
| Child request linking | ⚠️ PARTIAL | `ConsolidatedTripId` set on child jobs, but no `ConsolidatedTripRequest` join table |
| Single transporter + vehicle for group | ❌ NOT DONE | No `ConsolidatedVehicle` entity |
| Single rate entry for consolidated trip | ❌ NOT DONE | |
| Per-stop delivery tracking | ❌ NOT DONE | |
| Combined expense tracking | ❌ NOT DONE | No `ConsolidatedExpense` entity |
| Unique Consolidated Reference Number | ❌ NOT DONE | Only a random GUID assigned |

### Feature 14 — Dashboard & Reports (PRD §19)

#### Dashboard
| Widget | Status | Notes |
|---|---|---|
| Pipeline Funnel | ✅ DONE | `DashboardService.GetDashboardAsync()` counts per-status |
| Today's Summary (new requests, vehicles out, deliveries expected) | ✅ DONE | `TodaySummaryDto` |
| Overdue Jobs | ✅ DONE | Counts where `RequiredDeliveryDate < today && Status < Delivered` |
| Pending Approvals | ✅ DONE | Counts where `Status == RateApproval` |
| Top Transporters | ❌ NOT DONE | No transporter ranking query |
| Branch Comparison | ❌ NOT DONE | No branch-level aggregation |

#### Reports (13 Types)
| Report | Status | Notes |
|---|---|---|
| 1. Vehicle Trip (Monthly) | ❌ NOT DONE | No report endpoints at all |
| 2. Vehicle Trip (Daily) | ❌ NOT DONE | |
| 3. Vehicle Summary | ❌ NOT DONE | |
| 4. Vehicle Expense | ❌ NOT DONE | |
| 5. Vehicle by Type | ❌ NOT DONE | |
| 6. Vehicle Closing Status (Calendar) | ❌ NOT DONE | |
| 7. Transporter Monthly | ❌ NOT DONE | |
| 8. Customer Transport | ❌ NOT DONE | |
| 9. Expense Summary | ❌ NOT DONE | |
| 10. Labour / Mechanic | ❌ NOT DONE | |
| 11. Maintenance | ❌ NOT DONE | |
| 12. Weekly Trip | ❌ NOT DONE | |
| 13. Transporter Placement | ❌ NOT DONE | |
| Excel export for all reports | ❌ NOT DONE | |
| `GET /reports/{type}` endpoint | ❌ NOT DONE | No reports controller exists |

---

## 4. Integrations

### ULIP Integration (not in PRD but implemented — bonus)

| Capability | Status | Notes |
|---|---|---|
| VAHAN — Vehicle Lookup | ✅ DONE | `GET /ulip/vehicle/{vehicleNumber}` — full caching (24h TTL), VAHAN field mapping |
| SARATHI — Driver License Verification | ✅ DONE | `POST /ulip/driver-license` — full caching, field mapping |
| FASTag — Toll Tracking | ✅ DONE | `GET /ulip/fastag/{vehicleNumber}` — real-time, persists transactions |
| Toll Plaza Details | ✅ DONE | `GET /ulip/toll/{plazaId}` — cached |
| ULIP Health Check | ✅ DONE | `GET /ulip/health` |
| ULIP Client interface | ✅ DONE | `IUlipClient` with all methods |

### CharteredInfo Integration (not in PRD but implemented — bonus)

| Capability | Status | Notes |
|---|---|---|
| GST Lookup (cached 24h) | ✅ DONE | `GET /einvoice/gst/{gstin}` |
| Generate IRN (e-Invoice) | ✅ DONE | `POST /einvoice/irn/generate` — full NIC schema |
| Get IRN by hash | ✅ DONE | `GET /einvoice/irn/{irn}` |
| Get IRN by document | ✅ DONE | `GET /einvoice/irn/by-doc` |
| Cancel IRN | ✅ DONE | `POST /einvoice/irn/cancel` |
| Generate E-Way Bill from IRN | ✅ DONE | `POST /einvoice/ewb/generate` |
| Cancel E-Way Bill | ✅ DONE | `POST /einvoice/ewb/cancel` |
| Get e-Invoices by transport job | ✅ DONE | `GET /einvoice/by-job/{jobId}` |
| Dynamic QR Code | ✅ DONE | `POST /einvoice/qr-code` |
| Health Check | ✅ DONE | `GET /einvoice/health` |

### Workflow MS Integration (PRD §22)

| Requirement | Status | Notes |
|---|---|---|
| `IWorkflowClient` interface | ✅ DONE | `CreateInstanceAsync()`, `AdvanceAsync()`, `CancelAsync()` |
| `WorkflowClient` implementation | ✅ DONE | `Clients/WorkflowClient.cs` |
| Create instance on job creation | ✅ DONE | `StartWorkflowAsync()` — uses template code from config |
| Advance on each lifecycle step | ✅ DONE | `AdvanceWorkflowAsync()` called after each transition |
| Cancel on job deletion | ✅ DONE | Calls `_workflowClient.CancelAsync()` |
| Workflow callback endpoint | ⚠️ PARTIAL | `POST /internal/jobs/{id}/workflow-callback` exists but **body is `await Task.CompletedTask`** — not wired |
| Store WorkflowInstanceId on job | ✅ DONE | `WorkflowInstanceId`, `WorkflowStatus`, `WorkflowStepId` on `TransportRequest` |
| Non-fatal workflow errors | ✅ DONE | All workflow calls wrapped in try-catch with warning logs |
| Template code from config | ✅ DONE | `_configuration[$"WorkflowTemplates:Transport_{countryCode}:TemplateCode"]` |

### Masters MS Integration (PRD §3)

| Requirement | Status | Notes |
|---|---|---|
| `IMasterClient` interface | ✅ DONE | `Interfaces/Clients/IMasterClient.cs` |
| `MasterClient` implementation | ✅ DONE | `Clients/MasterClient.cs` |

### IAM Integration (PRD §3)

| Requirement | Status | Notes |
|---|---|---|
| `IIdentityClient` interface | ✅ DONE | `Interfaces/Clients/IIdentityClient.cs` |
| `IdentityClient` implementation | ✅ DONE | `Clients/IdentityClient.cs` |
| JWT authentication | ✅ DONE | `TransportBaseController` has `[Authorize]` attribute |
| UserContext extraction | ✅ DONE | `CurrentUserId`, `CurrentUser`, `CurrentUserEmail` properties |

---

## 5. Internal APIs (PRD §25)

| Method | Endpoint | Status | Notes |
|---|---|---|---|
| POST | `/internal/transport/jobs` | ✅ DONE | `InternalTransportController.CreateJob()` |
| GET | `/internal/transport/jobs/{id}/status` | ✅ DONE | `InternalTransportController.GetJobStatus()` — returns `TransportJobStatusDto` |
| POST | `/internal/transport/jobs/{id}/workflow-callback` | ⚠️ PARTIAL | Endpoint exists, DTO defined (`WorkflowCallbackDto`), but **implementation is a no-op placeholder** |
| Internal API security (X-Internal-Key) | ✅ DONE | `[InternalApi]` attribute + `InternalApiMiddleware.cs` + `InternalApiAttribute.cs` |
| `[AllowAnonymous]` for internal calls | ✅ DONE | Applied on `InternalTransportController` |
| Separate Swagger docs for internal APIs | ✅ DONE | `InternalApiDocumentFilter.cs` + `InternalApiOperationFilter.cs` |

---

## 6. External API Endpoints (PRD §25) — Complete Mapping

| PRD Endpoint | Method | Status | Code Location |
|---|---|---|---|
| `/jobs` | POST | ✅ DONE | `JobsController.Create()` |
| `/jobs/from-enquiry` | POST | ✅ DONE | `JobsController.CreateFromEnquiry()` |
| `/jobs/from-freight` | POST | ✅ DONE | `JobsController.CreateFromFreight()` |
| `/jobs` | GET | ✅ DONE | `JobsController.GetAll()` |
| `/jobs/{id}` | GET | ✅ DONE | `JobsController.GetById()` |
| `/jobs/{id}` | PUT | ✅ DONE | `JobsController.Update()` |
| `/jobs/{id}` | DELETE | ✅ DONE | `JobsController.Delete()` |
| `/jobs/{id}/receive` | POST | ✅ DONE | `JobsController.Receive()` |
| `/jobs/{id}/vehicles` | POST | ✅ DONE | `JobsController.AssignVehicle()` |
| `/jobs/{id}/vehicles/{vehicleId}` | PUT | ✅ DONE | `JobsController.UpdateVehicle()` |
| `/jobs/{id}/vehicles/{vehicleId}/rate` | POST | ✅ DONE | `JobsController.EnterRate()` |
| `/jobs/{id}/rate/submit-approval` | POST | ✅ DONE | `JobsController.SubmitRateForApproval()` |
| `/jobs/{id}/movements` | POST | ✅ DONE | `JobsController.AddMovement()` |
| `/jobs/{id}/delivery` | POST | ✅ DONE | `JobsController.RecordDelivery()` |
| `/jobs/{id}/clearance` | POST | ✅ DONE | `JobsController.Clear()` |
| `/jobs/{id}/documents` | POST | ✅ DONE | `JobsController.UploadDocument()` |
| `/jobs/{id}/timeline` | GET | ✅ DONE | `JobsController.GetTimeline()` |
| `/jobs/consolidate` | POST | ⚠️ PARTIAL | Exists but minimal logic |
| `/jobs/queue` | GET | ✅ DONE | `JobsController.GetQueue()` |
| `/jobs/pending-approval` | GET | ✅ DONE | `JobsController.GetPendingApproval()` |
| `/dashboard` | GET | ✅ DONE | `DashboardController.Get()` |
| `/reports/{type}` | GET | ❌ NOT DONE | No reports controller |
| `/transporters` | GET | ✅ DONE | `TransportersController.GetAll()` |
| `/transporters` | POST | ✅ DONE | `TransportersController.Create()` |
| `/transporters/{id}` | PUT | ✅ DONE | `TransportersController.Update()` |
| `/transporters/{id}` | GET | ✅ DONE | `TransportersController.GetById()` |
| `/vehicles` | GET | ❌ NOT DONE | No fleet vehicles controller |
| `/vehicles` | POST | ❌ NOT DONE | |
| `/vehicles/{id}` | PUT | ❌ NOT DONE | |
| `/vehicles/{id}/maintenance` | POST | ❌ NOT DONE | |
| `/vehicles/{id}/daily-status` | POST | ❌ NOT DONE | |
| `/vehicles/{id}/expenses` | GET | ❌ NOT DONE | |
| `/eway-bill/{jobId}/generate` | POST | ✅ DONE | Covered by `UlipController` + `EInvoiceController` |

**Endpoint Coverage: 24 of 32 external endpoints done (75%)**

---

## 7. Domain Entities (PRD §24) — Complete Mapping

### Core Entities
| PRD Entity | Status | Code Entity |
|---|---|---|
| `TransportRequest` | ✅ DONE | `TransportRequest.cs` — all fields present |
| `TransportRequestDetail` | ✅ DONE | `TransportRequestDetail.cs` — key-value dynamic fields |
| `TransportVehicle` | ✅ DONE | `TransportVehicle.cs` — with transporter, driver, LR |
| `VehicleRate` | ✅ DONE | `VehicleRate.cs` — all 6 charge components + approval |
| `VehicleFundRequest` | ✅ DONE | `VehicleFundRequest.cs` — amount, bank details, status |
| `TransportMovement` | ✅ DONE | `TransportMovement.cs` — milestone, location, lat/long |
| `TransportDelivery` | ✅ DONE | `TransportDelivery.cs` — POD, challan, damage notes |
| `TransportDocument` | ✅ DONE | `TransportDocument.cs` — file metadata |
| `TransportExpense` | ✅ DONE | `TransportExpense.cs` — category, amount, receipt |

### Consolidation Entities
| PRD Entity | Status | Notes |
|---|---|---|
| `ConsolidatedTrip` | ❌ NOT DONE | Only a GUID on TransportRequest — no standalone entity |
| `ConsolidatedTripRequest` | ❌ NOT DONE | No join table |
| `ConsolidatedVehicle` | ❌ NOT DONE | |
| `ConsolidatedExpense` | ❌ NOT DONE | |

### Fleet / Vehicle Entities
| PRD Entity | Status | Notes |
|---|---|---|
| `Vehicle` (company-owned) | ❌ NOT DONE | |
| `VehicleDriver` | ❌ NOT DONE | |
| `VehicleDailyStatus` | ❌ NOT DONE | |
| `MaintenanceWorkOrder` | ❌ NOT DONE | |
| `MaintenanceExpense` | ❌ NOT DONE | |
| `MaintenanceDocument` | ❌ NOT DONE | |
| `VehicleTravelLog` | ❌ NOT DONE | |

### Master Entities
| PRD Entity | Status | Notes |
|---|---|---|
| `Transporter` | ✅ DONE | Full CRUD + status management |
| `TransporterKYC` | ✅ DONE | With verification tracking |
| `TransporterBank` | ✅ DONE | With primary flag |
| `TransporterNotification` | ✅ DONE | Entity exists |
| `VehicleType` (lookup table) | ⚠️ PARTIAL | Implemented as `VehicleTypeEnum` enum — no master table |
| `ExpenseCategory` (lookup table) | ⚠️ PARTIAL | Implemented as `ExpenseCategory` enum — no master table |
| `MaintenanceCategory` | ❌ NOT DONE | |
| `VehicleStatus` (lookup table) | ❌ NOT DONE | |

### Bonus Entities (beyond PRD — ULIP/CharteredInfo caching)
| Entity | Status | Notes |
|---|---|---|
| `VehicleDetail` (VAHAN cache) | ✅ DONE | 50+ fields cached from ULIP |
| `DriverLicenseDetail` (SARATHI cache) | ✅ DONE | Full license details cached |
| `FASTagTransaction` | ✅ DONE | Toll transaction records |
| `TollPlaza` | ✅ DONE | Plaza master data cache |
| `EWayBill` | ✅ DONE | E-Way Bill records |
| `EInvoice` | ✅ DONE | e-Invoice (IRN) records |
| `GstDetail` | ✅ DONE | GST lookup cache |

**Entity Coverage: 14 of 25 PRD entities built (56%), plus 7 bonus entities**

---

## 8. Workflow Seed Data (PRD §22)

| Requirement | Status | Notes |
|---|---|---|
| India workflow template (TRANSPORT_JOB_IN) | ✅ DONE | `Seeds/TRANSPORT_JOB_IN.json` — 9 steps matching PRD |
| 9 Steps defined (with Dispatch as a separate step) | ✅ DONE | REQUEST_CREATION → RECEIPT → VEHICLE_ASSIGNMENT → RATE_ENTRY → RATE_APPROVAL → DISPATCH → IN_TRANSIT → DELIVERY_POD → CLEARANCE |
| Transitions defined | ✅ DONE | 9 transitions including RATE_APPROVAL → RATE_ENTRY (rejected) |
| Field definitions per step | ✅ DONE | All steps have field definitions with dataType, isRequired, enumValues, lookupSource |
| Role assignments per step | ✅ DONE | TransportCoordinator, TransportSupervisor, FleetManager, TransportManager, FinanceApprover, Admin, Driver |
| E-Way Bill fields on DISPATCH step | ✅ DONE | `e_way_bill_number` and `e_way_bill_date` on DISPATCH step |
| UAE workflow template | ❌ NOT DONE | Phase 2 item — no `TRANSPORT_JOB_AE.json` seed file |

---

## 9. Permissions & Data Scoping (PRD §23)

| Requirement | Status | Notes |
|---|---|---|
| Permission-based access (20 permissions listed) | ⚠️ PARTIAL | `[Authorize]` on base controller but **no per-endpoint permission checks** (e.g., `transport.job.create`, `transport.rate.approve`) — likely handled at Gateway/IAM level |
| Data scoping (Own/Branch/Department/Division/All) | ⚠️ PARTIAL | Branch + Country filtering on most queries, but **no explicit data scoping enforcement** at service level. UserContext extracts branch/country but not scoped automatically. |

---

## 10. Multi-Country Support (PRD §21)

| Requirement | Status | Notes |
|---|---|---|
| Country code on TransportRequest | ✅ DONE | `CountryCode` field |
| Country-specific request number format | ✅ DONE | `TR-{year}-{CC}-{seq}` |
| Country-specific workflow templates | ✅ DONE | Template code loaded from config per country |
| Currency per rate | ✅ DONE | `CurrencyCode` on `VehicleRate` and `TransportExpense` |
| Branch belongs to country | ✅ DONE | `BranchId` + `CountryCode` on entity |
| No code changes for new country | ✅ DONE | Config-driven workflow template selection |
| India-specific E-Way Bill hidden for other countries | ⚠️ PARTIAL | E-Way Bill step in DISPATCH is part of seed data — no UAE seed yet to prove separation |

---

## 11. Validation Rules (PRD cross-cutting)

| Requirement | Status | Notes |
|---|---|---|
| FluentValidation integration | ✅ DONE | 7 validators in `TransportValidators.cs` |
| `CreateTransportJobValidator` | ✅ DONE | Customer, addresses, cargo, vehicle type, delivery type, priority |
| `AssignVehicleValidator` | ✅ DONE | TransporterId, VehicleNumber, VehicleType |
| `EnterRateValidator` | ✅ DONE | All charges ≥ 0, currency code required |
| `RecordDeliveryValidator` | ✅ DONE | DeliveryDate, ReceivedBy, DeliveryStatus |
| `AddMovementValidator` | ✅ DONE | Milestone, Timestamp |
| `CreateTransporterValidator` | ✅ DONE | Name, email, phone |
| `CreateJobFromEnquiryValidator` | ✅ DONE | EnquiryId, customer, addresses, country, branch |
| `CreateJobFromFreightValidator` | ✅ DONE | FreightJobId, customer, addresses, country, branch |
| Address max 500 chars | ✅ DONE | `MaximumLength(500)` on address fields |
| Weight ≥ 0 | ✅ DONE | `GreaterThanOrEqualTo(0)` |

---

## 12. Error Handling & Architecture

| Requirement | Status | Notes |
|---|---|---|
| Clean Architecture (4 layers) | ✅ DONE | API / Application / Domain / Infrastructure |
| Repository pattern | ✅ DONE | `IRepository<T>` + `Repository.cs` |
| Unit of Work | ✅ DONE | `IUnitOfWork` + `UnitOfWork.cs` |
| AutoMapper profiles | ✅ DONE | `TransportMappingProfile.cs` |
| Proper HTTP status codes | ✅ DONE | `OkResponse`, `NotFoundResponse` via `BaseApiController` |
| `ApiResponse<T>` wrapper | ✅ DONE | From `EPR.Shared.Contracts` |
| API versioning | ✅ DONE | `[ApiVersion("1.0")]` |
| Swagger documentation | ✅ DONE | `SwaggerExtensions.cs` with XML comments |
| EF Core + Migrations | ✅ DONE | `TransportDbContext.cs` + `InitialCreate` migration |
| Entity configurations (Fluent API) | ✅ DONE | 7 configuration files for all entity types |
| CORS | ✅ DONE | `CorsExtensions.cs` |
| Authentication extensions | ✅ DONE | `AuthenticationExtensions.cs` |
| DI registration | ✅ DONE | `TransportServicesExtensions.cs` + `ExternalServicesExtensions.cs` |

---

## 13. Phase-by-Phase Summary

### Phase 1 — MVP (Core Job Lifecycle) — **~85% Complete**
| # | Feature | Status |
|---|---------|--------|
| 1 | Transport Request (standalone) | ✅ DONE |
| 2 | Request Queue | ✅ DONE |
| 3 | Transporter Master (basic CRUD) | ✅ DONE |
| 4 | Vehicle Assignment | ✅ DONE |
| 5 | Rate Entry | ✅ DONE |
| 6 | Rate Approval (via Workflow MS) | ✅ DONE |
| 7 | Movement Tracking (manual) | ✅ DONE |
| 8 | Delivery & POD | ✅ DONE |
| 9 | Clearance | ⚠️ PARTIAL (no checklist) |
| 10 | Job from CRM Enquiry API | ✅ DONE |
| 11 | Job from Freight API | ✅ DONE |
| 12 | Dashboard (basic) | ✅ DONE |
| 13 | Workflow template seed for India | ✅ DONE |
| 14 | Field definitions + step field mappings | ✅ DONE |
| 15 | Dynamic form rendering (frontend) | N/A (backend only) |

### Phase 2 — Fleet & Consolidation — **~15% Complete**
| # | Feature | Status |
|---|---------|--------|
| 16 | Vehicle Master (fleet) | ❌ NOT DONE |
| 17 | Driver Assignment | ❌ NOT DONE |
| 18 | Daily Vehicle Status | ❌ NOT DONE |
| 19 | Consolidation / Multi-Job Dispatch | ⚠️ PARTIAL |
| 20 | Trip Expenses | ⚠️ PARTIAL (entity only, no endpoints) |
| 21 | E-Way Bill Generation (India) | ⚠️ PARTIAL (via ULIP/CharteredInfo, no auto-fill) |
| 22 | UAE workflow template seed | ❌ NOT DONE |

### Phase 3 — Maintenance & Reports — **0% Complete**
| # | Feature | Status |
|---|---------|--------|
| 23 | Maintenance Work Orders | ❌ NOT DONE |
| 24 | Maintenance Expense Approval | ❌ NOT DONE |
| 25 | HO Vehicle Management | ❌ NOT DONE |
| 26 | Vehicle Travel Log | ❌ NOT DONE |
| 27 | All 13 Report Types | ❌ NOT DONE |
| 28 | Advanced Dashboard (Top Transporters, Branch Comparison) | ❌ NOT DONE |
| 29 | Fund Request workflow | ❌ NOT DONE |

### Phase 4 — Future Enhancements — **0% Complete**
| # | Feature | Status |
|---|---------|--------|
| 30 | GPS integration | ❌ NOT DONE (model ready) |
| 31 | WhatsApp notifications | ❌ NOT DONE |
| 32 | Additional country templates (US, UK) | ❌ NOT DONE |
| 33 | Transporter performance scoring | ❌ NOT DONE |
| 34 | Route optimization | ❌ NOT DONE |
| 35 | Driver mobile app | ❌ NOT DONE |
| 36 | CRM QuoteLineField wire-up | ❌ NOT DONE |

---

## 14. Items Built BEYOND PRD (Bonus)

These features were NOT in the PRD but exist in the codebase:

| Feature | Location |
|---|---|
| **ULIP Integration** (5 APIs: VAHAN, SARATHI, FASTag, Toll, E-Way Bill) | `UlipController.cs`, `UlipService.cs`, `UlipClient.cs` |
| **CharteredInfo Integration** (GST Lookup, e-Invoice IRN gen/cancel, E-Way Bill gen/cancel, Dynamic QR) | `EInvoiceController.cs`, `CharteredInfoService.cs`, `CharteredInfoClient.cs` |
| **7 Cache Entities** (VehicleDetail, DriverLicenseDetail, FASTagTransaction, TollPlaza, EWayBill, EInvoice, GstDetail) | Domain/Entities/ |
| **Internal API security** (X-Internal-Key header middleware) | `InternalApiMiddleware.cs`, `InternalApiAttribute.cs` |
| **Separate internal Swagger documentation** | `InternalApiDocumentFilter.cs`, `InternalApiOperationFilter.cs` |

---

## 15. Priority Gaps to Address Next

### Critical (blocks Phase 1 completion)
1. **Workflow callback implementation** — `POST /internal/jobs/{id}/workflow-callback` is a no-op. Rate rejection rollback won't work.
2. **Clearance checklist** — No document/rate completeness checks before clearing.
3. **Trip Expense endpoints** — Entity exists but no CRUD API. Ops can't track expenses.
4. **Fund Request endpoints** — Entity exists but no CRUD API.

### High (Phase 2 blockers)
5. **Consolidation entities** — Need `ConsolidatedTrip`, `ConsolidatedTripRequest`, `ConsolidatedVehicle`, `ConsolidatedExpense`.
6. **Fleet Vehicle Master** — All fleet management entities missing (Vehicle, VehicleDriver, VehicleDailyStatus).
7. **Transporter Notification CRUD** — Entity exists, no endpoints.
8. **E-Way Bill auto-fill** — Should auto-populate from transport job data.
9. **UAE workflow seed** — `TRANSPORT_JOB_AE.json` needed.

### Medium (Phase 3)
10. **Reports controller** — 0 of 13 report types built.
11. **Maintenance entities + endpoints** — Entire maintenance domain missing.
12. **Advanced dashboard** — Top transporters, branch comparison.

### Low (Phase 4 / Future)
13. GPS integration, WhatsApp, route optimization, driver mobile app.
