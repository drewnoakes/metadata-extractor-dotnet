#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using Xunit;

namespace MetadataExtractor.Tests
{
    /// <summary>Unit tests for <see cref="GeoLocation"/>.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class GeoLocationTest
    {
        [Fact]
        public void DecimalToDegreesMinutesSeconds()
        {
            var dms = GeoLocation.DecimalToDegreesMinutesSeconds(1);
            Assert.Equal(1.0, dms[0], 4);
            Assert.Equal(0.0, dms[1], 4);
            Assert.Equal(0.0, dms[2], 4);

            dms = GeoLocation.DecimalToDegreesMinutesSeconds(-12.3216);
            Assert.Equal(-12.0, dms[0], 4);
            Assert.Equal(19.0, dms[1], 4);
            Assert.Equal(17.76, dms[2], 4);

            dms = GeoLocation.DecimalToDegreesMinutesSeconds(32.698);
            Assert.Equal(32.0, dms[0], 4);
            Assert.Equal(41.0, dms[1], 4);
            Assert.Equal(52.8, dms[2], 4);
        }
    }
}
