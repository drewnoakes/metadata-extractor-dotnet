#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using System.IO;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>
    /// Wrapper for MemoryStream that allows us to configure whether it is
    /// seekable, and whether or not the Length property is supported.
    /// </summary>
    public class ConfigurableMemoryStream : MemoryStream
    {
        private bool _seekable;
        private bool _lengthSupported;

        public ConfigurableMemoryStream(byte[] buffer, bool seekable, bool lengthSupported) : base(buffer)
        {
            _seekable = seekable;
            _lengthSupported = lengthSupported;
        }

        public override long Length
        {
            get
            {
                if (_lengthSupported)
                {
                    return base.Length;
                }
                else
                {
                    throw new NotSupportedException("Length property was disabled");
                }
            }
        }

        public override bool CanSeek => _seekable;
    }

    /// <summary>Unit tests for <see cref="IndexedCapturingReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class StreamingIndexedCapturingReaderTestBase : IndexedReaderTestBase
    {
        [Fact]
        public void ConstructWithNullBufferThrows()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new IndexedCapturingReader(null));
        }
    }

    // Since the normal IndexedCapturingReaderTest uses MemoryStream, which both
    // supports the Length property and is seekable, the following classes test
    // the remaining permutations of options:
    // 
    // * non-seekable, has length
    // * seekable, doesn't have length
    // * non-seekable, doesn't have length
    public sealed class NonSeekableLengthSupportedIndexedCapturingReaderTest : StreamingIndexedCapturingReaderTestBase
    {
        protected override IndexedReader CreateReader(params byte[] bytes)
        {
            return new IndexedCapturingReader(new ConfigurableMemoryStream(bytes, false, true));
        }
    }

    public sealed class SeekableLengthUnsupportedIndexedCapturingReaderTest : StreamingIndexedCapturingReaderTestBase
    {
        protected override IndexedReader CreateReader(params byte[] bytes)
        {
            return new IndexedCapturingReader(new ConfigurableMemoryStream(bytes, true, false));
        }
    }

    public sealed class NonSeekableLengthUnsupportedIndexedCapturingReaderTest : StreamingIndexedCapturingReaderTestBase
    {
        protected override IndexedReader CreateReader(params byte[] bytes)
        {
            return new IndexedCapturingReader(new ConfigurableMemoryStream(bytes, false, false));
        }
    }
}
