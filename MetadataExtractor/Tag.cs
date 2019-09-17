// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace MetadataExtractor
{
    /// <summary>
    /// Models metadata of a tag within a <see cref="Directory"/> and provides methods
    /// for obtaining its value.
    /// </summary>
    /// <remarks>Immutable.</remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class Tag
    {
        private readonly Directory _directory;

        public Tag(int type, Directory directory)
        {
            Type = type;
            _directory = directory;
        }

        /// <summary>Gets the tag type as an int</summary>
        /// <value>the tag type as an int</value>
        public int Type { get; }

        [Obsolete("Use Type instead.")]
        public int TagType => Type;

        /// <summary>
        /// Get a description of the tag's value, considering enumerated values
        /// and units.
        /// </summary>
        /// <value>a description of the tag's value</value>
        public string? Description => _directory.GetDescription(Type);

        /// <summary>Get whether this tag has a name.</summary>
        /// <remarks>
        /// If <c>true</c>, it may be accessed via <see cref="Name"/>.
        /// If <c>false</c>, <see cref="Name"/> will return a string resembling <c>"Unknown tag (0x1234)"</c>.
        /// </remarks>
        public bool HasName => _directory.HasTagName(Type);

        [Obsolete("Use HasName instead.")]
        public bool HasTagName => HasName;

        /// <summary>
        /// Get the name of the tag, such as <c>Aperture</c>, or <c>InteropVersion</c>.
        /// </summary>
        public string Name => _directory.GetTagName(Type);

        [Obsolete("Use Name instead")]
        public string TagName => Name;

        /// <summary>
        /// Get the name of the <see cref="Directory"/> in which the tag exists, such as <c>Exif</c>, <c>GPS</c> or <c>Interoperability</c>.
        /// </summary>
        public string DirectoryName => _directory.Name;

        /// <summary>A basic representation of the tag's type and value.</summary>
        /// <remarks>EG: <c>[ExifIfd0] F Number - f/2.8</c>.</remarks>
        /// <returns>The tag's type and value.</returns>
        public override string ToString() => $"[{DirectoryName}] {Name} - {Description ?? _directory.GetString(Type) + " (unable to formulate description)"}";
    }
}
