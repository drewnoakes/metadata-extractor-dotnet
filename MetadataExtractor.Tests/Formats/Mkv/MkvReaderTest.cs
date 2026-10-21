// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Ebml;
using static MetadataExtractor.Formats.Mkv.EbmlTestUtil;

namespace MetadataExtractor.Formats.Mkv;

public sealed class MkvReaderTest
{
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PreservesRepeatedTracksAndParentage(int count)
    {
        var tracks = new[] { Video(1) }.Concat(Enumerable.Range(2, count - 1).Select(i => Audio((ulong)i))).ToArray();
        var metadata = Extract(Document(Element(Id.Tracks, tracks)));

        var segment = Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        var video = Assert.Single(metadata.OfType<MkvVideoDirectory>());
        Assert.Equal(32UL, UInt(video, MkvVideoDirectory.TagPixelWidth));
        Assert.Equal(24UL, UInt(video, MkvVideoDirectory.TagPixelHeight));
        Assert.Equal("V_VP8", video.GetString(MkvTrackDirectory.TagCodecId));
        Assert.Same(segment, video.Parent);
        var audio = metadata.OfType<MkvAudioDirectory>().ToArray();
        Assert.Equal(count - 1, audio.Length);
        Assert.Equal(Enumerable.Range(2, count - 1).Select(i => (ulong)i),
            audio.Select(track => UInt(track, MkvTrackDirectory.TagTrackNumber)));
        Assert.All(audio, track =>
        {
            Assert.Equal(8000d, track.GetDouble(MkvAudioDirectory.TagSamplingFrequency));
            Assert.Same(segment, track.Parent);
        });
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(8)]
    public void ReadsUnknownSizedSegmentAndClusterBeforeTracks(int width)
    {
        var bytes = Concat(Header("webm"), Unknown(Id.Segment, width,
            Unknown(Id.Cluster, width, Unsigned(0xE7, 0), Element(Id.Void, new byte[4])),
            Element(Id.Tracks, Video(1), Audio(2))));

        var metadata = Extract(bytes);

        Assert.Equal("webm", Assert.Single(metadata.OfType<EbmlDirectory>()).GetString(EbmlDirectory.TagDocType));
        Assert.Equal(1UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvTrackDirectory.TagTrackNumber));
        Assert.Equal(2UL, UInt(Assert.Single(metadata.OfType<MkvAudioDirectory>()), MkvTrackDirectory.TagTrackNumber));
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NewRootEndsUnknownMasters(bool secondHeader)
    {
        var bytes = Concat(Header(), Unknown(Id.Segment, 8,
                Element(Id.Tracks, Video(1)), Unknown(Id.Cluster, 1, Unsigned(0xE7, 0))),
            secondHeader ? Header("webm") : [],
            Element(Id.Segment, Element(Id.Tracks, Audio(2))));

        var metadata = Extract(bytes);

        var segments = metadata.OfType<MkvSegmentDirectory>().ToArray();
        Assert.Equal(2, segments.Length);
        Assert.Same(segments[0], Assert.Single(metadata.OfType<MkvVideoDirectory>()).Parent);
        Assert.Same(segments[1], Assert.Single(metadata.OfType<MkvAudioDirectory>()).Parent);
        Assert.Equal(secondHeader ? 2 : 1, metadata.OfType<EbmlDirectory>().Count());
        NoErrors(metadata);
    }

