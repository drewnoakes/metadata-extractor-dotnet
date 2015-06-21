using System.IO;

namespace Sharpen
{
    public class FileOutputStream : OutputStream
    {
        public FileOutputStream (FilePath file): this (file.GetPath (), false)
        {
        }

        public FileOutputStream (string file, bool append = false)
        {
            try {
                if (append) {
                    Wrapped = File.Open (file, FileMode.Append, FileAccess.Write);
                } else {
                    Wrapped = File.Open (file, FileMode.Create, FileAccess.Write);
                }
            } catch (DirectoryNotFoundException) {
                throw new FileNotFoundException ("File not found: " + file);
            }
        }
    }
}
