// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif
{
    /// <summary>Describes Exif interoperability tags.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class ExifInteropDirectory : ExifDirectoryBase
    {
        private static readonly Dictionary<int, string> _tagNameMap = [];

        static ExifInteropDirectory()
        {
            AddExifTagNames(_tagNameMap);
        }

        public ExifInteropDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new ExifInteropDescriptor(this));
        }

        public override string Name => "Interoperability";
    }
}
