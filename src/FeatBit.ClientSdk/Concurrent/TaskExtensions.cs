using System;
using System.Threading;
using System.Threading.Tasks;

namespace FeatBit.Sdk.Client.Concurrent
{
    public static class TaskExtensions
    {
        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource();
            var delayTask = Task.Delay(timeout, cts.Token);

            var resultTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
            if (resultTask == delayTask)
            {
                // Operation cancelled
                throw new OperationCanceledException();
            }

            // Cancel the timer task so that it does not fire
            cts.Cancel();

            return await task.ConfigureAwait(false);
        }
    }
}