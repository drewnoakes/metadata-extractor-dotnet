#region License
//
// Copyright 2002-2015 Drew Noakes
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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Reads JPEG comments.</summary>
    /// <remarks>JPEG files can store zero or more comments in COM segments.</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegCommentReader : IJpegSegmentMetadataReader
    {
        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.Com;
        }

        /// <summary>Reads JPEG comments, returning each in a <see cref="JpegCommentDirectory"/>.</summary>
        public IReadOnlyList<Directory> ReadJpegSegments(IEnumerable<byte[]> segments, JpegSegmentType segmentType)
        {
            Debug.Assert(segmentType == JpegSegmentType.Com);

            // TODO store bytes in the directory to allow different encodings when decoding

            // The entire contents of the segment are the comment
            return segments.Select(segment => new JpegCommentDirectory(Encoding.UTF8.GetString(segment))).ToList();
        }
    }
}
