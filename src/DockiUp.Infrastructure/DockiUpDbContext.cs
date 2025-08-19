using DockiUp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DockiUp.Infrastructure
{
    public class DockiUpDbContext : DbContext
    {
        public DbSet<ProjectInfo> ProjectInfo { get; set; }

        public DockiUpDbContext(DbContextOptions<DockiUpDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<ProjectInfo>(entity =>
            //{

            //});
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
