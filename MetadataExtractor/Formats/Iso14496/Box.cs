// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Iso14496
{
    internal class Box
    {
#if NET35 || NET45
        private static readonly Box[] _emptyChildren = new Box[0];
#else
        private static readonly Box[] _emptyChildren = Array.Empty<Box>();
#endif

        private readonly BoxLocation _location;

        public uint Type => _location.Type;
        public ulong NextPosition => _location.NextPosition;

        public Box(BoxLocation location) => _location = location;

        protected byte[] ReadRemainingData(SequentialReader sr)
        {
            return sr.GetBytes((int)((long)NextPosition - sr.Position));
        }

        public virtual IEnumerable<Box> Children() => _emptyChildren;

        public override string ToString() => $"{TypeStringConverter.ToTypeString(Type)} @ {_location.NextPosition}";
    }
}
