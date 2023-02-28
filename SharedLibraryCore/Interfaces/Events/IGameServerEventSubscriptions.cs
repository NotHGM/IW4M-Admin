using System;
using System.Threading;
using System.Threading.Tasks;
using SharedLibraryCore.Events;
using SharedLibraryCore.Events.Server;

namespace SharedLibraryCore.Interfaces.Events;

public interface IGameServerEventSubscriptions
{
    /// <summary>
    /// Raised when IW4MAdmin starts monitoring a game server
    /// <value><see cref="MonitorStartEvent"/></value>
    /// </summary>
    static event Func<MonitorStartEvent, CancellationToken, Task> MonitoringStarted;
    
    /// <summary>
    /// Raised when IW4MAdmin stops monitoring a game server
    /// <value><see cref="MonitorStopEvent"/></value>
    /// </summary>
    static event Func<MonitorStopEvent, CancellationToken, Task> MonitoringStopped;
    
    /// <summary>
    /// Raised when communication was interrupted with a game server
    /// <value><see cref="ConnectionInterruptEvent"/></value>
    /// </summary>
    static event Func<ConnectionInterruptEvent, CancellationToken, Task> ConnectionInterrupted;
    
    /// <summary>
    /// Raised when communication was resumed with a game server
    /// <value><see cref="ConnectionRestoreEvent"/></value>
    /// </summary>
    static event Func<ConnectionRestoreEvent, CancellationToken, Task> ConnectionRestored;
    
    /// <summary>
    /// Raised when updated client data was received from a game server
    /// <value><see cref="ClientDataUpdateEvent"/></value>
    /// </summary>
    static event Func<ClientDataUpdateEvent, CancellationToken, Task> ClientDataUpdated;
    
    /// <summary>
    /// Raised when a command was executed on a game server
    /// <value><see cref="ServerCommandExecuteEvent"/></value>
    /// </summary>
    static event Func<ServerCommandExecuteEvent, CancellationToken, Task> ServerCommandExecuted;
    
    /// <summary>
    /// Raised when a server value was received from a game server
    /// <value><see cref="ServerValueReceiveEvent"/></value>
    /// </summary>
    static event Func<ServerValueReceiveEvent, CancellationToken, Task> ServerValueReceived;

    static event Func<ServerValueRequestEvent, CancellationToken, Task> ServerValueRequested;

    static event Func<ServerValueSetRequestEvent, CancellationToken, Task> ServerValueSetRequested;

    static event Func<ServerValueSetCompleteEvent, CancellationToken, Task> ServerValueSetCompleted;
    
    static Task InvokeEventAsync(CoreEvent coreEvent, CancellationToken token)
    {
        return coreEvent switch
        {
            MonitorStartEvent monitoringStartEvent => MonitoringStarted?.InvokeAsync(monitoringStartEvent, token),
            ConnectionInterruptEvent connectionInterruptEvent => ConnectionInterrupted?.InvokeAsync(connectionInterruptEvent, token),
            ConnectionRestoreEvent connectionRestoreEvent => ConnectionRestored?.InvokeAsync(connectionRestoreEvent, token),
            ClientDataUpdateEvent clientDataUpdateEvent => ClientDataUpdated?.InvokeAsync(clientDataUpdateEvent, token),
            ServerCommandExecuteEvent dataReceiveEvent => ServerCommandExecuted?.InvokeAsync(dataReceiveEvent, token),
            ServerValueRequestEvent serverValueRequestEvent => ServerValueRequested?.InvokeAsync(serverValueRequestEvent, token),
            ServerValueReceiveEvent serverValueReceiveEvent => ServerValueReceived?.InvokeAsync(serverValueReceiveEvent, token),
            ServerValueSetRequestEvent serverValueSetRequestEvent => ServerValueSetRequested?.InvokeAsync(serverValueSetRequestEvent, token),
            ServerValueSetCompleteEvent serverValueSetCompleteEvent => ServerValueSetCompleted?.InvokeAsync(serverValueSetCompleteEvent, token),
            _ => Task.CompletedTask
        };
    }

    static Task InvokeMonitoringStoppedEvent(MonitorStopEvent server, CancellationToken token)
    {
        return MonitoringStopped?.InvokeAsync(server, token);
    }

    static void ClearEventInvocations()
    {
        MonitoringStarted = null;
        MonitoringStopped = null;
        ConnectionInterrupted = null;
        ConnectionRestored = null;
        ClientDataUpdated = null;
        ServerCommandExecuted = null;
        ServerValueReceived = null;
        ServerValueRequested = null;
        ServerValueSetRequested = null;
        ServerValueSetCompleted = null;
    }
}
