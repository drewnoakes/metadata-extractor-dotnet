using System.IO;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.File
{
    public class FileMetadataReader
    {
        /// <exception cref="System.IO.IOException"/>
        public virtual void Read([NotNull] FilePath file, [NotNull] Metadata metadata)
        {
            if (!file.IsFile())
            {
                throw new IOException("File object must reference a file");
            }
            if (!file.Exists())
            {
                throw new IOException("File does not exist");
            }
            if (!file.CanRead())
            {
                throw new IOException("File is not readable");
            }
            FileMetadataDirectory directory = new FileMetadataDirectory();
            directory.SetString(FileMetadataDirectory.TagFileName, file.GetName());
            directory.SetLong(FileMetadataDirectory.TagFileSize, file.Length());
            directory.SetDate(FileMetadataDirectory.TagFileModifiedDate, Extensions.CreateDate(file.LastModified()));
            metadata.AddDirectory(directory);
        }
    }
}
