/*
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
using Com.Drew.Lang;
using Com.Drew.Metadata;
using Sharpen;

namespace Com.Drew.Metadata
{
	/// <summary>
	/// Defines an object capable of processing a particular type of metadata from a
	/// <see cref="Com.Drew.Lang.RandomAccessReader"/>
	/// .
	/// <p/>
	/// Instances of this interface must be thread-safe and reusable.
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public interface MetadataReader
	{
		/// <summary>
		/// Extracts metadata from <code>reader</code> and merges it into the specified
		/// <see cref="Metadata"/>
		/// object.
		/// </summary>
		/// <param name="reader">
		/// The
		/// <see cref="Com.Drew.Lang.RandomAccessReader"/>
		/// from which the metadata should be extracted.
		/// </param>
		/// <param name="metadata">
		/// The
		/// <see cref="Metadata"/>
		/// object into which extracted values should be merged.
		/// </param>
		void Extract(RandomAccessReader reader, Com.Drew.Metadata.Metadata metadata);
	}
}
