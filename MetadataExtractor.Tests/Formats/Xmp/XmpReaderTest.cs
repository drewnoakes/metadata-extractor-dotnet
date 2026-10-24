// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers.Binary;
using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Formats.Xmp;

/// <summary>Unit tests for <see cref="XmpReader"/>.</summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public sealed class XmpReaderTest
{
    private const int ExpectedPropertyCount = 167;

    private readonly XmpDirectory _directory;

    public XmpReaderTest()
    {
        var jpegSegments = new[] { new JpegSegment(JpegSegmentType.App1, TestDataUtil.GetBytes("Data/withXmpAndIptc.jpg.app1.1"), offset: 0) };
        var directories = new XmpReader().ReadJpegSegments(jpegSegments);
        _directory = directories.OfType<XmpDirectory>().ToList().Single();
        Assert.False(_directory.HasError);
    }

    [Fact]
    public void Extract_HasXMPMeta()
    {
        Assert.NotNull(_directory.XmpMeta);
    }

    [Fact]
    public void Extract_PropertyCount()
    {
        Assert.Equal(ExpectedPropertyCount, _directory.GetInt32(XmpDirectory.TagXmpValueCount));
    }

    [Fact]
    public void GetXmpProperties()
    {
        var propertyMap = _directory.GetXmpProperties();

        Assert.Equal(ExpectedPropertyCount, propertyMap.Count);

        Assert.True(propertyMap.ContainsKey("photoshop:Country"));
        Assert.Equal("Deutschland", propertyMap["photoshop:Country"]);

        Assert.True(propertyMap.ContainsKey("tiff:ImageLength"));
        Assert.Equal("900", propertyMap["tiff:ImageLength"]);
    }

    [Theory]
    [InlineData(0x7FFFFFF0u, 0u, false)]
    [InlineData(0xFFFFFFFFu, 0u, false)]
    [InlineData(0x7FFFFFF0u, 0xFFFFFFF0u, false)]
    [InlineData(0xFFFFFFFFu, 0xFFFFFFF0u, false)]
    [InlineData(0x7FFFFFF0u, 0u, true)]
    [InlineData(0xFFFFFFFFu, 0u, true)]
    [InlineData(0x7FFFFFF0u, 0xFFFFFFF0u, true)]
    [InlineData(0xFFFFFFFFu, 0xFFFFFFF0u, true)]
    public void ReadJpegSegments_ExtensionParsesActualTinyXmlRegardlessOfAdvertisedLengthAndOffset(
        uint fullLength, uint offset, bool split)
    {
        // .NET concatenates actual payloads, ignoring fullLength and offset. These
        // fields must not become allocation sizes or seek destinations.
        var xml = Encoding.UTF8.GetBytes("""
            <x:xmpmeta xmlns:x="adobe:ns:meta/">
              <rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#">
                <rdf:Description rdf:about="" xmlns:tiff="http://ns.adobe.com/tiff/1.0/"
                                 tiff:ImageWidth="17" tiff:ImageLength="23"/>
              </rdf:RDF>
            </x:xmpmeta>
            """);
        var midpoint = xml.Length / 2;
        JpegSegment[] segments = split
            ? [ExtensionSegment(xml.AsSpan(0, midpoint).ToArray(), fullLength, offset),
               ExtensionSegment(xml.AsSpan(midpoint).ToArray(), fullLength, offset == 0 ? (uint)midpoint : offset)]
            : [ExtensionSegment(xml, fullLength, offset)];
        Assert.All(segments, segment => Assert.InRange(segment.Bytes.Length, 1, 1024));

        var directory = Assert.IsType<XmpDirectory>(Assert.Single(new XmpReader().ReadJpegSegments(segments)));

        Assert.False(directory.HasError);
        Assert.Empty(directory.Errors);
        var properties = directory.GetXmpProperties();
        Assert.Equal(2, properties.Count);
        Assert.Equal("17", properties["tiff:ImageWidth"]);
        Assert.Equal("23", properties["tiff:ImageLength"]);
        Assert.Equal(2, directory.GetInt32(XmpDirectory.TagXmpValueCount));
    }

    private static JpegSegment ExtensionSegment(byte[] xml, uint fullLength, uint offset)
    {
        const string guid = "0123456789ABCDEF0123456789ABCDEF";
        int preambleLength = XmpReader.JpegSegmentPreambleExtension.Length;
        var bytes = new byte[preambleLength + 32 + 8 + xml.Length];
        XmpReader.JpegSegmentPreambleExtension.CopyTo(bytes);
        Encoding.ASCII.GetBytes(guid).CopyTo(bytes, preambleLength);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(preambleLength + 32), fullLength);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(preambleLength + 36), offset);
        xml.CopyTo(bytes, preambleLength + 40);
        return new JpegSegment(JpegSegmentType.App1, bytes, offset: 0);
    }
}
