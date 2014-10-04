namespace Sharpen
{
	using System;

	public class PipedOutputStream : OutputStream
	{
		PipedInputStream ips;

		public PipedOutputStream ()
		{
		}

		public PipedOutputStream (PipedInputStream iss) : this()
		{
			Attach (iss);
		}

		public override void Close ()
		{
			ips.Close ();
			base.Close ();
		}

		public void Attach (PipedInputStream iss)
		{
			ips = iss;
		}

		public override void Write (int b)
		{
			ips.Write (b);
		}

		public override void Write (sbyte[] b, int offset, int len)
		{
			ips.Write (b, offset, len);
		}
	}
}
