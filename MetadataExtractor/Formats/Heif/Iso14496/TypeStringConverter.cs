// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal static class TypeStringConverter
    {
        public static string ToTypeString(uint input)
        {
            return new string(
                new[]
                {
                    (char)(input >> 24),
                    (char)((input >> 16) & 0xFF),
                    (char)((input >> 8) & 0xFF),
                    (char)(input & 0xFF)
                });
        }
    }
}
