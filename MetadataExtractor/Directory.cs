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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace MetadataExtractor
{
    /// <summary>
    /// Abstract base class for all directory implementations, having methods for getting and setting tag values of various
    /// data types.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class Directory
    {
        /// <summary>Map of values hashed by type identifiers.</summary>
        [NotNull]
        private readonly Dictionary<int?, object> _tagMap = new Dictionary<int?, object>();

        /// <summary>Holds tags in the order in which they were stored.</summary>
        [NotNull]
        private readonly List<Tag> _definedTagList = new List<Tag>();

        [NotNull]
        private readonly List<string> _errorList = new List<string>(4);

        /// <summary>The descriptor used to interpret tag values.</summary>
        private ITagDescriptor _descriptor;

        /// <summary>Provides the name of the directory, for display purposes.</summary>
        /// <value>the name of the directory</value>
        [NotNull]
        public abstract string Name { get; }

        /// <summary>Provides the map of tag names, hashed by tag type identifier.</summary>
        /// <returns>the map of tag names</returns>
        [NotNull]
        protected abstract IReadOnlyDictionary<int?, string> GetTagNameMap();

        /// <summary>Gets a value indicating whether the directory is empty, meaning it contains no errors and no tag values.</summary>
        public bool IsEmpty
        {
            get { return _errorList.Count == 0 && _definedTagList.Count == 0; }
        }

        /// <summary>Indicates whether the specified tag type has been set.</summary>
        /// <param name="tagType">the tag type to check for</param>
        /// <returns>true if a value exists for the specified tag type, false if not</returns>
        public bool ContainsTag(int tagType)
        {
            return _tagMap.ContainsKey(tagType);
        }

        /// <summary>Returns an Iterator of Tag instances that have been set in this Directory.</summary>
        /// <value>The list of <see cref="Tag"/> instances</value>
        [NotNull]
        public IReadOnlyList<Tag> Tags
        {
            get { return _definedTagList; }
        }

        /// <summary>Returns the number of tags set in this Directory.</summary>
        /// <value>the number of tags set in this Directory</value>
        public int TagCount
        {
            get { return _definedTagList.Count; }
        }

        /// <summary>Sets the descriptor used to interpret tag values.</summary>
        /// <param name="descriptor">the descriptor used to interpret tag values</param>
        public void SetDescriptor([NotNull] ITagDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            _descriptor = descriptor;
        }

        /// <summary>Registers an error message with this directory.</summary>
        /// <param name="message">an error message.</param>
        public void AddError([NotNull] string message)
        {
            _errorList.Add(message);
        }

        /// <summary>Gets a value indicating whether this directory has one or more errors.</summary>
        /// <remarks>Error messages are accessible via <see cref="Errors"/>.</remarks>
        /// <returns><c>true</c> if the directory contains errors, otherwise <c>false</c></returns>
        public bool HasError
        {
            get { return _errorList.Count > 0; }
        }

        /// <summary>Used to iterate over any error messages contained in this directory.</summary>
        /// <value>The collection of error message strings.</value>
        [NotNull]
        public IReadOnlyCollection<string> Errors
        {
            get { return _errorList; }
        }

        #region Tag Setters

        /// <summary>Sets a <c>Object</c> for the specified tag.</summary>
        /// <remarks>Any previous value for this tag is overwritten.</remarks>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag</param>
        /// <exception cref="ArgumentNullException">if value is <c>null</c></exception>
        public virtual void Set(int tagType, [NotNull] object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (!_tagMap.ContainsKey(tagType))
                _definedTagList.Add(new Tag(tagType, this));

            _tagMap[tagType] = value;
        }

        #endregion

        #region Tag Getters

        /// <summary>Returns the object hashed for the particular tag type specified, if available.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's value as an Object if available, else <c>null</c></returns>
        [CanBeNull]
        public object GetObject(int tagType)
        {
            object val;
            return _tagMap.TryGetValue(tagType, out val) ? val : null;
        }

        #endregion

        /// <summary>Returns the name of a specified tag as a String.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's name as a String</returns>
        [NotNull]
        public string GetTagName(int tagType)
        {
            var nameMap = GetTagNameMap();
            string value;
            return nameMap.TryGetValue(tagType, out value)
                ? value
                : string.Format("Unknown tag (0x{0:x4})", tagType);
        }

        /// <summary>Gets whether the specified tag is known by the directory and has a name.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>whether this directory has a name for the specified tag</returns>
        public bool HasTagName(int tagType)
        {
            return GetTagNameMap().ContainsKey(tagType);
        }

        /// <summary>
        /// Provides a description of a tag's value using the descriptor set by
        /// <c>setDescriptor(Descriptor)</c>.
        /// </summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag value's description as a String</returns>
        [CanBeNull]
        public string GetDescription(int tagType)
        {
            Debug.Assert((_descriptor != null));
            return _descriptor.GetDescription(tagType);
        }

        public override string ToString()
        {
            return string.Format("{0} Directory ({1} {2})", Name, _tagMap.Count, _tagMap.Count == 1 ? "tag" : "tags");
        }
    }
}
