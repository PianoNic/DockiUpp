using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace DockiUp.Application.Queries
{
    public sealed class GetProjectsQuery : IRequest<ProjectDto[]>
    {
        public GetProjectsQuery()
        {
        }
    }

    public sealed class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, ProjectDto[]>
    {
        private readonly IDockerService _localDocker;
        private readonly IDockerServiceResolver _dockerResolver;
        private readonly INodeDirectory _nodeDirectory;
        private readonly IDockiUpDbContext _dbContext;

        public GetProjectsQueryHandler(
            IDockerService localDocker,
            IDockerServiceResolver dockerResolver,
            INodeDirectory nodeDirectory,
            IDockiUpDbContext dbContext)
        {
            _localDocker = localDocker;
            _dockerResolver = dockerResolver;
            _nodeDirectory = nodeDirectory;
            _dbContext = dbContext;
        }

        public async ValueTask<ProjectDto[]> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            // Docker project names the DB assigns to a node — these belong to that node's listing, not
            // the control-plane host's (and de-duplicates when a node happens to share the local daemon).
            var nodeOwned = (await _dbContext.ProjectInfo
                    .Where(p => p.NodeId != null)
                    .Select(p => p.DockerProjectName)
                    .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Local control-plane projects (already reconciled against the DB; NodeId stays null).
            var result = (await _localDocker.GetProjectsAsync())
                .Where(p => !nodeOwned.Contains(p.DockerProjectName))
                .ToList();

            // Fan out to each online node and merge its projects, reconciled against our records.
            foreach (var nodeId in _nodeDirectory.GetOnlineNodeIds())
            {
                ProjectDto[] nodeProjects;
                try
                {
                    nodeProjects = await _dockerResolver.Resolve(nodeId).GetRawProjectsAsync();
                }
                catch
                {
                    // A node that drops out mid-listing shouldn't fail the whole call.
                    continue;
                }

                var dbProjects = await _dbContext.ProjectInfo
                    .Where(p => p.NodeId == nodeId)
                    .ToDictionaryAsync(p => p.DockerProjectName, cancellationToken);

                foreach (var project in nodeProjects)
                {
                    project.NodeId = nodeId;
                    if (dbProjects.TryGetValue(project.DockerProjectName, out var db))
                    {
                        project.Id = db.Id;
                        project.ProjectName = db.ProjectName;
                        project.ProjectDescription = db.Description ?? "Not Managed By DockiUp";
                        project.ManagedByDockiUp = true;
                        project.ProjectPath = db.ProjectPath;
                        project.UpdateMethod = db.ProjectUpdateMethod.ToString();
                    }
                    result.Add(project);
                }
            }

            return result.ToArray();
        }
    }
}
