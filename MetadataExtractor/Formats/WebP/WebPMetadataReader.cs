// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.FileSystem;
using MetadataExtractor.Formats.Riff;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.WebP;

/// <summary>Obtains metadata from WebP files.</summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public static class WebPMetadataReader
{
    /// <exception cref="IOException"/>
    /// <exception cref="RiffProcessingException"/>
    public static DirectoryList ReadMetadata(string filePath)
    {
        var directories = new List<Directory>();

        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            directories.AddRange(ReadMetadata(stream));

        directories.Add(new FileMetadataReader().Read(filePath));

        return directories;
    }

    /// <exception cref="IOException"/>
    /// <exception cref="RiffProcessingException"/>
    public static DirectoryList ReadMetadata(Stream stream)
    {
        var directories = new List<Directory>();
        new RiffReader().ProcessRiff(new SequentialStreamReader(stream), new WebPRiffHandler(directories));
        return directories;
    }
}
