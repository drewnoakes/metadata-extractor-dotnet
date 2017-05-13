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
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Jfxx
{
    /// <summary>Reads JFXX (JFIF Extensions) data.</summary>
    /// <remarks>
    /// JFXX is found in JPEG APP0 segments.
    /// <list type="bullet">
    ///   <item>http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format</item>
    ///   <item>http://www.w3.org/Graphics/JPEG/jfif3.pdf</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes</author>
    public sealed class JfxxReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "JFXX";

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.App0 };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Skip segments not starting with the required header
            return segments
                .Where(segment => segment.Bytes.Length >= JpegSegmentPreamble.Length && JpegSegmentPreamble == Encoding.UTF8.GetString(segment.Bytes, 0, JpegSegmentPreamble.Length))
                .Select(segment => Extract(new ByteArrayReader(segment.Bytes)))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }

        /// <summary>Reads JFXX values and returns them in an <see cref="JfxxDirectory"/>.</summary>
        [NotNull]
        public JfxxDirectory Extract([NotNull] IndexedReader reader)
        {
            var directory = new JfxxDirectory();

            try
            {
                // For JFIF the tag number is the value's byte offset
                directory.Set(JfxxDirectory.TagExtensionCode, reader.GetByte(JfxxDirectory.TagExtensionCode));
            }
            catch (IOException e)
            {
                directory.AddError(e.Message);
            }

            return directory;
        }
    }
}
