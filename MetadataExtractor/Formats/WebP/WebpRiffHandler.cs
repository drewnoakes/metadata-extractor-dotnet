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

using System;
using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Riff;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.WebP
{
    /// <summary>
    /// Implementation of
    /// <see cref="IRiffHandler"/>
    /// specialising in WebP support.
    /// Extracts data from chunk types:
    /// <list type="bullet">
    /// <item><c>"VP8X"</c>: width, height, is animation, has alpha</item>
    /// <item><c>"EXIF"</c>: full Exif data</item>
    /// <item><c>"ICCP"</c>: full ICC profile</item>
    /// <item><c>"XMP "</c>: full XMP data</item>
    /// </list>
    /// </summary>
    public sealed class WebPRiffHandler : IRiffHandler
    {
        [NotNull]
        private readonly Metadata _metadata;

        public WebPRiffHandler([NotNull] Metadata metadata)
        {
            _metadata = metadata;
        }

        public bool ShouldAcceptRiffIdentifier(string identifier)
        {
            return identifier.Equals("WEBP");
        }

        public bool ShouldAcceptChunk(string fourCc)
        {
            return
                fourCc.Equals("VP8X") ||
                fourCc.Equals("EXIF") ||
                fourCc.Equals("ICCP") ||
                fourCc.Equals("XMP ");
        }

        public void ProcessChunk(string fourCc, byte[] payload)
        {
            if (fourCc.Equals("EXIF"))
            {
                new ExifReader().Extract(new ByteArrayReader(payload), _metadata);
            }
            else if (fourCc.Equals("ICCP"))
            {
                new IccReader().Extract(new ByteArrayReader(payload), _metadata);
            }
            else if (fourCc.Equals("XMP "))
            {
                new XmpReader().Extract(payload, _metadata);
            }
            else if (fourCc.Equals("VP8X") && payload.Length == 10)
            {
                IndexedReader reader = new ByteArrayReader(payload);
                reader.IsMotorolaByteOrder = false;
                try
                {
                    // Flags
                    //                boolean hasFragments = reader.getBit(0);
                    var isAnimation = reader.GetBit(1);
                    //                boolean hasXmp = reader.getBit(2);
                    //                boolean hasExif = reader.getBit(3);
                    var hasAlpha = reader.GetBit(4);
                    //                boolean hasIcc = reader.getBit(5);
                    // Image size
                    var widthMinusOne = reader.GetInt24(4);
                    var heightMinusOne = reader.GetInt24(7);
                    var directory = new WebPDirectory();
                    directory.SetInt(WebPDirectory.TagImageWidth, widthMinusOne + 1);
                    directory.SetInt(WebPDirectory.TagImageHeight, heightMinusOne + 1);
                    directory.SetBoolean(WebPDirectory.TagHasAlpha, hasAlpha);
                    directory.SetBoolean(WebPDirectory.TagIsAnimation, isAnimation);
                    _metadata.AddDirectory(directory);
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }
    }
}