    [Fact]
    public void EndsUnknownClusterAtKnownSegmentBoundary()
    {
        var metadata = Extract(Document(Element(Id.Tracks, Video(1)),
            Unknown(Id.Cluster, 1, Unsigned(0xE7, 0))));

        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void UsesDefaultsForAbsentAndEmptyElements(bool empty)
    {
        var metadata = Extract(Document(
            Element(Id.Info, empty ? Element(Id.TimestampScale) : []),
            Element(Id.Tracks, Element(Id.TrackEntry, Unsigned(Id.TrackType, 2),
                empty ? Element(Id.Lacing) : [], empty ? Element(Id.Language) : [],
                Element(Id.Audio, empty ? Element(Id.Channels) : [],
                    empty ? Element(Id.SamplingFrequency) : [])))));

        var audio = Assert.Single(metadata.OfType<MkvAudioDirectory>());
        Assert.Equal(8000d, audio.GetDouble(MkvAudioDirectory.TagSamplingFrequency));
        Assert.Equal(1UL, UInt(audio, MkvAudioDirectory.TagChannels));
        Assert.Equal(1UL, UInt(audio, MkvTrackDirectory.TagFlagLacing));
        Assert.Equal("eng", audio.GetString(MkvTrackDirectory.TagLanguage));
        Assert.Equal(1000000UL, UInt(Assert.Single(metadata.OfType<MkvSegmentDirectory>()), MkvSegmentDirectory.TagTimestampScale));
        NoErrors(metadata);
    }

    [Fact]
    public void PreservesExplicitTrackAndNestedColourValues()
    {
        var metadata = Extract(Document(Element(Id.Tracks, Element(Id.TrackEntry,
            Unsigned(Id.TrackType, 1), Unsigned(Id.Lacing, 0), Text(Id.Language, "fra"),
            Text(Id.LanguageBcp47, "fr-CA"), Unsigned(Id.CodecDelay, 42), Unsigned(Id.SeekPreRoll, 80),
            Element(Id.Video, Unsigned(Id.FlagInterlaced, 2),
                Element(Id.Colour, Unsigned(Id.MatrixCoefficients, 1),
                    Unsigned(Id.TransferCharacteristics, 16), Unsigned(Id.Primaries, 9),
                    Unsigned(Id.Range, 1), Unsigned(Id.ChromaSitingHorz, 1), Unsigned(Id.ChromaSitingVert, 2)))))));

        var video = Assert.Single(metadata.OfType<MkvVideoDirectory>());
        Assert.Equal(0UL, UInt(video, MkvTrackDirectory.TagFlagLacing));
        Assert.Equal("fra", video.GetString(MkvTrackDirectory.TagLanguage));
        Assert.Equal("fr-CA", video.GetString(MkvTrackDirectory.TagLanguageBcp47));
        Assert.Equal(42UL, UInt(video, MkvTrackDirectory.TagCodecDelay));
        Assert.Equal(80UL, UInt(video, MkvTrackDirectory.TagSeekPreRoll));
        Assert.Equal(2UL, UInt(video, MkvVideoDirectory.TagFlagInterlaced));
        Assert.Equal(1UL, UInt(video, MkvVideoDirectory.TagMatrixCoefficients));
        Assert.Equal(16UL, UInt(video, MkvVideoDirectory.TagTransferCharacteristics));
        Assert.Equal(9UL, UInt(video, MkvVideoDirectory.TagPrimaries));
        Assert.Equal(1UL, UInt(video, MkvVideoDirectory.TagRange));
        Assert.Equal(1UL, UInt(video, MkvVideoDirectory.TagChromaSitingHorz));
        Assert.Equal(2UL, UInt(video, MkvVideoDirectory.TagChromaSitingVert));
        NoErrors(metadata);
    }

    [Fact]
    public void PreservesGenericSubtitleTrackWithoutInventingAudioOrVideoDirectories()
    {
        var metadata = Extract(Document(Element(Id.Tracks, Element(Id.TrackEntry,
            Unsigned(Id.TrackNumber, 3), Unsigned(Id.TrackType, 17),
            Text(Id.CodecId, "S_TEXT/UTF8"), Text(Id.Language, "spa")))));

        var track = Assert.IsType<MkvTrackDirectory>(Assert.Single(metadata.OfType<MkvTrackDirectory>()));
        Assert.Equal(3UL, UInt(track, MkvTrackDirectory.TagTrackNumber));
        Assert.Equal(17UL, UInt(track, MkvTrackDirectory.TagTrackType));
        Assert.Equal("S_TEXT/UTF8", track.GetString(MkvTrackDirectory.TagCodecId));
        Assert.Equal("spa", track.GetString(MkvTrackDirectory.TagLanguage));
        Assert.Empty(metadata.OfType<MkvVideoDirectory>());
        Assert.Empty(metadata.OfType<MkvAudioDirectory>());
        NoErrors(metadata);
    }

    [Fact]
    public void RetainsTrackFieldsFollowingVideoMasterAndAppliesColourDefaults()
    {
        var metadata = Extract(Document(Element(Id.Tracks, Element(Id.TrackEntry,
            Element(Id.Video, Unsigned(Id.PixelWidth, 64),
                Element(Id.Colour, Unsigned(Id.MatrixCoefficients, 1), Element(Id.Primaries))),
            Unsigned(Id.TrackType, 1), Unsigned(Id.TrackUid, 42),
            Unsigned(Id.DefaultDuration, 40000000), Text(Id.CodecId, "V_VP9")))));

        var video = Assert.Single(metadata.OfType<MkvVideoDirectory>());
        Assert.Equal(64UL, UInt(video, MkvVideoDirectory.TagPixelWidth));
        Assert.Equal(42UL, UInt(video, MkvTrackDirectory.TagTrackUid));
        Assert.Equal("V_VP9", video.GetString(MkvTrackDirectory.TagCodecId));
        Assert.Equal(25d, video.GetDouble(MkvVideoDirectory.TagFrameRate));
        Assert.Equal(1UL, UInt(video, MkvVideoDirectory.TagMatrixCoefficients));
        Assert.Equal(2UL, UInt(video, MkvVideoDirectory.TagPrimaries));
        Assert.Equal(2UL, UInt(video, MkvVideoDirectory.TagTransferCharacteristics));
        Assert.Equal(0UL, UInt(video, MkvVideoDirectory.TagRange));
        Assert.Equal(0UL, UInt(video, MkvVideoDirectory.TagChromaSitingHorz));
        Assert.Equal(0UL, UInt(video, MkvVideoDirectory.TagChromaSitingVert));
        Assert.Equal(0UL, UInt(video, MkvVideoDirectory.TagFlagInterlaced));
        Assert.Equal(0UL, UInt(video, MkvVideoDirectory.TagDisplayUnit));
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(0x7FFFFFFFFFFFFFFFUL, "9223372036854775807")]
    [InlineData(0x8000000000000000UL, "9223372036854775808")]
    [InlineData(ulong.MaxValue, "18446744073709551615")]
    public void PreservesUnsignedUidWithoutPrecisionLoss(ulong uid, string expected)
    {
        var metadata = Extract(Document(Element(Id.Tracks, Element(Id.TrackEntry,
            Unsigned(Id.TrackType, 1), Unsigned(Id.TrackUid, uid), Element(Id.Video)))));

        var video = Assert.Single(metadata.OfType<MkvVideoDirectory>());
        Assert.Equal(uid, UInt(video, MkvTrackDirectory.TagTrackUid));
        Assert.Equal(expected, video.GetString(MkvTrackDirectory.TagTrackUid));
        Assert.Equal(expected, video.GetDescription(MkvTrackDirectory.TagTrackUid));
        NoErrors(metadata);
    }

    [Fact]
    public void AcceptsNewerDocumentVersionWithSupportedReadVersion()
    {
        var metadata = Extract(Concat(Element(Id.Header, Text(Id.DocType, "matroska"),
            Unsigned(Id.DocTypeVersion, 100), Unsigned(Id.DocTypeReadVersion, 2)), Element(Id.Segment)));

        Assert.Equal(100UL, UInt(Assert.Single(metadata.OfType<EbmlDirectory>()), EbmlDirectory.TagDocTypeVersion));
        Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(Id.DocTypeReadVersion, 5)]
    [InlineData(Id.DocTypeReadVersion, 0)]
    [InlineData(Id.ReadVersion, 2)]
    [InlineData(Id.MaxIdLength, 8)]
    [InlineData(Id.MaxIdLength, 3)]
    [InlineData(Id.MaxSizeLength, 9)]
    [InlineData(Id.MaxSizeLength, 0)]
    public void RejectsUnsupportedHeaderLimits(int id, int value)
    {
        var metadata = Extract(Concat(Element(Id.Header, Text(Id.DocType, "matroska"),
            Unsigned(id, (ulong)value)), Element(Id.Segment)));

        Assert.Contains(metadata, directory => directory.HasError);
        Assert.Empty(metadata.OfType<MkvSegmentDirectory>());
    }

    [Theory]
    [InlineData(Id.DocTypeVersion)]
    [InlineData(Id.Version)]
    public void RejectsVersionLowerThanReadVersion(int versionId)
    {
        var metadata = Extract(Concat(Element(Id.Header, Text(Id.DocType, "matroska"),
            Unsigned(versionId, 0)), Element(Id.Segment)));

        Assert.Contains(metadata, directory => directory.HasError);
        Assert.Empty(metadata.OfType<MkvSegmentDirectory>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RejectsUnsupportedDocTypeAndMissingHeader(bool missingHeader)
    {
        var segment = Element(Id.Segment, Element(Id.Tracks, Video(1)));
        var metadata = Extract(missingHeader ? segment : Concat(Header("other"), segment));

        Assert.Contains(metadata, directory => directory.HasError);
        Assert.Empty(metadata.OfType<MkvVideoDirectory>());
        Assert.Empty(metadata.OfType<MkvSegmentDirectory>());
    }

    [Fact]
    public void EnforcesDeclaredSizeWidthInBody()
    {
        var metadata = Extract(Concat(Element(Id.Header,
            Text(Id.DocType, "matroska"), Unsigned(Id.MaxSizeLength, 1)),
            Bytes(Id.Segment), Size(0, 2)));

        Assert.Contains(metadata, directory => directory.HasError);
    }

    [Fact]
    public void RejectsChildExceedingParentBoundary()
    {
        var metadata = Extract(Document(Element(Id.Info,
            Concat(Bytes(Id.MuxingApp), Size(100), new byte[1]))));

        Assert.Contains(metadata, directory => directory.HasError);
        Assert.Single(metadata.OfType<MkvSegmentDirectory>());
    }

    [Fact]
    public void RejectsUnknownSizedScalar()
    {
        var metadata = Extract(Document(Element(Id.Info, Unknown(Id.MuxingApp, 1))));

        Assert.Contains(metadata, directory => directory.HasError);
        Assert.Single(metadata.OfType<MkvSegmentDirectory>());
    }

    [Theory]
    [InlineData(0xFF)]
    [InlineData(0x7FFF)]
    [InlineData(0x3FFFFF)]
    [InlineData(0x1FFFFFFF)]
    public void RejectsReservedElementIdentifiers(int id)
    {
        var metadata = Extract(Document(Element(id)));

        Assert.Contains(metadata, directory => directory.HasError);
    }

    [Fact]
    public void AcceptsChapterDisplayIdentifier()
    {
        // 0x80 is a valid identifier, unlike all-ones identifiers of each width.
        var metadata = Extract(Document(Element(0x80), Element(Id.Tracks, Video(1))));

        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ReportsMalformedHeadersWithoutDiscardingEarlierMetadata(int kind)
    {
        byte[][] malformed =
        [
            [0x1A],                         // Truncated four-byte ID.
            [0],                            // Invalid VINT marker.
            [8, 0, 0, 0, 1, 0x80],         // Five-byte ID.
            [0xEC, 0x40]                    // Truncated size VINT.
        ];
        var metadata = Extract(Concat(Document(Element(Id.Tracks, Video(1))), malformed[kind]));

        Assert.Contains(metadata, directory => directory.HasError);
        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void ReadsEverySizeVintWidth(int width)
    {
        var tracks = Element(Id.Tracks, Video(1));
        var metadata = Extract(Concat(Header(), Bytes(Id.Segment), Size((ulong)tracks.Length, width), tracks));

        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ReadsFloatingPointWidths(bool singlePrecision)
    {
        var metadata = Extract(Document(Element(Id.Info, Floating(Id.Duration, 1.25, singlePrecision))));

        var segment = Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        Assert.Equal(1.25, segment.GetDouble(MkvSegmentDirectory.TagDuration));
        Assert.Equal(0.00125, segment.GetDouble(MkvSegmentDirectory.TagDurationSeconds), 10);
        NoErrors(metadata);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RejectsInvalidScalarWidth(bool floatingPoint)
    {
        var malformed = floatingPoint
            ? Element(Id.Info, Element(Id.Duration, new byte[3]))
            : Element(Id.Tracks, Element(Id.TrackEntry, Element(Id.TrackUid, new byte[9])));
        var metadata = Extract(Document(malformed));

        Assert.Contains(metadata, directory => directory.HasError);
    }

    [Fact]
    public void ReadsEmptyNonDefaultedIntegerAndFloatAsZero()
    {
        var metadata = Extract(Document(Element(Id.Info, Element(Id.Duration)),
            Element(Id.Tracks, Element(Id.TrackEntry, Unsigned(Id.TrackType, 1),
                Element(Id.TrackUid), Element(Id.Video)))));

        var segment = Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        Assert.Equal(0d, segment.GetDouble(MkvSegmentDirectory.TagDuration));
        Assert.Equal(0d, segment.GetDouble(MkvSegmentDirectory.TagDurationSeconds));
        Assert.Equal(0UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvTrackDirectory.TagTrackUid));
        NoErrors(metadata);
    }

    [Fact]
    public void RejectsBinaryScalarTooLargeToAllocate()
    {
        const ulong ScalarSize = (ulong)int.MaxValue + 1;
        var scalarHeader = Concat(Bytes(Id.SegmentUuid), Size(ScalarSize));
        var bytes = Concat(Header(), Unknown(Id.Segment, 8),
            Bytes(Id.Info), Size(ScalarSize + (ulong)scalarHeader.Length), scalarHeader);

        var metadata = Extract(bytes);

        Assert.Contains(metadata.SelectMany(directory => directory.Errors),
            error => error.Contains("too large"));
        var segment = Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        Assert.False(segment.ContainsTag(MkvSegmentDirectory.TagSegmentUuid));
    }

    [Fact]
    public void ReportsDuplicateScalarButRetainsLastValue()
    {
        var metadata = Extract(Document(Element(Id.Info,
            Text(Id.MuxingApp, "first"), Text(Id.MuxingApp, "second"))));

        var segment = Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        Assert.True(segment.HasError);
        Assert.Equal("second", segment.GetString(MkvSegmentDirectory.TagMuxingApp));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(100)]
    public void DetectsSkipPastEofAndRetainsEarlierMetadata(int actualSize)
    {
        var payload = Concat(Element(Id.Info, Text(Id.MuxingApp, "mux")),
            Element(Id.Tracks, Video(1)), Bytes(Id.Cluster), Size(100), new byte[actualSize]);
        var bytes = Concat(Header(), Bytes(Id.Segment), Size((ulong)(payload.Length + 100 - actualSize)), payload);
        // MemoryStream.Seek, like FileStream.Seek, permits positions beyond EOF.
        using var stream = new MemoryStream(bytes);
        var metadata = EbmlMetadataReader.ReadMetadata(stream);

        Assert.Equal(actualSize != 100, metadata.Any(directory => directory.HasError));
        Assert.Equal("mux", Assert.Single(metadata.OfType<MkvSegmentDirectory>()).GetString(MkvSegmentDirectory.TagMuxingApp));
        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        Assert.True(stream.Position <= stream.Length);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(100)]
    public void VerifiesSkippedPayloadEvenWhenReaderClaimsSeekPastEofSucceeded(int actualSize)
    {
        var payload = Concat(Element(Id.Tracks, Video(1)), Bytes(Id.Cluster), Size(100), new byte[actualSize]);
        var bytes = Concat(Header(), Bytes(Id.Segment), Size((ulong)(payload.Length + 100 - actualSize)), payload);
        using var stream = new MemoryStream(bytes);
        var reader = new PermissiveSkipReader(stream);

        var metadata = new MkvReader().Extract(reader);

        Assert.Equal(actualSize != 100, metadata.Any(directory => directory.HasError));
        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        if (actualSize == 100)
            Assert.Equal(bytes.Length, reader.Position);
    }

    [Fact]
    public void ReportsTruncatedElementLargerThanFourGiBWithByteArrayReader()
    {
        // A five-byte size VINT for 4 GiB + 2. Truncating TrySkip's long argument to
        // int would skip one byte, consume the other and incorrectly report clean EOF.
        const ulong DeclaredSize = 0x100000002UL;
        var bytes = Concat(Header(), Unknown(Id.Segment, 8),
            Element(Id.Info, Text(Id.MuxingApp, "mux")),
            Bytes(Id.Cluster), Size(DeclaredSize, width: 5), new byte[2]);
        var reader = new SequentialByteArrayReader(bytes);

        var metadata = new MkvReader().Extract(reader);

        var header = Assert.Single(metadata.OfType<EbmlDirectory>());
        Assert.Equal("matroska", header.GetString(EbmlDirectory.TagDocType));
        Assert.False(header.HasError);
        var segment = Assert.Single(metadata.OfType<MkvSegmentDirectory>());
        Assert.Same(header, segment.Parent);
        Assert.Equal("mux", segment.GetString(MkvSegmentDirectory.TagMuxingApp));
        Assert.True(segment.HasError);
        Assert.Contains(segment.Errors, error => error.Contains("End of data reached."));
        Assert.Equal(bytes.Length, reader.Position);
    }

    [Fact]
    public void ReadsNonSeekableShortReadsAndSkipsOpaquePayload()
    {
        var bytes = Document(Element(Id.Cluster, new byte[32]), Element(Id.Tracks, Video(1), Audio(2)));
        using var stream = new NonSeekableStream(bytes, chunkSize: 1);

        var metadata = EbmlMetadataReader.ReadMetadata(stream);

        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        Assert.Equal(2UL, UInt(Assert.Single(metadata.OfType<MkvAudioDirectory>()), MkvTrackDirectory.TagTrackNumber));
        Assert.Equal(bytes.Length, stream.BytesRead);
        NoErrors(metadata);
    }

    [Fact]
    public void FallsBackToReadingWhenReaderCannotSkip()
    {
        var bytes = Document(Element(Id.Cluster, new byte[32]), Element(Id.Tracks, Video(1)));
        using var stream = new MemoryStream(bytes);
        var reader = new NoSkipReader(stream);

        var metadata = new MkvReader().Extract(reader);

        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        Assert.Equal(bytes.Length, reader.Position);
        NoErrors(metadata);
    }

    [Fact]
    public void ReportsIoFailureAtElementBoundaryRatherThanTreatingItAsCleanEof()
    {
        using var stream = new FailingAtEndStream(Document(Element(Id.Tracks, Video(1))));

        var metadata = EbmlMetadataReader.ReadMetadata(stream);

        Assert.Contains(metadata.SelectMany(directory => directory.Errors), error => error.Contains("Synthetic I/O failure"));
        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
    }

    [Fact]
    public void SkipsClusterLargerThanFourGiBWithoutBuffering()
    {
        const long PaddingSize = 5L * 1024 * 1024 * 1024 + 17;
        var tracks = Element(Id.Tracks, Video(1));
        var clusterHeader = Concat(Bytes(Id.Cluster), Size(PaddingSize));
        var prefix = Concat(Header(), Bytes(Id.Segment),
            Size((ulong)(clusterHeader.Length + PaddingSize + tracks.Length)), clusterHeader);
        using var stream = new SparseStream(prefix, PaddingSize, tracks);
        var reader = new SequentialStreamReader(stream);

        var metadata = new MkvReader().Extract(reader);

        Assert.Equal(32UL, UInt(Assert.Single(metadata.OfType<MkvVideoDirectory>()), MkvVideoDirectory.TagPixelWidth));
        Assert.Equal(prefix.Length + PaddingSize + tracks.Length, reader.Position);
        Assert.True(stream.BytesRead < 1024, "The opaque cluster must be skipped, not buffered or read.");
        NoErrors(metadata);
    }

    private sealed class SparseStream(byte[] prefix, long padding, byte[] suffix) : Stream
    {
        public long BytesRead { get; private set; }
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => prefix.Length + padding + suffix.Length;
        public override long Position { get; set; }
        public override int ReadByte()
        {
            if (Position >= Length)
                return -1;
            BytesRead++;
            long position = Position++;
            return position < prefix.Length ? prefix[(int)position]
                : position < prefix.Length + padding ? 0 : suffix[(int)(position - prefix.Length - padding)];
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            Assert.InRange(count, 0, 4096);
            int read = 0;
            while (read < count && Position < Length)
                buffer[offset + read++] = (byte)ReadByte();
            return read;
        }
        public override long Seek(long offset, SeekOrigin origin) =>
            Position = (origin == SeekOrigin.Begin ? 0 : origin == SeekOrigin.Current ? Position : Length) + offset;
        public override void Flush() => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    private sealed class NoSkipReader(Stream stream) : SequentialStreamReader(stream)
    {
        public override bool TrySkip(long n) => false;
    }

    private sealed class PermissiveSkipReader : SequentialStreamReader
    {
        private readonly Stream _stream;

        public PermissiveSkipReader(Stream stream) : base(stream) => _stream = stream;

        public override void Skip(long n) => _stream.Seek(n, SeekOrigin.Current);
    }

    private sealed class FailingAtEndStream(byte[] bytes) : MemoryStream(bytes)
    {
        public override int ReadByte() => Position == Length
            ? throw new IOException("Synthetic I/O failure")
            : base.ReadByte();
    }
}
