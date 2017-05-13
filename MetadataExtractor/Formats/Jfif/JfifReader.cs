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

namespace MetadataExtractor.Formats.Jfif
{
    /// <summary>Reads JFIF (JPEG File Interchange Format) data.</summary>
    /// <remarks>
    /// JFIF is found in JPEG APP0 segments.
    /// <list type="bullet">
    ///   <item>http://en.wikipedia.org/wiki/JPEG_File_Interchange_Format</item>
    ///   <item>http://www.w3.org/Graphics/JPEG/jfif3.pdf</item>
    /// </list>
    /// </remarks>
    /// <author>Yuri Binev, Drew Noakes, Markus Meyer</author>
    public sealed class JfifReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "JFIF";

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

        /// <summary>Reads JFIF values and returns them in an <see cref="JfifDirectory"/>.</summary>
        [NotNull]
        public JfifDirectory Extract([NotNull] IndexedReader reader)
        {
            var directory = new JfifDirectory();

            try
            {
                // For JFIF the tag number is the value's byte offset
                directory.Set(JfifDirectory.TagVersion,     reader.GetUInt16(JfifDirectory.TagVersion));
                directory.Set(JfifDirectory.TagUnits,       reader.GetByte(JfifDirectory.TagUnits));
                directory.Set(JfifDirectory.TagResX,        reader.GetUInt16(JfifDirectory.TagResX));
                directory.Set(JfifDirectory.TagResY,        reader.GetUInt16(JfifDirectory.TagResY));
                directory.Set(JfifDirectory.TagThumbWidth,  reader.GetByte(JfifDirectory.TagThumbWidth));
                directory.Set(JfifDirectory.TagThumbHeight, reader.GetByte(JfifDirectory.TagThumbHeight));
            }
            catch (IOException e)
            {
                directory.AddError(e.Message);
            }

            return directory;
        }
    }
}
