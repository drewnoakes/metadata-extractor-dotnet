// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Formats.Raf
{
    /// <summary>Obtains metadata from RAF (Fujifilm camera raw) image files.</summary>
    /// <author>TSGames https://github.com/TSGames</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public static class RafMetadataReader
    {
        public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
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
