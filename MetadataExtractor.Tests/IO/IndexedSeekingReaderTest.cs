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
    /// <summary>Unit tests for <see cref="IndexedSeekingReader"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IndexedSeekingReaderTest : IndexedReaderTestBase, IDisposable
    {
        private string _tempFile;
        private Stream _stream;

        protected override IndexedReader CreateReader(params byte[] bytes)
        {
            try
            {
                // Unit tests can create multiple readers in the same test, as long as they're used one after the other
                DeleteTempFile();
                _tempFile = Path.GetTempFileName();
                File.WriteAllBytes(_tempFile, bytes);
                _stream = new FileStream(_tempFile, FileMode.Open, FileAccess.Read);
                return new IndexedSeekingReader(_stream);
            }
            catch (IOException ex)
            {
                throw new IOException("Unable to create temp file", ex);
            }
        }

        public void Dispose()
        {
            DeleteTempFile();
        }

        private void DeleteTempFile()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            if (_tempFile != null)
            {
                if (File.Exists(_tempFile))
                    File.Delete(_tempFile);
                _tempFile = null;
            }
        }

        [Fact]
        public void ConstructWithNullBufferThrows()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new IndexedSeekingReader(null));
        }
    }
}
