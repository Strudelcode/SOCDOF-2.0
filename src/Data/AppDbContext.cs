using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SOCDOF.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SaleOrder> SaleOrders => Set<SaleOrder>();
    public DbSet<SaleOrderLine> SaleOrderLines => Set<SaleOrderLine>();
    public DbSet<StockMove> StockMoves => Set<StockMove>();

    public static AppDbContext Create()
    {
        AppConfig.EnsureDirectories();
        return new AppDbContext();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = AppConfig.DatabasePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            }.ToString();

            optionsBuilder.UseSqlite(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(partner => partner.Id);
            entity.Property(partner => partner.Name).IsRequired().HasMaxLength(200);
            entity.Property(partner => partner.Email).HasMaxLength(320);
            entity.Property(partner => partner.Phone).HasMaxLength(50);
            entity.Property(partner => partner.Address).HasMaxLength(500);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).IsRequired().HasMaxLength(200);
            entity.Property(product => product.SKU).IsRequired().HasMaxLength(100);
            entity.HasIndex(product => product.SKU).IsUnique();
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.Property(product => product.Unit).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<SaleOrder>(entity =>
        {
            entity.HasKey(order => order.Id);
            entity.Property(order => order.OrderNumber).IsRequired().HasMaxLength(100);
            entity.HasIndex(order => order.OrderNumber).IsUnique();
            entity.Property(order => order.TotalAmount).HasPrecision(18, 2);

            entity.HasOne(order => order.Partner)
                .WithMany()
                .HasForeignKey(order => order.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SaleOrderLine>(entity =>
        {
            entity.HasKey(line => line.Id);
            entity.Property(line => line.Quantity).HasPrecision(18, 3);
            entity.Property(line => line.UnitPrice).HasPrecision(18, 2);
            entity.Property(line => line.TotalAmount).HasPrecision(18, 2);

            entity.HasOne(line => line.SaleOrder)
                .WithMany(order => order.Lines)
                .HasForeignKey(line => line.SaleOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(line => line.Product)
                .WithMany()
                .HasForeignKey(line => line.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockMove>(entity =>
        {
            entity.HasKey(move => move.Id);
            entity.Property(move => move.Quantity).HasPrecision(18, 3);
            entity.Property(move => move.Type).HasConversion<string>().HasMaxLength(10);

            entity.HasOne(move => move.Product)
                .WithMany()
                .HasForeignKey(move => move.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public void EnsureCurrentSchema()
    {
        Database.OpenConnection();

        try
        {
            using var command = Database.GetDbConnection().CreateCommand();
            command.CommandText = "PRAGMA table_info(\"SaleOrders\");";

            var hasDeliveryDate = false;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (string.Equals(reader.GetString(1), "DeliveryDate", StringComparison.OrdinalIgnoreCase))
                    {
                        hasDeliveryDate = true;
                        break;
                    }
                }
            }

            if (!hasDeliveryDate)
            {
                using var alterCommand = Database.GetDbConnection().CreateCommand();
                alterCommand.CommandText = "ALTER TABLE \"SaleOrders\" ADD COLUMN \"DeliveryDate\" TEXT NULL;";
                alterCommand.ExecuteNonQuery();
            }
        }
        finally
        {
            Database.CloseConnection();
        }
    }

    public void ConfigureWalMode()
    {
        Database.OpenConnection();

        try
        {
            using var command = Database.GetDbConnection().CreateCommand();
            command.CommandText = "PRAGMA journal_mode=WAL;";
            var journalMode = command.ExecuteScalar()?.ToString();

            if (!string.Equals(journalMode, "wal", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"SQLite WAL mode could not be enabled. The database reported '{journalMode ?? "null"}'.");
            }
        }
        finally
        {
            Database.CloseConnection();
        }
    }
}

public sealed class Partner
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal StockQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    [NotMapped]
    public bool IsLowStock => StockQuantity < 5m;

    [NotMapped]
    public string StockStatus => IsLowStock ? "Niedriger Bestand" : "Verfügbar";
}

public sealed class SaleOrder
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int PartnerId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public Partner Partner { get; set; } = null!;
    public ICollection<SaleOrderLine> Lines { get; set; } = new List<SaleOrderLine>();
}

public sealed class SaleOrderLine
{
    public int Id { get; set; }
    public int SaleOrderId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public SaleOrder SaleOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public sealed class StockMove
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public StockMoveType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public Product Product { get; set; } = null!;
}

public enum StockMoveType
{
    In,
    Out
}
