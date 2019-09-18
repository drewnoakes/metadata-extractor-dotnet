// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PanasonicRawWbInfo2Directory"/>.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PanasonicRawWbInfo2Descriptor : TagDescriptor<PanasonicRawWbInfo2Directory>
    {
        public PanasonicRawWbInfo2Descriptor(PanasonicRawWbInfo2Directory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PanasonicRawWbInfo2Directory.TagWbType1:
                case PanasonicRawWbInfo2Directory.TagWbType2:
                case PanasonicRawWbInfo2Directory.TagWbType3:
                case PanasonicRawWbInfo2Directory.TagWbType4:
                case PanasonicRawWbInfo2Directory.TagWbType5:
                case PanasonicRawWbInfo2Directory.TagWbType6:
                case PanasonicRawWbInfo2Directory.TagWbType7:
                    return GetWbTypeDescription(tagType);
                default:
                    return base.GetDescription(tagType);
            }
        }

        public string? GetWbTypeDescription(int tagType)
        {
            if (!Directory.TryGetUInt16(tagType, out ushort value))
                return null;
            return ExifDescriptorBase<PanasonicRawWbInfo2Directory>.GetWhiteBalanceDescription(value);
        }
    }
}
