# SOCDOF 2.0 Release

## v2.0.4 - 2026-08-26

This patch release makes installed startup failures diagnosable and keeps the Windows package independent of the install drive and an external .NET runtime.

### Highlights

- Fatal startup and unhandled application errors are written to `%LOCALAPPDATA%\\SOCDOF\\logs\\error.log`.
- The application still displays a Windows error dialog with the exact exception details instead of failing silently.
- Database and backup paths remain user-specific and are resolved through Windows application-data locations, so installing on another drive does not change where operational data is stored.
- The Windows release uses a self-contained `win-x64` publish and packages that runtime into `SOCDOF_setup.exe`.
- The official Windows download remains the installer only: `SOCDOF_setup.exe`.
- Application, project, installer, website, and preview metadata are synchronized to `v2.0.4`.

## v2.0.3 - 2026-08-26

This patch release enables a fully user-controlled Windows installation without an administrator prompt.

### Highlights

- `SOCDOF_setup.exe` installs per user with no UAC administrator request.
- The installer keeps its directory page visible, allowing a destination on any writable drive such as `E:\\SOCDOF`.
- The default destination is `{localappdata}\\Programs\\SOCDOF 2.0`.
- Start menu, optional desktop shortcut, and standard uninstall entries remain available.
- The official Windows download remains the installer only: `SOCDOF_setup.exe`.
- Application, project, installer, and documentation metadata are synchronized to `v2.0.3`.

## v2.0.2 - 2026-08-26

This patch release adds automatic version delivery and a simple local documentation preview.

### Highlights

- Successful `main` builds read the active semantic version from `AppConfig.cs` and create the matching immutable Git tag.
- `vX.0.0` tags are published as regular releases; minor and patch tags such as `v2.0.2` are published as pre-releases.
- Release descriptions continue to resolve from `versions/releases/vX-release.md`, with `versions/VX.md` as a fallback.
- Added a dependency-free preview command: `npm run dev -- -p 3000 -H 0.0.0.0`.
- Tagged builds publish only `SOCDOF_setup.exe`; the self-contained application is packaged inside the installer and is not offered as a separate download.
- The installer uses `{autopf}\\SOCDOF 2.0` by default while allowing the user to choose a different destination, and provides Start menu, optional desktop, and uninstall entries.
- Synchronized application, project, and documentation metadata to `v2.0.2`.

## v2.0.1 - 2026-08-26

This patch release makes startup failures visible and turns the Windows setup package into a required tagged-release output.

### Highlights

- Global WPF and AppDomain exception handling now displays the exact failure details in a MessageBox instead of failing silently.
- SQLite initialization, AppData directory creation, and startup backup failures are surfaced with actionable diagnostics.
- The uploaded `socdof_v2_icon.ico` is used as the Windows application icon, with `socdof_v2_icon.png` as the visual fallback; missing or invalid assets are handled clearly.
- Tagged GitHub Actions builds validate and publish `SOCDOF_setup.exe` as the only official Windows release asset.
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
- The Windows setup package uses the self-contained application build and carries Publisher `Yuri / Strudel` with the official SOCDOF 2.0 product metadata.
- Tagged releases provide `SOCDOF_setup.exe` as the official Windows download with a selectable installation directory.

SOCDOF remains an offline-first native Windows application. Existing local SQLite data, WAL behavior, backups, exports, and read-only localhost API behavior remain part of the product. The release pipeline publishes only `SOCDOF_setup.exe`; the self-contained application binary is packaged inside the installer and is not offered as a separate download.

Download builds and view release history at the [SOCDOF 2.0 GitHub Releases](https://github.com/Strudelcode/SOCDOF-2.0/releases) page.

### Verification Note

The source and repository checks passed. Local .NET build and publish verification could not be executed in the current workspace because the .NET SDK is unavailable; the GitHub Actions Windows runner provides the final executable verification.
