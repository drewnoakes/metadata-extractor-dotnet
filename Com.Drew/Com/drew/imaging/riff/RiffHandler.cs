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
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Riff
{
	/// <summary>
	/// Interface of an class capable of handling events raised during the reading of a RIFF file
	/// via
	/// <see cref="RiffReader"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public interface RiffHandler
	{
		/// <summary>Gets whether the specified RIFF identifier is of interest to this handler.</summary>
		/// <remarks>
		/// Gets whether the specified RIFF identifier is of interest to this handler.
		/// Returning <code>false</code> causes processing to stop after reading only
		/// the first twelve bytes of data.
		/// </remarks>
		/// <param name="identifier">The four character code identifying the type of RIFF data</param>
		/// <returns>true if processing should continue, otherwise false</returns>
		bool ShouldAcceptRiffIdentifier([NotNull] string identifier);

		/// <summary>Gets whether this handler is interested in the specific chunk type.</summary>
		/// <remarks>
		/// Gets whether this handler is interested in the specific chunk type.
		/// Returns <code>true</code> if the data should be copied into an array and passed
		/// to
		/// <see cref="ProcessChunk(string, sbyte[])"/>
		/// , or <code>false</code> to avoid
		/// the copy and skip to the next chunk in the file, if any.
		/// </remarks>
		/// <param name="fourCC">the four character code of this chunk</param>
		/// <returns>
		/// true if
		/// <see cref="ProcessChunk(string, sbyte[])"/>
		/// should be called, otherwise false
		/// </returns>
		bool ShouldAcceptChunk([NotNull] string fourCC);

		/// <summary>
		/// Perform whatever processing is necessary for the type of chunk with its
		/// payload.
		/// </summary>
		/// <remarks>
		/// Perform whatever processing is necessary for the type of chunk with its
		/// payload.
		/// This is only called if a previous call to
		/// <see cref="ShouldAcceptChunk(string)"/>
		/// with the same <code>fourCC</code> returned <code>true</code>.
		/// </remarks>
		/// <param name="fourCC">the four character code of the chunk</param>
		/// <param name="payload">they payload of the chunk as a byte array</param>
		void ProcessChunk([NotNull] string fourCC, [NotNull] sbyte[] payload);
	}
}
