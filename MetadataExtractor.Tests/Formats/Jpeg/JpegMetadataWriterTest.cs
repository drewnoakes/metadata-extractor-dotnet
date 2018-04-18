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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegMetadataWriter"/>.</summary>
    /// <author>Michael Osthege</author>
    public sealed class JpegMetadataWriterTest
    {
        /// <summary>
        /// This test is very similar to <see cref="MetadataExtractor.Tests.Formats.Xmp.XmpWriterTest.TestUpdateFragments_Replace"/>
        /// but acts on one abstraction level higher, using a collection of metadata objects.
        /// It tests if the <see cref="JpegMetadataWriter.UpdateJpegFragments()"/> correctly uses a
        /// compatible metadata writer.
        /// </summary>
        [Fact]
        public void TestUpdateJpegFragments_Replace()
        {
            // substitute the original with re-encoded Xmp content
            List<JpegFragment> originalFragments = null;
            using (var stream = TestDataUtil.OpenRead("Data/xmpWriting_PictureWithMicrosoftXmp.jpg"))
                originalFragments = JpegFragmentWriter.SplitFragments(new SequentialStreamReader(stream));
            XDocument xmp = XDocument.Parse(File.ReadAllText("Data/xmpWriting_XmpContent.xmp"));
            byte[] originalApp1 = File.ReadAllBytes("Data/xmpWriting_MicrosoftXmp.app1");
            byte[] expectedApp1 = File.ReadAllBytes("Data/xmpWriting_MicrosoftXmpReencoded.app1");

            var metadata_objects = new object[] { xmp };
            var updatedFragments = JpegMetadataWriter.UpdateJpegFragments(originalFragments, metadata_objects);

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
        public void TestUpdateJpegFragments_FailsOnUnknownMetadataObject()
        {
            // substitute the original with re-encoded Xmp content
            List<JpegFragment> originalFragments = null;
            using (var stream = TestDataUtil.OpenRead("Data/xmpWriting_PictureWithMicrosoftXmp.jpg"))
                originalFragments = JpegFragmentWriter.SplitFragments(new SequentialStreamReader(stream));
            XDocument xmp = XDocument.Parse(File.ReadAllText("Data/xmpWriting_XmpContent.xmp"));

            Assert.Throws<NotImplementedException>(delegate
            {
                var metadata_objects = new object[] { xmp, "This is not a supported metadata object." };
                var updatedFragments = JpegMetadataWriter.UpdateJpegFragments(originalFragments, metadata_objects);
            });
        }

        [Fact]
        public void TestWriteMetadata()
        {
            var originalStream = TestDataUtil.OpenRead("Data/xmpWriting_PictureWithMicrosoftXmp.jpg");
            XDocument xmp = XDocument.Parse(File.ReadAllText("Data/xmpWriting_XmpContent.xmp"));
            byte[] expectedResult = TestDataUtil.GetBytes("Data/xmpWriting_PictureWithMicrosoftXmpReencoded.jpg");

            var metadata_objects = new object[] { xmp };
            var updatedStream = JpegMetadataWriter.WriteMetadata(originalStream, metadata_objects);

            var actualResult = updatedStream.ToArray();

            Assert.True(actualResult.SequenceEqual(expectedResult));
        }
    }
}
