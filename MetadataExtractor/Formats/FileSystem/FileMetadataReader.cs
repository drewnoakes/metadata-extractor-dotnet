using System.IO;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.FileSystem
{
    public sealed class FileMetadataReader
    {
        /// <exception cref="System.IO.IOException"/>
        public void Read([NotNull] string file, [NotNull] Metadata metadata)
        {
            var attr = File.GetAttributes(file);
            if (attr.HasFlag(FileAttributes.Directory))
                throw new IOException("File object must reference a file");

            var fileInfo = new FileInfo(file);
            if (!fileInfo.Exists)
                throw new IOException("File does not exist");

            var directory = new FileMetadataDirectory();
            directory.SetString(FileMetadataDirectory.TagFileName, Path.GetFileName(file));
            directory.SetLong(FileMetadataDirectory.TagFileSize, fileInfo.Length);
            directory.SetDate(FileMetadataDirectory.TagFileModifiedDate, fileInfo.LastWriteTime);
            metadata.AddDirectory(directory);
        }
    }
}
