// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Riff;

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
    public sealed class AviRiffHandler(List<Directory> directories) : IRiffHandler
    {
        private readonly List<Directory> _directories = directories;
        private AviDirectory? _directory;

        public bool ShouldAcceptRiffIdentifier(ReadOnlySpan<byte> identifier) => identifier.SequenceEqual("AVI "u8);

        public bool ShouldAcceptChunk(string fourCc) => fourCc switch
        {
            "strh" => true,
            "avih" => true,
            "IDIT" => true,
            _ => false
        };

        public bool ShouldAcceptList(string fourCc) => fourCc switch
        {
            "hdrl" => true,
            "strl" => true,
            "AVI " => true,
            _ => false
        };

        public void ProcessChunk(string fourCc, byte[] payload)
        {
            switch (fourCc)
            {
                case "strh":
                {
                    var reader = new ByteArrayReader(payload, isMotorolaByteOrder: false);

                    var directory = GetOrCreateAviDirectory();
                    try
                    {
                        var fccType = reader.GetString(0, 4, Encoding.ASCII);
                        var fccHandler = reader.GetString(4, 4, Encoding.ASCII);
                        //int dwFlags = reader.GetInt32(8);
                        //int wPriority = reader.GetInt16(12);
                        //int wLanguage = reader.GetInt16(14);
                        //int dwInitialFrames = reader.GetInt32(16);
                        var dwScale = reader.GetFloat32(20);
                        var dwRate = reader.GetFloat32(24);
                        //int dwStart = reader.GetInt32(28);
                        var dwLength = reader.GetInt32(32);
                        //int dwSuggestedBufferSize = reader.GetInt32(36);
                        //int dwQuality = reader.GetInt32(40);
                        //int dwSampleSize = reader.GetInt32(44);
                        //byte[] rcFrame = reader.GetBytes(48, 2);

                        if (fccType == "vids")
                        {
                            directory.Set(AviDirectory.TagFramesPerSecond, (dwRate / dwScale));

                            double duration = dwLength / (dwRate / dwScale);
                            int hours = (int)duration / (int)(Math.Pow(60, 2));
                            int minutes = ((int)duration / (int)(Math.Pow(60, 1))) - (hours * 60);
                            int seconds = (int)Math.Round((duration / (Math.Pow(60, 0))) - (minutes * 60));
                            string time = new DateTime(new TimeSpan(hours, minutes, seconds).Ticks).ToString("HH:mm:ss");

                            directory.Set(AviDirectory.TagDuration, time);
                            directory.Set(AviDirectory.TagVideoCodec, fccHandler);
                        }
                        else if (fccType == "auds")
                        {
                            directory.Set(AviDirectory.TagSamplesPerSecond, (dwRate / dwScale));
                        }
                    }
                    catch (IOException e)
                    {
                        directory.AddError("Exception reading AviRiff chunk 'strh' : " + e.Message);
                    }

                    break;
                }
                case "avih":
                {
                    var directory = GetOrCreateAviDirectory();

                    var reader = new ByteArrayReader(payload, isMotorolaByteOrder: false);
                    try
                    {
                        //int dwMicroSecPerFrame = reader.GetInt32(0);
                        //int dwMaxBytesPerSec = reader.GetInt32(4);
                        //int dwPaddingGranularity = reader.GetInt32(8);
                        //int dwFlags = reader.GetInt32(12);
                        //int dwTotalFrames = reader.GetInt32(16);
                        //int dwInitialFrames = reader.GetInt32(20);
                        var dwStreams = reader.GetInt32(24);
                        //int dwSuggestedBufferSize = reader.GetInt32(28);
                        var dwWidth = reader.GetInt32(32);
                        var dwHeight = reader.GetInt32(36);
                        //byte[] dwReserved = reader.GetBytes(40, 4);

                        directory.Set(AviDirectory.TagWidth, dwWidth);
                        directory.Set(AviDirectory.TagHeight, dwHeight);
                        directory.Set(AviDirectory.TagStreams, dwStreams);
                    }
                    catch (IOException e)
                    {
                        directory.AddError("Exception reading AviRiff chunk 'avih' : " + e.Message);
                    }

                    break;
                }
                case "IDIT":
                {
                    var reader = new ByteArrayReader(payload);
                    var str = reader.GetString(0, payload.Length, Encoding.ASCII);
                    if (str.Length == 26 && str.EndsWith("\n\0", StringComparison.Ordinal))
                    {
                        // ?0A 00? "New Line" + padded to nearest WORD boundary
                        str = str.Substring(0, 24);
                    }
                    GetOrCreateAviDirectory().Set(AviDirectory.TagDateTimeOriginal, str);
                    break;
                }
            }
        }

        public void AddError(string errorMessage)
        {
            GetOrCreateAviDirectory().AddError(errorMessage);
        }

        private AviDirectory GetOrCreateAviDirectory()
        {
            if (_directory is null)
            {
                _directory = new AviDirectory();
                _directories.Add(_directory);
            }

            return _directory;
        }
    }
}
