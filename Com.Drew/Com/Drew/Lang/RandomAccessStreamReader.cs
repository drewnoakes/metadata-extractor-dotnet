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
using System;
using System.IO;
using Com.Drew.Lang;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Lang
{
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class RandomAccessStreamReader : RandomAccessReader
	{
		private const int DefaultChunkLength = 2 * 1024;

		[NotNull]
		private readonly InputStream _stream;

		private readonly int _chunkLength;

		private readonly AList<sbyte[]> _chunks = new AList<sbyte[]>();

		private bool _isStreamFinished;

		private int _streamLength;

		public RandomAccessStreamReader(InputStream stream)
			: this(stream, DefaultChunkLength)
		{
		}

		public RandomAccessStreamReader(InputStream stream, int chunkLength)
		{
			if (stream == null)
			{
				throw new ArgumentNullException();
			}
			if (chunkLength <= 0)
			{
				throw new ArgumentException("chunkLength must be greater than zero");
			}
			_chunkLength = chunkLength;
			_stream = stream;
		}

		/// <summary>Reads to the end of the stream, in order to determine the total number of bytes.</summary>
		/// <remarks>
		/// Reads to the end of the stream, in order to determine the total number of bytes.
		/// In general, this is not a good idea for this implementation of
		/// <see cref="RandomAccessReader"/>
		/// .
		/// </remarks>
		/// <returns>the length of the data source, in bytes.</returns>
		/// <exception cref="System.IO.IOException"/>
		public override long GetLength()
		{
			IsValidIndex(int.MaxValue, 1);
			System.Diagnostics.Debug.Assert((_isStreamFinished));
			return _streamLength;
		}

		/// <summary>Ensures that the buffered bytes extend to cover the specified index.</summary>
		/// <remarks>
		/// Ensures that the buffered bytes extend to cover the specified index. If not, an attempt is made
		/// to read to that point.
		/// <p/>
		/// If the stream ends before the point is reached, a
		/// <see cref="BufferBoundsException"/>
		/// is raised.
		/// </remarks>
		/// <param name="index">the index from which the required bytes start</param>
		/// <param name="bytesRequested">the number of bytes which are required</param>
		/// <exception cref="BufferBoundsException">if the stream ends before the required number of bytes are acquired</exception>
		/// <exception cref="System.IO.IOException"/>
		protected internal override void ValidateIndex(int index, int bytesRequested)
		{
			if (index < 0)
			{
				throw new BufferBoundsException(Sharpen.Extensions.StringFormat("Attempt to read from buffer using a negative index (%d)", index));
			}
			else
			{
				if (bytesRequested < 0)
				{
					throw new BufferBoundsException("Number of requested bytes must be zero or greater");
				}
				else
				{
					if ((long)index + bytesRequested - 1 > int.MaxValue)
					{
						throw new BufferBoundsException(Sharpen.Extensions.StringFormat("Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: %d, requested count: %d)", index, bytesRequested));
					}
				}
			}
			if (!IsValidIndex(index, bytesRequested))
			{
				System.Diagnostics.Debug.Assert((_isStreamFinished));
				// TODO test that can continue using an instance of this type after this exception
				throw new BufferBoundsException(index, bytesRequested, _streamLength);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		protected internal override bool IsValidIndex(int index, int bytesRequested)
		{
			if (index < 0 || bytesRequested < 0)
			{
				return false;
			}
			long endIndexLong = (long)index + bytesRequested - 1;
			if (endIndexLong > int.MaxValue)
			{
				return false;
			}
			int endIndex = (int)endIndexLong;
			if (_isStreamFinished)
			{
				return endIndex < _streamLength;
			}
			int chunkIndex = endIndex / _chunkLength;
			// TODO test loading several chunks for a single request
			while (chunkIndex >= _chunks.Count)
			{
				System.Diagnostics.Debug.Assert((!_isStreamFinished));
				sbyte[] chunk = new sbyte[_chunkLength];
				int totalBytesRead = 0;
				while (!_isStreamFinished && totalBytesRead != _chunkLength)
				{
					int bytesRead = _stream.Read(chunk, totalBytesRead, _chunkLength - totalBytesRead);
					if (bytesRead == -1)
					{
						// the stream has ended, which may be ok
						_isStreamFinished = true;
						_streamLength = _chunks.Count * _chunkLength + totalBytesRead;
						// check we have enough bytes for the requested index
						if (endIndex >= _streamLength)
						{
							_chunks.Add(chunk);
							return false;
						}
					}
					else
					{
						totalBytesRead += bytesRead;
					}
				}
				_chunks.Add(chunk);
			}
			return true;
		}

		/// <exception cref="System.IO.IOException"/>
		protected internal override sbyte GetByte(int index)
		{
			System.Diagnostics.Debug.Assert((index >= 0));
			int chunkIndex = index / _chunkLength;
			int innerIndex = index % _chunkLength;
			sbyte[] chunk = _chunks[chunkIndex];
			return chunk[innerIndex];
		}

		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public override sbyte[] GetBytes(int index, int count)
		{
			ValidateIndex(index, count);
			sbyte[] bytes = new sbyte[count];
			int remaining = count;
			int fromIndex = index;
			int toIndex = 0;
			while (remaining != 0)
			{
				int fromChunkIndex = fromIndex / _chunkLength;
				int fromInnerIndex = fromIndex % _chunkLength;
				int length = Math.Min(remaining, _chunkLength - fromInnerIndex);
				sbyte[] chunk = _chunks[fromChunkIndex];
				System.Array.Copy(chunk, fromInnerIndex, bytes, toIndex, length);
				remaining -= length;
				fromIndex += length;
				toIndex += length;
			}
			return bytes;
		}
	}
}
