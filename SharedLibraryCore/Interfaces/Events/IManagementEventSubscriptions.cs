using System;
using System.Threading;
using System.Threading.Tasks;
using SharedLibraryCore.Events;
using SharedLibraryCore.Events.Management;

namespace SharedLibraryCore.Interfaces.Events;

public interface IManagementEventSubscriptions
{
    static event Func<IManager, CancellationToken, Task> Load;
    static event Func<IManager, CancellationToken, Task> Unload;

    /// <summary>
    /// Raised when client state becomes tracked
    /// <remarks>
    /// At this point, the client is not guaranteed to be allowed to play on the server.
    /// See <see cref="ClientStateAuthorized"/> for final state.
    /// </remarks>
    /// <value><see cref="ClientStateInitializeEvent"/></value>
    /// </summary>
    static event Func<ClientStateInitializeEvent, CancellationToken, Task> ClientStateInitialized;

    /// <summary>
    /// Raised when client state marked as authorized (valid data and no bans)
    /// <value><see cref="ClientStateAuthorizeEvent"/></value>
    /// </summary>
    static event Func<ClientStateAuthorizeEvent, CancellationToken, Task> ClientStateAuthorized;

    static event Func<ClientStateEvent, CancellationToken, Task> ClientStateDisposed;

    static event Func<ClientPenaltyEvent, CancellationToken, Task> ClientPenaltyAdministered;
    static event Func<ClientPenaltyRevokeEvent, CancellationToken, Task> ClientPenaltyRevoked;

    static event Func<ClientExecuteCommandEvent, CancellationToken, Task> ClientCommandExecuted;

    static event Func<ClientPermissionChangeEvent, CancellationToken, Task> ClientPermissionChanged;

    static event Func<LoginEvent, CancellationToken, Task> ClientLoggedIn;

    static event Func<LogoutEvent, CancellationToken, Task> ClientLoggedOut;

    static event Func<NotifyAfterDelayRequestEvent, CancellationToken, Task> NotifyAfterDelayRequested;

    static event Func<NotifyAfterDelayCompleteEvent, CancellationToken, Task> NotifyAfterDelayCompleted;

    static event Func<ClientPersistentIdReceiveEvent, CancellationToken, Task> ClientPersistentIdReceived;

    static Task InvokeEventAsync(CoreEvent coreEvent, CancellationToken token)
    {
        return coreEvent switch
        {
            ClientStateInitializeEvent clientStateInitializeEvent => ClientStateInitialized?.InvokeAsync(
                clientStateInitializeEvent, token),
            ClientStateDisposeEvent clientStateDisposedEvent => ClientStateDisposed?.InvokeAsync(
                clientStateDisposedEvent, token),
            ClientStateAuthorizeEvent clientStateAuthorizeEvent => ClientStateAuthorized?.InvokeAsync(
                clientStateAuthorizeEvent, token),
            ClientPenaltyRevokeEvent clientPenaltyRevokeEvent => ClientPenaltyRevoked?.InvokeAsync(
                clientPenaltyRevokeEvent, token),
            ClientPenaltyEvent clientPenaltyEvent => ClientPenaltyAdministered?.InvokeAsync(clientPenaltyEvent, token),
            ClientPermissionChangeEvent clientPermissionChangeEvent => ClientPermissionChanged?.InvokeAsync(
                clientPermissionChangeEvent, token),
            ClientExecuteCommandEvent clientExecuteCommandEvent => ClientCommandExecuted?.InvokeAsync(
                clientExecuteCommandEvent, token),
            LogoutEvent logoutEvent => ClientLoggedOut?.InvokeAsync(logoutEvent, token),
            LoginEvent loginEvent => ClientLoggedIn?.InvokeAsync(loginEvent, token),
            NotifyAfterDelayRequestEvent notifyAfterDelayRequestEvent => NotifyAfterDelayRequested?.InvokeAsync(
                notifyAfterDelayRequestEvent, CancellationToken.None),
            NotifyAfterDelayCompleteEvent notifyAfterDelayCompleteEvent => NotifyAfterDelayCompleted?.InvokeAsync(
                notifyAfterDelayCompleteEvent, token),
            ClientPersistentIdReceiveEvent clientPersistentIdReceiveEvent => ClientPersistentIdReceived?.InvokeAsync(
                clientPersistentIdReceiveEvent, token),
            _ => Task.CompletedTask
        };
    }

    static Task InvokeLoadAsync(IManager manager, CancellationToken token) => Load?.InvokeAsync(manager, token);
    static Task InvokeUnloadAsync(IManager manager, CancellationToken token) => Unload?.InvokeAsync(manager, token);

    static void ClearEventInvocations()
    {
        Load = null;
        Unload = null;
        ClientStateInitialized = null;
        ClientStateAuthorized = null;
        ClientStateDisposed = null;
        ClientPenaltyAdministered = null;
        ClientPenaltyRevoked = null;
        ClientCommandExecuted = null;
        ClientPermissionChanged = null;
        ClientLoggedIn = null;
        ClientLoggedOut = null;
        NotifyAfterDelayRequested = null;
        NotifyAfterDelayCompleted = null;
        ClientPersistentIdReceived = null;
    }
}
