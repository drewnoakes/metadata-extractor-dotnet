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
using JetBrains.Annotations;
using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Riff.riff;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.WebP.webp
{
    /// <summary>Obtains metadata from WebP files.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class WebPMetadataReader
    {
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="RiffProcessingException"/>
        [NotNull]
        public static Metadata ReadMetadata([NotNull] string filePath)
        {
            Metadata metadata;
            using (Stream stream = new FileStream(filePath, FileMode.Open))
                metadata = ReadMetadata(stream);
            new FileMetadataReader().Read(filePath, metadata);
            return metadata;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="RiffProcessingException"/>
        [NotNull]
        public static Metadata ReadMetadata([NotNull] Stream stream)
        {
            var metadata = new Metadata();
            new RiffReader().ProcessRiff(new SequentialStreamReader(stream), new WebpRiffHandler(metadata));
            return metadata;
        }
    }
}
