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
using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Raf
{
    /// <summary>Obtains metadata from RAF (Fujifilm camera raw) image files.</summary>
    /// <author>TSGames https://github.com/TSGames</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class RafMetadataReader
    {
        [NotNull]
        public static DirectoryList ReadMetadata([NotNull] Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Must support seek", nameof(stream));

            var data = new byte[512];
            var bytesRead = stream.Read(data, 0, 512);

            if (bytesRead == 0)
                throw new IOException("Stream is empty");

            stream.Seek(-bytesRead, SeekOrigin.Current);

            for (var i = 0; i < bytesRead - 2; i++)
            {
                // Look for the first three bytes of a JPEG encoded file
                if (data[i] == 0xff && data[i + 1] == 0xd8 && data[i + 2] == 0xff)
                {
                    stream.Seek(i, SeekOrigin.Current);
                    break;
                }
            }

            return JpegMetadataReader.ReadMetadata(stream);
        }
    }
}