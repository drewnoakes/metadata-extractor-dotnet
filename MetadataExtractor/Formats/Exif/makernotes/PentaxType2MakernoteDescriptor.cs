// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Exif.Makernotes
{
    public sealed class PentaxType2MakernoteDescriptor : TagDescriptor<PentaxType2MakernoteDirectory>
    {
        public PentaxType2MakernoteDescriptor(PentaxType2MakernoteDirectory directory)
            : base(directory)
        {
        }
    }
}
