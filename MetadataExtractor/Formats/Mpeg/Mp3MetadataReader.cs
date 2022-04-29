// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Mpeg
{
    public static class Mp3MetadataReader
    {
        public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
        {
            return new[] { new Mp3Reader().Extract(new SequentialStreamReader(stream)) };
        }
    }
}
