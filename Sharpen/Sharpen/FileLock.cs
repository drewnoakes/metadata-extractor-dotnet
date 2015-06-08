using System.IO;

namespace Sharpen
{
    public class FileLock
    {
        private FileStream s;

        public FileLock (FileStream s)
        {
            this.s = s;
        }

        public void Release ()
        {
            this.s.Unlock (0, int.MaxValue);
        }
    }
}
