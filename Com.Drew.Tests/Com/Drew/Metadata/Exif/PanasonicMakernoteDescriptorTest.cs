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
using Com.Drew.Metadata;
using Com.Drew.Metadata.Exif;
using Com.Drew.Metadata.Exif.Makernotes;
using Sharpen;

namespace Com.Drew.Metadata.Exif
{
	/// <author>psandhaus, Drew Noakes</author>
	public class PanasonicMakernoteDescriptorTest
	{
		private PanasonicMakernoteDirectory _panasonicDirectory;

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			_panasonicDirectory = ExifReaderTest.ProcessBytes<PanasonicMakernoteDirectory>("Tests/Data/withPanasonicFaces.jpg.app1");
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetDetectedFaces()
		{
			Face expResult = new Face(142, 120, 76, 76, null, null);
			Face[] result = _panasonicDirectory.GetDetectedFaces();
			NUnit.Framework.Assert.IsNotNull(result);
			Sharpen.Tests.AreEqual(expResult, result[0]);
		}

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public virtual void TestGetRecognizedFaces()
		{
			Face expResult = new Face(142, 120, 76, 76, "NIELS", new Age(31, 7, 15, 0, 0, 0));
			Face[] result = _panasonicDirectory.GetRecognizedFaces();
			NUnit.Framework.Assert.IsNotNull(result);
			Sharpen.Tests.AreEqual(expResult, result[0]);
		}
	}
}
