// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Buffers.Binary;
using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Formats.Icc;

/// <summary>Unit tests for <see cref="IccReader"/>.</summary>
/// <author>Drew Noakes https://drewnoakes.com</author>
public sealed class IccReaderTest
{
    // TODO add a test with well-formed ICC data and assert output values are correct

    [Fact]
    public void Extract_InvalidData()
    {
        var app2Bytes = TestDataUtil.GetBytes("Data/iccDataInvalid1.jpg.app2");

        // When in an APP2 segment, ICC data starts after a 14-byte preamble
        var icc = TestHelper.SkipBytes(app2Bytes, 14);
        var directory = new IccReader().Extract(new ByteArrayReader(icc));
        Assert.NotNull(directory);
        Assert.True(directory.HasError);
    }

    [Fact]
    public void ReadJpegSegments_InvalidData()
    {
        var app2 = new JpegSegment(JpegSegmentType.App2, TestDataUtil.GetBytes("Data/iccDataInvalid1.jpg.app2"), offset: 0);
        var directory = new IccReader().ReadJpegSegments([app2]);
        Assert.NotNull(directory);
        Assert.True(directory.Single().HasError);
    }

    [Fact]
    public void GetStringFromUInt32()
    {
        Assert.Equal("ABCD", IccReader.GetStringFromUInt32(0x41424344u));
    }

    [Fact]
    public void Extract_ProfileDateTime()
    {
        var app2 = new JpegSegment(JpegSegmentType.App2, TestDataUtil.GetBytes("Data/withExifAndIptc.jpg.app2"), offset: 0);

        var directory = new IccReader()
            .ReadJpegSegments(new[] { app2 })
            .OfType<IccDirectory>()
            .Single();

        Assert.NotNull(directory);
        //Assert.Equal("1998:02:09 06:49:00", directory.GetString(IccDirectory.TagProfileDateTime));
        Assert.Equal(new DateTime(1998, 2, 9, 6, 49, 0), directory.GetDateTime(IccDirectory.TagProfileDateTime));
    }

