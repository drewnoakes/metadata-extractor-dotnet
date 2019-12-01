// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;
using System.IO;

namespace MetadataExtractor.Formats.Tga
{
    /// <summary>Reads TGA image file developer area.</summary>
    /// <author>Dmitry Shechtman</author>
    internal sealed class TgaDeveloperReader : TgaDirectoryReader<TgaDeveloperDirectory>
    {
        protected override void Populate(Stream stream, int offset, TgaDeveloperDirectory directory)
        {
            var reader = new IndexedSeekingReader(stream, isMotorolaByteOrder: false);
            var pos = stream.Position;
            var tags = new TgaTagReader().Extract(stream);
            stream.Seek(pos - offset, SeekOrigin.Begin);
            foreach (var tag in tags)
            {
                var bytes = reader.GetBytes(tag.Offset, tag.Size);
                directory.Set(tag.Id, bytes);
            }
        }
    }
}
