using DockiUp.Domain;
using Microsoft.EntityFrameworkCore;

namespace DockiUp.Application.Interfaces
{
    public interface IDockiUpDbContext
    {
        public DbSet<ProjectInfo> ProjectInfo { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
