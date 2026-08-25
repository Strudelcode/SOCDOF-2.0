# Open Tasks

## v1.2.0 Verification

- [ ] Run `dotnet build` and `dotnet publish -c Release` on a machine with the .NET 8 SDK and Windows desktop targeting support.

## v1.2.0 Follow-up Work

- [ ] Add EF Core migrations once the data model is approved for production schema evolution.
- [ ] Add the localized resource system for German, English, French, and Spanish UI text.
- [ ] Replace temporary module actions with the first production workflow implementations for Products, Sales, and Inventory.
- [ ] Add automated tests for database initialization, WAL verification, backup creation, and retention.
- [ ] Verify portable release publishing and configure self-contained publishing when the target runtime is selected.
