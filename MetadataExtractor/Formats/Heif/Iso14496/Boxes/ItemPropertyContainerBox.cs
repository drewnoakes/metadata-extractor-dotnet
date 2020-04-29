// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496.Boxes
{
    internal class ItemPropertyContainerBox : Box
    {
        public IList<Box> Boxes { get; }

        public ItemPropertyContainerBox(BoxLocation loc, SequentialReader sr)
            : base(loc)
        {
            Boxes = BoxReader.ReadBoxes(sr, loc);
        }

        public override IEnumerable<Box> Children() => Boxes;
    }
}
