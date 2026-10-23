// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Util;

internal interface ITypeChecker
{
    int ByteCount { get; }

    /// <summary>Checks the buffered prefix and, if needed, probes the seekable stream without changing its position.</summary>
    FileType CheckType(byte[] bytes, Stream stream);
}
