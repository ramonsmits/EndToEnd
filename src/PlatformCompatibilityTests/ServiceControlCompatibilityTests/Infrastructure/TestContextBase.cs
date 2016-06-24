namespace ServiceControlCompatibilityTests
{
    using System.Threading.Tasks;

    abstract class TestContextBase
    {
        TaskCompletionSource<bool> doneSource = new TaskCompletionSource<bool>();

        public void Done(bool success)
        {
            doneSource.SetResult(success);
        }

        public Task<bool> WaitForDone()
        {
            return doneSource.Task;
        }
    }
}