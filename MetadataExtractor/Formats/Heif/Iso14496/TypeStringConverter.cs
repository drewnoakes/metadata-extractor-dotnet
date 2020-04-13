// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal static class TypeStringConverter
    {
        public static uint ToCode(string s)
        {
            uint ret = 0;
            for (int i = 0; i < 4; i++)
            {
                ret = ret << 8;
                ret |= s[i];
            }

            return ret;
        }

        public static string ToTypeString(uint input)
        {
            return string.Concat(MakeChar(input >> 24), MakeChar(input >> 16), MakeChar(input >> 8), MakeChar(input));
        }

        private static char MakeChar(uint v)
        {
            return (char)(v & 0xFF);
        }
    }
}
