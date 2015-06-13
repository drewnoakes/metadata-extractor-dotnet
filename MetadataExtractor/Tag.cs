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

namespace MetadataExtractor
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
        [NotNull]
        private readonly Directory _directory;

        public Tag(int tagType, [NotNull] Directory directory)
        {
            TagType = tagType;
            _directory = directory;
        }

        /// <summary>Gets the tag type as an int</summary>
        /// <value>the tag type as an int</value>
        public int TagType { get; private set; }

        /// <summary>
        /// Get a description of the tag's value, considering enumerated values
        /// and units.
        /// </summary>
        /// <value>a description of the tag's value</value>
        [CanBeNull]
        public string Description
        {
            get { return _directory.GetDescription(TagType); }
        }

        /// <summary>Get whether this tag has a name.</summary>
        /// <remarks>
        /// If <c>true</c>, it may be accessed via <see cref="TagName"/>.
        /// If <c>false</c>, <see cref="TagName"/> will return a string resembling <c>"Unknown tag (0x1234)"</c>.
        /// </remarks>
        public bool HasTagName
        {
            get { return _directory.HasTagName(TagType); }
        }

        /// <summary>
        /// Get the name of the tag, such as <c>Aperture</c>, or <c>InteropVersion</c>.
        /// </summary>
        [NotNull]
        public string TagName
        {
            get { return _directory.GetTagName(TagType); }
        }

        /// <summary>
        /// Get the name of the <see cref="Directory"/> in which the tag exists, such as <c>Exif</c>, <c>GPS</c> or <c>Interoperability</c>.
        /// </summary>
        [NotNull]
        public string DirectoryName
        {
            get { return _directory.Name; }
        }

        /// <summary>A basic representation of the tag's type and value.</summary>
        /// <remarks>A basic representation of the tag's type and value.  EG: <c>[FNumber] F2.8</c>.</remarks>
        /// <returns>the tag's type and value</returns>
        public override string ToString()
        {
            return string.Format("[{0}] {1} - {2}",
                _directory.Name,
                TagName,
                Description ?? _directory.GetString(TagType) + " (unable to formulate description)");
        }
    }
}
