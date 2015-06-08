using System;
using System.IO;

namespace Sharpen
{
    public class FileWriter : StreamWriter
    {
        public FileWriter (FilePath path) : base(path.GetPath ())
        {
        }

        public FileWriter(string path, bool append): base(path)
        {
            if(append)
                throw new NotSupportedException();
        }

        public FileWriter Append (string sequence)
        {
            Write (sequence);
            return this;
        }
    }
}
