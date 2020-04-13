// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class ItemReferenceBox : FullBox
    {
        public IList<SingleItemTypeReferenceBox> Boxes { get; }

        public ItemReferenceBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            var list = new List<SingleItemTypeReferenceBox>();
            Boxes = list;
            while (!loc.DoneReading(sr))
            {
                list.Add(
                    (SingleItemTypeReferenceBox)BoxReader.ReadBox(
                        sr,
                        (l, r) => new SingleItemTypeReferenceBox(l, r, Version)));
            }
        }

        public override IEnumerable<Box> Children() => Boxes.OfType<Box>();
    }
}
