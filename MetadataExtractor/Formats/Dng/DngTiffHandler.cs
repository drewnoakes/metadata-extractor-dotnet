// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Tiff;

namespace MetadataExtractor.Formats.Dng;

internal sealed class DngTiffHandler(List<Directory> directories) : ExifTiffHandler(directories, exifStartOffset: 0)
{
    private readonly List<Directory> _imageDirectories = [];
    private readonly HashSet<Directory> _imageDirectorySet = [];

    public override TiffStandard ProcessTiffMarker(ushort marker)
    {
        var standard = base.ProcessTiffMarker(marker);
        if (marker is 0x002A or 0x002B)
            AddImageDirectory(CurrentDirectory!);
        return standard;
    }

    public override bool TryEnterSubIfd(int tagId)
    {
        var isImage = tagId == ExifDirectoryBase.TagSubIfdOffset && _imageDirectorySet.Contains(CurrentDirectory!);
        var entered = base.TryEnterSubIfd(tagId);
        if (entered && isImage)
            AddImageDirectory(CurrentDirectory!);
        return entered;
    }

    public override bool HasFollowerIfd()
    {
        var isImage = _imageDirectorySet.Contains(CurrentDirectory!);
        var entered = base.HasFollowerIfd();
        if (entered && isImage)
            AddImageDirectory(CurrentDirectory!);
        return entered;
    }

    private void AddImageDirectory(Directory directory)
    {
        _imageDirectories.Add(directory);
        _imageDirectorySet.Add(directory);
    }

    public override void EndingIfd(in TiffReaderContext context)
    {
        // SubIFDs may have been visited before IFD0's DNGVersion. Defer interpretation
        // until the entire image tree is available, retaining the original TIFF directories.
        if (_imageDirectories.Count != 0 && ReferenceEquals(CurrentDirectory, _imageDirectories[0]))
        {
            var version = CurrentDirectory!.GetObject(DngDirectory.TagDngVersion);
            if (version is not null)
            {
                // CLR array compatibility also matches sbyte[] with byte[].
                if (version is not byte[] { Length: 4 } bytes || version.GetType() != typeof(byte[]) || bytes[0] == 0)
                {
                    CurrentDirectory.AddError("Invalid DNG version: expected four bytes with a nonzero major version");
                }
                else
                {
                    foreach (var source in _imageDirectories)
                    {
                        var dng = new DngDirectory { Parent = source };
                        foreach (var tag in source.Tags)
                        {
                            if (dng.HasTagName(tag.Type))
                                dng.Set(tag.Type, source.GetObject(tag.Type)!);
                        }
                        if (!dng.IsEmpty)
                            Directories.Add(dng);
                    }
                }
            }
        }

        base.EndingIfd(in context);
    }
}
