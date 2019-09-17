// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>Unit tests for <see cref="SequentialStreamReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class SequentialStreamReaderTest : SequentialReaderTestBase
    {
        [Fact]
        public void ConstructWithNullStreamThrows()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new SequentialStreamReader(null!));
        }

        protected override SequentialReader CreateReader(byte[] bytes)
        {
            return new SequentialStreamReader(new MemoryStream(bytes));
        }
    }
}
