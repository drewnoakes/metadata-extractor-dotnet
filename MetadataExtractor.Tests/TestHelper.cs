// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace MetadataExtractor.Tests
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class TestHelper
    {
        public static byte[] SkipBytes(byte[] input, int countToSkip)
        {
            if (input.Length - countToSkip < 0)
                throw new ArgumentException("Attempting to skip more bytes than exist in the array.");

            var output = new byte[input.Length - countToSkip];
            Array.Copy(input, countToSkip, output, 0, input.Length - countToSkip);
            return output;
        }
    }
}
