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
using System.Linq;
using System.Text;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Reads JPEG comments.</summary>
    /// <remarks>JPEG files can store zero or more comments in COM segments.</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class JpegCommentReader : IJpegSegmentMetadataReader
    {
        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.Com };

        /// <summary>Reads JPEG comments, returning each in a <see cref="JpegCommentDirectory"/>.</summary>
        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // The entire contents of the segment are the comment
            return segments.Select(segment => new JpegCommentDirectory(new StringValue(segment.Bytes, Encoding.UTF8)))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }
    }
}
