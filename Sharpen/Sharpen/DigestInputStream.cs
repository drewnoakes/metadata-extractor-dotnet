using System;

namespace Sharpen
{
	public class DigestInputStream : FilterInputStream
	{
		MessageDigest Digest {
			get; set;
		}

		public DigestInputStream (InputStream stream, MessageDigest digest)
			: base (stream)
		{
			Digest = digest;
		}

		public override int Read ()
		{
			var read = this.@in.Read ();
			if (read > 0)
				Digest.Update ((sbyte) read);
			return read;
		}
		
		public override int Read (sbyte[] buf)
		{
			var read = this.@in.Read (buf);
			if (read > 0)
				Digest.Update (buf, 0, read);
			return read;
		}
		
		public override int Read (sbyte[] b, int off, int len)
		{
			var read = this.@in.Read (b, off, len);
			if (read > 0)
				Digest.Update (b, off, read);
			return read;
		}
	}
}

