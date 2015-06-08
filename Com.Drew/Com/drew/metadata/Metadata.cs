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
using System.Linq;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata
{
    /// <summary>A top-level object that holds the metadata values extracted from an image.</summary>
    /// <remarks>
    /// A top-level object that holds the metadata values extracted from an image.
    /// <p>
    /// Metadata objects may contain zero or more
    /// <see cref="Directory"/>
    /// objects.  Each directory may contain zero or more tags
    /// with corresponding values.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class Metadata
    {
        [NotNull]
        private readonly IDictionary<Type, ICollection<Directory>> _directoryListByClass = new Dictionary<Type, ICollection<Directory>>();

        /// <summary>
        /// Returns an iterable set of the
        /// <see cref="Directory"/>
        /// instances contained in this metadata collection.
        /// </summary>
        /// <returns>an iterable set of directories</returns>
        [NotNull]
        public Iterable<Directory> GetDirectories()
        {
            return new DirectoryIterable(_directoryListByClass);
        }

        [CanBeNull]
        public ICollection<T> GetDirectoriesOfType<T>()
            where T : Directory
        {
            Type type = typeof(T);
            return (from item in _directoryListByClass.Get(type) select (T) item).ToList();
        }

        /// <summary>Returns the count of directories in this metadata collection.</summary>
        /// <returns>the number of unique directory types set for this metadata collection</returns>
        public int GetDirectoryCount()
        {
            return _directoryListByClass.EntrySet().Sum(pair => pair.Value.Count);
        }

        /// <summary>Adds a directory to this metadata collection.</summary>
        /// <param name="directory">
        /// the
        /// <see cref="Directory"/>
        /// to add into this metadata collection.
        /// </param>
        public void AddDirectory<T>([NotNull] T directory)
            where T : Directory
        {
            GetOrCreateDirectoryList(directory.GetType()).Add(directory);
        }

        /// <summary>
        /// Gets the first
        /// <see cref="Directory"/>
        /// of the specified type contained within this metadata collection.
        /// If no instances of this type are present, <code>null</code> is returned.
        /// </summary>
        /// <param name="type">the Directory type</param>
        /// <?/>
        /// <returns>the first Directory of type T in this metadata collection, or <code>null</code> if none exist</returns>
        [CanBeNull]
        public T GetFirstDirectoryOfType<T>()
            where T : Directory
        {
            Type type = typeof(T);
            // We suppress the warning here as the code asserts a map signature of Class<T>,T.
            // So after get(Class<T>) it is for sure the result is from type T.
            ICollection<Directory> list = GetDirectoryList(type);
            if (list == null || list.IsEmpty())
            {
                return null;
            }
            return (T)list.Iterator().Next();
        }

        /// <summary>Indicates whether an instance of the given directory type exists in this Metadata instance.</summary>
        /// <param name="type">
        /// the
        /// <see cref="Directory"/>
        /// type
        /// </param>
        /// <returns>
        /// <code>true</code> if a
        /// <see cref="Directory"/>
        /// of the specified type exists, otherwise <code>false</code>
        /// </returns>
        public bool ContainsDirectoryOfType(Type type)
        {
            ICollection<Directory> list = GetDirectoryList(type);
            return list != null && !list.IsEmpty();
        }

        /// <summary>Indicates whether any errors were reported during the reading of metadata values.</summary>
        /// <remarks>
        /// Indicates whether any errors were reported during the reading of metadata values.
        /// This value will be true if Directory.hasErrors() is true for one of the contained
        /// <see cref="Directory"/>
        /// objects.
        /// </remarks>
        /// <returns>whether one of the contained directories has an error</returns>
        public bool HasErrors()
        {
            return GetDirectories().Any(directory => directory.HasErrors());
        }

        public override string ToString()
        {
            int count = GetDirectoryCount();
            return Extensions.StringFormat("Metadata (%d %s)", count, count == 1 ? "directory" : "directories");
        }

        [CanBeNull]
        private ICollection<Directory> GetDirectoryList(Type type)
        {
            return _directoryListByClass.Get(type);
        }

        [NotNull]
        private ICollection<Directory> GetOrCreateDirectoryList(Type type)
        {
            ICollection<Directory> collection = GetDirectoryList(type);
            if (collection != null)
            {
                return collection;
            }
            collection = new AList<Directory>();
            _directoryListByClass.Put(type, collection);
            return collection;
        }

        private class DirectoryIterable : Iterable<Directory>
        {
            private readonly IDictionary<Type, ICollection<Directory>> _map;

            public DirectoryIterable(IDictionary<Type, ICollection<Directory>> map)
            {
                _map = map;
            }

            public override Iterator<Directory> Iterator()
            {
                return new DirectoryIterator(_map);
            }

            private class DirectoryIterator : Iterator<Directory>
            {
                [NotNull]
                private readonly Iterator<KeyValuePair<Type, ICollection<Directory>>> _mapIterator;

                [CanBeNull]
                private Iterator<Directory> _listIterator;

                public DirectoryIterator(IDictionary<Type, ICollection<Directory>> map)
                {
                    _mapIterator = map.EntrySet().Iterator();
                    if (_mapIterator.HasNext())
                    {
                        _listIterator = _mapIterator.Next().Value.Iterator();
                    }
                }

                public override bool HasNext()
                {
                    return _listIterator != null && (_listIterator.HasNext() || _mapIterator.HasNext());
                }

                public override Directory Next()
                {
                    if (_listIterator == null || (!_listIterator.HasNext() && !_mapIterator.HasNext()))
                    {
                        throw new NoSuchElementException();
                    }
                    while (!_listIterator.HasNext())
                    {
                        _listIterator = _mapIterator.Next().Value.Iterator();
                    }
                    return _listIterator.Next();
                }

                public override void Remove()
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
