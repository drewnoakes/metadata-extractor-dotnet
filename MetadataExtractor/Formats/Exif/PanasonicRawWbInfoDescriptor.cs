// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="PanasonicRawWbInfoDirectory"/>.
    /// </summary>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PanasonicRawWbInfoDescriptor : TagDescriptor<PanasonicRawWbInfoDirectory>
    {
        public PanasonicRawWbInfoDescriptor(PanasonicRawWbInfoDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PanasonicRawWbInfoDirectory.TagWbType1:
                case PanasonicRawWbInfoDirectory.TagWbType2:
                case PanasonicRawWbInfoDirectory.TagWbType3:
                case PanasonicRawWbInfoDirectory.TagWbType4:
                case PanasonicRawWbInfoDirectory.TagWbType5:
                case PanasonicRawWbInfoDirectory.TagWbType6:
                case PanasonicRawWbInfoDirectory.TagWbType7:
                    return GetWbTypeDescription(tagType);
                default:
                    return base.GetDescription(tagType);
            }
        }

        public string? GetWbTypeDescription(int tagType)
        {
            if (!Directory.TryGetUInt16(tagType, out ushort value))
                return null;
            return ExifDescriptorBase<PanasonicRawWbInfoDirectory>.GetWhiteBalanceDescription(value);
        }
    }
}
