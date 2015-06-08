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
using Com.Drew.Imaging.Riff;
using Com.Drew.Lang;
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.Icc;
using Com.Drew.Metadata.Xmp;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Webp
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
    public class WebpRiffHandler : IRiffHandler
    {
        [NotNull]
        private readonly Metadata _metadata;

        public WebpRiffHandler([NotNull] Metadata metadata)
        {
            _metadata = metadata;
        }

        public virtual bool ShouldAcceptRiffIdentifier([NotNull] string identifier)
        {
            return identifier.Equals("WEBP");
        }

        public virtual bool ShouldAcceptChunk([NotNull] string fourCc)
        {
            return fourCc.Equals("VP8X") || fourCc.Equals("EXIF") || fourCc.Equals("ICCP") || fourCc.Equals("XMP ");
        }

        public virtual void ProcessChunk([NotNull] string fourCc, [NotNull] sbyte[] payload)
        {
            //        System.out.println("Chunk " + fourCC + " " + payload.length + " bytes");
            if (fourCc.Equals("EXIF"))
            {
                new ExifReader().Extract(new ByteArrayReader(payload), _metadata);
            }
            else
            {
                if (fourCc.Equals("ICCP"))
                {
                    new IccReader().Extract(new ByteArrayReader(payload), _metadata);
                }
                else
                {
                    if (fourCc.Equals("XMP "))
                    {
                        new XmpReader().Extract(payload, _metadata);
                    }
                    else
                    {
                        if (fourCc.Equals("VP8X") && payload.Length == 10)
                        {
                            RandomAccessReader reader = new ByteArrayReader(payload);
                            reader.SetMotorolaByteOrder(false);
                            try
                            {
                                // Flags
                                //                boolean hasFragments = reader.getBit(0);
                                bool isAnimation = reader.GetBit(1);
                                //                boolean hasXmp = reader.getBit(2);
                                //                boolean hasExif = reader.getBit(3);
                                bool hasAlpha = reader.GetBit(4);
                                //                boolean hasIcc = reader.getBit(5);
                                // Image size
                                int widthMinusOne = reader.GetInt24(4);
                                int heightMinusOne = reader.GetInt24(7);
                                WebpDirectory directory = new WebpDirectory();
                                directory.SetInt(WebpDirectory.TagImageWidth, widthMinusOne + 1);
                                directory.SetInt(WebpDirectory.TagImageHeight, heightMinusOne + 1);
                                directory.SetBoolean(WebpDirectory.TagHasAlpha, hasAlpha);
                                directory.SetBoolean(WebpDirectory.TagIsAnimation, isAnimation);
                                _metadata.AddDirectory(directory);
                            }
                            catch (IOException e)
                            {
                                Runtime.PrintStackTrace(e, Console.Error);
                            }
                        }
                    }
                }
            }
        }
    }
}
