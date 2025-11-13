using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<Repair> Repairs { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<RentDevice> RentDevices { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<ReceiptItem> ReceiptItems { get; set; }
    public DbSet<PurchaseItem> PurchaseItems { get; set; }
    public DbSet<Scale> Scales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar clave primaria compuesta para ReceiptItem
        modelBuilder.Entity<ReceiptItem>()
            .HasKey(ri => new { ri.ReceiptId, ri.RepairId });

        // Configurar relación Receipt -> ReceiptItems (CASCADE)
        modelBuilder.Entity<ReceiptItem>()
            .HasOne(ri => ri.Receipt)
            .WithMany(r => r.ReceiptItems)
            .HasForeignKey(ri => ri.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar relación Repair -> ReceiptItems (RESTRICT para evitar ciclos)
        modelBuilder.Entity<ReceiptItem>()
            .HasOne(ri => ri.Repair)
            .WithMany(r => r.ReceiptItems)
            .HasForeignKey(ri => ri.RepairId)
            .OnDelete(DeleteBehavior.Restrict);
        //
        //---------------------------------------------------------------------------------------
        modelBuilder.Entity<RentDevice>()
        .HasKey(rd => new { rd.DeviceId, rd.RentId });

        modelBuilder.Entity<RentDevice>()
            .HasOne(rd => rd.Rental)
            .WithMany(r => r.RentDevices)
            .HasForeignKey(rd => rd.RentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RentDevice>()
            .HasOne(rd => rd.Device)
            .WithMany(d => d.RentedDevices)
            .HasForeignKey(rd => rd.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}