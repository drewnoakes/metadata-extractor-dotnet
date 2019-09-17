// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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

            public T Value { get; private set; } = default!;
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
        [return: MaybeNull]
        [SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Global")]
        public T Find(byte[] bytes)
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
        public void Add(T value, params byte[][] parts)
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
