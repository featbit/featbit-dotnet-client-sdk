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

            static async Task ForgetAwaited(Task task)
            {
                try
                {
                    // No need to resume on the original SynchronizationContext, so use ConfigureAwait(false)
                    await task.ConfigureAwait(false);
                }
                catch
                {
                    // Nothing to do here
                }
            }
        }
    }
}