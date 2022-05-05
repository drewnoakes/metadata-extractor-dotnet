// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor
{
    /// <summary>
    /// Abstract base class for all directory implementations, having methods for getting and setting tag values of various
    /// data types.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class Directory
    {
#if NETSTANDARD2_0
        static Directory()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
#endif

        private readonly Dictionary<int, string>? _tagNameMap;

        /// <summary>Map of values hashed by type identifiers.</summary>
        private readonly Dictionary<int, object> _tagMap = new();

        /// <summary>Holds tags in the order in which they were stored.</summary>
        private readonly List<Tag> _definedTagList = new();

        private readonly List<string> _errorList = new(capacity: 4);

        /// <summary>The descriptor used to interpret tag values.</summary>
        private ITagDescriptor? _descriptor;

        /// <summary>Provides the name of the directory, for display purposes.</summary>
        /// <value>the name of the directory</value>
        public abstract string Name { get; }

        /// <summary>
        /// The parent <see cref="Directory"/>, when available, which may be used to construct information about the hierarchical structure of metadata.
        /// </summary>
        public Directory? Parent { get; internal set; }

        protected Directory(Dictionary<int, string>? tagNameMap)
        {
            _tagNameMap = tagNameMap;
        }

        /// <summary>Attempts to find the name of the specified tag.</summary>
        /// <param name="tagType">The tag to look up.</param>
        /// <param name="tagName">The found name, if any.</param>
        /// <returns><c>true</c> if the tag is known and <paramref name="tagName"/> was set, otherwise <c>false</c>.</returns>
        protected virtual bool TryGetTagName(int tagType, [NotNullWhen(returnValue: true)] out string? tagName)
        {
            if (_tagNameMap == null)
            {
                tagName = default;
                return false;
            }

            return _tagNameMap.TryGetValue(tagType, out tagName);
        }

        /// <summary>Gets a value indicating whether the directory is empty, meaning it contains no errors and no tag values.</summary>
        public bool IsEmpty => _errorList.Count == 0 && _definedTagList.Count == 0;

        /// <summary>Indicates whether the specified tag type has been set.</summary>
        /// <param name="tagType">the tag type to check for</param>
        /// <returns>true if a value exists for the specified tag type, false if not</returns>
        public bool ContainsTag(int tagType) => _tagMap.ContainsKey(tagType);

        /// <summary>Returns all <see cref="Tag"/> objects that have been set in this <see cref="Directory"/>.</summary>
        /// <value>The list of <see cref="Tag"/> objects.</value>
        public IReadOnlyList<Tag> Tags => _definedTagList;

        /// <summary>Returns the number of tags set in this Directory.</summary>
        /// <value>the number of tags set in this Directory</value>
        public int TagCount => _definedTagList.Count;

        /// <summary>Sets the descriptor used to interpret tag values.</summary>
        /// <param name="descriptor">the descriptor used to interpret tag values</param>
        protected void SetDescriptor(ITagDescriptor descriptor)
        {
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        #region Errors

        /// <summary>Registers an error message with this directory.</summary>
        /// <param name="message">an error message.</param>
        public void AddError(string message) => _errorList.Add(message);

        /// <summary>Gets a value indicating whether this directory has one or more errors.</summary>
        /// <remarks>Error messages are accessible via <see cref="Errors"/>.</remarks>
        /// <returns><c>true</c> if the directory contains errors, otherwise <c>false</c></returns>
        public bool HasError => _errorList.Count > 0;

        /// <summary>Used to iterate over any error messages contained in this directory.</summary>
        /// <value>The collection of error message strings.</value>
        public IReadOnlyList<string> Errors => _errorList;

        #endregion

        #region Get / set values

        /// <summary>Sets a <c>Object</c> for the specified tag.</summary>
        /// <remarks>Any previous value for this tag is overwritten.</remarks>
        /// <param name="tagType">the tag's value as an int</param>
        /// <param name="value">the value for the specified tag</param>
        /// <exception cref="ArgumentNullException">if value is <see langword="null" /></exception>
        public virtual void Set(int tagType, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!_tagMap.ContainsKey(tagType))
                _definedTagList.Add(new Tag(tagType, this));

            _tagMap[tagType] = value;
        }

        /// <summary>Returns the object hashed for the particular tag type specified, if available.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's value as an Object if available, else <see langword="null" /></returns>
        public object? GetObject(int tagType)
        {
            return _tagMap.TryGetValue(tagType, out object? val) ? val : null;
        }

        #endregion

        public void RemoveTag(int tagId)
        {
            if (_tagMap.Remove(tagId))
            {
                var index = _definedTagList.FindIndex(tag => tag.Type == tagId);

                if (index != -1)
                {
                    _definedTagList.RemoveAt(index);
                }
            }
        }

        /// <summary>Returns the name of a specified tag as a String.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag's name as a String</returns>
        public string GetTagName(int tagType)
        {
            return !TryGetTagName(tagType, out string? name)
                ? $"Unknown tag (0x{tagType:x4})"
                : name;
        }

        /// <summary>Gets whether the specified tag is known by the directory and has a name.</summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>whether this directory has a name for the specified tag</returns>
        public bool HasTagName(int tagType) => TryGetTagName(tagType, out _);

        /// <summary>
        /// Provides a description of a tag's value using the descriptor set by <see cref="SetDescriptor"/>.
        /// </summary>
        /// <param name="tagType">the tag type identifier</param>
        /// <returns>the tag value's description as a String</returns>
        public string? GetDescription(int tagType)
        {
            Debug.Assert(_descriptor != null);
            return _descriptor!.GetDescription(tagType);
        }

        public override string ToString() => $"{Name} Directory ({_tagMap.Count} {(_tagMap.Count == 1 ? "tag" : "tags")})";
    }

    /// <summary>
    /// A directory to use for the reporting of errors. No values may be added to this directory, only warnings and errors.
    /// </summary>
    public sealed class ErrorDirectory : Directory
    {
        public override string Name => "Error";

        public ErrorDirectory() : base(new Dictionary<int, string>()) { }

        public ErrorDirectory(string error) : this() => AddError(error);

        public override void Set(int tagType, object value) => throw new NotSupportedException($"Cannot add values to {nameof(ErrorDirectory)}.");
    }
}
