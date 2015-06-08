using System.IO;

namespace Sharpen
{
    public class ObjectOutputStream : OutputStream
    {
        private readonly BinaryWriter bw;

        public ObjectOutputStream (OutputStream os)
        {
            this.bw = new BinaryWriter (os.GetWrappedStream ());
        }

        public virtual void WriteInt (int i)
        {
            this.bw.Write (i);
        }
    }
}
