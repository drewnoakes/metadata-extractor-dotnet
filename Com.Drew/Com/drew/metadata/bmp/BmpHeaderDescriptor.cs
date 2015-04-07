using System;
using Com.Drew.Metadata;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata.Bmp
{
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class BmpHeaderDescriptor : TagDescriptor<BmpHeaderDirectory>
	{
		public BmpHeaderDescriptor([NotNull] BmpHeaderDirectory directory)
			: base(directory)
		{
		}

		public override string GetDescription(int tagType)
		{
			switch (tagType)
			{
				case BmpHeaderDirectory.TagCompression:
				{
					return GetCompressionDescription();
				}

				default:
				{
					return base.GetDescription(tagType);
				}
			}
		}

		[CanBeNull]
		public virtual string GetCompressionDescription()
		{
			// 0 = None
			// 1 = RLE 8-bit/pixel
			// 2 = RLE 4-bit/pixel
			// 3 = Bit field (or Huffman 1D if BITMAPCOREHEADER2 (size 64))
			// 4 = JPEG (or RLE-24 if BITMAPCOREHEADER2 (size 64))
			// 5 = PNG
			// 6 = Bit field
			try
			{
				int? value = _directory.GetInteger(BmpHeaderDirectory.TagCompression);
				if (value == null)
				{
					return null;
				}
				int? headerSize = _directory.GetInteger(BmpHeaderDirectory.TagHeaderSize);
				if (headerSize == null)
				{
					return null;
				}
				switch (value)
				{
					case 0:
					{
						return "None";
					}

					case 1:
					{
						return "RLE 8-bit/pixel";
					}

					case 2:
					{
						return "RLE 4-bit/pixel";
					}

					case 3:
					{
						return headerSize == 64 ? "Bit field" : "Huffman 1D";
					}

					case 4:
					{
						return headerSize == 64 ? "JPEG" : "RLE-24";
					}

					case 5:
					{
						return "PNG";
					}

					case 6:
					{
						return "Bit field";
					}

					default:
					{
						return base.GetDescription(BmpHeaderDirectory.TagCompression);
					}
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
