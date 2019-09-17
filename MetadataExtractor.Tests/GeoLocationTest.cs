// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
