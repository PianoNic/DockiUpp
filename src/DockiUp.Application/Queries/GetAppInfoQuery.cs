using System.Reflection;
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
            // Report the real build version and runtime environment instead of hardcoded placeholders,
            // so the sidenav/footer reflect what's actually running. Whatever the build stamps flows
            // through the assembly's informational version; the +<sha> build suffix, if any, is trimmed.
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            var version = informational?.Split('+')[0]
                ?? assembly.GetName().Version?.ToString()
                ?? "0.0.0";
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            return new ValueTask<AppInfoDto>(new AppInfoDto { Version = $"v{version}", Environment = environment });
        }
    }
}
