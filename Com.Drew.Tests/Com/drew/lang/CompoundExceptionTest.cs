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

using System;
using System.IO;
using NUnit.Framework;

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class CompoundExceptionTest
    {
        [Test]
        public void TestGetMessage_NonNested()
        {
            try
            {
                throw new CompoundException("message");
            }
            catch (CompoundException e)
            {
                Assert.AreEqual("message", e.Message);
            }
        }

        [Test]
        public void TestGetMessage_Nested()
        {
            try
            {
                try
                {
                    throw new IOException("io");
                }
                catch (IOException e)
                {
                    throw new CompoundException("compound", e);
                }
            }
            catch (CompoundException e)
            {
                Assert.AreEqual("compound", e.Message);
                var innerException = e.GetInnerException();
                Assert.IsNotNull(innerException);
                Assert.AreEqual("io", innerException.Message);
            }
        }

        [Test]
        public void TestNoInnerException()
        {
            try
            {
                throw new CompoundException("message");
            }
            catch (CompoundException e)
            {
                try
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    e.ToString();
                }
                catch (Exception)
                {
                    Assert.Fail("Exception during CompoundException.ToString when no inner exception exists");
                }
            }
        }
    }
}
