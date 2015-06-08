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

namespace Com.Drew.Lang
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public class GeoLocationTest
    {
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestDecimalToDegreesMinutesSeconds()
        {
            double[] dms = GeoLocation.DecimalToDegreesMinutesSeconds(1);
            Sharpen.Tests.AreEqual(1.0, dms[0], 0.0001);
            Sharpen.Tests.AreEqual(0.0, dms[1], 0.0001);
            Sharpen.Tests.AreEqual(0.0, dms[2], 0.0001);
            dms = GeoLocation.DecimalToDegreesMinutesSeconds(-12.3216);
            Sharpen.Tests.AreEqual(-12.0, dms[0], 0.0001);
            Sharpen.Tests.AreEqual(19.0, dms[1], 0.0001);
            Sharpen.Tests.AreEqual(17.76, dms[2], 0.0001);
            dms = GeoLocation.DecimalToDegreesMinutesSeconds(32.698);
            Sharpen.Tests.AreEqual(32.0, dms[0], 0.0001);
            Sharpen.Tests.AreEqual(41.0, dms[1], 0.0001);
            Sharpen.Tests.AreEqual(52.8, dms[2], 0.0001);
        }
    }
}
