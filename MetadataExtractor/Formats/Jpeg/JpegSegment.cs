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

using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>
    /// Holds information about a JPEG segment.
    /// </summary>
    /// <seealso cref="JpegSegmentReader"/>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegSegment
    {
        public JpegSegmentType Type { get; }
        public int Length { get; }
        public int Padding { get; set; }
        public long Offset { get; }
        public string Preamble { get; }

        public JpegSegment(JpegSegmentType type, [NotNull] int length, int padding, long offset, string preamble)
        {
            Type = type;
            Length = length;
            Padding = padding;
            Offset = offset;
            Preamble = preamble;
        }

        private IndexedReader _byteReader = null;
        public IndexedReader ByteReader(Stream stream)
        {
            if (_byteReader == null)
                _byteReader = new IndexedSeekingReader(stream, (int)Offset);
            return _byteReader;
        }
        /*public byte[] Bytes(Stream stream)
        {
            return new IndexedSeekingReader(stream, (int)Offset).GetBytes(0, Length);
        }*/
    }
}
