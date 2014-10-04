using Com.Drew.Imaging.Png;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	[System.Serializable]
	public sealed class PngColorType
	{
		/// <summary>Each pixel is a greyscale sample.</summary>
		public static readonly Com.Drew.Imaging.Png.PngColorType Greyscale = new Com.Drew.Imaging.Png.PngColorType(0, "Greyscale", 1, 2, 4, 8, 16);

		/// <summary>Each pixel is an R,G,B triple.</summary>
		public static readonly Com.Drew.Imaging.Png.PngColorType TrueColor = new Com.Drew.Imaging.Png.PngColorType(2, "True Color", 8, 16);

		/// <summary>Each pixel is a palette index.</summary>
		/// <remarks>Each pixel is a palette index. Seeing this value indicates that a <code>PLTE</code> chunk shall appear.</remarks>
		public static readonly Com.Drew.Imaging.Png.PngColorType IndexedColor = new Com.Drew.Imaging.Png.PngColorType(3, "Indexed Color", 1, 2, 4, 8);

		/// <summary>Each pixel is a greyscale sample followed by an alpha sample.</summary>
		public static readonly Com.Drew.Imaging.Png.PngColorType GreyscaleWithAlpha = new Com.Drew.Imaging.Png.PngColorType(4, "Greyscale with Alpha", 8, 16);

		/// <summary>Each pixel is an R,G,B triple followed by an alpha sample.</summary>
		public static readonly Com.Drew.Imaging.Png.PngColorType TrueColorWithAlpha = new Com.Drew.Imaging.Png.PngColorType(6, "True Color with Alpha", 8, 16);

		[CanBeNull]
		public static Com.Drew.Imaging.Png.PngColorType FromNumericValue(int numericValue)
		{
			Com.Drew.Imaging.Png.PngColorType[] colorTypes = typeof(Com.Drew.Imaging.Png.PngColorType).GetEnumConstants();
			foreach (Com.Drew.Imaging.Png.PngColorType colorType in colorTypes)
			{
				if (colorType.GetNumericValue() == numericValue)
				{
					return colorType;
				}
			}
			return null;
		}

		private readonly int _numericValue;

		[NotNull]
		private readonly string _description;

		[NotNull]
		private readonly int[] _allowedBitDepths;

		private PngColorType(int numericValue, string description, params int[] allowedBitDepths)
		{
			Com.Drew.Imaging.Png.PngColorType._numericValue = numericValue;
			Com.Drew.Imaging.Png.PngColorType._description = description;
			Com.Drew.Imaging.Png.PngColorType._allowedBitDepths = allowedBitDepths;
		}

		public int GetNumericValue()
		{
			return Com.Drew.Imaging.Png.PngColorType._numericValue;
		}

		[NotNull]
		public string GetDescription()
		{
			return Com.Drew.Imaging.Png.PngColorType._description;
		}

		[NotNull]
		public int[] GetAllowedBitDepths()
		{
			return Com.Drew.Imaging.Png.PngColorType._allowedBitDepths;
		}
	}
}
