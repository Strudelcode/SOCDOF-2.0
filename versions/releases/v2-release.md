# SOCDOF 2.0 Release

## v2.0.1 - 2026-08-26

This patch release makes startup failures visible and turns the Windows setup package into a required tagged-release output.

### Highlights

- Global WPF and AppDomain exception handling now displays the exact failure details in a MessageBox instead of failing silently.
- SQLite initialization, AppData directory creation, and startup backup failures are surfaced with actionable diagnostics.
- Optional icon and logo assets remain safe when absent, while invalid or unreadable assets are reported clearly.
- Tagged GitHub Actions builds validate and publish `SOCDOF_setup.exe` as the primary Windows release asset alongside `SOCDOF_2.0.exe`.
- Application and project metadata are synchronized to `v2.0.1`.

SOCDOF remains offline-first and self-contained. No cloud service or external runtime installation is required for the published Windows application.

## v2.0.0 - 2026-08-25

SOCDOF 2.0 is the first major release under the new SOCDOF 2.0 identity.

### Highlights

- More reliable Windows builds with the corrected `ICollectionView` compilation issue fixed across the inventory, partner, product, and sales screens.
- A cleaner multi-page documentation experience with dedicated pages for the product overview, features, and local REST API.
- Consistent version and branding metadata across the application, documentation, and release workflow.
- Release automation that treats `v2.0.0`-style major tags as regular releases while marking later minor and patch tags as pre-releases.
- Optional local logo support that does not prevent the application from starting when an asset is unavailable.
- Self-contained Windows executable output named `SOCDOF_2.0.exe` with Publisher `Yuri / Strudel` and the official SOCDOF 2.0 product metadata.
- Optional tagged-release installer output named `SOCDOF_setup.exe` for a faster Windows setup experience.

SOCDOF remains an offline-first native Windows application. Existing local SQLite data, WAL behavior, backups, exports, and read-only localhost API behavior remain part of the product. The release pipeline packages `SOCDOF_2.0.exe` and can attach `SOCDOF_setup.exe` for tagged releases.

Download builds and view release history at the [SOCDOF 2.0 GitHub Releases](https://github.com/Strudelcode/SOCDOF-2.0/releases) page.

### Verification Note

The source and repository checks passed. Local .NET build and publish verification could not be executed in the current workspace because the .NET SDK is unavailable; the GitHub Actions Windows runner provides the final executable verification.
