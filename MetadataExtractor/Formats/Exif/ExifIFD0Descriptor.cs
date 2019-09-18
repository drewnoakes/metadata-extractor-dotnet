// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="ExifIfd0Directory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ExifIfd0Descriptor : ExifDescriptorBase<ExifIfd0Directory>
    {
        public ExifIfd0Descriptor(ExifIfd0Directory directory)
            : base(directory)
        {
        }
    }
}
