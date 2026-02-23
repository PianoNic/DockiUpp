using DockiUp.Application.Interfaces;
using DockiUp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DockiUp.Infrastructure
{
    public class DockiUpDbContext : DbContext, IDockiUpDbContext
    {
        public DbSet<ProjectInfo> ProjectInfo { get; set; }

        public DockiUpDbContext(DbContextOptions<DockiUpDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProjectInfo>(e =>
            {
                e.Property(p => p.LastPeriodicUpdateAt);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<EntityBase>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        public class DockiUpDbContextFactory : IDesignTimeDbContextFactory<DockiUpDbContext>
        {
            public DockiUpDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<DockiUpDbContext>();
                optionsBuilder.UseNpgsql();
                return new DockiUpDbContext(optionsBuilder.Options);
            }
        }
    }
}
