#region License
//
// Copyright 2002-2017 Drew Noakes
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

using System.Diagnostics;
using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using Xunit;
using Xunit.Abstractions;

namespace MetadataExtractor.Tests.Formats.Exif
{
    /// <summary>
    /// Long-running test of <see cref="ExifReader"/> that attempts to verify exceptions are not thrown for invalid input.
    /// </summary>
    /// <remarks>
    /// This test takes a valid APP1 segment and reads it in a loop. At each iteration of the loop, one
    /// byte from within the segment is modified to some different value. Extraction is performed. This may
    /// cause extraction to fail, but failure should be captured in the returned directories not as an
    /// exception.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifReaderGremlinTest
    {
        private readonly ITestOutputHelper _output;

        public ExifReaderGremlinTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "Don't run on CI machines as it takes an age to complete")]
        public void DoesntThrowNoMatterWhat()
        {
            RunGremlinTest("Data/withExif.jpg.app1");
        }

        private void RunGremlinTest(string filePath)
        {
            var sw = Stopwatch.StartNew();

            var app1 = File.ReadAllBytes(filePath);
            var segments = new[] { new JpegSegment(JpegSegmentType.App1, app1, 0) };

            Assert.Same(app1, segments[0].Bytes);

            var exifReader = new ExifReader();

            for (var i = 0; i < app1.Length; i++)
            {
                if (i % 1000 == 0)
                    _output.WriteLine($"{i}/{app1.Length} bytes");

                var original = app1[i];

                for (var b = byte.MinValue; b < byte.MaxValue; b++)
                {
                    app1[i] = b;

                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    exifReader.ReadJpegSegments(segments).ToList();
                }

                app1[i] = original;
            }

            _output.WriteLine($"Finished in {sw.Elapsed.TotalSeconds:#,##0.#} seconds");
        }
    }
}