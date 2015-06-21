using System.IO;

namespace Sharpen
{
    internal static class FileHelper
    {
        public static bool CanRead(FilePath path)
        {
            try {
                using (File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    return true;
                }
            }
            catch (IOException) {
                return false;
            }
        }

        public static bool Delete (FilePath path)
        {
            if (Directory.Exists (path)) {
                if (Directory.GetFileSystemEntries (path).Length != 0)
                    return false;
                MakeDirWritable (path);
                Directory.Delete (path, true);
                return true;
            }
            if (File.Exists(path)) {
                MakeFileWritable (path);
                File.Delete (path);
                return true;
            }
            return false;
        }

        public static bool Exists (FilePath path)
        {
            return (File.Exists (path) || Directory.Exists (path));
        }

        public static bool IsDirectory (FilePath path)
        {
            return Directory.Exists (path);
        }

        public static bool IsFile (FilePath path)
        {
            return File.Exists (path);
        }

        public static long LastModified (FilePath path)
        {
            if (IsFile(path)) {
                var info2 = new FileInfo(path);
                return info2.Exists ? info2.LastWriteTimeUtc.ToMillisecondsSinceEpoch() : 0;
            }
            if (IsDirectory (path)) {
                var info = new DirectoryInfo(path);
                return info.Exists ? info.LastWriteTimeUtc.ToMillisecondsSinceEpoch() : 0;
            }
            return 0;
        }

        public static long Length (FilePath path)
        {
            // If you call .Length on a file that doesn't exist, an exception is thrown
            var info2 = new FileInfo (path);
            return info2.Exists ? info2.Length : 0;
        }

        public static void MakeDirWritable (FilePath path)
        {
            foreach (string file in Directory.GetFiles (path)) {
                MakeFileWritable (file);
            }
            foreach (string subdir in Directory.GetDirectories (path)) {
                MakeDirWritable (subdir);
            }
        }

        public static void MakeFileWritable (FilePath file)
        {
            FileAttributes fileAttributes = File.GetAttributes (file);
            if ((fileAttributes & FileAttributes.ReadOnly) != 0) {
                fileAttributes &= ~FileAttributes.ReadOnly;
                File.SetAttributes (file, fileAttributes);
            }
        }
    }
}

