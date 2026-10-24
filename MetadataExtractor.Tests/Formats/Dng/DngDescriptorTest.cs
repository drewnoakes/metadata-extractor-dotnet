// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Dng;

public sealed class DngDescriptorTest
{
    [Theory]
    [CombinatorialData]
    public void DescribesAllFourVersionComponents(
        [CombinatorialValues(DngDirectory.TagDngVersion, DngDirectory.TagDngBackwardVersion)] int tag)
    {
        var directory = new DngDirectory();
        directory.Set(tag, new byte[] { 1, 4, 7, 255 });

        Assert.Equal("1.4.7.255", directory.GetDescription(tag));
    }

    [Theory]
    [CombinatorialData]
    public void MissingOrMalformedVersionsHaveNoDescription(
        [CombinatorialValues(DngDirectory.TagDngVersion, DngDirectory.TagDngBackwardVersion)] int tag,
        [CombinatorialValues(-1, 3, 5)] int length)
    {
        var directory = new DngDirectory();
        if (length >= 0)
            directory.Set(tag, Enumerable.Repeat((byte)1, length).ToArray());

        Assert.Null(directory.GetDescription(tag));
    }
}
