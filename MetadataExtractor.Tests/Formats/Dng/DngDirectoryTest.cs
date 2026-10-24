// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Reflection;
using MetadataExtractor.Formats.Exif;

namespace MetadataExtractor.Formats.Dng;

public sealed class DngDirectoryTest
{
    [Fact]
    public void PublicTagConstantsAreUniqueNamedAndSpecificToDng()
    {
        var directory = new DngDirectory();
        var fields = typeof(DngDirectory).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.IsLiteral && field.FieldType == typeof(int) && field.Name.StartsWith("Tag", StringComparison.Ordinal))
            .ToArray();
        var ids = fields.Select(field => (int)field.GetRawConstantValue()!).ToArray();
        Assert.NotEmpty(ids);
        Assert.Equal(ids.Length, ids.Distinct().Count());
        Assert.Equal("DNG", directory.Name);
        Assert.True(directory.IsEmpty);
        Directory[] exifDirectories =
        [
            new ExifIfd0Directory(), new ExifSubIfdDirectory(), new ExifThumbnailDirectory(0),
            new ExifImageDirectory(), new ExifInteropDirectory(), new GpsDirectory()
        ];
        foreach (var id in ids)
        {
            Assert.True(directory.HasTagName(id), $"Missing name for DNG tag 0x{id:X4}");
            Assert.DoesNotContain("Unknown", directory.GetTagName(id));
            foreach (var exif in exifDirectories)
                Assert.False(exif.HasTagName(id), $"{exif.Name} incorrectly names DNG tag 0x{id:X4}");
        }
        Assert.False(directory.HasTagName(0xC7FF));
    }

    [Theory]
    [InlineData(DngDirectory.TagAsShotNeutral, 0xC628, "As Shot Neutral")]
    [InlineData(DngDirectory.TagAsShotWhiteXY, 0xC629, "As Shot White XY")]
    [InlineData(DngDirectory.TagBayerGreenSplit, 0xC62D, "Bayer Green Split")]
    [InlineData(DngDirectory.TagLinearResponseLimit, 0xC62E, "Linear Response Limit")]
    [InlineData(DngDirectory.TagDngPrivateData, 0xC634, "DNG Private Data")]
    [InlineData(DngDirectory.TagMakerNoteSafety, 0xC635, "Maker Note Safety")]
    public void UsesSpecificationTagIdsAndDistinctNames(int tag, int expectedId, string expectedName)
    {
        Assert.Equal(expectedId, tag);
        Assert.Equal(expectedName, new DngDirectory().GetTagName(tag));
    }
}
