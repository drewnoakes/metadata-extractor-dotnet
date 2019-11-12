// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Tga
{
    /// <author>Dmitry Shechtman</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class TgaDeveloperDirectory : Directory
    {
        public TgaDeveloperDirectory()
        {
            SetDescriptor(new TagDescriptor<TgaDeveloperDirectory>(this));
        }

        public override string Name => "TGA Developer Area";

        protected override bool TryGetTagName(int tagType, [NotNullWhen(true)] out string? tagName)
        {
            tagName = null;
            return false;
        }
    }
}
