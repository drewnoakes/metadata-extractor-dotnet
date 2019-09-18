// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MetadataExtractor.Formats.Riff;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Avi
{
    /// <summary>
    /// Implementation of <see cref="IRiffHandler"/> specialising in AVI support.
    /// </summary>
    /// <remarks>
    /// Extracts data from chunk/list types:
    /// <list type="bullet">
    ///   <item><c>"avih"</c>: width, height, streams</item>
    ///   <item><c>"strh"</c>: frames/second, samples/second, duration, video codec</item>
    /// </list>
    /// Sources:
    /// http://www.alexander-noe.com/video/documentation/avi.pdf
    /// https://msdn.microsoft.com/en-us/library/ms899422.aspx
    /// https://www.loc.gov/preservation/digital/formats/fdd/fdd000025.shtml
    /// </remarks>
    /// <author>Payton Garland</author>
    public sealed class AviRiffHandler : IRiffHandler
    {
        private readonly List<Directory> _directories;

        public AviRiffHandler(List<Directory> directories)
        {
            _directories = directories;
        }

        public bool ShouldAcceptRiffIdentifier(string identifier) => identifier == "AVI ";

        public bool ShouldAcceptChunk(string fourCc) => fourCc == "strh" ||
                                                        fourCc == "avih";

        public bool ShouldAcceptList(string fourCc) => fourCc == "hdrl" ||
                                                       fourCc == "strl" ||
                                                       fourCc == "AVI ";

        public void ProcessChunk(string fourCc, byte[] payload)
        {
            switch (fourCc)
            {
                case "strh":
                {
                    string? error = null;
                    var reader = new ByteArrayReader(payload, isMotorolaByteOrder: false);
                    string? fccType = null;
                    string? fccHandler = null;
                    float dwScale = 0;
                    float dwRate = 0;
                    int dwLength = 0;
                    try
                    {
                        fccType = reader.GetString(0, 4, Encoding.ASCII);
                        fccHandler = reader.GetString(4, 4, Encoding.ASCII);
                        //int dwFlags = reader.GetInt32(8);
                        //int wPriority = reader.GetInt16(12);
                        //int wLanguage = reader.GetInt16(14);
                        //int dwInitialFrames = reader.GetInt32(16);
                        dwScale = reader.GetFloat32(20);
                        dwRate = reader.GetFloat32(24);
                        //int dwStart = reader.GetInt32(28);
                        dwLength = reader.GetInt32(32);
                        //int dwSuggestedBufferSize = reader.GetInt32(36);
                        //int dwQuality = reader.GetInt32(40);
                        //int dwSampleSize = reader.GetInt32(44);
                        //byte[] rcFrame = reader.GetBytes(48, 2);
                    }
                    catch (IOException e)
                    {
                        error = "Exception reading AviRiff chunk 'strh' : " + e.Message;
                    }

                    var directory = new AviDirectory();
                    if (error == null)
                    {
                        if (fccType == "vids")
                        {
                            directory.Set(AviDirectory.TAG_FRAMES_PER_SECOND, (dwRate / dwScale));

                            double duration = dwLength / (dwRate / dwScale);
                            int hours = (int)duration / (int)(Math.Pow(60, 2));
                            int minutes = ((int)duration / (int)(Math.Pow(60, 1))) - (hours * 60);
                            int seconds = (int)Math.Round((duration / (Math.Pow(60, 0))) - (minutes * 60));
                            string time = new DateTime(new TimeSpan(hours, minutes, seconds).Ticks).ToString("HH:mm:ss");

                            directory.Set(AviDirectory.TAG_DURATION, time);
                            directory.Set(AviDirectory.TAG_VIDEO_CODEC, fccHandler!);
                        }
                        else
                        if (fccType == "auds")
                        {
                            directory.Set(AviDirectory.TAG_SAMPLES_PER_SECOND, (dwRate / dwScale));
                        }
                    }
                    else
                        directory.AddError(error);
                    _directories.Add(directory);
                    break;
                }
                case "avih":
                {
                    string? error = null;
                    var reader = new ByteArrayReader(payload, isMotorolaByteOrder: false);
                    int dwStreams = 0;
                    int dwWidth = 0;
                    int dwHeight = 0;
                    try
                    {
                        //int dwMicroSecPerFrame = reader.GetInt32(0);
                        //int dwMaxBytesPerSec = reader.GetInt32(4);
                        //int dwPaddingGranularity = reader.GetInt32(8);
                        //int dwFlags = reader.GetInt32(12);
                        //int dwTotalFrames = reader.GetInt32(16);
                        //int dwInitialFrames = reader.GetInt32(20);
                        dwStreams = reader.GetInt32(24);
                        //int dwSuggestedBufferSize = reader.GetInt32(28);
                        dwWidth = reader.GetInt32(32);
                        dwHeight = reader.GetInt32(36);
                        //byte[] dwReserved = reader.GetBytes(40, 4);
                    }
                    catch (IOException e)
                    {
                        error = "Exception reading AviRiff chunk 'avih' : " + e.Message;
                    }

                    var directory = new AviDirectory();
                    if (error == null)
                    {
                        directory.Set(AviDirectory.TAG_WIDTH, dwWidth);
                        directory.Set(AviDirectory.TAG_HEIGHT, dwHeight);
                        directory.Set(AviDirectory.TAG_STREAMS, dwStreams);
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
