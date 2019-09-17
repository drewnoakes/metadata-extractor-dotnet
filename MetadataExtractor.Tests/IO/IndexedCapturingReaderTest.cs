// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>Unit tests for <see cref="IndexedCapturingReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IndexedCapturingReaderTest : IndexedReaderTestBase
    {
        [Fact]
        public void ConstructWithNullBufferThrows()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new IndexedCapturingReader(null!));
        }

        protected override IndexedReader CreateReader(params byte[] bytes)
        {
            return new IndexedCapturingReader(new MemoryStream(bytes));
        }
    }
}
