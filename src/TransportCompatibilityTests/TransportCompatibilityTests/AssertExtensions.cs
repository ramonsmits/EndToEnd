namespace TransportCompatibilityTests
{
    using System;
    using NUnit.Framework;
    using System.Threading.Tasks;

    public static class AssertEx
    {
        /// <summary>
        /// Executes a task returns when done.
        /// <exception cref="AssertionException">Throws when task wasn't done within 20 seconds.</exception>
        /// </summary>
        /// <param name="predicate">Task to execute. A Func for backwards compatibility reasons.</param>
        /// <param name="timeout">Override default timeout of 20 seconds.</param>
        public static void WaitUntilIsTrue(Func<bool> predicate, TimeSpan? timeout = null)
        {
            if (timeout.HasValue == false)
            {
                timeout = TimeSpan.FromSeconds(90);
            }

            var timeoutTask = Task.Delay(timeout.Value);
            var finishedResult = Task.WhenAny(Task.FromResult(predicate), timeoutTask).GetAwaiter().GetResult();

            if (finishedResult.Equals(timeoutTask))
            {
                throw new AssertionException($"Condition has not been met for {timeout.Value.Seconds} seconds.");
            }
        }
    }
}