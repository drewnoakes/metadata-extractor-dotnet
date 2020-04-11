using System.Collections.Generic;

namespace MetadataExtractor.Formats.Heif
{
    public class HeicThumbnailDirectory: Directory
    {
        public override string Name => "HEIC Thumbnail Data";

        public HeicThumbnailDirectory()
        {
            SetDescriptor(new HeicThumbnailTagDescriptor(this));
        }

        public const int FileOffset = 1;
        public const int Length = 2;
        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>()
        {
            {FileOffset, "Offset From Beginning of File"},
            {Length, "Data Length"}
        };

        protected override bool TryGetTagName(int tagType, out string? tagName) =>
            _tagNameMap.TryGetValue(tagType, out tagName);
    }

    public class HeicThumbnailTagDescriptor : ITagDescriptor
    {
        private readonly HeicThumbnailDirectory _heicThumbnailDirectory;

        public HeicThumbnailTagDescriptor(HeicThumbnailDirectory heicThumbnailDirectory)
        {
            _heicThumbnailDirectory = heicThumbnailDirectory;
        }

        public string? GetDescription(int tagType) =>
            _heicThumbnailDirectory.GetObject(tagType)?.ToString()??"";
    }
}
