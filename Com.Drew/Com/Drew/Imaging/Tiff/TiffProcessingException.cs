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
using Com.Drew.Imaging;
using Sharpen;

namespace Com.Drew.Imaging.Tiff
{
	/// <summary>An exception class thrown upon unexpected and fatal conditions while processing a TIFF file.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	/// <author>Darren Salomons</author>
	[System.Serializable]
	public class TiffProcessingException : ImageProcessingException
	{
		private const long serialVersionUID = -1658134119488001891L;

		public TiffProcessingException(string message)
			: base(message)
		{
		}

		public TiffProcessingException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public TiffProcessingException(Exception cause)
			: base(cause)
		{
		}
	}
}
