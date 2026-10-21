// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.FileType;
using static MetadataExtractor.Formats.Mkv.EbmlTestUtil;
using DetectedFileType = MetadataExtractor.Util.FileType;

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvFileTypeTest
{
    [Theory]
    [InlineData("webm", DetectedFileType.WebM)]
    [InlineData("matroska", DetectedFileType.Mkv)]
    [InlineData("other", DetectedFileType.Ebml)]
    public void DetectsDocumentTypeAndRestoresNonzeroPosition(string docType, DetectedFileType expected)
    {
        var bytes = Concat(new byte[7], Header(docType), Element(Id.Segment));
        using var stream = new MemoryStream(bytes);
        stream.Position = 7;

        Assert.Equal(expected, FileTypeDetector.DetectFileType(stream));

        Assert.Equal(7, stream.Position);
        Assert.Equal(0x1A, stream.ReadByte());
    }

    [Fact]
    public void DocTypeTakesPrecedenceOverConflictingFilename()
    {
        using var stream = new MemoryStream(Concat(Header("webm"), Element(Id.Segment)));

        Assert.Equal(DetectedFileType.WebM, FileTypeDetector.DetectFileType(stream, "sample.mkv"));
        Assert.Equal(0, stream.Position);
        Assert.Equal(0x1A, stream.ReadByte());
    }

    [Theory]
    [InlineData("webm", "WebM", "webm", "video/webm")]
    [InlineData("matroska", "MKV", "mkv", "video/x-matroska")]
    public void ImageMetadataReaderDispatchesAndReportsContainerType(string docType, string name, string extension, string mime)
    {
        using var stream = new MemoryStream(Concat(Header(docType), Element(Id.Segment, Element(Id.Tracks, Video(1)))));

        var metadata = ImageMetadataReader.ReadMetadata(stream);

        var type = Assert.Single(metadata.OfType<FileTypeDirectory>());
        Assert.Equal(name, type.GetString(FileTypeDirectory.TagDetectedFileTypeName));
        Assert.Equal(extension, type.GetString(FileTypeDirectory.TagExpectedFileNameExtension));
        Assert.Equal(mime, type.GetString(FileTypeDirectory.TagDetectedFileMimeType));
        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        NoErrors(metadata);
    }

    [Fact]
    public void AudioOnlyWebmStillUsesDeclaredContainerMime()
    {
        using var stream = new MemoryStream(Concat(Header("webm"), Element(Id.Segment, Element(Id.Tracks, Audio(1)))));

        var metadata = ImageMetadataReader.ReadMetadata(stream);

        Assert.Equal("video/webm", Assert.Single(metadata.OfType<FileTypeDirectory>()).GetString(FileTypeDirectory.TagDetectedFileMimeType));
        Assert.Equal(8000d, Assert.Single(metadata.OfType<MkvAudioDirectory>()).GetDouble(MkvAudioDirectory.TagSamplingFrequency));
        Assert.Empty(metadata.OfType<MkvVideoDirectory>());
        NoErrors(metadata);
    }

    [Fact]
    public void DetectionIsBoundedButMetadataReaderStillHandlesLargeHeaders()
    {
        var bytes = Concat(Element(Id.Header,
                Element(Id.Void, new byte[131072]), Text(Id.DocType, "webm")),
            Element(Id.Segment, Element(Id.Tracks, Video(1))));
        using var stream = new CountingStream(bytes);

        Assert.Equal(DetectedFileType.Ebml, FileTypeDetector.DetectFileType(stream));
        Assert.InRange(stream.BytesRead, 4, 65536 + 1024);
        Assert.InRange(stream.HighestPosition, 4, 65536);
        Assert.Equal(0, stream.Position);

        var metadata = ImageMetadataReader.ReadMetadata(stream);

        Assert.Equal("EBML", Assert.Single(metadata.OfType<FileTypeDirectory>()).GetString(FileTypeDirectory.TagDetectedFileTypeName));
        Assert.Equal("webm", Assert.Single(metadata.OfType<EbmlDirectory>()).GetString(EbmlDirectory.TagDocType));
        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        NoErrors(metadata);
    }

    [Fact]
    public void DetectsDocTypeBeyondNormalPrefixAndConsumesAsciiPadding()
    {
        var bytes = Concat(Element(Id.Header, Element(Id.Void, new byte[4096]),
            Text(Id.DocType, "webm\0\0"), Unsigned(Id.DocTypeVersion, 4),
            Unsigned(Id.DocTypeReadVersion, 2)), Element(Id.Segment));
        using var stream = new CountingStream(bytes);

        Assert.Equal(DetectedFileType.WebM, FileTypeDetector.DetectFileType(stream));
        Assert.InRange(stream.HighestPosition, 4096, 65536);
        Assert.Equal(0, stream.Position);

        var metadata = ImageMetadataReader.ReadMetadata(stream);
        var header = Assert.Single(metadata.OfType<EbmlDirectory>());
        Assert.Equal("webm", header.GetString(EbmlDirectory.TagDocType));
        Assert.Equal(4UL, UInt(header, EbmlDirectory.TagDocTypeVersion));
        Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        NoErrors(metadata);
    }

    [Fact]
    public void MalformedEbmlProbeRestoresStream()
    {
        using var stream = new MemoryStream(Concat(Bytes(Id.Header), Size(20), Text(Id.DocType, "webm")));

        Assert.Equal(DetectedFileType.WebM, FileTypeDetector.DetectFileType(stream));
        Assert.Equal(0, stream.Position);

        var metadata = ImageMetadataReader.ReadMetadata(stream);
        Assert.Contains(metadata, directory => directory.HasError);
        Assert.Equal("webm", Assert.Single(metadata.OfType<EbmlDirectory>()).GetString(EbmlDirectory.TagDocType));
    }

    [Fact]
    public void OtherFormatsDoNotPayForEbmlProbe()
    {
        var jpeg = new byte[65536];
        jpeg[0] = 0xFF;
        jpeg[1] = 0xD8;
        using var stream = new CountingStream(jpeg);

        Assert.Equal(DetectedFileType.Jpeg, FileTypeDetector.DetectFileType(stream));
        Assert.InRange(stream.BytesRead, 2, 1023);
        Assert.Equal(0, stream.Position);
    }

    [Fact]
    public void UnsupportedEbmlRemainsGenericAndReportsError()
    {
        using var stream = new MemoryStream(Concat(Header("other"), Element(Id.Segment)));

        var metadata = ImageMetadataReader.ReadMetadata(stream);

        Assert.Equal("EBML", Assert.Single(metadata.OfType<FileTypeDirectory>()).GetString(FileTypeDirectory.TagDetectedFileTypeName));
        Assert.Contains(metadata, directory => directory.HasError);
        Assert.Empty(metadata.OfType<MkvVideoDirectory>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TruncatedSignatureDoesNotMatchEbml(int length)
    {
        using var stream = new MemoryStream(new byte[] { 0x1A, 0x45, 0xDF, 0xA3 }.Take(length).ToArray());

        Assert.Equal(DetectedFileType.Unknown, FileTypeDetector.DetectFileType(stream));
        Assert.Equal(0, stream.Position);
    }

    private sealed class CountingStream(byte[] bytes) : MemoryStream(bytes)
    {
        public int BytesRead { get; private set; }
        public long HighestPosition { get; private set; }
        public override long Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                HighestPosition = Math.Max(HighestPosition, value);
            }
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = base.Read(buffer, offset, count);
            BytesRead += read;
            HighestPosition = Math.Max(HighestPosition, base.Position);
            return read;
        }
        public override int ReadByte()
        {
            int value = base.ReadByte();
            if (value != -1)
                BytesRead++;
            HighestPosition = Math.Max(HighestPosition, base.Position);
            return value;
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            long position = base.Seek(offset, origin);
            HighestPosition = Math.Max(HighestPosition, position);
            return position;
        }
    }
}
