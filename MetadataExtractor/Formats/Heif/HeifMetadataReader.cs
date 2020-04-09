using System;
using System.Collections.Generic;
using System.IO;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.QuickTime;
#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif


namespace MetadataExtractor.Formats.Heif
{
    public class HeifMetadataReader
    {
        public static DirectoryList ReadMetadata(Stream stream) => new HeifMetadataReader(stream).Process();

        private readonly Stream _stream;
        private HeifMetadataReader(Stream stream)
        {
            _stream = stream;
        }

        List<Directory> _directories = new List<Directory>();
        private DirectoryList Process()
        {
            return _directories;
        }

    }

    public class DataReferenceDirectory : Directory
    {
        public const int Version = 1;
        public const int Flags = 2;
        public const int Items = 3;

        public static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>()
        {
            { Version, "Data Reference Version" },
            { Flags, "Flags" },
            { Items, "Numeber of Entries" }
        };

        public override string Name { get; } = "Hief Metadata";
        protected override bool TryGetTagName(int tagType, out string? tagName)
        {
            throw new NotImplementedException();
        }
    }
}
