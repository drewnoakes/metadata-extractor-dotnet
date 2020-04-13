using System.Collections.Generic;

namespace MetadataExtractor.Formats.Heif
{
    public class HeicThumbnailDirectory : Directory
    {
        public override string Name => "HEIC Thumbnail Data";

        public HeicThumbnailDirectory()
        {
            SetDescriptor(new HeicThumbnailTagDescriptor(this));
        }

        public const int TagFileOffset = 1;
        public const int TagLength = 2;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>()
        {
            { TagFileOffset, "Offset From Beginning of File" },
            { TagLength, "Data Length" }
        };

        protected override bool TryGetTagName(int tagType, out string? tagName) =>
            _tagNameMap.TryGetValue(tagType, out tagName);
    }
}
