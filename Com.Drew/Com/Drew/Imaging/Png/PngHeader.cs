using System.IO;
using Com.Drew.Imaging.Png;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class PngHeader
	{
		private int _imageWidth;

		private int _imageHeight;

		private sbyte _bitsPerSample;

		[NotNull]
		private PngColorType _colorType;

		private sbyte _compressionType;

		private sbyte _filterMethod;

		private sbyte _interlaceMethod;

		/// <exception cref="Com.Drew.Imaging.Png.PngProcessingException"/>
		public PngHeader(sbyte[] bytes)
		{
			if (bytes.Length != 13)
			{
				throw new PngProcessingException("PNG header chunk must have 13 data bytes");
			}
			SequentialReader reader = new SequentialByteArrayReader(bytes);
			try
			{
				_imageWidth = reader.GetInt32();
				_imageHeight = reader.GetInt32();
				_bitsPerSample = reader.GetInt8();
				sbyte colorTypeNumber = reader.GetInt8();
				PngColorType colorType = PngColorType.FromNumericValue(colorTypeNumber);
				if (colorType == null)
				{
					throw new PngProcessingException("Unexpected PNG color type: " + colorTypeNumber);
				}
				_colorType = colorType;
				_compressionType = reader.GetInt8();
				_filterMethod = reader.GetInt8();
				_interlaceMethod = reader.GetInt8();
			}
			catch (IOException e)
			{
				// Should never happen
				throw new PngProcessingException(e);
			}
		}

		public virtual int GetImageWidth()
		{
			return _imageWidth;
		}

		public virtual int GetImageHeight()
		{
			return _imageHeight;
		}

		public virtual sbyte GetBitsPerSample()
		{
			return _bitsPerSample;
		}

		[NotNull]
		public virtual PngColorType GetColorType()
		{
			return _colorType;
		}

		public virtual sbyte GetCompressionType()
		{
			return _compressionType;
		}

		public virtual sbyte GetFilterMethod()
		{
			return _filterMethod;
		}

		public virtual sbyte GetInterlaceMethod()
		{
			return _interlaceMethod;
		}
	}
}
