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
using Com.Drew.Tools;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Tools
{
	/// <summary>A series of utility methods for working with the file system.</summary>
	/// <remarks>
	/// A series of utility methods for working with the file system. The methods herein are used in unit testing.
	/// Use them in production code at your own risk!
	/// </remarks>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	public class FileUtil
	{
		/// <summary>
		/// Saves the contents of a <code>byte[]</code> to the specified
		/// <see cref="Sharpen.FilePath"/>
		/// .
		/// </summary>
		/// <exception cref="System.IO.IOException"/>
		public static void SaveBytes(FilePath file, sbyte[] bytes)
		{
			FileOutputStream stream = null;
			try
			{
				stream = new FileOutputStream(file);
				stream.Write(bytes);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		/// <summary>
		/// Reads the contents of a
		/// <see cref="Sharpen.FilePath"/>
		/// into a <code>byte[]</code>. This relies upon <code>File.length()</code>
		/// returning the correct value, which may not be the case when using a network file system. However this method is
		/// intended for unit test support, in which case the files should be on the local volume.
		/// </summary>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static sbyte[] ReadBytes(FilePath file)
		{
			int length = (int)file.Length();
			// should only be zero if loading from a network or similar
			System.Diagnostics.Debug.Assert((length != 0));
			sbyte[] bytes = new sbyte[length];
			int totalBytesRead = 0;
			FileInputStream inputStream = null;
			try
			{
				inputStream = new FileInputStream(file);
				while (totalBytesRead != length)
				{
					int bytesRead = inputStream.Read(bytes, totalBytesRead, length - totalBytesRead);
					if (bytesRead == -1)
					{
						break;
					}
					totalBytesRead += bytesRead;
				}
			}
			finally
			{
				if (inputStream != null)
				{
					inputStream.Close();
				}
			}
			return bytes;
		}

		/// <summary>
		/// Reads the contents of a
		/// <see cref="Sharpen.FilePath"/>
		/// into a <code>byte[]</code>. This relies upon <code>File.length()</code>
		/// returning the correct value, which may not be the case when using a network file system. However this method is
		/// intended for unit test support, in which case the files should be on the local volume.
		/// </summary>
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static sbyte[] ReadBytes(string filePath)
		{
			return ReadBytes(new FilePath(filePath));
		}
	}
}
