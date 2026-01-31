using Microsoft.EntityFrameworkCore;

namespace Linky.Api.Domain.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ConsumptionEntry> ConsumptionEntries => Set<ConsumptionEntry>();

    public DbSet<MarketPrice> MarketPrices => Set<MarketPrice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MarketPrice>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PricePerMWh).HasPrecision(18, 4);
            builder.HasIndex(x => x.Timestamp).IsUnique();
        });

        modelBuilder.Entity<ConsumptionEntry>(builder =>
        {
            builder.HasKey(x => x.Id);

            // Settings for PRM (Point de Référence Mesure)
            builder.Property(x => x.Prm)
                .IsRequired()
                .HasMaxLength(14);

            // Precision settings for money/power
            builder.Property(x => x.Watts)
                .HasPrecision(18, 2);

            // Unique index: cannot record data twice for the same meter at the same time
            builder.HasIndex(x => new { x.Prm, x.Timestamp }).IsUnique();
        });
    }
}