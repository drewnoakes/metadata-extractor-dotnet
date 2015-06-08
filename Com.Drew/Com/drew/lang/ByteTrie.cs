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
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <summary>Stores values using a prefix tree (aka 'trie', i.e.</summary>
    /// <remarks>Stores values using a prefix tree (aka 'trie', i.e. reTRIEval data structure).</remarks>
    /// <?/>
    public class ByteTrie<T>
    {
        /// <summary>A node in the trie.</summary>
        /// <remarks>A node in the trie. Has children and may have an associated value.</remarks>
        internal class ByteTrieNode<T>
        {
            internal readonly IDictionary<sbyte, ByteTrieNode<T>> Children = new Dictionary<sbyte, ByteTrieNode<T>>();

            internal T Value = default(T);

            public virtual void SetValue(T value)
            {
                if (Value != null)
                {
                    throw new RuntimeException("Value already set for this trie node");
                }
                Value = value;
            }
        }

        private readonly ByteTrieNode<T> _root = new ByteTrieNode<T>();

        private int _maxDepth;

        /// <summary>Return the most specific value stored for this byte sequence.</summary>
        /// <remarks>
        /// Return the most specific value stored for this byte sequence.
        /// If not found, returns <code>null</code> or a default values as specified by
        /// calling <see cref="SetDefaultValue"/>.
        /// </remarks>
        [CanBeNull]
        public virtual T Find(sbyte[] bytes)
        {
            ByteTrieNode<T> node = _root;
            T value = node.Value;
            foreach (sbyte b in bytes)
            {
                ByteTrieNode<T> child = Extensions.Get<sbyte, ByteTrieNode<T>>(node.Children, b);
                if (child == null)
                {
                    break;
                }
                node = child;
                if (node.Value != null)
                {
                    value = node.Value;
                }
            }
            return value;
        }

        /// <summary>Store the given value at the specified path.</summary>
        public virtual void AddPath(T value, params sbyte[][] parts)
        {
            int depth = 0;
            ByteTrieNode<T> node = _root;
            foreach (sbyte[] part in parts)
            {
                foreach (sbyte b in part)
                {
                    ByteTrieNode<T> child = Extensions.Get<sbyte, ByteTrieNode<T>>(node.Children, b);
                    if (child == null)
                    {
                        child = new ByteTrieNode<T>();
                        Extensions.Put<sbyte, ByteTrieNode<T>>(node.Children, b, child);
                    }
                    node = child;
                    depth++;
                }
            }
            node.SetValue(value);
            _maxDepth = Math.Max(_maxDepth, depth);
        }

        /// <summary>
        /// Sets the default value to use in
        /// <see cref="ByteTrie{T}.Find(sbyte[])"/>
        /// when no path matches.
        /// </summary>
        public virtual void SetDefaultValue(T defaultValue)
        {
            _root.SetValue(defaultValue);
        }

        /// <summary>Gets the maximum depth stored in this trie.</summary>
        public virtual int GetMaxDepth()
        {
            return _maxDepth;
        }
    }
}
