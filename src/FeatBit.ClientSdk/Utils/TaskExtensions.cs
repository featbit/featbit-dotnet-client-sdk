using System.Threading.Tasks;

namespace FeatBit.Sdk.Client.Utils
{
    /// <summary>
    /// Adds <see cref="Forget(Task)"/> to safely ignore the result of a task execution.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Observes the task to avoid the UnobservedTaskException event to be raised.
        /// </summary>
        public static void Forget(this Task task)
        {
            if (!task.IsCompleted || task.IsFaulted)
            {
                _ = ForgetAwaited(task);
            }

            return;

            // Allocate the async/await state machine only when needed for performance reasons.
            // More info about the state machine: https://blogs.msdn.microsoft.com/seteplia/2017/11/30/dissecting-the-async-methods-in-c/?WT.mc_id=DT-MVP-5003978
            static async Task ForgetAwaited(Task task)
            {
#if NET8_0_OR_GREATER
                await task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
#else
                try
                {
                    // No need to resume on the original SynchronizationContext, so use ConfigureAwait(false)
                    await task.ConfigureAwait(false);
                }
                catch
                {
                    // Nothing to do here
                }
#endif
            }
        }
    }
}