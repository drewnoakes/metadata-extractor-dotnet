// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>One of several Exif directories.</summary>
    /// <remarks>Holds information about image IFD's in a chain after the first. The first page is stored in IFD0.</remarks>
    /// <remarks>Currently, this only applies to multi-page TIFF images</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class ExifImageDirectory : ExifDirectoryBase
    {
        public ExifImageDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new ExifImageDescriptor(this));
        }

        private static readonly Dictionary<int, string> _tagNameMap = [];

        static ExifImageDirectory()
        {
            AddExifTagNames(_tagNameMap);
        }

        public override string Name => "Exif Image";
    }
}
