// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections;

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

        private readonly ByteTrieNode _root = new();

        /// <summary>Gets the maximum depth stored in this trie.</summary>
        public int MaxDepth { get; private set; }

        /// <summary>
        /// Initialises a new instance of <see cref="ByteTrie{T}"/> with specified default value.
        /// </summary>
        /// <param name="defaultValue">
        /// The default value to use in <see cref="ByteTrie{T}.Find(byte[])"/> when no path matches.
        /// </param>
        public ByteTrie(T defaultValue) => _root.SetValue(defaultValue);

        /// <summary>Return the most specific value stored for this byte sequence.</summary>
        /// <remarks>
        /// If not found, returns the default value specified in the constructor.
        /// </remarks>
        public T Find(byte[] bytes) => Find(bytes, 0, bytes.Length);

        /// <summary>Return the most specific value stored for this byte sequence.</summary>
        /// <remarks>
        /// If not found, returns the default value specified in the constructor.
        /// </remarks>
        public T Find(byte[] bytes, int offset, int count)
        {
            var maxIndex = offset + count;
            if (maxIndex > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset and length are not in bounds for byte array.");

            var node = _root;
            var value = node.Value;
            for (var i = offset; i < maxIndex; i++)
            {
                var b = bytes[i];
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
                    if (!node.Children.TryGetValue(b, out ByteTrieNode? child))
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

        /// <summary>Store the given value at the specified path.</summary>
        public void Add(T value, ReadOnlySpan<byte> part)
        {
            var depth = 0;
            var node = _root;

            foreach (var b in part)
            {
                if (!node.Children.TryGetValue(b, out ByteTrieNode? child))
                {
                    child = new ByteTrieNode();
                    node.Children[b] = child;
                }
                node = child;
                depth++;
            }

            node.SetValue(value);
            MaxDepth = Math.Max(MaxDepth, depth);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    }
}
