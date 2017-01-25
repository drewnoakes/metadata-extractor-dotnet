#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Pcx
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class PcxDescriptor : TagDescriptor<PcxDirectory>
    {
        public PcxDescriptor([NotNull] PcxDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case PcxDirectory.TagVersion:
                    return GetVersionDescription();
                case PcxDirectory.TagColorPlanes:
                    return GetColorPlanesDescription();
                case PcxDirectory.TagPaletteType:
                    return GetPaletteTypeDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        public string GetVersionDescription()
        {
            // Prior to v2.5 of PC Paintbrush, the PCX image file format was considered proprietary information
            // by ZSoft Corporation
            return GetIndexedDescription(PcxDirectory.TagVersion,
                "2.5 with fixed EGA palette information",
                null,
                "2.8 with modifiable EGA palette information",
                "2.8 without palette information (default palette)",
                "PC Paintbrush for Windows",
                "3.0 or better");
        }

        [CanBeNull]
        public string GetColorPlanesDescription()
        {
            return GetIndexedDescription(PcxDirectory.TagColorPlanes, 3, "24-bit color", "16 colors");
        }

        [CanBeNull]
        public string GetPaletteTypeDescription()
        {
            return GetIndexedDescription(PcxDirectory.TagPaletteType, 1, "Color or B&W", "Grayscale");
        }
    }
}
