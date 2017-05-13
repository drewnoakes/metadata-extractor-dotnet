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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Management.Automation;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Jpeg;
using System.Linq;
using System.Text;
using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.Formats.Jfxx;
using MetadataExtractor.Formats.Photoshop;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.Util;

namespace MetadataExtractor.PowerShell
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public struct JpegSegment
    {
        public JpegSegmentType Type { get; }
        public int Length { get; }
        public int Padding { get; }
        public long Offset { get; }
        public string Preamble { get; }

        public JpegSegment(JpegSegmentType type, int length, int padding, long offset, string preamble)
        {
            Type = type;
            Length = length;
            Padding = padding;
            Offset = offset;
            Preamble = preamble;
        }
    }

    [Cmdlet(VerbsCommon.Show, "JpegStructure")]
    [UsedImplicitly]
    public sealed class ShowJpegStructure : PSCmdlet
    {
        private static readonly ByteTrie<string> _appSegmentByPreambleBytes = new ByteTrie<string>
        {
            { "Adobe",          Encoding.UTF8.GetBytes(AdobeJpegReader.JpegSegmentPreamble) },
            { "Ducky",          Encoding.UTF8.GetBytes(DuckyReader.JpegSegmentPreamble) },
            { "Exif",           Encoding.UTF8.GetBytes(ExifReader.JpegSegmentPreamble) },
            { "ICC",            Encoding.UTF8.GetBytes(IccReader.JpegSegmentPreamble) },
            { "JFIF",           Encoding.UTF8.GetBytes(JfifReader.JpegSegmentPreamble) },
            { "JFXX",           Encoding.UTF8.GetBytes(JfxxReader.JpegSegmentPreamble) },
            { "Photoshop",      Encoding.UTF8.GetBytes(PhotoshopReader.JpegSegmentPreamble) },
            { "XMP",            Encoding.UTF8.GetBytes(XmpReader.JpegSegmentPreamble) },
            { "XMP (Extended)", Encoding.UTF8.GetBytes(XmpReader.JpegSegmentPreambleExtension) }
        };

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Path to the file to process")]
        [ValidateNotNullOrEmpty]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public string FilePath { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            WriteVerbose($"Extracting metadata from file: {FilePath}");

            using (var stream = File.OpenRead(FilePath))
                WriteObject(ReadSegments(stream).ToList());
        }

        [NotNull]
        private static IEnumerable<JpegSegment> ReadSegments([NotNull] Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Must be able to seek.", nameof(stream));

            // first two bytes should be JPEG magic number
            var magicNumber = GetUInt16(stream);

            if (magicNumber != 0xFFD8)
                throw new JpegProcessingException($"JPEG data should begin with 0xFFD8, not 0x{magicNumber:X4}.");

            while (true)
            {
                var padding = 0;

                // Find the segment marker. Markers are zero or more 0xFF bytes, followed
                // by a 0xFF and then a byte not equal to 0x00 or 0xFF.
                var segmentIdentifier = stream.ReadByte();
                var segmentTypeByte = stream.ReadByte();

                if (segmentTypeByte == -1)
                    yield break;

                // Read until we have a 0xFF byte followed by a byte that is not 0xFF or 0x00
                while (segmentIdentifier != 0xFF || segmentTypeByte == 0xFF || segmentTypeByte == 0)
                {
                    padding++;
                    segmentIdentifier = segmentTypeByte;
                    segmentTypeByte = stream.ReadByte();

                    if (segmentTypeByte == -1)
                        yield break;
                }

                var segmentType = (JpegSegmentType)segmentTypeByte;
                var offset = stream.Position - 2;

                // if there is a payload, then segment length includes the two size bytes
                if (segmentType.ContainsPayload())
                {
                    var pos = stream.Position;

                    // Read the 2-byte big-endian segment length (excludes two marker bytes)
                    var b1 = stream.ReadByte();
                    var b2 = stream.ReadByte();
                    if (b2 == -1)
                        yield break;
                    var segmentLength = unchecked((ushort)(b1 << 8 | b2));

                    var preambleBytes = new byte[Math.Min(segmentLength, _appSegmentByPreambleBytes.MaxDepth)];
                    if (stream.Read(preambleBytes, 0, preambleBytes.Length) != preambleBytes.Length)
                        yield break;
                    var preamble = _appSegmentByPreambleBytes.Find(preambleBytes);

                    yield return new JpegSegment(segmentType, segmentLength, padding, offset, preamble);

                    // A length of less than two would be an error
                    if (segmentLength < 2)
                        yield break;

                    stream.Position = pos + segmentLength;
                }
                else
                {
                    yield return new JpegSegment(segmentType, 0, padding, offset, "");
                }
            }
        }

        private static ushort GetUInt16(Stream stream)
        {
            var b1 = stream.ReadByte();
            var b2 = stream.ReadByte();
            if (b2 == -1)
                throw new IOException("Unexpected end of stream.");
            return unchecked((ushort)(b1 << 8 | b2));
        }
    }
}