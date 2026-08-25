# SOCDOF v1 Release

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
