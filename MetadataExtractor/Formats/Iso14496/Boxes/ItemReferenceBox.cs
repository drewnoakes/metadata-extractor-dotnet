﻿// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Iso14496.Boxes
{
    internal sealed class ItemReferenceBox : FullBox
    {
        public IList<SingleItemTypeReferenceBox> Boxes { get; }

        public ItemReferenceBox(BoxLocation location, SequentialReader reader)
            : base(location, reader)
        {
            var list = new List<SingleItemTypeReferenceBox>();
            while (reader.IsWithinBox(location))
            {
                var box = BoxReader.ReadBox(reader, (l, r) => new SingleItemTypeReferenceBox(l, r, Version));
                if (box != null)
                    list.Add((SingleItemTypeReferenceBox)box);
            }
            Boxes = list;
        }

        public override IEnumerable<Box> Children() => Boxes;
    }
}
