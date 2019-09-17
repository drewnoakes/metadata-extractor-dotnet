// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.WebP
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class WebPDescriptor : TagDescriptor<WebPDirectory>
    {
        public WebPDescriptor(WebPDirectory directory)
            : base(directory)
        {
        }
    }
}
