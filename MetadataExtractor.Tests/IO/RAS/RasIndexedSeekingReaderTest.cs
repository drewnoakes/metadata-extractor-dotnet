// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>Unit tests for <see cref="RandomAccessStream"/> with indexed reading on a FileStream.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class RasIndexedSeekingReaderTest : RasIndexedTestBase, IDisposable
    {
        private string? _tempFile;
        private Stream? _stream;

        protected override ReaderInfo CreateReader(params byte[] bytes)
        {
            try
            {
                // Unit tests can create multiple readers in the same test, as long as they're used one after the other
                DeleteTempFile();
                _tempFile = Path.GetTempFileName();
                File.WriteAllBytes(_tempFile, bytes);
                _stream = new FileStream(_tempFile, FileMode.Open, FileAccess.Read);
                return ReaderInfo.CreateFromStream(_stream);
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
            Assert.Throws<ArgumentNullException>(() => ReaderInfo.CreateFromStream(null!));
        }
    }
}
