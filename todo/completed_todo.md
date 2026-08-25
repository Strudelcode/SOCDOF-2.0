# Completed Tasks

## v2.0.0 Dynamic Release Notes Resolution

- [x] Resolve GitHub release descriptions from the matching major-version documentation.
  - Completed: 2026-08-25
  - Release: v2.0.0
  - Details: The workflow now derives `v2` from a semantic tag such as `v2.0.0`, prefers `versions/releases/v2-release.md`, falls back to `versions/V2.md`, and passes the resolved file through `body_path`.
  - Verification: The v2.0.0 tag resolves to `versions/releases/v2-release.md`; `git diff --check` is required before delivery.


## v2.0.0 Release Notes Automation

- [x] Configure tagged GitHub releases to use the maintained V2 release document as their release body.
  - Completed: 2026-08-25
  - Release: v2.0.0
  - Details: Added `body_path: versions/releases/v2-release.md` to the tagged release action and disabled generated notes so the curated release summary is used verbatim.
  - Verification: `git diff --check` passed; the workflow is triggered by version tags and uses the existing Windows publish artifact.


## v2.0.0 MSBuild and Release-Line Audit

- [x] Correct the MSBuild item structure, synchronize the v2.0.0 major release metadata, and update release automation.
  - Completed: 2026-08-25
  - Release: v2.0.0
  - Details: Kept all resource and optional icon declarations inside valid MSBuild groups, synchronized application/project/documentation versions, added V2 technical and user-facing release notes, and classified major tags as full releases while later minor and patch tags are pre-releases using a supported PowerShell output step.
  - Verification: `git diff --check` passed; optional assets remain safe when absent. Local `dotnet build` is unavailable because the workspace has no .NET SDK; GitHub Actions is the required Windows build verification.


## v1.9.2 CI Fixes and Documentation Navigation

- [x] Fix the reported `ICollectionView` compilation errors and update the documentation structure.
  - Completed: 2026-08-25
  - Release: v1.9.2
  - Details: Added `System.ComponentModel` to all four affected WPF code-behind files, added `features.html` and `api.html`, synchronized release links to the `SOCDOF-2.0` repository, and updated project/application version metadata. Added conditional WPF resource metadata and optional runtime loading for the supplied branding asset path.
  - Verification: `git diff --check` passed; the `dotnet` SDK is unavailable in this workspace. The attached PNG could not be synchronized as `src/Assets/logo.png`, so the final Windows build remains dependent on that asset being present in the repository.


## v1.9.1 System Audit

- [x] Complete the cross-module SOCDOF system audit and close the verification backlog.
  - Completed: 2026-08-25
  - Release: v1.9.1
  - Details: Audited startup lifecycle, centralized branding, SQLite/WAL initialization, existing-database schema compatibility, dashboard, partners, products, inventory, sales, local read-only API, offline ICS/EML exports, Amazon cart links, no-mock data behavior, and ten-file backup retention. Added the `SaleOrders.DeliveryDate` compatibility check and centralized runtime diagnostic labels.
  - Verification: Static integration checks and `git diff --check` passed; `dotnet build` and `dotnet publish -c Release` remain unavailable because this workspace does not provide the .NET SDK.


## v1.9.0 Documentation Website

- [x] Add a responsive standalone documentation website for SOCDOF 2.0.
  - Completed: 2026-08-25
  - Release: v1.9.0
  - Details: Added a framework-free `index.html` with responsive dark-mode styling, product and architecture overview, local REST API endpoint documentation, and GitHub release links. The page contains no mock operational data or external JavaScript dependency.
  - Verification: HTML structure and `git diff --check` reviewed; local .NET build verification remains pending a .NET SDK environment.


## v1.8.0 Branding and Amazon Cart Links

- [x] Update SOCDOF branding and add the local Amazon cart-link workflow.
  - Completed: 2026-08-25
  - Release: v1.8.0
  - Details: Updated the central application identity to `SOCDOF 2.0`, preserved the existing `%APPDATA%\\SOCDOF\\` storage location, prepared `src/Assets/` for approved logo files, and added a local multi-item Amazon cart URL generator using selected product SKUs. The generated URL opens through the Windows default browser without cloud APIs or credentials.
  - Verification: `git diff --check` passed; local .NET build verification remains pending a .NET SDK environment. No logo files were available, so no conditional icon reference was added.


## v1.7.0 Offline Calendar and E-Mail Exports

- [x] Add offline ICS calendar and EML e-mail draft exports for sales orders and partners.
  - Completed: 2026-08-25
  - Release: v1.7.0
  - Details: Added local `.ics` export for order and optional delivery events, EML drafts with plain-text and HTML alternatives, clipboard copying, and Windows save dialogs. Added export actions to the sales detail panel and the partner overview without external cloud dependencies.
  - Verification: `git diff --check` passed; local build verification remains pending a .NET SDK environment.


## v1.6.0 Local Read-Only API

- [x] Add an optional localhost-only REST API server for local third-party read access.
  - Completed: 2026-08-25
  - Release: v1.6.0
  - Details: Added a Kestrel-backed server controlled by `AppConfig.LocalApiEnabled`, bound to `http://localhost:5050`, with read-only `GET /api/status`, `GET /api/products`, and `GET /api/sales` endpoints. The server uses fresh EF Core contexts per request, logs failures without exposing exception details, and is disposed during application shutdown.
  - Verification: `git diff --check` passed; local build verification remains pending a .NET SDK environment.


## v1.5.0 Main Dashboard

- [x] Implement the live SOCDOF dashboard with business metrics and recent activity.
  - Completed: 2026-08-25
  - Release: v1.5.0
  - Details: Added live SQLite metrics for partners, products, low-stock products, sales orders, and revenue; quick actions for partner/product/sale creation; and a combined table of the five latest sales and stock movements. Empty databases display zero values and no fabricated records.
  - Verification: `git diff --check` passed; local build verification remains pending a .NET SDK environment.


