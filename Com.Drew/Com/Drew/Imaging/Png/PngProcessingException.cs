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
using Com.Drew.Imaging;
using Sharpen;

namespace Com.Drew.Imaging.Png
{
	/// <summary>An exception class thrown upon unexpected and fatal conditions while processing a JPEG file.</summary>
	/// <author>Drew Noakes http://drewnoakes.com</author>
	[System.Serializable]
	public class PngProcessingException : ImageProcessingException
	{
		private const long serialVersionUID = -687991554932005033L;

		public PngProcessingException(string message)
			: base(message)
		{
		}

		public PngProcessingException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngProcessingException(Exception cause)
			: base(cause)
		{
		}
	}
}
