# SOCDOF v1 Release

## v1.9.0 - Documentation Website

SOCDOF 2.0 now has a focused documentation website for understanding the local workspace and its integrations.

### Included

- Responsive documentation page for desktop, tablet, and mobile screens.
- Clear overview of offline-first operation and SQLite-WAL storage.
- Feature summaries for partners, products, inventory, sales, ICS/EML exports, and Amazon cart links.
- Read-only local REST API reference for status, products, and sales.
- Direct links to GitHub Releases for downloads and release history.
- Self-contained design without a JavaScript framework or required online service.


## v1.8.0 - SOCDOF 2.0 Branding and Amazon Cart Links

SOCDOF now carries the `SOCDOF 2.0` application identity and provides a local product-to-cart link workflow.

### Included

- Updated application branding to `SOCDOF 2.0`.
- Prepared the local assets folder for future logo and icon files.
- Select one or more catalog products and generate a direct Amazon multi-item cart link from their SKUs.
- Open the generated link in the user's standard browser.
- No Amazon account credentials, cloud API, or external integration is required by SOCDOF.
- Existing local data remains in the established application-data location.

No demo products or placeholder branding files were added. The application icon will be linked when an approved icon asset is supplied.


## v1.7.0 - Offline Calendar and E-Mail Exports

SOCDOF now supports local exports for calendar entries and e-mail drafts without requiring an online service.

### Included

- Export sales order and optional delivery dates as standard `.ics` calendar files.
- Import-compatible calendar files for Google Calendar, Outlook, and Apple Calendar.
- Generate e-mail confirmations as `.eml` drafts with plain-text and HTML content.
- Copy drafts to the clipboard or save them locally through a Windows file dialog.
- Export actions in the sales order detail area.
- Partner e-mail draft action for partners with an e-mail address.

All exports remain offline and no cloud or paid service is required.


## v1.6.0 - Local Read-Only API

SOCDOF now offers an optional local API for third-party applications that need to read current business data.

### Included

- Local read-only server at `http://localhost:5050`.
- Status endpoint with application identity and server state.
- Product endpoint with current prices and inventory levels.
- Sales endpoint with order and partner summaries.
- API disabled/enabled through a central application setting.
- Automatic server cleanup when SOCDOF closes.

The API does not provide external write access and is not exposed beyond the local machine.


## v1.5.0 - Main Dashboard

SOCDOF now opens with a live business dashboard for a quick overview of current operations.

### Included

- Partner count for customers and suppliers.
- Product count and low-stock count for products below 5 units.
- Sales order count and total revenue in EUR.
- Quick actions for creating partners, products, and sales.
- Recent activity table with the five latest sales and stock movements.
- Clear zero values and empty activity state when no data exists.

No mock statistics or fabricated records are included.


## v1.4.1 - Automated Windows Builds

SOCDOF now creates a downloadable Windows executable automatically whenever changes are pushed to the `main` branch.

### Included

- Automated Windows build on GitHub-hosted infrastructure.
- Self-contained executable that does not require a separate .NET runtime.
- Single-file `win-x64` publish output.
- Downloadable `SOCDOF-Windows-Executable` build artifact.


## v1.4.0 - Sales Orders

SOCDOF now supports completing sales orders with automatic inventory tracking.

### Included

- Overview of completed sales with order number, date, partner, and total.
- Partner selection from existing local partner records.
- Multiple product positions per order.
- Automatic calculation of line totals and order total.
- Automatic stock reduction after order completion.
- Inventory history entry for every product sold.
- Prevention of orders that exceed available stock.
- Empty state without demo sales.


## v1.3.0 - Products and Inventory

SOCDOF now supports local product and inventory management without creating any demo data.

### Included

- Product catalog with SKU, name, price, stock, and unit.
- Forms for creating and editing products.
- Inventory actions for booking goods in and out.
- Automatic inventory history for every successful stock change.
- Protection against booking out more stock than is available.
- Clear warning color and status for stock below 5 units.
- Empty states for catalogs without products.


## v1.2.0 - Partner Management

SOCDOF now provides a complete local partner workflow for managing customers and suppliers.

### Included

- Searchable partner overview.
- Partner list with name, e-mail, phone, address, and creation date.
- Dialogs for adding and editing partner details.
- Required-name validation before saving.
- Confirmation before deleting a partner.
- Clear empty state with `Neuen Partner anlegen` when the list is empty.
- No demo records or pre-filled data.


## v1.1.0 - Main Window Navigation

SOCDOF now opens with a clear native workspace that makes the main modules directly accessible.

### Included

- Sidebar navigation for Dashboard, Partner, Produkte, Verkäufe, and Lager.
- Dedicated content area for each module.
- Clean empty states that explain when no records exist yet.
- Primary create actions ready for the upcoming workflows.
- Dynamic application name and version display.

No demo or placeholder records are included.


## v1.0.1 - Core Data Foundation

SOCDOF now has the initial local data structure required for managing partners, products, orders, and stock movements.

### Included

- Partner records with contact and address information.
- Product records with SKU, price, stock quantity, and unit.
- Sales orders with associated order lines and partners.
- Stock movement records for incoming and outgoing quantities.
- Clean empty tables ready for the user's own operational data.


## v1.0.0 - Initial Foundation

SOCDOF is now established as an offline-first Windows desktop application built for local operation.

### Included

- Native Windows desktop application foundation using WPF and .NET 8.
- Local SQLite storage in the user's application data directory.
- Automatic database backup when the application starts.
- Backup history limited to the ten most recent snapshots.
- Clean initial state without demo or placeholder records.

This release provides the storage and application foundation for the first SOCDOF workflows. User-created records and domain workflows will be added in subsequent releases.
