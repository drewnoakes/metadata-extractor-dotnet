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

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>Reads Photoshop "ducky" segments, created during Save-for-Web.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class DuckyReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "Ducky";

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.AppC };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Skip segments not starting with the required header
            return segments
                .Where(segment => segment.Bytes.Length >= JpegSegmentPreamble.Length && JpegSegmentPreamble == Encoding.UTF8.GetString(segment.Bytes, 0, JpegSegmentPreamble.Length))
                .Select(segment => Extract(new SequentialByteArrayReader(segment.Bytes, JpegSegmentPreamble.Length)))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }

        [NotNull]
        public DuckyDirectory Extract([NotNull] SequentialReader reader)
        {
            var directory = new DuckyDirectory();

            try
            {
                while (true)
                {
                    var tag = reader.GetUInt16();

                    // End of Segment is marked with zero
                    if (tag == 0)
                        break;

                    var length = reader.GetUInt16();

                    switch (tag)
                    {
                        case DuckyDirectory.TagQuality:
                        {
                            if (length != 4)
                            {
                                directory.AddError("Unexpected length for the quality tag");
                                return directory;
                            }
                            directory.Set(tag, reader.GetUInt32());
                            break;
                        }
                        case DuckyDirectory.TagComment:
                        case DuckyDirectory.TagCopyright:
                        {
                            reader.Skip(4);
                            directory.Set(tag, reader.GetString(length - 4, Encoding.BigEndianUnicode));
                            break;
                        }
                        default:
                        {
                            // Unexpected tag
                            directory.Set(tag, reader.GetBytes(length));
                            break;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                directory.AddError(e.Message);
            }

            return directory;
        }
    }
}
