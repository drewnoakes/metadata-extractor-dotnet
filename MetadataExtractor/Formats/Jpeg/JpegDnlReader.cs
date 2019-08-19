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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.IO;
#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Decodes JPEG DNL data, adjusting the image height with information missing from the JPEG SOFx segment.</summary>
    /// <seealso cref="JpegSegment"/>
    /// <author>Nadahar</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public sealed class JpegDnlReader : IJpegSegmentMetadataReader
    {
        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new[] { JpegSegmentType.Dnl };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            return new List<Directory>();
        }

        public void ReadJpegSegments(IEnumerable<JpegSegment> segments, List<Directory> directories)
        {
            foreach (var segment in segments)
            {
                Extract(segment, directories);
            }
        }
        
        public void Extract([NotNull] JpegSegment segment, List<Directory> directories)
        {
            var directory = directories.OfType<JpegDirectory>().FirstOrDefault();
            if (directory == null)
            {
                directories.Add(new ErrorDirectory("DNL segment found without SOFx - illegal JPEG format"));
                return;
            }

            var reader = new SequentialByteArrayReader(segment.Bytes);

            try
            {
                // Only set height from DNL if it's not already defined
                if(!directory.TryGetInt32(JpegDirectory.TagImageHeight, out int i) || i == 0)
                {
                    directory.Set(JpegDirectory.TagImageHeight, reader.GetUInt16());
                }
            }
            catch (IOException me)
            {
                directory.AddError(me.ToString());
            }
        }
        

    }
}
