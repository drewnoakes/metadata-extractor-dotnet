// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    /// <summary>
    /// Provides human-readable string representations of tag values stored in a <see cref="RicohMakernoteDirectory"/>.
    /// </summary>
    /// <remarks>
    /// Some information about this makernote taken from here:
    /// http://www.ozhiker.com/electronics/pjmt/jpeg_info/ricoh_mn.html
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class RicohMakernoteDescriptor(RicohMakernoteDirectory directory)
        : TagDescriptor<RicohMakernoteDirectory>(directory)
    {
    }
}
