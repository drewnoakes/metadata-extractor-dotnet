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
using JetBrains.Annotations;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Riff;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.WebP
{
    /// <summary>
    /// Implementation of <see cref="IRiffHandler"/> specialising in WebP support.
    /// </summary>
    /// <remarks>
    /// Extracts data from chunk types:
    /// <list type="bullet">
    ///   <item><c>"VP8X"</c>: width, height, is animation, has alpha</item>
    ///   <item><c>"VP8L"</c>: width, height</item>
    ///   <item><c>"VP8 "</c>: width, height</item>
    ///   <item><c>"EXIF"</c>: full Exif data</item>
    ///   <item><c>"ICCP"</c>: full ICC profile</item>
    ///   <item><c>"XMP "</c>: full XMP data</item>
    /// </list>
    /// </remarks>
    public sealed class WebPRiffHandler : IRiffHandler
    {
        [NotNull]
        private readonly List<Directory> _directories;

        public WebPRiffHandler([NotNull] List<Directory> directories)
        {
            _directories = directories;
        }

        public bool ShouldAcceptRiffIdentifier(string identifier) => identifier == "WEBP";

        public bool ShouldAcceptChunk(string fourCc) => fourCc == "VP8X" ||
                                                        fourCc == "VP8L" ||
                                                        fourCc == "VP8 " ||
                                                        fourCc == "EXIF" ||
                                                        fourCc == "ICCP" ||
                                                        fourCc == "XMP ";

        public void ProcessChunk(string fourCc, byte[] payload)
        {
            switch (fourCc)
            {
                case "EXIF":
                {
                    _directories.AddRange(new ExifReader().Extract(new ByteArrayReader(payload)));
                    break;
                }
                case "ICCP":
                {
                    _directories.Add(new IccReader().Extract(new ByteArrayReader(payload)));
                    break;
                }
                case "XMP ":
                {
                    _directories.Add(new XmpReader().Extract(payload));
                    break;
                }
                case "VP8X":
                {
                    if (payload.Length != 10)
                        break;

                    string error = null;
                    var reader = new ByteArrayReader(payload, isMotorolaByteOrder: false);
                    var isAnimation = false;
                    var hasAlpha = false;
                    var widthMinusOne = -1;
                    var heightMinusOne = -1;
                    try
                    {
                        // Flags
//                      var hasFragments = reader.GetBit(0);
                        isAnimation = reader.GetBit(1);
//                      var hasXmp = reader.GetBit(2);
//                      var hasExif = reader.GetBit(3);
                        hasAlpha = reader.GetBit(4);
//                      var hasIcc = reader.GetBit(5);
                        // Image size
                        widthMinusOne = reader.GetInt24(4);
                        heightMinusOne = reader.GetInt24(7);
                    }
                    catch (IOException e)
                    {
                        error = "Exception reading WebpRiff chunk 'VP8X' : " + e.Message;
                    }

                    var directory = new WebPDirectory();
                    if (error == null)
                    {
                        directory.Set(WebPDirectory.TagImageWidth, widthMinusOne + 1);
                        directory.Set(WebPDirectory.TagImageHeight, heightMinusOne + 1);
                        directory.Set(WebPDirectory.TagHasAlpha, hasAlpha);
                        directory.Set(WebPDirectory.TagIsAnimation, isAnimation);
                    }
                    else
                        directory.AddError(error);
                    _directories.Add(directory);
                    break;
                }
                case "VP8L":
                {
                    if (payload.Length < 5)
                        break;

                    var reader = new ByteArrayReader(payload, isMotorolaByteOrder: false);

                    string error = null;
                    var widthMinusOne = -1;
                    var heightMinusOne = -1;
                    try
                    {
                        // https://developers.google.com/speed/webp/docs/webp_lossless_bitstream_specification#2_riff_header

                        // Expect the signature byte
                        if (reader.GetByte(0) != 0x2F)
                            break;
                        var b1 = reader.GetByte(1);
                        var b2 = reader.GetByte(2);
                        var b3 = reader.GetByte(3);
                        var b4 = reader.GetByte(4);
                        // 14 bits for width
                        widthMinusOne = (b2 & 0x3F) << 8 | b1;
                        // 14 bits for height
                        heightMinusOne = (b4 & 0x0F) << 10 | b3 << 2 | (b2 & 0xC0) >> 6;
                    }
                    catch (IOException e)
                    {
                        error = "Exception reading WebpRiff chunk 'VP8L' : " + e.Message;
                    }

                    var directory = new WebPDirectory();
                    if (error == null)
                    {
                        directory.Set(WebPDirectory.TagImageWidth, widthMinusOne + 1);
                        directory.Set(WebPDirectory.TagImageHeight, heightMinusOne + 1);
                    }
                    else
                        directory.AddError(error);
                    _directories.Add(directory);
                    break;
                }
                case "VP8 ":
                {
                    if (payload.Length < 10)
                        break;

                    var reader = new ByteArrayReader(payload, isMotorolaByteOrder: false);

                    string error = null;
                    var width = 0;
                    var height = 0;
                    try
                    {
                        // https://tools.ietf.org/html/rfc6386#section-9.1
                        // https://github.com/webmproject/libwebp/blob/master/src/enc/syntax.c#L115

                        // Expect the signature bytes
                        if (reader.GetByte(3) != 0x9D ||
                            reader.GetByte(4) != 0x01 ||
                            reader.GetByte(5) != 0x2A)
                            break;
                        width = reader.GetUInt16(6);
                        height = reader.GetUInt16(8);
                    }
                    catch (IOException e)
                    {
                        error = "Exception reading WebpRiff chunk 'VP8' : " + e.Message;
                    }

                    var directory = new WebPDirectory();
                    if (error == null)
                    {
                        directory.Set(WebPDirectory.TagImageWidth, width);
                        directory.Set(WebPDirectory.TagImageHeight, height);
                    }
                    else
                        directory.AddError(error);
                    _directories.Add(directory);
                    break;
                }
            }
        }
    }
}
