using DockiUp.Application.Dtos;
using Mediator;

namespace DockiUp.Application.Queries
{
    public sealed class GetAppInfoQuery : IRequest<AppInfoDto>
    {
        public GetAppInfoQuery()
        {
        }
    }

    public sealed class GetAppInfoQueryHandler : IRequestHandler<GetAppInfoQuery, AppInfoDto>
    {
        public GetAppInfoQueryHandler()
        {
        }

        public ValueTask<AppInfoDto> Handle(GetAppInfoQuery request, CancellationToken cancellationToken)
        {
            return new ValueTask<AppInfoDto>(new AppInfoDto() { Environment = "Dev", Version = "v1.0.0" });
        }
    }
}
