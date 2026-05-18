using Coffee4You.Server.Domain.Common;
using Coffee4You.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Coffee4You.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Bean> Beans => Set<Bean>();
    public DbSet<Colour> Colours => Set<Colour>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<BeanOfTheDay> BeansOfTheDay => Set<BeanOfTheDay>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bean>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).IsRequired();
            b.Property(x => x.ImageUrl).HasMaxLength(2048).IsRequired();
            b.Property(x => x.Cost).HasColumnType("decimal(10,2)");
            b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();

            b.HasOne(x => x.Colour)
                .WithMany(c => c.Beans)
                .HasForeignKey(x => x.ColourId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Country)
                .WithMany(c => c.Beans)
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.CountryId);
            b.HasIndex(x => x.ColourId);
        });

        modelBuilder.Entity<Colour>(c =>
        {
            c.Property(x => x.Name).HasMaxLength(50).IsRequired();
            c.HasIndex(x => x.Name);
        });

        modelBuilder.Entity<Country>(c =>
        {
            c.Property(x => x.Name).HasMaxLength(100).IsRequired();
            c.Property(x => x.IsoCode).HasMaxLength(2).IsRequired();
            c.HasIndex(x => x.IsoCode);
        });

        modelBuilder.Entity<BeanOfTheDay>(b =>
        {
            b.HasOne(x => x.Bean)
                .WithMany()
                .HasForeignKey(x => x.BeanId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.SelectedFor).IsUnique();
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var notDeleted = Expression.Not(property);
            var lambda = Expression.Lambda(notDeleted, parameter);
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }

    public override int SaveChanges()
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditAndSoftDelete()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}
