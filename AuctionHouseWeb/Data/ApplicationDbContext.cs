using Microsoft.EntityFrameworkCore;
using AuctionHouseWeb.Models;

namespace AuctionHouseWeb.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<Lot> Lots => Set<Lot>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<LotImage> LotImages => Set<LotImage>();
    public DbSet<Payment> Payments { get; set; } = null!;

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bid>()
            .Property(b => b.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Lot>()
            .Property(l => l.StartPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Lot>()
            .Property(l => l.CurrentPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Lot>()
            .Property(l => l.EstimatedValue)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<LotImage>()
            .HasOne(li => li.Lot)
            .WithMany(l => l.Images)
            .HasForeignKey(li => li.LotId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}