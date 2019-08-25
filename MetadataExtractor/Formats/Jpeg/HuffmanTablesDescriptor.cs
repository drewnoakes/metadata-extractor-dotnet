#region License
//
// Copyright 2002-2019 Drew Noakes
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

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Provides a human-readable string version of the tag stored in a HuffmanTablesDirectory.</summary>
    /// <remarks>
    /// Provides a human-readable string versions of the tag stored in a HuffmanTablesDirectory.
    /// <list type="bullet">
    ///   <item>https://en.wikipedia.org/wiki/Huffman_coding</item>
    ///   <item>http://stackoverflow.com/a/4954117</item>
    /// </list>
    /// </remarks>
    /// <author>Nadahar</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class HuffmanTablesDescriptor : TagDescriptor<HuffmanTablesDirectory>
    {
        public HuffmanTablesDescriptor(HuffmanTablesDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            switch (tagType)
            {
                case HuffmanTablesDirectory.TagNumberOfTables:
                    return GetNumberOfTablesDescription();
                default:
                    return base.GetDescription(tagType);
            }
        }

        public string? GetNumberOfTablesDescription()
        {
            if (!Directory.TryGetInt32(HuffmanTablesDirectory.TagNumberOfTables, out int value))
                return null;

            return value + (value == 1 ? " Huffman table" : " Huffman tables");
        }

    }
}
