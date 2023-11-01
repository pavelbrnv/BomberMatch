namespace BomberMatch.Utils
{
    public static class Runner
    {
        public static T RunWithTimeout<T>(Func<T> getResult, TimeSpan timeout)
        {
            var task = Task.Run(getResult);
            if (task.Wait(timeout))
            {
                return task.Result;
            }
            throw new Exception("Method execution canceled by timeout");
        }
    }
}
