namespace Sharpen
{
	using System;
	using System.IO;

    public class ByteArrayInputStream : InputStream
	{
		public ByteArrayInputStream (sbyte[] data)
		{
            base.Wrapped = new MemoryStream(Sharpen.Extensions.ConvertToByteArray(data));
		}

		public ByteArrayInputStream (sbyte[] data, int off, int len)
		{
			base.Wrapped = new MemoryStream (Sharpen.Extensions.ConvertToByteArray(data), off, len);
		}
		
		public override int Available ()
		{
			MemoryStream ms = (MemoryStream) Wrapped;
			return (int)(ms.Length - ms.Position);
		}
	}
}
