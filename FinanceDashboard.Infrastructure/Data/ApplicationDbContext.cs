using Microsoft.EntityFrameworkCore;
using FinanceDashboard.Core.Entities;
using FinanceDashboard.Core.Common;
using FinanceDashboard.Core.Enums;

namespace FinanceDashboard.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Status)
            .HasConversion<string>();
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType.IsEnum || 
                           (Nullable.GetUnderlyingType(p.PropertyType)?.IsEnum ?? false));

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion<string>();
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseAuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.LastModifiedAt = DateTime.UtcNow;
            
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}