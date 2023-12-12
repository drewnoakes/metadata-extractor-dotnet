// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif;

/// <author>Drew Noakes https://drewnoakes.com</author>
/// <author>Kevin Mott https://github.com/kwhopper</author>
public class GifCommentDirectory : Directory
{
    public const int TagComment = 1;

    private static readonly Dictionary<int, string> _tagNameMap = new()
    {
        { TagComment, "Comment" }
    };

    public GifCommentDirectory(StringValue comment) : base(_tagNameMap)
    {
        SetDescriptor(new GifCommentDescriptor(this));
        Set(TagComment, comment);
    }

    public override string Name => "GIF Comment";
}