## v1.4.1 GitHub Actions Build Workflow

- [x] Add an automated Windows build and artifact workflow for pushes to `main`.
  - Completed: 2026-08-25
  - Release: v1.4.1
  - Details: Added `.github/workflows/build.yml` using `windows-latest`, .NET 8 setup, restore, Release build, self-contained `win-x64` single-file publish, and the `SOCDOF-Windows-Executable` artifact upload.
  - Verification: YAML structure and Git diff checks passed; the hosted GitHub runner verification is pending the next push to `main`.


## v1.4.0 Sales Orders

- [x] Implement the sales order overview and completion workflow.
  - Completed: 2026-08-25
  - Release: v1.4.0
  - Details: Added sales overview with order number, date, partner, and total; partner selection; dynamic product lines with quantity, unit price, line total, and automatic order total; and transactional stock decrement with `StockMoveType.Out` entries for each product.
  - Verification: `git diff --check` passed; `dotnet build` could not run because the environment does not provide the `dotnet` command.


## v1.3.0 Product and Inventory Management

- [x] Implement the product catalog and inventory movement workflow.
  - Completed: 2026-08-25
  - Release: v1.3.0
  - Details: Added product DataGrid with SKU, name, price, stock, and unit columns; create/edit dialogs; low-stock highlighting below 5 units; and inventory inbound/outbound dialogs. Every successful stock change updates `Product.StockQuantity` and creates a `StockMove` in the same SQLite transaction.
  - Verification: `git diff --check` passed; `dotnet build` could not run because the environment does not provide the `dotnet` command.


## v1.2.0 Partner Management

- [x] Implement partner overview and CRUD management.
  - Completed: 2026-08-25
  - Release: v1.2.0
  - Details: Added a SQLite-backed partner DataGrid with search, create/edit dialog, required-name validation, delete confirmation, reference-aware delete handling, and a clean empty state with a "Neuen Partner anlegen" action.
  - Verification: `git diff --check` passed; `dotnet build` could not run because the environment does not provide the `dotnet` command.


## v1.1.0 Main Window Navigation

- [x] Create the native SOCDOF main window with module navigation and clean empty states.
  - Completed: 2026-08-25
  - Release: v1.1.0
  - Details: Added the dynamic application title, modern sidebar navigation for Dashboard, Partner, Produkte, Verkäufe, and Lager, a central `ContentControl`, and reusable empty-state views with primary create actions. No fake records were added.
  - Verification: `git diff --check` passed; `dotnet build` could not run because the environment does not provide the `dotnet` command.

## v1.0.1 Core Data Models

- [x] Add the initial SQLite entities and relationships.
  - Completed: 2026-08-25
  - Release: v1.0.1
  - Details: Added empty EF Core tables for `Partner`, `Product`, `SaleOrder`, `SaleOrderLine`, and `StockMove`, including foreign keys, order lines, the `In`/`Out` stock movement enum, and basic field constraints.
  - Verification: `EnsureCreated()` remains executed during application startup; build verification is pending a .NET SDK environment.

## v1.0.0 Foundation

- [x] Create the .NET 8 WPF application skeleton.
  - Completed: 2026-08-25
  - Release: v1.0.0
  - Details: Added the WinExe WPF project, application entry point, and initial empty main window.
  - Verification: Source implementation completed; `dotnet build` could not run because the environment does not provide the `dotnet` command.

- [x] Add centralized application identity and AppData paths.
  - Completed: 2026-08-25
  - Release: v1.0.0
  - Details: Added `AppConfig` with the required application name/version and paths under `%APPDATA%\\SOCDOF\\` and `%APPDATA%\\SOCDOF\\backups\\`.
  - Verification: Source implementation completed; build verification is pending a .NET SDK environment.

- [x] Add EF Core SQLite initialization with WAL mode verification.
  - Completed: 2026-08-25
  - Release: v1.0.0
  - Details: Added `AppDbContext` with Microsoft EF Core SQLite, database creation, and a checked `PRAGMA journal_mode=WAL;` command.
  - Verification: Source implementation completed; build verification is pending a .NET SDK environment.

- [x] Add startup database backup and ten-file retention.
  - Completed: 2026-08-25
  - Release: v1.0.0
  - Details: Added `BackupService` using SQLite's online backup API, timestamped backup names, failure logging, and retention of the ten newest files.
  - Verification: Source implementation completed; build verification is pending a .NET SDK environment.

- [x] Add v1.0.0 technical and user-facing release documentation.
  - Completed: 2026-08-25
  - Release: v1.0.0
  - Details: Added `versions/V1.md` and `versions/releases/v1-release.md`.
  - Verification: Documentation reviewed for English technical/release content.

## Verification Limitations

- Local `dotnet build` remains pending because the current environment does not provide the `dotnet` command.


- Local `dotnet build` remains pending because the current environment does not provide the `dotnet` command.


- The v1.4.1 GitHub Actions workflow has not run in this workspace because it is triggered by a push to `main`.


- `dotnet build` was requested for v1.4.0 but could not run because the current environment does not provide the `dotnet` command.


- `dotnet build` was requested for v1.3.0 but could not run because the current environment does not provide the `dotnet` command.


- `dotnet build` was requested for v1.2.0 but could not run because the current environment does not provide the `dotnet` command.


- `dotnet build` was requested for v1.1.0 but could not run because the current environment does not provide the `dotnet` command.

- `dotnet build` was requested but could not run because the current environment does not provide the `dotnet` command.
- `dotnet publish -c Release` remains to be run on a machine with the .NET 8 SDK and Windows desktop targeting support.
