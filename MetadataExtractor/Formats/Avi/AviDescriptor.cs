// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Avi
{
    /// <author>Payton Garland</author>
    public class AviDescriptor : TagDescriptor<AviDirectory>
    {
        public AviDescriptor(AviDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case AviDirectory.TagWidth:
                case AviDirectory.TagHeight:
                    return Directory.GetString(tagType) + " pixels";
                default:
                    return base.GetDescription(tagType);
            }
        }
    }
}
