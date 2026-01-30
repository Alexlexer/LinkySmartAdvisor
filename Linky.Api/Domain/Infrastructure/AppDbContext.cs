using Microsoft.EntityFrameworkCore;

namespace Linky.Api.Domain.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ConsumptionEntry> ConsumptionEntries => Set<ConsumptionEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ConsumptionEntry>(builder =>
        {
            builder.HasKey(x => x.Id);

            // Настройки для PRM (Point de Référence Mesure)
            builder.Property(x => x.Prm)
                .IsRequired()
                .HasMaxLength(14);

            // Настройки точности для денег/мощности
            builder.Property(x => x.Watts)
                .HasPrecision(18, 2);

            // Уникальный индекс: нельзя дважды записать данные для одного счетчика на одно и то же время
            builder.HasIndex(x => new { x.Prm, x.Timestamp }).IsUnique();
        });
    }
}