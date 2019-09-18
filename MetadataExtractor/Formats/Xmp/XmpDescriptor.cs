// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Xmp
{
    /// <summary>Contains logic for the presentation of data stored in an <see cref="XmpDirectory"/>.</summary>
    /// <author>Torsten Skadell, Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class XmpDescriptor : TagDescriptor<XmpDirectory>
    {
        public XmpDescriptor(XmpDirectory directory)
            : base(directory)
        {}
    }
}
