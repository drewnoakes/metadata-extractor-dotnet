// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace MetadataExtractor.Tests.Formats.Apple;

public sealed class LivePhotoMovTest
{
    private const string TestFile = "Data/apple-livephoto-quicktime.mov";

    [Fact]
    public void GetLivePhotoQuickTimeInfo()
    {
        var exception = Record.Exception(() => ImageMetadataReader.ReadMetadata(TestFile));
        Assert.Null(exception);
    }

    [Fact]
    public void GetDirectoryNames()
    {
        var directories =ImageMetadataReader.ReadMetadata(TestFile);
        Assert.NotNull(directories);
        Assert.NotEmpty(directories);
        var names = directories.Select(d => d.Name).ToArray();
        Assert.Contains("QuickTime Movie Header", names);
        Assert.Contains("QuickTime Track Header", names);
        Assert.Contains("QuickTime Metadata Header", names);
        Assert.Contains("File Type", names);
        Assert.Contains("File", names);
    }

    /// <summary>
    /// Live Photos is actually two files. Original JPEG/HEIF Image and Full HD Video(QuickTime).
    /// in the ios ecology, live photo connect image with mov with same name, and meta key `Content Identifier`
    /// </summary>
    [Fact]
    public void GetLivePhotoContentIdentifier()
    {
        var directories =ImageMetadataReader.ReadMetadata(TestFile);
        var quickTimeMetadataHeaderDirectory = directories.Where(d => d.Name == "QuickTime Metadata Header")!.First();
        var contentIdentifierTag = quickTimeMetadataHeaderDirectory.Tags.Where(t => t.Name == "Content Identifier")!.First();
        Assert.Equal("EE6D649E-788F-4E3A-BCD2-483651BF7B34", contentIdentifierTag.Description);
    }

}
