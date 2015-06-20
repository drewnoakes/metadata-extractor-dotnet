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
using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Jpeg
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegReaderTest
    {
        [NotNull]
        private static JpegDirectory ProcessBytes([NotNull] string filePath)
        {
            var directory = new JpegReader().Extract(File.ReadAllBytes(filePath), JpegSegmentType.Sof0);
            Assert.NotNull(directory);
            return directory;
        }

        private readonly JpegDirectory _directory;

        public JpegReaderTest()
        {
            _directory = ProcessBytes("Tests/Data/simple.jpg.sof0");
        }


        [Fact]
        public void TestExtract_Width()
        {
            Assert.Equal(800, _directory.GetInt32(JpegDirectory.TagImageWidth));
        }


        [Fact]
        public void TestExtract_Height()
        {
            Assert.Equal(600, _directory.GetInt32(JpegDirectory.TagImageHeight));
        }


        [Fact]
        public void TestExtract_DataPrecision()
        {
            Assert.Equal(8, _directory.GetInt32(JpegDirectory.TagDataPrecision));
        }


        [Fact]
        public void TestExtract_NumberOfComponents()
        {
            Assert.Equal(3, _directory.GetInt32(JpegDirectory.TagNumberOfComponents));
        }


        [Fact]
        public void TestComponentData1()
        {
            var component = (JpegComponent)_directory.GetObject(JpegDirectory.TagComponentData1);
            Assert.NotNull(component);
            Assert.Equal("Y", component.GetComponentName());
            Assert.Equal(1, component.GetComponentId());
            Assert.Equal(0, component.GetQuantizationTableNumber());
            Assert.Equal(2, component.GetHorizontalSamplingFactor());
            Assert.Equal(2, component.GetVerticalSamplingFactor());
        }


        [Fact]
        public void TestComponentData2()
        {
            var component = (JpegComponent)_directory.GetObject(JpegDirectory.TagComponentData2);
            Assert.NotNull(component);
            Assert.Equal("Cb", component.GetComponentName());
            Assert.Equal(2, component.GetComponentId());
            Assert.Equal(1, component.GetQuantizationTableNumber());
            Assert.Equal(1, component.GetHorizontalSamplingFactor());
            Assert.Equal(1, component.GetVerticalSamplingFactor());
            Assert.Equal("Cb component: Quantization table 1, Sampling factors 1 horiz/1 vert", _directory.GetDescription(JpegDirectory.TagComponentData2));
        }


        [Fact]
        public void TestComponentData3()
        {
            var component = (JpegComponent)_directory.GetObject(JpegDirectory.TagComponentData3);
            Assert.NotNull(component);
            Assert.Equal("Cr", component.GetComponentName());
            Assert.Equal(3, component.GetComponentId());
            Assert.Equal(1, component.GetQuantizationTableNumber());
            Assert.Equal(1, component.GetHorizontalSamplingFactor());
            Assert.Equal(1, component.GetVerticalSamplingFactor());
            Assert.Equal("Cr component: Quantization table 1, Sampling factors 1 horiz/1 vert", _directory.GetDescription(JpegDirectory.TagComponentData3));
        }
/*
    // this test is part of an incomplete investigation into extracting audio from JPG files
    public void testJpegWithAudio() throws Exception
    {
        // use a known testing image
        File jpegFile = new File("Tests/com/drew/metadata/jpeg/audioPresent.jpg");

        JpegSegmentReader jpegSegmentReader = new JpegSegmentReader(jpegFile);
        byte[] segment1Bytes = jpegSegmentReader.readSegment(JpegSegmentReader.APP2);
        System.out.println(segment1Bytes.length);

//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APP1));
        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APP2).length);
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APP3));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APP4));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APP5));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APP6));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APP7));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APP8));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APP9));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APPA));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APPB));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APPC));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APPD));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APPE));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.APPF));
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.COM));
        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.DHT).length);
        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.DQT).length);
        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.SOF0).length);
//        System.out.println(jpegSegmentReader.readSegment(JpegSegmentReader.SOI));

        // write the segment's data out to a wav file...
        File audioFile = new File("Tests/com/drew/metadata/jpeg/audio.wav");
        FileOutputStream os = null;
        try
        {
            os = new FileOutputStream(audioFile);
            os.write(segment1Bytes);
        }
        finally
        {
            if (os!=null)
                os.close();
        }
    }
*/
    }
}
