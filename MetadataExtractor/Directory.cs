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
using System.Collections.Generic;
using System.Diagnostics;
#if NETSTANDARD1_3
using System.Text;
#endif
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
#if NETSTANDARD1_3
        static Directory()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
#endif

        /// <summary>Map of values hashed by type identifiers.</summary>
        [NotNull]
        private readonly Dictionary<int, object> _tagMap = new Dictionary<int, object>();

        /// <summary>Holds tags in the order in which they were stored.</summary>
        [NotNull]
        private readonly List<Tag> _definedTagList = new List<Tag>();

        [NotNull]
        private readonly List<string> _errorList = new List<string>(capacity: 4);

        /// <summary>The descriptor used to interpret tag values.</summary>
        private ITagDescriptor _descriptor;

        /// <summary>Provides the name of the directory, for display purposes.</summary>
        /// <value>the name of the directory</value>
        [NotNull]
        public abstract string Name { get; }

        /// <summary>
        /// The parent <see cref="Directory"/>, when available, which may be used to construct information about the hierarchical structure of metadata.
        /// </summary>
        [CanBeNull]
        public Directory Parent { get; internal set; }

        /// <summary>Attempts to find the name of the specified tag.</summary>
        /// <param name="tagType">The tag to look up.</param>
        /// <param name="tagName">The found name, if any.</param>
        /// <returns><c>true</c> if the tag is known and <paramref name="tagName"/> was set, otherwise <c>false</c>.</returns>
        [ContractAnnotation("=>false,tagName:null")]
        [ContractAnnotation("=>true, tagName:notnull")]
        protected abstract bool TryGetTagName(int tagType, out string tagName);

        /// <summary>Gets a value indicating whether the directory is empty, meaning it contains no errors and no tag values.</summary>
        public bool IsEmpty => _errorList.Count == 0 && _definedTagList.Count == 0;

        /// <summary>Indicates whether the specified tag type has been set.</summary>
        /// <param name="tagType">the tag type to check for</param>
        /// <returns>true if a value exists for the specified tag type, false if not</returns>
        public bool ContainsTag(int tagType) => _tagMap.ContainsKey(tagType);

        /// <summary>Returns all <see cref="Tag"/> objects that have been set in this <see cref="Directory"/>.</summary>
        /// <value>The list of <see cref="Tag"/> objects.</value>
        [NotNull]
        public
#if NET35
            IEnumerable<Tag>
#else
            IReadOnlyList<Tag>
#endif
            Tags => _definedTagList;

        /// <summary>Returns the number of tags set in this Directory.</summary>
        /// <value>the number of tags set in this Directory</value>
        public int TagCount => _definedTagList.Count;

        /// <summary>Sets the descriptor used to interpret tag values.</summary>
        /// <param name="descriptor">the descriptor used to interpret tag values</param>
        protected void SetDescriptor([NotNull] ITagDescriptor descriptor)
        {
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        #region Errors

        /// <summary>Registers an error message with this directory.</summary>
        /// <param name="message">an error message.</param>
        public void AddError([NotNull] string message) => _errorList.Add(message);

        /// <summary>Gets a value indicating whether this directory has one or more errors.</summary>
        /// <remarks>Error messages are accessible via <see cref="Errors"/>.</remarks>
        /// <returns><c>true</c> if the directory contains errors, otherwise <c>false</c></returns>
        public bool HasError => _errorList.Count > 0;

        /// <summary>Used to iterate over any error messages contained in this directory.</summary>
        /// <value>The collection of error message strings.</value>
        [NotNull]
        public
#if NET35
            IEnumerable<string>
#else
            IReadOnlyList<string>
#endif
            Errors => _errorList;

        #endregion

        #region Get / set values

        /// <summary>Sets a <c>Object</c> for the specified tag.</summary>
        /// <remarks>Any previous value for this tag is overwritten.</remarks>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag</param>
        /// <exception cref="ArgumentNullException">if value is <c>null</c></exception>
        public virtual void Set(int tagType, [NotNull] object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!_tagMap.ContainsKey(tagType))
                _definedTagList.Add(new Tag(tagType, this));

            _tagMap[tagType] = value;
        }

        /// <summary>Returns the object hashed for the particular tag type specified, if available.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's value as an Object if available, else <c>null</c></returns>
        [CanBeNull]
        public object GetObject(int tagType)
        {
            return _tagMap.TryGetValue(tagType, out object val) ? val : null;
        }

        #endregion

        /// <summary>Returns the name of a specified tag as a String.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's name as a String</returns>
        [NotNull]
        public string GetTagName(int tagType)
        {
            return !TryGetTagName(tagType, out string name)
                ? $"Unknown tag (0x{tagType:x4})"
                : name;
        }

        /// <summary>Gets whether the specified tag is known by the directory and has a name.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>whether this directory has a name for the specified tag</returns>
        public bool HasTagName(int tagType) => TryGetTagName(tagType, out string _);

        /// <summary>
        /// Provides a description of a tag's value using the descriptor set by <see cref="SetDescriptor"/>.
        /// </summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag value's description as a String</returns>
        [CanBeNull]
        public string GetDescription(int tagType)
        {
            Debug.Assert(_descriptor != null);
            return _descriptor.GetDescription(tagType);
        }

        public override string ToString() => $"{Name} Directory ({_tagMap.Count} {(_tagMap.Count == 1 ? "tag" : "tags")})";
    }

    /// <summary>
    /// A directory to use for the reporting of errors. No values may be added to this directory, only warnings and errors.
    /// </summary>
    public sealed class ErrorDirectory : Directory
    {
        public override string Name => "Error";

        public ErrorDirectory() { }

        public ErrorDirectory(string error) => AddError(error);

        protected override bool TryGetTagName(int tagType, out string tagName)
        {
            tagName = null;
            return false;
        }

        public override void Set(int tagType, object value) => throw new NotSupportedException($"Cannot add values to {nameof(ErrorDirectory)}.");
    }
}
