using JetBrains.Annotations;

namespace Com.Drew.Metadata
{
    public interface ITagDescriptor
    {
        /// <summary>Returns a descriptive value of the specified tag for this image.</summary>
        /// <remarks>
        /// Returns a descriptive value of the specified tag for this image.
        /// Where possible, known values will be substituted here in place of the raw
        /// tokens actually kept in the metadata segment.  If no substitution is
        /// available, the value provided by <code>getString(tagType)</code> will be returned.
        /// </remarks>
        /// <param name="tagType">the tag to find a description for</param>
        /// <returns>
        /// a description of the image's value for the specified tag, or
        /// <code>null</code> if the tag hasn't been defined.
        /// </returns>
        [CanBeNull]
        string GetDescription(int tagType);
    }
}