// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="JpegCommentDirectory"/>.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JpegCommentDescriptor : TagDescriptor<JpegCommentDirectory>
    {
        public JpegCommentDescriptor(JpegCommentDirectory directory)
            : base(directory)
        {
        }

        public string? GetJpegCommentDescription()
        {
            return Directory.GetString(JpegCommentDirectory.TagComment);
        }
    }
}
