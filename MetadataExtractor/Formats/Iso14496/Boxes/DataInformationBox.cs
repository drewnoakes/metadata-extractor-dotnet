// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Iso14496.Boxes
{
    internal class DataInformationBox : Box
    {
        public IList<Box> Boxes { get; }

        public DataInformationBox(BoxLocation loc, SequentialReader sr)
            : base(loc)
        {
            Boxes = BoxReader.ReadBoxes(sr, loc);
        }

        public override IEnumerable<Box> Children() => Boxes;
    }
}
