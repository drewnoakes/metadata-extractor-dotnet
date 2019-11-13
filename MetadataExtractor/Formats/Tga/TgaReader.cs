// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;

namespace MetadataExtractor.Formats.Tga
{
    internal abstract class TgaReader<TData, TReader>
    {
        public TData Extract(Stream stream, int offset = 0, SeekOrigin origin = SeekOrigin.Current)
        {
            var pos = stream.Position;
            stream.Seek(offset, origin);
            var reader = CreateReader(stream);
            var data = Extract(stream, offset, reader);
            stream.Seek(pos, SeekOrigin.Begin);
            return data;
        }

        protected abstract TReader CreateReader(Stream stream);
        protected abstract TData Extract(Stream stream, int offset, TReader reader);
    }

    internal abstract class TgaDirectoryReader<TDirectory, TReader> : TgaReader<TDirectory, TReader>
        where TDirectory : Directory, new()
    {
        protected sealed override TDirectory Extract(Stream stream, int offset, TReader reader)
        {
            var directory = new TDirectory();
            try
            {
                Populate(stream, offset, reader, directory);
            }
            catch (Exception ex)
            {
                directory.AddError($"Exception reading {directory.Name}: {ex.Message}");
            }
            return directory;
        }

        protected abstract void Populate(Stream stream, int offset, TReader reader, TDirectory directory);
    }
}
