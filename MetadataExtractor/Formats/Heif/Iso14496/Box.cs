// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496
{
    public class Box
    {
        private static readonly Box[] _emptyChildren = new Box[0];

        private readonly BoxLocation _location;

        public uint Type => this._location.Type;
        public ulong Origin => this._location.Origin;
        public ulong Length => this._location.Length;
        public ulong NextPosition => this._location.NextPosition;
        public string TypeString => TypeStringConverter.ToTypeString(Type);

        public Box(BoxLocation location) => this._location = location;

        public void SkipRemainingData(SequentialReader sr)
        {
            sr.Skip((long)NextPosition - sr.Position);
        }

        public byte[] ReadRemainingData(SequentialReader sr)
        {
            return sr.GetBytes((int)((long)this.NextPosition - sr.Position));
        }

        public virtual IEnumerable<Box> Children() => _emptyChildren;
    }
}
