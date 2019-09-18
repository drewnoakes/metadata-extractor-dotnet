// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
            return tagType switch
            {
                HuffmanTablesDirectory.TagNumberOfTables => GetNumberOfTablesDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetNumberOfTablesDescription()
        {
            if (!Directory.TryGetInt32(HuffmanTablesDirectory.TagNumberOfTables, out int value))
                return null;

            return value + (value == 1 ? " Huffman table" : " Huffman tables");
        }

    }
}
