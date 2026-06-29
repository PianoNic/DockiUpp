namespace DockiUp.Application.Interfaces
{
    /// <summary>Tracks interactive exec (shell) sessions into running containers.</summary>
    public interface IContainerExecRegistry
    {
        Task<IContainerExecSession> StartAsync(string containerId, uint cols, uint rows, CancellationToken cancellationToken = default);
        IContainerExecSession? Get(string sessionId);
        Task EndAsync(string sessionId);
    }

    /// <summary>A live shell session bridged to a container's TTY over a hijacked Docker stream.</summary>
    public interface IContainerExecSession : IAsyncDisposable
    {
        string Id { get; }
        event Func<ReadOnlyMemory<byte>, Task>? Output;
        event Func<long?, Task>? Exited;
        Task WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
        Task ResizeAsync(uint cols, uint rows, CancellationToken cancellationToken = default);
    }
}
