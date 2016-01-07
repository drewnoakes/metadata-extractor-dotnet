namespace MetadataExtractor
{
    public class DeferredTagValue
    {
        public DeferredTagValue(int tagId, int tagValueOffset, int tiffHeaderOffset)
        {
            //TagParentDirectory = tagParentDirectory;
            TagId = tagId;
            TagValueOffset = tagValueOffset;
            TiffHeaderOffset = tiffHeaderOffset;
        }

        //public Directory TagParentDirectory { get; set; }
        public int TagId { get; set; }
        public int TagValueOffset { get; set; }
        public int TiffHeaderOffset { get; set; }
    }
}
