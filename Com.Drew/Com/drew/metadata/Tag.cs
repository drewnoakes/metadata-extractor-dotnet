/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata
{
    /// <summary>
    /// Models a particular tag within a
    /// <see cref="Directory"/>
    /// and provides methods for obtaining its value.
    /// Immutable.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class Tag
    {
        private readonly int _tagType;

        [NotNull]
        private readonly Directory _directory;

        public Tag(int tagType, [NotNull] Directory directory)
        {
            _tagType = tagType;
            _directory = directory;
        }

        /// <summary>Gets the tag type as an int</summary>
        /// <returns>the tag type as an int</returns>
        public int GetTagType()
        {
            return _tagType;
        }

        /// <summary>
        /// Gets the tag type in hex notation as a String with padded leading
        /// zeroes if necessary (i.e. <c>0x100E</c>).
        /// </summary>
        /// <returns>the tag type as a string in hexadecimal notation</returns>
        [NotNull]
        public string GetTagTypeHex()
        {
            return _tagType.ToString("X4");
        }

        /// <summary>
        /// Get a description of the tag's value, considering enumerated values
        /// and units.
        /// </summary>
        /// <returns>a description of the tag's value</returns>
        [CanBeNull]
        public string GetDescription()
        {
            return _directory.GetDescription(_tagType);
        }

        /// <summary>Get whether this tag has a name.</summary>
        /// <remarks>
        /// Get whether this tag has a name.
        /// If <c>true</c>, it may be accessed via <see cref="GetTagName()"/>.
        /// If <c>false</c>, <see cref="GetTagName()"/> will return a string resembling <c>"Unknown tag (0x1234)"</c>.
        /// </remarks>
        /// <returns>whether this tag has a name</returns>
        public bool HasTagName()
        {
            return _directory.HasTagName(_tagType);
        }

        /// <summary>
        /// Get the name of the tag, such as <c>Aperture</c>, or
        /// <c>InteropVersion</c>.
        /// </summary>
        /// <returns>the tag's name</returns>
        [NotNull]
        public string GetTagName()
        {
            return _directory.GetTagName(_tagType);
        }

        /// <summary>
        /// Get the name of the
        /// <see cref="Directory"/>
        /// in which the tag exists, such as
        /// <c>Exif</c>, <c>GPS</c> or <c>Interoperability</c>.
        /// </summary>
        /// <returns>
        /// name of the
        /// <see cref="Directory"/>
        /// in which this tag exists
        /// </returns>
        [NotNull]
        public string GetDirectoryName()
        {
            return _directory.GetName();
        }

        /// <summary>A basic representation of the tag's type and value.</summary>
        /// <remarks>A basic representation of the tag's type and value.  EG: <c>[FNumber] F2.8</c>.</remarks>
        /// <returns>the tag's type and value</returns>
        public override string ToString()
        {
            string description = GetDescription();
            if (description == null)
            {
                description = _directory.GetString(GetTagType()) + " (unable to formulate description)";
            }
            return "[" + _directory.GetName() + "] " + GetTagName() + " - " + description;
        }
    }
}
