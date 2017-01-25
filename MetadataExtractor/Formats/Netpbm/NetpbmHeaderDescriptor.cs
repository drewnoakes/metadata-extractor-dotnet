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

using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Netpbm
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NetpbmHeaderDescriptor : TagDescriptor<NetpbmHeaderDirectory>
    {
        public NetpbmHeaderDescriptor([NotNull] NetpbmHeaderDirectory directory)
            : base(directory)
        {
        }

        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case NetpbmHeaderDirectory.TagFormatType:
                    return GetFormatTypeDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        [CanBeNull]
        private string GetFormatTypeDescription()
        {
            return GetIndexedDescription(NetpbmHeaderDirectory.TagFormatType, 1,
                "Portable BitMap (ASCII, B&W)",
                "Portable GrayMap (ASCII, B&W)",
                "Portable PixMap (ASCII, B&W)",
                "Portable BitMap (RAW, B&W)",
                "Portable GrayMap (RAW, B&W)",
                "Portable PixMap (RAW, B&W)",
                "Portable Arbitrary Map");
        }
    }
}