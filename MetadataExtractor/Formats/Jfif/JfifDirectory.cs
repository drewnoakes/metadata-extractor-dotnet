// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Jfif
{
    /// <summary>Directory of tags and values for the SOF0 Jfif segment.</summary>
    /// <author>Yuri Binev, Drew Noakes</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class JfifDirectory : Directory
    {
        public const int TagVersion = 5;

        /// <summary>Units for pixel density fields.</summary>
        /// <remarks>One of None, Pixels per Inch, Pixels per Centimetre.</remarks>
        public const int TagUnits = 7;

        public const int TagResX = 8;
        public const int TagResY = 10;
        public const int TagThumbWidth = 12;
        public const int TagThumbHeight = 13;

        private static readonly Dictionary<int, string> _tagNameMap = new Dictionary<int, string>
        {
            { TagVersion, "Version" },
            { TagUnits, "Resolution Units" },
            { TagResY, "Y Resolution" },
            { TagResX, "X Resolution" },
            { TagThumbWidth, "Thumbnail Width Pixels" },
            { TagThumbHeight, "Thumbnail Height Pixels" }
        };

        public JfifDirectory()
        {
            SetDescriptor(new JfifDescriptor(this));
        }

        public override string Name => "JFIF";

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        public int GetVersion()
        {
            return this.GetInt32(TagVersion);
        }

        public int GetResUnits()
        {
            return this.GetInt32(TagUnits);
        }

        public int GetImageWidth()
        {
            return this.GetInt32(TagResY);
        }

        public int GetImageHeight()
        {
            return this.GetInt32(TagResX);
        }
    }
}
