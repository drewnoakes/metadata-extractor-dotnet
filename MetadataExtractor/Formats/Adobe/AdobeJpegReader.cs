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

namespace MetadataExtractor.Formats.Adobe
{
    /// <summary>Decodes Adobe formatted data stored in JPEG files, normally in the APPE (App14) segment.</summary>
    /// <author>Philip</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class AdobeJpegReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "Adobe";

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.AppE };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            return segments
                .Where(segment => segment.Bytes.Length == 12 && JpegSegmentPreamble.Equals(Encoding.UTF8.GetString(segment.Bytes, 0, JpegSegmentPreamble.Length), StringComparison.OrdinalIgnoreCase))
                .Select(bytes => Extract(new SequentialByteArrayReader(bytes.Bytes)))
#if NET35
                .Cast<Directory>()
#endif
                .ToList();
        }

        [NotNull]
        public AdobeJpegDirectory Extract([NotNull] SequentialReader reader)
        {
            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

            var directory = new AdobeJpegDirectory();

            try
            {
                if (reader.GetString(JpegSegmentPreamble.Length, Encoding.UTF8) != JpegSegmentPreamble)
                {
                    directory.AddError("Invalid Adobe JPEG data header.");
                    return directory;
                }

                directory.Set(AdobeJpegDirectory.TagDctEncodeVersion, reader.GetUInt16());
                directory.Set(AdobeJpegDirectory.TagApp14Flags0, reader.GetUInt16());
                directory.Set(AdobeJpegDirectory.TagApp14Flags1, reader.GetUInt16());
                directory.Set(AdobeJpegDirectory.TagColorTransform, reader.GetSByte());
            }
            catch (IOException ex)
            {
                directory.AddError("IO exception processing data: " + ex.Message);
            }

            return directory;
        }
    }
}
