using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class Box
    {
        private BoxLocation location;
        public uint Type => location.Type;
        public ulong Origin => location.Origin;
        public ulong Length => location.Length;
        public ulong NextPosition => location.NextPosition;

        public Box(BoxLocation location) => this.location = location;
        public string TypeString => TypeStringConverter.ToTypeString(Type);
        public void SkipRemainingData(SequentialReader sr)
        {
            sr.Skip((long) NextPosition - sr.Position);
        }
        public byte[] ReadRemainingData(SequentialReader sr) => sr.GetBytes((int) ((long) NextPosition - sr.Position));
    }
}
