using DockiUp.Application.Interfaces;
using DockiUp.Domain;

namespace DockiUp.Infrastructure.Services
{
    public class ActivityLogger : IActivityLogger
    {
        private readonly IDockiUpDbContext _dbContext;

        public ActivityLogger(IDockiUpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogAsync(string action, string target, Guid? projectId = null, string? details = null, CancellationToken cancellationToken = default)
        {
            _dbContext.ActivityEntries.Add(new ActivityEntry
            {
                Action = action,
                Target = target,
                ProjectId = projectId,
                Details = details,
                // DockiUp has no user context yet; background and request-triggered actions both log as system.
                ActorName = null,
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
