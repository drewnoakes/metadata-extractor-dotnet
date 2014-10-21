/*
 * Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#) 
 * Copyright 2002-2013 Drew Noakes
 *
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
 *    http://drewnoakes.com/code/exif/
 *    http://code.google.com/p/metadata-extractor/
 */
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Metadata
{
	/// <summary>A top-level object that holds the metadata values extracted from an image.</summary>
	/// <remarks>
	/// A top-level object that holds the metadata values extracted from an image.
	/// <p/>
	/// Metadata objects may contain zero or more
	/// <see cref="Directory"/>
	/// objects.  Each directory may contain zero or more tags
	/// with corresponding values.
	/// </remarks>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public sealed class Metadata
	{
		[NotNull]
		private readonly IDictionary<Type, Com.Drew.Metadata.Directory> _directoryByClass = new Dictionary<Type, Com.Drew.Metadata.Directory>();

		/// <summary>List of Directory objects set against this object.</summary>
		/// <remarks>
		/// List of Directory objects set against this object.  Keeping a list handy makes
		/// creation of an Iterator and counting tags simple.
		/// </remarks>
		[NotNull]
		private readonly ICollection<Com.Drew.Metadata.Directory> _directoryList = new AList<Com.Drew.Metadata.Directory>();

		/// <summary>Returns an objects for iterating over Directory objects in the order in which they were added.</summary>
		/// <returns>an iterable collection of directories</returns>
		[NotNull]
		public Iterable<Com.Drew.Metadata.Directory> GetDirectories()
		{
			return _directoryList.AsIterable();
		}

		/// <summary>Returns a count of unique directories in this metadata collection.</summary>
		/// <returns>the number of unique directory types set for this metadata collection</returns>
		public int GetDirectoryCount()
		{
			return _directoryList.Count;
		}

        /// <summary>
        /// Returns a
        /// <see cref="Directory"/>
        /// of specified type.  If this
        /// <see cref="Metadata"/>
        /// object already contains
        /// such a directory, it is returned.  Otherwise a new instance of this directory will be created and stored within
        /// this
        /// <see cref="Metadata"/>
        /// object.
        /// </summary>
        /// <param name="type">the type of the Directory implementation required.</param>
        /// <returns>a directory of the specified type.</returns>
        /// HACK: this method is absent in core library. It was converted to GetOrCreateDirectory<T>()
        [NotNull]
        public Directory GetOrCreateDirectory(Type type)
        {
            // We suppress the warning here as the code asserts a map signature of Class<T>,T.
            // So after get(Class<T>) it is for sure the result is from type T.
            // check if we've already issued this type of directory
            if (_directoryByClass.ContainsKey(type))
            {
                return _directoryByClass.Get(type);
            }
            Directory directory;
            try
            {
                directory = (Directory)System.Activator.CreateInstance(type);
            }
            catch (Exception)
            {
                throw new RuntimeException("Cannot instantiate provided Directory type: " + type.ToString());
            }
            // store the directory
            _directoryByClass.Put(type, directory);
            _directoryList.Add(directory);
            return directory;
        }

		/// <summary>
		/// Returns a
		/// <see cref="Directory"/>
		/// of specified type.  If this
		/// <see cref="Metadata"/>
		/// object already contains
		/// such a directory, it is returned.  Otherwise a new instance of this directory will be created and stored within
		/// this
		/// <see cref="Metadata"/>
		/// object.
		/// </summary>
        /// <typeparam name="T">the type of the Directory implementation required.</typeparam>
		/// <returns>a directory of the specified type.</returns>
		[NotNull]
		public T GetOrCreateDirectory<T>()
			where T : Com.Drew.Metadata.Directory
		{
			System.Type type = typeof(T);
			// We suppress the warning here as the code asserts a map signature of Class<T>,T.
			// So after get(Class<T>) it is for sure the result is from type T.
			// check if we've already issued this type of directory
			if (_directoryByClass.ContainsKey(type))
			{
				return (T)_directoryByClass.Get(type);
			}
			T directory;
			try
			{
				directory = (T) System.Activator.CreateInstance(type);
			}
			catch (Exception)
			{
				throw new RuntimeException("Cannot instantiate provided Directory type: " + type.ToString());
			}
			// store the directory
			_directoryByClass.Put(type, directory);
			_directoryList.Add(directory);
			return directory;
		}

		/// <summary>
		/// If this
		/// <see cref="Metadata"/>
		/// object contains a
		/// <see cref="Directory"/>
		/// of the specified type, it is returned.
		/// Otherwise <code>null</code> is returned.
		/// </summary>
        /// <typeparam name="T">the Directory type</typeparam>
		/// <returns>
		/// a Directory of type T if it exists in this
		/// <see cref="Metadata"/>
		/// object, otherwise <code>null</code>.
		/// </returns>
		[CanBeNull]
		public T GetDirectory<T>()
			where T : Com.Drew.Metadata.Directory
		{
			System.Type type = typeof(T);
			// We suppress the warning here as the code asserts a map signature of Class<T>,T.
			// So after get(Class<T>) it is for sure the result is from type T.
			return (T)_directoryByClass.Get(type);
		}

		/// <summary>
		/// Indicates whether a given directory type has been created in this metadata
		/// repository.
		/// </summary>
		/// <remarks>
		/// Indicates whether a given directory type has been created in this metadata
		/// repository.  Directories are created by calling <code>getOrCreateDirectory(Class)</code>.
		/// </remarks>
		/// <typeparam name="T">
		/// the
		/// <see cref="Directory"/>
		/// type
        /// </typeparam>
		/// <returns>
		/// true if the
		/// <see cref="Directory"/>
		/// has been created
		/// </returns>
		public bool ContainsDirectory<T>()
			where T : Com.Drew.Metadata.Directory
		{
			return _directoryByClass.ContainsKey(typeof(T));
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
			foreach (Com.Drew.Metadata.Directory directory in _directoryList)
			{
				if (directory.HasErrors())
				{
					return true;
				}
			}
			return false;
		}
	}
}
