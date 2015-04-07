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
using System.IO;
using Com.Drew.Metadata.Bmp;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Imaging.Bmp
{
	/// <summary>Obtains metadata from BMP files.</summary>
	/// <author>Drew Noakes https://drewnoakes.com</author>
	public class BmpMetadataReader
	{
		/// <exception cref="System.IO.IOException"/>
		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata([NotNull] FilePath file)
		{
			FileInputStream stream = null;
			try
			{
				stream = new FileInputStream(file);
				return ReadMetadata(stream);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		[NotNull]
		public static Com.Drew.Metadata.Metadata ReadMetadata([NotNull] InputStream inputStream)
		{
			Com.Drew.Metadata.Metadata metadata = new Com.Drew.Metadata.Metadata();
			new BmpReader().Extract(new Com.Drew.Lang.StreamReader(inputStream), metadata);
			return metadata;
		}
	}
}
