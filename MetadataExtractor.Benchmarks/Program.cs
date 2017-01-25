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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using System.Windows.Media.Imaging;

namespace MetadataExtractor.Benchmarks
{
    internal static class Program
    {
        private static void Main() => BenchmarkRunner.Run<JpegBenchmark>();
    }

    /*
    Host Process Environment Information:
    BenchmarkDotNet.Core=v0.9.9.0
    OS=Microsoft Windows NT 6.2.9200.0
    Processor=Intel(R) Core(TM) i7-6700HQ CPU 2.60GHz, ProcessorCount=8
    Frequency=2531252 ticks, Resolution=395.0614 ns, Timer=TSC
    CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
    GC=Concurrent Workstation
    JitModules=clrjit-v4.6.1586.0

    Type=JpegBenchmark  Mode=Throughput

                                        Method |        Median |     StdDev | Scaled | Scaled-SD |
    ------------------------------------------ |-------------- |----------- |------- |---------- |
                  ImageMetadataReaderBenchmark |    64.7841 us |  5.9613 us |   1.00 |      0.00 |
                   JpegMetadataReaderBenchmark |    61.4718 us |  3.6344 us |   0.96 |      0.10 |
     JpegMetadataReaderOnlyExifReaderBenchmark |    37.1950 us |  2.1209 us |   0.59 |      0.06 |
                        SystemDrawingBenchmark |   471.9776 us | 30.1085 us |   7.43 |      0.76 |
                 WpfBitmapFrameCreateBenchmark |   822.8064 us | 36.6499 us |  12.92 |      1.19 |
                 WpfJpegBitmapDecoderBenchmark | 1,097.6627 us | 26.6706 us |  17.05 |      1.44 |



    Host Process Environment Information:
    BenchmarkDotNet.Core=v0.9.9.0
    OS=Microsoft Windows NT 6.2.9200.0
    Processor=Intel(R) Core(TM) i7-6700HQ CPU 2.60GHz, ProcessorCount=8
    Frequency=2531252 ticks, Resolution=395.0614 ns, Timer=TSC
    CLR=MS.NET 4.0.30319.42000, Arch=64-bit RELEASE [RyuJIT]
    GC=Concurrent Workstation
    JitModules=clrjit-v4.6.1586.0

    Type=JpegBenchmark  Mode=Throughput

                                        Method |      Median |     StdDev | Scaled | Scaled-SD |
    ------------------------------------------ |------------ |----------- |------- |---------- |
                  ImageMetadataReaderBenchmark |  66.3789 us |  2.2331 us |   1.00 |      0.00 |
                   JpegMetadataReaderBenchmark |  65.0763 us |  4.1237 us |   0.97 |      0.07 |
     JpegMetadataReaderOnlyExifReaderBenchmark |  33.0017 us |  1.0802 us |   0.50 |      0.02 |
                        SystemDrawingBenchmark | 325.8014 us |  8.3526 us |   4.94 |      0.20 |
                 WpfBitmapFrameCreateBenchmark | 690.9871 us | 18.9663 us |  10.46 |      0.44 |
                 WpfJpegBitmapDecoderBenchmark | 941.8040 us | 22.8158 us |  14.25 |      0.57 |

    */

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class JpegBenchmark
    {
        private readonly MemoryStream _stream;

        public JpegBenchmark()
        {
            _stream = new MemoryStream();

            // This is the largest JPEG file in this repository
            using (var fs = File.OpenRead("../MetadataExtractor.Tests/Data/nikonMakernoteType2b.jpg"))
                fs.CopyTo(_stream);
        }

        [Benchmark(Baseline = true)]
        public DateTime JpegMetadataReaderOnlyExifReaderBenchmark()
        {
            _stream.Position = 0;

            var directories = JpegMetadataReader.ReadMetadata(_stream, new[] { new ExifReader() });
            var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().First();
            return subIfdDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
        }

        [Benchmark]
        public DateTime JpegMetadataReaderBenchmark()
        {
            _stream.Position = 0;

            var directories = JpegMetadataReader.ReadMetadata(_stream);
            var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().First();
            return subIfdDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
        }

        [Benchmark]
        public DateTime ImageMetadataReaderBenchmark()
        {
            _stream.Position = 0;

            var directories = ImageMetadataReader.ReadMetadata(_stream);
            var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().First();
            return subIfdDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
        }

        private readonly Regex _dateTimeRegex = new Regex(":");

        [Benchmark]
        public DateTime SystemDrawingBenchmark()
        {
            _stream.Position = 0;

            // Based on code from http://stackoverflow.com/a/7713780/24874

            using (var myImage = Image.FromStream(_stream, useEmbeddedColorManagement: false, validateImageData: false))
            {
                const int PropertyTagExifDTOrig = ExifDirectoryBase.TagDateTimeOriginal;
                var propItem = myImage.GetPropertyItem(PropertyTagExifDTOrig);
                var dateTakenStr = Encoding.UTF8.GetString(propItem.Value);
                return DateTime.Parse(_dateTimeRegex.Replace(dateTakenStr, "-", count: 2));
            }
        }

        [Benchmark]
        public DateTime WpfBitmapFrameCreateBenchmark()
        {
            _stream.Position = 0;

            var frame = BitmapFrame.Create(_stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            var metadata = (BitmapMetadata)frame.Metadata;
            var dateTakenStr = (string)metadata.GetQuery("/app1/ifd/exif/subifd:{uint=36867}");
            return DateTime.Parse(_dateTimeRegex.Replace(dateTakenStr, "-", count: 2));
        }

        [Benchmark]
        public DateTime WpfJpegBitmapDecoderBenchmark()
        {
            _stream.Position = 0;

            var decoder = new JpegBitmapDecoder(_stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            var metadata = (BitmapMetadata)decoder.Frames[0].Metadata;
            var dateTakenStr = (string)metadata.GetQuery("/app1/ifd/exif/subifd:{uint=36867}");
            return DateTime.Parse(_dateTimeRegex.Replace(dateTakenStr, "-", count: 2));
        }
    }
}