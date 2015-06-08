using System.IO;

namespace Sharpen
{
    public class FileOutputStream : OutputStream
    {
        public FileOutputStream (FilePath file): this (file.GetPath (), false)
        {
        }

        public FileOutputStream (string file): this (file, false)
        {
        }

        public FileOutputStream (FilePath file, bool append) : this(file.GetPath (), append)
        {
        }

        public FileOutputStream (string file, bool append)
        {
            try {
                if (append) {
                    base.Wrapped = File.Open (file, FileMode.Append, FileAccess.Write);
                } else {
                    base.Wrapped = File.Open (file, FileMode.Create, FileAccess.Write);
                }
            } catch (DirectoryNotFoundException) {
                throw new FileNotFoundException ("File not found: " + file);
            }
        }

        public FileChannel GetChannel ()
        {
            return new FileChannel ((FileStream)base.Wrapped);
        }
    }
}
