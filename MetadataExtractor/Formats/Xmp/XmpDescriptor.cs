// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Xmp
{
    /// <summary>Contains logic for the presentation of data stored in an <see cref="XmpDirectory"/>.</summary>
    /// <author>Torsten Skadell, Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpDescriptor(XmpDirectory directory) : TagDescriptor<XmpDirectory>(directory)
    {
    }
}
