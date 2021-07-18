// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>Unit tests for <see cref="RandomAccessStream"/> with sequential reading on a MemoryStream.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class RasSequentialStreamReaderTest : RasSequentialReaderTestBase
    {
        [Fact]
        public void ConstructWithNullStreamThrows()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => ReaderInfo.CreateFromStream(null!));
        }

        protected override ReaderInfo CreateReader(byte[] bytes)
        {
            return ReaderInfo.CreateFromStream(new MemoryStream(bytes));
        }
    }
}
