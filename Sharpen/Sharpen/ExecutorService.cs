namespace Sharpen
{
    public interface ExecutorService : Executor
    {
        bool AwaitTermination (long n, TimeUnit unit);
        void Shutdown ();
        void ShutdownNow ();
        Future<T> Submit<T> (Callable<T> ob);
    }
}
