using DockiUp.Domain;
using Microsoft.EntityFrameworkCore;

namespace DockiUp.Application.Interfaces
{
    public interface IDockiUpDbContext
    {
        public DbSet<ProjectInfo> ProjectInfo { get; set; }
        public DbSet<Node> Nodes { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
