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
using Sharpen;

namespace Com.Drew.Metadata.Jpeg
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class JpegComponentTest
    {
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestGetComponentCharacter()
        {
            JpegComponent component;
            component = new JpegComponent(1, 2, 3);
            Sharpen.Tests.AreEqual("Y", component.GetComponentName());
            component = new JpegComponent(2, 2, 3);
            Sharpen.Tests.AreEqual("Cb", component.GetComponentName());
            component = new JpegComponent(3, 2, 3);
            Sharpen.Tests.AreEqual("Cr", component.GetComponentName());
            component = new JpegComponent(4, 2, 3);
            Sharpen.Tests.AreEqual("I", component.GetComponentName());
            component = new JpegComponent(5, 2, 3);
            Sharpen.Tests.AreEqual("Q", component.GetComponentName());
        }
    }
}
