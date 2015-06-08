/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.IO;
using NUnit.Framework;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class RandomAccessFileReaderTest : RandomAccessTestBase
    {
        private FilePath _tempFile;

        private RandomAccessFile _randomAccessFile;

        protected internal override RandomAccessReader CreateReader(sbyte[] bytes)
        {
            try
            {
                // Unit tests can create multiple readers in the same test, as long as they're used one after the other
                DeleteTempFile();
                _tempFile = FilePath.CreateTempFile("metadata-extractor-test-", ".tmp");
                FileOutputStream stream = new FileOutputStream(_tempFile);
                stream.Write(bytes);
                stream.Close();
                _randomAccessFile = new RandomAccessFile(_tempFile, "r");
                return new RandomAccessFileReader(_randomAccessFile);
            }
            catch (IOException)
            {
                Assert.Fail("Unable to create temp file");
                return null;
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [TearDown]
        public virtual void DeleteTempFile()
        {
            if (_randomAccessFile == null)
            {
                return;
            }
            _randomAccessFile.Close();
            if (_tempFile == null)
            {
                return;
            }
            Tests.IsTrue("Unable to delete temp file used during unit test: " + _tempFile.GetAbsolutePath(), _tempFile.Delete());
            _tempFile = null;
            _randomAccessFile = null;
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual void TestConstructWithNullBufferThrows()
        {
            new RandomAccessFileReader(null);
        }
    }
}
