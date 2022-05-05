﻿// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Iso14496
{
    internal class Box
    {
        private readonly BoxLocation _location;

        public uint Type => _location.Type;
        public ulong NextPosition => _location.NextPosition;

        public Box(BoxLocation location) => _location = location;

        protected byte[] ReadRemainingData(SequentialReader sr)
        {
            return sr.GetBytes((int)((long)NextPosition - sr.Position));
        }

        public virtual IEnumerable<Box> Children() => System.Array.Empty<Box>();

        public override string ToString() => $"{TypeStringConverter.ToTypeString(Type)} @ {_location.NextPosition}";
    }
}
