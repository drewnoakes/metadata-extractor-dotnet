// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Mkv;

namespace MetadataExtractor.Formats.Ebml;

/// <summary>Extracts metadata from EBML documents using the supported Matroska and WebM schemas.</summary>
/// <remarks>Other EBML schemas are reported as unsupported. The input may be non-seekable and is left open.</remarks>
public static class EbmlMetadataReader
{
    public static IReadOnlyList<Directory> ReadMetadata(Stream stream) => new MkvReader().Extract(new SequentialStreamReader(stream));
}
