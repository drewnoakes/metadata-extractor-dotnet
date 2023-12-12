// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Mpeg;

public static class Mp3MetadataReader
{
    public static DirectoryList ReadMetadata(Stream stream)
    {
        return new[] { new Mp3Reader().Extract(new SequentialStreamReader(stream)) };
    }
}
