// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>Unit tests for <see cref="ByteArrayReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ByteArrayReaderTest : IndexedReaderTestBase
    {
        protected override IndexedReader CreateReader(params byte[] bytes)
        {
            return new ByteArrayReader(bytes);
        }

        [Fact]
        public void ConstructWithNullBufferThrows()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new ByteArrayReader(null!));
        }
    }
}
