// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Iso14496
{
    internal static class TypeStringConverter
    {
        public static string ToTypeString(uint input)
        {
            return new(
                [
                    (char)(input >> 24),
                    (char)((input >> 16) & 0xFF),
                    (char)((input >> 8) & 0xFF),
                    (char)(input & 0xFF)
                ]);
        }

        public static uint ToTypeId(string typeString)
        {
            if (typeString.Length != 4)
            {
                throw new ArgumentException("Must be four characters long.", nameof(typeString));
            }

            return (uint)
                (((typeString[0] & 0xFF) << 24) |
                 ((typeString[1] & 0xFF) << 16) |
                 ((typeString[2] & 0xFF) << 8) |
                 (typeString[3] & 0xFF));
        }
    }
}
