// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>Unit tests for <see cref="RandomAccessStream"/> with indexed reading on a byte array.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class RasByteArrayReaderTest : RasIndexedTestBase
    {
        protected override ReaderInfo CreateReader(params byte[] bytes)
        {
            return ReaderInfo.CreateFromArray(bytes);
        }

        [Fact]
        public void ConstructWithNullBufferThrows()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => CreateReader(null!));
        }

    }
}
