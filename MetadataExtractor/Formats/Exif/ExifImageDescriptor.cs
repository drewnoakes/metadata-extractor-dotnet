// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif;

/// <summary>
/// Provides human-readable string representations of tag values stored in a <see cref="ExifImageDirectory"/>.
/// </summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public sealed class ExifImageDescriptor : ExifDescriptorBase<ExifImageDirectory>
{
    public ExifImageDescriptor(ExifImageDirectory directory)
        : base(directory)
    {
    }
}
