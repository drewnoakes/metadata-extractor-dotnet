using System;
using System.IO;

namespace Sharpen
{
    public class FileWriter : StreamWriter
    {
        public FileWriter(string path, bool append): base(path)
        {
            if(append)
                throw new NotSupportedException();
        }
    }
}
