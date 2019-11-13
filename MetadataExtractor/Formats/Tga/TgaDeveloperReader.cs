// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;
using System.IO;

namespace MetadataExtractor.Formats.Tga
{
    /// <summary>Reads TGA image file developer area.</summary>
    /// <author>Dmitry Shechtman</author>
    internal sealed class TgaDeveloperReader : TgaDirectoryReader<TgaDeveloperDirectory, IndexedReader>
    {
        public static readonly TgaDeveloperReader Instance = new TgaDeveloperReader();

        private TgaDeveloperReader()
        {
        }

        protected override IndexedReader CreateReader(Stream stream)
        {
            return new IndexedSeekingReader(stream, isMotorolaByteOrder: false);
        }

        protected override void Populate(Stream stream, int offset, IndexedReader reader, TgaDeveloperDirectory directory)
        {
            var pos = stream.Position;
            var tags = TgaTagReader.Instance.Extract(stream);
            stream.Seek(pos - offset, SeekOrigin.Begin);
            for (int i = 0; i < tags.Length; i++)
            {
                var tag = tags[i];
                var bytes = reader.GetBytes(tag.Offset, tag.Size);
                directory.Set(tag.Id, bytes);
            }
        }
    }
}
