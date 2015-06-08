/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System.IO;
using Com.Drew.Imaging.Jpeg;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Adobe
{
    /// <summary>Decodes Adobe formatted data stored in JPEG files, normally in the APPE (App14) segment.</summary>
    /// <author>Philip</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class AdobeJpegReader : JpegSegmentMetadataReader
    {
        public const string Preamble = "Adobe";

        [NotNull]
        public virtual Iterable<JpegSegmentType> GetSegmentTypes()
        {
            return Arrays.AsList(JpegSegmentType.Appe).AsIterable();
        }

        public virtual void ReadJpegSegments([NotNull] Iterable<sbyte[]> segments, [NotNull] Metadata metadata, [NotNull] JpegSegmentType segmentType)
        {
            foreach (sbyte[] bytes in segments)
            {
                if (bytes.Length == 12 && Runtime.EqualsIgnoreCase(Preamble, Runtime.GetStringForBytes(bytes, 0, Preamble.Length)))
                {
                    Extract(new SequentialByteArrayReader(bytes), metadata);
                }
            }
        }

        public virtual void Extract([NotNull] SequentialReader reader, [NotNull] Metadata metadata)
        {
            Directory directory = new AdobeJpegDirectory();
            metadata.AddDirectory(directory);
            try
            {
                reader.SetMotorolaByteOrder(false);
                if (!reader.GetString(Preamble.Length).Equals(Preamble))
                {
                    directory.AddError("Invalid Adobe JPEG data header.");
                    return;
                }
                directory.SetInt(AdobeJpegDirectory.TagDctEncodeVersion, reader.GetUInt16());
                directory.SetInt(AdobeJpegDirectory.TagApp14Flags0, reader.GetUInt16());
                directory.SetInt(AdobeJpegDirectory.TagApp14Flags1, reader.GetUInt16());
                directory.SetInt(AdobeJpegDirectory.TagColorTransform, reader.GetInt8());
            }
            catch (IOException ex)
            {
                directory.AddError("IO exception processing data: " + ex.Message);
            }
        }
    }
}
