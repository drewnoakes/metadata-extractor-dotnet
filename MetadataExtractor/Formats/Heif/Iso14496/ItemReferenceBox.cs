// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
#if NET35
using System.Linq;
#endif
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    internal class ItemReferenceBox : FullBox
    {
        public IList<SingleItemTypeReferenceBox> Boxes { get; }

        public ItemReferenceBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            var list = new List<SingleItemTypeReferenceBox>();
            while (!loc.DoneReading(sr))
            {
                var box = BoxReader.ReadBox(sr, (l, r) => new SingleItemTypeReferenceBox(l, r, Version));
                if (box != null)
                    list.Add((SingleItemTypeReferenceBox)box);
            }
            Boxes = list;
        }

#if NET35
        public override IEnumerable<Box> Children() => Boxes.OfType<Box>();
#else
        public override IEnumerable<Box> Children() => Boxes;
#endif
    }
}
