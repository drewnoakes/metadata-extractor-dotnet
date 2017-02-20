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
using JetBrains.Annotations;
using System.IO;

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Defines an object that extracts metadata from in JPEG segments.</summary>
    public interface IJpegSegmentMetadataReader
    {
        /// <summary>Gets the set of JPEG segment types that this reader is interested in.</summary>
        [NotNull]
        ICollection<JpegSegmentType> SegmentTypes { get; }

        /// <summary>Extracts metadata from all instances of a particular JPEG segment type.</summary>
        /// <param name="stream"></param>
        /// <param name="segments">
        /// A sequence of JPEG segments from which the metadata should be extracted. These are in the order encountered in the original file.
        /// </param>
        [NotNull]
#if NET35
        IList<Directory>
#else
        IReadOnlyList<Directory>
#endif
            ReadJpegSegments(Stream stream, [NotNull] IEnumerable<JpegSegment> segments);
    }
}
