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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Directory of tags and values for the SOF0 JPEG segment.</summary>
    /// <remarks>This segment holds basic metadata about the image.</remarks>
    /// <author>Darrell Silver http://www.darrellsilver.com and Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JpegDirectory : Directory
    {
        public const int TagCompressionType = -3;

        /// <summary>This is in bits/sample, usually 8 (12 and 16 not supported by most software).</summary>
        public const int TagDataPrecision = 0;

        /// <summary>The image's height.</summary>
        /// <remarks>Necessary for decoding the image, so it should always be there.</remarks>
        public const int TagImageHeight = 1;

        /// <summary>The image's width.</summary>
        /// <remarks>Necessary for decoding the image, so it should always be there.</remarks>
        public const int TagImageWidth = 3;

        /// <summary>
        /// Usually 1 = grey scaled, 3 = color YcbCr or YIQ, 4 = color CMYK
        /// Each component TAG_COMPONENT_DATA_[1-4], has the following meaning:
        /// component Id(1byte)(1 = Y, 2 = Cb, 3 = Cr, 4 = I, 5 = Q),
        /// sampling factors (1byte) (bit 0-3 vertical., 4-7 horizontal.),
        /// quantization table number (1 byte).
        /// </summary>
        /// <remarks>
        /// This info is from http://www.funducode.com/freec/Fileformats/format3/format3b.htm
        /// </remarks>
        public const int TagNumberOfComponents = 5;

        /// <summary>The first of a possible 4 color components.</summary>
        /// <remarks>The number of components specified in <see cref="TagNumberOfComponents"/>.</remarks>
        public const int TagComponentData1 = 6;

        /// <summary>The second of a possible 4 color components.</summary>
        /// <remarks>The number of components specified in <see cref="TagNumberOfComponents"/>.</remarks>
        public const int TagComponentData2 = 7;

        /// <summary>The third of a possible 4 color components.</summary>
        /// <remarks>The number of components specified in <see cref="TagNumberOfComponents"/>.</remarks>
        public const int TagComponentData3 = 8;

        /// <summary>The fourth of a possible 4 color components.</summary>
        /// <remarks>The number of components specified in <see cref="TagNumberOfComponents"/>.</remarks>
        public const int TagComponentData4 = 9;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagCompressionType, "Compression Type" },
            { TagDataPrecision, "Data Precision" },
            { TagImageWidth, "Image Width" },
            { TagImageHeight, "Image Height" },
            { TagNumberOfComponents, "Number of Components" },
            { TagComponentData1, "Component 1" },
            { TagComponentData2, "Component 2" },
            { TagComponentData3, "Component 3" },
            { TagComponentData4, "Component 4" }
        };

        public JpegDirectory()
        {
            SetDescriptor(new JpegDescriptor(this));
        }

        public override string Name => "JPEG";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        /// <param name="componentNumber">
        /// The zero-based index of the component.  This number is normally between 0 and 3.
        /// Use <see cref="GetNumberOfComponents"/> for bounds-checking.
        /// </param>
        /// <returns>the JpegComponent having the specified number, or <c>null</c>.</returns>
        [CanBeNull]
        public JpegComponent GetComponent(int componentNumber)
        {
            var tagType = TagComponentData1 + componentNumber;
            return GetObject(tagType) as JpegComponent;
        }

        /// <exception cref="MetadataException"/>
        public int GetImageWidth()
        {
            return this.GetInt32(TagImageWidth);
        }

        /// <exception cref="MetadataException"/>
        public int GetImageHeight()
        {
            return this.GetInt32(TagImageHeight);
        }

        /// <exception cref="MetadataException"/>
        public int GetNumberOfComponents()
        {
            return this.GetInt32(TagNumberOfComponents);
        }
    }
}
