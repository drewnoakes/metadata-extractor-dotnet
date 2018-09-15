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
using MetadataExtractor.Util;
using MetadataExtractor.Formats.Jpeg;
using XmpCore;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Xmp
{
    /// <summary>Extracts XMP data JPEG APP1 segments.</summary>
    /// <remarks>
    /// XMP uses a namespace and path format for identifying values, which does not map to metadata-extractor's
    /// integer based tag identifiers. Therefore, XMP data is extracted and exposed via <see cref="XmpDirectory.XmpMeta"/>
    /// which returns an instance of Adobe's <see cref="IXmpMeta"/> which exposes the full XMP data set.
    /// <para />
    /// The extraction is done with Adobe's XmpCore-Library (XMP-Toolkit)
    /// Copyright (c) 1999 - 2007, Adobe Systems Incorporated All rights reserved.
    /// </remarks>
    /// <author>Torsten Skadell</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpReader : IJpegSegmentMetadataReader
    {
        public const string JpegSegmentPreamble = "http://ns.adobe.com/xap/1.0/\0";
        public const string JpegSegmentPreambleExtension = "http://ns.adobe.com/xmp/extension/\0";

        private static byte[] JpegSegmentPreambleBytes { get; } = Encoding.UTF8.GetBytes(JpegSegmentPreamble);
        private static byte[] JpegSegmentPreambleExtensionBytes { get; } = Encoding.UTF8.GetBytes(JpegSegmentPreambleExtension);

        ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes => new [] { JpegSegmentType.App1 };

        public DirectoryList ReadJpegSegments(IEnumerable<JpegSegment> segments)
        {
            // Ensure collection materialised (avoiding multiple lazy enumeration)
            segments = segments.ToList();

            var directories = segments
                .Where(IsXmpSegment)
                .Select(segment => Extract(segment.Bytes, JpegSegmentPreambleBytes.Length, segment.Bytes.Length - JpegSegmentPreambleBytes.Length))
                .Cast<Directory>()
                .ToList();

            var extensionGroups = segments.Where(IsExtendedXmpSegment).GroupBy(GetExtendedDataGuid);

            foreach (var extensionGroup in extensionGroups)
            {
                var buffer = new MemoryStream();
                foreach (var segment in extensionGroup)
                {
                    // TODO: re-evaluate this method, and maybe use a pre-parse strategy instead
                    // Sometimes the byte arrays contain nul (0x00) characters. These can be legal if it's UTF-16 encoded,
                    // but should expect UTF-8. Otherwise, these characters are illegal in XML and C#'s XmlReader will not 
                    // handle or ignore them.
                    // Replace nul (0x00) bytes with space character (0x20)
                    /*for (int i = 0; i < segment.Bytes.Length; i++)
                    {
                        if (!IsLegalXmlChar(segment.Bytes[i]))
                            segment.Bytes[i] = 0x20;
                    }*/

                    var N = JpegSegmentPreambleExtensionBytes.Length + 32 + 4 + 4;
                    buffer.Write(segment.Bytes, N, segment.Bytes.Length - N);
                }

                var directory = new XmpDirectory();
                directory.Set(XmpDirectory.TagXmpByteArray, buffer.ToArray());
                directories.Add(directory);
            }

            return directories;
        }

        private static string GetExtendedDataGuid(JpegSegment segment) => Encoding.UTF8.GetString(segment.Bytes, JpegSegmentPreambleExtensionBytes.Length, 32);

        private static bool IsXmpSegment(JpegSegment segment) => segment.Bytes.StartsWith(JpegSegmentPreambleBytes);
        private static bool IsExtendedXmpSegment(JpegSegment segment) => segment.Bytes.StartsWith(JpegSegmentPreambleExtensionBytes);

        [NotNull]
        public XmpDirectory Extract([NotNull] byte[] xmpBytes) => Extract(xmpBytes, 0, xmpBytes.Length);

        [NotNull]
        public XmpDirectory Extract([NotNull] byte[] xmpBytes, int offset, int length)
        {
            var directory = new XmpDirectory();
            try
            {
                // TODO: re-evaluate this method, and maybe use a pre-parse strategy instead
                // Sometimes the byte arrays contain nul (0x00) characters. These can be legal if it's UTF-16 encoded,
                // but should expect UTF-8. Otherwise, these characters are illegal in XML and C#'s XmlReader will not 
                // handle or ignore them.
                // Replace nul (0x00) bytes with space character (0x20)
                /*for (int i = 0; i < xmpBytes.Length; i++)
                {
                    if (!IsLegalXmlChar(xmpBytes[i]))
                        xmpBytes[i] = 0x20;
                }*/

                if (offset == 0 && length == xmpBytes.Length)
                    directory.Set(XmpDirectory.TagXmpByteArray, xmpBytes);
                else
                {
                    var destArray = new byte[length];
                    Array.Copy(xmpBytes, offset, destArray, 0, length);
                    directory.Set(XmpDirectory.TagXmpByteArray, destArray);
                }
            }
            catch (XmpException e)
            {
                directory.AddError("Error processing XMP data: " + e.Message);
            }
            return directory;
        }

        /// <summary>
        /// Borrowed from:  https://seattlesoftware.wordpress.com/2008/09/11/hexadecimal-value-0-is-an-invalid-character/
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool IsLegalXmlChar(int character)
        {
            return
            (
                character != 0x00

                // TODO: Might expand the list in the future if other bad characters appear
                // character == 0x9 /* == '\t' == 9   */          ||
                // character == 0xA /* == '\n' == 10  */          ||
                // character == 0xD /* == '\r' == 13  */          ||
                //(character >= 0x20 && character <= 0xD7FF)      ||
                //(character >= 0xE000 && character <= 0xFFFD)    ||
                //(character >= 0x10000 && character <= 0x10FFFF)
            );
        }
    }
}
