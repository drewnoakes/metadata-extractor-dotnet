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

using System.Diagnostics;
using System.IO;
using Com.Drew.Lang;
using JetBrains.Annotations;

namespace Com.Drew.Metadata.Ico
{
    /// <summary>Reads ICO (Windows Icon) file metadata.</summary>
    /// <remarks>
    /// Reads ICO (Windows Icon) file metadata.
    /// <list type="bullet">
    /// <item>https://en.wikipedia.org/wiki/ICO_(file_format)</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IcoReader
    {
        public void Extract([NotNull] SequentialReader reader, [NotNull] Metadata metadata)
        {
            reader.SetMotorolaByteOrder(false);
            int type;
            int imageCount;
            // Read header (ICONDIR structure)
            try
            {
                int reserved = reader.GetUInt16();
                if (reserved != 0)
                {
                    IcoDirectory directory = new IcoDirectory();
                    directory.AddError("Invalid header bytes");
                    metadata.AddDirectory(directory);
                    return;
                }
                type = reader.GetUInt16();
                if (type != 1 && type != 2)
                {
                    IcoDirectory directory = new IcoDirectory();
                    directory.AddError("Invalid type " + type + " -- expecting 1 or 2");
                    metadata.AddDirectory(directory);
                    return;
                }
                imageCount = reader.GetUInt16();
                if (imageCount == 0)
                {
                    IcoDirectory directory = new IcoDirectory();
                    directory.AddError("Image count cannot be zero");
                    metadata.AddDirectory(directory);
                    return;
                }
            }
            catch (IOException ex)
            {
                IcoDirectory directory = new IcoDirectory();
                directory.AddError("Exception reading ICO file metadata: " + ex.Message);
                metadata.AddDirectory(directory);
                return;
            }
            // Read each embedded image
            IcoDirectory directory1 = null;
            try
            {
                for (int imageIndex = 0; imageIndex < imageCount; imageIndex++)
                {
                    directory1 = new IcoDirectory();
                    metadata.AddDirectory(directory1);
                    directory1.SetInt(IcoDirectory.TagImageType, type);
                    directory1.SetInt(IcoDirectory.TagImageWidth, reader.GetUInt8());
                    directory1.SetInt(IcoDirectory.TagImageHeight, reader.GetUInt8());
                    directory1.SetInt(IcoDirectory.TagColourPaletteSize, reader.GetUInt8());
                    // Ignore this byte (normally zero, though .NET's System.Drawing.Icon.Save method writes 255)
                    reader.GetUInt8();
                    if (type == 1)
                    {
                        // Icon
                        directory1.SetInt(IcoDirectory.TagColourPlanes, reader.GetUInt16());
                        directory1.SetInt(IcoDirectory.TagBitsPerPixel, reader.GetUInt16());
                    }
                    else
                    {
                        // Cursor
                        directory1.SetInt(IcoDirectory.TagCursorHotspotX, reader.GetUInt16());
                        directory1.SetInt(IcoDirectory.TagCursorHotspotY, reader.GetUInt16());
                    }
                    directory1.SetLong(IcoDirectory.TagImageSizeBytes, reader.GetUInt32());
                    directory1.SetLong(IcoDirectory.TagImageOffsetBytes, reader.GetUInt32());
                }
            }
            catch (IOException ex)
            {
                Debug.Assert((directory1 != null));
                directory1.AddError("Exception reading ICO file metadata: " + ex.Message);
            }
        }
    }
}
