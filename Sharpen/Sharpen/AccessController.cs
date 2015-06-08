namespace Sharpen
{
    public class AccessController
    {
        public static T DoPrivileged<T> (PrivilegedAction<T> action)
        {
            return action.Run ();
        }
    }
}
