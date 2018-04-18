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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Xmp
{
    /// <summary>Unit tests for <see cref="XmpWriter"/>.</summary>
    /// <author>Michael Osthege</author>
    public sealed class XmpWriterTest
    {
        [Fact]
        public void TestEncodeXmpToPayloadBytes()
        {
            XDocument xmp = XDocument.Parse(File.ReadAllText("Data/xmpWriting_XmpContent.xmp"));
            byte[] payloadBytes = XmpWriter.EncodeXmpToPayloadBytes(xmp);
            JpegSegment app1 = new JpegSegment(JpegSegmentType.App1, payloadBytes, offset: 0);
            JpegFragment frag = JpegFragment.FromJpegSegment(app1);

            byte[] expected = File.ReadAllBytes("Data/xmpWriting_MicrosoftXmpReencoded.app1");

            Assert.Equal(expected.Length, frag.Bytes.Length);
            Assert.True(frag.Bytes.SequenceEqual(expected));
        }

        [Fact]
        public void TestUpdateFragments_Replace()
        {
            // substitute the original with re-encoded Xmp content
            List<JpegFragment> originalFragments = null;
            using (var stream = TestDataUtil.OpenRead("Data/xmpWriting_PictureWithMicrosoftXmp.jpg"))
                originalFragments = JpegFragmentWriter.SplitFragments(new SequentialStreamReader(stream));
            XDocument xmp = XDocument.Parse(File.ReadAllText("Data/xmpWriting_XmpContent.xmp"));
            byte[] originalApp1 = File.ReadAllBytes("Data/xmpWriting_MicrosoftXmp.app1");
            byte[] expectedApp1 = File.ReadAllBytes("Data/xmpWriting_MicrosoftXmpReencoded.app1");

            var writer = new XmpWriter();
            var updatedFragments = writer.UpdateFragments(originalFragments, xmp);

            Assert.Equal(originalFragments.Count, updatedFragments.Count);
            // Check that only the App1 Xmp fragment is modified
            for (int i = 0; i < originalFragments.Count; i++)
            {
                var ofrag = originalFragments[i];
                var ufrag = updatedFragments[i];

                if (ofrag.Segment?.Type == JpegSegmentType.App1 && ofrag.Bytes.SequenceEqual(originalApp1))
                {
                    // If this fragment is the original Xmp fragment, we expect the updated fragment
                    Assert.True(ufrag.Bytes.SequenceEqual(expectedApp1));
                }
                else
                {
                    // In all other cases, the fragments must remain identical
                    Assert.True(ufrag.Bytes.SequenceEqual(ofrag.Bytes));
                }
            }
        }

        [Fact]
        public void TestUpdateFragments_Insert()
        {
            // substitute the original with re-encoded Xmp content
            List<JpegFragment> originalFragments = null;
            using (var stream = TestDataUtil.OpenRead("Data/xmpWriting_PictureWithoutXmp.jpg"))
                originalFragments = JpegFragmentWriter.SplitFragments(new SequentialStreamReader(stream));
            XDocument xmp = XDocument.Parse(File.ReadAllText("Data/xmpWriting_XmpContent.xmp"));
            byte[] originalApp1 = File.ReadAllBytes("Data/xmpWriting_MicrosoftXmp.app1");
            byte[] expectedApp1 = File.ReadAllBytes("Data/xmpWriting_MicrosoftXmpReencoded.app1");

            var writer = new XmpWriter();
            var updatedFragments = writer.UpdateFragments(originalFragments, xmp);

            Assert.True(updatedFragments.Count == originalFragments.Count + 1);
            // Check that only the App1 Xmp fragment is modified
            bool foundInsertedFragment = false;
            for (int i = 0; i < originalFragments.Count; i++)
            {
                // for all fragments after the inserted App1, the previous original fragment must be selected
                var ofrag = originalFragments[(foundInsertedFragment ? i - 1 : i)];
                var ufrag = updatedFragments[i];

                if (ufrag.Bytes.SequenceEqual(expectedApp1))
                {
                    foundInsertedFragment = true;
                }
                else
                {
                    // In all other cases, the fragments must remain identical
                    Assert.True(ufrag.Bytes.SequenceEqual(ofrag.Bytes));
                }
            }
            Assert.True(foundInsertedFragment);
        }

        [Fact]
        public void TestCreateWhitespace()
        {
            string created = XmpWriter.CreateWhitespace(size: 12345);
            Assert.Equal(12345, created.Length);
        }
    }
}
