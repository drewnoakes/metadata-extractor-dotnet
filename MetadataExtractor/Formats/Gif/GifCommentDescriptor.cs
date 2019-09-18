// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class GifCommentDescriptor : TagDescriptor<GifCommentDirectory>
    {
        public GifCommentDescriptor(GifCommentDirectory directory)
            : base(directory)
        {
        }
    }
}
