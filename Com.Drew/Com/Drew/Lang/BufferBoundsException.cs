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
using System.IO;
using Sharpen;

namespace Com.Drew.Lang
{
	/// <summary>
	/// A checked replacement for
	/// <see cref="System.IndexOutOfRangeException"/>
	/// .  Used by
	/// <see cref="RandomAccessReader"/>
	/// .
	/// </summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	[System.Serializable]
	public sealed class BufferBoundsException : IOException
	{
		private const long serialVersionUID = 2911102837808946396L;

		public BufferBoundsException(int index, int bytesRequested, long bufferLength)
			: base(GetMessage(index, bytesRequested, bufferLength))
		{
		}

		public BufferBoundsException(string message)
			: base(message)
		{
		}

		private static string GetMessage(int index, int bytesRequested, long bufferLength)
		{
			if (index < 0)
			{
				return Sharpen.Extensions.StringFormat("Attempt to read from buffer using a negative index (%d)", index);
			}
			if (bytesRequested < 0)
			{
                return Sharpen.Extensions.StringFormat("Number of requested bytes cannot be negative (%d)", bytesRequested);
			}
			if ((long)index + (long)bytesRequested - 1L > (long)int.MaxValue)
			{
                return Sharpen.Extensions.StringFormat("Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: %d, requested count: %d)", index, bytesRequested);
			}
            return Sharpen.Extensions.StringFormat("Attempt to read from beyond end of underlying data source (requested index: %d, requested count: %d, max index: %d)", index, bytesRequested, bufferLength - 1);
		}
	}
}
