// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Ico
{
    /// <summary>Reads ICO (Windows Icon) file metadata.</summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>https://en.wikipedia.org/wiki/ICO_(file_format)</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IcoReader
    {
        public DirectoryList Extract(SequentialReader reader)
        {
            var directories = new List<Directory>();

            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

            var type = 0;
            var imageCount = 0;

            // Read header (ICONDIR structure)

            string? error = null;
            try
            {
                var reserved = reader.GetUInt16();
                type = reader.GetUInt16();
                imageCount = reader.GetUInt16();

                if (reserved != 0)
                    error = "Invalid header bytes";
                else if (type != 1 && type != 2)
                    error = "Invalid type " + type + " -- expecting 1 or 2";
                else if (imageCount == 0)
                    error = "Image count cannot be zero";
            }
            catch (IOException ex)
            {
                error = "Exception reading ICO file metadata: " + ex.Message;
            }

            if (error != null)
            {
                var directory = new IcoDirectory();
                directory.AddError(error);
                directories.Add(directory);
                return directories;
            }

            // Read each embedded image
            for (var imageIndex = 0; imageIndex < imageCount; imageIndex++)
            {
                var directory = new IcoDirectory();

                try
                {
                    directory.Set(IcoDirectory.TagImageType, type);
                    directory.Set(IcoDirectory.TagImageWidth, reader.GetByte());
                    directory.Set(IcoDirectory.TagImageHeight, reader.GetByte());
                    directory.Set(IcoDirectory.TagColourPaletteSize, reader.GetByte());
                    // Ignore this byte (normally zero, though .NET's System.Drawing.Icon.Save method writes 255)
                    reader.GetByte();

                    if (type == 1)
                    {
                        // Icon
                        directory.Set(IcoDirectory.TagColourPlanes, reader.GetUInt16());
                        directory.Set(IcoDirectory.TagBitsPerPixel, reader.GetUInt16());
                    }
                    else
                    {
                        // Cursor
                        directory.Set(IcoDirectory.TagCursorHotspotX, reader.GetUInt16());
                        directory.Set(IcoDirectory.TagCursorHotspotY, reader.GetUInt16());
                    }

                    directory.Set(IcoDirectory.TagImageSizeBytes, reader.GetUInt32());
                    directory.Set(IcoDirectory.TagImageOffsetBytes, reader.GetUInt32());
                }
                catch (IOException ex)
                {
                    directory.AddError("Exception reading ICO file metadata: " + ex.Message);
                }

                directories.Add(directory);

                if (directory.HasError)
                    break;
            }

            return directories;
        }
    }
}
