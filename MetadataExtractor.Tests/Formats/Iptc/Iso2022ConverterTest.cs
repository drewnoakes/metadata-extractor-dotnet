// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Iptc;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Iptc
{
    /// <summary>Unit tests for <see cref="Iso2022Converter"/>.</summary>
    public sealed class Iso2022ConverterTest
    {
        [Fact]
        public void ConvertEscapeSequenceToEncodingName()
        {
            Assert.Equal("UTF-8", Iso2022Converter.ConvertEscapeSequenceToEncodingName(new byte[] { 0x1B, 0x25, 0x47 }));
            Assert.Equal("ISO-8859-1", Iso2022Converter.ConvertEscapeSequenceToEncodingName(new byte[] { 0x1B, 0xE2, 0x80, 0xA2, 0x41 }));
            Assert.Null(Iso2022Converter.ConvertEscapeSequenceToEncodingName(new byte[] { 1, 2, 3, 4 }));
        }
    }
}
