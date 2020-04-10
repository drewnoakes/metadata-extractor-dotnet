using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public class PixelInformationBox : FullBox
    {
        public byte ChannelCount { get; }
        public byte[] BitsPerChannel { get; }

        public PixelInformationBox(BoxLocation loc, SequentialReader sr) : base(loc, sr)
        {
            ChannelCount = sr.GetByte();
            BitsPerChannel = new byte[ChannelCount];
            for (int i = 0; i < BitsPerChannel.Length; i++)
            {
                BitsPerChannel[i] = sr.GetByte();
            }
        }
    }
}
