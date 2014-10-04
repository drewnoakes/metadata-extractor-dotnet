using Com.Drew.Imaging.Jpeg;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Jpeg
{
	/// <summary>Defines an object that extracts metadata from in JPEG segments.</summary>
	public interface JpegSegmentMetadataReader
	{
		/// <summary>Gets the set of JPEG segment types that this reader is interested in.</summary>
		[NotNull]
		Iterable<JpegSegmentType> GetSegmentTypes();

		/// <summary>Gets a value indicating whether the supplied byte data can be processed by this reader.</summary>
		/// <remarks>
		/// Gets a value indicating whether the supplied byte data can be processed by this reader. This is not a guarantee
		/// that no errors will occur, but rather a best-effort indication of whether the parse is likely to succeed.
		/// Implementations are expected to check things such as the opening bytes, data length, etc.
		/// </remarks>
		bool CanProcess(sbyte[] segmentBytes, JpegSegmentType segmentType);

		/// <summary>
		/// Extracts metadata from a JPEG segment's byte array and merges it into the specified
		/// <see cref="Com.Drew.Metadata.Metadata"/>
		/// object.
		/// </summary>
		/// <param name="segmentBytes">The byte array from which the metadata should be extracted.</param>
		/// <param name="metadata">
		/// The
		/// <see cref="Com.Drew.Metadata.Metadata"/>
		/// object into which extracted values should be merged.
		/// </param>
		/// <param name="segmentType">
		/// The
		/// <see cref="JpegSegmentType"/>
		/// being read.
		/// </param>
		void Extract(sbyte[] segmentBytes, Com.Drew.Metadata.Metadata metadata, JpegSegmentType segmentType);
	}
}
