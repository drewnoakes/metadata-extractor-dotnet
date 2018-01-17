using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="DJIMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>Using information from https://metacpan.org/pod/distribution/Image-ExifTool/lib/Image/ExifTool/TagNames.pod#DJI-Tags</remarks>
    /// <author>Charlie Matherne, adapted from Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class DJIMakernoteDescriptor : TagDescriptor<DJIMakernoteDirectory>
    {
        public DJIMakernoteDescriptor([NotNull] DJIMakernoteDirectory directory)
            : base(directory)
        {
        }
    }
}