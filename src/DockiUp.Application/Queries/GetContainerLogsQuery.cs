using DockiUp.Application.Interfaces;
using Mediator;

namespace DockiUp.Application.Queries
{
    public sealed class GetContainerLogsQuery : IRequest<string>
    {
        public string ContainerId { get; }
        public int? Tail { get; }
        public Guid? NodeId { get; }

        public GetContainerLogsQuery(string containerId, int? tail = null, Guid? nodeId = null)
        {
            ContainerId = containerId;
            Tail = tail;
            NodeId = nodeId;
        }
    }

    public sealed class GetContainerLogsQueryHandler : IRequestHandler<GetContainerLogsQuery, string>
    {
        private readonly IDockerServiceResolver _dockerResolver;

        public GetContainerLogsQueryHandler(IDockerServiceResolver dockerResolver)
        {
            _dockerResolver = dockerResolver;
        }

        public async ValueTask<string> Handle(GetContainerLogsQuery request, CancellationToken cancellationToken)
        {
            return await _dockerResolver.Resolve(request.NodeId).GetContainerLogsAsync(request.ContainerId, request.Tail, cancellationToken);
        }
    }
}
