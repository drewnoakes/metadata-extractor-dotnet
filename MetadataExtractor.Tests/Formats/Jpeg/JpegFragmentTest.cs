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
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <summary>Unit tests for <see cref="JpegFragment"/>.</summary>
    /// <author>Michael Osthege</author>
    public sealed class JpegFragmentTest
    {
        [Fact]
        public void TestFromJpegSegment()
        {
            IEnumerable<JpegSegment> segments;
            using (var stream = TestDataUtil.OpenRead("Data/withExifAndIptc.jpg"))
                segments = JpegSegmentReader.ReadSegments(new SequentialStreamReader(stream)).ToList();

            foreach (JpegSegment segment in segments)
            {
                JpegFragment fragment = JpegFragment.FromJpegSegment(segment);
                Assert.Equal(2 + 2 + segment.Bytes.Length, fragment.Bytes.Length);
                byte[] payloadBytes = fragment.Bytes.Skip(4).ToArray();
                Assert.Equal(payloadBytes, segment.Bytes);
            }
        }

        [Fact]
        public void TestSplitSingleFragment()
        {
            // The test file contains exactly one App1 XMP segment (marker, size, payload)
            string pathApp1 = "Data/xmpWriting_MicrosoftXmp.app1";

            List<JpegFragment> fragments;
            using (var stream = TestDataUtil.OpenRead(pathApp1))
                fragments = JpegFragmentWriter.SplitFragments(new SequentialStreamReader(stream));
            byte[] fileBytes = TestDataUtil.GetBytes(pathApp1);

            Assert.Single(fragments);
            Assert.True(fragments.First().Bytes.SequenceEqual(fileBytes));
            Assert.NotNull(fragments.First().Segment);
            Assert.Equal(JpegSegmentType.App1, fragments.First().Segment.Type);
        }

        [Fact]
        public void TestFindsFragment()
        {
            // The file is an image that contains an App1 Xmp segment
            string pathJpeg = "Data/xmpWriting_PictureWithMicrosoftXmp.jpg";
            string pathApp1 = "Data/xmpWriting_MicrosoftXmp.app1";

            List<JpegFragment> fragments;
            using (var stream = TestDataUtil.OpenRead(pathJpeg))
                fragments = JpegFragmentWriter.SplitFragments(new SequentialStreamReader(stream));
            byte[] xmpFragmentBytes = TestDataUtil.GetBytes(pathApp1);

            bool foundXmpFragment = false;
            foreach (var fragment in fragments)
            {
                if (fragment.Bytes.Length == xmpFragmentBytes.Length)
                {
                    Assert.True(fragment.Bytes.SequenceEqual(xmpFragmentBytes));
                    foundXmpFragment = true;
                }
            }
            Assert.True(foundXmpFragment, "The Xmp App1 fragment was not found correctly.");
        }

        [Fact]
        public void TestSplitConcatenation()
        {
            // The file is an image that contains an App1 Xmp segment
            string pathJpeg = "Data/xmpWriting_PictureWithMicrosoftXmp.jpg";

            List<JpegFragment> fragments;
            using (var stream = TestDataUtil.OpenRead(pathJpeg))
                fragments = JpegFragmentWriter.SplitFragments(new SequentialStreamReader(stream));
            byte[] original = File.ReadAllBytes(pathJpeg);

            int nRead = fragments.Select(f => f.Bytes.Length).Sum();
            byte[] joined = JpegFragmentWriter.JoinFragments(fragments).ToArray();

            Assert.Equal(original.Length, nRead);
            Assert.Equal(original.Length, joined.Length);
            Assert.True(original.SequenceEqual(joined));
        }

    }
}
