# Completed Tasks

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

- `dotnet build` was requested for v1.2.0 but could not run because the current environment does not provide the `dotnet` command.


- `dotnet build` was requested for v1.1.0 but could not run because the current environment does not provide the `dotnet` command.

- `dotnet build` was requested but could not run because the current environment does not provide the `dotnet` command.
- `dotnet publish -c Release` remains to be run on a machine with the .NET 8 SDK and Windows desktop targeting support.
