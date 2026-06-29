using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace DockiUp.Application.Queries
{
    /// <summary>Most-recent activity entries (audit log), newest first.</summary>
    public sealed class ListActivityQuery : IRequest<IReadOnlyList<ActivityEntryDto>>
    {
        public ListActivityQuery(int limit = 200)
        {
            Limit = limit;
        }

        public int Limit { get; }
    }

    public sealed class ListActivityQueryHandler : IRequestHandler<ListActivityQuery, IReadOnlyList<ActivityEntryDto>>
    {
        private readonly IDockiUpDbContext _dbContext;

        public ListActivityQueryHandler(IDockiUpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<IReadOnlyList<ActivityEntryDto>> Handle(ListActivityQuery request, CancellationToken cancellationToken)
        {
            var limit = Math.Clamp(request.Limit, 1, 1000);
            var rows = await _dbContext.ActivityEntries
                .OrderByDescending(e => e.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return rows.Select(e => new ActivityEntryDto
            {
                Id = e.Id,
                Action = e.Action,
                Target = e.Target,
                ProjectId = e.ProjectId,
                Details = e.Details,
                ActorName = e.ActorName,
                CreatedAt = e.CreatedAt,
            }).ToList();
        }
    }
}
