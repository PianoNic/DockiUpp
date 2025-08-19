using DockiUp.Application.Dtos;
using MediatR;

namespace DockiUp.Application.Queries
{
    public class GetAppInfoQuery : IRequest<AppInfoDto>
    {
        public GetAppInfoQuery()
        {
        }
    }

    public class GetAppInfoQueryHandler : IRequestHandler<GetAppInfoQuery, AppInfoDto>
    {
        public GetAppInfoQueryHandler()
        {
        }

        public async Task<AppInfoDto> Handle(GetAppInfoQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new AppInfoDto() { Environment = "Dev", Version = "v1.0.0" });
        }
    }
}
