using System.Collections.Generic;

namespace MetadataExtractor.Formats.Heif
{
    public class HeicImagePropertiesDirectory: Directory
    {
        public override string Name { get; }

        public HeicImagePropertiesDirectory(string name)
        {
            Name = name;
            SetDescriptor(new HeicImagePropertyDescriptor(this));
        }

        public const int ImageWidth = 1;
        public const int ImageHeight = 2;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>()
        {
            { ImageHeight, "Image Height" },
            { ImageWidth, "Image Width" }
        };


        protected override bool TryGetTagName(int tagType, out string? tagName) =>
            _tagNameMap.TryGetValue(tagType, out tagName);

        string? GetDescription(int tagType) => null;
    }

    public class HeicImagePropertyDescriptor: ITagDescriptor
    {
        private readonly HeicImagePropertiesDirectory dir;

        public HeicImagePropertyDescriptor(HeicImagePropertiesDirectory dir)
        {
            this.dir = dir;
        }

        public string? GetDescription(int tagType) => dir.GetString(tagType);
    }
}
