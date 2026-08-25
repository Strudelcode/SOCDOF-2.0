# Open Tasks

## v1.9.0 Verification

- [ ] Run `dotnet build` and `dotnet publish -c Release` on a machine with the .NET 8 SDK and Windows desktop targeting support.
- [ ] Validate the documentation page in desktop and mobile browsers.

## v1.9.0 Follow-up Work

- [ ] Add localized resource-backed documentation content when the application localization system is available.
- [ ] Add a generated API schema or machine-readable endpoint examples.

## v1.8.0 Verification

- [ ] Run `dotnet build` and `dotnet publish -c Release` on a machine with the .NET 8 SDK and Windows desktop targeting support.
- [ ] Verify the Amazon cart URL flow with valid Amazon product ASIN values on Windows.

## v1.8.0 Follow-up Work

- [ ] Add real localized resources for the new branding and cart-link UI strings.
- [ ] Add logo assets and set `<ApplicationIcon>` when approved branding files are supplied.

## v1.7.0 Verification

- [ ] Run `dotnet build` and `dotnet publish -c Release` on a machine with the .NET 8 SDK and Windows desktop targeting support.

## v1.7.0 Follow-up Work

- [ ] Add automated tests for ICS escaping, EML MIME structure, and offline export actions.
- [ ] Add configurable calendar duration and localized export templates.

## v1.6.0 Verification

- [ ] Run `dotnet build` and `dotnet publish -c Release` on a machine with the .NET 8 SDK and Windows desktop targeting support.

## v1.6.0 Follow-up Work

- [ ] Add EF Core migrations once the data model is approved for production schema evolution.
- [ ] Add the localized resource system for German, English, French, and Spanish UI text.
- [ ] Add order editing, cancellation, and detailed order history workflows.
- [ ] Add automated tests for database initialization, WAL verification, backup creation, and retention.
- [ ] Verify portable release publishing and configure self-contained publishing when the target runtime is selected.
- [ ] Add an optional API integration test suite for the local read-only endpoints.
