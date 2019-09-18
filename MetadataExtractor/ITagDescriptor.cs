// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor
{
    public interface ITagDescriptor
    {
        /// <summary>Decodes the raw value stored for <paramref name="tagType"/>.</summary>
        /// <remarks>
        /// Where possible, known values will be substituted here in place of the raw
        /// tokens actually kept in the metadata segment.  If no substitution is
        /// available, the value provided by <see cref="DirectoryExtensions.GetString(MetadataExtractor.Directory,int)"/> will be returned.
        /// </remarks>
        /// <param name="tagType">The tag to find a description for.</param>
        /// <returns>
        /// A description of the image's value for the specified tag, or
        /// <c>null</c> if the tag hasn't been defined.
        /// </returns>
        string? GetDescription(int tagType);
    }
}
