using DockiUp.Application.Enums;

namespace DockiUp.Application.Mappers
{
    public static class ContainerStateMapper
    {
        public static UpdateMethodType ToEnum(this string rawDockerState)
        {
            string normalizedState = rawDockerState?.Trim().ToLowerInvariant() ?? string.Empty;

            switch (normalizedState)
            {
                case "created":
                    return UpdateMethodType.Created;

                case "running":
                case "restarting":
                case "removing":
                    return UpdateMethodType.Running;

                case "paused":
                    return UpdateMethodType.Stopped;

                case "exited":
                case "dead":
                    return UpdateMethodType.Crashed;

                default:
                    Console.WriteLine($"Warning: Unrecognized Docker state '{rawDockerState}'. Mapping to Unknown.");
                    return UpdateMethodType.Unknown;
            }
        }
    }
}
