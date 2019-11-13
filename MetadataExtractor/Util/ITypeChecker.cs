// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Util
{
    internal interface ITypeChecker
    {
        int ByteCount { get; }
        FileType CheckType(byte[] bytes);
    }
}
