#region License
//
// Copyright 2002-2019 Drew Noakes
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

using JetBrains.Annotations;

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
        [NotNull] public byte[] Bytes { get; }
        public long Offset { get; }

        public JpegSegment(JpegSegmentType type, [NotNull] byte[] bytes, long offset)
        {
            Type = type;
            Bytes = bytes;
            Offset = offset;
        }
    }
}
