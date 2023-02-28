using System;
using System.Collections.Concurrent;
using SharedLibraryCore;
using SharedLibraryCore.Events;
using SharedLibraryCore.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharedLibraryCore.Events.Management;
using SharedLibraryCore.Events.Server;
using SharedLibraryCore.Interfaces.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace IW4MAdmin.Application
{
    public class CoreEventHandler : ICoreEventHandler
    {
        private const int MaxCurrentEvents = 10;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _onProcessingEvents = new(MaxCurrentEvents, MaxCurrentEvents);
        private readonly ConcurrentQueue<(IManager, CoreEvent)> _runningEventTasks = new();
        private CancellationToken _cancellationToken;

        private static readonly GameEvent.EventType[] OverrideEvents =
        {
            GameEvent.EventType.Connect,
            GameEvent.EventType.Disconnect,
            GameEvent.EventType.Quit,
            GameEvent.EventType.Stop
        };

        public CoreEventHandler(ILogger<CoreEventHandler> logger)
        {
            _logger = logger;
        }

        public void QueueEvent(IManager manager, CoreEvent coreEvent)
        {
            _runningEventTasks.Enqueue((manager, coreEvent));
        }

        public async Task StartProcessing(CancellationToken token)
        {
            _cancellationToken = token;

            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _onProcessingEvents.WaitAsync(_cancellationToken);

                    if (!_runningEventTasks.TryDequeue(out var coreEvent))
                    {
                        await Task.Delay(50, _cancellationToken);
                        continue;
                    }

                    _ = Task.Run(async () =>
                        {
                            try
                            {
                                await GetEventTask(coreEvent.Item1, coreEvent.Item2);
                            }
                            catch (OperationCanceledException)
                            {
                                _logger.LogWarning("Event timed out {Type}", coreEvent.Item2.GetType().Name);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Could not complete invoke for {EventType}",
                                    coreEvent.Item2.GetType().Name);
                            }
                        },
                        _cancellationToken);
                }
                finally
                {
                    if (_onProcessingEvents.CurrentCount < MaxCurrentEvents)
                    {
                        _onProcessingEvents.Release(1);
                    }
                }
            }
        }

        private Task GetEventTask(IManager manager, CoreEvent coreEvent)
        {
            return coreEvent switch
            {
                GameEvent gameEvent => BuildLegacyEventTask(manager, coreEvent, gameEvent),
                GameServerEvent gameServerEvent => IGameServerEventSubscriptions.InvokeEventAsync(gameServerEvent,
                    manager.CancellationToken),
                ManagementEvent managementEvent => IManagementEventSubscriptions.InvokeEventAsync(managementEvent,
                    manager.CancellationToken),
                _ => Task.CompletedTask
            };
        }

        private Task BuildLegacyEventTask(IManager manager, CoreEvent coreEvent, GameEvent gameEvent)
        {
            if (manager.IsRunning || OverrideEvents.Contains(gameEvent.Type))
            {
                return manager.ExecuteEvent(gameEvent).ContinueWith(_ =>
                        IGameEventSubscriptions.InvokeEventAsync(coreEvent, manager.CancellationToken),
                    manager.CancellationToken);
            }
            
            _logger.LogDebug("Skipping event as we're shutting down {EventId}", gameEvent.IncrementalId);
            return Task.CompletedTask;
        }
    }
}
