// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Describes tags used by a JPEG file comment.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class JpegCommentDirectory : Directory
    {
        /// <summary>This value does not apply to a particular standard.</summary>
        /// <remarks>
        /// This value does not apply to a particular standard. Rather, this value has been fabricated to maintain
        /// consistency with other directory types.
        /// </remarks>
        public const int TagComment = 0;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagComment, "JPEG Comment" }
        };

        public JpegCommentDirectory(StringValue comment)
        {
            SetDescriptor(new JpegCommentDescriptor(this));
            Set(TagComment, comment);
        }

        public override string Name => "JpegComment";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }
    }
}
