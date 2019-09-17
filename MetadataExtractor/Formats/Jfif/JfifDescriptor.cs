// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Jfif
{
    /// <summary>Provides human-readable string versions of the tags stored in a <see cref="JfifDirectory"/>.</summary>
    /// <remarks>
    /// Provides human-readable string versions of the tags stored in a <see cref="JfifDirectory"/>.
    /// <list type="bullet">
    ///   <item>http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format</item>
    ///   <item>http://www.w3.org/Graphics/JPEG/jfif3.pdf</item>
    /// </list>
    /// </remarks>
    /// <author>Yuri Binev, Drew Noakes</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JfifDescriptor : TagDescriptor<JfifDirectory>
    {
        public JfifDescriptor(JfifDirectory directory)
            : base(directory)
        {
        }

        public override string? GetDescription(int tagType)
        {
            return tagType switch
            {
                JfifDirectory.TagResX => GetImageResXDescription(),
                JfifDirectory.TagResY => GetImageResYDescription(),
                JfifDirectory.TagVersion => GetImageVersionDescription(),
                JfifDirectory.TagUnits => GetImageResUnitsDescription(),
                _ => base.GetDescription(tagType),
            };
        }

        public string? GetImageVersionDescription()
        {
            if (!Directory.TryGetInt32(JfifDirectory.TagVersion, out int value))
                return null;
            return $"{(value & 0xFF00) >> 8}.{value & 0x00FF}";
        }

        public string? GetImageResYDescription()
        {
            if (!Directory.TryGetInt32(JfifDirectory.TagResY, out int value))
                return null;
            return $"{value} dot{(value == 1 ? string.Empty : "s")}";
        }

        public string? GetImageResXDescription()
        {
            if (!Directory.TryGetInt32(JfifDirectory.TagResX, out int value))
                return null;
            return $"{value} dot{(value == 1 ? string.Empty : "s")}";
        }

        public string? GetImageResUnitsDescription()
        {
            if (!Directory.TryGetInt32(JfifDirectory.TagUnits, out int value))
                return null;
            return value switch
            {
                0 => "none",
                1 => "inch",
                2 => "centimetre",
                _ => "unit",
            };
        }
    }
}
