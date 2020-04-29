// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal static class BoxReaderExtensions
    {
        public static bool IsWithinBox(this SequentialReader reader, BoxLocation location) => (ulong)reader.Position < location.NextPosition;

        public static ulong BytesRemainingInBox(this SequentialReader reader, BoxLocation boxLocation) => boxLocation.NextPosition - (ulong)reader.Position;
    }
}
