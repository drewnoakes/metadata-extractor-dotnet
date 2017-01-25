#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
using JetBrains.Annotations;

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
        [NotNull]
        private readonly Directory _directory;

        public Tag(int type, [NotNull] Directory directory)
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
        [CanBeNull]
        public string Description => _directory.GetDescription(Type);

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
        [NotNull]
        public string Name => _directory.GetTagName(Type);

        [NotNull]
        [Obsolete("Use Name instead")]
        public string TagName => Name;

        /// <summary>
        /// Get the name of the <see cref="Directory"/> in which the tag exists, such as <c>Exif</c>, <c>GPS</c> or <c>Interoperability</c>.
        /// </summary>
        [NotNull]
        public string DirectoryName => _directory.Name;

        /// <summary>A basic representation of the tag's type and value.</summary>
        /// <remarks>EG: <c>[ExifIfd0] F Number - f/2.8</c>.</remarks>
        /// <returns>The tag's type and value.</returns>
        public override string ToString() => $"[{DirectoryName}] {Name} - {Description ?? _directory.GetString(Type) + " (unable to formulate description)"}";
    }
}
