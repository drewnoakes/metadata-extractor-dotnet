namespace Sharpen
{
    public interface Future<T>
    {
        bool Cancel (bool mayInterruptIfRunning);
        T Get ();
    }
}
