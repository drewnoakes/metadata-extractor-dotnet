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
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Tools;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Jpeg
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class JpegReaderTest
    {
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public static JpegDirectory ProcessBytes(string filePath)
        {
            Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
            new JpegReader().Extract(FileUtil.ReadBytes(filePath), metadata, JpegSegmentType.Sof0);
            JpegDirectory directory = metadata.GetFirstDirectoryOfType<JpegDirectory>();
            NUnit.Framework.Assert.IsNotNull(directory);
            return directory;
        }

        private JpegDirectory _directory;

        /// <exception cref="Com.Drew.Imaging.Jpeg.JpegProcessingException"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.SetUp]
        public virtual void SetUp()
        {
            _directory = ProcessBytes("Tests/Data/simple.jpg.sof0");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestExtract_Width()
        {
            Sharpen.Tests.AreEqual(800, _directory.GetInt(JpegDirectory.TagImageWidth));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestExtract_Height()
        {
            Sharpen.Tests.AreEqual(600, _directory.GetInt(JpegDirectory.TagImageHeight));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestExtract_DataPrecision()
        {
            Sharpen.Tests.AreEqual(8, _directory.GetInt(JpegDirectory.TagDataPrecision));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestExtract_NumberOfComponents()
        {
            Sharpen.Tests.AreEqual(3, _directory.GetInt(JpegDirectory.TagNumberOfComponents));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestComponentData1()
        {
            JpegComponent component = (JpegComponent)_directory.GetObject(JpegDirectory.TagComponentData1);
            NUnit.Framework.Assert.IsNotNull(component);
            Sharpen.Tests.AreEqual("Y", component.GetComponentName());
            Sharpen.Tests.AreEqual(1, component.GetComponentId());
            Sharpen.Tests.AreEqual(0, component.GetQuantizationTableNumber());
            Sharpen.Tests.AreEqual(2, component.GetHorizontalSamplingFactor());
            Sharpen.Tests.AreEqual(2, component.GetVerticalSamplingFactor());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestComponentData2()
        {
            JpegComponent component = (JpegComponent)_directory.GetObject(JpegDirectory.TagComponentData2);
            NUnit.Framework.Assert.IsNotNull(component);
            Sharpen.Tests.AreEqual("Cb", component.GetComponentName());
            Sharpen.Tests.AreEqual(2, component.GetComponentId());
            Sharpen.Tests.AreEqual(1, component.GetQuantizationTableNumber());
            Sharpen.Tests.AreEqual(1, component.GetHorizontalSamplingFactor());
            Sharpen.Tests.AreEqual(1, component.GetVerticalSamplingFactor());
            Sharpen.Tests.AreEqual("Cb component: Quantization table 1, Sampling factors 1 horiz/1 vert", _directory.GetDescription(JpegDirectory.TagComponentData2));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestComponentData3()
        {
            JpegComponent component = (JpegComponent)_directory.GetObject(JpegDirectory.TagComponentData3);
            NUnit.Framework.Assert.IsNotNull(component);
            Sharpen.Tests.AreEqual("Cr", component.GetComponentName());
            Sharpen.Tests.AreEqual(3, component.GetComponentId());
            Sharpen.Tests.AreEqual(1, component.GetQuantizationTableNumber());
            Sharpen.Tests.AreEqual(1, component.GetHorizontalSamplingFactor());
            Sharpen.Tests.AreEqual(1, component.GetVerticalSamplingFactor());
            Sharpen.Tests.AreEqual("Cr component: Quantization table 1, Sampling factors 1 horiz/1 vert", _directory.GetDescription(JpegDirectory.TagComponentData3));
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