    [Fact]
    public void Extract_MinimumProfileWithEmptyTable()
    {
        var reader = new GuardedReader(CreateProfile(132), 132);

        var directory = new IccReader().Extract(reader);

        Assert.False(directory.HasError);
        Assert.Equal(132, directory.GetInt32(IccDirectory.TagProfileByteCount));
        Assert.Equal(0, directory.GetInt32(IccDirectory.TagTagCount));
        Assert.Equal(new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc), directory.GetDateTime(IccDirectory.TagProfileDateTime));
        Assert.Empty(reader.RejectedReads);
    }

    [Fact]
    public void Extract_MaximumDeclaredSizeProbesLastByteWithoutReadingLengthOrAllocatingWholeProfile()
    {
        const int maxSize = 16 * 1024 * 1024;
        var profile = CreateProfile(132);
        WriteInt32(profile, 0, maxSize);
        // A sparse logical profile: only the tiny header and terminal byte are
        // accessible. No 16 MiB buffer or stream traversal is needed.
        var reader = new GuardedReader(profile, allowedEnd: 132, virtualLastByte: maxSize - 1);

        var directory = new IccReader().Extract(reader);

        Assert.False(directory.HasError);
        Assert.Equal(maxSize, directory.GetInt32(IccDirectory.TagProfileByteCount));
        Assert.Equal(0, directory.GetInt32(IccDirectory.TagTagCount));
        Assert.Contains((maxSize - 1, 1), reader.Reads);
        Assert.Empty(reader.RejectedReads);
    }

    [Fact]
    public void Extract_TruncatedDeclaredProfileReportsErrorEvenWithNoTags()
    {
        var profile = CreateProfile(132);
        WriteInt32(profile, 0, 148);
        var reader = new GuardedReader(profile, allowedEnd: 148);

        var directory = new IccReader().Extract(reader);

        Assert.True(directory.HasError);
        Assert.NotEmpty(directory.Errors);
        Assert.Equal(0, directory.GetInt32(IccDirectory.TagTagCount));
        Assert.Contains((147, 1), reader.Reads);
        Assert.Empty(reader.RejectedReads);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(131)]
    [InlineData(16 * 1024 * 1024 + 1)]
    [InlineData(int.MaxValue)]
    public void Extract_InvalidDeclaredSizeDoesNotReadPastSizeField(int declaredSize)
    {
        var profile = CreateProfile(132);
        WriteInt32(profile, 0, declaredSize);
        var reader = new GuardedReader(profile, allowedEnd: 4);

        var directory = new IccReader().Extract(reader);

        Assert.True(directory.HasError);
        Assert.NotEmpty(directory.Errors);
        Assert.Empty(reader.RejectedReads);
        Assert.All(reader.Reads, read => Assert.InRange(read.Index + (long)read.Count, 0L, 4L));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void Extract_TagCountMustFitDeclaredProfileBeforeReadingEntries(int count)
    {
        // Backing bytes contain an entry, but the declared profile ends before it.
        var profile = CreateProfile(144);
        WriteInt32(profile, 0, 132);
        WriteInt32(profile, 128, count);
        var reader = new GuardedReader(profile, allowedEnd: 132);

        var directory = new IccReader().Extract(reader);

        Assert.True(directory.HasError);
        Assert.NotEmpty(directory.Errors);
        Assert.Empty(reader.RejectedReads);
        Assert.False(directory.ContainsTag(IccDirectory.TagTagDesc));
    }

    [Theory]
    [InlineData(-1, 4)]
    [InlineData(144, -1)]
    [InlineData(149, 0)]
    [InlineData(148, 1)]
    [InlineData(145, 4)]
    [InlineData(int.MaxValue, 4)]
    [InlineData(144, int.MaxValue)]
    public void Extract_InvalidTagRangeDoesNotAttemptPayloadRead(int pointer, int length)
    {
        var profile = CreateProfile(160, (IccDirectory.TagTagDesc, pointer, length));
        WriteInt32(profile, 0, 148);
        var reader = new GuardedReader(profile, allowedEnd: 144);

        var directory = new IccReader().Extract(reader);

        Assert.True(directory.HasError);
        Assert.NotEmpty(directory.Errors);
        Assert.False(directory.ContainsTag(IccDirectory.TagTagDesc));
        // The guard is checked by IndexedReader before allocating a GetBytes result.
        // A guard exception must NOT make this test pass.
        Assert.Empty(reader.RejectedReads);
    }

    [Theory]
    [InlineData(144, 4)]
    [InlineData(148, 0)]
    public void Extract_TagRangeMayEndExactlyAtDeclaredBoundary(int pointer, int length)
    {
        var profile = CreateProfile(148, (IccDirectory.TagTagDesc, pointer, length));
        new byte[] { 11, 22, 33, 44 }.CopyTo(profile, 144);
        var reader = new GuardedReader(profile, 148);

        var directory = new IccReader().Extract(reader);

        Assert.False(directory.HasError);
        Assert.Equal(length == 0 ? Array.Empty<byte>() : new byte[] { 11, 22, 33, 44 },
            Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagDesc)));
        Assert.Equal(1, directory.GetInt32(IccDirectory.TagTagCount));
        Assert.Empty(reader.RejectedReads);
    }

    [Fact]
    public void Extract_IdenticalTagRangesShareOneByteArray()
    {
        var profile = CreateProfile(164,
            (IccDirectory.TagTagDesc, 156, 8),
            (IccDirectory.TagTagCprt, 156, 8));
        byte[] payload = [1, 2, 3, 4, 5, 6, 7, 8];
        payload.CopyTo(profile, 156);
        var reader = new GuardedReader(profile, 164);

        var directory = new IccReader().Extract(reader);

        Assert.False(directory.HasError);
        var first = Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagDesc));
        var second = Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagCprt));
        Assert.Equal(payload, first);
        Assert.Same(first, second);
        Assert.Single(reader.Reads, read => read.Index == 156 && read.Count == 8);
        Assert.Equal(2, directory.GetInt32(IccDirectory.TagTagCount));
        Assert.Empty(reader.RejectedReads);
    }

    [Fact]
    public void Extract_OverlappingButDifferentRangesAreNotShared()
    {
        var profile = CreateProfile(164,
            (IccDirectory.TagTagDesc, 156, 4),
            (IccDirectory.TagTagCprt, 158, 4));
        new byte[] { 1, 2, 3, 4, 5, 6 }.CopyTo(profile, 156);

        var directory = new IccReader().Extract(new ByteArrayReader(profile));

        Assert.False(directory.HasError);
        var first = Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagDesc));
        var second = Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagCprt));
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, first);
        Assert.Equal(new byte[] { 3, 4, 5, 6 }, second);
        Assert.NotSame(first, second);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Extract_UniqueCopyBudgetIsDeclaredProfileLength(bool exceedsBudget)
    {
        // Each range is individually valid. Their overlap does not reduce the
        // number of bytes copied: only EXACT duplicates may share an allocation.
        var profile = CreateProfile(168,
            (IccDirectory.TagTagDesc, 0, 56),
            (IccDirectory.TagTagCprt, 4, 56),
            (IccDirectory.TagTagWtpt, 8, exceedsBudget ? 57 : 56));
        var reader = new GuardedReader(profile, 168);

        var directory = new IccReader().Extract(reader);

        Assert.Equal(exceedsBudget, directory.HasError);
        Assert.Empty(reader.RejectedReads);
        Assert.InRange(reader.Reads.Where(read => read.Count >= 56).Sum(read => read.Count), 0, 168);
        if (!exceedsBudget)
        {
            Assert.Equal(profile.AsSpan(0, 56).ToArray(), Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagDesc)));
            Assert.Equal(profile.AsSpan(4, 56).ToArray(), Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagCprt)));
            Assert.Equal(profile.AsSpan(8, 56).ToArray(), Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagWtpt)));
        }
        else
        {
            Assert.NotEmpty(directory.Errors);
            Assert.False(directory.ContainsTag(IccDirectory.TagTagWtpt));
        }
    }

    [Fact]
    public void Extract_DuplicateRangesDoNotConsumeUniqueCopyBudget()
    {
        var profile = CreateProfile(168,
            (IccDirectory.TagTagDesc, 0, 80),
            (IccDirectory.TagTagCprt, 0, 80),
            (IccDirectory.TagTagWtpt, 0, 80));

        var directory = new IccReader().Extract(new ByteArrayReader(profile));

        Assert.False(directory.HasError);
        var payload = Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagDesc));
        Assert.Equal(profile.AsSpan(0, 80).ToArray(), payload);
        Assert.Same(payload, directory.GetObject(IccDirectory.TagTagCprt));
        Assert.Same(payload, directory.GetObject(IccDirectory.TagTagWtpt));
        Assert.Equal(3, directory.GetInt32(IccDirectory.TagTagCount));
    }

    [Theory]
    [InlineData(4096, 0, false)]
    [InlineData(4096, 1, false)]
    [InlineData(4096, 0, true)]
    [InlineData(4096, 1, true)]
    [InlineData(4097, 0, false)]
    [InlineData(4097, 1, false)]
    [InlineData(4097, 0, true)]
    [InlineData(4097, 1, true)]
    public void Extract_TagCountBoundsCacheAndDirectoryOverhead(int count, int payloadLength, bool distinctSignatures)
    {
        const int maxTagCount = 4096;
        var payloadOffset = 132 + count * 12;
        var tags = Enumerable.Range(0, count)
            .Select(i => (IccDirectory.TagTagDesc + (distinctSignatures ? i : 0), payloadOffset + i, payloadLength))
            .ToArray();
        var profile = CreateProfile(payloadOffset + count, tags);
        var exceedsLimit = count > maxTagCount;
        var reader = new GuardedReader(profile, allowedEnd: exceedsLimit ? 132 : profile.Length);

        var directory = new IccReader().Extract(reader);

        Assert.Equal(exceedsLimit, directory.HasError);
        Assert.Equal(count, directory.GetInt32(IccDirectory.TagTagCount));
        Assert.Empty(reader.RejectedReads);
        if (exceedsLimit)
        {
            Assert.Contains(directory.Errors, error => error.Contains("4096"));
            Assert.False(directory.ContainsTag(IccDirectory.TagTagDesc));
            Assert.All(reader.Reads, read => Assert.InRange(read.Index + (long)read.Count, 0L, 132L));
        }
        else
        {
            Assert.Equal(new byte[payloadLength],
                Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagDesc + (distinctSignatures ? count - 1 : 0))));
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReadJpegSegments_DoesNotTreatPoolPaddingAsProfileBytes(bool split)
    {
        var profile = CreateProfile(148, (IccDirectory.TagTagDesc, 144, 4));
        profile[144] = 0xAB;
        // Only one of the four tag bytes is present. An ArrayPool bucket is larger
        // than 145 bytes, but its padding is not part of this profile.
        var truncated = profile.AsSpan(0, 145).ToArray();
        JpegSegment[] segments = split
            ? [IccSegment(truncated.AsSpan(0, 70).ToArray(), 1, 2), IccSegment(truncated.AsSpan(70).ToArray(), 2, 2)]
            : [IccSegment(truncated, 1, 1)];

        var directory = Assert.IsType<IccDirectory>(Assert.Single(new IccReader().ReadJpegSegments(segments)));

        Assert.True(directory.HasError);
        Assert.NotEmpty(directory.Errors);
        Assert.False(directory.ContainsTag(IccDirectory.TagTagDesc));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReadJpegSegments_ConcatenatesExactValidPayload(bool split)
    {
        var profile = CreateProfile(148, (IccDirectory.TagTagDesc, 144, 4));
        new byte[] { 11, 22, 33, 44 }.CopyTo(profile, 144);
        JpegSegment[] segments = split
            ? [IccSegment(profile.AsSpan(0, 70).ToArray(), 1, 2), IccSegment(profile.AsSpan(70).ToArray(), 2, 2)]
            : [IccSegment(profile, 1, 1)];

        var directory = Assert.IsType<IccDirectory>(Assert.Single(new IccReader().ReadJpegSegments(segments)));

        Assert.False(directory.HasError);
        Assert.Equal(148, directory.GetInt32(IccDirectory.TagProfileByteCount));
        Assert.Equal(new byte[] { 11, 22, 33, 44 }, Assert.IsType<byte[]>(directory.GetObject(IccDirectory.TagTagDesc)));
    }

    [Theory]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    public void ReadJpegSegments_ShortIccPreambleReportsDirectoryError(int length)
    {
        var bytes = new byte[length];
        IccReader.JpegSegmentPreamble.CopyTo(bytes);
        var segment = new JpegSegment(JpegSegmentType.App2, bytes, offset: 0);

        var directory = Assert.IsType<IccDirectory>(Assert.Single(new IccReader().ReadJpegSegments([segment])));

        Assert.True(directory.HasError);
        Assert.NotEmpty(directory.Errors);
        Assert.False(directory.ContainsTag(IccDirectory.TagProfileByteCount));
    }

    private static byte[] CreateProfile(int length, params (int Tag, int Pointer, int Length)[] tags)
    {
        var bytes = new byte[length];
        WriteInt32(bytes, 0, length);
        ushort[] date = [2026, 1, 2, 3, 4, 5];
        for (int i = 0; i < date.Length; i++)
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(24 + i * 2), date[i]);
        WriteInt32(bytes, 128, tags.Length);
        for (int i = 0; i < tags.Length; i++)
        {
            WriteInt32(bytes, 132 + i * 12, tags[i].Tag);
            WriteInt32(bytes, 136 + i * 12, tags[i].Pointer);
            WriteInt32(bytes, 140 + i * 12, tags[i].Length);
        }
        return bytes;
    }

    private static void WriteInt32(byte[] bytes, int offset, int value) =>
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(offset), value);

    private static JpegSegment IccSegment(byte[] payload, byte sequence, byte count)
    {
        var bytes = new byte[14 + payload.Length];
        IccReader.JpegSegmentPreamble.CopyTo(bytes);
        bytes[12] = sequence;
        bytes[13] = count;
        payload.CopyTo(bytes, 14);
        return new JpegSegment(JpegSegmentType.App2, bytes, offset: 0);
    }

    private sealed class GuardedReader : ByteArrayReader
    {
        private readonly int _allowedEnd;
        private readonly int _bufferLength;
        private readonly int? _virtualLastByte;

        public GuardedReader(byte[] bytes, int allowedEnd, int? virtualLastByte = null) : base(bytes)
        {
            _allowedEnd = allowedEnd;
            _bufferLength = bytes.Length;
            _virtualLastByte = virtualLastByte;
        }

        public List<(int Index, int Count)> Reads { get; } = [];
        public List<(int Index, int Count)> RejectedReads { get; } = [];

        public override long Length => throw new InvalidOperationException("Parser must not query reader.Length.");

        public override void GetBytes(int index, Span<byte> destination)
        {
            Reads.Add((index, destination.Length));
            if (index == _virtualLastByte && destination.Length == 1)
            {
                destination[0] = 0;
                return;
            }
            base.GetBytes(index, destination);
        }

        protected override void ValidateIndex(int index, int bytesRequested)
        {
            if (index < 0 || bytesRequested < 0 || index + (long)bytesRequested > _allowedEnd)
            {
                RejectedReads.Add((index, bytesRequested));
                throw new InvalidOperationException("Parser attempted a read outside the permitted synthetic region.");
            }
            // ByteArrayReader.ValidateIndex queries virtual Length; validate
            // against the actual tiny buffer here instead.
            if (index + (long)bytesRequested > _bufferLength)
                throw new BufferBoundsException(index, bytesRequested, _bufferLength);
        }
    }
}
