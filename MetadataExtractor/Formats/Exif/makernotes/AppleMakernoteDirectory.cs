using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>Describes tags specific to Apple cameras.</summary>
    /// <remarks>Using information from http://owl.phy.queensu.ca/~phil/exiftool/TagNames/Apple.html</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class AppleMakernoteDirectory : Directory
    {
        public const int TagRunTime      = 0x0003;
        public const int TagHdrImageType = 0x000a;
        public const int TagBurstUuid    = 0x000b;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagRunTime, "Run Time" },
            { TagHdrImageType, "HDR Image Type" },
            { TagBurstUuid, "Burst UUID" }
        };

        public AppleMakernoteDirectory()
        {
            SetDescriptor(new AppleMakernoteDescriptor(this));
        }

        public override string Name => "Apple Makernote";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}