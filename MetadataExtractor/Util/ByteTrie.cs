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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace MetadataExtractor.Util
{
    /// <summary>Stores values using a prefix tree (aka 'trie', i.e. reTRIEval data structure).</summary>
    public sealed class ByteTrie<T> : IEnumerable<T>
    {
        /// <summary>A node in the trie.</summary>
        /// <remarks>Has children and may have an associated value.</remarks>
        private sealed class ByteTrieNode
        {
            public readonly IDictionary<byte, ByteTrieNode> Children = new Dictionary<byte, ByteTrieNode>();

            public T Value { get; private set; }
            public bool HasValue { get; private set; }

            public void SetValue(T value)
            {
                Debug.Assert(!HasValue, "Value already set for this trie node");
                Value = value;
                HasValue = true;
            }
        }

        private readonly ByteTrieNode _root = new ByteTrieNode();

        /// <summary>Gets the maximum depth stored in this trie.</summary>
        public int MaxDepth { get; private set; }

        public ByteTrie() {}

        public ByteTrie(T defaultValue) => SetDefaultValue(defaultValue);

        /// <summary>Return the most specific value stored for this byte sequence.</summary>
        /// <remarks>
        /// If not found, returns <c>null</c> or a default values as specified by
        /// calling <see cref="SetDefaultValue"/>.
        /// </remarks>
        [CanBeNull]
        [SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Global")]
        public T Find([NotNull] byte[] bytes)
        {
            var node = _root;
            var value = node.Value;
            foreach (var b in bytes)
            {
                if (!node.Children.TryGetValue(b, out node))
                    break;
                if (node.HasValue)
                    value = node.Value;
            }
            return value;
        }

        /// <summary>Store the given value at the specified path.</summary>
        public void Add(T value, [NotNull] params byte[][] parts)
        {
            var depth = 0;
            var node = _root;
            foreach (var part in parts)
            {
                foreach (var b in part)
                {
                    if (!node.Children.TryGetValue(b, out ByteTrieNode child))
                    {
                        child = new ByteTrieNode();
                        node.Children[b] = child;
                    }
                    node = child;
                    depth++;
                }
            }
            node.SetValue(value);
            MaxDepth = Math.Max(MaxDepth, depth);
        }

        /// <summary>
        /// Sets the default value to use in <see cref="ByteTrie{T}.Find(byte[])"/> when no path matches.
        /// </summary>
        public void SetDefaultValue(T defaultValue) => _root.SetValue(defaultValue);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}
