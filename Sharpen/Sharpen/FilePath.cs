using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sharpen
{
    public class FilePath
    {
        private readonly string _path;
        private static long _tempCounter;

        public FilePath (string path)
            : this ((string) null, path)
        {
        }

        public FilePath (FilePath other, string child)
            : this ((string) other, child)
        {
        }

        private FilePath (string other, string child)
        {
            if (other == null) {
                _path = child;
            } else {
                while (!string.IsNullOrEmpty(child) && (child[0] == Path.DirectorySeparatorChar || child[0] == Path.AltDirectorySeparatorChar))
                    child = child.Substring (1);

                if (!string.IsNullOrEmpty(other) && other[other.Length - 1] == Path.VolumeSeparatorChar)
                    other += Path.DirectorySeparatorChar;

                _path = Path.Combine (other, child);
            }
        }

        public static implicit operator FilePath (string name)
        {
            return new FilePath (name);
        }

        public static implicit operator string (FilePath filePath)
        {
            return filePath == null ? null : filePath._path;
        }

        public override bool Equals (object obj)
        {
            FilePath other = obj as FilePath;
            if (other == null)
                return false;
            return GetCanonicalPath () == other.GetCanonicalPath ();
        }

        public override int GetHashCode ()
        {
            return _path.GetHashCode ();
        }

        public bool CanRead()
        {
            return FileHelper.Instance.CanRead(this);
        }

        public static FilePath CreateTempFile (string prefix, string suffix, FilePath directory = null)
        {
            string file;
            if (prefix == null) {
                throw new ArgumentNullException ("prefix");
            }
            if (prefix.Length < 3) {
                throw new ArgumentException ("prefix must have at least 3 characters");
            }
            string str = (directory == null) ? Path.GetTempPath () : directory.GetPath ();
            do {
                file = Path.Combine (str, prefix + Interlocked.Increment (ref _tempCounter) + suffix);
            } while (File.Exists (file));

            new FileOutputStream (file).Close ();
            return new FilePath (file);
        }

        public bool Delete ()
        {
            try {
                return FileHelper.Instance.Delete (this);
            } catch (Exception exception) {
                Console.WriteLine (exception);
                return false;
            }
        }

        public bool Exists ()
        {
            return FileHelper.Instance.Exists (this);
        }

        public string GetAbsolutePath ()
        {
            return Path.GetFullPath (_path);
        }

        public string GetCanonicalPath ()
        {
            string p = Path.GetFullPath (_path);
            p.TrimEnd (Path.DirectorySeparatorChar);
            return p;
        }

        public string GetName ()
        {
            return Path.GetFileName (_path);
        }

        public string GetPath ()
        {
            return _path;
        }

        public bool IsDirectory ()
        {
            return FileHelper.Instance.IsDirectory (this);
        }

        public bool IsFile ()
        {
            return FileHelper.Instance.IsFile (this);
        }

        public long LastModified ()
        {
            return FileHelper.Instance.LastModified (this);
        }

        public long Length ()
        {
            return FileHelper.Instance.Length (this);
        }

        public string[] List ()
        {
            try {
                if (IsFile ())
                    return null;
                return Directory.GetFileSystemEntries(_path).Select(filePth => Path.GetFileName(filePth)).ToArray();
            } catch {
                return null;
            }
        }

        public bool Mkdir ()
        {
            try {
                if (Directory.Exists (_path))
                    return false;
                Directory.CreateDirectory (_path);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        // Don't change the case of this method, since ngit does reflection on it

        // Don't change the case of this method, since ngit does reflection on it

        public string GetParent ()
        {
            var p = Path.GetDirectoryName (_path);
            return string.IsNullOrEmpty(p) || p == _path ? null : p;
        }

        public override string ToString ()
        {
            return _path;
        }
    }
}
