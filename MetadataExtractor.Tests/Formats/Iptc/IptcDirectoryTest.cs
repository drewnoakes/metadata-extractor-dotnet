// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using MetadataExtractor.Formats.Iptc;
using Xunit;

namespace MetadataExtractor.Tests.Formats.Iptc
{
    /// <summary>Unit tests for <see cref="IptcDirectory"/>.</summary>
    /// <author>Akihiko Kusanagi https://github.com/nagix</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class IptcDirectoryTest
    {
        private readonly IptcDirectory _directory;

        public IptcDirectoryTest()
        {
            _directory = new IptcDirectory();
        }

        [Fact]
        public void GetDateSent()
        {
            _directory.Set(IptcDirectory.TagDateSent, "20101212");
            _directory.Set(IptcDirectory.TagTimeSent, "124135+0100");

            Assert.Equal(
                new DateTimeOffset(2010, 12, 12, 12, 41, 35, TimeSpan.FromHours(1)),
                _directory.GetDateSent());
        }

        [Fact]
        public void GetReleaseDate()
        {
            _directory.Set(IptcDirectory.TagReleaseDate, "20101212");
            _directory.Set(IptcDirectory.TagReleaseTime, "124135+0100");

            Assert.Equal(
                new DateTimeOffset(2010, 12, 12, 12, 41, 35, TimeSpan.FromHours(1)),
                _directory.GetReleaseDate());
        }

        [Fact]
        public void GetExpirationDate()
        {
            _directory.Set(IptcDirectory.TagExpirationDate, "20101212");
            _directory.Set(IptcDirectory.TagExpirationTime, "124135+0100");

            Assert.Equal(
                new DateTimeOffset(2010, 12, 12, 12, 41, 35, TimeSpan.FromHours(1)),
                _directory.GetExpirationDate());
        }

        [Fact]
        public void GetDateCreated()
        {
            _directory.Set(IptcDirectory.TagDateCreated, "20101212");
            _directory.Set(IptcDirectory.TagTimeCreated, "124135+0100");

            Assert.Equal(
                new DateTimeOffset(2010, 12, 12, 12, 41, 35, TimeSpan.FromHours(1)),
                _directory.GetDateCreated());
        }

        [Fact]
        public void GetDigitalDateCreated()
        {
            _directory.Set(IptcDirectory.TagDigitalDateCreated, "20101212");
            _directory.Set(IptcDirectory.TagDigitalTimeCreated, "124135+0100");

            Assert.Equal(
                new DateTimeOffset(2010, 12, 12, 12, 41, 35, TimeSpan.FromHours(1)),
                _directory.GetDigitalDateCreated());
        }
    }
}
