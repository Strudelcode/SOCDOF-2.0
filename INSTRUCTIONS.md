# SOCDOF Development & Agent Guidelines

This document defines the operational standards, architecture rules, versioning protocol, task workflow, storage requirements, and quality gates for **SOCDOF** (*Strudel's Organization, Commerce & Documentation Offline Flow*).

These instructions apply to all development work, refactoring, bug fixes, release preparation, and agent-assisted changes in the repository.

---

## 1. Global Core Configuration

### 1.1 Dynamic Application Identity

- The application name MUST be managed centrally and dynamically across the codebase.
- Use a central configuration value such as `AppConfig.AppName = "SOCDOF"`.
- Do not hardcode the application name in UI labels, views, dialogs, window titles, or file paths.
- Changes to the application name MUST be made through the central configuration mechanism.

### 1.2 Offline-First Native Desktop Architecture

- SOCDOF MUST be a native Windows desktop application built with C# and .NET 8 or newer.
- The UI framework MUST be WPF or WinUI 3, consistent with the existing project implementation.
- The application MUST run fully offline.
- Cloud services and required online dependencies are prohibited unless explicitly approved by the user.
- The application SHOULD have a low memory footprint and no subscription dependency.
- Release builds SHOULD run without requiring users to install a separate .NET runtime when a self-contained publish is requested.

### 1.3 Language and Documentation Standards

- Developer notes, internal technical documentation, release notes under `versions/`, code comments, commit messages, and task files MUST be written strictly in English.
- User-facing UI strings, labels, tooltips, and dialogs MUST use the centralized localization system.
- The localization system SHOULD support German, English, French, and Spanish where the application requires those languages.
- Do not place user-facing text directly in code when a localized resource is available.

---

## 2. Repository Directory Structure

The repository SHOULD maintain the following structure:

```text
SOCDOF/
├── INSTRUCTIONS.md
├── AppConfig.cs
├── versions/
│   ├── V1.md
│   └── releases/
│       └── v1-release.md
├── todo/
│   ├── todo.md
│   └── completed_todo.md
└── src/
    └── ...
```

### 2.1 Directory Responsibilities

| Path | Responsibility |
| --- | --- |
| `INSTRUCTIONS.md` | Master development and agent instructions |
| `AppConfig.cs` | Central application identity and global configuration |
| `versions/V<Major>.md` | Detailed English technical history for one major version |
| `versions/releases/v<Major>-release.md` | User-facing release and feature summary for one major version |
| `todo/todo.md` | Open, planned, and in-progress tasks |
| `todo/completed_todo.md` | Permanent archive of completed tasks |
| `src/` | Application source code and implementation modules |

---

## 3. Mandatory Versioning and Release Logging Protocol

### 3.1 Semantic Versioning

All application versions MUST follow Semantic Versioning:

```text
v<Major>.<Minor>.<Patch>
```

Examples:

- `v1.0.0`
- `v1.1.0`
- `v1.1.5`

### 3.2 Major Version Rule

- A new major version MUST NEVER be initiated automatically.
- Moving from `v1.x.x` to `v2.0.0` requires explicit discussion with and consent from the user.
- Minor and patch versions MAY be incremented flexibly when the change scope justifies it.

### 3.3 Technical Release History

Technical changes for the current major version MUST be appended to its dedicated file under `versions/`.

For example, all `v1.x.x` changes belong in:

```text
versions/V1.md
```

Each technical release entry MUST include:

- Version number
- Date
- Detailed description of changes
- Affected modules
- Bug fixes and refactoring details where applicable
- Build verification status
- English-only technical wording

Use a consistent entry format such as:

```markdown
## v1.2.0 - 2026-08-25

### Changes
- Added ...
- Updated ...

### Affected Modules
- `src/...`
- `AppConfig.cs`

### Build Verification
- `dotnet build`: Passed
- `dotnet publish -c Release`: Passed
```

### 3.4 User-Facing Release Summary

User-facing release documentation MUST be maintained in:

```text
versions/releases/v<Major>-release.md
```

This file MUST summarize capabilities across releases in an accessible format. It SHOULD highlight:

- New features
- UI improvements
- Functional capabilities
- Important user-visible bug fixes
- Relevant workflow improvements

Technical implementation details that are not useful to end users SHOULD be omitted from this summary.

### 3.5 Synchronized Version Updates

Every version increment MUST be updated across all applicable locations in the same change. Check and update:

1. `AppConfig.Version` and/or central assembly metadata
2. Project configuration files such as `.csproj`
3. `todo/todo.md`
4. `todo/completed_todo.md`
5. `versions/V<Major>.md`
6. `versions/releases/v<Major>-release.md`

No release is considered complete while these locations contain inconsistent version values.

---

## 4. Two-File Todo and Task Archiving Workflow

### 4.1 Strict Separation

#### `todo/todo.md`

This file is strictly reserved for:

- Open tasks marked with `[ ]`
- Planned tasks
- In-progress tasks
- Clearly stated follow-up work

Keep this file clean, structured, and focused on work that remains.

#### `todo/completed_todo.md`

This file is the permanent archive for:

- Completed tasks marked with `[x]`
- Implementation details
- Verification results
- The release version associated with the completed work

Completed tasks MUST NOT be permanently deleted.

### 4.2 Archiving Protocol

As soon as a feature, bug fix, or refactoring task has been implemented, verified, and completed:

1. Mark the task as completed.
2. Move the completed task from `todo/todo.md` to `todo/completed_todo.md`.
3. Add implementation details and verification results.
4. Add the applicable release version tag.
5. Ensure that `todo/todo.md` contains only remaining work.

This archiving step is mandatory and MUST happen immediately after verification.

Suggested archive format:

```markdown
- [x] Add ...
  - Completed: 2026-08-25
  - Release: v1.2.0
  - Details: ...
  - Verification: `dotnet build` passed; `dotnet publish -c Release` passed.
```

---

## 5. Strict No-Mock and No-Placeholder Data Rule

### 5.1 Real-World Data Policy

Do not pre-fill or inject artificial records into production application modules. This includes:

- Demo records
- Dummy contacts
- Fake support tickets
- Sample invoices
- Mock products
- Placeholder customers
- Artificial dashboard metrics

Mock data is permitted only in isolated automated tests, test fixtures, or explicitly marked development-only environments. It MUST NOT appear in the normal user experience or production data store.

### 5.2 Clean Initial States

Every module MUST initialize with clean empty states, such as:

- Empty collections: `[]`
- Empty database tables
- No fabricated statistics
- No invented user records

Empty states MUST provide:

- A clear explanation of what will appear after the user creates data
- An intuitive onboarding guide where appropriate
- A visible and clear creation action, such as **Create**, **Add**, or **New**
- Localized user-facing text

Users MUST enter their own real operational data.

---

## 6. Storage Engine, Security, and Backup System

### 6.1 SQLite and WAL Mode

- Persistent application data MUST use SQLite.
- SQLite MUST run in Write-Ahead Logging mode (`WAL`) to support efficient concurrent local reads and writes.
- Database initialization MUST configure and verify the required journal mode.
- Database connections, transactions, and migrations MUST be handled safely and consistently.
- The application MUST avoid storing operational data in temporary, cloud, or repository directories.

### 6.2 Local Application Data Directory

Database files, user settings, and local application logs MUST reside under:

```text
%APPDATA%\SOCDOF\
```

The application SHOULD create and use dedicated subdirectories where appropriate, for example:

```text
%APPDATA%\SOCDOF\backups\
%APPDATA%\SOCDOF\logs\
```

Paths MUST be resolved through the operating system's application-data APIs instead of being hardcoded to a user-specific absolute path.

### 6.3 Automated Database Backups

The application MUST automatically create a timestamped database backup on startup and/or during configured scheduled triggers.

Backup files MUST follow this naming pattern:

```text
SOCDOF_backup_YYYYMMDD_HHMMSS.db
```

Backups MUST be stored in:

```text
%APPDATA%\SOCDOF\backups\
```

The backup service MUST:

- Create the backup directory when it does not exist.
- Produce a consistent SQLite backup.
- Use a timestamp that is unambiguous and sortable.
- Apply an automated retention policy.
- Delete older snapshots after a successful new backup.
- Retain only the 10 most recent backup files.
- Log backup failures without corrupting or deleting the source database.

---

## 7. Quality and Build Verification Standards

Before completing any development iteration, feature, bug fix, refactoring task, or prompt execution, verify all applicable quality gates.

### 7.1 Required Build Checks

The following commands MUST pass with zero compilation errors:

```bash
dotnet build
```

Release publishing MUST compile successfully as a portable executable:

```bash
dotnet publish -c Release
```

When the project supports self-contained single-file publishing, use the project's configured runtime identifier and verify the resulting artifact accordingly.

### 7.2 Completion Checklist

Before reporting work as complete:

- Compilation passes cleanly with zero errors.
- Release publishing completes successfully.
- Relevant tests and static checks pass.
- Active tasks are current in `todo/todo.md`.
- Verified completed tasks are archived in `todo/completed_todo.md`.
- Detailed technical release notes are appended to `versions/V<Major>.md`.
- User-facing release notes are updated in `versions/releases/v<Major>-release.md`.
- Version values are synchronized across all applicable project locations.
- No mock or placeholder production data was introduced.
- SQLite, WAL, local storage, and backup behavior remain compliant.

If a required verification step cannot be run, document the reason and the exact remaining risk in the relevant release notes and completion summary.

---

## 8. Change Discipline

- Keep changes focused on the requested task.
- Follow existing project conventions and abstractions.
- Do not introduce new frameworks or libraries without verifying that they are compatible with the project and justified by the requirement.
- Do not overwrite unrelated user changes.
- Prefer small, reviewable changes over broad refactoring.
- Add tests for behavior with meaningful regression risk.
- Keep all technical documentation and task records synchronized with the implementation.

---

## 9. Definition of Done

A change is done only when:

1. The implementation is complete.
2. The application builds successfully.
3. Release publishing succeeds.
4. Relevant behavior has been tested.
5. No prohibited mock data has been introduced.
6. Todo items have been updated and archived as required.
7. Technical and user-facing release documentation has been updated.
8. All applicable version locations are synchronized.
9. Any verification limitation is explicitly documented.
