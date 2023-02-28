using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharedLibraryCore.Events;

public static class EventExtensions
{
    public static Task InvokeAsync<TEventType>(this Func<TEventType, CancellationToken, Task> function,
        TEventType eventArgType, CancellationToken token)
    {
        if (function is null)
        {
            return Task.CompletedTask;
        }

        return Parallel.ForEachAsync(function.GetInvocationList().Cast<Func<TEventType, CancellationToken, Task>>(),
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 5,
                CancellationToken = token
            }, async (handler, innerToken) =>
            {

                if (token == CancellationToken.None)
                {
                    // special case to allow tasks like request after delay to run longer
                    await handler(eventArgType, innerToken);
                    return;
                }

                using var timeoutToken = new CancellationTokenSource(Utilities.DefaultCommandTimeout);
                using var tokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(innerToken, timeoutToken.Token);

                await handler(eventArgType, tokenSource.Token).WithWaitCancellation(tokenSource.Token);

            });
    }
}
