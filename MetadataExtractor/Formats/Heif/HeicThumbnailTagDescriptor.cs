namespace MetadataExtractor.Formats.Heif
{
    public class HeicThumbnailTagDescriptor : TagDescriptor<HeicThumbnailDirectory>
    {
        public HeicThumbnailTagDescriptor(HeicThumbnailDirectory directory)
            : base(directory)
        {
        }
    }
}
