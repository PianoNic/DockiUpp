using System.Runtime.InteropServices;
using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using DockiUp.Application.Models;
using DockiUp.Domain.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace DockiUp.API.Nodes
{
    /// <summary>Runs only when this process boots in the <c>node</c> role. It dials OUT to the control
    /// plane's <c>/hubs/node</c> over SignalR (NAT-friendly), registers itself, and answers control-plane
    /// invocations. Phase 1 only proves the channel (<c>Ping</c>); routing real Docker/compose work over
    /// it comes later. Booting never blocks on the control plane being up - the connection retries.</summary>
    public class NodeAgentHostedService(
        IConfiguration configuration,
        IServiceProvider services,
        ILogger<NodeAgentHostedService> logger) : IHostedService, IAsyncDisposable
    {
        private HubConnection? _connection;
        private readonly CancellationTokenSource _shutdown = new();
        private string _nodeId = "";

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var controlPlaneUrl = configuration["Node:ControlPlaneUrl"];
            var token = configuration["Node:Token"];

            if (string.IsNullOrWhiteSpace(controlPlaneUrl) || string.IsNullOrWhiteSpace(token))
            {
                logger.LogError("Node role is active but Node:ControlPlaneUrl and/or Node:Token are not set. The agent will not connect.");
                return Task.CompletedTask;
            }

            // A stable id identifies this node across reconnects so deployed projects keep pointing at
            // it. Pin it via Node:Id; if unset we generate one but it won't survive a restart.
            if (!Guid.TryParse(configuration["Node:Id"], out var nodeId))
            {
                nodeId = Guid.NewGuid();
                logger.LogWarning("Node:Id is not set; generated {NodeId} for this run. Set Node:Id to keep a stable identity across restarts.", nodeId);
            }
            _nodeId = nodeId.ToString();

            var name = configuration["Node:Name"];
            if (string.IsNullOrWhiteSpace(name)) name = Environment.MachineName;

            var hubUrl = $"{controlPlaneUrl.TrimEnd('/')}/hubs/node?access_token={Uri.EscapeDataString(token)}";

            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            // Server -> node calls. Ping is the phase-1 channel proof.
            _connection.On("Ping", () => "pong");
            RegisterDockerHandlers(_connection);
            RegisterProjectHandlers(_connection);

            // Re-register after every (re)connect, since the registry is keyed by connection id and a
            // reconnect yields a fresh one.
            _connection.Reconnected += async _ => await RegisterAsync(name, CancellationToken.None);

            // Kick off the connect loop in the background so app startup isn't blocked on the control plane.
            _ = ConnectLoopAsync(name, hubUrl);
            _ = HeartbeatLoopAsync(_shutdown.Token);
            return Task.CompletedTask;
        }

        // Liveness: every 5s the node pings the control plane (which refreshes its "last seen").
        private async Task HeartbeatLoopAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    if (_connection is { State: HubConnectionState.Connected } connection)
                    {
                        try { await connection.SendAsync("Heartbeat", cancellationToken); }
                        catch (Exception ex) { logger.LogDebug("Node heartbeat failed: {Message}", ex.Message); }
                    }
                }
            }
            catch (OperationCanceledException) { /* shutting down */ }
        }

        private async Task ConnectLoopAsync(string name, string hubUrl)
        {
            var delay = TimeSpan.FromSeconds(2);
            while (_connection is not null)
            {
                try
                {
                    await _connection.StartAsync();
                    logger.LogInformation("Node agent connected to control plane at {Url}.", hubUrl);
                    await RegisterAsync(name, CancellationToken.None);
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Node agent could not reach the control plane ({Message}); retrying in {Delay}s.", ex.Message, delay.TotalSeconds);
                    await Task.Delay(delay);
                    delay = TimeSpan.FromSeconds(Math.Min(30, delay.TotalSeconds * 2));
                }
            }
        }

        private async Task RegisterAsync(string name, CancellationToken cancellationToken)
        {
            if (_connection is null) return;

            var dockerVersion = "unknown";
            try
            {
                using var scope = services.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<IDockiUpDockerClient>();
                var version = await client.DockerClient.System.GetVersionAsync(cancellationToken);
                dockerVersion = version.Version;
            }
            catch (Exception ex)
            {
                logger.LogWarning("Node agent could not read the local Docker version: {Message}", ex.Message);
            }

            var registration = new NodeRegistrationDto(
                Id: _nodeId,
                Name: name,
                MachineName: Environment.MachineName,
                Os: RuntimeInformation.OSDescription,
                DockerVersion: dockerVersion);

            try
            {
                await _connection.InvokeAsync("Register", registration, cancellationToken);
                logger.LogInformation("Node agent registered as {Name}.", name);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Node agent failed to register: {Message}", ex.Message);
            }
        }

        // The control plane invokes these on the node's connection (RemoteDockerService); each runs
        // against the node's local Docker daemon and returns the result. Void Docker ops return a bool
        // because SignalR client-result invocations must return a value.
        private void RegisterDockerHandlers(HubConnection c)
        {
            c.On("GetProjects", () => WithDocker(d => d.GetProjectsAsync()));
            c.On<string, ProjectDto?>("GetProjectByDockerName", name => WithDocker(d => d.GetProjectByDockerNameAsync(name)));
            c.On<string, ContainerDto?>("InspectContainer", id => WithDocker(d => d.InspectContainerAsync(id)));
            c.On<string, bool>("StartProject", path => WithDocker(async d => { await d.StartProjectAsync(path); return true; }));
            c.On<string, bool>("StopProject", path => WithDocker(async d => { await d.StopProjectAsync(path); return true; }));
            c.On<string, bool>("RestartProject", path => WithDocker(async d => { await d.RestartProjectAsync(path); return true; }));
            c.On<string, bool>("StartContainer", id => WithDocker(async d => { await d.StartContainerAsync(id); return true; }));
            c.On<string, bool>("StopContainer", id => WithDocker(async d => { await d.StopContainerAsync(id); return true; }));
            c.On<string, bool>("RestartContainer", id => WithDocker(async d => { await d.RestartContainerAsync(id); return true; }));
            c.On<string, int?, string>("GetContainerLogs", (id, tail) => WithDocker(d => d.GetContainerLogsAsync(id, tail)));
        }

        // Deploy + git-pull run against the node's own filesystem (no app database here), so the node
        // clones/writes/composes locally and reports the paths it used back to the control plane.
        private void RegisterProjectHandlers(HubConnection c)
        {
            c.On<SetupProjectDto, NodeDeployResultDto>("DeployProject", DeployLocallyAsync);
            c.On<string, bool>("PullRepository", path => WithConfig(async cfg => { await cfg.UpdateRepositoryAsync(path); return true; }));
        }

        private async Task<NodeDeployResultDto> DeployLocallyAsync(SetupProjectDto dto)
        {
            using var scope = services.CreateScope();
            var paths = scope.ServiceProvider.GetRequiredService<IOptions<SystemPaths>>().Value;
            var config = scope.ServiceProvider.GetRequiredService<IDockiUpProjectConfigurationService>();
            var docker = scope.ServiceProvider.GetRequiredService<IDockerService>();

            var projectPath = Path.Combine(paths.ProjectsPath, dto.ProjectName);
            Directory.CreateDirectory(projectPath);

            if (dto.ProjectOrigin == ProjectOriginType.Git)
                await config.CloneRepositoryAsync(projectPath, dto.GitUrl!);
            var composePath = await config.WriteComposeFileAsync(projectPath, dto.Compose!);
            await docker.StartProjectAsync(projectPath);

            return new NodeDeployResultDto(projectPath, composePath);
        }

        private async Task<T> WithDocker<T>(Func<IDockerService, Task<T>> work)
        {
            using var scope = services.CreateScope();
            return await work(scope.ServiceProvider.GetRequiredService<IDockerService>());
        }

        private async Task<T> WithConfig<T>(Func<IDockiUpProjectConfigurationService, Task<T>> work)
        {
            using var scope = services.CreateScope();
            return await work(scope.ServiceProvider.GetRequiredService<IDockiUpProjectConfigurationService>());
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _shutdown.CancelAsync();
            if (_connection is not null)
                await _connection.StopAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            _shutdown.Cancel();
            _shutdown.Dispose();
            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
