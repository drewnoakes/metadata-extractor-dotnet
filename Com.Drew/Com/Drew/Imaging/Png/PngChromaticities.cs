using System.IO;
using Com.Drew.Imaging.Png;
using Com.Drew.Lang;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PngChromaticities
	{
		private readonly int _whitePointX;

		private readonly int _whitePointY;

		private readonly int _redX;

		private readonly int _redY;

		private readonly int _greenX;

		private readonly int _greenY;

		private readonly int _blueX;

		private readonly int _blueY;

		/// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
		public PngChromaticities(sbyte[] bytes)
		{
			if (bytes.Length != 8 * 4)
			{
				throw new PngProcessingException("Invalid number of bytes");
			}
			SequentialByteArrayReader reader = new SequentialByteArrayReader(bytes);
			try
			{
				_whitePointX = reader.GetInt32();
				_whitePointY = reader.GetInt32();
				_redX = reader.GetInt32();
				_redY = reader.GetInt32();
				_greenX = reader.GetInt32();
				_greenY = reader.GetInt32();
				_blueX = reader.GetInt32();
				_blueY = reader.GetInt32();
			}
			catch (IOException ex)
			{
				throw new PngProcessingException(ex);
			}
		}

		public virtual int GetWhitePointX()
		{
			return _whitePointX;
		}

		public virtual int GetWhitePointY()
		{
			return _whitePointY;
		}

		public virtual int GetRedX()
		{
			return _redX;
		}

		public virtual int GetRedY()
		{
			return _redY;
		}

		public virtual int GetGreenX()
		{
			return _greenX;
		}

		public virtual int GetGreenY()
		{
			return _greenY;
		}

		public virtual int GetBlueX()
		{
			return _blueX;
		}

		public virtual int GetBlueY()
		{
			return _blueY;
		}
	}
}
