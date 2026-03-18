// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tga;

internal abstract class TgaReader<T>
{
    public T Extract(Stream stream, int offset = 0, SeekOrigin origin = SeekOrigin.Current)
    {
        var pos = stream.Position;
        stream.Seek(offset, origin);
        var data = Extract(stream, offset);
        stream.Seek(pos, SeekOrigin.Begin);
        return data;
    }

    protected abstract T Extract(Stream stream, int offset);
}

internal abstract class TgaDirectoryReader<TDirectory> : TgaReader<TDirectory>
    where TDirectory : Directory, new()
{
    protected sealed override TDirectory Extract(Stream stream, int offset)
    {
        var directory = new TDirectory();
        try
        {
            Populate(stream, offset, directory);
        }
        catch (Exception ex)
        {
            directory.AddError($"Exception reading {directory.Name}: {ex.Message}");
        }
        return directory;
    }

    protected abstract void Populate(Stream stream, int offset, TDirectory directory);
}
