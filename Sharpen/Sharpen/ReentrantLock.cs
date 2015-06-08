using System.Threading;

namespace Sharpen
{
    public class ReentrantLock
    {
        public void Lock ()
        {
            Monitor.Enter (this);
        }

        public bool TryLock ()
        {
            return Monitor.TryEnter (this);
        }

        public void Unlock ()
        {
            Monitor.Exit (this);
        }
    }
}
