using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class Box
    {
        private static readonly Box[] EmptyChildren = new Box[0];

        private readonly BoxLocation location;

        public uint Type => location.Type;
        public ulong Origin => location.Origin;
        public ulong Length => location.Length;
        public ulong NextPosition => location.NextPosition;
        public string TypeString => TypeStringConverter.ToTypeString(Type);

        public Box(BoxLocation location) => this.location = location;

        public void SkipRemainingData(SequentialReader sr)
        {
            sr.Skip((long)NextPosition - sr.Position);
        }

        public byte[] ReadRemainingData(SequentialReader sr)
        {
            return sr.GetBytes((int)((long)this.NextPosition - sr.Position));
        }

        public virtual IEnumerable<Box> Children() => EmptyChildren;
    }
}
