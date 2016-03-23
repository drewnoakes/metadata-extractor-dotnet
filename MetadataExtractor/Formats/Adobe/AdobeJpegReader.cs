#region License
//
// Copyright 2002-2016 Drew Noakes
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
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Adobe
{
    /// <summary>Decodes Adobe formatted data stored in JPEG files, normally in the APPE (App14) segment.</summary>
    /// <author>Philip</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class AdobeJpegReader : IJpegSegmentMetadataReader
    {
        private const string Preamble = "Adobe";

        public IEnumerable<JpegSegmentType> GetSegmentTypes()
        {
            yield return JpegSegmentType.AppE;
        }

        public
#if NET35 || PORTABLE
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            ReadJpegSegments(IEnumerable<byte[]> segments, JpegSegmentType segmentType)
        {
            return segments
                .Where(segment => segment.Length == 12 && Preamble.Equals(Encoding.UTF8.GetString(segment, 0, Preamble.Length), StringComparison.OrdinalIgnoreCase))
                .Select(bytes => Extract(new SequentialByteArrayReader(bytes)))
#if NET35 || PORTABLE
                .Cast<Directory>()
#endif
                .ToList();
        }

        public AdobeJpegDirectory Extract(SequentialReader reader)
        {
            reader.IsMotorolaByteOrder = false;

            var directory = new AdobeJpegDirectory();

            try
            {
                if (reader.GetString(Preamble.Length) != Preamble)
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
